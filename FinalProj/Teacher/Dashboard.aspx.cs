using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using CanteenProject;

namespace CanteenProject.Teacher
{
    public partial class Dashboard : BasePage
    {
        // Unified search text
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
                LoadUserInfo();
                LoadTeacherDashboard();
                UpdateNotificationBadge();
            }

            // Load profile picture
            LoadProfilePicture();
        }

        private void LoadUserInfo()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                lblTeacherName.Text = user.FullName;
                lblPopupPermanentID.Text = user.PermanentID;
                lblPopupName.Text = user.FullName;
                lblPopupEmail.Text = user.Email;
                lblPopupRole.Text = user.Role;
                lblLogoutUserName.Text = user.FullName + " (" + user.Email + ")";
            }
        }

        private void LoadTeacherDashboard()
        {
            LoadPendingRequests();
            LoadActiveBorrows();
            LoadExtensionRequests();
            UpdateSearchStats();
        }

        private void LoadPendingRequests()
        {
            var pending = AppData.BorrowRequests.Where(r => r.Status == "Pending").AsQueryable();

            if (!string.IsNullOrEmpty(UnifiedSearchText))
            {
                string searchLower = UnifiedSearchText.ToLower();
                pending = pending.Where(r =>
                    r.StudentName.ToLower().Contains(searchLower) ||
                    r.EquipmentName.ToLower().Contains(searchLower) ||
                    r.RequestDate.ToLower().Contains(searchLower));
            }

            var pendingList = pending.OrderBy(r => r.RequestDate).ToList();
            gvPendingRequests.DataSource = pendingList;
            gvPendingRequests.DataBind();

            lblPendingCountBadge.Text = "(" + pendingList.Count + ")";
        }

        private void LoadActiveBorrows()
        {
            var active = AppData.BorrowRecords.Where(b => b.Status == "Borrowed").AsQueryable();

            if (!string.IsNullOrEmpty(UnifiedSearchText))
            {
                string searchLower = UnifiedSearchText.ToLower();
                active = active.Where(b =>
                    b.StudentName.ToLower().Contains(searchLower) ||
                    b.EquipmentName.ToLower().Contains(searchLower) ||
                    b.BorrowDate.ToLower().Contains(searchLower) ||
                    b.DueDate.ToLower().Contains(searchLower));
            }

            var activeList = active.OrderBy(b => b.DueDate).ToList();
            gvActiveBorrows.DataSource = activeList;
            gvActiveBorrows.DataBind();

            lblActiveCountBadge.Text = "(" + activeList.Count + ")";
        }

        private void UpdateSearchStats()
        {
            if (string.IsNullOrEmpty(UnifiedSearchText))
            {
                lblSearchStats.Text = "📋 Showing all pending requests and active borrows";
            }
            else
            {
                int pendingCount = AppData.BorrowRequests.Count(r => r.Status == "Pending" &&
                    (r.StudentName.ToLower().Contains(UnifiedSearchText.ToLower()) ||
                     r.EquipmentName.ToLower().Contains(UnifiedSearchText.ToLower())));

                int activeCount = AppData.BorrowRecords.Count(b => b.Status == "Borrowed" &&
                    (b.StudentName.ToLower().Contains(UnifiedSearchText.ToLower()) ||
                     b.EquipmentName.ToLower().Contains(UnifiedSearchText.ToLower())));

                lblSearchStats.Text = "🔍 Search results for '" + UnifiedSearchText + "': " + pendingCount + " pending request(s), " + activeCount + " active borrow(s)";
            }
        }

        protected string GetDueDateClass(string dueDate)
        {
            if (DateTime.TryParse(dueDate, out DateTime due))
            {
                int daysLeft = (due - DateTime.Now).Days;
                if (daysLeft < 0) return "overdue";
                if (daysLeft <= 3) return "due-soon";
            }
            return "";
        }

        // Highlight search text in GridView rows
        protected void gvPendingRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && !string.IsNullOrEmpty(UnifiedSearchText))
            {
                string searchLower = UnifiedSearchText.ToLower();
                foreach (TableCell cell in e.Row.Cells)
                {
                    if (cell.Text != null && cell.Text.ToLower().Contains(searchLower))
                    {
                        cell.Text = HighlightText(cell.Text, UnifiedSearchText);
                    }
                }
            }
        }

        protected void gvActiveBorrows_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && !string.IsNullOrEmpty(UnifiedSearchText))
            {
                string searchLower = UnifiedSearchText.ToLower();
                foreach (TableCell cell in e.Row.Cells)
                {
                    if (cell.Text != null && cell.Text.ToLower().Contains(searchLower))
                    {
                        cell.Text = HighlightText(cell.Text, UnifiedSearchText);
                    }
                }

                Label lblDueDate = (Label)e.Row.FindControl("lblDueDate");
                if (lblDueDate != null && lblDueDate.Text.ToLower().Contains(UnifiedSearchText.ToLower()))
                {
                    lblDueDate.Text = HighlightText(lblDueDate.Text, UnifiedSearchText);
                }
            }
        }

        private string HighlightText(string text, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || string.IsNullOrEmpty(text))
                return text;

            string searchLower = searchTerm.ToLower();
            string textLower = text.ToLower();

            int index = textLower.IndexOf(searchLower);
            if (index >= 0)
            {
                return text.Substring(0, index) +
                       "<span class='highlight'>" +
                       text.Substring(index, searchTerm.Length) +
                       "</span>" +
                       text.Substring(index + searchTerm.Length);
            }
            return text;
        }

        // Search handlers
        protected void txtUnifiedSearch_TextChanged(object sender, EventArgs e)
        {
            UnifiedSearchText = txtUnifiedSearch.Text.Trim();
            LoadTeacherDashboard();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            UnifiedSearchText = "";
            txtUnifiedSearch.Text = "";
            LoadTeacherDashboard();
        }

        private void UpdateNotificationBadge()
        {
            var borrowed = AppData.BorrowRecords.Where(b => b.Status == "Borrowed");
            int count = 0;
            foreach (var item in borrowed)
            {
                if (DateTime.TryParse(item.DueDate, out DateTime dueDate))
                {
                    if ((dueDate - DateTime.Now).Days <= 3) count++;
                }
            }
            lblNotificationBadge.Text = count > 0 ? count.ToString() : "0";
        }

        // Load extension requests
        private void LoadExtensionRequests()
        {
            var extensions = AppData.ExtensionRequests.Where(r => r.Status == "Pending").AsQueryable();

            if (!string.IsNullOrEmpty(UnifiedSearchText))
            {
                string searchLower = UnifiedSearchText.ToLower();
                extensions = extensions.Where(r =>
                    r.StudentName.ToLower().Contains(searchLower) ||
                    r.EquipmentName.ToLower().Contains(searchLower));
            }

            var extensionList = extensions.OrderBy(r => r.RequestDate).ToList();
            gvExtensionRequests.DataSource = extensionList;
            gvExtensionRequests.DataBind();

            lblExtensionCountBadge.Text = "(" + extensionList.Count + ")";
        }

        // DEBUG METHOD - Check Extension Requests
        protected void btnCheckExtensions_Click(object sender, EventArgs e)
        {
            string message = "=== EXTENSION REQUESTS DEBUG ===<br/>";
            message += "Total ExtensionRequests count: " + AppData.ExtensionRequests.Count + "<br/><br/>";

            if (AppData.ExtensionRequests.Count == 0)
            {
                message += "No extension requests found. Make sure a student has submitted a request.";
            }
            else
            {
                message += "List of Extension Requests:<br/>";
                foreach (var ext in AppData.ExtensionRequests)
                {
                    message += "- Student: " + ext.StudentName + "<br/>";
                    message += "  Equipment: " + ext.EquipmentName + "<br/>";
                    message += "  Status: " + ext.Status + "<br/>";
                    message += "  Days: " + ext.DaysRequested + "<br/>";
                    message += "  BorrowID: " + ext.BorrowID + "<br/>";
                    message += "  ---<br/>";
                }
            }

            lblDebugInfo.Text = message;
            lblDebugInfo.Visible = true;
            lblDebugInfo.CssClass = "msg-info";
        }

        // Handle approve/deny extension requests
        protected void gvExtensionRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ApproveExt" && e.CommandName != "DenyExt") return;

            int extensionID = Convert.ToInt32(e.CommandArgument);
            var extension = AppData.ExtensionRequests.FirstOrDefault(r => r.ExtensionID == extensionID);
            if (extension == null) return;

            string teacherEmail = Session["LoggedInUser"].ToString();
            string teacherName = Session["UserName"].ToString();

            if (e.CommandName == "ApproveExt")
            {
                var borrow = AppData.BorrowRecords.FirstOrDefault(b => b.BorrowID == extension.BorrowID);
                if (borrow != null)
                {
                    string oldDueDate = borrow.DueDate;
                    borrow.DueDate = extension.RequestedNewDate;
                    extension.Status = "Approved";

                    AddActivityLog(teacherEmail, teacherName, "Teacher", "Approve Extension",
                        "Approved extension for " + extension.StudentName + " on '" + extension.EquipmentName +
                        "' from " + oldDueDate + " to " + extension.RequestedNewDate);

                    lblTeacherMessage.Text = "✅ Approved extension for " + extension.StudentName + " on '" + extension.EquipmentName + "'.";
                    lblTeacherMessage.CssClass = "msg-success";
                }
            }
            else
            {
                extension.Status = "Denied";
                AddActivityLog(teacherEmail, teacherName, "Teacher", "Deny Extension",
                    "Denied extension request for " + extension.StudentName + " on '" + extension.EquipmentName + "'");
                lblTeacherMessage.Text = "❌ Denied extension request from " + extension.StudentName + ".";
                lblTeacherMessage.CssClass = "msg-error";
            }

            lblTeacherMessage.Visible = true;
            LoadTeacherDashboard();
            UpdateNotificationBadge();
        }

        // RowDataBound for highlighting
        protected void gvExtensionRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && !string.IsNullOrEmpty(UnifiedSearchText))
            {
                string searchLower = UnifiedSearchText.ToLower();
                foreach (TableCell cell in e.Row.Cells)
                {
                    if (cell.Text != null && cell.Text.ToLower().Contains(searchLower))
                    {
                        cell.Text = HighlightText(cell.Text, UnifiedSearchText);
                    }
                }
            }
        }

        protected void gvPendingRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Approve" && e.CommandName != "Deny") return;

            int requestID = Convert.ToInt32(e.CommandArgument);
            var request = AppData.BorrowRequests.FirstOrDefault(r => r.RequestID == requestID);
            if (request == null) return;

            string teacherEmail = Session["LoggedInUser"].ToString();
            string teacherName = Session["UserName"].ToString();

            if (e.CommandName == "Approve")
            {
                var equip = AppData.EquipmentList.FirstOrDefault(eq => eq.EquipmentID == request.EquipmentID);
                if (equip != null && equip.Quantity > 0)
                {
                    equip.Quantity--;
                    request.Status = "Approved";
                    AppData.BorrowRecords.Add(new BorrowRecord
                    {
                        BorrowID = AppData.NextBorrowID++,
                        StudentEmail = request.StudentEmail,
                        StudentName = request.StudentName,
                        EquipmentID = request.EquipmentID,
                        EquipmentName = request.EquipmentName,
                        BorrowDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        DueDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"),
                        Status = "Borrowed"
                    });
                    AddActivityLog(teacherEmail, teacherName, "Teacher", "Approve Request",
                                   "Approved " + request.StudentName + "'s request for '" + request.EquipmentName + "'");
                    lblTeacherMessage.Text = "✅ Approved: '" + request.EquipmentName + "' for " + request.StudentName + ".";
                    lblTeacherMessage.CssClass = "msg-success";
                }
                else
                {
                    AddActivityLog(teacherEmail, teacherName, "Teacher", "Approve Failed",
                                   "Out of stock – " + request.EquipmentName);
                    lblTeacherMessage.Text = "❌ Cannot approve – '" + request.EquipmentName + "' is out of stock.";
                    lblTeacherMessage.CssClass = "msg-error";
                }
            }
            else
            {
                request.Status = "Denied";
                AddActivityLog(teacherEmail, teacherName, "Teacher", "Deny Request",
                               "Denied " + request.StudentName + "'s request for '" + request.EquipmentName + "'");
                lblTeacherMessage.Text = "❌ Denied request from " + request.StudentName + ".";
                lblTeacherMessage.CssClass = "msg-error";
            }

            lblTeacherMessage.Visible = true;
            LoadTeacherDashboard();
            UpdateNotificationBadge();
        }

        protected void gvActiveBorrows_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "MarkReturned") return;

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            var activeBorrows = AppData.BorrowRecords.Where(b => b.Status == "Borrowed").ToList();
            if (rowIndex < 0 || rowIndex >= activeBorrows.Count) return;

            var borrow = activeBorrows[rowIndex];
            borrow.Status = "Returned";
            borrow.ReturnDate = DateTime.Now.ToString("yyyy-MM-dd");

            var equip = AppData.EquipmentList.FirstOrDefault(eq => eq.EquipmentID == borrow.EquipmentID);
            if (equip != null) equip.Quantity++;

            AddActivityLog(Session["LoggedInUser"].ToString(), Session["UserName"].ToString(),
                           "Teacher", "Mark Returned", "Marked '" + borrow.EquipmentName + "' returned by " + borrow.StudentName);
            lblTeacherMessage.Text = "✅ '" + borrow.EquipmentName + "' marked as returned by " + borrow.StudentName + ".";
            lblTeacherMessage.CssClass = "msg-success";
            lblTeacherMessage.Visible = true;
            LoadTeacherDashboard();
            UpdateNotificationBadge();
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

                                AddActivityLog(email, user.FullName, "Teacher", "Profile Picture", "Updated profile picture");
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

        // ========== END PROFILE PICTURE METHODS ==========

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}