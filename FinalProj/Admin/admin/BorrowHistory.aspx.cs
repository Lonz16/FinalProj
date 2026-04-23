using System;
using System.Linq;

namespace CanteenProject.Admin
{
    public partial class BorrowHistory : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBorrowHistory();
            }
        }

        private void LoadBorrowHistory()
        {
            var records = AppData.BorrowRecords.AsEnumerable();

            // Apply status filter
            string status = ddlStatus.SelectedValue;
            if (status != "All")
            {
                records = records.Where(r => r.Status == status);
            }

            // Apply search filter
            string search = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                records = records.Where(r =>
                    r.StudentName.ToLower().Contains(search) ||
                    r.EquipmentName.ToLower().Contains(search));
            }

            gvBorrowHistory.DataSource = records
                .OrderByDescending(r => r.BorrowID)
                .ToList();
            gvBorrowHistory.DataBind();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadBorrowHistory();
        }
    }
}