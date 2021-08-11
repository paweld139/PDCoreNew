using PDCoreNew.Extensions;
using System;
using System.IO;

namespace PDCoreNew.Helpers.IO
{
    public class FileLocation
    {
        public string DestinationVirtualRelativePath => RelativePath.PathSeparator + RelativePath.JoinedFolders;
        public string DestinationVirtualRelativePathWithFileName => Path.Combine(DestinationVirtualRelativePath, FileName);

        public string DestinationPhysicalRelativePath => RelativePath.JoinedFolders;
        public string DestinationPhysicalPath => Path.Combine(RelativePath.ApplicationRootPath, DestinationPhysicalRelativePath);
        public string DestinationPhysicalPathWithFileName => Path.Combine(DestinationPhysicalPath, FileName);

        public RelativePath RelativePath { get; }
        public string FileName { get; private set; }

        public FileLocation(RelativePath relativePath, string fileName = null)
        {
            RelativePath = relativePath;
            FileName = fileName;
        }

        public void ChangeFileName(string fileName)
        {
            FileName = fileName;
        }

        public static FileLocation FromSource(string source, string pathSeparator = null)
        {
            source = source
                .Replace("\\", pathSeparator)
                .Replace("/", pathSeparator);

            string fileName = Path.GetFileName(source);

            var folders = source.Substring(0, source.Length - fileName.Length).Split(StringSplitOptions.RemoveEmptyEntries, pathSeparator);

            var relativePath = new RelativePath(folders, pathSeparator);

            return new FileLocation(relativePath, fileName);
        }
    }
}
