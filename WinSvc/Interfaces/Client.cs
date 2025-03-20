using System;
using System.Linq;

namespace SalesTool.Service.Interfaces
{
    /// <summary>
    /// This interface is mandatory for the plugin. Without this interface the service will not recognize a plugin.
    /// </summary>
	public interface IClientTask: IDisposable
	{
		void Init(IServiceDataAccess db, IServiceLog log, string path);
		void Execute(IServiceDataAccess da, IServiceLog log, string path);
	}

    /// <summary>
    /// Implement this interface to convey information about the plugin. It is not required by the service.
    /// </summary>
    public interface IClientInformation
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
    }

    /// <summary>
    /// Implement this interface to make it easier for the service and other programs to query about the plugin
    /// It is not required by the service.
    /// </summary>
    public interface IClient
    {
        IClientTask Task { get; }
        IClientInformation Information { get; }
    }
}
