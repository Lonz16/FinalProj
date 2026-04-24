using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace CanteenProject.Admin
{
    public partial class Dashboard : BasePage
    {
        private string UnifiedSearchText
        {
            get { return ViewState["UnifiedSearchText"] as string ?? ""; }
            set { ViewState["UnifiedSearchText"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string userEmail = Session["LoggedInUser"].ToString();
              

                var user = GetCurrentUser();
                if (user != null) lblLogoutUserName.Text = $"{user.FullName} ({user.Email})";

                LoadUserInfo();
                LoadAdminDashboard();
                UpdateNotificationBadge();
                LoadActivityLog();
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
            lblTotalUsers.Text = AppData.Users.Count.ToString();
            lblTotalEquipment.Text = AppData.EquipmentList.Count.ToString();
            lblActiveBorrows.Text = AppData.BorrowRecords.Count(b => b.Status == "Borrowed").ToString();
        }

        private void LoadActivityLog()
        {
            var logs = AppData.ActivityLogs.AsQueryable();

            if (!string.IsNullOrEmpty(UnifiedSearchText))
            {
                string searchLower = UnifiedSearchText.ToLower();
                logs = logs.Where(l =>
                    l.ActorName.ToLower().Contains(searchLower) ||
                    l.ActorEmail.ToLower().Contains(searchLower) ||
                    l.Action.ToLower().Contains(searchLower) ||
                    l.Details.ToLower().Contains(searchLower));
            }

            var logList = logs.OrderByDescending(l => l.LogID).ToList();
            gvActivityLog.DataSource = logList;
            gvActivityLog.DataBind();

            lblSearchStats.Text = string.IsNullOrEmpty(UnifiedSearchText)
                ? $"📋 Showing {logList.Count} total activity records"
                : $"🔍 Found {logList.Count} record(s) matching '{UnifiedSearchText}'";
        }

        protected void gvActivityLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && !string.IsNullOrEmpty(UnifiedSearchText))
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    if (cell.Text != null && cell.Text.ToLower().Contains(UnifiedSearchText.ToLower()))
                    {
                        cell.Text = HighlightText(cell.Text, UnifiedSearchText);
                    }
                }
            }
        }

        private string HighlightText(string text, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || string.IsNullOrEmpty(text)) return text;
            int index = text.ToLower().IndexOf(searchTerm.ToLower());
            if (index >= 0)
            {
                return text.Substring(0, index) + "<span class='highlight'>" + text.Substring(index, searchTerm.Length) + "</span>" + text.Substring(index + searchTerm.Length);
            }
            return text;
        }

        protected void txtUnifiedSearch_TextChanged(object sender, EventArgs e)
        {
            UnifiedSearchText = txtUnifiedSearch.Text.Trim();
            LoadActivityLog();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            UnifiedSearchText = "";
            txtUnifiedSearch.Text = "";
            LoadActivityLog();
        }

        private void UpdateNotificationBadge()
        {
            int count = AppData.BorrowRecords.Count(b => b.Status == "Borrowed" && DateTime.TryParse(b.DueDate, out DateTime due) && (due - DateTime.Now).Days <= 3);
            lblNotificationBadge.Text = count > 0 ? count.ToString() : "0";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}