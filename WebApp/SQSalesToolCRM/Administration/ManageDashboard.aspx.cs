using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Administration_ManageDashboard : SalesBasePage
{
    

    
    protected void Page_Load(object o, EventArgs args)
    {
        BindEvents();

        if (!IsPostBack)
            InitializeUI();
    }

    void InitializeUI()
    {
        ddlSections.DataSource = Engine.DashboadActions.All;
        ddlSections.DataBind();
        ddlSections.SelectedIndex = 0;
        BindGrid();
    }
    void BindEvents()
    {
        ddlSections.SelectedIndexChanged += (o, a) => BindGrid();
        grdAnnouncements.ItemCommand += (o, a) => CommandRouter(Convert.ToInt32(a.CommandArgument.ToString()), a.CommandName);

        btnOK.Click += (o, a) => { SaveRecord();ShowGrid(); }; 
        btnApply.Click += (o, a) => SaveRecord(); 
        btnClose.Click += (o, a) => ShowGrid();

        ctlPaging.IndexChanged+= (o, a) => BindGrid();
        ctlPaging.SizeChanged += (o, a) => BindGrid();
        ctlPaging.NewRecord += (o, a) => AddRecord();
 
    }
    void BindGrid()
    {
        int sectionId = 0;
        int.TryParse(ddlSections.SelectedValue, out sectionId);
        sectionId = sectionId < 1 ? 1 : sectionId;

        grdAnnouncements.DataSource = Engine.AnnouncementActions.GetAnnouncementsBySection(sectionId);
        grdAnnouncements.DataBind();
    }
    void CommandRouter(int id, string command)
    {
        switch (command)
        {
            case "xedit":
                EditRecord(id);
                break;
            case "xdelete":
                DeleteRecord(id);
                BindGrid();
                break;
        }
    }

    int RecordId
    {
        get
        {
            int Ans = 0;
            int.TryParse(hdnRecordId.Value, out Ans);
            return Ans;
        }
        set
        {
            hdnRecordId.Value = value.ToString();
        }
    }
    void DeleteRecord(int id)
    {
        try
        {
            Engine.AnnouncementActions.Delete(id);
            BindGrid();
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
        
    }
    void SaveRecord()
    {
        var record = RecordId < 1 ? new SalesTool.DataAccess.Models.Announcement() : Engine.AnnouncementActions.Get(RecordId);
        GetValues(ref record);

        try
        {
            if (RecordId < 1)
            {
                record.DateAdded = DateTime.Now;
                Engine.AnnouncementActions.Add(record);
            }
            else
            {
                Engine.AnnouncementActions.Change(record);
            }
            ctlStatus.SetStatus(Messages.RecordSavedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    void AddRecord()
    {
        ClearFields();
        chkEnabled.Checked = true; // SZ [Jul 24, 2013] added coz uses commonly dont check the checkbox and confused coz it disappears
        ShowGrid(false);
    }
    void EditRecord(int id)
    {
        var record = Engine.AnnouncementActions.Get(id);
        ClearFields();
        RecordId = record.Id;
        SetValues(record);
        ShowGrid(false);
    }
    void ClearFields()
    {
        RecordId = 0;
        txtName.Text = "";
        txtBody.Content = "";
       chkEnabled.Checked = false;
       ddlSectionForm.DataSource = Engine.DashboadActions.All;
       ddlSectionForm.DataBind();
       ddlSectionForm.SelectedIndex = ddlSections.SelectedIndex;
    }
    void SetValues(SalesTool.DataAccess.Models.Announcement record)
    {
        txtName.Text = record.Title;
        txtBody.Content = record.Body;
        chkEnabled.Checked = record.IsActive ?? false;
        ddlSectionForm.SelectedValue = record.SectionId.ToString();
    }
    void GetValues(ref SalesTool.DataAccess.Models.Announcement record)
    {
        record.Title = txtName.Text;
        record.Body = txtBody.Content;
         record.IsActive=chkEnabled.Checked;
        record.SectionId=Helper.SafeConvert<int>(ddlSectionForm.SelectedValue);
    }

    void ShowGrid(bool bShow = true)
    {
        dvForm.Visible = !bShow;
        dvGrid.Visible = bShow;
        if (bShow)
            BindGrid();

    }
    protected void grdAnnouncements_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is Telerik.Web.UI.GridDataItem)
        {
            var ctl = e.Item.FindControl("lnkDelete") as LinkButton;
            if (ctl != null && !CurrentUser.Security.Administration.CanDelete)
                ctl.Visible = false;
        }
    }
}