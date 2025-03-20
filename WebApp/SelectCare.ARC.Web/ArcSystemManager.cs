// Be aware this class is intentionally using leadContext only by leaving adminContext and dashboard context 
// UNINITIALIZED.
// 

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Xml.Serialization;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using SelectCare.ARC.ArcRequest;


namespace SelectCare.ARC
{
    /// <summary>
    /// Arc response Statuses
    /// </summary>
    public class AcceptStatus
    {
        /// <summary>
        /// use for lead
        /// </summary>
        public const string Insert = "I";

        /// <summary>
        /// use for lead
        /// </summary>
        public const string Update = "U";

        /// <summary>
        /// user for lead, and update function
        /// </summary>
        public const string No = "N";

        /// <summary>
        /// for update functions
        /// </summary>
        public const string Yes = "Y";
    }
    public class ArcSystemManager : IDisposable
    {
        private const string ConsentLoggerName = "Consent";
        private const string InsertUpdateLeadLoggerName = "InsertUpdateLead";
        private const string CampaignLoggerName = "Campaign";
        private const string StatusLoggerName = "Status";
        private const string StopCommunicationLoggerName = "StopCommunication";
        private const string AcdCapLoggerName = "AcdCap";

        protected ArcServiceLogger Logger;

        protected DBEngine Engine = null;
        private bool _isDisposed;

        public string LocalTimeZoneId { get { return Engine.ApplicationSettings.ARC_SC_TIMEZONE_ID; } }
        /// <summary>
        /// Default Campaign Key for Arc received post
        /// </summary>
        public int DefaultCampaign
        {
            get { return Engine.ApplicationSettings.SC_ARC_DefaultPostCampaign; }
        }

        /// <summary>
        /// Default Lead Status for Arc Received post
        /// </summary>
        public int DefaultLeadStatus
        {
            get { return Engine.ApplicationSettings.SC_ARC_DefaultPostStatus; }
        }

        public bool HasLeadNewLayout { get { return Engine.ApplicationSettings.HasLeadNewLayout; } }

        /// <summary>
        /// login user for post
        /// </summary>
        public string Creator { get; set; }

        public string CurrentUniversalTime
        {
            get { return DateTime.Now.ToUniversalTime().ToString(ExtensionMethods.UtcDateFormat); }
        }
        /// <summary>
        /// Initialize Database Engine
        /// </summary>
        /// <param name="engine"></param>
        [Obsolete]
        public ArcSystemManager(DBEngine engine)
        {
            Engine = engine;
            Logger = new ArcServiceLogger();
            Logger.CanLog = Engine.ApplicationSettings.IsEnabledArcServiceLogging;
        }

        /// <summary>
        /// Initialize Database engine with given connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public ArcSystemManager(string connectionString)
        {
            Engine = new DBEngine();
            Engine.HookArcServiceChanges = true;
            // taking too much time..
            //_engine.Init(connectionString);
            Engine.InitLeadsContext(connectionString);
            Logger = new ArcServiceLogger();
            Logger.CanLog = Engine.ApplicationSettings.IsEnabledArcServiceLogging;
        }


        #region Core functionality

        /// <summary>
        /// Process the lead request and determine to insert or update
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ArcResponse ProcessInsertAndUpdate(ArcLeadRequest request)
        {
            var response = new ArcResponse();
            var leadResults = new List<ArcResponseResult>();

            var login = request.Login;
            if (login != null)
            {
                if (IsAuthenticatedPost(login.UserId, login.Password))
                {
                    Creator = login.UserId.Truncate(50);
                    foreach (var lead in request.Leads.OrderBy(p => p.Timestamp.ConvertUTCToLocal(LocalTimeZoneId)))
                    {
                        ArcResponseResult result = null;
                        if (!string.IsNullOrEmpty(lead.Reference))
                        {
                            Engine.ArcDateModified = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
                            //Summery: Arc-Reference has Top priority that update everything if it is found.(campanion refr will tie account to parent )
                            //Then Comes Indv_key of individual it will update(migrate)account to arc-case
                            //Then comes companion ref or companion key (Insert Scenario)
                            // Then comes individual matching rules to migrate account
                            //at last brand new account inserted if non of above get processed
                            var arcCase = Engine.ArcActions.Get(lead.Reference, true);
                            if (arcCase != null)
                            {
                                // process update
                                result = UpdateExistingCase(arcCase, lead);
                            }
                            else
                            {
                                // process for migration and insertion.
                                result = MigrateInsertLead(lead);
                            }
                        }
                        else
                        {
                            //lead reference missing
                        }
                        leadResults.Add(result);
                    }
                }
            }
            response.ResponseResults = leadResults.ToArray();
            if (request.Leads.Any())
                Logger.WriteLog(InsertUpdateLeadLoggerName, typeof(ArcLeadRequest), request, typeof(ArcResponse), response);
            return response;
        }

