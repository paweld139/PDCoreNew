using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;

namespace PDCore.Common.Utils
{
    public static class IOUtils
    {
        public static void ToggleConfigEncryption(string sectionName = "connectionStrings")
        {
            // Takes the executable file name without the
            // .config extension.

            // Open the configuration file and retrieve
            // the connectionStrings section.
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            ConfigurationSection section = config.GetSection(sectionName);


            if (section.SectionInformation.IsProtected)
            {
                // Remove encryption.
                section.SectionInformation.UnprotectSection();
            }
            else
            {
                // Encrypt the section.
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            }

            // Save the current configuration.
            config.Save();
        }

        public const string DatabaseConnectionError = "Wystąpił błąd podczas łączenia z bazą {0}. Błąd: {1}";

        public static string CheckDatabaseStatus(params DbContext[] dbContexts)
        {
            List<string> statuses = new List<string>();

            foreach (var context in dbContexts)
            {
                var cnn = context.Database.Connection;

                try
                {
                    cnn.Open();
                }
                catch (Exception ex)
                {
                    statuses.Add(string.Format(DatabaseConnectionError, cnn.Database, ex.Message));
                }
                finally
                {
                    if (cnn.State != System.Data.ConnectionState.Closed)
                    {
                        cnn.Close();
                    }
                }
            }

            return string.Join(Environment.NewLine, statuses.ToArray());
        }
    }
}
