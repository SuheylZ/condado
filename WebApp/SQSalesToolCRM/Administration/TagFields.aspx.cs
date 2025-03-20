using System;

using System.Linq;


public partial class Admin_TagFields : SalesBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            BindTagFieldsGrid();
        }
    }
    private void BindTagFieldsGrid()
    {
        try
        {
            var tagFields = Engine.TagFieldsActions.GetAll();
            var records = (from T in tagFields
                           where T.ShowInViewTags == true
                           select new { TagFieldKey = T.Id, TagFieldName = T.Name, TagFieldValue = T.TagDisplayName }).OrderBy(m => m.TagFieldName);
            grdTagFields.DataSource = records;
            grdTagFields.DataBind();
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error: " + ex.Message;
        }
    }
}