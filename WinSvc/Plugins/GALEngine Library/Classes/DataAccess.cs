using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using GALEngine_Library;

namespace GalEngine
{
    public static class DataAccess
    {
        private static GALEngineEntities GE = new GALEngineEntities();


        public static string LoadGWBListAsJson()
        {
            string json = "";
            var v = (from a in GE.GALEngineListsDump
                     select a).FirstOrDefault();

            if (v != null)
            {
                json = v.GWBListDump;
            }
            return json;
        }

        public static string LoadACDListAsJson()
        {
            string json = "";
            var v = (from a in GE.GALEngineListsDump
                     select a).FirstOrDefault();

            if (v != null)
            {
                json = v.ACDListDump;
            }
            return json;
        }

        public static void SaveGWBListAsJson(string json)
        {
            GALEngineListsDump g = (from a in GE.GALEngineListsDump
                     select a).FirstOrDefault();

            if (g != null)
            {
                g.ACDListDump = json;
                GE.SaveChanges();
            }
            else
            {
                g = new GALEngineListsDump();
                g.GWBListDump = json;
                GE.GALEngineListsDump.Add(g);
                GE.SaveChanges();
            }
        }

        public static void SaveACDListAsJson(string json)
        {
            GALEngineListsDump g = (from a in GE.GALEngineListsDump
                                    select a).FirstOrDefault();

            if (g != null)
            {
                g.ACDListDump = json;
                GE.SaveChanges();
            }
            else
            {
                g = new GALEngineListsDump();
                g.ACDListDump = json;
                GE.GALEngineListsDump.Add(g);
                GE.SaveChanges();
            }
        }

        public static string getAcdGalSignalRurl()
        {
            return GetAppStorageString("GalAcdSignalRurl");
        }

        public static string getWebGalSignalRurl()
        {
            return GetAppStorageString("GalWebSignalRurl");
        }

        public static int getMilisecondsToSleep()
        {
            return GetAppStorageint("GalMilisecondsToSleep");
        }

        public static bool getAcdGalEnabled()
        {
            return GetAppStorageBool("GalAcdEnabled");
        }

        public static bool getWebGalEnabled()
        {
            return GetAppStorageBool("GalWebEnabled");
        }

        private static string GetAppStorageString(string key)
        {
            ApplicationStorage appStor = new ApplicationStorage();
            appStor = new ApplicationStorage();
            appStor = GE.Application_Storage.Where(app => app.Key == key).FirstOrDefault();
            if (appStor != null && !string.IsNullOrWhiteSpace(appStor.tvalue))
                return appStor.tvalue.Trim();
            return null;
        }

        private static int GetAppStorageint(string key)
        {
            ApplicationStorage appStor = new ApplicationStorage();
            appStor = new ApplicationStorage();
            appStor = GE.Application_Storage.Where(app => app.Key == key).FirstOrDefault();
            if (appStor != null)
                return (int)(appStor.iValue.HasValue == true ? appStor.iValue : 0);
            return 0;
        }

        private static bool GetAppStorageBool(string key)
        {
            ApplicationStorage appStor = new ApplicationStorage();
            appStor = new ApplicationStorage();
            appStor = GE.Application_Storage.Where(app => app.Key == key).FirstOrDefault();
            if (appStor != null)
                return (bool)(appStor.bValue.HasValue == true ? appStor.bValue : false);
            return false;
        }
    }
}