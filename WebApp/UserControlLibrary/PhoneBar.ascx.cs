using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using UserControlLibrary.BaseControls;

namespace UserControlLibrary
{
    public partial class PhoneBar : UserControlBase
    {
        private HtmlGenericControl queueDiv;
        private HtmlGenericControl hotKeyDiv1;
        private HtmlGenericControl hotKeyDiv2;

        protected void Page_Load(object sender, EventArgs e)
        {
            queueDiv = new HtmlGenericControl("div");
        }
        protected override void WireControls(Control userControl)
        {
            queueDiv = (HtmlGenericControl)userControl.FindControl("divQueue");
            hotKeyDiv1 = (HtmlGenericControl)userControl.FindControl("hotKeyDiv1");
            hotKeyDiv2 = (HtmlGenericControl)userControl.FindControl("hotKeyDiv2");
        }
    }
}