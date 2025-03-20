using System;
using System.Data;
using System.Linq;
using SalesTool.DataAccess;
using Interface = SalesTool.Service.Interfaces;


namespace SalesTool.Service
{
    public sealed class Task :
     Interface.IServiceDataAccess
    {
        DBEngine _engine = new DBEngine();
        IDbConnection _cnn = null;
        Interface.IClientTask _task;
        Interface.IServiceLog _log;
        string _path = string.Empty;

        internal Task(Interface.IClientTask task,
            string cnnAdmin, string cnnLead, string cnnDashboard, string cnn,
            Interface.IServiceLog log, string path)
        {
            try
            {
                _log = log;
                //_engine.Init(cnnAdmin, cnnLead, cnnDashboard);
                _engine.Init(cnnAdmin);
                _cnn = new System.Data.SqlClient.SqlConnection(cnn);
                _task = task;
                _path = path;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        public bool IsValid { get { return true; } }
        public DBEngine Engine { get { return _engine; } }
        public IDbConnection Connection { get { return _cnn; } }

        [System.Diagnostics.DebuggerStepperBoundary()]
        public void Init()
        {
            try
            {
                if (_task != null)
                    _task.Init(this as Interface.IServiceDataAccess, _log, _path);
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException)
                    _log.Warning("Thread has been aborted, possibly due to service stop.");
                else
                    _log.Error(ex.ToString());
            }
            finally
            {
                if(_engine!=null)
                    _engine.Dispose();
                if (_cnn != null)
                {
                    _cnn.Close();
                    _cnn.Dispose();
                    _cnn = null;
                }
            }
        }

        [System.Diagnostics.DebuggerStepperBoundary()]
        public void Execute()
        {
            try
            {
                if(_task!=null)
                    _task.Execute(this as Interface.IServiceDataAccess, _log, _path);
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException)
                    _log.Warning("Thread has been aborted, possibly due to service stop.");
                else
                    _log.Error(ex.ToString());
            }
            finally
            {
                if (_engine != null)
                    _engine.Dispose();
                if (_cnn != null)
                {
                    _cnn.Close();
                    _cnn.Dispose();
                    _cnn = null;
                }
            }
        }
    }
}
