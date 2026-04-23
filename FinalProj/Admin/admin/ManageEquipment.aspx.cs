using System;
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

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(qtyText))
            {
                lblAddMessage.Text = "❌ All fields are required.";
                lblAddMessage.CssClass = "msg-error";
                return;
            }

            if (!int.TryParse(qtyText, out int qty) || qty < 0)
            {
                lblAddMessage.Text = "❌ Quantity must be a valid non-negative number.";
                lblAddMessage.CssClass = "msg-error";
                return;
            }

            var newEquip = new Equipment
            {
                EquipmentID = AppData.NextEquipID++,
                Name = name,
                Category = category,
                Quantity = qty
            };

            AppData.EquipmentList.Add(newEquip);
            AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                           "Admin", "Add Equipment", $"Added '{name}' (Qty: {qty})");

            lblAddMessage.Text = $"✅ '{name}' added successfully!";
            lblAddMessage.CssClass = "msg-success";

            txtEquipName.Text = "";
            txtCategory.Text = "";
            txtQuantity.Text = "";

            LoadEquipment();
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
            var txtQty = (TextBox)row.FindControl("txtEditQty");

            if (txtQty != null && int.TryParse(txtQty.Text.Trim(), out int newQty) && newQty >= 0)
            {
                equip.Quantity = newQty;
            }

            AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                           "Admin", "Edit Equipment", $"Updated EquipmentID {equipID} → Qty: {equip.Quantity}");

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
                lblAddMessage.Text = $"❌ Cannot delete '{equip.Name}' - it is currently borrowed.";
                lblAddMessage.CssClass = "msg-error";
                return;
            }

            AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                           "Admin", "Delete Equipment", $"Deleted '{equip.Name}'");

            AppData.EquipmentList.Remove(equip);
            lblAddMessage.Text = $"✅ '{equip.Name}' deleted successfully.";
            lblAddMessage.CssClass = "msg-success";

            LoadEquipment();
        }
    }
}