using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace CanteenProject.Admin
{
    public partial class ManageUsers : BasePage
    {
        // Search properties
        private string UserSearchText
        {
            get { return ViewState["UserSearchText"] as string ?? ""; }
            set { ViewState["UserSearchText"] = value; }
        }

        private string UserRoleFilter
        {
            get { return ViewState["UserRoleFilter"] as string ?? "All"; }
            set { ViewState["UserRoleFilter"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            var users = AppData.Users.AsQueryable();

            // Apply role filter
            if (UserRoleFilter != "All")
            {
                users = users.Where(u => u.Role == UserRoleFilter);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(UserSearchText))
            {
                string searchLower = UserSearchText.ToLower();
                users = users.Where(u =>
                    u.FullName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower) ||
                    u.Role.ToLower().Contains(searchLower) ||
                    u.PermanentID.ToLower().Contains(searchLower));
            }

            var userList = users
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FullName)
                .Select(u => new {
                    u.PermanentID,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.InvitedByAdminCode,
                    u.ProfilePictureUrl
                }).ToList();

            gvUsers.DataSource = userList;
            gvUsers.DataBind();

            lblUserCount.Text = $"Showing {userList.Count} user(s)";
        }

        protected string GetInviterName(string inviteCode)
        {
            if (string.IsNullOrEmpty(inviteCode)) return "System Admin";
            var admin = AppData.Users.FirstOrDefault(u => u.InviteCode == inviteCode);
            return admin != null ? admin.FullName : "Unknown";
        }

        // Search text changed event handler
        protected void txtUserSearch_TextChanged(object sender, EventArgs e)
        {
            UserSearchText = txtUserSearch.Text.Trim();
            LoadUsers();
        }

        // Filter button click event handler
        protected void btnFilterRole_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            UserRoleFilter = btn.CommandArgument;

            // Highlight active button
            btnAllUsers.CssClass = "filter-btn";
            btnStudents.CssClass = "filter-btn";
            btnTeachers.CssClass = "filter-btn";
            btnAdmins.CssClass = "filter-btn";
            btn.CssClass = "filter-btn active";

            LoadUsers();
        }

        // Clear button click event handler
        protected void btnClearUsers_Click(object sender, EventArgs e)
        {
            UserSearchText = "";
            UserRoleFilter = "All";
            txtUserSearch.Text = "";

            // Reset button styles
            btnAllUsers.CssClass = "filter-btn active";
            btnStudents.CssClass = "filter-btn";
            btnTeachers.CssClass = "filter-btn";
            btnAdmins.CssClass = "filter-btn";

            LoadUsers();
        }

        // GridView row command event handler
        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ResetPassword")
            {
                string email = e.CommandArgument.ToString();
                var user = AppData.Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    string tempPwd = GenerateRandomPassword();
                    user.PasswordHash = HashPassword(tempPwd);

                    AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                                   "Admin", "Reset Password", $"Reset password for {user.FullName} ({user.Email})");

                    lblMessage.Text = $"✅ Password reset for {user.FullName}. Temporary password: {tempPwd}";
                    lblMessage.CssClass = "msg-success";
                    lblMessage.Visible = true;
                }
            }
        }

        // Create Admin button click event handler
        protected void btnCreateAdmin_Click(object sender, EventArgs e)
        {
            string name = txtAdminName.Text.Trim();
            string email = txtAdminEmail.Text.Trim();
            string password = txtAdminPassword.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblAdminCreateMessage.Text = "❌ All fields are required.";
                lblAdminCreateMessage.CssClass = "msg-error";
                return;
            }

            if (password.Length < 6)
            {
                lblAdminCreateMessage.Text = "❌ Password must be at least 6 characters.";
                lblAdminCreateMessage.CssClass = "msg-error";
                return;
            }

            if (AppData.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                lblAdminCreateMessage.Text = "❌ An account with that email already exists.";
                lblAdminCreateMessage.CssClass = "msg-error";
                return;
            }

            // Generate invite code for the new Admin
            string inviteCode = GenerateInviteCode();

            // Create new Admin
            var newAdmin = new User
            {
                UserID = AppData.NextUserID,
                PermanentID = GeneratePermanentID(),
                FullName = name,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = "Admin",
                InviteCode = inviteCode,
                InvitedByAdminCode = null,
                ProfilePictureUrl = null
            };

            AppData.Users.Add(newAdmin);
            AppData.NextUserID++;

            AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                           "Admin", "Create Admin", $"Created new Admin: {name} ({email})");

            lblAdminCreateMessage.Text = $"✅ Admin '{name}' created successfully! Invite code: {inviteCode}";
            lblAdminCreateMessage.CssClass = "msg-success";

            // Clear fields
            txtAdminName.Text = "";
            txtAdminEmail.Text = "";
            txtAdminPassword.Text = "";

            // Refresh user list
            LoadUsers();
        }
    }
}   