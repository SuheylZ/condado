using System;
using System.Collections.Generic;
using System.Linq;


public class PagingEventArgs: EventArgs
{
    internal PagingEventArgs(int isize, int inumber)
    {
        pageSize = isize;
        pageNumber = inumber;
    }

    internal int pageNumber = 0;
    internal int pageSize=0;
    
    public int PageSize
    {
        get { return pageSize; }
    }
    public int PageNumber
    {
        get
        {
            return pageNumber;
        }
    }
}

public partial class UserControls_PagingBar : System.Web.UI.UserControl
{
    protected void Evt_Add_Clicked(object sender, EventArgs e)
    {
        if (NewRecord != null)
            NewRecord(this, new EventArgs());
    }

    public event EventHandler<PagingEventArgs> SizeChanged;
    public event EventHandler<PagingEventArgs> IndexChanged;
    public event EventHandler NewRecord;

    public override void Dispose()
    {
        base.Dispose();
        SizeChanged = null;
        IndexChanged = null;
        NewRecord = null;
    }
    #region Properties
    
    public int SkipRecords
    {
        get
        {
            int iSize = (RecordCount < PageSize) ? RecordCount : PageSize;
            return iSize * (PageNumber - 1);
        }
    }
    public int PageSize
    {
        get
        {
            int iSize = 0;
            bool bSuccess = int.TryParse(txtPageSize.Text, out iSize);
            if (!bSuccess)
            {
                iSize = 25;
                txtPageSize.Text = iSize.ToString();
            }
            iSize = iSize < 1 ? 1 : iSize > 100 ? 100 : iSize;
            txtPageSize.Text = iSize.ToString();
            return iSize;
        }
        set
        {
            txtPageSize.Text = (value < 1 ? 1 : value > 100 ? 100 : value).ToString();
        }
    }
    public int PageNumber
    {
        get
        {
            int iIndex = 0;
            bool bSuccess = int.TryParse(txtPageNumber.Text, out iIndex);
            if (!bSuccess)
            {
                iIndex = 1;
                txtPageNumber.Text = iIndex.ToString();
            }
            return iIndex;
        }
        set
        {
            txtPageNumber.Text = (value > MaxIndex ? MaxIndex : value < 1 ? 1 : value).ToString();
            lblTotalPages.Text = MaxIndex.ToString();
        }
    }
    public int RecordCount
    {
        get
        {
            int Count = 0;
            if (!int.TryParse(hdRecordCount.Value, out Count))
                Count = 0;
            return Count;
        }
        set
        {
            hdRecordCount.Value = (value<1? 1:value).ToString();
            lblTotalPages.Text = MaxIndex.ToString();
            lblCount.Text = string.Format("{0} Record(s) available", value.ToString());
        }
    }


    public void  SetRecordLabel(string strMessage)
    {
        lblCount.Text = string.Format("{0} {1}(s) available", RecordCount.ToString(), strMessage);        
    }

    public string NewButtonTitle
    {
        set { 
            if (value == string.Empty) 
                spnButton.Visible= false; 
            else btnAdd.Text = value; 
        }
    }

    public string SortBy
    {
        get
        {
            string sAns = string.Empty;
            sAns = hdSortColumnX.Value.ToString();
            return sAns;
        }
        set
        {
            hdSortColumnX.Value = value;
        }
    }
    public bool SortAscending
    {
        get
        {
            bool bAns = true;
            bool.TryParse(hdSortDirectionX.Value, out bAns);
            return bAns;
        }
        set {
            hdSortDirectionX.Value = value.ToString();
        }
    }

    private int MaxIndex
    {
        get
        {
            int iSize = PageSize;
            int iMaxIndex = (RecordCount / iSize);
            iMaxIndex = iMaxIndex < 1 ? 1 : iMaxIndex;
            if(RecordCount > iSize)
                iMaxIndex += (RecordCount%iSize)>0? 1:0;
            return iMaxIndex;
        }
    }
    #endregion

    #region Methods
    private PagingEventArgs CreatePagingEvent()
    {
        return new PagingEventArgs(PageSize, PageNumber);
    }
    
    public IEnumerable<object> ApplyPaging(IEnumerable<object> source)
    {
        RecordCount = source.Count();
        return source.Skip(SkipRecords).Take(PageSize);
    }

    public void ApplyPagingWithRecordCount(int sourceCount)
    {
        RecordCount = sourceCount;
    }

    public void Initialize(bool hidePageSize = false)
    {
        PageNumber = 1;
        if (!hidePageSize)
            PageSize = Profile.GridPaging.PageSize;
        else
        {
            PageSize = 1;
            pageSizeSpan.Visible = false;            
        }
    }
    #endregion

    #region Events

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if(!IsPostBack)
            Initialize();
    }

    
    protected void Evt_PageSizeChanged(object sender, EventArgs e)
    {
        PageNumber = 1;
        Profile.GridPaging.PageSize = PageSize;
        if (SizeChanged != null)
            SizeChanged(this, CreatePagingEvent());
         
    }
    
    protected void Evt_PreviousClicked(object sender, EventArgs e)
    {
        PageNumber -= 1;
        if (IndexChanged != null)
            IndexChanged(this, CreatePagingEvent());
    }
    protected void Evt_NextClicked(object sender, EventArgs e)
    {
        PageNumber += 1;
        if (IndexChanged != null)
            IndexChanged(this, CreatePagingEvent());
    }
    protected void Evt_PageNumberChanged(object sender, EventArgs e)
    {
        PageNumber = PageNumber; //Done to correct the input if wrong is typed
        if (IndexChanged != null)
            IndexChanged(this, CreatePagingEvent());
    }
    
    #endregion
}