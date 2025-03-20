using System;
using System.Collections.Generic;
using System.Linq;
using DAL = SalesTool.DataAccess;

public partial class UserControls_Schedule : SalesUserControl
{
    Telerik.Web.UI.RadTimePicker[, ,] time = null;

    #region Members/Properties
    protected override void InnerInit()
    {
        InitilizeArray();
     
    }

    protected override void InnerLoad(bool bFirstTime)
    {
        InitilizeArray();
    }
    protected override void InnerPostBack()
    {
        
    }
    private void InitilizeArray()
    {
        // SZ [Dec  31, 2012] Added to optimize the code and remove the code redun
        // written by yasir a
        time = new Telerik.Web.UI.RadTimePicker[3, 7, 2];

        time[0, 0, 0] = tlSundayS1StartTime;
        time[0, 0, 1] = tlSundayS1EndTime;

        time[0, 1, 0] = tlMondayS1StartTime;
        time[0, 1, 1] = tlMondayS1EndTime;

        time[0, 2, 0] = tlTuesdayS1StartTime;
        time[0, 2, 1] = tlTuesdayS1EndTime;

        time[0, 3, 0] = tlWednesdayS1StartTime;
        time[0, 3, 1] = tlWednesdayS1EndTime;

        time[0, 4, 0] = tlThursdayS1StartTime;
        time[0, 4, 1] = tlThursdayS1EndTime;

        time[0, 5, 0] = tlFridayS1StartTime;
        time[0, 5, 1] = tlFridayS1EndTime;

        time[0, 6, 0] = tlSaturdayS1StartTime;
        time[0, 6, 1] = tlSaturdayS1EndTime;

        //Shift 2

        time[1, 0, 0] = tlSundayS2StartTime;
        time[1, 0, 1] = tlSundayS2EndTime;

        time[1, 1, 0] = tlMondayS2StartTime;
        time[1, 1, 1] = tlMondayS2EndTime;

        time[1, 2, 0] = tlTuesdayS2StartTime;
        time[1, 2, 1] = tlTuesdayS2EndTime;

        time[1, 3, 0] = tlWednesdayS2StartTime;
        time[1, 3, 1] = tlWednesdayS2EndTime;

        time[1, 4, 0] = tlThursdayS2StartTime;
        time[1, 4, 1] = tlThursdayS2EndTime;

        time[1, 5, 0] = tlFridayS2StartTime;
        time[1, 5, 1] = tlFridayS2EndTime;

        time[1, 6, 0] = tlSaturdayS2StartTime;
        time[1, 6, 1] = tlSaturdayS2EndTime;


        //Shift 3
        time[2, 0, 0] = tlSundayS3StartTime;
        time[2, 0, 1] = tlSundayS3EndTime;

        time[2, 1, 0] = tlMondayS3StartTime;
        time[2, 1, 1] = tlMondayS3EndTime;

        time[2, 2, 0] = tlTuesdayS3StartTime;
        time[2, 2, 1] = tlTuesdayS3EndTime;

        time[2, 3, 0] = tlWednesdayS3StartTime;
        time[2, 3, 1] = tlWednesdayS3EndTime;

        time[2, 4, 0] = tlThursdayS3StartTime;
        time[2, 4, 1] = tlThursdayS3EndTime;

        time[2, 5, 0] = tlFridayS3StartTime;
        time[2, 5, 1] = tlFridayS3EndTime;

        time[2, 6, 0] = tlSaturdayS3StartTime;
        time[2, 6, 1] = tlSaturdayS3EndTime;
    }



    public TimeSpan defaultTime = new TimeSpan(0, 0, 0);

    
    public void SetTime(DAL.SubType type, DayOfWeek day, bool isStart, TimeSpan? span)
    {
        // SZ [Dec  31, 2012] Added to optimize the code and remove the code redancy
        int itype = (int)type - 1;
        int iday = DayToInt(day);
        int iStart = isStart ? 0 : 1;

        time[itype, iday, iStart].SelectedTime = span;
    }
  
