using System;
using System.Linq;

namespace CanteenProject.Account
{
    public partial class Login : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string hash = HashPassword("Admin@123");
            System.Diagnostics.Debug.WriteLine("Admin password hash: " + hash);
            // If already logged in, redirect to appropriate dashboard
            if (Session["LoggedInUser"] != null)
            {
                RedirectToDashboard();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtLoginEmail.Text.Trim();
            string password = txtLoginPassword.Text.Trim();

            var user = AppData.Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user != null && user.PasswordHash == HashPassword(password))
            {
                // Set session variables
                Session["LoggedInUser"] = user.Email;
                Session["UserRole"] = user.Role;
                Session["UserName"] = user.FullName;
                Session["UserPermanentID"] = user.PermanentID;

                // Log the login
                AddActivityLog(user.Email, user.FullName, user.Role, "Login", "User logged in.");

                // Redirect based on role
                RedirectToDashboard();
            }
            else
            {
                lblLoginMessage.Text = "❌ Invalid email or password.";
                lblLoginMessage.Visible = true;
            }
        }

        private void RedirectToDashboard()
        {
            string role = Session["UserRole"]?.ToString();
            switch (role)
            {
                case "Student":
                    Response.Redirect("~/Student/Dashboard.aspx");
                    break;
                case "Teacher":
                    Response.Redirect("~/Teacher/Dashboard.aspx");
                    break;
                case "Admin":
                    Response.Redirect("~/Admin/Dashboard.aspx");
                    break;
                default:
                    Response.Redirect("~/Account/Login.aspx");
                    break;
            }
        }
    }
}