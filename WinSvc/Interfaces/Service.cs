using System;
using System.Linq;

namespace SalesTool.Service.Interfaces
{
    /// <summary>
    /// Allows the plugin to control various aspects of the service
    /// </summary>
	public interface IService
	{
	}
    /// <summary>
    /// It allows the plugin to access the loging features provided by the service
    /// </summary>
    public interface IServiceLog
    {
        void Error(string text);
        void Information(string text);
        void Warning(string text);
        void Debug(string text);
        string ServicePath{get;}
    }

    /// <summary>
    /// Allows the simplified access to the database. Do not store the connections, they are temporary
    /// </summary>
    public interface IServiceDataAccess
    {
        bool IsValid { get; }
        SalesTool.DataAccess.DBEngine Engine { get; }
        System.Data.IDbConnection Connection { get; }
    }


}