        /// <summary>
        ///  Set Marketing information to primary lead and update campaign key
        /// </summary>
        /// <param name="campaignRequest"></param>
        /// <returns></returns>
        public ArcResponse UpdateCampaign(ArcCampaignRequest campaignRequest)
        {
            var response = new ArcResponse();
            var leadResults = new List<ArcResponseResult>();
            var login = campaignRequest.Login;
            if (login != null)
            {
                string userName = login.UserId;
                string password = login.Password;
                if (IsAuthenticatedPost(userName, password))
                {
                    foreach (var lead in campaignRequest.Leads)
                    {
                        Engine.ArcDateModified = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
                        ArcResponseResult result = null;
                        if (!string.IsNullOrEmpty(lead.Reference))
                        {
                            var arcCases = Engine.ArcActions.Get(lead.Reference, true);
                            if (arcCases != null)
                            {
                                result = SetCampaign(arcCases, lead);
                                leadResults.Add(result);
                            }
                            else
                            {
                                // to meet exiting data.
                                Individual individual = Engine.ArcActions.GetByExternalReference(lead.Reference);
                                if (individual != null)
                                {
                                    arcCases = individual.arc_cases.FirstOrDefault();
                                    if (arcCases != null)
                                    {
                                        result = SetCampaign(arcCases, lead);
                                        leadResults.Add(result);
                                    }
                                    else
                                    {
                                        result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                                             ArcMessages.NotExistArcCase);
                                        leadResults.Add(result);
                                    }
                                }
                                else
                                {
                                    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                                              ArcMessages.NotExistArcCase);
                                    leadResults.Add(result);
                                }
                                // end to meet existing data
                                //result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                //                             ArcMessages.NotExistArcCase);
                                //leadResults.Add(result);
                            }
                        }
                        else
                        {
                            result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                   ArcMessages.MissingReferenceKey);
                            leadResults.Add(result);
                        }
                    }
                }
                else
                {
                    // not authenticated user
                    var result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.InvalidCredential);
                    leadResults.Add(result);
                }

            }
            else
            { // login missing
                var result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.InvalidCredential);
                leadResults.Add(result);
            }

            response.ResponseResults = leadResults.ToArray();
            if (campaignRequest.Leads.Any())
                Logger.WriteLog(CampaignLoggerName, typeof(ArcCampaignRequest), campaignRequest, typeof(ArcResponse), response);
            return response;
        }
        /// <summary>
        /// Set Status to ArcCase, create history, set at associated individual.
        /// </summary>
        /// <param name="statusRequest"></param>
        /// <returns></returns>
        public ArcResponse UpdateStatus(ArcStatusRequest statusRequest)
        {
            var response = new ArcResponse();
            var leadResults = new List<ArcResponseResult>();
            var login = statusRequest.Login;
            if (login != null)
            {
                string userName = login.UserId;
                string password = login.Password;
                if (IsAuthenticatedPost(userName, password))
                {
                    foreach (var lead in statusRequest.Leads)
                    {
                        Engine.ArcDateModified = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
                        ArcResponseResult result;
                        if (!string.IsNullOrEmpty(lead.Reference))
                        {
                            var arcCases = Engine.ArcActions.Get(lead.Reference, true);
                            if (arcCases != null)
                            {
                                string leadStatusReason;
                                result = ManageLeadStatus(arcCases, lead.Status, lead.Reference, out leadStatusReason, true);
                                Engine.ArcActions.Save(arcCases);
                                leadResults.Add(result);
                            }
                            else
                            {
                                // to meet exiting data.
                                Individual individual = Engine.ArcActions.GetByExternalReference(lead.Reference);

                                if (individual != null)
                                {
                                    arcCases = individual.arc_cases.FirstOrDefault();
                                    if (arcCases != null)
                                    {
                                        string leadStatusReason;
                                        result = ManageLeadStatus(arcCases, lead.Status, lead.Reference, out leadStatusReason, true, individual);
                                        Engine.ArcActions.Save(arcCases);
                                        leadResults.Add(result);
                                    }
                                    else
                                    {
                                        result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                                             ArcMessages.NotExistArcCase);
                                        leadResults.Add(result);
                                    }
                                }
                                else
                                {
                                    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                      ArcMessages.NotExistArcCase);
                                    leadResults.Add(result);
                                }

                                //result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                //  ArcMessages.NotExistArcCase);
                                //leadResults.Add(result);
                            }
                        }
                        else
                        {
                            result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                   ArcMessages.MissingReferenceKey);
                            leadResults.Add(result);
                        }
                    }
                }
                else
                {
                    // not authenticated user
                    var result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.InvalidCredential);
                    leadResults.Add(result);
                }

            }
            else
            { // login missing
                var result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.InvalidCredential);
                leadResults.Add(result);
            }

            response.ResponseResults = leadResults.ToArray();
            if (statusRequest.Leads.Any())
                Logger.WriteLog(StatusLoggerName, typeof(ArcStatusRequest), statusRequest, typeof(ArcResponse), response);
            return response;
        }

        /// <summary>
        /// 4.4 Update's IndividualEmailOptOut to request preferences.OptOut value
        /// </summary>
        /// <param name="communicationRequest"></param>
        /// <returns></returns>
        public ArcResponse UpdateStopCommunication(ArcStopCommunicationRequest communicationRequest)
        {
            var response = new ArcResponse();
            var leadResults = new List<ArcResponseResult>();
            var login = communicationRequest.Login;
            if (login != null)
            {
                string userName = login.UserId;
                string password = login.Password;
                if (IsAuthenticatedPost(userName, password))
                {
                    foreach (var lead in communicationRequest.Leads)
                    {
                        Engine.ArcDateModified = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
                        ArcResponseResult result = null;
                        if (!string.IsNullOrEmpty(lead.Reference))
                        {
                            var arcCases = Engine.ArcActions.Get(lead.Reference, true);
                            if (arcCases != null)
                            {
                                result = SetEmailCommunication(arcCases.ArcIndividual, lead.EmailCommunications, lead.Reference, arcCases.AccountKey, true);
                                Engine.ArcActions.Save(arcCases);
                                leadResults.Add(result);
                            }
                            else
                            {
                                // to meet exiting data.
                                Individual individual = Engine.ArcActions.GetByExternalReference(lead.Reference);

                                if (individual != null)
                                {
                                    arcCases = individual.arc_cases.FirstOrDefault();
                                    if (arcCases != null)
                                    {
                                        result = SetEmailCommunication(individual, lead.EmailCommunications, lead.Reference, arcCases.AccountKey, true);
                                        Engine.ArcActions.Save(arcCases);
                                        leadResults.Add(result);
                                    }
                                    else
                                    {
                                        result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                                             ArcMessages.NotExistArcCase);
                                        leadResults.Add(result);
                                    }
                                }
                                else
                                {
                                    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                      ArcMessages.NotExistArcCase);
                                    leadResults.Add(result);
                                }
                                //result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                //    ArcMessages.NotExistArcCase);
                                //leadResults.Add(result);
                            }
                        }
                        else
                        {
                            result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                 ArcMessages.MissingReferenceKey);
                            leadResults.Add(result);
                        }

                    }

                }
                else
                {
                    // not authenticated user
                    var result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.InvalidCredential);
                    leadResults.Add(result);
                }

            }
            else
            { // login missing
                var result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.InvalidCredential);
                leadResults.Add(result);
            }

            response.ResponseResults = leadResults.ToArray();
            if (communicationRequest.Leads.Any())
                Logger.WriteLog(StopCommunicationLoggerName, typeof(ArcStopCommunicationRequest), communicationRequest, typeof(ArcResponse), response);
            return response;

        }

        /// <summary>
        /// 4.5 Update Agent's ArcAcdCap based on  UserArcId
        /// </summary>
        /// <param name="crcAcdCapUpdateRequest"></param>
        /// <returns></returns>
        public ArcAgentResponse AcdCapUpdate(ArcAcdCapUpdateRequest crcAcdCapUpdateRequest)
        {
            ArcAgentResponse response = new ArcAgentResponse();
            List<ArcAgentResponseResult> agentResponseResults = new List<ArcAgentResponseResult>();
            var login = crcAcdCapUpdateRequest.Login;
            if (login != null)
            {
                string userId = login.UserId;
                string password = login.Password;
                if (IsAuthenticatedPost(userId, password))
                {
                    var time = DateTime.Now;
                    foreach (var requestAgent in crcAcdCapUpdateRequest.Agents)
                    {

                        ArcAgentResponseResult result = null;
                        string agentInitials = requestAgent.AgentInitials;
                        if (!string.IsNullOrEmpty(agentInitials))
                        {
                            var user = Engine.ArcActions.GetArcAgent(agentInitials);
                            if (user != null)
                            {
                                user.ArcAcdCap = requestAgent.AcdCap;
                                user.ChangedOn = time;
                                Engine.ArcActions.Save(user);
                                result = GetArcAgentResponseResult(AcceptStatus.Yes, agentInitials,
                                                                   null, time.ToUniversalTime());
                                agentResponseResults.Add(result);
                            }
                            else
                            {
                                // agent not found
                                result = GetArcAgentResponseResult(AcceptStatus.No, agentInitials,
                                                                   ArcMessages.NotExistUser);
                                agentResponseResults.Add(result);
                            }
                        }
                        else
                        {
                            // agent agentInitials missing
                            result = GetArcAgentResponseResult(AcceptStatus.No, agentInitials,
                                                               ArcMessages.MissingAgentInitial);
                            agentResponseResults.Add(result);
                        }
                    }
                }
                else
                {
                    // invalid post credentials
                    var result = GetArcAgentResponseResult(AcceptStatus.No, null, ArcMessages.InvalidCredential);
                    agentResponseResults.Add(result);
                }
            }
            else
            {
                var result = GetArcAgentResponseResult(AcceptStatus.No, null, ArcMessages.InvalidCredential);
                agentResponseResults.Add(result);
            }
            // Write responce
            response.ArcAgentResults = agentResponseResults.ToArray();
            if (crcAcdCapUpdateRequest.Agents.Any())
                Logger.WriteLog(AcdCapLoggerName, typeof(ArcAcdCapUpdateRequest), crcAcdCapUpdateRequest, typeof(ArcAgentResponse), response);
            return response;
        }

        /// <summary>
        /// Update Arc Consent 4.6
        /// </summary>
        /// <param name="consentRequest"></param>
        /// <returns></returns>
        public ArcResponse ConsentUpdate(ArcConsentUpdateRequest consentRequest)
        {

            var response = new ArcResponse();
            var leadResults = new List<ArcResponseResult>();
            var login = consentRequest.Login;
            if (login != null)
            {
                string userName = login.UserId;
                string password = login.Password;
                if (IsAuthenticatedPost(userName, password))
                {
                    foreach (var lead in consentRequest.Leads)
                    {
                        Engine.ArcDateModified = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
                        ArcResponseResult result = null;
                        TCPAConsentType consentType;
                        var arcCases = Engine.ArcActions.Get(lead.Reference, true);
                        if (arcCases != null)
                        {
                            result = GetConsent(arcCases.ArcIndividual, lead.Individual, out consentType, lead.Reference, null);
                            if (result == null)
                            {
                                if (consentType != TCPAConsentType.Undefined)
                                {
                                    if (arcCases.AccountKey != null)
                                    {
                                        Engine.AccountActions.SetConsent(arcCases.AccountKey.Value, consentType);
                                        // save phone information if new.
                                        Engine.ArcActions.Save(arcCases);
                                        result = GetArcLeadResult(AcceptStatus.Yes, lead.Reference, arcCases.AccountKey,
                                                                  null, indiv_key: arcCases.ArcIndividualKey);
                                    }
                                }
                                else
                                {
                                    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, arcCases.AccountKey,
                                                              ArcMessages.InvalidConsent);
                                }
                            }
                            //leadResults.Add(result);
                        }
                        else
                        {
                            // to meet exiting data.
                            Individual individual = Engine.ArcActions.GetByExternalReference(lead.Reference);
                            if (individual != null)
                            {
                                arcCases = individual.arc_cases.FirstOrDefault();
                                if (arcCases != null)
                                {
                                    result = GetConsent(individual, lead.Individual, out consentType, lead.Reference, null);
                                    if (result == null)
                                    {
                                        if (consentType != TCPAConsentType.Undefined)
                                        {
                                            if (arcCases.AccountKey != null)
                                            {
                                                Engine.AccountActions.SetConsent(arcCases.AccountKey.Value, consentType);
                                                // save phone information if new.
                                                Engine.ArcActions.Save(arcCases);
                                                result = GetArcLeadResult(AcceptStatus.Yes, lead.Reference, arcCases.AccountKey,
                                                                          null, indiv_key: arcCases.ArcIndividualKey);
                                            }
                                        }
                                        else
                                        {
                                            result = GetArcLeadResult(AcceptStatus.No, lead.Reference, arcCases.AccountKey,
                                                                      ArcMessages.InvalidConsent);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                                  ArcMessages.NotExistArcCase);
                                leadResults.Add(result);
                            }
                            //result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null,
                            //       ArcMessages.NotExistArcCase);

                        }
                        leadResults.Add(result);
                    }
                }
                else
                {
                    // not authenticated user
                    var result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.InvalidCredential);
                    leadResults.Add(result);
                }

            }
            else
            { // login missing
                var result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.InvalidCredential);
                leadResults.Add(result);
            }

            response.ResponseResults = leadResults.ToArray();
            if (consentRequest.Leads.Any())
                Logger.WriteLog(ConsentLoggerName, typeof(ArcConsentUpdateRequest), consentRequest, typeof(ArcResponse), response);
            return response;

        }

        #endregion

        #region InsertUpdateLead flow

        /// <summary>
        /// Update Lead request to existing case.
        /// </summary>
        /// <param name="arcCase">Case</param>
        /// <param name="lead">lead</param>
        /// <returns></returns>
        private ArcResponseResult UpdateExistingCase(ArcCases arcCase, ArcLeadRequestLead lead)
        {

            ArcResponseResult result = null;
            DateTime? changedAt = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
            if (lead.Individual == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingIndividualNode);
                return result;
            }
            if (lead.Individual.Contact == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingIndividualNode);
                return result;
            }
            if (lead.Status == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingStatusNode);
                return result;
            }
            //03 June 2014 Default Status
            //if (string.IsNullOrEmpty(lead.Status.Code))
            //{
            //    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingStatusNode);
            //    return result;
            //}
            //if (!string.IsNullOrEmpty(lead.Status.Code))
            //{
            //    // validate valid status.
            //    int? statusId = Engine.ArcActions.GetIndividualStatusId(lead.Status.Code);
            //    if (statusId == null)
            //    {
            //        result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.InvalidStatus);
            //        return result;
            //    }
            //}
            var primaryLead = arcCase.Account.PrimaryLead;
            SetPrimaryLead(primaryLead, lead);
            if (primaryLead.CampaignId == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.InvalidCampainKey);
                return result;
            }
            TCPAConsentType consentType = SetIndividual(arcCase.ArcIndividual, lead.Individual);

            //MH:23 April 2014 
            //Associate account with parent account fi campanion-reference and campanionIndvKey founds
            AssociateParentChild(arcCase.Account, lead);

            // update the external reference to latest.(will overwrite if individual is attached from companion reference or individual matching rule)
            arcCase.ArcIndividual.ExternalReferenceID = lead.Reference;

            SetEmailCommunication(arcCase.ArcIndividual, lead.EmailCommunications);
            SetPolicy(arcCase, lead);
            string leadStatusReason = "";
            ManageLeadStatus(arcCase, lead.Status, lead.Reference, out leadStatusReason, false, null);

            // if a Primary comes in with a consent of yes and the companion is a consent as no, use the yes as well.
            //
            TCPAConsentType consent = Engine.AccountActions.GetConsent((long)arcCase.AccountKey);
            if (arcCase.Account != null && arcCase.Account.PrimaryIndividual != null)
            {
                if (!string.IsNullOrEmpty(lead.CompanionReference))
                {
                    if (consent != TCPAConsentType.Yes)
                    {
                        if (consentType != TCPAConsentType.Undefined)
                        {
                            if (arcCase.AccountKey != null)
                                Engine.AccountActions.SetConsent(arcCase.AccountKey.Value, consentType);
                        }
                    }
                }
                else
                {
                    if (consentType != TCPAConsentType.Undefined)
                    {
                        if (arcCase.AccountKey != null)
                            Engine.AccountActions.SetConsent(arcCase.AccountKey.Value, consentType);
                    }
                }
            }

            Engine.ArcActions.Save(arcCase);
            result = GetArcLeadResult(AcceptStatus.Update, lead.Reference, arcCase.AccountKey, leadStatusReason, indiv_key: arcCase.ArcIndividual.Key);
            return result;
        }

        /// <summary>
        ///1. Search by companion reference and insert secondary or more individual, insert new ArcCase and associate individual, and update the associated Account,primary lead, consent
        ///2. search by matching rules, insert new ArcCase associate matched individual, and update the associated Account,primary lead, consent
        ///3. insert new record.
        /// </summary>
        /// <param name="lead"></param>
        /// <returns></returns>
        private ArcResponseResult MigrateInsertLead(ArcLeadRequestLead lead)
        {
            ArcResponseResult result = null;
            string companionReference = lead.CompanionReference;
            Account account = null;

            //MH:22 April 2014 handle new arc call scenario.
            if (lead.Indv_key.HasValue)
            {
                Individual individual;
                account = Engine.ArcActions.GetAccountbyIndiv_key(lead.Indv_key.Value, out individual);
                if (account != null)
                {
                    // create arcCase, arc history
                    result = UpdateAccountForArcCaseManagement(account, individual, lead, true);
                    return result;
                }
            }

            //1. Search by companion reference and insert secondary or more individual, insert new ArcCase and associate individual, and update the associated Account,primary lead, consent
            if (!string.IsNullOrEmpty(companionReference) || lead.CompanionIndvKey.HasValue)
            {

                account = Engine.ArcActions.GetAccountbyCampanionKey(lead.CompanionReference, lead.CompanionIndvKey);
                if (account != null)
                {
                    //1. Search by companion reference and insert secondary or more individual, insert new ArcCase and associate individual, and update the associated Account,primary lead, consent

                    //[27 Feb 2014] new requirements, 1 create new account, primary lead, individual, arc case, arc history 
                    result = HasLeadNewLayout
                                 ? InsertNewRecord(lead, account)
                                 : ProcessLeadUnderCompanionReference(account, lead);
                    return result;
                }
                else
                {
                    string s = String.Format("Critical: ARC-Lead is found which have campanion-reference but campanion is not found in SC, [Reference={0}] and [Companion-reference={1}]", lead.Reference, lead.CompanionReference);
                    LogCritical(null, s);
                }
            }

            //2. search by matching rules, insert new ArcCase associate matched individual, and update the associated Account,primary lead, consent
            Individual matchedIndividual;
            account = SearchAccountByMatchingIndividualRule(lead.Individual, out matchedIndividual);
            if (account != null)
            {
                result = UpdateAccountForArcCaseManagement(account, matchedIndividual, lead);
                return result;
            }

            //3. insert new record.
            result = InsertNewRecord(lead);
            return result;
        }



        /// <summary>
        /// 2. search by matching rules, insert new ArcCase associate matched individual, and update the associated Account,primary lead, consent
        /// </summary>
        /// <param name="account"></param>
        /// <param name="matchedIndividual"></param>
        /// <param name="lead"></param>
        /// <returns></returns>
        private ArcResponseResult UpdateAccountForArcCaseManagement(Account account, Individual matchedIndividual, ArcLeadRequestLead lead, bool isParent = false)
        {
            ArcResponseResult result;
            DateTime? changedAt = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
            if (lead.Individual == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingIndividualNode);
                return result;
            }
            if (lead.Individual.Contact == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingIndividualNode);
                return result;
            }
            if (lead.Status == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingStatusNode);
                return result;
            }
            //03 June 2014 Default Status
            //if (string.IsNullOrEmpty(lead.Status.Code))
            //{
            //    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingStatusNode);
            //    return result;
            //}
            //if (!string.IsNullOrEmpty(lead.Status.Code))
            //{
            //    // validate valid status.
            //    int? statusId = Engine.ArcActions.GetIndividualStatusId(lead.Status.Code);
            //    if (statusId == null)
            //    {
            //        result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.InvalidStatus);
            //        return result;
            //    }
            //}
            var createdAt = DateTime.Now;
            // update lead information
            SetPrimaryLead(account.PrimaryLead, lead);

            //if (account.PrimaryLead.CampaignId == null)
            //{
            //    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.InvalidCampainKey);
            //    return result;
            //}
            TCPAConsentType consentType = SetIndividual(matchedIndividual, lead.Individual);
            //Associate account with parent account fi campanion-reference and campanionIndvKey founds
            if (isParent)
                AssociateParentChild(account, lead);
            //IMPORTANT: overwrite the existing reference no.
            matchedIndividual.ExternalReferenceID = lead.Reference;
            SetEmailCommunication(matchedIndividual, lead.EmailCommunications);
            // create new case and individual 
            ArcCases arcCase = null;
            arcCase = account.ArcCases.FirstOrDefault(p => p.ArcRefreanceKey == lead.Reference);
            arcCase = arcCase ?? new ArcCases { AddOn = createdAt };
            //set policy node to arcCase
            SetPolicy(arcCase, lead);

            if (arcCase.IsNew())
            {
                // associate to individual
                matchedIndividual.arc_cases.Add(arcCase);
                account.ArcCases.Add(arcCase);
            }

            string leadStatusReason;
            ManageLeadStatus(arcCase, lead.Status, lead.Reference, out leadStatusReason);

            Engine.ArcActions.Save(account);
            // sync primary lead at case. 
            arcCase.LeadKey = account.PrimaryLeadKey;
            if (consentType != TCPAConsentType.Undefined)
            {
                Engine.AccountActions.SetConsent(account.Key, consentType);
            }
            result = GetArcLeadResult(AcceptStatus.Update, lead.Reference, account.Key, leadStatusReason, indiv_key: matchedIndividual.Key);
            Engine.ArcActions.Save(account);
            return result;
        }

        /// <summary>
        /// Associate account with parent account fi campanion-reference and campanionIndvKey founds
        /// </summary>
        /// <param name="account"></param>
        /// <param name="lead"></param>
        /// <author>MH:23 April</author>
        private void AssociateParentChild(Account account, ArcLeadRequestLead lead)
        {
            if ((!string.IsNullOrEmpty(lead.CompanionReference) || lead.CompanionIndvKey.HasValue))
            {
                if (account.AccountParent == null)
                {
                    bool hasParentAssigned;
                    var parentIndividual = Engine.ArcActions.GetIndividualByExternalRefOrByKey(lead.CompanionReference,
                                                                                                lead.CompanionIndvKey, out hasParentAssigned);
                    //MH: 17 June 2014 HasParentAssigned flag
                    if (parentIndividual != null && !hasParentAssigned)
                        account.AccountParent = parentIndividual.AccountId;
                }
            }
        }

        /// <summary>
        /// Search Account under individual matching rules 
        /// </summary>
        /// <param name="leadIndividual"></param>
        /// <param name="matchedIndividual"></param>
        /// <returns></returns>
        private Account SearchAccountByMatchingIndividualRule(ArcLeadIndividual leadIndividual, out Individual matchedIndividual)
        {
            if (leadIndividual != null)
            {
                var individualContact = leadIndividual.Contact;
                if (individualContact != null)
                {
                    string firstName = individualContact.FirstName;
                    string lastName = individualContact.LastName;
                    string email = "";
                    string address = "";
                    string gender = GetGenderInformation(individualContact.Gender);
                    if (individualContact.Email != null)
                    {
                        email = individualContact.Email;
                    }
                    if (!string.IsNullOrEmpty(individualContact.Address))
                    {
                        address = individualContact.Address;
                    }
                    DateTime dob = individualContact.BirthDate;
                    long? dayPhoneNumber = null;
                    long? eveningPhoneNumber = null;
                    long? cellNumber = null;


                    var dayPhone = individualContact.DayPhone;
                    if (dayPhone != null)
                    {
                        if (!string.IsNullOrEmpty(dayPhone.Number)) dayPhoneNumber = ConvertPhoneStringToLong(dayPhone.Number);
                    }
                    var eveningPhone = individualContact.EveningPhone;
                    if (eveningPhone != null)
                    {
                        if (!string.IsNullOrEmpty(eveningPhone.Number)) eveningPhoneNumber = ConvertPhoneStringToLong(eveningPhone.Number);
                    }

                    var cellPhone = individualContact.MobilePhone;
                    if (cellPhone != null)
                    {
                        if (!string.IsNullOrEmpty(cellPhone.Number)) cellNumber = ConvertPhoneStringToLong(cellPhone.Number);
                    }
                    var account = Engine.ArcActions.GetLeadBy(firstName, lastName, dob, dayPhoneNumber, eveningPhoneNumber, cellNumber, email,
                                                              address, gender, out matchedIndividual);
                    return account;
                }
            }
            matchedIndividual = null;
            return null;
        }

        /// <summary>
        /// 1. Search by companion reference and insert secondary or more individual, insert new ArcCase and associate individual, and update the associated Account,primary lead, consent
        /// </summary>
        /// <param name="account"></param>
        /// <param name="lead">Requested Lead</param>
        /// <returns></returns>
        private ArcResponseResult ProcessLeadUnderCompanionReference(Account account, ArcLeadRequestLead lead)
        {
            ArcResponseResult result;
            DateTime? changedAt = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
            if (lead.Individual == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingIndividualNode);
                return result;
            }
            if (lead.Individual.Contact == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingIndividualNode);
                return result;
            }
            if (lead.Status == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingStatusNode);
                return result;
            }
            //03 June 2014 Default Status
            //if (string.IsNullOrEmpty(lead.Status.Code))
            //{
            //    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingStatusNode);
            //    return result;
            //}
            //if (!string.IsNullOrEmpty(lead.Status.Code))
            //{
            //    // validate valid status.
            //    int? statusId = Engine.ArcActions.GetIndividualStatusId(lead.Status.Code);
            //    if (statusId == null)
            //    {
            //        result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.InvalidStatus);
            //        return result;
            //    }
            //}
            var createdAt = DateTime.Now;

            // update lead information
            SetPrimaryLead(account.PrimaryLead, lead);
            if (account.PrimaryLead.CampaignId == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.InvalidCampainKey);
                return result;
            }
            var individual = new Individual
            {
                AddedOn = createdAt,
                ChangedOn = changedAt,
                IsActive = true,
                IsDeleted = false,
                ExternalReferenceID = lead.Reference,
                AddedBy = Creator
            };

            TCPAConsentType consentType = SetIndividual(individual, lead.Individual);
            SetEmailCommunication(individual, lead.EmailCommunications);

            // associate individual with account.
            account.Individuals.Add(individual);

            // create new case and individual 
            var arcCase = new ArcCases { AddOn = createdAt };
            //set policy node to arcCase
            SetPolicy(arcCase, lead);

            // associate to individual
            individual.arc_cases.Add(arcCase);

            account.ArcCases.Add(arcCase);
            string leadStatusReason;
            ManageLeadStatus(arcCase, lead.Status, lead.Reference, out leadStatusReason);

            Engine.ArcActions.Save(account);
            if (account.SecondaryIndividualId == null)
            {
                account.SecondaryIndividualId = individual.Key;
            }

            // sync primary lead at case. 
            arcCase.LeadKey = account.PrimaryLeadKey;
            if (arcCase.Account != null && arcCase.Account.PrimaryIndividual != null)
            {
                // if a Primary comes in with a consent of yes and the companion is a consent as no, use the yes as well.
                TCPAConsentType consent = Engine.AccountActions.GetConsent((long)arcCase.AccountKey);
                if (consent != TCPAConsentType.Yes)
                {
                    if (consentType != TCPAConsentType.Undefined)
                    {
                        Engine.AccountActions.SetConsent(account.Key, consentType);
                    }
                }
            }

            result = GetArcLeadResult(AcceptStatus.Update, lead.Reference, account.Key, leadStatusReason, indiv_key: individual.Key);
            Engine.ArcActions.Save(account);
            return result;
        }

        /// <summary>
        /// Insert new Account and its association for lead, Assign parentAccount if available
        /// </summary>
        /// <param name="lead"></param>
        /// <param name="parentAccount"></param>
        /// <returns></returns>
        private ArcResponseResult InsertNewRecord(ArcLeadRequestLead lead, Account parentAccount = null)
        {
            ArcResponseResult result;
            //1. Create Account.
            //2. create primary lead.
            //3. create individual as primary
            //4. create Arc_table.
            DateTime? changedAt = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);
            if (lead.Individual == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingIndividualNode);
                return result;
            }
            if (lead.Individual.Contact == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingIndividualNode);
                return result;
            }
            if (lead.Status == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingStatusNode);
                return result;
            }
            //03 June 2014 Default Status
            //if (string.IsNullOrEmpty(lead.Status.Code))
            //{
            //    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingStatusNode);
            //    return result;
            //}
            //if (!string.IsNullOrEmpty(lead.Status.Code))
            //{
            //    // validate valid status.
            //    int? statusId = Engine.ArcActions.GetIndividualStatusId(lead.Status.Code);
            //    if (statusId == null)
            //    {
            //        result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.InvalidStatus);
            //        return result;
            //    }
            //}
            var createdAt = DateTime.Now;
            var account = new Account
            {
                AddedOn = createdAt,
                IsActive = true,
                IsDeleted = false,
                AddedBy = Creator,
                AccountParent = parentAccount != null ? parentAccount.Key : default(long?)
            };
            // copy from parent account
            if (parentAccount != null)
            {
                account.TransferUserKey = parentAccount.TransferUserKey;
                account.AssignedUserKey = parentAccount.AssignedUserKey;
                account.AssignedCsrKey = parentAccount.AssignedCsrKey;
                //30 May 2014
                //account.OriginalUserKey = parentAccount.OriginalUserKey ?? parentAccount.AssignedUserKey;
                account.LifeInfo = parentAccount.LifeInfo;
                account.Notes = parentAccount.Notes;
                account.ExternalAgent = parentAccount.ExternalAgent;
                account.NextEvenDate = parentAccount.NextEvenDate;
                account.IsActive = parentAccount.IsActive;
                account.IsDeleted = parentAccount.IsDeleted;
            }
            var primaryLead = new Lead
            {
                AddedOn = createdAt,
                ChangedOn = changedAt,
                IsActive = true,
                IsDeleted = false,
                AddedBy = Creator,
                //CampaignId = DefaultCampaign,
                //StatusId = DefaultLeadStatus
            };
            //copy information form parent accounts primary lead.
            if (parentAccount != null && parentAccount.PrimaryLead != null)
            {
                primaryLead.CampaignId = parentAccount.PrimaryLead.CampaignId;
                primaryLead.StatusId = parentAccount.PrimaryLead.StatusId;
            }

            if (primaryLead.StatusId == null)
                primaryLead.StatusId = DefaultLeadStatus;

            if (primaryLead.CampaignId == null)
                primaryLead.CampaignId = DefaultCampaign;
            //overriders default values with available values. 

            //Sets Marketing , Individual information related to lead (individual scoring information goes to lead), also sets status and campaign values.
            SetPrimaryLead(primaryLead, lead);
            //primaryLead.StatusId = SalesTool.Common.CGlobalStorage.Instance.Get<int?>("ARC_LEAD_DEFAULT_STATUSID");
            // campaignId as required field
            if (primaryLead.CampaignId == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.InvalidCampainKey);
                return result;
            }

            account.Leads.Add(primaryLead);

            var individual = new Individual
            {
                AddedOn = createdAt,
                ChangedOn = changedAt,
                IsActive = true,
                IsDeleted = false,
                ExternalReferenceID = lead.Reference,
                AddedBy = Creator
            };
            // Sets email communication related properties. 
            SetEmailCommunication(individual, lead.EmailCommunications);

            TCPAConsentType consentType = SetIndividual(individual, lead.Individual);
            account.Individuals.Add(individual);

            //manage status.
            var arcCase = new ArcCases { AddOn = createdAt };
            // associate to individual
            individual.arc_cases.Add(arcCase);
            // update arccase fields
            SetPolicy(arcCase, lead);

            // associate arc case with primary lead
            primaryLead.ArcCases.Add(arcCase);
            // associate case with account
            account.ArcCases.Add(arcCase);
            // save to get primary keys
            Engine.ArcActions.Add(account);
            //set primary individual, primary lead, and consent
            account.PrimaryLeadKey = primaryLead.Key;
            account.PrimaryIndividualId = individual.Key;
            Engine.ArcActions.Save(account);

            // Apply status , case, individualStatusID and create history
            string leadStatusReason;
            ManageLeadStatus(arcCase, lead.Status, lead.Reference, out leadStatusReason);
            Engine.ArcActions.Save(arcCase);
            // update consent
            if (consentType != TCPAConsentType.Undefined)
            {
                Engine.AccountActions.SetConsent(account.Key, consentType);
            }
            result = GetArcLeadResult(AcceptStatus.Insert, lead.Reference, account.Key, leadStatusReason, indiv_key: individual.Key);
            return result;
        }

        #endregion

        #region Setter funtions

        /// <summary>
        /// Apply latest status to Account.PrimaryLead.status ,individual status and creates entry in history table.
        ///  IMPORTANT NOTE:Call this method after ensuring case and individual association.
        /// </summary>
        /// <param name="arcCase">ARC CASE</param>
        /// <param name="status"></param>
        /// <param name="reference"></param>
        /// <param name="validateStatus"></param>
        /// <returns></returns>
        private ArcResponseResult ManageLeadStatus(ArcCases arcCase, ArcLeadStatus status, string reference, out string leadStatusReason, bool validateStatus = false, Individual individual = null)
        {
            leadStatusReason = "";
            ArcResponseResult result = null;
            if (status != null)
            {
                var code = status.Code;
                if (validateStatus)
                {
                    if (string.IsNullOrEmpty(code))
                    {
                        result = GetArcLeadResult(AcceptStatus.No, reference, arcCase.AccountKey,
                               ArcMessages.MissingStatusCode);
                        return result;
                    }
                }
                int? statusId = default(int?);
                if (!String.IsNullOrEmpty(code))
                {
                    statusId = Engine.ArcActions.GetIndividualStatusId(code);
                    if (validateStatus)
                    {
                        if (statusId == null)
                        {
                            result = GetArcLeadResult(AcceptStatus.No, reference, arcCase.AccountKey,
                                                      ArcMessages.InvalidStatus);
                            return result;
                        }
                    }

                    //For new entry put default lead status.
                    string statusCode = string.Format("{0}-{1}", code, status.SubStatus);
                    if (!string.IsNullOrEmpty(statusCode))
                    {
                        var subStatus = GetLeadStatusByTitle(statusCode);
                        if (subStatus != null)
                        {
                            if (arcCase != null && arcCase.Account != null && arcCase.Account.PrimaryLead != null)
                            {
                                arcCase.Account.PrimaryLead.StatusId = subStatus.Id;
                            }
                        }
                        else
                        {
                            leadStatusReason = string.Format("Lead Status [{0}] Not Found ", statusCode);
                        }
                    }
                }
                long? accountId = null;
                //default status.
                if (arcCase != null && arcCase.Account != null && arcCase.Account.PrimaryLead != null && arcCase.Account.PrimaryLead.StatusId == null)
                {
                    arcCase.Account.PrimaryLead.StatusId = DefaultLeadStatus;
                }
                DateTime? changedOn = status.LastUpdate.ConvertUTCToLocal(LocalTimeZoneId);
                // assigned agent.
                string agentId = Engine.ArcActions.GetArcAgentId(status.AgentInitials);
                AgentAssignment(arcCase, agentId);

                accountId = arcCase.IfNotNull(p => p.AccountKey) ?? individual.IfNotNull(p => p.AccountId);
                if (agentId == null)
                {
                    LogCritical(accountId, string.Format("Could not find mapping for agent initial [{0}] in SC for account [{1}]",status.AgentInitials,accountId));
                }
                AccountHistory policy = ManageAccountHistoryPolicy(status, accountId, agentId.AsGuid());
                if (policy != null)
                {
                    if (arcCase != null && arcCase.Account != null)
                        arcCase.Account.AccountHistory.Add(policy);
                    else
                    {

                    }
                }
                bool isStatusMissed = false;
                // statusId will be null if code is not found 
                if (statusId == null)
                {
                    isStatusMissed = true;
                    leadStatusReason = string.Format("Lead Status [{0}] Not Found with SubStatus [{1}] ", code, status.SubStatus);
                    code = Engine.ArcActions.GetIndividualStatusTitle(DefaultIndividualStatusId);
                    statusId = DefaultIndividualStatusId;

                }
                long? key = null;
                if (!string.IsNullOrEmpty(arcCase.Status) && isStatusMissed)
                {
                    // don't do anything becuase it may be situation when status was posted before but not now . so to avoit reset.
                }
                else
                {
                    if (arcCase.ArcHistory.Any(p => p.ChangedOn == changedOn && p.Status == code))
                    {
                        // dont make entry.
                    }
                    else
                    {
                        // make history in arc history table.
                        var history = new ArcHistory()
                            {
                                AddedOn = DateTime.Now,
                                Status = code,
                                ChangedOn = changedOn,
                                UserKey = agentId.AsGuid()
                            };
                        arcCase.ArcHistory.Add(history);
                    }



                    // update lead status
                    arcCase.Status = code;
                    arcCase.ModifyOn = changedOn;
                    if (individual != null)
                    {
                        // to meet old implementation.
                        individual.IndividualStatusID = statusId;
                        key = individual.Key;
                    }
                    else
                    {
                        // update to arc individual.
                        arcCase.ArcIndividual.IndividualStatusID = statusId;
                        key = arcCase.ArcIndividual.Key;
                    }
                }
                result = GetArcLeadResult(AcceptStatus.Yes, reference, arcCase.AccountKey, leadStatusReason, indiv_key: key);

                //result = GetArcLeadResult(AcceptStatus.No, lead.Reference, arcCase.AccountKey,
                //                                  ArcMessages.MissingStatusNode);
            }
            else
            {
                result = GetArcLeadResult(AcceptStatus.No, reference, arcCase.AccountKey,
                                                  ArcMessages.MissingStatusNode);
            }
            return result;
        }

        private AccountHistory ManageAccountHistoryPolicy(ArcLeadStatus status, long? accountid, Guid? agnetId)
        {
            AccountHistory policy = null;

            if (!string.IsNullOrEmpty(status.ExamVendor) && !string.IsNullOrEmpty(status.AppointmentType) && !string.IsNullOrEmpty(status.AppointmentDateTime))
            {
                string entry = string.Format("{0}-{1} ", status.Code, status.SubStatus);
                string comment = string.Format("Exam Vender: {0}; Appointment Type: {1}; Appointment Date: {2};",
                    status.ExamVendor, status.AppointmentType,
                    status.AppointmentDateTime.ConvertUTCToLocal(LocalTimeZoneId));

                var isExists = Engine.AccountHistory.CheckHistoryExistence(accountid, entry, 6, comment);
                if (!isExists)
                    policy = new AccountHistory()
                    {
                        AccountId = accountid,
                        EntryType = 6,
                        Datetime = DateTime.Now,
                        Entry = entry,
                        Comment = comment,
                        User = agnetId
                    };
            }

            return policy;
        }

        private void AgentAssignment(ArcCases arcCase, string agentId)
        {
            //[MH] 05 feb- 2014
            //if the value of act_assigned_usr = null, can you use the AgentInitials field to do a lookup in the Users table against ARC ID 
            //and use that user if ti is found?  
            //If no user is found, leave null?
            if (arcCase.Account != null && agentId.AsGuid() != null &&
                (arcCase.Account.AssignedUserKey == null || arcCase.Account.AssignedUserKey != agentId.AsGuid()))
            {
                if (HasLeadNewLayout)
                {
                    if (arcCase.Account.AssignedUserKey == null)
                        arcCase.Account.AssignedUserKey = agentId.AsGuid();

                    if (arcCase.Account.OriginalUserKey == null)
                        arcCase.Account.OriginalUserKey = agentId.AsGuid();
                    
                    if (!string.IsNullOrEmpty(agentId) && agentId.AsGuid()!=null)
                    {
                        // update all related accounts' assigned agents.
                        long accountId = arcCase.Account.AccountParent ?? arcCase.Account.Key;
                        Engine.AccountActions.UpdateAssignedAgentToAssociatedAccounts(accountId, agentId.AsGuid());
                    }
                }
                else
                {
                    if (arcCase.Account.AssignedUserKey == null)
                        arcCase.Account.AssignedUserKey = agentId.AsGuid();

                    if (arcCase.Account.OriginalUserKey == null)
                        arcCase.Account.OriginalUserKey = agentId.AsGuid();
                }
            }
        }

        /// <summary>
        /// Gets leads status by statusCode
        /// Data Access<see cref="SalesTool.DataAccess.Arc.ArcActions.GetLeadStatusByTitle"/>
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private SalesTool.DataAccess.Arc.StatusModel GetLeadStatusByTitle(string statusCode)
        {
            statusCode = statusCode.Replace(" ", "");
            return Engine.ArcActions.GetLeadStatusByTitle(statusCode.Trim());
            //commented due to use of admin context.
            //return _engine.StatusActions.All.FirstOrDefault(p => p.Title.Trim().Replace(" ", "") == statusCode.Trim());
        }

        /// <summary>
        /// Set EmailCommunication values to Individual
        /// </summary>
        /// <param name="individual"></param>
        /// <param name="emailCommunications"></param>
        /// <param name="reference"></param>
        /// <param name="accountId"></param>
        /// <param name="validate"></param>
        private ArcResponseResult SetEmailCommunication(Individual individual,
            ArcLeadEmailCommunications emailCommunications, string reference = null, long? accountId = null, bool validate = false)
        {
            ArcResponseResult result = null;
            if (validate)
            {
                if (individual == null)
                {
                    result = GetArcLeadResult(AcceptStatus.No, reference, accountId, ArcMessages.NotExistAssocatedIndividual);
                    return result;
                }
                if (emailCommunications == null)
                {
                    result = GetArcLeadResult(AcceptStatus.No, reference, accountId, ArcMessages.MissingCommunicationNode);
                    return result;
                } if (emailCommunications.Preferences == null)
                {
                    result = GetArcLeadResult(AcceptStatus.No, reference, accountId, ArcMessages.MissingCommunicationPreferencesNode);
                    return result;
                }
                if (string.IsNullOrEmpty(emailCommunications.Preferences.OptOut))
                {
                    result = GetArcLeadResult(AcceptStatus.No, reference, accountId, ArcMessages.MissingEmailOptOut);
                    return result;
                }
            }

            if (emailCommunications != null)
            {
                var preferences = emailCommunications.Preferences;
                if (preferences != null)
                {
                    string optOut = preferences.OptOut;
                    individual.IndividualEmailOptOut = optOut == "Y";
                    if (preferences.LastUpdate != null)
                    {
                        DateTime? changedAt = preferences.LastUpdate.ConvertUTCToLocal(LocalTimeZoneId);
                        if (changedAt != null)
                            individual.ChangedOn = changedAt;
                    }
                    result = GetArcLeadResult(AcceptStatus.Yes, reference, accountId, null, indiv_key: individual.Key);
                }
            }
            return result;
        }

        /// <summary>
        /// Set Policy node to arc Case 
        /// </summary>
        /// <param name="arcCase"></param>
        /// <param name="arcLead"></param>
        private void SetPolicy(ArcCases arcCase, ArcLeadRequestLead arcLead)
        {
            var timestampString = arcLead.Timestamp;
            DateTime? timeStamp = timestampString.ConvertUTCToLocal(LocalTimeZoneId);

            arcCase.ModifyOn = timeStamp;
            arcCase.ArcRefreanceKey = arcLead.Reference;


            // Policy data goes to ArcCases.
            var policy = arcLead.Policy;
            if (policy != null)
            {
                if (policy.FaceAmount != null)
                    arcCase.FaceAmount = policy.FaceAmount;

                if (policy.Duration != null)
                    arcCase.Duration = policy.Duration;

                if (policy.CompanyCode != null)
                    arcCase.CompanyCode = policy.CompanyCode;

                var specialist = policy.CaseSpecialist;
                if (specialist != null)
                {
                    if (specialist.Name != null) arcCase.CaseSpecialistName = specialist.Name;
                    if (specialist.Ext != null) arcCase.CaseSpecialistExt = specialist.Ext;
                }
                if (!string.IsNullOrEmpty(policy.Note))
                {
                    arcCase.Notes = policy.Note;
                }
            }
        }

        /// <summary>
        /// Helper method to create LeadResponse object
        /// </summary>
        /// <param name="acceptState">AcceptStatus.Member</param>
        /// <param name="reference">Lead reference</param>
        /// <param name="accountId">Account Key of exist</param>
        /// <param name="reason">Reason if failed</param>
        /// <param name="timeStamp"></param>
        /// <returns>Response Result</returns>
        private ArcResponseResult GetArcLeadResult(string acceptState, string reference, long? accountId, string reason, DateTime? timeStamp = null, long? indiv_key = null)
        {
            ArcResponseResult responseResult = new ArcResponseResult();
            if (!string.IsNullOrEmpty(reason)) responseResult.Reason = reason;
            if (accountId != null && accountId != 0) responseResult.AccountID = accountId.Value;
            if (!string.IsNullOrEmpty(reference)) responseResult.Reference = reference;
            if (!string.IsNullOrEmpty(acceptState)) responseResult.Accepted = acceptState;
            if (indiv_key.HasValue)
                responseResult.Indv_key = indiv_key;
            if (timeStamp.HasValue)
                responseResult.Timestamp = timeStamp.Value.ToUniversalTime().ToString(ExtensionMethods.UtcDateFormat);
            else
                responseResult.Timestamp = DateTime.Now.ToUniversalTime().ToString(ExtensionMethods.UtcDateFormat);
            return responseResult;
        }

        /// <summary>
        /// Helper method to create AgentResponse result
        /// </summary>
        /// <param name="acceptState"></param>
        /// <param name="initialAgent"></param>
        /// <param name="reason"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private ArcAgentResponseResult GetArcAgentResponseResult(string acceptState = null, string initialAgent = null, string reason = null, DateTime? timeStamp = null)
        {
            ArcAgentResponseResult responseResult = new ArcAgentResponseResult();
            if (!string.IsNullOrEmpty(reason)) responseResult.Reason = reason;
            if (!string.IsNullOrEmpty(initialAgent)) responseResult.AgentInitials = initialAgent;
            if (!string.IsNullOrEmpty(acceptState)) responseResult.Accepted = acceptState;
            if (timeStamp != null)
                responseResult.Timestamp = timeStamp.Value.ToUniversalTime().ToString(ExtensionMethods.UtcDateFormat);
            else
                responseResult.Timestamp = DateTime.Now.ToUniversalTime().ToString(ExtensionMethods.UtcDateFormat);
            return responseResult;
        }

        /// <summary>
        /// Set Markings and Individual Scoring information to Primary lead.
        /// Sets Lead status and campaign as well
        /// </summary>
        /// <param name="primaryLead">Primary Lead</param>
        /// <param name="lead">Lead Request Method</param>
        private void SetPrimaryLead(Lead primaryLead, ArcLeadRequestLead lead)
        {
            if (primaryLead != null)
            {
                // Marketing and Scoring information nodes goes to lead.
                var marketing = lead.Marketing;
                if (marketing != null)
                {
                    if (marketing.SourceCode != null) primaryLead.SourceCode = marketing.SourceCode;
                    if (marketing.IpAddress != null) primaryLead.IPAddress = marketing.IpAddress;
                    if (marketing.TrackingCode != null) primaryLead.TrackingCode = marketing.TrackingCode;

                    if (marketing.Campaign != null)
                    {
                        int? campaignById = GetCampaignById(marketing.Campaign);
                        if (campaignById != null)
                        {
                            // do not reset to null.
                            primaryLead.CampaignId = campaignById;
                        }
                    }
                }
                // setting default campaign
                if (primaryLead.CampaignId == null)
                {
                    primaryLead.CampaignId = DefaultCampaign;
                }

                //For new entry put default lead status.
                if (lead.Status != null)
                {
                    if (!string.IsNullOrEmpty(lead.Status.Code))
                    {
                        string statusCode = string.Format("{0}-{1}", lead.Status.Code, lead.Status.SubStatus);
                        if (!string.IsNullOrEmpty(statusCode))
                        {
                            var subStatus = GetLeadStatusByTitle(statusCode);
                            if (subStatus != null)
                            {
                                primaryLead.StatusId = subStatus.Id;
                            }
                        }
                    }

                }
                if (primaryLead.StatusId == null)
                {
                    // put default status
                    primaryLead.StatusId = DefaultLeadStatus;
                }

                // individual scoring information goes to lead
                var individual = lead.Individual;
                if (individual != null)
                {
                    var scoringInformation = individual.ScoringInformation;
                    if (scoringInformation != null)
                    {
                        if (scoringInformation.Substatus != null) primaryLead.ArcSubStatus = scoringInformation.Substatus;
                        primaryLead.ArcEScore = scoringInformation.eScore;
                        primaryLead.ArcFraudScore = scoringInformation.FraudScore;
                        primaryLead.ArcTier = scoringInformation.Tier;
                    }
                }
            }
        }

        /// <summary>
        /// Set individual properties from leadIndividual request
        /// doesn't update external reference and individual state
        /// </summary>
        /// <param name="individual">Individual</param>
        /// <param name="leadIndividual">ArcIndividual </param>
        private TCPAConsentType SetIndividual(Individual individual, ArcLeadIndividual leadIndividual)
        {
            var arcContact = leadIndividual.Contact;
            if (arcContact != null)
            {
                if (arcContact.FirstName != null) individual.FirstName = arcContact.FirstName;
                if (arcContact.LastName != null) individual.LastName = arcContact.LastName;

                if (arcContact.AppState != null)
                    individual.ApplicationState = GetStateId(arcContact.AppState);

                individual.Birthday = arcContact.BirthDate;

                if (arcContact.Address != null) individual.Address1 = arcContact.Address;

                if (arcContact.City != null) individual.City = arcContact.City;
                if (arcContact.State != null)
                    individual.StateID = (byte?)GetStateId(arcContact.State);
                if (arcContact.Zip != null)
                    individual.Zipcode = arcContact.Zip;
                bool isConcent = false;
                // Day phone
                var dayPhone = arcContact.DayPhone;
                if (dayPhone != null)
                {
                    if (!string.IsNullOrEmpty(dayPhone.Number))
                    {
                        individual.DayPhone = ConvertPhoneStringToLong(dayPhone.Number);
                        isConcent = (dayPhone.Consent == "Y");
                    }
                }

                // evening phone
                var arcEvenPh = arcContact.EveningPhone;
                if (arcEvenPh != null)
                {
                    if (!string.IsNullOrEmpty(arcEvenPh.Number))
                    {
                        individual.EveningPhone = ConvertPhoneStringToLong(arcEvenPh.Number);
                        isConcent = (arcEvenPh.Consent == "Y" || isConcent);
                    }
                }
                // mobile phone
                var arcMobilePh = arcContact.MobilePhone;
                if (arcMobilePh != null)
                {
                    if (!string.IsNullOrEmpty(arcMobilePh.Number))
                    {
                        individual.CellPhone = ConvertPhoneStringToLong(arcMobilePh.Number);
                        isConcent = (arcMobilePh.Consent == "Y" || isConcent);
                    }
                }
                //[31 Dec 2013]
                // if mobile is null then put any if day or evening phone that is marked as cell
                if (arcMobilePh == null || string.IsNullOrEmpty(arcMobilePh.Number))
                {
                    bool isApplied = false;
                    if (dayPhone != null)
                    {
                        if (dayPhone.IsMobile == "Y" && !string.IsNullOrEmpty(dayPhone.Number))
                        {
                            individual.CellPhone = ConvertPhoneStringToLong(dayPhone.Number);
                            isApplied = true;
                        }
                    }
                    if (!isApplied && arcEvenPh != null)
                    {
                        if (arcEvenPh.IsMobile == "Y" && !string.IsNullOrEmpty(arcEvenPh.Number))
                        {
                            individual.CellPhone = ConvertPhoneStringToLong(arcEvenPh.Number);
                        }
                    }

                }

                individual.Email = arcContact.Email;
                individual.Gender = GetGenderInformation(arcContact.Gender);

                // Existing Insurance goes to new fields of individual
                var existingInsurance = leadIndividual.ExistingInsurance;
                if (existingInsurance != null)
                {
                    individual.indv_exist_ins = existingInsurance.Insurance;
                    individual.indv_exist_ins_amt = existingInsurance.Amount;
                    individual.indv_exist_ins_rplc = existingInsurance.WantsToReplace;
                }
                // Desire Insurance goes to new fields of individual
                var desireInsurance = leadIndividual.DesiredInsurance;
                if (desireInsurance != null)
                {
                    individual.indv_desire_ins_amt = desireInsurance.Amount;
                    individual.indv_desire_ins_alt_amt = desireInsurance.AlternateAmount;
                    int termDeration = 0;
                    if (int.TryParse(desireInsurance.TermDuration, out termDeration))
                    {
                        individual.indv_desire_ins_term = termDeration;
                    }
                }

                return isConcent ? TCPAConsentType.Yes : TCPAConsentType.No;
            }
            return TCPAConsentType.Undefined;
        }

        /// <summary>
        /// Get Consent information
        /// </summary>
        /// <param name="arcIndividual"></param>
        /// <param name="individual"></param>
        /// <param name="consentType"></param>
        /// <param name="reference"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private ArcResponseResult GetConsent(Individual arcIndividual, ArcLeadIndividual individual, out TCPAConsentType consentType, string reference, long? accountId)
        {
            ArcResponseResult result = null;
            if (individual == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, reference, accountId, ArcMessages.MissingIndividualNode);
                consentType = TCPAConsentType.Undefined;
                return result;
            }
            var contact = individual.Contact;
            if (contact == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, reference, accountId, ArcMessages.MissingIndividualNode);
                consentType = TCPAConsentType.Undefined;
                return result;

            }
            bool? isConcent = null;
            // Day phone
            var dayPhone = contact.DayPhone;
            if (dayPhone != null)
            {
                if (!string.IsNullOrEmpty(dayPhone.Number))
                {

                    long? dayPhoneNumber = ConvertPhoneStringToLong(dayPhone.Number);
                    if (dayPhoneNumber != null)
                    {
                        arcIndividual.DayPhone = dayPhoneNumber;
                    }
                    isConcent = (dayPhone.Consent == "Y");
                }
            }

            // evening phone
            var arcEvenPh = contact.EveningPhone;
            if (arcEvenPh != null)
            {
                if (!string.IsNullOrEmpty(arcEvenPh.Number))
                {
                    long? dayPhoneNumber = ConvertPhoneStringToLong(arcEvenPh.Number);
                    if (dayPhoneNumber != null)
                    {
                        arcIndividual.EveningPhone = dayPhoneNumber;
                    }
                    isConcent = (arcEvenPh.Consent == "Y" || Convert.ToBoolean(isConcent));
                }
            }

            // mobile phone
            var arcMobilePh = contact.MobilePhone;
            if (arcMobilePh != null)
            {
                if (!string.IsNullOrEmpty(arcMobilePh.Number))
                {
                    long? mobileNumber = ConvertPhoneStringToLong(arcMobilePh.Number);
                    if (mobileNumber != null)
                    {
                        arcIndividual.CellPhone = mobileNumber;
                    }
                    isConcent = (arcMobilePh.Consent == "Y" || Convert.ToBoolean(isConcent));
                }
            }
            //[31 Dec 2013]
            // if mobile is null then put any if day or evening phone that is marked as cell
            if (arcMobilePh == null)
            {
                bool isApplied = false;
                if (dayPhone != null)
                {
                    if (dayPhone.IsMobile == "Y" && !string.IsNullOrEmpty(dayPhone.Number))
                    {
                        arcIndividual.CellPhone = ConvertPhoneStringToLong(dayPhone.Number);
                        isApplied = true;
                    }
                }
                if (!isApplied && arcEvenPh != null)
                {
                    if (arcEvenPh.IsMobile == "Y" && !string.IsNullOrEmpty(arcEvenPh.Number))
                    {
                        arcIndividual.CellPhone = ConvertPhoneStringToLong(arcEvenPh.Number);
                    }
                }

            }
            //consentType = isConcent ? TCPAConsentType.Yes : TCPAConsentType.No;
            if (isConcent != null)
            {
                consentType = isConcent.Value ? TCPAConsentType.Yes : TCPAConsentType.No;
            }
            else
            {
                consentType = TCPAConsentType.Undefined;
            }

            return result;
        }

        /// <summary>
        /// Set Marketing information to primary lead and update campaign key
        /// </summary>
        /// <param name="arcCases">ArcCase</param>
        /// <param name="lead">Lead</param>
        /// <returns></returns>
        private ArcResponseResult SetCampaign(ArcCases arcCases, ArcCampaignRequestLead lead)
        {
            ArcResponseResult result = null;
            if (lead == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, null, null, ArcMessages.MissingLead);
                return result;
            }
            var marketing = lead.Marketing;
            if (marketing == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.MissingMarketingNode);
                return result;
            }
            if (string.IsNullOrEmpty(marketing.Campaign))
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.RequiredCampainKey);
                return result;
            }
            var account = arcCases.Account;
            if (account == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, null, ArcMessages.NotExistAccount);
                return result;
            }
            var primaryLead = account.PrimaryLead;
            if (primaryLead == null)
            {
                result = GetArcLeadResult(AcceptStatus.No, lead.Reference, arcCases.AccountKey, ArcMessages.NotExistPrimaryLead);
                return result;
            }
            if (!string.IsNullOrEmpty(marketing.Campaign))
            {
                var id = GetCampaignById(marketing.Campaign);
                if (id == null)
                {
                    result = GetArcLeadResult(AcceptStatus.No, lead.Reference, arcCases.AccountKey, ArcMessages.InvalidCampainKey);
                    return result;
                }
                primaryLead.CampaignId = id;
            }
            else
            {
                // Default campaign.
                primaryLead.CampaignId = DefaultCampaign;

            }
            if (marketing.SourceCode != null) primaryLead.SourceCode = marketing.SourceCode;
            if (marketing.IpAddress != null) primaryLead.IPAddress = marketing.IpAddress;
            if (marketing.TrackingCode != null) primaryLead.TrackingCode = marketing.TrackingCode;
            if (!string.IsNullOrEmpty(lead.Timestamp))
                primaryLead.ChangedOn = lead.Timestamp.ConvertUTCToLocal(LocalTimeZoneId);

            Engine.ArcActions.Save(arcCases);
            long? key = arcCases.ArcIndividualKey;
            result = GetArcLeadResult(AcceptStatus.Yes, lead.Reference, arcCases.AccountKey, null, indiv_key: key);
            return result;

        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Get CampaignId under ArcCampaign identifier
        /// </summary>
        /// <param name="campaign"></param>
        /// <returns></returns>
        private int? GetCampaignById(string campaign)
        {
            return Engine.ArcActions.GetCampaignIdByMapId(campaign);
        }

        /// <summary>
        ///  Get Gender 
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        private string GetGenderInformation(string gender)
        {
            switch (gender.ToUpper())
            {
                case "M":
                    return "Male";
                case "F":
                    return "Female";
            }
            return gender;
        }

        /// <summary>
        /// Get Individual State id by state code.s
        /// </summary>
        /// <param name="appState">Abbreviation</param>
        /// <returns>int?</returns>
        private int? GetStateId(string appState)
        {
            //var state = _engine.ManageStates.Get(appState);
            var state = Engine.ArcActions.GetStateByAbbreviation(appState);
            return state != null ? (int?)state.Id : null;
        }

        /// <summary>
        /// Fix phone format and return long
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private long? ConvertPhoneStringToLong(string number)
        {
            number = number.Replace("-", "");
            long? ph = null;
            try
            {
                ph = Convert.ToInt64(number);
            }
            catch (Exception ex)
            {
                throw new Exception("Phone number is not in correct format only integers are allowed to map");
            }
            return ph;
        }

        /// <summary>
        /// Validate user credential 
        /// </summary>
        /// <param name="userName">UserId from request object</param>
        /// <param name="password">password</param>
        /// <returns></returns>
        private bool IsAuthenticatedPost(string userName, string password)
        {
            //TODO: Validate request Credential 
            var res = Membership.ValidateUser(userName, password);
            if (res)
            {

                try
                {
                    var key = Membership.GetUser(userName).ProviderUserKey.ToString();
                    Guid? id = key.AsGuid();
                    if (id != null)
                    {
                        return Engine.ArcActions.ValidateCredential(id.Value);
                    }
                }
                catch (Exception e)
                {
                    return false;

                }
            }
            return false;
        }

        #endregion

        #region Disposable pattren

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            if (isDisposing)
            {
                // free managed resources
                Engine.Dispose();
            }
            Logger = null;
            _isDisposed = true;
        }

        #endregion

        public void LogCritical(long? actId, string msg)
        {
            string query = @"INSERT INTO dbo.account_history( ach_entry , ach_comment ,ach_added_date , ach_entryType )
                            VALUES  ( @entry , @comments ,GETDATE() ,5 )";
            var parameters = new SqlParameterFluent().Add("entry", "ARC Critical").Add("comments", msg).ToObjectArray();
            int res = Engine.ExecuteStoreCommand(query, parameters);
        }

        public int DefaultIndividualStatusId { get { return Engine.ApplicationSettings.SC_ARC_DefaultPostIndividualStatusId; } }
    }

    public class ArcMessages
    {
        /// <summary>
        /// Missing prefix for xml node.
        /// Invalid for field invalid
        /// NotExist for Dataobject
        /// </summary>
        public const string UnableToProcess = "Unable to process the request";
        public const string RequiredCampainKey = "Change ignored, campaign required";
        public const string InvalidCampainKey = "Campaign not found";
        public const string InvalidStatus = "Status not found";
        public const string NoChangesApplied = "No changes to apply";
        public const string AlreadyUpdated = "Record is already updated";
        public const string SameIndividualInfoExist = "Same individual information already exist can't insert as secondary individual";
        public const string MissingLoginNode = "Post credential information missing";
        public const string MissingCommunicationNode = "Communication information not found";
        public const string MissingStatusCode = "Status code not found";
        public const string MissingStatusNode = "Status information not found";
        public const string MissingAgentInitial = "Agent initial not found";
        public const string MissingIndividualNode = "Individual information not found";
        public const string MissingMarketingNode = "Marketing node not found";
        public const string MissingReferenceKey = "lead reference key not found";
        public const string MissingCompanionReferenceKey = "Companion reference key not found";
        public const string MissingLead = "Lead node not found";
        public const string MissingCommunicationPreferencesNode = "Preferences information not found in email communication";
        public const string MissingEmailOptOut = "Email opt out not found";

        public const string InvalidCredential = "Invalid Post credential";
        public const string InvalidRequest = "Invalid request can't process.";
        public const string InvalidConsent = "Invalid consent information";

        public const string NotExistArcCase = "Record not foudn for lead reference key";
        public const string NotExistArcCaseIndividual = "Case individual not found";
        public const string NotExistPrimaryIndividual = "Primary individual not found";
        public const string NotExistAccount = "Account not found";
        public const string NotExistUser = "Agent not found";
        public const string NotExistPrimaryLead = "Primary lead not found";
        public const string NotExistAssocatedIndividual = "Arc Associated individual not found";



    }

    public class ArcServiceLogger
    {

        public bool CanLog
        {
            get;
            set;
        }

        public void WriteLog(string loggerName, Type requestType, object request, Type responseType, object response)
        {
            if (request != null)
            {
                if (CanLog)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("========================================================================");
                    builder.AppendFormat("\t\t\t\t\t{0} Request", loggerName);
                    builder.AppendLine();
                    builder.AppendLine("========================================================================");
                    var requestXmlString = GetObjectAsXmlString(requestType, request);
                    builder.AppendLine(requestXmlString);
                    builder.AppendLine("========================================================================");
                    builder.AppendFormat("\t\t\t\t\t{0} Response", loggerName);
                    builder.AppendLine();
                    builder.AppendLine("========================================================================");
                    var responseXmlString = GetObjectAsXmlString(responseType, response);
                    builder.AppendLine(responseXmlString);
                    builder.AppendLine();
                    builder.AppendLine("####################################################################");
                    builder.AppendLine();
                    builder.AppendLine();
                    var logger = NLog.LogManager.GetLogger(loggerName);
                    string record = builder.ToString();
                    logger.SafeLog(record);
                    //SafeLog(logger.Info,record);

                }
            }
        }

        public void WriteError(string loggerName, Type requestType, object request, string error)
        {
            if (request != null)
            {
                if (CanLog)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("========================================================================");
                    builder.AppendFormat("\t\t\t\t\t{0} Request", loggerName);
                    builder.AppendLine("========================================================================");
                    var requestXmlString = GetObjectAsXmlString(requestType, request);
                    builder.AppendLine(requestXmlString);
                    builder.AppendLine("========================================================================");
                    builder.AppendLine("\t\t\t\t\tError");
                    builder.AppendLine("========================================================================");
                    builder.AppendLine(error);
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.AppendLine();
                    var logger = NLog.LogManager.GetLogger(loggerName);
                    string record = builder.ToString();
                    logger.SafeLogError(record);
                    //SafeLog(logger.Error,record);
                }
            }
        }
        private static string GetObjectAsXmlString(Type type, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            serializer.Serialize(writer, obj);
            return builder.ToString();
        }

    }
}
