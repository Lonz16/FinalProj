using System;
using System.Linq;

namespace CanteenProject.Student
{
    public partial class MyBorrowings : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBorrowings();
            }
        }

        private void LoadBorrowings()
        {
            string studentEmail = Session["LoggedInUser"].ToString();

            // Active borrows
            gvActiveBorrows.DataSource = AppData.BorrowRecords
                .Where(b => b.StudentEmail == studentEmail && b.Status == "Borrowed")
                .ToList();
            gvActiveBorrows.DataBind();

            // History
            gvHistory.DataSource = AppData.BorrowRecords
                .Where(b => b.StudentEmail == studentEmail && b.Status == "Returned")
                .OrderByDescending(b => b.ReturnDate)
                .ToList();
            gvHistory.DataBind();
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

        protected void gvActiveBorrows_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
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

            lblMessage.Text = $"✅ '{borrow.EquipmentName}' returned successfully.";
            lblMessage.CssClass = "msg-success";
            LoadBorrowings();
        }
    }
}