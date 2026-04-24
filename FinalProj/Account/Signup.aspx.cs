using System;
using System.Linq;

namespace CanteenProject.Account
{
    public partial class Signup : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlInviteCode.Visible = true;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string name = txtSignupName.Text.Trim();
            string email = txtSignupEmail.Text.Trim();
            string password = txtSignupPassword.Text.Trim();
            string role = ddlSignupRole.SelectedValue;
            string inviteCode = txtInviteCode.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowMessage("❌ All fields are required.", "msg-error");
                return;
            }

            if (password.Length < 6)
            {
                ShowMessage("❌ Password must be at least 6 characters.", "msg-error");
                return;
            }

            if (AppData.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                ShowMessage("❌ An account with that email already exists.", "msg-error");
                return;
            }

            // Invite code is required for Students and Teachers
            if (string.IsNullOrEmpty(inviteCode))
            {
                ShowMessage("❌ Invite code is required. Please ask an Admin.", "msg-error");
                return;
            }

            // Verify invite code from an Admin
            bool validCode = AppData.Users.Any(u =>
                u.Role == "Admin" && u.InviteCode == inviteCode);

            if (!validCode)
            {
                ShowMessage("❌ Invalid invite code. Please ask an Admin for the correct code.", "msg-error");
                return;
            }

            // Create new user (Student or Teacher only)
            var newUser = new User
            {
                UserID = AppData.NextUserID,
                PermanentID = GeneratePermanentID(),
                FullName = name,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role,
                InviteCode = null,
                InvitedByAdminCode = inviteCode
            };

            AppData.Users.Add(newUser);
            AppData.NextUserID++;

            AddActivityLog(newUser.Email, newUser.FullName, newUser.Role, "Register",
                           $"New {role} account created using invite code from Admin.");

            ShowMessage("✅ Account created successfully! You can now log in.", "msg-success");

            // Clear fields
            txtSignupName.Text = "";
            txtSignupEmail.Text = "";
            txtSignupPassword.Text = "";
            txtInviteCode.Text = "";
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblSignupMessage.Text = message;
            lblSignupMessage.CssClass = cssClass;
            lblSignupMessage.Visible = true;
        }
    }
}