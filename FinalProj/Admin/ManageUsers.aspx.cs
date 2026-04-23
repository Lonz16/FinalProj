using System;
using System.Linq;

namespace CanteenProject.Admin
{
    public partial class ManageUsers : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            gvUsers.DataSource = AppData.Users
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FullName)
                .Select(u => new {
                    u.PermanentID,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.InvitedByAdminCode
                }).ToList();
            gvUsers.DataBind();
        }

        protected string GetInviterName(string inviteCode)
        {
            if (string.IsNullOrEmpty(inviteCode)) return "N/A";
            var admin = AppData.Users.FirstOrDefault(u => u.InviteCode == inviteCode);
            return admin != null ? admin.FullName : "Unknown";
        }

        protected void gvUsers_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
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
    }
}