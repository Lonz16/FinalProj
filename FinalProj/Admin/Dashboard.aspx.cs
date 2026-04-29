using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using CanteenProject;

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
            // Check login
            if (Session["LoggedInUser"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                var user = GetCurrentUser();
                if (user != null)
                {
                    lblLogoutUserName.Text = user.FullName + " (" + user.Email + ")";
                }

                LoadUserInfo();
                LoadAdminDashboard();
                UpdateNotificationBadge();
                LoadActivityLog();
            }

            // Load profile picture
            LoadProfilePicture();
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

            if (string.IsNullOrEmpty(UnifiedSearchText))
            {
                lblSearchStats.Text = "📋 Showing " + logList.Count + " total activity records";
            }
            else
            {
                lblSearchStats.Text = "🔍 Found " + logList.Count + " record(s) matching '" + UnifiedSearchText + "'";
            }
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

        // ========== PROFILE PICTURE METHODS ==========

        private void LoadProfilePicture()
        {
            try
            {
                string email = Session["LoggedInUser"] as string;
                if (string.IsNullOrEmpty(email)) return;

                var user = GetUserByEmail(email);
                if (user != null && !string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    string physicalPath = Server.MapPath(user.ProfilePictureUrl);
                    if (File.Exists(physicalPath))
                    {
                        string finalUrl = user.ProfilePictureUrl + "?t=" + DateTime.Now.Ticks;
                        imgProfile.ImageUrl = finalUrl;
                        imgLargeAvatar.ImageUrl = finalUrl;
                        return;
                    }
                }
                imgProfile.ImageUrl = "~/Images/default-avatar.png";
                imgLargeAvatar.ImageUrl = "~/Images/default-avatar.png";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadProfilePicture Error: " + ex.Message);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                string ext = Path.GetExtension(fileUpload.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                {
                    if (fileUpload.PostedFile.ContentLength <= 2 * 1024 * 1024)
                    {
                        try
                        {
                            string folder = Server.MapPath("~/Images/Profiles/");
                            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                            string fileName = Guid.NewGuid().ToString() + ext;
                            string relativePath = "~/Images/Profiles/" + fileName;
                            fileUpload.SaveAs(Server.MapPath(relativePath));

                            string email = Session["LoggedInUser"] as string;
                            var user = GetUserByEmail(email);
                            if (user != null)
                            {
                                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                                {
                                    string oldPath = Server.MapPath(user.ProfilePictureUrl);
                                    if (File.Exists(oldPath)) File.Delete(oldPath);
                                }
                                user.ProfilePictureUrl = relativePath;
                                LoadProfilePicture();
                                lblUploadMsg.Text = "Profile picture updated!";
                                lblUploadMsg.ForeColor = System.Drawing.Color.Green;
                            }
                        }
                        catch (Exception ex)
                        {
                            lblUploadMsg.Text = "Error: " + ex.Message;
                            lblUploadMsg.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else
                    {
                        lblUploadMsg.Text = "File too large (max 2MB)";
                        lblUploadMsg.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    lblUploadMsg.Text = "Only JPG, PNG, GIF files allowed";
                    lblUploadMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblUploadMsg.Text = "Please select a file";
                lblUploadMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void UpdateNotificationBadge()
        {
            int count = 0;
            foreach (var b in AppData.BorrowRecords)
            {
                if (b.Status == "Borrowed")
                {
                    DateTime due;
                    if (DateTime.TryParse(b.DueDate, out due))
                    {
                        if ((due - DateTime.Now).Days <= 3) count++;
                    }
                }
            }
            lblNotificationBadge.Text = count > 0 ? count.ToString() : "0";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}