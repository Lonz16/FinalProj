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
            if (!IsPostBack)
            {
                string userEmail = Session["LoggedInUser"].ToString();
              

                var user = GetCurrentUser();
                if (user != null)
                {
                    lblLogoutUserName.Text = $"{user.FullName} ({user.Email})";
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
            lblEquipmentCount.Text = $"({equipmentList.Count})";
        }

        private void LoadMyBorrows()
        {
            string studentEmail = Session["LoggedInUser"].ToString();
            var borrows = AppData.BorrowRecords
                .Where(b => b.StudentEmail == studentEmail && b.Status == "Borrowed").ToList();

            gvMyBorrows.DataSource = borrows;
            gvMyBorrows.DataBind();
            lblBorrowedCount.Text = $"({borrows.Count})";
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

                lblSearchStats.Text = $"🔍 Search results for '{UnifiedSearchText}': {count} item(s) found";
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
                lblStudentMessage.Text = $"⚠️ You already have a pending request for {equip.Name}.";
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
                lblStudentMessage.Text = $"⚠️ You already have '{equip.Name}' borrowed.";
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

            AddActivityLog(studentEmail, studentName, "Student", "Borrow Request", $"Requested '{equip.Name}'");

            lblStudentMessage.Text = $"✅ Request for '{equip.Name}' sent to teacher.";
            lblStudentMessage.CssClass = "msg-success";
            lblStudentMessage.Visible = true;
            LoadAvailableEquipment();
        }

        protected void gvMyBorrows_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Return") return;

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            string email = Session["LoggedInUser"].ToString();
            var myBorrows = AppData.BorrowRecords
                .Where(b => b.StudentEmail == email && b.Status == "Borrowed").ToList();

            if (rowIndex < 0 || rowIndex >= myBorrows.Count) return;
            var borrow = myBorrows[rowIndex];

            borrow.Status = "Returned";
            borrow.ReturnDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            var equip = AppData.EquipmentList.FirstOrDefault(eq => eq.EquipmentID == borrow.EquipmentID);
            if (equip != null) equip.Quantity++;

            AddActivityLog(email, Session["UserName"].ToString(), "Student", "Return Item", $"Returned '{borrow.EquipmentName}'");

            lblStudentMessage.Text = $"✅ '{borrow.EquipmentName}' returned successfully.";
            lblStudentMessage.CssClass = "msg-success";
            lblStudentMessage.Visible = true;
            LoadStudentDashboard();
            UpdateNotificationBadge();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}