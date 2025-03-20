using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Dashboard : SalesBasePage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        HtmlLink homeCSS = this.Master.FindControl("homeCss") as HtmlLink;
        homeCSS.Href = "Styles/HomeTable.css";
        //(ctlStatistics as IStatisticsMetricsNotifier).Register(ctlFilter as IStatisticsMetricsHandler);
    }
}