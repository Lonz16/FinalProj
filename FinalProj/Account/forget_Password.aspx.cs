using System;
using System.Linq;

namespace CanteenProject.Account
{
    public partial class ForgotPassword : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Show debug info only if you're testing
            // debugInfo.Visible = true;
            if (debugInfo.Visible)
            {
                LoadDebugInfo();
            }
        }

        private void LoadDebugInfo()
        {
            lblDebugUserCount.Text = AppData.Users.Count.ToString();
            var emails = string.Join(", ", AppData.Users.Select(u => u.Email));
            lblDebugEmails.Text = "Registered emails: " + emails;
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            string email = txtForgotEmail.Text.Trim();

            // Clear previous message
            lblForgotMessage.Visible = false;
            lblForgotMessage.Text = "";

            // Validation 1: Check if email is empty
            if (string.IsNullOrEmpty(email))
            {
                ShowMessage("❌ Please enter your email address.", "msg-error");
                return;
            }

            // Validation 2: Basic email format check
            if (!IsValidEmail(email))
            {
                ShowMessage("❌ Please enter a valid email address.", "msg-error");
                return;
            }

            // Find user - CASE INSENSITIVE search
            var user = AppData.Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            // Validation 3: Check if user exists
            if (user == null)
            {
                ShowMessage($"❌ No account found with email '{email}'. Please check your email or sign up.", "msg-error");
                return;
            }

            try
            {
                // Generate temporary password
                string tempPwd = GenerateRandomPassword();

                // Hash the temporary password
                string hashedTempPwd = HashPassword(tempPwd);

                // Update user's password
                user.PasswordHash = hashedTempPwd;

                // Log the activity
                AddActivityLog(user.Email, user.FullName, user.Role, "Password Reset",
                               $"Temporary password issued for {user.Email}");

                // Show success message with temporary password
                string successMessage = $@"✅ Password reset successful!
                    <div class='temp-password-box'>
                        <div style='font-size:0.8rem; margin-bottom:0.5rem;'>Your temporary password is:</div>
                        <div class='temp-password'>{tempPwd}</div>
                        <div style='font-size:0.7rem; margin-top:0.5rem;'>Please log in and change your password immediately.</div>
                    </div>";

                ShowMessage(successMessage, "msg-success");

                // Clear the email field
                txtForgotEmail.Text = "";
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ An error occurred: {ex.Message}", "msg-error");
            }
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblForgotMessage.Text = message;
            lblForgotMessage.CssClass = cssClass;
            lblForgotMessage.Visible = true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Override the GenerateRandomPassword to make it more user-friendly
        private new string GenerateRandomPassword()
        {
            // Generate a more readable password: word + number
            string[] words = { "Blue", "Red", "Green", "Yellow", "Purple", "Orange",
                               "Tiger", "Eagle", "Lion", "Bear", "Wolf", "Hawk",
                               "Summer", "Winter", "Spring", "Autumn", "Cloud", "Star" };
            Random rand = new Random();
            string word = words[rand.Next(words.Length)];
            int number = rand.Next(100, 999);
            return word + number;
        }
    }
}