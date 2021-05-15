using System.Configuration;

namespace PDCore.Utils
{
    public static class ConfigurationUtils
    {
        private const string connectionStringsSectionName = "connectionStrings";

        public static ConnectionStringSettings GetConnectionStringSettings(System.Configuration.Configuration configuration, string key)
        {
            return configuration.ConnectionStrings.ConnectionStrings[key];
        }

        public static ConnectionStringSettings GetConnectionStringSettings(string key)
        {
            return ConfigurationManager.ConnectionStrings[key];
        }

        public static string GetConnectionString(System.Configuration.Configuration configuration, string key)
        {
            var settings = GetConnectionStringSettings(configuration, key);

            return settings.ConnectionString;
        }

        public static string GetConnectionString(string key)
        {
            var settings = GetConnectionStringSettings(key);

            return settings.ConnectionString;
        }

        public static void SetConnectionString(System.Configuration.Configuration configuration, string key, string value)
        {
            var settings = GetConnectionStringSettings(configuration, key);

            settings.ConnectionString = value;
        }

        public static void SetConnectionString(string key, string value)
        {
            var settings = GetConnectionStringSettings(key);

            settings.ConnectionString = value;
        }

        public static void SaveConnectionString(System.Configuration.Configuration configuration, string connectionStringName, string connectionString)
        {
            SetConnectionString(configuration, connectionStringName, connectionString);

            configuration.Save();

            ConfigurationManager.RefreshSection(connectionStringsSectionName);
        }

        public static void SaveConnectionString(string connectionStringName, string connectionString)
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            SaveConnectionString(configuration, connectionStringName, connectionString);
        }

        public static void SaveConnectionString(string configFile, string connectionStringName, string connectionString)
        {
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = configFile }, ConfigurationUserLevel.None);

            SaveConnectionString(configuration, connectionStringName, connectionString);
        }
    }
}
