using System;
using System.Collections.Generic;

using System.Linq;
using System.Diagnostics;

using INTERFACE = SalesTool.Service.Interfaces;

using System.Threading;
using System.Xml;
using System.Reflection;

namespace SalesTool.Service
{
    // SZ [Mar 18, 2013] Created a seperate class so that the service class does not deal with the functionalit
    // as a proper object oriented way a class must be restricted to do only the things that are required and it must
    // not implement an anti-pattern. The Engine below implements the actual functionality
    public class ServiceEngine:
        INTERFACE.IService, 
        INTERFACE.IServiceLog,
        IDisposable
    {
        static ServiceEngine _MyEngine = null;

        System.Timers.Timer _timer = null;

        string
            _connectionString = string.Empty,
            _cnnLead = string.Empty,
            _cnnAdmin = string.Empty,
            _cnnDashboard = string.Empty;
        
        string _dbgFile = String.Empty;
        bool _dbgEnabled = false;

        string _name = string.Empty;
        int _interval = 1000;
        


        string _pluginFile = string.Empty;

        PluginManager plugins = null;

        ServiceEngine() {}

        
        public static ServiceEngine Instance
        {
            [System.Diagnostics.DebuggerHidden()]
            get
            {
                if (_MyEngine == null)
                    _MyEngine = new ServiceEngine();
                return _MyEngine;
            }
        }
        
        #region IServiceLog

        public void Error(string text)
        {
            lock (this)
            {
                EventLog.WriteEntry(_name, text, EventLogEntryType.Error);
            }
        }
        public void Information(string text)
        {
            lock (this)
            {
                EventLog.WriteEntry(_name, text, EventLogEntryType.Information);
            }
        }
        public void Warning(string text)
        {
            lock (this)
            {
                EventLog.WriteEntry(_name, text, EventLogEntryType.Warning);
            }
        }
        public void Debug(string text)
        {
#if DEBUG
            const string KSeprator = "***********************************************";
            lock (this)
            {
                if (_dbgEnabled)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(4096);

                    sb.AppendFormat("[0} {1} {2}", System.Environment.NewLine, KSeprator, System.Environment.NewLine);
                    sb.AppendFormat("{0} : {1} {2}", DateTime.Now.ToString(), text, System.Environment.NewLine);
                    sb.AppendFormat("[0} {1} {2}", System.Environment.NewLine, KSeprator, System.Environment.NewLine);

                    _dbgFile = _dbgFile + string.Format("{0:MMM_dy_hh_mm_ss}", DateTime.Now);
                    using (System.IO.FileStream fs = new System.IO.FileStream(_dbgFile, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                        {
                            sw.Write(sb.ToString());
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
            }
#endif
        }
        public string PluginPath
        {
            get { throw new NotImplementedException(); }
        }

        public string ServicePath
        {
            get { return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }
        #endregion

           
        void ReadConfiguration()
        {
            float tmp = default(float);
            float.TryParse(System.Configuration.ConfigurationManager.AppSettings["interval"].ToString(), out tmp);
            _interval = (int)((tmp==default(float)? 1:tmp) * Konstants.Milli_In_Minute);

            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            _cnnAdmin = System.Configuration.ConfigurationManager.ConnectionStrings["AdminModel"].ConnectionString;
            _cnnLead = System.Configuration.ConfigurationManager.ConnectionStrings["LeadModel"].ConnectionString;
            _cnnDashboard = System.Configuration.ConfigurationManager.ConnectionStrings["DashboardModel"].ConnectionString;

            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["IsTraceOn"].ToString(), out _dbgEnabled);
            _dbgFile = System.Configuration.ConfigurationManager.AppSettings["logFilePath"].ToString();



            _name = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            _pluginFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Konstants.Plugin_File);

            DEBUG_MESSAGE(1);
        }

        public bool Init()
        {
            bool bSuccess = false;
            try
            {
                
                ReadConfiguration();
                
                plugins = new PluginManager(_cnnAdmin, _cnnLead, _cnnDashboard, _connectionString, _pluginFile, this);
                plugins.Load();
                plugins.Initialize();

                _timer = new System.Timers.Timer { Enabled = false, Interval = _interval };
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTick);
                _timer.Start();

                bSuccess = true;

                DEBUG_MESSAGE(2);
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
            return bSuccess;
        }
        public void Dispose()
        {
            try
            {
                //UnloadPlugins(true);
                plugins.Unload(true);

                //Dispose the dynamic query parser timer
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Close();
                    _timer.Dispose();
                    _timer = null;
                }

                DEBUG_MESSAGE(3);
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }

        void Execute()
        {
            plugins.Execute();
        }
        public static void OnTick(object source, System.Timers.ElapsedEventArgs e)
        {
            ServiceEngine E = ServiceEngine.Instance;
            E._timer.Enabled = false;
            E.Execute();
            E._timer.Enabled = true;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        void DEBUG_MESSAGE(int i)
        {
#if DEBUG
            switch (i)
            {
                case 1: Information(string.Format("{0} has been read configuration scuccessfully.", _name)); break;
                case 2: Information(string.Format("plugin manager has found {0} clients. Loaded all succefully for execution.", plugins.Tasks)); break;
                case 3: Information(string.Format("{0} has shutdown with {1} thread(s) running.", _name, plugins.Threads)); break;
            }
#endif
        }





    }


}
