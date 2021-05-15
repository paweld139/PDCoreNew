using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PDCore.Helpers.IO
{
    public class RelativePath
    {
        public string PathSeparator { get; }

        private readonly List<string> _folders;

        public RelativePath(IEnumerable<string> folders, string pathSeparator = null)
        {
            PathSeparator = pathSeparator ?? Path.DirectorySeparatorChar.ToString();

            _folders = new List<string>(folders);
        }

        public string ApplicationRootPath { get; private set; }

        public void SetApplicationRootPath(string applicationRootPath)
        {
            ApplicationRootPath = applicationRootPath;
        }

        public string JoinedFolders => string.Join(PathSeparator, _folders);

        public string PhysicalPath => Path.Combine(ApplicationRootPath, JoinedFolders);

        public void AddFolder(string folder)
        {
            _folders.Add(folder);
        }

        public string Pop()
        {
            if (_folders.Count == 0)
                return null;

            string lastName = _folders.Last();

            _folders.RemoveAt(_folders.Count - 1);

            return lastName;
        }
    }
}
