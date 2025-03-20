using System;

using System.Linq;
using System.Linq.Dynamic;




public partial class UserControls_GridSorter: SalesUserControl 
{

    protected override void InnerInit()
    {
        SortBy = string.Empty;
        Ascending = false;
    }
    public string SortBy
    {
        get { return hdSortColumn.Value; }
        set { hdSortColumn.Value = value; }
    }
    public bool Ascending
    {
        get
        {
            bool bAns = false;
            bool.TryParse(hdSortDirection.Value, out bAns);
            return bAns;
        }
        set
        {
            hdSortDirection.Value = value.ToString();
        }
    }

    public IQueryable<object> ApplySorting(IQueryable<object> source)
    {
        return
            (SortBy == string.Empty) ?
            source : 
            source.OrderBy(SortBy + (Ascending? "" : " desc"));
    }
    public void Hook(Telerik.Web.UI.RadGrid grid)
    {
        throw new NotImplementedException();
    }
}