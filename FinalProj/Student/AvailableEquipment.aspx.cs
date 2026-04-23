using System;
using System.Linq;

namespace CanteenProject.Student
{
    public partial class AvailableEquipment : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAvailableEquipment();
            }
        }

        private void LoadAvailableEquipment()
        {
            gvAvailableEquipment.DataSource = AppData.EquipmentList
                .Where(eq => eq.Quantity > 0).ToList();
            gvAvailableEquipment.DataBind();
        }

        protected void gvAvailableEquipment_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Borrow") return;

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            var available = AppData.EquipmentList.Where(eq => eq.Quantity > 0).ToList();
            if (rowIndex < 0 || rowIndex >= available.Count) return;

            var equip = available[rowIndex];
            string studentEmail = Session["LoggedInUser"].ToString();
            string studentName = Session["UserName"].ToString();

            // Check for existing pending request
            bool alreadyPending = AppData.BorrowRequests.Any(r =>
                r.StudentEmail == studentEmail &&
                r.EquipmentID == equip.EquipmentID &&
                r.Status == "Pending");

            if (alreadyPending)
            {
                lblMessage.Text = $"⚠️ You already have a pending request for {equip.Name}.";
                lblMessage.CssClass = "msg-error";
                return;
            }

            // Add borrow request
            AppData.BorrowRequests.Add(new BorrowRequest
            {
                RequestID = AppData.NextRequestID++,
                StudentEmail = studentEmail,
                StudentName = studentName,
                EquipmentID = equip.EquipmentID,
                EquipmentName = equip.Name,
                RequestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Status = "Pending"
            });

            AddActivityLog(studentEmail, studentName, "Student", "Borrow Request", $"Requested '{equip.Name}'");

            lblMessage.Text = $"✅ Request for '{equip.Name}' sent to teacher.";
            lblMessage.CssClass = "msg-success";
            LoadAvailableEquipment();
        }
    }
}