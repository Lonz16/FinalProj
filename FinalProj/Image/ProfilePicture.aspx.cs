using System;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace CanteenProject.Shared
{
    public partial class ProfilePicture : System.Web.UI.UserControl
    {
        // Public property to set the user email from the parent page
        private string _userEmail;
        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                LoadProfilePicture();
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
                    // Validate file type
                    string extension = Path.GetExtension(fileUpload.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        lblUploadMessage.Text = "❌ Only JPG, JPEG, PNG, and GIF files are allowed.";
                        lblUploadMessage.CssClass = "msg-error";
                        lblUploadMessage.Visible = true;
                        return;
                    }

                    // Validate file size (max 2MB)
                    if (fileUpload.PostedFile.ContentLength > 2 * 1024 * 1024)
                    {
                        lblUploadMessage.Text = "❌ File size must be less than 2MB.";
                        lblUploadMessage.CssClass = "msg-error";
                        lblUploadMessage.Visible = true;
                        return;
                    }

                    // Create Images directory if not exists
                    string imagesDir = Server.MapPath("~/Images/Profiles/");
                    if (!Directory.Exists(imagesDir))
                    {
                        Directory.CreateDirectory(imagesDir);
                    }

                    // Generate unique filename
                    string fileName = Guid.NewGuid().ToString() + extension;
                    string filePath = "~/Images/Profiles/" + fileName;
                    string physicalPath = Server.MapPath(filePath);

                    // Save file
                    fileUpload.SaveAs(physicalPath);

                    // Update user's profile picture URL
                    var user = AppData.Users.FirstOrDefault(u =>
                        u.Email.Equals(_userEmail, StringComparison.OrdinalIgnoreCase));

                    if (user != null)
                    {
                        // Delete old profile picture if exists and not default
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
                        AddActivityLog(user.Email, user.FullName, user.Role,
                            "Profile Picture", "Updated profile picture");

                        lblUploadMessage.Text = "✅ Profile picture updated successfully!";
                        lblUploadMessage.CssClass = "msg-success";
                        lblUploadMessage.Visible = true;

                        // Reload the image with cache busting
                        imgProfile.ImageUrl = filePath + "?t=" + DateTime.Now.Ticks;

                        // Close modal after 2 seconds
                        ClientScript.RegisterStartupScript(this.GetType(), "closeModal",
                            "setTimeout(function() { closeUploadModal(); }, 2000);", true);
                    }
                }
                catch (Exception ex)
                {
                    lblUploadMessage.Text = $"❌ Error uploading file: {ex.Message}";
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

        private void AddActivityLog(string actorEmail, string actorName, string actorRole,
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