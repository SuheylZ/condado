using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.Common
{
    internal class Konstants
    {
        private const string K_APPLICATION_SERVICES = "ApplicationServices";

        internal static string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings[K_APPLICATION_SERVICES].ConnectionString.Trim();
            }
        }
    }
}
