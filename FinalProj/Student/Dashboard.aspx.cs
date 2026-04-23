using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace CanteenProject.Student
{
    public partial class Dashboard : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserInfo();
                LoadStudentDashboard();
                UpdateNotificationBadge();

                // Set UserEmail for profile picture controls
                string userEmail = Session["LoggedInUser"].ToString();
                profilePicture1.UserEmail = userEmail;
                profilePicturePopup.UserEmail = userEmail;
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
            // Available equipment (stock > 0)
            gvAvailableEquipment.DataSource = AppData.EquipmentList
                .Where(eq => eq.Quantity > 0).ToList();
            gvAvailableEquipment.DataBind();

            // Student's own active borrows
            string studentEmail = Session["LoggedInUser"].ToString();
            var activeBorrows = AppData.BorrowRecords
                .Where(b => b.StudentEmail == studentEmail && b.Status == "Borrowed")
                .ToList();
            gvMyBorrows.DataSource = activeBorrows;
            gvMyBorrows.DataBind();
        }

        // Helper method to get CSS class for due date
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

        // Apply row styling based on due date
        protected void gvMyBorrows_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var borrow = (BorrowRecord)e.Row.DataItem;
                if (DateTime.TryParse(borrow.DueDate, out DateTime dueDate))
                {
                    int daysLeft = (dueDate - DateTime.Now).Days;
                    if (daysLeft < 0)
                    {
                        e.Row.CssClass = "overdue-row";
                    }
                    else if (daysLeft <= 3)
                    {
                        e.Row.CssClass = "due-soon-row";
                    }
                }
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

            // Generate notification HTML
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

        protected void gvAvailableEquipment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Borrow") return;

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            var available = AppData.EquipmentList.Where(eq => eq.Quantity > 0).ToList();
            if (rowIndex < 0 || rowIndex >= available.Count) return;

            var equip = available[rowIndex];
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

            // Check if already borrowed (not returned yet)
            bool alreadyBorrowed = AppData.BorrowRecords.Any(b =>
                b.StudentEmail == studentEmail &&
                b.EquipmentID == equip.EquipmentID &&
                b.Status == "Borrowed");

            if (alreadyBorrowed)
            {
                lblStudentMessage.Text = $"⚠️ You already have '{equip.Name}' borrowed. Please return it before requesting again.";
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
            LoadStudentDashboard();
        }

        protected void gvMyBorrows_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            string email = Session["LoggedInUser"].ToString();
            var myBorrows = AppData.BorrowRecords
                .Where(b => b.StudentEmail == email && b.Status == "Borrowed").ToList();

            if (rowIndex < 0 || rowIndex >= myBorrows.Count) return;
            var borrow = myBorrows[rowIndex];

            if (e.CommandName == "Return")
            {
                // Return the item
                borrow.Status = "Returned";
                borrow.ReturnDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                var equip = AppData.EquipmentList.FirstOrDefault(eq => eq.EquipmentID == borrow.EquipmentID);
                if (equip != null) equip.Quantity++;

                AddActivityLog(email, Session["UserName"].ToString(), "Student", "Return Item", $"Returned '{borrow.EquipmentName}'");

                lblStudentMessage.Text = $"✅ '{borrow.EquipmentName}' returned successfully.";
                lblStudentMessage.CssClass = "msg-success";
            }
            else if (e.CommandName == "Extend")
            {
                // Extend the borrow period by 7 days
                if (DateTime.TryParse(borrow.DueDate, out DateTime currentDueDate))
                {
                    DateTime newDueDate = currentDueDate.AddDays(7);
                    borrow.DueDate = newDueDate.ToString("yyyy-MM-dd");

                    AddActivityLog(email, Session["UserName"].ToString(), "Student", "Extend Borrow",
                        $"Extended '{borrow.EquipmentName}' until {borrow.DueDate}");

                    lblStudentMessage.Text = $"✅ '{borrow.EquipmentName}' due date extended to {borrow.DueDate}.";
                    lblStudentMessage.CssClass = "msg-success";
                }
                else
                {
                    lblStudentMessage.Text = $"❌ Could not extend '{borrow.EquipmentName}'. Invalid due date format.";
                    lblStudentMessage.CssClass = "msg-error";
                }
            }

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