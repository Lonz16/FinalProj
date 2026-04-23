using System;
using System.Linq;

namespace CanteenProject.Admin
{
    public partial class Dashboard : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserInfo();
                LoadAdminDashboard();
                UpdateNotificationBadge();
            }
        }

        private void LoadUserInfo()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                lblAdminName.Text = user.FullName;
                lblAdminInviteCode.Text = user.InviteCode;
                lblPopupPermanentID.Text = user.PermanentID;
                lblPopupName.Text = user.FullName;
                lblPopupEmail.Text = user.Email;
                lblPopupRole.Text = user.Role;
            }
        }

        private void LoadAdminDashboard()
        {
            // Stats
            lblTotalUsers.Text = AppData.Users.Count.ToString();
            lblTotalEquipment.Text = AppData.EquipmentList.Count.ToString();
            lblActiveBorrows.Text = AppData.BorrowRecords.Count(b => b.Status == "Borrowed").ToString();
            lblPendingRequests.Text = AppData.BorrowRequests.Count(r => r.Status == "Pending").ToString();

            // Recent Activity (last 10)
            gvRecentActivity.DataSource = AppData.ActivityLogs
                .OrderByDescending(l => l.LogID)
                .Take(10)
                .ToList();
            gvRecentActivity.DataBind();
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

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}