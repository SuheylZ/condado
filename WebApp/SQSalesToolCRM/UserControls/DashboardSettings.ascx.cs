using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalesTool.Web.UserControls
{
    public partial class DashboardSettings : HomeUserControl
    {
        
        protected override void InnerInit(bool bFirstTime)
        {

        }
        protected override void InnerLoad(bool bFirstTime)
        {
            if (bFirstTime)
                InitiaizeUI();

        }

        void InitiaizeUI()
        {
            tlkDashboardType.Items.Add(new Telerik.Web.UI.DropDownListItem("Agent", "1"));
            tlkDashboardType.Items.Add(new Telerik.Web.UI.DropDownListItem("Manager", "2"));
            tlkDashboardType.SelectedIndex = 0;
        }

        public void GetPermissions(ref SalesTool.DataAccess.Models.DashboardPermission dp)
        {
            dp.DashboardType = Convert.ToByte(tlkDashboardType.SelectedValue);
        }
        public void SetPermissions(SalesTool.DataAccess.Models.DashboardPermission prm)
        {
            tlkDashboardType.SelectedValue = prm.DashboardType.ToString();
        }

        public void Enable(bool bEnable = true)
        {
            tlkDashboardType.Enabled = bEnable;
        }
    }
}