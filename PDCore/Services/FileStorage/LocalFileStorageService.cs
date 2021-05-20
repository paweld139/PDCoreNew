using PDCore.Services.IServ;
using PDCore.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDCore.Services.FileStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        public void CreateFolder(string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);
        }

        public void DeleteFile(string targetDirectory)
        {
            File.Delete(targetDirectory);
        }

        public void DeleteFolder(string targetDirectory)
        {
            Directory.Delete(targetDirectory, true);
        }

        public bool FileExists(string targetDirectory)
        {
            return File.Exists(targetDirectory);
        }

        public bool FolderExists(string targetDirectory)
        {
            return Directory.Exists(targetDirectory);
        }

        public bool FolderIsEmpty(string targetDirectory)
        {
            return !Directory.EnumerateFileSystemEntries(targetDirectory).Any();
        }

        public void RenameFolder(string oldFolderTargetName, string newFolderTargetName)
        {
            Directory.Move(oldFolderTargetName, newFolderTargetName);
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public void SaveFile(string filePath, byte[] data)
        {
            File.WriteAllBytes(filePath, data);
        }

        public async Task SaveFile(string filePath, string fileContent)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                await streamWriter.WriteAsync(fileContent);
            }
        }

        public byte[] Download(string targetDirectory)
        {
            return File.ReadAllBytes(targetDirectory);
        }

        public Task SaveFileAsyncTask(string filePath, byte[] data)
        {
            return IOUtils.WriteAllBytesAsync(filePath, data);
        }

        public long GetFileSize(string filePath) => new FileInfo(filePath).Length;

        public DateTime GetFileCreationTime(string filePath) => new FileInfo(filePath).CreationTimeUtc;
    }
}
