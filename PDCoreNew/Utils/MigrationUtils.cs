using System;
using System.IO;
using System.Reflection;

namespace PDCoreNew.Utils
{
    public static class MigrationUtils
    {
        /// <summary>
        /// Read a SQL script that is embedded into a resource.
        /// </summary>
        /// <param name="migrationType">The migration type the SQL file script is attached to.</param>
        /// <param name="sqlFileName">The embedded SQL file name.</param>
        /// <returns>The content of the SQL file.</returns>
        public static string ReadSql(Type migrationType, string sqlFileName)
        {
            return ReadSql(migrationType.Assembly, migrationType.Namespace, sqlFileName);
        }

        public static string ReadSql(Assembly assembly, string typeNamespace, string sqlFileName)
        {
            string resourceName = $"{typeNamespace}.{sqlFileName}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new FileNotFoundException($"Unable to find the SQL file from an embedded resource {resourceName}", resourceName);
            }

            using var reader = new StreamReader(stream);

            string content = reader.ReadToEnd();

            return content;
        }
    }
}
