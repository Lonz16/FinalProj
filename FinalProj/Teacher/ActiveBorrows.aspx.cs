using System;
using System.Linq;

namespace CanteenProject.Teacher
{
    public partial class ActiveBorrows : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBorrows();
            }
        }

        // ADD THIS METHOD - it was missing
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

        private void LoadBorrows()
        {
            // Active borrows
            var activeBorrows = AppData.BorrowRecords
                .Where(b => b.Status == "Borrowed")
                .OrderBy(b => b.DueDate)
                .ToList();
            gvActiveBorrows.DataSource = activeBorrows;
            gvActiveBorrows.DataBind();

            // Recently returned (last 10)
            var returnedHistory = AppData.BorrowRecords
                .Where(b => b.Status == "Returned")
                .OrderByDescending(b => b.ReturnDate)
                .Take(10)
                .ToList();
            gvReturnedHistory.DataSource = returnedHistory;
            gvReturnedHistory.DataBind();
        }

        protected void gvActiveBorrows_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
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

            string teacherEmail = Session["LoggedInUser"].ToString();
            string teacherName = Session["UserName"].ToString();
            AddActivityLog(teacherEmail, teacherName, "Teacher", "Mark Returned",
                           $"Marked '{borrow.EquipmentName}' returned by {borrow.StudentName}");

            lblMessage.Text = $"✅ '{borrow.EquipmentName}' marked as returned by {borrow.StudentName}.";
            lblMessage.CssClass = "msg-success";
            lblMessage.Visible = true;
            LoadBorrows();
        }
    }
}