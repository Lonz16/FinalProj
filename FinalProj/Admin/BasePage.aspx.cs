using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace CanteenProject
{
    public class BasePage : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SeedDataIfNeeded();

            // Skip auth for account pages
            string path = Request.Url.AbsolutePath.ToLower();
            if (path.Contains("/account/") || path == "/default.aspx")
                return;

            // Check login
            if (Session["LoggedInUser"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            // Check role access
            string userRole = Session["UserRole"]?.ToString();
            if (path.Contains("/student/") && userRole != "Student")
                Response.Redirect("~/Unauthorized.aspx");
            else if (path.Contains("/teacher/") && userRole != "Teacher")
                Response.Redirect("~/Unauthorized.aspx");
            else if (path.Contains("/admin/") && userRole != "Admin")
                Response.Redirect("~/Unauthorized.aspx");
        }

        private void SeedDataIfNeeded()
        {
            if (AppData.IsSeeded) return;

            if (AppData.Users.Count == 0)
            {
                string adminCode = GenerateInviteCode();
                var admin = new User
                {
                    UserID = 1,
                    PermanentID = "UID-1001",
                    FullName = "Jayr Admin",
                    Email = "admin@borrowbox.com",
                    PasswordHash = HashPassword("Admin@123"),
                    Role = "Admin",
                    InviteCode = adminCode,
                    InvitedByAdminCode = null,
                    ProfilePictureUrl = "~/Admin/SetProfilePicture/jayr.jpg"  // ← PERMANENTLY SET HERE
                };
                AppData.Users.Add(admin);
                AppData.NextUserID = 1002;

                AddActivityLog(admin.Email, admin.FullName, admin.Role, "System",
                               $"System initialized. Admin invite code: {adminCode}");
            }

            // ... rest of your code
        }
        protected User GetCurrentUser()
        {
            string email = Session["LoggedInUser"]?.ToString();
            if (string.IsNullOrEmpty(email)) return null;
            return AppData.Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        protected string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        protected string GenerateInviteCode()
        {
            Random rand = new Random();
            string code;
            do
            {
                code = rand.Next(100000, 999999).ToString();
            }
            while (AppData.Users.Any(u => u.InviteCode == code));
            return code;
        }

        protected string GeneratePermanentID()
        {
            return "UID-" + (AppData.NextUserID++).ToString();
        }

        protected string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }

        protected void AddActivityLog(string actorEmail, string actorName, string actorRole,
     string action, string details)
        {
            AppData.ActivityLogs.Add(new ActivityLog
            {
                LogID = AppData.ActivityLogs.Count + 1,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ActorEmail = actorEmail,
                ActorName = actorName,
                ActorRole = actorRole,
                Action = action,
                Details = details
            });
        }

        protected void Logout()
        {
            if (Session["LoggedInUser"] != null)
            {
                AddActivityLog(Session["LoggedInUser"].ToString(),
                    Session["UserName"].ToString(),
                    Session["UserRole"].ToString(),
                    "Logout", "User logged out.");
            }
            Session.Clear();
            Response.Redirect("~/Account/Login.aspx");
        }
    }
}