using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using Newtonsoft.Json;
using PDCoreNew.Extensions;
using PDCoreNew.Helpers.DataLoaders;
using PDCoreNew.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace PDCoreNew.Utils
{
    public static class IOUtils
    {
        public static string Unzip(string strFileName, string strExtractTo)
        {
            using Stream stream = File.OpenRead(strFileName);

            return Unzip(stream, strExtractTo);
        }

        public static string Unzip(Stream stream, string strPathToExtract, bool setDateAndTime = false)
        {
            ZipInputStream s = new(stream);

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

        [SupportedOSPlatform("windows")]
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
            using FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate);

            sourceStream.CopyTo(fileStream);
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
            using FileStream sourceStream = File.Open(path, FileMode.OpenOrCreate);

            sourceStream.Seek(0, SeekOrigin.End);

            await sourceStream.WriteAsync(data.AsMemory(0, data.Length));
        }

        public static async Task<byte[]> ReadAllBytesAsync(string path)
        {
            byte[] result;

            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                result = new byte[stream.Length];

                await stream.ReadAsync(result.AsMemory(0, (int)stream.Length));
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

        public static async Task WriteAllTextAsync(string path, string text)
        {
            using var streamWriter = File.CreateText(path);

            await streamWriter.WriteAsync(text);
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

        public static ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            return extension.ToLower() switch
            {
                @".bmp" => ImageFormat.Bmp,
                @".gif" => ImageFormat.Gif,
                @".ico" => ImageFormat.Icon,
                @".jpg" or @".jpeg" => ImageFormat.Jpeg,
                @".png" => ImageFormat.Png,
                @".tif" or @".tiff" => ImageFormat.Tiff,
                @".wmf" => ImageFormat.Wmf,
                _ => throw new NotImplementedException(),
            };
        }

        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException(nameof(decimalPlaces)); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public async static Task<T> DeserializeJson<T>(string path)
        {
            string content = await File.ReadAllTextAsync(path);

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async static Task<(string output, string errorMsg)> ExecuteCommand(string fileName, string command, string workingDirectory = null)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory.EmptyIfNull()
            };

            string output = null, errorMsg = null;

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();

                output = await process.StandardOutput.ReadToEndAsync();
                errorMsg = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();
            }

            return (output, errorMsg);
        }

        public static Task<(string output, string errorMsg)> ExecuteBashCommand(string command, string workingDirectory = null)
        {
            return ExecuteCommand("/bin/bash", $"-c \"{command}\"", workingDirectory);
        }

        public static Task<(string output, string errorMsg)> ExecuteCmdCommand(string command, string workingDirectory = null)
        {
            return ExecuteCommand("cmd.exe", $"/C {command}", workingDirectory);
        }

        // This method accepts two strings the represent two files to
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the
        // files are not the same.
        public static bool FilesAreEqual(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is
            // equal to "file2byte" at this point only if the files are
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        public static void CloneDirectory(string source, string destination, bool clean, bool isRoot = true)
        {
            var directoryInfo = new DirectoryInfo(source);

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                string directoryName = directory.Name;

                string directoryToSave = Path.Combine(destination, directoryName);

                if (!Directory.Exists(directoryToSave))
                {
                    Directory.CreateDirectory(directoryToSave);
                }

                CloneDirectory(directory.FullName, directoryToSave, clean, false);
            }

            foreach (var file in directoryInfo.EnumerateFiles())
            {
                string fileName = file.Name;

                string fileToSave = Path.Combine(destination, fileName);

                if (!File.Exists(fileToSave))
                {
                    if (clean)
                        file.MoveTo(fileToSave);
                    else
                        file.CopyTo(fileToSave);
                }
                else
                {
                    bool filesAreEqual = FilesAreEqual(file.FullName, fileToSave);

                    if (!filesAreEqual)
                    {
                        if (clean)
                        {
                            File.Delete(fileToSave);

                            file.MoveTo(fileToSave);
                        }
                        else
                        {
                            file.CopyTo(fileToSave, true);
                        }
                    }
                    else
                    {
                        file.Delete();
                    }
                }
            }

            if (clean && !isRoot)
                directoryInfo.Delete();
        }

        public static void ClearDirectory(string path)
        {
            var di = new DirectoryInfo(path);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
