using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CanteenProject.Admin
{
    public partial class Dashboard : BasePage
    {
        private string ActivitySearchText
        {
            get { return ViewState["ActivitySearchText"] as string ?? ""; }
            set { ViewState["ActivitySearchText"] = value; }
        }

        private string ActivityFilter
        {
            get { return ViewState["ActivityFilter"] as string ?? "All"; }
            set { ViewState["ActivityFilter"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string userEmail = Session["LoggedInUser"].ToString();

                // Set UserEmail for profile picture controls
                

                LoadUserInfo();
                LoadAdminDashboard();
                UpdateNotificationBadge();
                LoadActivityLog();
            }
        }

        private void LoadUserInfo()
        {
            string email = Session["LoggedInUser"].ToString();

            var user = AppData.Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                lblAdminName.Text = user.FullName;
                lblPopupName.Text = user.FullName;
                lblPopupEmail.Text = user.Email;
                lblPopupRole.Text = user.Role;
                lblPopupPermanentID.Text = user.PermanentID;
                lblAdminInviteCode.Text = user.InviteCode ?? "No Code";
            }
        }

        private void LoadAdminDashboard()
        {
            lblTotalUsers.Text = AppData.Users.Count.ToString();
            lblTotalEquipment.Text = AppData.EquipmentList.Count.ToString();
            lblActiveBorrows.Text = AppData.BorrowRecords.Count(b => b.Status == "Borrowed").ToString();
            lblPendingRequests.Text = AppData.BorrowRequests.Count(r => r.Status == "Pending").ToString();
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

        private void LoadActivityLog()
        {
            var logs = AppData.ActivityLogs.AsQueryable();

            if (ActivityFilter != "All")
            {
                logs = logs.Where(l => l.Action.Contains(ActivityFilter));
            }

            if (!string.IsNullOrEmpty(ActivitySearchText))
            {
                string searchLower = ActivitySearchText.ToLower();
                logs = logs.Where(l =>
                    l.ActorName.ToLower().Contains(searchLower) ||
                    l.ActorEmail.ToLower().Contains(searchLower) ||
                    l.Action.ToLower().Contains(searchLower) ||
                    l.Details.ToLower().Contains(searchLower));
            }

            var logList = logs.OrderByDescending(l => l.LogID).ToList();
            gvActivityLog.DataSource = logList;
            gvActivityLog.DataBind();

            lblActivityCount.Text = $"Showing {logList.Count} activity record(s)";
        }

        protected void txtActivitySearch_TextChanged(object sender, EventArgs e)
        {
            ActivitySearchText = txtActivitySearch.Text.Trim();
            LoadActivityLog();
        }

        protected void btnFilterAction_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            ActivityFilter = btn.CommandArgument;

            btnAllActions.CssClass = "filter-btn";
            btnLoginActions.CssClass = "filter-btn";
            btnLogoutActions.CssClass = "filter-btn";
            btnRegisterActions.CssClass = "filter-btn";
            btnBorrowActions.CssClass = "filter-btn";
            btnReturnActions.CssClass = "filter-btn";
            btn.CssClass = "filter-btn active";

            LoadActivityLog();
        }

        protected void btnClearActivity_Click(object sender, EventArgs e)
        {
            ActivitySearchText = "";
            ActivityFilter = "All";
            txtActivitySearch.Text = "";

            btnAllActions.CssClass = "filter-btn active";
            btnLoginActions.CssClass = "filter-btn";
            btnLogoutActions.CssClass = "filter-btn";
            btnRegisterActions.CssClass = "filter-btn";
            btnBorrowActions.CssClass = "filter-btn";
            btnReturnActions.CssClass = "filter-btn";

            LoadActivityLog();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] != null)
            {
                AddActivityLog(Session["LoggedInUser"].ToString(),
                               Session["UserName"]?.ToString() ?? "Admin",
                               Session["UserRole"]?.ToString() ?? "Admin",
                               "Logout", "User logged out.");
            }

            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Account/Login.aspx");
        }
    }
}