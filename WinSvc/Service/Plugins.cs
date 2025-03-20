using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using Interfaces = SalesTool.Service.Interfaces;

namespace SalesTool.Service
{
    public class PluginItem
    {
        public string Path;
        public Interfaces.IClientTask Task;

        public PluginItem(string path, Interfaces.IClientTask task)
        {
            Path = path;
            Task = task;
        }
    }
    public class PluginManager
    {
        Interfaces.IServiceLog _log = null;
        string
           _connectionString = string.Empty,
           _cnnLead = string.Empty,
           _cnnAdmin = string.Empty,
           _cnnDashboard = string.Empty;

        string _pluginFile = string.Empty;
        Dictionary<string, PluginItem> _clients = new Dictionary<string, PluginItem>();
        List<Thread> _threads = new List<Thread>();

        public PluginManager(string cnnAdmin, string cnnLead, string cnnDashboard, string cnnCommon, string configfile, Interfaces.IServiceLog log)
        {
            _log = log;
            _cnnAdmin = cnnAdmin;
            _cnnLead = cnnLead;
            _cnnDashboard = cnnDashboard;
            _connectionString = cnnCommon;
            _pluginFile = configfile; 
        }
        
        //public void Load()
        //{
        //    const string K_ENABLED = "enabled";
        //    const string K_PATH = "path";
        //    const string K_NAME = "name";

            
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(_pluginFile);
        //    foreach (XmlNode node in doc.ChildNodes[1].ChildNodes)
        //    {
        //        bool bEnabled = false;
        //        bool.TryParse(node.Attributes[K_ENABLED].Value, out bEnabled);
        //        if (bEnabled)
        //        {
        //            string path = node.Attributes[K_PATH].Value;
                    
        //            //Set the path
        //            string pathx = System.IO.Path.GetDirectoryName(path);
        //            if(pathx==string.Empty)
        //                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);

        //            Assembly plugin = Assembly.LoadFrom(path);
        //            if (plugin != null)
        //                foreach (Type type in plugin.GetTypes())
        //                {
        //                    if (type.GetInterfaces().Contains(typeof(Interfaces.IClientTask)))
        //                    {
        //                        Interfaces.IClientTask task = (Interfaces.IClientTask)Activator.CreateInstance(type);
        //                        KeyValuePair<string, Interfaces.IClientTask> pair = new KeyValuePair<string, Interfaces.IClientTask>(path, task);
        //                        if (task != null)
        //                            _clients.Add(node.Attributes[K_NAME].Value, pair);
        //                        break;
        //                    }
        //                }
        //        }
        //    }

        //}
        public void Load()
        {
            const string K_ENABLED = "enabled", K_PATH = "path", K_NAME = "name";

            string WorkingPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            XDocument doc = XDocument.Load(_pluginFile);
            var data = from T in doc.Descendants("plugin")
                       select new
                       {
                           Name = T.Attribute(K_NAME) != null ? T.Attribute(K_NAME).Value : "",
                           Enabled = Convert.ToBoolean(T.Attribute(K_ENABLED) != null ? T.Attribute(K_ENABLED).Value : "false"),
                           Path = T.Attribute(K_PATH) != null ? T.Attribute(K_PATH).Value : ""
                       };

            foreach (var t in data.Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Path)))
            {
                if (t.Enabled)
                {
                    string path = string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(t.Path)) ?
                        System.IO.Path.Combine(WorkingPath, t.Path) :
                        t.Path;

                    _clients.Add(t.Name, new PluginItem(System.IO.Path.GetDirectoryName(path), GetInterface(path)));
                }
            }
        }
        Interfaces.IClientTask GetInterface(string path)
        {
            Interfaces.IClientTask task =null;
            Assembly plugin = Assembly.LoadFrom(path);
            if (plugin != null)
                foreach (Type type in plugin.GetTypes())
                {
                    if (type.GetInterfaces().Contains(typeof(Interfaces.IClientTask)))
                    {
                        task = (Interfaces.IClientTask)Activator.CreateInstance(type);
                        if (task != null)
                        break;
                    }
                }
            return task;
        }

        public void Unload(bool bForce = false)
        {
            FreeThreads(bForce);

            foreach (var client in _clients)
                client.Value.Task.Dispose();
            _clients.Clear();
        }
        
        void FreeThreads(bool bForce = false)
        {
            try
            {
                if (bForce)
                {
                    foreach (Thread th in _threads)
                        th.Abort();
                    _threads.Clear();
                }
                else
                {
                    List<Thread> deadIndices = new List<Thread>();
                    for (int i = 0; i < _threads.Count; i++)
                        if (!_threads[i].IsAlive)
                            deadIndices.Add(_threads[i]);

                    foreach (var th in deadIndices)
                        _threads.Remove(th);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        
        public void Initialize()
        {
            foreach (var client in _clients)
            {
                Task task = new Task(client.Value.Task, _cnnAdmin, _cnnLead, _cnnDashboard, _connectionString, _log, client.Value.Path);
                Thread othread = new Thread(new ThreadStart(task.Init));
                _threads.Add(othread);
                othread.Start();
            }
        }
        public void Execute()
        {
            foreach (var item in _clients)
            {
                Task task = new Task(item.Value.Task, _cnnAdmin, _cnnLead, _cnnDashboard, _connectionString, _log, item.Value.Path);
                Thread othread = new Thread(new ThreadStart(task.Execute));
                _threads.Add(othread);
                othread.Start();
            }
            FreeThreads();
        }


        public int Tasks { get { return _clients.Count; } }
        public int Threads { get { return _threads.Count; } }
    }
}
