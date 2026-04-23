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
                lblSignupMessage.Text = "❌ All fields are required.";
                lblSignupMessage.CssClass = "msg-error";
                return;
            }

            if (password.Length < 6)
            {
                lblSignupMessage.Text = "❌ Password must be at least 6 characters.";
                lblSignupMessage.CssClass = "msg-error";
                return;
            }

            if (AppData.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                lblSignupMessage.Text = "❌ An account with that email already exists.";
                lblSignupMessage.CssClass = "msg-error";
                return;
            }

            // Invite code check for Students and Teachers
            if (role == "Student" || role == "Teacher")
            {
                bool validCode = AppData.Users.Any(u =>
                    u.Role == "Admin" && u.InviteCode == inviteCode);

                if (!validCode)
                {
                    lblSignupMessage.Text = "❌ Invalid invite code. Please ask an Admin for their invite code.";
                    lblSignupMessage.CssClass = "msg-error";
                    return;
                }
            }

            // Create new user
            string newInviteCode = (role == "Admin") ? GenerateInviteCode() : null;
            var newUser = new User
            {
                UserID = AppData.NextUserID,
                PermanentID = GeneratePermanentID(),
                FullName = name,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role,
                InviteCode = newInviteCode,
                InvitedByAdminCode = (role != "Admin") ? inviteCode : null
            };

            AppData.Users.Add(newUser);
            AppData.NextUserID++;

            AddActivityLog(newUser.Email, newUser.FullName, newUser.Role, "Register",
                           $"New {role} account created.");

            lblSignupMessage.Text = "✅ Account created successfully! You can now log in.";
            lblSignupMessage.CssClass = "msg-success";

            // Clear fields
            txtSignupName.Text = "";
            txtSignupEmail.Text = "";
            txtSignupPassword.Text = "";
            txtInviteCode.Text = "";
        }
    }
}