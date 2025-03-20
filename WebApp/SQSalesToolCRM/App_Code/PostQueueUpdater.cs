using System;
using System.Web.UI;
using System.Linq;
//using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Timers;
using SalesTool.DataAccess.Models;
using SalesTool.DataAccess;
using System.Data.SqlClient;
using SalesTool.Schema;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DBG = System.Diagnostics.Debug;

/// <summary>
/// This class will be responsible for updating the post_queue table and set the post queue records with Queued status
/// Further these records will be processed by the window service to send post(s) of the queued records.
/// </summary>
public class PostQueueUpdater : IDisposable
{
    //SZ [Apr 8, 2013] it is a self contained class now.
    DBEngine _E = null;
    
    #region Members/Properties

   

    #endregion

    private PostQueueUpdater()
    {        
        //SZ [Apr 8, 2013] This is added so that it is a complete self contaiend class
        _E = new DBEngine();
        _E.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
        //_E.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
        _E.Init(ApplicationSettings.ADOConnectionString);
    }
    public void Dispose()
    {
        //SZ [Apr 8, 2013] This has been added to cleanup the code
        if (_E != null)
        {
            _E.Dispose();
            _E = null;
        }
    }


    #region Methods
    public static void Execute(long CurrentAccountID = 0, int? CurrentActionID = 0, int? CurrentStatusID = 0, int? CurrentSubStatusID = 0)
    {
        System.Threading.Thread th = new System.Threading.Thread(() => _InnerExecute(CurrentAccountID, CurrentActionID, CurrentStatusID, CurrentSubStatusID));
        th.Name = string.Format("Post Queue Update{0}", DateTime.Now.Ticks.ToString());
        th.IsBackground = true;
        th.Start();
    }
    private static void _InnerExecute(long CurrentAccountID = 0, int? CurrentActionID = 0, int? CurrentStatusID = 0, int? CurrentSubStatusID = 0)
    {

        using (PostQueueUpdater tmp = new PostQueueUpdater())
        {
            DBG.WriteLine("Entering the Run Function at :" + DateTime.Now.ToString());
            tmp.Run(CurrentAccountID, CurrentActionID, CurrentStatusID, CurrentSubStatusID);
            DBG.WriteLine("Exiting the Run Function at :" + DateTime.Now.ToString());
        }
    }