    private int DayToInt(DayOfWeek day)
    {
        int iday = day == DayOfWeek.Sunday ? 0 :
            day == DayOfWeek.Monday ? 1 :
            day == DayOfWeek.Tuesday ? 2 :
            day == DayOfWeek.Wednesday ? 3 :
            day == DayOfWeek.Thursday ? 4 :
            day == DayOfWeek.Friday ? 5 :
            6;
        return iday;
    }

    public TimeSpan? this[int i, int j, int k]
    {
        get
        {
            return time[i, j, k].SelectedTime;
        }
        set
        {
            time[i, j, k].SelectedTime = value;
        }
    }

    public TimeSpan? GetTime(SalesTool.DataAccess.SubType type, DayOfWeek day, bool isStart)
    {
        // SZ [Dec  31, 2012] Added to optimize the code and remove the code redun
        // written by yasir a

        int itype = (int)type - 1;
        int iday = DayToInt(day);

        int iStart = isStart ? 0 : 1;
        if (time[itype, iday, iStart].SelectedTime == null)
        {
            return null;
        }
        else
            return time[itype, iday, iStart].SelectedTime;
    }
   

    #endregion

    #region Methods

    protected void CopyValuesToAll(TimeSpan? spanS1TimeStart, TimeSpan? spanS1TimeEnd, TimeSpan? spanS2TimeStart, TimeSpan? spanS2TimeEnd, TimeSpan? spanS3TimeStart, TimeSpan? spanS3TimeEnd)
    {
        for (int j = 0; j < 7; j++)
        {
            time[0, j, 0].SelectedTime = spanS1TimeStart;
            time[0, j, 1].SelectedTime = spanS1TimeEnd;

            time[1, j, 0].SelectedTime = spanS2TimeStart;
            time[1, j, 1].SelectedTime = spanS2TimeEnd;

            time[2, j, 0].SelectedTime = spanS3TimeStart;
            time[2, j, 1].SelectedTime = spanS3TimeEnd;

        }        
    }

