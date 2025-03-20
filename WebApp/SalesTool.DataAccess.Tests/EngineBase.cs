using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess.Tests
{
    public class EngineBase
    {

        DBEngine engine = null;

        internal EngineBase()
        {
            engine = new DBEngine();
            //engine.Init(System.Configuration.ConfigurationManager.ConnectionStrings["SalesToolEntities"].ConnectionString, System.Configuration.ConfigurationManager.ConnectionStrings["LeadEntities"].ConnectionString, System.Configuration.ConfigurationManager.ConnectionStrings["DashboardEntities"].ConnectionString);
            engine.Init(System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
        }

        protected DBEngine E
        {
            get
            {
                return engine;
            }
        }
    }
}
