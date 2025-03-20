/*
 * Filename:  ArcClientHelper
 * Author:    MH
 * Purpose: This class is responsible for sending requests to appropriate endpoints to the arc api.
 *          
      Initial Implementation:  18:12:2013
      
 */

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SelectCare.ArcApi
{
    public class ArcClientHelper
    {
        internal const string EndPointLeadChangeAgent = "LifeApi/Leads/ChangeAgent";
        internal const string EndPointLeadAddAction = "LifeApi/Leads/AddAction";
        internal const string EndPointLeadStopLetter = "LifeApi/Leads/StopLetter";
        internal const string EndPointLeadLetterLog = "LifeApi/Leads/LetterLog";
        internal const string EndPointLeadUpdate = "LifeApi/Leads/Update";
        internal const string EndPointOpportunitiesCreateOp = "LifeApi/Opportunities/CreateOp";
        internal const string EndPointContactUpdate = "LifeApi/Leads/UpdateContactInforamtion";
        // NOTE: not required ....
        //private const string EndPointPostAcdCapUpdateRequest = "SCServices/PostAcdCapUpdateRequest";
        //private const string EndPointPostConsentUpdateRequest = "SCServices/PostConsentUpdateRequest";
        private const string EndPointPostInsertUpdateLeadRequest = "LifeApi/SCServices/PostInsertUpdateLeadRequest";
        //private const string EndPointPostStopCommunicationRequest = "SCServices/PostStopCommunicationRequest";
        //private const string EndPointPostUpdateCampaignRequest = "SCServices/PostUpdateCampaignRequest";
        //private const string EndPointPostUpdateStatusRequest = "SCServices/PostUpdateStatusRequest";

        /// <summary>
        /// Get New HttpClient from base Address.
        /// </summary>
        /// <param name="baseUrl">Base Url</param>
        /// <param name="timeout">Timeout default is 100s</param>
        /// <returns>HttpClient</returns>
        /// <author>MH:</author>
        private static HttpClient GetClient(string baseUrl, TimeSpan? timeout = null)
        {
            if (baseUrl == null) throw new ArgumentNullException("baseUrl");
            HttpClient client = timeout != null ? new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = timeout.Value } : new HttpClient { BaseAddress = new Uri(baseUrl) };
            return client;
        }


        /// <summary>
        /// This function call will request that the agent in ARC be changed to the agent identified by the 
        /// Request.Leads.Lead.Status.AgentInitials value. 
        /// The agent must exist in ARC for this call to be successful. 
        /// Because this is a queued operation, you can consider a "Q" response as a successful response.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="request">This is the request that contains one or more Leads. 
        /// Each lead provided must at minimum contain a Status element with AgentInitials provided.</param>
        /// <returns></returns>
        /// <author>MH:</author>
        public static Task<HttpResponseMessage> PostChangeAgent(string baseUrl, ArcRequest request)
        {
            if (baseUrl == null) throw new ArgumentNullException("baseUrl");
            var client = GetClient(baseUrl);
            Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointLeadChangeAgent, request);
            return task;
        }

        /// <summary>
        /// This function call will add one or more actions to one or more leads. 
        /// Actions in SelectCare are like Dispositions in ARC.
        /// </summary>
        /// <param name="baseUrl">ServiceBase url</param>
        /// <param name="request">This is the request that contains one or more Leads. 
        /// Each lead provided should contain one or more actions to add.</param>
        /// <returns></returns>
        /// <author>MH:</author>
        public static Task<HttpResponseMessage> PostAddAction(string baseUrl, ArcRequest request)
        {
            var client = GetClient(baseUrl);
            Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointLeadAddAction, request);
            return task;
        }

        /// <summary>
        /// This function call will add or remove a stop letter in ARC. 
        /// Stop letters prevent marketing emails from being sent to the individual. 
        /// Transactional emails will continue to be sent. This occurs at the reference number level.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="request">This is the request that contains one or more Leads. 
        /// Each lead provided should contain EmailCommunications.Preferences
        /// with the updated preferences.</param>
        /// <returns></returns>
        /// <author>MH:</author>
        public static Task<HttpResponseMessage> PostStopLetter(string baseUrl, ArcRequest request)
        {
            var client = GetClient(baseUrl);
            Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointLeadStopLetter, request);
            return task;
        }

        /// <summary>
        /// ARC maintains a list of all email communications with the individual.
        /// In the case where SelectCare sends emails, the ARC's letter log needs to be updated.
        /// This API call will add a one or more letter log entries for one or more leads.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="request">This is the request that contains one or more Leads. 
        /// Each lead provided should contain one or more EmailCommunications.Message.Messages
        /// with details about the communication.</param>
        /// <returns></returns>
        /// <author>MH:</author>
        public static Task<HttpResponseMessage> PostLetterLog(string baseUrl, ArcRequest request)
        {
            var client = GetClient(baseUrl);
            Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointLeadLetterLog, request);
            return task;
        }

        /// <summary>
        /// This function call will update the TCPA consent field in ARC. All other data will be ignored.
        /// Consent is only evaluated if the phone number IsMobile is "Y".
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="request">This is the request that contains one or more Leads. 
        /// Each lead provided should contain an Individual with a DayPhone or EveningPhone with a Consent defined.</param>
        /// <returns></returns>
        /// <author>MH:</author>
        public static Task<HttpResponseMessage> PostUpdate(string baseUrl, ArcRequest request)
        {
            var client = GetClient(baseUrl);
            Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointLeadUpdate, request);
            return task;
        }
        /// <summary>
        /// If a communication occurs with an individual, 
        /// but a reference number is not generated in ARC, 
        /// it is necessary to create an Opportunity.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="request">This is the request that contains one or more Opportunities.
        ///  Each opportunity provided should contain the AgentInitials and TalkTimeMinutes.</param>
        /// <returns></returns>
        /// <author>MH:</author>
        public static Task<HttpResponseMessage> PostCreateOp(string baseUrl, OpRequest request)
        {
            var client = GetClient(baseUrl);
            Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointOpportunitiesCreateOp, request);
            return task;
        }

        /// <summary>
        /// This function call will update the contact inforamtion for a lead in ARC
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <author>MH:29 Mar 2014</author>
        public static Task<HttpResponseMessage> PostContactUpdate(string baseUrl, ArcRequest request)
        {
            var client = GetClient(baseUrl);
            Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointContactUpdate, request);
            return task;
        }
        #region SCServices
        //NOTE: not required 
        //public static Task<HttpResponseMessage> PostAcdCapUpdateRequest(string baseUrl, ArcRequestAgent request)
        //{
        //    var client = GetClient(baseUrl);
        //    Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointPostAcdCapUpdateRequest, request);
        //    return task;
        //}

        //public static Task<HttpResponseMessage> PostConsentUpdateRequest(string baseUrl, ArcRequest request)
        //{
        //    var client = GetClient(baseUrl);
        //    Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointPostConsentUpdateRequest, request);
        //    return task;
        //}

        //public static Task<HttpResponseMessage> PostInsertUpdateLeadRequest(string baseUrl, ArcRequest request)
        //{
        //    var client = GetClient(baseUrl);
        //    Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointPostInsertUpdateLeadRequest, request);
        //    return task;
        //}

        //public static Task<HttpResponseMessage> PostStopCommunicationRequest(string baseUrl, ArcRequest request)
        //{
        //    var client = GetClient(baseUrl);
        //    Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointPostStopCommunicationRequest, request);
        //    return task;
        //}

        //public static Task<HttpResponseMessage> PostUpdateCampaignRequest(string baseUrl, ArcRequest request)
        //{
        //    var client = GetClient(baseUrl);
        //    Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointPostUpdateCampaignRequest, request);
        //    return task;
        //}

        //public static Task<HttpResponseMessage> PostUpdateStatusRequest(string baseUrl, ArcRequest request)
        //{
        //    var client = GetClient(baseUrl);
        //    Task<HttpResponseMessage> task = client.PostAsJsonAsync(EndPointPostUpdateStatusRequest, request);
        //    return task;
        //}

        #endregion

      
    }
}
