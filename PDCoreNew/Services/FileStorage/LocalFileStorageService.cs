using PDCoreNew.Services.IServ;
using PDCoreNew.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDCoreNew.Services.FileStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        public ValueTask CreateFolder(string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);

            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteFile(string targetDirectory)
        {
            File.Delete(targetDirectory);

            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteFolder(string targetDirectory)
        {
            Directory.Delete(targetDirectory, true);

            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> FileExists(string targetDirectory)
        {
            return ValueTask.FromResult(File.Exists(targetDirectory));
        }

        public ValueTask<bool> FolderExists(string targetDirectory)
        {
            return ValueTask.FromResult(Directory.Exists(targetDirectory));
        }

        public ValueTask<bool> FolderIsEmpty(string targetDirectory)
        {
            return ValueTask.FromResult(!Directory.EnumerateFileSystemEntries(targetDirectory).Any());
        }

        public ValueTask RenameFolder(string oldFolderTargetName, string newFolderTargetName)
        {
            Directory.Move(oldFolderTargetName, newFolderTargetName);

            return ValueTask.CompletedTask;
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
            using var fileStream = new FileStream(filePath, FileMode.Create);

            using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

            await streamWriter.WriteAsync(fileContent);
        }

        public ValueTask<byte[]> Download(string targetDirectory)
        {
            return ValueTask.FromResult(File.ReadAllBytes(targetDirectory));
        }

        public Task SaveFileAsyncTask(string filePath, byte[] data)
        {
            return IOUtils.WriteAllBytesAsync(filePath, data);
        }

        public ValueTask<long> GetFileSize(string filePath) => ValueTask.FromResult(new FileInfo(filePath).Length);

        public ValueTask<DateTime> GetFileCreationTime(string filePath) => ValueTask.FromResult(new FileInfo(filePath).CreationTimeUtc);
    }
}
