using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace CanteenProject.Admin
{
    public partial class ManageEquipment : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEquipment();
            }
        }

        private void LoadEquipment()
        {
            gvEquipment.DataSource = AppData.EquipmentList;
            gvEquipment.DataBind();
        }

        protected void btnAddEquipment_Click(object sender, EventArgs e)
        {
            string name = txtEquipName.Text.Trim();
            string category = txtCategory.Text.Trim();
            string qtyText = txtQuantity.Text.Trim();
            string description = txtDescription.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(qtyText))
            {
                ShowMessage("❌ Name, Category, and Quantity are required.", "msg-error");
                return;
            }

            if (!int.TryParse(qtyText, out int qty) || qty < 0)
            {
                ShowMessage("❌ Quantity must be a valid non-negative number.", "msg-error");
                return;
            }

            // Handle image upload
            string imageUrl = "";
            if (fileEquipmentImage.HasFile)
            {
                imageUrl = SaveEquipmentImage(fileEquipmentImage);
            }
            else
            {
                imageUrl = "~/Images/equipment/default-equipment.png";
            }

            // Create new equipment
            var newEquip = new Equipment
            {
                EquipmentID = AppData.NextEquipID++,
                Name = name,
                Category = category,
                Quantity = qty,
                Description = description,
                ImageUrl = imageUrl
            };

            AppData.EquipmentList.Add(newEquip);
            AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                           "Admin", "Add Equipment", $"Added '{name}' (Qty: {qty}) with image");

            ShowMessage($"✅ '{name}' added successfully!", "msg-success");

            // Clear form
            txtEquipName.Text = "";
            txtCategory.Text = "";
            txtQuantity.Text = "";
            txtDescription.Text = "";

            LoadEquipment();
        }

        private string SaveEquipmentImage(FileUpload fileUpload)
        {
            try
            {
                // Validate file type
                string extension = Path.GetExtension(fileUpload.FileName).ToLower();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                {
                    throw new Exception("Only JPG, JPEG, PNG, and GIF files are allowed.");
                }

                // Validate file size (max 1MB for equipment images)
                if (fileUpload.PostedFile.ContentLength > 1 * 1024 * 1024)
                {
                    throw new Exception("File size must be less than 1MB.");
                }

                // Create directory if not exists
                string imagesDir = Server.MapPath("~/Images/equipment/");
                if (!Directory.Exists(imagesDir))
                {
                    Directory.CreateDirectory(imagesDir);
                }

                // Generate unique filename
                string fileName = "equip_" + Guid.NewGuid().ToString() + extension;
                string relativePath = "~/Images/equipment/" + fileName;
                string physicalPath = Server.MapPath(relativePath);

                // Save file
                fileUpload.SaveAs(physicalPath);

                return relativePath;
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Image upload failed: {ex.Message}", "msg-error");
                return "~/Images/equipment/default-equipment.png";
            }
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblAddMessage.Text = message;
            lblAddMessage.CssClass = cssClass;
        }

        protected void gvEquipment_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEquipment.EditIndex = e.NewEditIndex;
            LoadEquipment();
        }

        protected void gvEquipment_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEquipment.EditIndex = -1;
            LoadEquipment();
        }

        protected void gvEquipment_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int equipID = Convert.ToInt32(gvEquipment.DataKeys[e.RowIndex].Value);
            var equip = AppData.EquipmentList.FirstOrDefault(eq => eq.EquipmentID == equipID);
            if (equip == null) return;

            var row = gvEquipment.Rows[e.RowIndex];

            // Get values from edit template
            var txtName = (TextBox)row.FindControl("txtEditName");
            var txtCategory = (TextBox)row.FindControl("txtEditCategory");
            var txtQuantity = (TextBox)row.FindControl("txtEditQuantity");
            var txtDescription = (TextBox)row.FindControl("txtEditDescription");
            var fileImage = (FileUpload)row.FindControl("fileEditImage");
            var hdnCurrentImage = (HiddenField)row.FindControl("hdnCurrentImage");

            if (txtName != null) equip.Name = txtName.Text.Trim();
            if (txtCategory != null) equip.Category = txtCategory.Text.Trim();
            if (txtDescription != null) equip.Description = txtDescription.Text.Trim();

            if (txtQuantity != null && int.TryParse(txtQuantity.Text.Trim(), out int newQty) && newQty >= 0)
            {
                equip.Quantity = newQty;
            }

            // Handle image update
            if (fileImage != null && fileImage.HasFile)
            {
                // Delete old image if not default
                if (!string.IsNullOrEmpty(equip.ImageUrl) &&
                    !equip.ImageUrl.Contains("default-equipment.png"))
                {
                    string oldPath = Server.MapPath(equip.ImageUrl);
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }

                equip.ImageUrl = SaveEquipmentImage(fileImage);
            }

            AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                           "Admin", "Edit Equipment", $"Updated EquipmentID {equipID}");

            gvEquipment.EditIndex = -1;
            LoadEquipment();
        }

        protected void gvEquipment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Delete") return;

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int equipID = Convert.ToInt32(gvEquipment.DataKeys[rowIndex].Value);
            var equip = AppData.EquipmentList.FirstOrDefault(eq => eq.EquipmentID == equipID);
            if (equip == null) return;

            // Check if equipment is currently borrowed
            bool isBorrowed = AppData.BorrowRecords.Any(b => b.EquipmentID == equipID && b.Status == "Borrowed");
            if (isBorrowed)
            {
                ShowMessage($"❌ Cannot delete '{equip.Name}' - it is currently borrowed.", "msg-error");
                return;
            }

            // Delete equipment image if not default
            if (!string.IsNullOrEmpty(equip.ImageUrl) &&
                !equip.ImageUrl.Contains("default-equipment.png"))
            {
                string imagePath = Server.MapPath(equip.ImageUrl);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                           "Admin", "Delete Equipment", $"Deleted '{equip.Name}'");

            AppData.EquipmentList.Remove(equip);
            ShowMessage($"✅ '{equip.Name}' deleted successfully.", "msg-success");

            LoadEquipment();
        }
    }
}