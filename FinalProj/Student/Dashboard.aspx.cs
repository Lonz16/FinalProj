using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace CanteenProject.Student
{
    public partial class Dashboard : BasePage
    {
        // Store current filter and search text
        private string CurrentCategoryFilter
        {
            get { return ViewState["CurrentCategoryFilter"] as string ?? "All"; }
            set { ViewState["CurrentCategoryFilter"] = value; }
        }

        private string CurrentSearchText
        {
            get { return ViewState["CurrentSearchText"] as string ?? ""; }
            set { ViewState["CurrentSearchText"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string userEmail = Session["LoggedInUser"].ToString();

             
                // Set logout user info
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
        }

        private void LoadAvailableEquipment()
        {
            // Start with all equipment that has stock
            var equipment = AppData.EquipmentList.Where(eq => eq.Quantity > 0).AsQueryable();

            // Apply category filter
            if (CurrentCategoryFilter != "All")
            {
                equipment = equipment.Where(eq => eq.Category == CurrentCategoryFilter);
            }

            // Apply search filter (search by name or category)
            if (!string.IsNullOrEmpty(CurrentSearchText))
            {
                string searchLower = CurrentSearchText.ToLower();
                equipment = equipment.Where(eq =>
                    eq.Name.ToLower().Contains(searchLower) ||
                    eq.Category.ToLower().Contains(searchLower));
            }

            var filteredList = equipment.ToList();

            // Update results count
            lblResultsCount.Text = $"Showing {filteredList.Count} item(s)";

            // Bind to grid
            gvAvailableEquipment.DataSource = filteredList;
            gvAvailableEquipment.DataBind();
        }

        private void LoadMyBorrows()
        {
            string studentEmail = Session["LoggedInUser"].ToString();
            gvMyBorrows.DataSource = AppData.BorrowRecords
                .Where(b => b.StudentEmail == studentEmail && b.Status == "Borrowed").ToList();
            gvMyBorrows.DataBind();
        }

        private void HighlightActiveFilter()
        {
            // Reset all button styles
            btnAll.CssClass = "filter-btn";
            btnElectronics.CssClass = "filter-btn";
            btnMath.CssClass = "filter-btn";
            btnSports.CssClass = "filter-btn";
            btnSupplies.CssClass = "filter-btn";

            // Highlight active button
            switch (CurrentCategoryFilter)
            {
                case "All":
                    btnAll.CssClass = "filter-btn active";
                    break;
                case "Electronics":
                    btnElectronics.CssClass = "filter-btn active";
                    break;
                case "Math":
                    btnMath.CssClass = "filter-btn active";
                    break;
                case "Sports":
                    btnSports.CssClass = "filter-btn active";
                    break;
                case "Supplies":
                    btnSupplies.CssClass = "filter-btn active";
                    break;
            }
        }

        private void UpdateNotificationBadge()
        {
            string userEmail = Session["LoggedInUser"].ToString();
            var borrowed = AppData.BorrowRecords
                .Where(b => b.StudentEmail == userEmail && b.Status == "Borrowed");

            int count = 0;
            var dueItems = new System.Collections.Generic.List<dynamic>();

            foreach (var item in borrowed)
            {
                if (DateTime.TryParse(item.DueDate, out DateTime dueDate))
                {
                    int daysLeft = (dueDate - DateTime.Now).Days;
                    if (daysLeft <= 3)
                    {
                        count++;
                        dueItems.Add(new { item.EquipmentName, item.DueDate, DaysLeft = daysLeft });
                    }
                }
            }

            lblNotificationBadge.Text = count > 0 ? count.ToString() : "0";

            var html = new System.Text.StringBuilder();
            if (count == 0)
            {
                html.Append("<div class='no-notifications'>✨ No items due soon.</div>");
            }
            else
            {
                html.Append("<h4>⚠️ Items due within 3 days</h4>");
                foreach (var item in dueItems)
                {
                    string css = item.DaysLeft < 0 ? "overdue" : "due-soon";
                    string text = item.DaysLeft < 0 ? "OVERDUE" : $"Due in {item.DaysLeft} day(s)";
                    html.Append($@"
                        <div class='notification-item'>
                            <strong>{item.EquipmentName}</strong><br />
                            Due: {item.DueDate} – <span class='{css}'>{text}</span>
                        </div>");
                }
            }
            litNotificationContent.Text = html.ToString();
        }

        // Search text changed - triggers auto-search
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            CurrentSearchText = txtSearch.Text.Trim();
            LoadAvailableEquipment();
        }

        // Filter button click
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            CurrentCategoryFilter = btn.CommandArgument;
            HighlightActiveFilter();
            LoadAvailableEquipment();
        }

        // Clear all filters
        protected void btnClear_Click(object sender, EventArgs e)
        {
            CurrentCategoryFilter = "All";
            CurrentSearchText = "";
            txtSearch.Text = "";
            HighlightActiveFilter();
            LoadAvailableEquipment();
        }

        protected void gvAvailableEquipment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Borrow") return;

            int rowIndex = Convert.ToInt32(e.CommandArgument);

            // Get the currently filtered list
            var equipment = AppData.EquipmentList.Where(eq => eq.Quantity > 0).AsQueryable();

            if (CurrentCategoryFilter != "All")
            {
                equipment = equipment.Where(eq => eq.Category == CurrentCategoryFilter);
            }

            if (!string.IsNullOrEmpty(CurrentSearchText))
            {
                string searchLower = CurrentSearchText.ToLower();
                equipment = equipment.Where(eq =>
                    eq.Name.ToLower().Contains(searchLower) ||
                    eq.Category.ToLower().Contains(searchLower));
            }

            var filteredList = equipment.ToList();

            if (rowIndex < 0 || rowIndex >= filteredList.Count) return;

            var equip = filteredList[rowIndex];
            string studentEmail = Session["LoggedInUser"].ToString();
            string studentName = Session["UserName"].ToString();

            // Prevent duplicate pending request
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

            // Check if already borrowed
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

            // Add borrow request
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
            LoadAvailableEquipment(); // Refresh the filtered list
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

        // SINGLE logout method - uses BasePage.Logout()
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}