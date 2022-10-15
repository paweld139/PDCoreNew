using FluentFTP;
using Microsoft.Extensions.Logging;
using PDCoreNew.Extensions;
using PDCoreNew.Services.IServ;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PDCoreNew.Services.FileStorage
{
    public class WebFileStorageService : IFileStorageService
    {
        private readonly ILogger<WebFileStorageService> logger;
        private readonly NetworkCredential networkCredential;

        public WebFileStorageService(ILogger<WebFileStorageService> logger, string login, string password)
        {
            this.logger = logger;

            networkCredential = new NetworkCredential(login, password);
        }

        private static void FixUrl(ref string url)
        {
            url = url.FixPathSlashes();
        }

        private async Task<AsyncFtpClient> GetAsyncFtpClient(string path)
        {
            FixUrl(ref path);

            path = new Uri(path).GetLeftPart(UriPartial.Authority);

            AsyncFtpClient client = new(path, networkCredential);

            await client.AutoConnect();

            return client;
        }

        public async ValueTask CreateFolder(string targetDirectory)
        {
            var client = await GetAsyncFtpClient(targetDirectory);

            await client.CreateDirectory(targetDirectory);
        }

        public async ValueTask DeleteFile(string targetDirectory)
        {
            var client = await GetAsyncFtpClient(targetDirectory);

            await client.DeleteFile(targetDirectory);
        }

        public async ValueTask DeleteFolder(string targetDirectory)
        {
            var client = await GetAsyncFtpClient(targetDirectory);

            await client.DeleteDirectory(targetDirectory);
        }

        public async ValueTask<bool> FileExists(string targetDirectory)
        {
            var client = await GetAsyncFtpClient(targetDirectory);

            return await client.FileExists(targetDirectory);
        }

        public async ValueTask<bool> FolderExists(string targetDirectory)
        {
            var client = await GetAsyncFtpClient(targetDirectory);

            return await client.DirectoryExists(targetDirectory);
        }

        public async ValueTask<bool> FolderIsEmpty(string targetDirectory)
        {
            bool result = false;

            var client = await GetAsyncFtpClient(targetDirectory);

            await foreach (FtpListItem item in client.GetListingEnumerable())
            {
                result = true;

                break;
            }

            return result;
        }

        public async ValueTask RenameFolder(string oldFolderTargetName, string newFolderTargetName)
        {
            var client = await GetAsyncFtpClient(oldFolderTargetName);

            await client.MoveDirectory(oldFolderTargetName, newFolderTargetName);
        }

        public string Combine(params string[] paths)
        {
            return new Uri(paths.First()).Append(paths.Skip(1).ToArray()).AbsoluteUri;
        }

        public void SaveFile(string filePath, byte[] data) => SaveFileAsyncTask(filePath, data).Wait();

        public async Task SaveFile(string filePath, string fileContent)
        {
            var client = await GetAsyncFtpClient(filePath);

            using var stream = new MemoryStream();

            using var streamWriter = new StreamWriter(stream, Encoding.UTF8);

            await streamWriter.WriteAsync(fileContent);

            await client.UploadStream(stream, filePath);
        }

        public async ValueTask<byte[]> Download(string targetDirectory)
        {
            var client = await GetAsyncFtpClient(targetDirectory);

            return await client.DownloadBytes(targetDirectory, CancellationToken.None);
        }

        public async Task SaveFileAsyncTask(string filePath, byte[] data)
        {
            var client = await GetAsyncFtpClient(filePath);

            await client.UploadBytes(data, filePath);
        }

        public async ValueTask<long> GetFileSize(string filePath)
        {
            var client = await GetAsyncFtpClient(filePath);

            return await client.GetFileSize(filePath);
        }

        public async ValueTask<DateTime> GetFileCreationTime(string filePath)
        {
            var client = await GetAsyncFtpClient(filePath);

            var info = await client.GetObjectInfo(filePath);

            return info.Created.ToUniversalTime();
        }
    }
}
