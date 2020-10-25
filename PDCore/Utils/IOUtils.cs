using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using PDCore.Extensions;
using PDCore.Helpers.DataLoaders;
using PDCore.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDCore.Utils
{
    public static class IOUtils
    {
        public static string Unzip(string strFileName, string strExtractTo)
        {
            using (Stream stream = File.OpenRead(strFileName))
            {
                return Unzip(stream, strExtractTo);
            }
        }

        public static string Unzip(Stream stream, string strPathToExtract, bool setDateAndTime = false)
        {
            ZipInputStream s = new ZipInputStream(stream);

            ZipEntry theEntry;

            byte[] data = new byte[2048];

            string fileName = string.Empty;


            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = Path.GetDirectoryName(theEntry.Name);

                fileName = Path.GetFileName(theEntry.Name);


                Directory.CreateDirectory(Path.Combine(strPathToExtract, directoryName));

                if (!string.IsNullOrEmpty(fileName))
                {
                    using (FileStream streamWriter = File.Create(Path.Combine(strPathToExtract, theEntry.Name)))
                    {
                        int size = 2048;

                        while (size > 0)
                        {
                            size = s.Read(data, 0, data.Length);

                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                        }

                        streamWriter.Close();
                    }

                    if (setDateAndTime)
                    {
                        // Set date and time
                        File.SetCreationTime(Path.Combine(strPathToExtract, theEntry.Name), theEntry.DateTime);

                        File.SetLastAccessTime(Path.Combine(strPathToExtract, theEntry.Name), theEntry.DateTime);

                        File.SetLastWriteTime(Path.Combine(strPathToExtract, theEntry.Name), theEntry.DateTime);
                    }

                    if (fileName.EndsWith(".zip"))
                    {
                        Unzip(Path.Combine(strPathToExtract, theEntry.Name), Path.Combine(strPathToExtract, Path.GetFileNameWithoutExtension(theEntry.Name)));

                        File.Delete(Path.Combine(strPathToExtract, theEntry.Name));
                    }
                }
            }

            return fileName;
        }

        public static IEnumerable<FileInfo> GetLargeFiles(string path, int maxFilesCount)
        {
            var query = from file in new DirectoryInfo(path).GetFiles()
                        orderby file.Length descending
                        select file;

            return query.Take(maxFilesCount);
        }

        public static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";

            string ext = Path.GetExtension(fileName).ToLower();

            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext);

            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();

            //string mimeType = Registry.GetValue(@"HKEY_CLASSES_ROOT\.pdf", "Content Type", null) as string;

            return mimeType;
        }

        public static int GetFilesCount(string path, bool allDirectories = false, bool throwIfDirectoryNotExists = false)
        {
            var searchOption = allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            int filesCount = 0;

            if (Directory.Exists(path) || throwIfDirectoryNotExists)
            {
                filesCount = Directory.GetFiles(path, "*.*", searchOption).Length;
            }

            return filesCount;
        }

        public static IEnumerable<KeyValuePair<string, int>> GetProcessesWithThreads()
        {
            Process[] processes = Process.GetProcesses();

            return processes.GetKVP(p => p.ProcessName, p => p.Threads.Count);
        }

        public static Dictionary<string, int> GetProcessesWithThreadsDictionary()
        {
            return GetProcessesWithThreads().ToDictionary();
        }

        public static SortedDictionary<string, int> GetProcessesWithThreadsSortedDictionary()
        {
            return GetProcessesWithThreads().ToSortedDictionary();
        }

        public static SortedList<string, int> GetProcessesWithThreadsSortedList()
        {
            return GetProcessesWithThreads().ToSortedList();
        }

        public static void SaveFile(string fileName, Stream sourceStream)
        {
            using (FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate))
            {
                sourceStream.CopyTo(fileStream);
            }
        }

        /// <summary>
        ///   Gets the extension without the dot
        /// </summary>
        /// <param name = "fileName">Name of the file.</param>
        /// <returns></returns>
        public static string GetSimpleExtension(string fileName)
        {
            return Path.GetExtension(fileName).Replace(".", "");
        }

        public static async Task WriteAllBytesAsync(string path, byte[] data)
        {
            using (FileStream SourceStream = File.Open(path, FileMode.OpenOrCreate))
            {
                SourceStream.Seek(0, SeekOrigin.End);

                await SourceStream.WriteAsync(data, 0, data.Length);
            }
        }

        public static async Task<byte[]> ReadAllBytesAsync(string path)
        {
            byte[] result;

            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                result = new byte[stream.Length];

                await stream.ReadAsync(result, 0, (int)stream.Length);
            }

            return result;
        }

        public static async Task<string> ReadAllTextAsync(string path)
        {
            string result;

            using (StreamReader stream = File.OpenText(path))
            {
                result = await stream.ReadToEndAsync();
            }

            return result;
        }

        /// <summary>
        /// This is the same default buffer size as
        /// <see cref="StreamReader"/> and <see cref="FileStream"/>.
        /// </summary>
        private const int DefaultBufferSize = 4096;

        /// <summary>
        /// Indicates that
        /// 1. The file is to be used for asynchronous reading.
        /// 2. The file is to be accessed sequentially from beginning to end.
        /// </summary>
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public static Task<string[]> ReadAllLinesAsync(string path)
        {
            return ReadAllLinesAsync(path, Encoding.UTF8);
        }

        public static async Task<string[]> ReadAllLinesAsync(string path, Encoding encoding)
        {
            var lines = new List<string>();

            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }

        public static Task RemoveFileAsync(string path)
        {
            return Task.Run(() => { File.Delete(path); });
        }

        public static IObservable<TProperty> ObservePropertyChanges<TProperty>(Expression<Func<TProperty>> property, object sender)
        {
            PropertyInfo propertyInfo = (PropertyInfo)((MemberExpression)property.Body).Member;

            return Observable
                .FromEventPattern(sender, "PropertyChanged")
                .Where(prop => ((PropertyChangedEventArgs)prop.EventArgs).PropertyName == propertyInfo.Name)
                .Select(x => propertyInfo.GetValue(sender, null))
                .DistinctUntilChanged()
                .Cast<TProperty>();
        }

        public static IDataLoader GetLoaderFor(string source)
        {
            IDataLoader loader;

            if (source.IsUrl())
            {
                loader = new WebLoader(source);
            }
            else
            {
                loader = new LocalLoader(source);
            }

            return loader;
        }
    }
}
