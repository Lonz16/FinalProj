using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace CanteenProject.Student
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
            // Check if user is logged in
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
                LoadStudentDashboard();
                UpdateNotificationBadge();
            }
        }

        private void LoadUserInfo()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                lblStudentName.Text = user.FullName;
                lblPopupPermanentID.Text = user.PermanentID;
                lblPopupName.Text = user.FullName;
                lblPopupEmail.Text = user.Email;
                lblPopupRole.Text = user.Role;
            }
        }

        private void LoadStudentDashboard()
        {
            LoadAvailableEquipment();
            LoadMyBorrows();
            UpdateSearchStats();
        }

        private void LoadAvailableEquipment()
        {
            var equipment = AppData.EquipmentList.Where(eq => eq.Quantity > 0).AsQueryable();

            if (!string.IsNullOrEmpty(UnifiedSearchText))
            {
                string searchLower = UnifiedSearchText.ToLower();
                equipment = equipment.Where(eq =>
                    eq.Name.ToLower().Contains(searchLower) ||
                    (eq.Category != null && eq.Category.ToLower().Contains(searchLower)) ||
                    (eq.Description != null && eq.Description.ToLower().Contains(searchLower)));
            }

            var equipmentList = equipment.ToList();
            gvAvailableEquipment.DataSource = equipmentList;
            gvAvailableEquipment.DataBind();
            lblEquipmentCount.Text = "(" + equipmentList.Count + ")";
        }

        private void LoadMyBorrows()
        {
            string studentEmail = Session["LoggedInUser"].ToString();
            var borrows = AppData.BorrowRecords
                .Where(b => b.StudentEmail == studentEmail && b.Status == "Borrowed").ToList();

            gvMyBorrows.DataSource = borrows;
            gvMyBorrows.DataBind();
            lblBorrowedCount.Text = "(" + borrows.Count + ")";
        }

        private void UpdateSearchStats()
        {
            if (string.IsNullOrEmpty(UnifiedSearchText))
            {
                lblSearchStats.Text = "📋 Showing all available equipment";
            }
            else
            {
                int count = AppData.EquipmentList.Count(eq => eq.Quantity > 0 &&
                    (eq.Name.ToLower().Contains(UnifiedSearchText.ToLower()) ||
                     (eq.Category != null && eq.Category.ToLower().Contains(UnifiedSearchText.ToLower())) ||
                     (eq.Description != null && eq.Description.ToLower().Contains(UnifiedSearchText.ToLower()))));

                lblSearchStats.Text = "🔍 Search results for '" + UnifiedSearchText + "': " + count + " item(s) found";
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

        protected void gvAvailableEquipment_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void txtUnifiedSearch_TextChanged(object sender, EventArgs e)
        {
            UnifiedSearchText = txtUnifiedSearch.Text.Trim();
            LoadAvailableEquipment();
            UpdateSearchStats();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            UnifiedSearchText = "";
            txtUnifiedSearch.Text = "";
            LoadAvailableEquipment();
            UpdateSearchStats();
        }

        private void UpdateNotificationBadge()
        {
            string userEmail = Session["LoggedInUser"].ToString();
            var borrowed = AppData.BorrowRecords
                .Where(b => b.StudentEmail == userEmail && b.Status == "Borrowed");

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

        protected void gvAvailableEquipment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Borrow") return;

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            var equipment = AppData.EquipmentList.Where(eq => eq.Quantity > 0).ToList();
            if (rowIndex < 0 || rowIndex >= equipment.Count) return;

            var equip = equipment[rowIndex];
            string studentEmail = Session["LoggedInUser"].ToString();
            string studentName = Session["UserName"].ToString();

            bool alreadyPending = AppData.BorrowRequests.Any(r =>
                r.StudentEmail == studentEmail &&
                r.EquipmentID == equip.EquipmentID &&
                r.Status == "Pending");

            if (alreadyPending)
            {
                lblStudentMessage.Text = "⚠️ You already have a pending request for " + equip.Name + ".";
                lblStudentMessage.CssClass = "msg-error";
                lblStudentMessage.Visible = true;
                return;
            }

            bool alreadyBorrowed = AppData.BorrowRecords.Any(b =>
                b.StudentEmail == studentEmail &&
                b.EquipmentID == equip.EquipmentID &&
                b.Status == "Borrowed");

            if (alreadyBorrowed)
            {
                lblStudentMessage.Text = "⚠️ You already have '" + equip.Name + "' borrowed.";
                lblStudentMessage.CssClass = "msg-error";
                lblStudentMessage.Visible = true;
                return;
            }

            AppData.BorrowRequests.Add(new BorrowRequest
            {
                RequestID = AppData.NextRequestID++,
                StudentEmail = studentEmail,
                StudentName = studentName,
                EquipmentID = equip.EquipmentID,
                EquipmentName = equip.Name,
                RequestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Status = "Pending"
            });

            AddActivityLog(studentEmail, studentName, "Student", "Borrow Request", "Requested '" + equip.Name + "'");

            lblStudentMessage.Text = "✅ Request for '" + equip.Name + "' sent to teacher.";
            lblStudentMessage.CssClass = "msg-success";
            lblStudentMessage.Visible = true;
            LoadAvailableEquipment();
        }

        // ========== SUBMIT EXTENSION REQUEST FROM MODAL ==========
        protected void btnSubmitExtension_Click(object sender, EventArgs e)
        {
            // Get BorrowID from HiddenField (not ViewState)
            int borrowId = 0;
            int.TryParse(hfBorrowID.Value, out borrowId);
            int daysToAdd = Convert.ToInt32(ddlExtendDays.SelectedValue);
            string email = Session["LoggedInUser"].ToString();

            // Debug output
            System.Diagnostics.Debug.WriteLine("=== SUBMIT EXTENSION REQUEST ===");
            System.Diagnostics.Debug.WriteLine("BorrowID from hidden field: " + borrowId);
            System.Diagnostics.Debug.WriteLine("Days to add: " + daysToAdd);

            var borrow = AppData.BorrowRecords.FirstOrDefault(b => b.BorrowID == borrowId && b.StudentEmail == email);
            if (borrow == null)
            {
                lblStudentMessage.Text = "❌ Borrow record not found. BorrowID: " + borrowId;
                lblStudentMessage.CssClass = "msg-error";
                lblStudentMessage.Visible = true;

                // Close modal
                ClientScript.RegisterStartupScript(this.GetType(), "closeModal", "closeExtensionModal();", true);
                return;
            }

            // Check if there's already a pending extension request
            bool hasPendingRequest = false;
            if (AppData.ExtensionRequests != null)
            {
                hasPendingRequest = AppData.ExtensionRequests.Any(r => r.BorrowID == borrowId && r.Status == "Pending");
            }

            if (hasPendingRequest)
            {
                lblStudentMessage.Text = "⚠️ You already have a pending extension request for this item. Please wait for teacher approval.";
                lblStudentMessage.CssClass = "msg-error";
                lblStudentMessage.Visible = true;
                ClientScript.RegisterStartupScript(this.GetType(), "closeModal", "closeExtensionModal();", true);
                return;
            }

            // Check if already extended once
            bool alreadyExtended = false;
            if (AppData.ExtensionRequests != null)
            {
                alreadyExtended = AppData.ExtensionRequests.Any(r => r.BorrowID == borrowId && r.Status == "Approved");
            }

            if (alreadyExtended)
            {
                lblStudentMessage.Text = "⚠️ You have already extended this item. Maximum 7 days extension allowed once.";
                lblStudentMessage.CssClass = "msg-error";
                lblStudentMessage.Visible = true;
                ClientScript.RegisterStartupScript(this.GetType(), "closeModal", "closeExtensionModal();", true);
                return;
            }

            // Parse current due date
            DateTime currentDueDate;
            if (!DateTime.TryParse(borrow.DueDate, out currentDueDate))
            {
                lblStudentMessage.Text = "❌ Invalid due date format.";
                lblStudentMessage.CssClass = "msg-error";
                lblStudentMessage.Visible = true;
                ClientScript.RegisterStartupScript(this.GetType(), "closeModal", "closeExtensionModal();", true);
                return;
            }

            // Calculate new due date
            DateTime newDueDate = currentDueDate.AddDays(daysToAdd);

            // Check if extension exceeds 30 days total from today
            if (newDueDate > DateTime.Now.AddDays(30))
            {
                lblStudentMessage.Text = "⚠️ Cannot extend beyond 30 days from today. Maximum extension not allowed.";
                lblStudentMessage.CssClass = "msg-error";
                lblStudentMessage.Visible = true;
                ClientScript.RegisterStartupScript(this.GetType(), "closeModal", "closeExtensionModal();", true);
                return;
            }

            // Initialize ExtensionRequests if null
            if (AppData.ExtensionRequests == null)
            {
                AppData.ExtensionRequests = new System.Collections.Generic.List<ExtensionRequest>();
                AppData.NextExtensionID = 1;
            }

            // Create extension request
            AppData.ExtensionRequests.Add(new ExtensionRequest
            {
                ExtensionID = AppData.NextExtensionID++,
                BorrowID = borrow.BorrowID,
                StudentEmail = email,
                StudentName = Session["UserName"].ToString(),
                EquipmentName = borrow.EquipmentName,
                CurrentDueDate = borrow.DueDate,
                RequestedNewDate = newDueDate.ToString("yyyy-MM-dd"),
                RequestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Status = "Pending",
                DaysRequested = daysToAdd
            });

            AddActivityLog(email, Session["UserName"].ToString(), "Student", "Extension Request",
                "Requested " + daysToAdd + "-day extension for '" + borrow.EquipmentName + "' to " + newDueDate.ToString("yyyy-MM-dd"));

            lblStudentMessage.Text = "✅ Extension request for " + daysToAdd + " days sent to teacher for approval.";
            lblStudentMessage.CssClass = "msg-success";
            lblStudentMessage.Visible = true;

            // Close modal and refresh
            ClientScript.RegisterStartupScript(this.GetType(), "closeModal", "closeExtensionModal();", true);
            LoadStudentDashboard();
            UpdateNotificationBadge();
        }
        // ========== END EXTENSION REQUEST METHOD ==========

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}