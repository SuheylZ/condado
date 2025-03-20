using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SalesTool.DataAccess.Models;

//public enum ReportFormat { Unknown=0, Excel, Text };
//public enum EmailFrequency{Unknown=0, Now, Once, Daily, Weekly, Monthly};

//public class SalesToolEmailRecipient 
//{
//    public string Name{get;set;}
//    public Guid Id { get; set; }

//    public SalesToolEmailRecipient(Guid id, string name)
//    {
//        Name = name;
//        Id = id;
//    }

//    public SalesToolEmailRecipient(SalesTool.DataAccess.Models.User user)
//    {
//        Name = user.FullName;
//        Id = user.Key;
//    }
//}

//public struct EmailData{
//    public ReportFormat Format;
//    public List<SalesToolEmailRecipient> Recipients;
//    public string Subject;
//    public string Message;
//    public bool FilterByRole;
//    public EmailFrequency SendingFrequency;

//    public EmailData Init()
//    {
//    Format= ReportFormat.Unknown;
//    Recipients = new List<SalesToolEmailRecipient>();
//    Subject=string.Empty;
//    Message =string.Empty;
//    FilterByRole = false;
//    SendingFrequency= EmailFrequency.Now;
//    return this;
//    }
//};


/// <summary>
/// SZ [may 24, 2013] Provides functionality for the email for Condado project. this control is not AJAX compliant. If you put it in 
/// UpdatePanel, use PostBack trigger only otherwise the functionality is breaks.
/// </summary>
public partial class UserControls_EmailEditor : System.Web.UI.UserControl
{
    public void Clear()
    {
        lbAvailble.Items.Clear();
        lbChosen.Items.Clear();
        txtSubject.Text = string.Empty;
        txtMessage.Text = string.Empty;
        rbText.Checked = true;
        rbSendNow.Checked = true;
        chkFilter.Checked = false;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // SZ [May 24, 2013] This is an important code. Since the listbox is manipulated by the Jquery, 
        // The steps below need to be performed to maintain ASP.NEt integrity. otherwise the asp.net gets confused.
        if(IsPostBack){
            if (!string.IsNullOrEmpty(Request.Form[lbChosen.UniqueID]))
            {
                lbChosen.Items.Clear();
                foreach (var s in Request.Form[lbChosen.UniqueID].Split(','))
                {
                    var U = lbAvailble.Items.FindByValue(s);
                    if(U !=null)
                        lbChosen.Items.Add(U);
                }
            }
       }
        
    }

    public void AddUser(string Name, Guid id)
    {
        lbAvailble.Items.Add(new ListItem { Text = Name, Value = id.ToString() });
    }
    public void AddUser(SalesTool.DataAccess.Models.User user)
    {
        AddUser(user.FullName, user.Key);
    }
    public void AddUsers(IEnumerable<SalesTool.DataAccess.Models.User> users)
    {
        foreach (var u in users)
            AddUser(u.FullName, u.Key);
    }
    
    public EmailData EmailInformation
    {
        get
        {
            EmailData Ans = new EmailData();
            Ans.Init();

            Ans.Format = rbExcel.Checked? EReportFormat.Excel: rbText.Checked? EReportFormat.Text: EReportFormat.Unknown;

            //Set email people
            Ans.Recipients = GetRecipients();

            Ans.Subject = txtSubject.Text;
            Ans.Message=txtMessage.Text;
            Ans.FilterByRole = chkFilter.Checked;

            Ans.SendingFrequency = 
                rbSendNow.Checked? EmailFrequency.Now:
                rbSendOnce.Checked? EmailFrequency.Once:
                rbSendDaily.Checked? EmailFrequency.Daily:
                rbSendWeekly.Checked? EmailFrequency.Weekly:
                rbSendMonthly.Checked? EmailFrequency.Monthly:
                EmailFrequency.Unknown;

            return Ans;
        }
        set
        {
                switch (value.Format)
                {
                    case EReportFormat.Excel: rbExcel.Checked = true; break;
                    case EReportFormat.Text: rbText.Checked = true; break;
                }

                //Set email people
                lbChosen.Items.Clear();
                MoveRecipeints(lbAvailble, lbChosen, value.UserKeys);


                txtSubject.Text = value.Subject;
                txtMessage.Text = value.Message;
                chkFilter.Checked = value.FilterByRole;

                switch (value.SendingFrequency)
                {
                    case EmailFrequency.Now: rbSendNow.Checked = true; break;
                    case EmailFrequency.Once: rbSendOnce.Checked = true; break;
                    case EmailFrequency.Daily: rbSendDaily.Checked = true; break;
                    case EmailFrequency.Weekly: rbSendWeekly.Checked = true; break;
                    case EmailFrequency.Monthly: rbSendMonthly.Checked = true; break;
                }
            
        }
    }

    void MoveRecipeints(ListBox source, ListBox target, Guid[] users)
    {
        if(users!=null && source.Items.Count>0)
            foreach (Guid id in users)
            {
                ListItem li = source.Items.FindByValue(id.ToString());
                if (li != null)
                {
                    source.Items.Remove(li);
                    target.Items.Add(li);
                }
            }
    }

    //SZ [May 24, 2013] These functions retrive and set the values of recipients in the control
    List<SalesToolEmailRecipient> GetRecipients()
    {
        List<SalesToolEmailRecipient> Ans = new List<SalesToolEmailRecipient>();

        foreach (ListItem I in lbChosen.Items)
            Ans.Add(new SalesToolEmailRecipient(new Guid(I.Value), I.Text));

        return Ans;
    }
    void SetRecipients(List<SalesToolEmailRecipient> items)
    {
        foreach (var I in items)
        {
            if (lbChosen.Items.FindByValue(I.Id.ToString()) == null)
                lbChosen.Items.Add(new ListItem(I.Name, I.Id.ToString()));

            var x = lbAvailble.Items.FindByValue(I.Id.ToString());
            if(x!=null)
                lbAvailble.Items.Remove(x);
        }
    }
}