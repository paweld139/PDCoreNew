using System;
using System.Threading.Tasks;

namespace PDCoreNew.Services.IServ
{
    public interface IFileStorageService
    {
        string Combine(params string[] paths);
        ValueTask CreateFolder(string targetDirectory);
        ValueTask DeleteFile(string targetDirectory);
        ValueTask DeleteFolder(string targetDirectory);
        ValueTask<byte[]> Download(string targetDirectory);
        ValueTask<bool> FileExists(string targetDirectory);
        ValueTask<bool> FolderExists(string targetDirectory);
        ValueTask<bool> FolderIsEmpty(string targetDirectory);
        ValueTask<DateTime> GetFileCreationTime(string filePath);
        ValueTask<long> GetFileSize(string filePath);
        ValueTask RenameFolder(string oldFolderTargetName, string newFolderTargetName);
        void SaveFile(string filePath, byte[] data);
        Task SaveFile(string filePath, string fileContent);
        Task SaveFileAsyncTask(string filePath, byte[] data);
    }
}