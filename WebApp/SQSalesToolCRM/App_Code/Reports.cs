using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/// <summary>
/// Implements functionality that helps display reports menu
/// </summary>
public interface IReportMenu
{
    void Add(string name, string url);
    int ReportCount { get; }
    void Clear();
}


public class ReportExporter
{
    public static string ToExcel(System.Web.UI.WebControls.WebControl view)
    {
        string Ans = string.Empty;
        using (System.IO.StringWriter sw = new System.IO.StringWriter())
        {
            using (System.Web.UI.HtmlTextWriter writer = new System.Web.UI.HtmlTextWriter(sw))
            {
                view.RenderControl(writer);
                Ans = sw.ToString();
            }
        }
        return Ans;
    }
    public static void SendDownload(System.Web.HttpResponse response, string content, string filename="report.xls")
    {
        response.ClearHeaders();
        response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);
        response.ContentType = "application/vnd.ms-excel";
        response.Write(content);
        response.End();
    }
}