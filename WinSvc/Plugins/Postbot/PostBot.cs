using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SalesTool.Service.Interfaces;
using SalesTool.DataAccess.Models;
using Schema = SalesTool.Schema;

namespace Postbot
{
    public class PostBot: SalesTool.Service.Interfaces.IClient, 
        SalesTool.Service.Interfaces.IClientInformation, 
        SalesTool.Service.Interfaces.IClientTask
    {
        public enum PostQueueStatus
        {
            Error = 0,
            Queued = 1,
            Posted = 2
        }
        public void Init(IServiceDataAccess db, IServiceLog log, string path)
        {
            // TODO: Implement this method
            //throw new NotImplementedException();
            log.Information(string.Format("{0} : {1}", Name, "Initialization successful"));
        }

        public void Execute(IServiceDataAccess da, IServiceLog log, string path)
        {            
           var nPostQueue =  da.Engine.PostQueueActions.All.ToList();
           foreach (var item in nPostQueue)
           {
               Post nPost = da.Engine.ManagePostsActions.Get(item.PostTemplateKey.HasValue? item.PostTemplateKey.Value: 0);
               
               if (nPost != null && item.RunDateTime.HasValue && item.RunDateTime.Value.CompareTo(DateTime.Now) <= 0)
               {
                   string responseData = "";
                   string responseDescription = "";
                   PostType postType = PostType.Post;
                   postType = GetPostType(nPost, postType);
                   log.Information(string.Format("Post Queue ID = {0} Entered for post processing..", item.Key.ToString()));
                   System.Net.HttpStatusCode x = PostHelper.Post(nPost.Url, postType, nPost.Header, nPost.Body, ref responseDescription, ref responseData);
                   int code = 0;
                   int.TryParse(x.ToString(),out code);
                   if (code >= 200 && code < 300)//2xx Success
                   {
                       item.Status = (short)PostQueueStatus.Posted;
                   }
                   else //Consider it error
                   {
                       item.Status = (short)PostQueueStatus.Error;
                   }
                   item.ResponseCode = code.ToString();
                   item.ResponseMessage = responseData;
                   item.ChangedOn = DateTime.Now;
                   da.Engine.PostQueueActions.Change(item);
                   log.Information(string.Format("Post Queue ID = {0} successfully processed.",item.Key.ToString()));
               }
           }
        }

        private static PostType GetPostType(Post nPost, PostType postType)
        {
            switch (nPost.Type)
            {
                case 0:
                    postType = PostType.XML;
                    break;
                case 1:
                    postType = PostType.Post;
                    break;
                case 2:
                    postType = PostType.Get;
                    break;
            }
            return postType;
        }

        public string Name
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
        }
        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public string Description
        {
            get
            {
                string Ans = string.Empty;
                Assembly currentAssem = System.Reflection.Assembly.GetExecutingAssembly();
                object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
                if (attribs.Length > 0)
                    Ans = ((AssemblyDescriptionAttribute)attribs[0]).Description;
                return Ans;
            }
        }

        public IClientTask Task { get { return this; }  }

        public IClientInformation Information { get{ return this;} }

        public void Dispose()
        {
            // TODO: Implement this method
            //throw new NotImplementedException();
        }
    }
}
