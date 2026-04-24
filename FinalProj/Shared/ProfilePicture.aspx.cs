using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CanteenProject.Shared
{
    public partial class ProfilePicture : System.Web.UI.UserControl
    {
        private string _userEmail;

        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                if (!string.IsNullOrEmpty(_userEmail))
                {
                    LoadProfilePicture();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(_userEmail))
                {
                    LoadProfilePicture();
                }
            }
        }

        private void LoadProfilePicture()
        {
            if (string.IsNullOrEmpty(_userEmail)) return;

            var user = AppData.Users.FirstOrDefault(u =>
                u.Email.Equals(_userEmail, StringComparison.OrdinalIgnoreCase));

            if (user != null && !string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                imgProfile.ImageUrl = user.ProfilePictureUrl;
            }
            else
            {
                imgProfile.ImageUrl = "~/Images/default-avatar.png";
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                try
                {
                    string extension = Path.GetExtension(fileUpload.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        lblUploadMessage.Text = "❌ Only JPG, JPEG, PNG, and GIF files are allowed.";
                        lblUploadMessage.CssClass = "msg-error";
                        lblUploadMessage.Visible = true;
                        return;
                    }

                    if (fileUpload.PostedFile.ContentLength > 2 * 1024 * 1024)
                    {
                        lblUploadMessage.Text = "❌ File size must be less than 2MB.";
                        lblUploadMessage.CssClass = "msg-error";
                        lblUploadMessage.Visible = true;
                        return;
                    }

                    string imagesDir = Server.MapPath("~/Images/Profiles/");
                    if (!Directory.Exists(imagesDir))
                    {
                        Directory.CreateDirectory(imagesDir);
                    }

                    string fileName = Guid.NewGuid().ToString() + extension;
                    string filePath = "~/Images/Profiles/" + fileName;
                    string physicalPath = Server.MapPath(filePath);

                    fileUpload.SaveAs(physicalPath);

                    var user = AppData.Users.FirstOrDefault(u =>
                        u.Email.Equals(_userEmail, StringComparison.OrdinalIgnoreCase));

                    if (user != null)
                    {
                        if (!string.IsNullOrEmpty(user.ProfilePictureUrl) &&
                            !user.ProfilePictureUrl.Contains("default-avatar"))
                        {
                            string oldPath = Server.MapPath(user.ProfilePictureUrl);
                            if (File.Exists(oldPath))
                            {
                                File.Delete(oldPath);
                            }
                        }

                        user.ProfilePictureUrl = filePath;

                        // Log activity
                        LogActivity(user.Email, user.FullName, user.Role,
                            "Profile Picture", "Updated profile picture");

                        lblUploadMessage.Text = "✅ Profile picture updated successfully!";
                        lblUploadMessage.CssClass = "msg-success";
                        lblUploadMessage.Visible = true;

                        imgProfile.ImageUrl = filePath + "?t=" + DateTime.Now.Ticks;

                        string script = "setTimeout(function() { closeUploadModal(); }, 2000);";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "closeModal", script, true);
                    }
                }
                catch (Exception ex)
                {
                    lblUploadMessage.Text = $"❌ Error: {ex.Message}";
                    lblUploadMessage.CssClass = "msg-error";
                    lblUploadMessage.Visible = true;
                }
            }
            else
            {
                lblUploadMessage.Text = "❌ Please select a file to upload.";
                lblUploadMessage.CssClass = "msg-error";
                lblUploadMessage.Visible = true;
            }
        }

        private void LogActivity(string actorEmail, string actorName, string actorRole,
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
    }
}