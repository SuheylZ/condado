using System.Data.SqlClient;

namespace SelectCARE.CallDataSync
{
    public class DatabaseSettings
    {

        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public string DataSource { get; private set; }


        public DatabaseSettings(string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            DatabaseName = connectionStringBuilder.InitialCatalog;
            DataSource = connectionStringBuilder.DataSource;
            ConnectionString = connectionString;
        }

        public DatabaseSettings(string applicationName, string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                ApplicationName = applicationName
            };
            DatabaseName = connectionStringBuilder.InitialCatalog;
            DataSource = connectionStringBuilder.DataSource;
            ConnectionString = connectionString;

        }
    }
}