    public void ClearAllValues()
    {
        // SZ [Dec  31, 2012] Added to optimize the code and remove the code redun
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 7; j++)
                for (int k = 0; k < 2; k++)
                    time[i, j, k].SelectedTime = null;               
    }
    

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    // SZ [Dec 31, 2012] Created to initialize days
    private void InitializeDay(DayOfWeek day)
    {
        for (int i = 0; i < 3; i++)
            time[i, DayToInt(day), 0].SelectedTime = time[i, DayToInt(day), 1].SelectedTime = null;
    }
    protected void hlnkSundayClear_Click(object sender, EventArgs e)
    {
        InitializeDay(DayOfWeek.Sunday);

    }
    protected void hlnkSundayCopy_Click(object sender, EventArgs e)
    {
        CopyValuesToAll(GetTime(DAL.SubType.S1, DayOfWeek.Sunday, true), GetTime(DAL.SubType.S1, DayOfWeek.Sunday, false),
                        GetTime(DAL.SubType.S2, DayOfWeek.Sunday, true), GetTime(DAL.SubType.S2, DayOfWeek.Sunday, false),
                        GetTime(DAL.SubType.S3, DayOfWeek.Sunday, true), GetTime(DAL.SubType.S3, DayOfWeek.Sunday, false));
    }
    protected void hlnkMondayClear_Click(object sender, EventArgs e)
    {
        InitializeDay(DayOfWeek.Monday);
    }
    protected void hlnkMondayCopy_Click(object sender, EventArgs e)
    {
        CopyValuesToAll(GetTime(DAL.SubType.S1, DayOfWeek.Monday, true), GetTime(DAL.SubType.S1, DayOfWeek.Monday, false),
                        GetTime(DAL.SubType.S2, DayOfWeek.Monday, true), GetTime(DAL.SubType.S2, DayOfWeek.Monday, false),
                        GetTime(DAL.SubType.S3, DayOfWeek.Monday, true), GetTime(DAL.SubType.S3, DayOfWeek.Monday, false));
    }
    protected void hlnkTuesdayClear_Click(object sender, EventArgs e)
    {
        InitializeDay(DayOfWeek.Tuesday);
    }
    protected void hlnkTuesdayCopy_Click(object sender, EventArgs e)
    {
        CopyValuesToAll(GetTime(DAL.SubType.S1, DayOfWeek.Tuesday, true), GetTime(DAL.SubType.S1, DayOfWeek.Tuesday, false),
                         GetTime(DAL.SubType.S2, DayOfWeek.Tuesday, true), GetTime(DAL.SubType.S2, DayOfWeek.Tuesday, false),
                         GetTime(DAL.SubType.S3, DayOfWeek.Tuesday, true), GetTime(DAL.SubType.S3, DayOfWeek.Tuesday, false));
    }
    protected void hlnkWednesdayClear_Click(object sender, EventArgs e)
    {
        InitializeDay(DayOfWeek.Wednesday);
    }
    protected void hlnkWednesdayCopy_Click(object sender, EventArgs e)
    {
        CopyValuesToAll(GetTime(DAL.SubType.S1, DayOfWeek.Wednesday, true), GetTime(DAL.SubType.S1, DayOfWeek.Wednesday, false),
                         GetTime(DAL.SubType.S2, DayOfWeek.Wednesday, true), GetTime(DAL.SubType.S2, DayOfWeek.Wednesday, false),
                         GetTime(DAL.SubType.S3, DayOfWeek.Wednesday, true), GetTime(DAL.SubType.S3, DayOfWeek.Wednesday, false));
    }
    protected void hlnkThursdayClear_Click(object sender, EventArgs e)
    {
        InitializeDay(DayOfWeek.Thursday);
    }
    protected void hlnkThursdayCopy_Click(object sender, EventArgs e)
    {
        CopyValuesToAll(GetTime(DAL.SubType.S1, DayOfWeek.Thursday, true), GetTime(DAL.SubType.S1, DayOfWeek.Thursday, false),
                              GetTime(DAL.SubType.S2, DayOfWeek.Thursday, true), GetTime(DAL.SubType.S2, DayOfWeek.Thursday, false),
                              GetTime(DAL.SubType.S3, DayOfWeek.Thursday, true), GetTime(DAL.SubType.S3, DayOfWeek.Thursday, false));
    }
    protected void hlnkFridayClear_Click(object sender, EventArgs e)
    {
        InitializeDay(DayOfWeek.Friday);
    }
    protected void hlnkFridayCopy_Click(object sender, EventArgs e)
    {
        CopyValuesToAll(GetTime(DAL.SubType.S1, DayOfWeek.Friday, true), GetTime(DAL.SubType.S1, DayOfWeek.Friday, false),
                            GetTime(DAL.SubType.S2, DayOfWeek.Friday, true), GetTime(DAL.SubType.S2, DayOfWeek.Friday, false),
                            GetTime(DAL.SubType.S3, DayOfWeek.Friday, true), GetTime(DAL.SubType.S3, DayOfWeek.Friday, false));
    }
    protected void hlnkSaturdayClear_Click(object sender, EventArgs e)
    {
        InitializeDay(DayOfWeek.Saturday);
    }
    protected void hlnkSaturdayCopy_Click(object sender, EventArgs e)
    {
        CopyValuesToAll(GetTime(DAL.SubType.S1, DayOfWeek.Saturday, true), GetTime(DAL.SubType.S1, DayOfWeek.Saturday, false),
                              GetTime(DAL.SubType.S2, DayOfWeek.Saturday, true), GetTime(DAL.SubType.S2, DayOfWeek.Saturday, false),
                              GetTime(DAL.SubType.S3, DayOfWeek.Saturday, true), GetTime(DAL.SubType.S3, DayOfWeek.Saturday, false));
    }

    #endregion
  


   

   
}