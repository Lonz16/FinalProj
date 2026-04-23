using System;
using System.Linq;

namespace CanteenProject.Teacher
{
    public partial class PendingRequests : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPendingRequests();
            }
        }

        private void LoadPendingRequests()
        {
            gvPendingRequests.DataSource = AppData.BorrowRequests
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.RequestDate)
                .ToList();
            gvPendingRequests.DataBind();
        }

        protected void gvPendingRequests_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Approve" && e.CommandName != "Deny") return;

            int requestID = Convert.ToInt32(e.CommandArgument);
            var request = AppData.BorrowRequests.FirstOrDefault(r => r.RequestID == requestID);
            if (request == null) return;

            string teacherEmail = Session["LoggedInUser"].ToString();
            string teacherName = Session["UserName"].ToString();

            if (e.CommandName == "Approve")
            {
                var equip = AppData.EquipmentList.FirstOrDefault(eq => eq.EquipmentID == request.EquipmentID);
                if (equip != null && equip.Quantity > 0)
                {
                    equip.Quantity--;
                    request.Status = "Approved";
                    AppData.BorrowRecords.Add(new BorrowRecord
                    {
                        BorrowID = AppData.NextBorrowID++,
                        StudentEmail = request.StudentEmail,
                        StudentName = request.StudentName,
                        EquipmentID = request.EquipmentID,
                        EquipmentName = request.EquipmentName,
                        BorrowDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        DueDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"),
                        Status = "Borrowed"
                    });
                    AddActivityLog(teacherEmail, teacherName, "Teacher", "Approve Request",
                                   $"Approved {request.StudentName}'s request for '{request.EquipmentName}'");
                    lblMessage.Text = $"✅ Approved: '{request.EquipmentName}' for {request.StudentName}.";
                    lblMessage.CssClass = "msg-success";
                }
                else
                {
                    AddActivityLog(teacherEmail, teacherName, "Teacher", "Approve Failed",
                                   $"Out of stock – {request.EquipmentName}");
                    lblMessage.Text = $"❌ Cannot approve – '{request.EquipmentName}' is out of stock.";
                    lblMessage.CssClass = "msg-error";
                }
            }
            else // Deny
            {
                request.Status = "Denied";
                AddActivityLog(teacherEmail, teacherName, "Teacher", "Deny Request",
                               $"Denied {request.StudentName}'s request for '{request.EquipmentName}'");
                lblMessage.Text = $"❌ Denied request from {request.StudentName}.";
                lblMessage.CssClass = "msg-error";
            }

            lblMessage.Visible = true;
            LoadPendingRequests();
        }
    }
}