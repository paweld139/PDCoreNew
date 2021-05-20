using System;
using System.Threading.Tasks;

namespace PDCore.Services.IServ
{
    public interface IFileStorageService
    {
        string Combine(params string[] paths);
        void CreateFolder(string targetDirectory);
        void DeleteFile(string targetDirectory);
        void DeleteFolder(string targetDirectory);
        byte[] Download(string targetDirectory);
        bool FileExists(string targetDirectory);
        bool FolderExists(string targetDirectory);
        bool FolderIsEmpty(string targetDirectory);
        DateTime GetFileCreationTime(string filePath);
        long GetFileSize(string filePath);
        void RenameFolder(string oldFolderTargetName, string newFolderTargetName);
        void SaveFile(string filePath, byte[] data);
        Task SaveFile(string filePath, string fileContent);
        Task SaveFileAsyncTask(string filePath, byte[] data);
    }
}