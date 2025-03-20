using System;
using System.Linq;

namespace SalesTool.Logging
{
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method)]
    public sealed class Journal: Attribute
    {
        public Journal(string function, string message)
        {
#if DEBUG 
            using (Logging log = Logging.Instance)
            {
                log.Write(AuditEvent.Other, string.Format("{0} : {1}", function, message));
            }
#endif
        }
    }
}
