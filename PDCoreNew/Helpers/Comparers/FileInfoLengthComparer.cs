using System.Collections.Generic;
using System.IO;

namespace PDCoreNew.Helpers.Comparers
{
    public class FileInfoLengthComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            return y.Length.CompareTo(x.Length);
        }
    }
}
