using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace CanteenProject.Teacher
{
    public partial class Dashboard : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserInfo();
                LoadTeacherDashboard();
                UpdateNotificationBadge();
            }
        }

        private void LoadUserInfo()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                lblTeacherName.Text = user.FullName;
                lblPopupPermanentID.Text = user.PermanentID;
                lblPopupName.Text = user.FullName;
                lblPopupEmail.Text = user.Email;
                lblPopupRole.Text = user.Role;
            }
        }

        private void LoadTeacherDashboard()
        {
            // Pending requests
            var pendingRequests = AppData.BorrowRequests
                .Where(r => r.Status == "Pending").ToList();
            gvPendingRequests.DataSource = pendingRequests;
            gvPendingRequests.DataBind();
            lblPendingCount.Text = pendingRequests.Count.ToString();

            // Active borrows (show last 10)
            gvActiveBorrows.DataSource = AppData.BorrowRecords
                .Where(b => b.Status == "Borrowed")
                .OrderByDescending(b => b.BorrowDate)
                .Take(10)
                .ToList();
            gvActiveBorrows.DataBind();
        }

        private void UpdateNotificationBadge()
        {
            var borrowed = AppData.BorrowRecords.Where(b => b.Status == "Borrowed");
            int count = 0;
            var dueItems = new System.Collections.Generic.List<dynamic>();

            foreach (var item in borrowed)
            {
                if (DateTime.TryParse(item.DueDate, out DateTime dueDate))
                {
                    int daysLeft = (dueDate - DateTime.Now).Days;
                    if (daysLeft <= 3)
                    {
                        count++;
                        dueItems.Add(new { item.StudentName, item.EquipmentName, item.DueDate, DaysLeft = daysLeft });
                    }
                }
            }

            lblNotificationBadge.Text = count > 0 ? count.ToString() : "0";

            // Generate notification HTML
            var html = new System.Text.StringBuilder();
            if (count == 0)
            {
                html.Append("<div class='no-notifications'>✨ No items due soon.</div>");
            }
            else
            {
                html.Append("<h4>⚠️ Items due within 3 days</h4>");
                foreach (var item in dueItems)
                {
                    string css = item.DaysLeft < 0 ? "overdue" : "due-soon";
                    string text = item.DaysLeft < 0 ? "OVERDUE" : $"Due in {item.DaysLeft} day(s)";
                    html.Append($@"
                        <div class='notification-item'>
                            <strong>{item.EquipmentName}</strong><br />
                            Student: {item.StudentName}<br />
                            Due: {item.DueDate} – <span class='{css}'>{text}</span>
                        </div>");
                }
            }
            litNotificationContent.Text = html.ToString();
        }

        protected void gvPendingRequests_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    lblTeacherMessage.Text = $"✅ Approved: '{request.EquipmentName}' for {request.StudentName}.";
                    lblTeacherMessage.CssClass = "msg-success";
                }
                else
                {
                    AddActivityLog(teacherEmail, teacherName, "Teacher", "Approve Failed",
                                   $"Out of stock – {request.EquipmentName}");
                    lblTeacherMessage.Text = $"❌ Cannot approve – '{request.EquipmentName}' is out of stock.";
                    lblTeacherMessage.CssClass = "msg-error";
                }
            }
            else // Deny
            {
                request.Status = "Denied";
                AddActivityLog(teacherEmail, teacherName, "Teacher", "Deny Request",
                               $"Denied {request.StudentName}'s request for '{request.EquipmentName}'");
                lblTeacherMessage.Text = $"❌ Denied request from {request.StudentName}.";
                lblTeacherMessage.CssClass = "msg-error";
            }

            lblTeacherMessage.Visible = true;
            LoadTeacherDashboard();
            UpdateNotificationBadge();
        }

        protected void gvActiveBorrows_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "MarkReturned") return;

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            var activeBorrows = AppData.BorrowRecords.Where(b => b.Status == "Borrowed")
                .OrderByDescending(b => b.BorrowDate).Take(10).ToList();
            if (rowIndex < 0 || rowIndex >= activeBorrows.Count) return;

            var borrow = activeBorrows[rowIndex];
            borrow.Status = "Returned";
            borrow.ReturnDate = DateTime.Now.ToString("yyyy-MM-dd");

            var equip = AppData.EquipmentList.FirstOrDefault(eq => eq.EquipmentID == borrow.EquipmentID);
            if (equip != null) equip.Quantity++;

            string teacherEmail = Session["LoggedInUser"].ToString();
            string teacherName = Session["UserName"].ToString();
            AddActivityLog(teacherEmail, teacherName, "Teacher", "Mark Returned",
                           $"Marked '{borrow.EquipmentName}' returned by {borrow.StudentName}");

            lblTeacherMessage.Text = $"✅ '{borrow.EquipmentName}' marked as returned by {borrow.StudentName}.";
            lblTeacherMessage.CssClass = "msg-success";
            lblTeacherMessage.Visible = true;
            LoadTeacherDashboard();
            UpdateNotificationBadge();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}