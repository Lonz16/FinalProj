using System;
using System.Linq;

namespace CanteenProject.Admin
{
    public partial class ActivityLog : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadActivityLog();
            }
        }

        private void LoadActivityLog()
        {
            var logs = AppData.ActivityLogs.AsEnumerable();

            // Apply action filter
            string action = ddlActionType.SelectedValue;
            if (action != "All")
            {
                logs = logs.Where(l => l.Action == action);
            }

            gvActivityLog.DataSource = logs
                .OrderByDescending(l => l.LogID)
                .ToList();
            gvActivityLog.DataBind();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadActivityLog();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlActionType.SelectedIndex = 0;
            LoadActivityLog();
        }
    }
}