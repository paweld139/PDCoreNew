using Microsoft.Extensions.Logging;
using PDCore.Extensions;
using PDCore.Services.IServ;
using PDCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PDCore.Services.FileStorage
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

        private void FixUrl(ref string url)
        {
            url = url.FixPathSlashes();
        }

        private FtpWebRequest GetFtpWebRequest(string path, string method)
        {
            FixUrl(ref path);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Method = method;
            request.Credentials = networkCredential;

            return request;
        }

        private FtpWebResponse GetFtpWebResponse(string path, string method)
        {
            var request = GetFtpWebRequest(path, method);

            return (FtpWebResponse)request.GetResponse();
        }

        private Stream GetFtpRequestStream(string path, string method)
        {
            var request = GetFtpWebRequest(path, method);

            return request.GetRequestStream();
        }

        private FtpStatusCode ExecuteFtpOperation(FtpWebRequest request)
        {
            using (var resp = (FtpWebResponse)request.GetResponse())
            {
                logger.LogInformation($"{ReflectionUtils.GetCallerMethodName()} status: {resp.StatusDescription}");

                return resp.StatusCode;
            }
        }

        private FtpStatusCode ExecuteFtpOperation(string path, string method)
        {
            var request = GetFtpWebRequest(path, method);

            return ExecuteFtpOperation(request);
        }

        public void CreateFolder(string targetDirectory)
        {
            ExecuteFtpOperation(targetDirectory, WebRequestMethods.Ftp.MakeDirectory);
        }

        public void DeleteFile(string targetDirectory)
        {
            ExecuteFtpOperation(targetDirectory, WebRequestMethods.Ftp.DeleteFile);
        }

        public void DeleteFolder(string targetDirectory)
        {
            ExecuteFtpOperation(targetDirectory, WebRequestMethods.Ftp.RemoveDirectory);
        }

        public bool FileExists(string targetDirectory)
        {
            bool result = true;

            try
            {
                var request = GetFtpWebRequest(targetDirectory, WebRequestMethods.Ftp.GetDateTimestamp);

                request.UseBinary = true;

                ExecuteFtpOperation(request);
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;

                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable || response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailableOrBusy)
                {
                    result = false;
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        public bool FolderExists(string targetDirectory)
        {
            bool result = true;

            try
            {
                ExecuteFtpOperation(targetDirectory, WebRequestMethods.Ftp.ListDirectory);
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;

                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable || response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailableOrBusy)
                {
                    result = false;
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        public bool FolderIsEmpty(string targetDirectory)
        {
            List<string> res = new List<string>();

            var request = GetFtpWebRequest(targetDirectory, WebRequestMethods.Ftp.ListDirectory);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var streamReader = new StreamReader(stream))
                while (!streamReader.EndOfStream)
                {
                    res.Add(streamReader.ReadLine());
                }

            return res.Count <= 2;
        }

        public void RenameFolder(string oldFolderTargetName, string newFolderTargetName)
        {
            var request = GetFtpWebRequest(oldFolderTargetName, WebRequestMethods.Ftp.Rename);

            request.RenameTo = newFolderTargetName.Split("/").Last();

            ExecuteFtpOperation(request);
        }

        public string Combine(params string[] paths)
        {
            return new Uri(paths.First()).Append(paths.Skip(1).ToArray()).AbsoluteUri;
        }

        public void SaveFile(string filePath, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var ftpStream = GetFtpRequestStream(filePath, WebRequestMethods.Ftp.UploadFile))
            {
                memoryStream.CopyTo(ftpStream);
            }
        }

        public async Task SaveFile(string filePath, string fileContent)
        {
            using (var ftpStream = GetFtpRequestStream(filePath, WebRequestMethods.Ftp.UploadFile))
            using (var streamWriter = new StreamWriter(ftpStream, Encoding.UTF8))
            {
                await streamWriter.WriteAsync(fileContent);
            }
        }

        public byte[] Download(string targetDirectory)
        {
            var request = GetFtpWebRequest(targetDirectory, WebRequestMethods.Ftp.DownloadFile);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
                return stream.ReadFully();
        }

        public async Task SaveFileAsyncTask(string filePath, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var ftpStream = GetFtpRequestStream(filePath, WebRequestMethods.Ftp.UploadFile))
            {
                await memoryStream.CopyToAsync(ftpStream);
            }
        }

        public long GetFileSize(string filePath)
        {
            using (var response = GetFtpWebResponse(filePath, WebRequestMethods.Ftp.GetFileSize))
                return response.ContentLength;
        }

        public DateTime GetFileCreationTime(string filePath)
        {
            using (var response = GetFtpWebResponse(filePath, WebRequestMethods.Ftp.GetDateTimestamp))
                return response.LastModified.ToUniversalTime();
        }
    }
}