    /// <summary>
    /// Starting point of setting the post queue records
    /// </summary>
    private void Run(long CurrentAccountID = 0, int? CurrentActionID = 0, int? CurrentStatusID = 0, int? CurrentSubStatusID = 0)
    {
        try
        {
            Dictionary<int, Post> templates = new Dictionary<int, Post>();

            RetrievePostTemplatesForAction(CurrentActionID ?? -1, ref templates);
            RetrievePostTemplatesForStatus(CurrentStatusID?? -1, ref templates);
            RetrievePostTemplatesForStatus(CurrentSubStatusID ?? -1, ref templates);
            
            TableStructure nTable = new TableStructure();
            //string query = ApplicationSettings.DefaultQuery;
            string query =_E. ApplicationSettings.DefaultQuery;
            List<long> accountKeyList = new List<long>();

            foreach (var item in templates.Values) // emailTemplateObjects)
            {
                using (CreateWhereClause nDynamicWhereClause = new CreateWhereClause(ref _E))
                {
                    query = nDynamicWhereClause.CreateDynamicWhereClause(query, item.Id, (short)Konstants.FilterParentType.Posts, item.FilterSelection == null ? (short)0 : item.FilterSelection.Value, item.FilterCustomValue);
                }
                query = query.Replace("*", "distinct accounts.act_key");                
                //DataTable dtRecords = nTable.GetDatatable(ApplicationSettings.ADOConnectionString, query);
                //string accountIDsList = "";
                //for (int i = 0; i < dtRecords.Rows.Count; i++)
                //{
                //    DataRow itemDataRow = dtRecords.Rows[i];
                //    long AccountID = Convert.ToInt64(itemDataRow["act_key"].ToString());
                //    if (!accountKeyList.Contains(AccountID)) accountKeyList.Add(AccountID);
                //    accountIDsList += AccountID.ToString() + ",";
                //}

                //SZ [Apr 10, 2013] this is an optimized version of the above. It performs the same function
                if (nTable.AccountExists(ApplicationSettings.ADOConnectionString, query, CurrentAccountID))
                {
                    //Below code refers to the settings tab in the Manage Post form.
                    bool OldQueueDelete = false;
                    OldQueueDelete = item.CancelUponStatus.HasValue ? item.CancelUponStatus.Value : false;
                    //If cancel upon status is true, then remove the old queue record.
                    if (OldQueueDelete)
                    {
                        var itemToDelete = _E.PostQueueActions.GetAll().Where(x => x.AccountKey == CurrentAccountID && x.PostTemplateKey == item.Id && x.MainStatusID == CurrentStatusID).FirstOrDefault();
                        if (itemToDelete != null) _E.PostQueueActions.Delete(itemToDelete.Key);
                    }
                    switch ((Konstants.DeliveryUnits)item.PostSend)
                    {
                        case Konstants.DeliveryUnits.SendImmediately:
                            _E.PostQueueActions.Add(CurrentAccountID, DateTime.Now, item.Id, (short)Konstants.PostQueueStatus.Queued, CurrentStatusID.HasValue ? CurrentStatusID.Value : 0);
                            break;
                        case Konstants.DeliveryUnits.SendAfterTrigger:
                            Konstants.PostDateUnits unitDelay = (Konstants.PostDateUnits)item.TriggerIncrementType;
                            double increment = (double)item.TriggerIncrement;
                            DateTime delayDateTime = DateTime.Now;
                            delayDateTime = SetDelayDateTime(unitDelay, increment, delayDateTime);
                            _E.PostQueueActions.Add(CurrentAccountID, delayDateTime, item.Id, (short)Konstants.PostQueueStatus.Queued, CurrentStatusID.HasValue ? CurrentStatusID.Value : 0);
                            break;
                        case Konstants.DeliveryUnits.SendBeforeOrAfterSpecificDate:
                            Konstants.PostDateUnits unitDelaySpecificDate = (Konstants.PostDateUnits)item.SpecificDateIncrementType;
                            double incrementSpecificDate = (double)item.SpecificDateIncrement;
                            DateTime delayDateTimeSpecificDate = DateTime.Now;
                            //To get the specific datetime value other than current, its the field tags having the Date fields datatype.
                            var filterFieldsValue = _E.TagFieldsActions.GetAll().Where(l => l.FilterDataType == (short)Konstants.FilterFieldDataType.Date && l.Id == item.SpecificDateField).FirstOrDefault();
                            if (filterFieldsValue !=null)
                            {
                                string queryRecord =_E. ApplicationSettings.DefaultQuery + " accounts.act_key = " + CurrentAccountID;
                                queryRecord = queryRecord.Replace("*", "distinct " + filterFieldsValue.FieldSystemName);                
                                DataTable dtDefaultRecords = nTable.GetDatatable(ApplicationSettings.ADOConnectionString, queryRecord);

                                // SZ [Nov 1, 2013] the code below is altered due to the requirement of only one iteration(fixed)
                                //for (int i = 0; i < dtDefaultRecords.Rows.Count; i++)
                                {
                                    int i=0;
                                    DataRow itemDataRow = dtDefaultRecords.Rows[i];
                                    string[] strFieldSystemName = filterFieldsValue.FieldSystemName.Split('.');
                                    DateTime nSelectedDateTime = DateTime.Now;
                                    if(DateTime.TryParse(itemDataRow[strFieldSystemName[strFieldSystemName.Count() - 1]].ToString(), out nSelectedDateTime))
                                        delayDateTimeSpecificDate = nSelectedDateTime;
                                    
                                    // break; SZ [Aug 22, 2013] Why break in the first iteration? break is not conditional so looping is redundent. 
                                    //delayDateTimeSpecificDate = Convert.ToDateTime(itemDataRow[strFieldSystemName[strFieldSystemName.Count() - 1]].ToString());
                                }    
                            }                            
                            //Check for Before or After the specific date time check, if value is true than before specific date is selected.
                            if (item.SpecificDateBeforeAfter.HasValue)
                            {
                                if (item.SpecificDateBeforeAfter.Value)
                                {
                                    incrementSpecificDate = -incrementSpecificDate;
                                }
                            }
                            delayDateTimeSpecificDate = SetDelayDateTime(unitDelaySpecificDate, incrementSpecificDate, delayDateTimeSpecificDate);
                            _E.PostQueueActions.Add(CurrentAccountID, delayDateTimeSpecificDate, item.Id, (short)Konstants.PostQueueStatus.Queued, CurrentStatusID.HasValue ? CurrentStatusID.Value : 0);
                            break;
                    }
                }
                //query = ApplicationSettings.DefaultQuery;
                query =_E. ApplicationSettings.DefaultQuery;
            }
        }
        catch (Exception ex)
        {
            while (ex.Message.Contains("inner exception for details"))
                ex = ex.InnerException;
            
        }
    }
    /// <summary>
    /// Add post templates attached to the action
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="templates"></param>
    private void RetrievePostTemplatesForAction(int Id, ref Dictionary<int, Post> templates)
    {
        foreach (var item in _E.LocalActions.GetEnabledPostTemplates(Id, false))
            if(!templates.ContainsKey(item.Id))
                templates.Add(item.Id, item);
    }
    /// <summary>
    /// Add post templates attached to the status
    /// </summary>
    /// <param name="Id">Account Status or substatus ID</param>
    /// <param name="templates">Post templates dictionary</param>
    private void RetrievePostTemplatesForStatus(int Id, ref Dictionary<int, Post> templates)
    {
        //Get post templates attached to the current status
        foreach (var item in _E.StatusActions.GetEnabledPostTemplates(Id, false))
        {
            StatusPost nStatusPost = _E.StatusActions.GetStatusPosts(Id, item.Id);
            if (nStatusPost != null && nStatusPost.TriggerType != null )
            {
                //Use the post templates whose trigger type are Auto or Both
                if ( (Konstants.PostTriggerType)nStatusPost.TriggerType == Konstants.PostTriggerType.Auto || (Konstants.PostTriggerType)nStatusPost.TriggerType == Konstants.PostTriggerType.Both)
                {
                    //If same post template already exists than ignore it
                    if(!templates.ContainsKey(item.Id))
                        templates.Add(item.Id, item);
                }
            }

        }     
    }

    /// <summary>
    /// This method will setup the delay datetime according to specified conditions
    /// </summary>
    /// <param name="unitDelay">Minutes, Hours, Days, Weeks</param>
    /// <param name="increment">Increment value</param>
    /// <param name="delayDateTime">Delay date time</param>
    /// <returns>Calculated Delay Date time</returns>
    private DateTime SetDelayDateTime(Konstants.PostDateUnits unitDelay, double increment, DateTime delayDateTime)
    {
        switch (unitDelay)
        {
            case Konstants.PostDateUnits.Minutes:
                delayDateTime = delayDateTime.AddMinutes(increment);
                break;
            case Konstants.PostDateUnits.Hours:
                delayDateTime = delayDateTime.AddHours(increment);
                break;
            case Konstants.PostDateUnits.Days:
                delayDateTime = delayDateTime.AddDays(increment);
                break;
            case Konstants.PostDateUnits.Weeks:
                delayDateTime = delayDateTime.AddDays(increment * 7);
                break;
        }
        return delayDateTime;
    }

    
    #endregion

}