using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;

public partial class ViewSettings : SalesBasePage
{
    private GlobalAppSettings settings;
    internal class SettingItem
    {
     public string Name { get; set; }
     public string Key { get; set; }
     public Object Value { get; set; }
     public string AppliedAt { get; set; }

    }
    List< SettingItem>  dataSource=new List<SettingItem>();
    public ViewSettings()
    {
        var storageHelper = new SalesTool.DataAccess.ApplicationStorageHelper("ApplicationServices");
        //settings = storageHelper.Load<SalesTool.DataAccess.GlobalAppSettings>();
        settings = HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings();
        foreach (var property in settings.GetType().GetProperties())
        {
            SettingItem item=new SettingItem();
            item.Name = property.Name;
            KeyValue keyValue = property.GetCustomAttributes(false).OfType<KeyValue>().FirstOrDefault();
            item.Key = keyValue != null ? keyValue.Key : property.Name;
            item.AppliedAt =keyValue!=null? keyValue.Mode.ToString():"";
            item.Value = property.GetValue(settings, null);
            dataSource.Add(item);
        }
        var arcSettings = storageHelper.Load<SalesTool.DataAccess.ArcGlobalSettings>();
        foreach (var property in arcSettings.GetType().GetProperties())
        {
            SettingItem item=new SettingItem();
            item.Name = property.Name;
            KeyValue keyValue = property.GetCustomAttributes(false).OfType<KeyValue>().FirstOrDefault();
            item.Key = keyValue != null ? keyValue.Key : property.Name;
            item.AppliedAt =keyValue!=null? keyValue.Mode.ToString():"";
            item.Value = property.GetValue(arcSettings, null);
            dataSource.Add(item);
        }

        var appSettings = storageHelper.Load<SalesTool.DataAccess.AppointmentGlobalSettings>();
        foreach (var property in appSettings.GetType().GetProperties())
        {
            SettingItem item=new SettingItem();
            item.Name = property.Name;
            KeyValue keyValue = property.GetCustomAttributes(false).OfType<KeyValue>().FirstOrDefault();
            item.Key = keyValue != null ? keyValue.Key : property.Name;
            item.AppliedAt =keyValue!=null? keyValue.Mode.ToString():"";
            item.Value = property.GetValue(appSettings, null);
            dataSource.Add(item);
        }
        
    }
    protected override void OnInit(EventArgs e)
    {
        if (!User.Identity.IsAuthenticated || User.Identity.Name != "admin@condado.com")
        {
            Response.Redirect(Konstants.K_LOGINPAGE, true);
        }
        base.OnInit(e);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        radGrid.DataSource = dataSource;//.OrderBy(s=>s.Name).ToString();
        radGrid.DataBind();
    }
}