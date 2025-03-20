/*
 * Filename:  ArcHelperAndExtensionMethods
 * Author:    MH
 * Purpose: This class is responsible data conversion from SelectCARE to Arc request.
 *          
      Initial Implementation:  18:12:2013
      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SalesTool.DataAccess.Arc;
using SalesTool.DataAccess.Models;

namespace SelectCare.ArcApi
{

    public static class ArcDataConversion
    {
        public const string UtcDateFormat = "yyyy-MM-dd HH:mm:ss UTC";
        public const string BirthDateFormat = "yyyy-MM-dd";
       
        public static string CurrentUniversalTime
        {
            get { return DateTime.Now.ToUniversalTime().ApplyUtcFormat(); }
        }
       

        public static ArcMessage ConvertToArcMessage(this vw_ArcLetterLog log)
        {
            var message = new ArcMessage
                {
                    //we need to send template title for code and title for name
                    // previously it was int mapped to template id
                    //Code = log.Code,
                    Code = log.Name,
                    Date = log.Date.HasValue
                               ? log.Date.Value.ConvertToUtcDateTimeString()
                               : CurrentUniversalTime,
                    ID = log.Id,
                    Name = log.Name
                };

            if (log.Status == 2)
                message.Status = "Success";
            else if (log.Status == 0)
                message.Status = "Failed";
            else
                message.Status = "Queued";
            return message;
        }

        /// <summary>
        /// Will Add Email Communication node to request. 
        /// Required ArcIndividual.
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        public static ArcLead ConvertToLeadStopLetterRequest(this ArcCases cases)
        {
            if (cases != null)
            {
                var lead = new ArcLead()
                    {
                        Reference = cases.ArcRefreanceKey,
                        Timestamp = cases.ModifyOn.ConvertToUtcDateTimeString(),
                        EmailCommunications = GetEmailCommunication(cases)
                    };
                return lead;
            }
            return null;
        }

        /// <summary>
        /// Will Add Status Node to the Request.
        /// </summary>
        /// <param name="accountHistory"></param>
        /// <returns></returns>
        public static List<ArcLead> ConvertToLeadChangeAgentRequest(this AccountHistory accountHistory)
        {

            if (accountHistory != null)
            {
                List<ArcLead> leads = new List<ArcLead>();
                var account = accountHistory.Account;
                if (account != null)
                {
                    foreach (var @case in account.ArcCases)
                    {
                        var lead = new ArcLead()
                        {
                            Timestamp = accountHistory.Datetime.ConvertToUtcDateTimeString(),
                            Reference = @case.ArcRefreanceKey,
                            Status = GetStatus(account),
                        };
                        if (lead.Status != null)
                            leads.Add(lead);
                    }
                }

                return leads;
            }
            return null;

        }
        /// <summary>
        /// Will Add Status Node to the Request.
        /// </summary>
        /// <param name="accountHistory"></param>
        /// <returns></returns>
        public static List<ArcLead> ConvertToLeadChangeAgentRequest(this vw_ArcChangeAgent accountHistory)
        {

            if (accountHistory != null)
            {
                List<ArcLead> leads = new List<ArcLead>();
                var account = accountHistory.Account;
                if (account != null)
                {
                    foreach (var @case in account.ArcCases)
                    {
                        var lead = new ArcLead()
                        {
                            Timestamp = accountHistory.Datetime.ConvertToUtcDateTimeString(),
                            Reference = @case.ArcRefreanceKey,
                            Status = GetStatus(account),
                        };
                        if (lead.Status != null)
                            leads.Add(lead);
                    }
                }

                return leads;
            }
            return null;

        }
        /// <summary>
        /// This will add nodes to request
        /// <remarks>
        /// Required Individual node with phone contacts
        /// </remarks>
        /// </summary>
        /// <param name="cases"></param>
        /// <param name="states"></param>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        /// <summary> This function will update the TCPA consent field for the given reference.</summary>
        public static ArcLead ConvertToLeadUpdateRequest(this ArcCases cases, List<StateModel> states, List<CampaignModel> campaigns)
        {
            if (cases != null)
            {
                var lead = new ArcLead()
                    {
                        Timestamp = cases.ModifyOn.ConvertToUtcDateTimeString(),
                        Reference = cases.ArcRefreanceKey,
                        Individual = GetIndividualConcentInformation(cases, states)
                        //Status = GetStatus(cases),
                        //EmailCommunications = GetEmailCommunication(cases),
                        //Individual = GetIndividualInformation(cases, states),
                        //Marketing = GetMarketingInformation(cases, campaigns)
                    };
                return lead;
            }
            return null;
        }

        /// <summary>
        /// Converts case into Lead Request having Individual contact informations only 
        /// </summary>
        /// <param name="cases"></param>
        /// <param name="states"></param>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        public static ArcLead ConvertToContactUpdateLeadRequest(this ArcCases cases, List<StateModel> states, List<CampaignModel> campaigns)
        {
            if (cases != null)
            {
                var lead = new ArcLead()
                    {
                        Timestamp = cases.ArcIndividual.ChangedOn.ConvertToUtcDateTimeString(),//cases.ModifyOn.HasValue ? cases.ModifyOn.Value.ToString(UtcDateFormat) : "",
                        Reference = cases.ArcRefreanceKey,
                        //Status = GetStatus(cases),
                        //EmailCommunications = GetEmailCommunication(cases),
                        Individual = GetIndividualContactInformation(cases, states),
                        //Marketing = GetMarketingInformation(cases, campaigns)
                    };
                return lead;
            }
            return null;
        }
        public static string GetConsentInformation(this Account account)
        {
            var tcpaConsent = account.PrimaryIndividual.TCPAConsent;
            return tcpaConsent;
        }


        private static LeadIndividual GetIndividualConcentInformation(ArcCases cases, List<StateModel> states)
        {
            if (cases != null)
            {
                var individual = cases.ArcIndividual;
                if (individual != null)
                {
                    string consent = cases.Account.GetConsentInformation();
                    var leadIndividual = new LeadIndividual
                    {
                        Contact = GetIndividualContactForConsentUpdate(individual, states, consent),
                    };

                    return leadIndividual;
                }
                return null;
            }
            return null;
        }

        private static IndividualContact GetIndividualContactForConsentUpdate(Individual individual, List<StateModel> states, string consent)
        {
            if (individual != null)
            {
                string c = null;
                if (!string.IsNullOrEmpty(consent))
                {
                    c = consent.ToUpper();
                }

                var contact = new IndividualContact()
                {

                    DayPhone = new ContactPhone()
                        {
                            Consent = c,
                            Number = individual.DayPhone.HasValue ? individual.DayPhone.Value.ToString("#") : ""
                        },

                    EveningPhone = new ContactPhone()
                        {
                            Consent = c,
                            Number = individual.EveningPhone.HasValue ? individual.EveningPhone.Value.ToString("#") : ""
                        },
                    MobilePhone = new ContactPhone()
                    {
                        Consent = c,
                        Number = individual.CellPhone.HasValue ? individual.CellPhone.Value.ToString("#") : ""
                    }

                    //Phone = 
                    //Title = individual.
                    //Suffix = 

                };
                return contact;
            }
            return null;
        }


        /// <summary>
        /// Get lead Marketing node information from the case
        /// it refers to primary lead of the arcCase.
        /// </summary>
        /// <param name="cases"></param>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        private static RequestLeadMarketing GetMarketingInformation(ArcCases cases, IEnumerable<CampaignModel> campaigns)
        {
            if (cases != null)
            {
                var primaryLead = cases.PrimaryLead;
                if (primaryLead != null)
                {
                    var marketing = new RequestLeadMarketing()
                        {
                            Campaign = GetArcCampaignMapfromId(primaryLead.CampaignId, campaigns),
                            IpAddress = primaryLead.IPAddress,
                            SourceCode = primaryLead.SourceCode,
                            TrackingCode = primaryLead.TrackingCode
                        };

                    return marketing;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Get Lead Individual information 
        /// Nodes are Contact, DesiredInsurance, ExistingInsurance, ScoringInformation.
        /// </summary>
        /// <param name="cases"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        private static LeadIndividual GetIndividualInformation(ArcCases cases, List<StateModel> states)
        {
            if (cases != null)
            {
                var individual = cases.ArcIndividual;
                if (individual != null)
                {
                    string consent = cases.Account.GetConsentInformation();
                    var leadIndividual = new LeadIndividual
                        {
                            Contact = GetIndividualContactInformation(individual, states,consent),
                            DesiredInsurance = GetDesiredInsurance(individual),
                            ExistingInsurance = GetExistingInsurance(individual),
                            ScoringInformation = GetScoringInformation(cases)

                        };
                    return leadIndividual;
                }
                return null;
            }
            return null;
        }
        private static LeadIndividual GetIndividualContactInformation(ArcCases cases, List<StateModel> states)
        {
            if (cases != null)
            {
                var individual = cases.ArcIndividual;
                if (individual != null)
                {
                    string consent = cases.Account.GetConsentInformation();
                    var leadIndividual = new LeadIndividual
                        {
                            Contact = GetIndividualContactInformation(individual, states,consent),
                        };
                    return leadIndividual;
                }
                return null;
            }
            return null;
        }
        /// <summary>
        /// Get policy information form ArcCase.
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        private static LeadPolicy GetLeadPolicyInformation(ArcCases cases)
        {
            if (cases != null)
            {
                var policy = new LeadPolicy()
                    {
                        CompanyCode = cases.CompanyCode,
                        Duration = cases.Duration,
                        FaceAmount = cases.FaceAmount,
                        Note = cases.Notes,
                        CaseSpecialist = new LeadPolicyCaseSpecialist()
                            {
                                Name = cases.CaseSpecialistName,
                                Ext = cases.CaseSpecialistExt
                            }

                    };
                return policy;
            }
            return null;
        }

        /// <summary>
        /// Get Scoring Information from case Primary lead
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        private static IndividualScoringInformation GetScoringInformation(ArcCases cases)
        {
            // TODO:
            if (cases != null)
            {
                //Possibility: primary lead on account and primary lead on case not be the same.
                var primaryLead = cases.PrimaryLead;
                if (primaryLead != null)
                {
                    var scoring = new IndividualScoringInformation()
                        {
                            FraudScore = primaryLead.ArcFraudScore,
                            Substatus = primaryLead.ArcSubStatus,
                            Tier = primaryLead.ArcTier,
                            eScore = primaryLead.ArcEScore
                        };
                    return scoring;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Get the existing insurance from SC individual
        /// </summary>
        /// <param name="individual">SC individual</param>
        /// <returns></returns>
        /// <author>MH</author>
        private static IndividualExistingInsurance GetExistingInsurance(Individual individual)
        {
            if (individual != null)
            {
                var existing = new IndividualExistingInsurance
                    {
                        Insurance = individual.indv_exist_ins,
                        Amount = individual.indv_exist_ins_amt.HasValue ? individual.indv_exist_ins_amt.Value : 0,
                        WantsToReplace = individual.indv_exist_ins_rplc,
                    };
                return existing;
            }
            return null;
        }

        /// <summary>
        /// Get DesireInsurance information from SC individual.
        /// </summary>
        /// <param name="individual">SC individual</param>
        /// <returns></returns>
        private static IndividualDesiredInsurance GetDesiredInsurance(Individual individual)
        {
            if (individual != null)
            {
                var desired = new IndividualDesiredInsurance
                    {
                        AlternateAmount = individual.indv_desire_ins_alt_amt,
                        Amount = individual.indv_desire_ins_amt,
                        TermDuration =
                            individual.indv_desire_ins_term.HasValue
                                ? individual.indv_desire_ins_term.Value.ToString()
                                : ""
                    };
                return desired;
            }
            return null;
        }

        /// <summary>
        /// Get Individual contact information from SC individual.
        /// </summary>
        /// <param name="individual">SC Individual</param>
        /// <param name="states">SC state collection</param>
        /// <param name="consent"></param>
        /// <returns></returns>
        private static IndividualContact GetIndividualContactInformation(Individual individual, List<StateModel> states, string consent)
        {
            if (individual != null)
            {
                var contact = new IndividualContact()
                    {
                        //TODO:Title
                        //Title = individual.
                        FirstName = individual.FirstName,
                        LastName = individual.LastName,
                        //TODO:Suffix
                        //Suffix = 
                        Address = individual.Address1,
                        City = individual.City,
                        State = GetArcStateMapFromStateId(individual.StateID, states),
                        Zip = individual.Zipcode,
                        BirthDate = individual.Birthday.HasValue ? individual.Birthday.Value.ToString(BirthDateFormat) : "",
                        Email = individual.Email,
                        Gender = individual.Gender.FirstOrDefault().ToString(),
                        AppState = GetArcStateMapFromStateId(individual.ApplicationState, states),

                        //DayPhone = individual.DayPhone
                        //Phone = 

                    };
               
                var indvConsent = GetIndividualContactForConsentUpdate(individual, states, consent);
                contact.DayPhone = indvConsent.DayPhone;
                contact.EveningPhone = indvConsent.EveningPhone;
                contact.MobilePhone = indvConsent.MobilePhone;
                return contact;
            }
            return null;
        }
        /// <summary>
        /// Get Email communication information form ArcCase
        /// <remarks>Should have arcIndividual associated to ArcLead</remarks>
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        /// <author>MH</author>
        private static EmailCommunications GetEmailCommunication(ArcCases cases)
        {
            var individual = cases.ArcIndividual;
            if (individual != null)
            {
                var email = new EmailCommunications
                    {
                        Preferences = new CommunicationsPreferences()
                            {
                                //NOTE: Not saving in Arc
                                Reason = "",
                                LastUpdate = individual.ChangedOn.ConvertToUtcDateTimeString(),
                                OptOut = individual.IndividualEmailOptOut ? "Y" : "N"
                            }
                    };
                return email;
            }

            return null;
        }

        /// <summary>
        /// Get the status information form latest ArcHistory for the case.
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        private static RequestLeadStatus GetStatus(ArcCases cases)
        {
            if (cases != null)
            {
                var account = cases.Account;
                if (account != null)
                {
                    if (account.User != null)
                    {
                        string userInitial = account.User.ArcId;
                        var status = new RequestLeadStatus
                        {
                            AgentInitials = userInitial,
                            LastUpdate = account.ChangedOn.ConvertToUtcDateTimeString(),
                            //LastUpdate = cases.ModifyOn.HasValue ? cases.ModifyOn.Value.ToString(UtcDateFormat) : "",
                        };
                        return status;
                    }
                }
            }

            //ArcHistory history = cases.ArcHistory.OrderByDescending(p => p.AddedOn).FirstOrDefault();
            //if (history != null)
            //{
            //    var status = new RequestLeadStatus
            //        {
            //            Code = history.Status,
            //            AgentInitials = history.UserInitial,
            //            LastUpdate = history.ChangedOn.HasValue ? history.ChangedOn.Value.ToUniversalTime().ToString(UtcDateFormat) : ""
            //        };
            //    return status;
            //}
            return null;
        }


        /// <summary>
        /// Get the status information form Account
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        private static RequestLeadStatus GetStatus(Account account)
        {
            if (account != null)
            {
                if (account.User != null)
                {
                    string userInitial = account.User.ArcId;
                    var status = new RequestLeadStatus
                    {
                        //Code = 
                        AgentInitials = userInitial,
                        LastUpdate = account.ChangedOn.ConvertToUtcDateTimeString(),
                    };
                    return status;
                }
            }
            return null;
        }

        /// <summary>
        /// Get Arc State Abbreviation from state Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        private static string GetArcStateMapFromStateId(int? id, IEnumerable<StateModel> states)
        {
            var state = states.FirstOrDefault(p => p.Id == id);
            if (state != null)
                return state.Abbreviation;
            return null;
        }

        /// <summary>
        /// Get arc campaign map id from SC campaign id
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        private static string GetArcCampaignMapfromId(int? campaignId, IEnumerable<CampaignModel> campaigns)
        {
            var campaign = campaigns.FirstOrDefault(p => p.ID == campaignId);
            if (campaign != null)
                return campaign.ArcMap;
            return null;
        }



    }
}