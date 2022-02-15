using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PDCoreNew.Configuration;
using PDCoreNew.Context.IContext;
using PDCoreNew.Interfaces;
using PDCoreNew.Models;
using PDCoreNew.Repositories.IRepo;
using PDCoreNew.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.Repo
{
    public class FileRepository : SqlRepositoryEntityFrameworkCore<FileModel>, IFileRepository
    {
        private readonly string targetFolder;

        public FileRepository(IEntityFrameworkCoreDbContext ctx, ILogger<FileModel> logger, IMapper mapper,
            IOptions<FileStorageOptions> fileStorageOptions)
            : base(ctx, logger, mapper, null)
        {
            string defaultFolder = Path.Join(AppContext.BaseDirectory, "SampleData");

            targetFolder = ObjectUtils.FirstNotNullOrWhiteSpace(fileStorageOptions.Value.UploadUrl, defaultFolder);
        }

        public async Task AddFile(string name, int objId, ObjType objType, byte[] file)
        {
            FileModel filee = new()
            {
                Name = name,
                Extension = IOUtils.GetSimpleExtension(name),
                RefId = objId,
                RefGid = objType
            };

            Add(filee);

            await CommitAsync();

            await IOUtils.WriteAllBytesAsync(Path.Combine(targetFolder, filee.Id.ToString()), file);
        }

        public async Task AddFileFromObject(FileModel file)
        {
            Add(file);

            await CommitAsync();

            await IOUtils.WriteAllBytesAsync(Path.Combine(targetFolder, file.Id.ToString()), file.Data);
        }

        public async Task AddFileFromObjectsList(ICollection<FileModel> File)
        {
            AddRange(File);

            await CommitAsync();

            var tasks = new List<Task>();

            foreach (var item in File)
            {
                tasks.Add(IOUtils.WriteAllBytesAsync(Path.Combine(targetFolder, item.Id.ToString()), item.Data));
            }

            await Task.WhenAll(tasks);
        }

        public async Task AddFile(List<Tuple<string, int, ObjType, byte[]>> File)
        {
            List<FileModel> files = new();

            foreach (var item in File)
            {
                FileModel filee = new()
                {
                    Name = item.Item1,

                    Extension = IOUtils.GetSimpleExtension(item.Item1),
                    RefId = item.Item2,
                    RefGid = item.Item3
                };

                files.Add(filee);

                Add(filee);
            }

            await CommitAsync();

            int index = 0;

            var tasks = new List<Task>();

            foreach (var item in files)
            {
                tasks.Add(IOUtils.WriteAllBytesAsync(Path.Combine(targetFolder, item.Id.ToString()), File[index].Item4));

                index++;
            }

            await Task.WhenAll(tasks);
        }

        public async Task<string> GetFilePath(int objId, ObjType objType)
        {
            FileModel file = await base.FindAll().Where(f => f.RefId == objId && f.RefGid == objType).OrderByDescending(f => f.Id).FirstOrDefaultAsync();
            string targetPath = Path.Combine(targetFolder, file.Id.ToString());

            return targetPath;
        }

        public string GetFilePath(int imgId)
        {
            string targetPath = Path.Combine(targetFolder, imgId.ToString());

            return targetPath;
        }

        public async Task<string> GetFileName(int imgId)
        {
            FileModel file = await base.FindAll().FirstOrDefaultAsync(f => f.Id == imgId);

            string fileName = file.Name;

            return fileName;
        }

        public async Task<string> GetFileName(int objId, ObjType objType)
        {
            FileModel file = await base.FindAll().Where(f => f.RefId == objId && f.RefGid == objType).OrderByDescending(f => f.Id).FirstOrDefaultAsync();

            string fileName = file.Name;

            return fileName;
        }

        public async Task Download(int imgId, string path)
        {
            var file = await FindAll().FirstOrDefaultAsync(f => f.Id == imgId);

            byte[] data = await GetFile(file.RefId, file.RefGid, false);

            await IOUtils.WriteAllBytesAsync(path, data);
        }

        public async Task<List<Tuple<int, int, byte[]>>> GetFile(ObjType objType)
        {
            try
            {
                List<Tuple<int, int, byte[]>> datas = new();

                List<Tuple<int, int, string>> paths = await GetFileNames2(objType);

                List<Task<byte[]>> tasks = paths.Select(x => IOUtils.ReadAllBytesAsync(x.Item3)).ToList();

                await Task.WhenAll(tasks);

                int i = 0;

                foreach (var item in paths)
                {
                    if (File.Exists(item.Item3))
                    {
                        datas.Add(new Tuple<int, int, byte[]>(item.Item1, item.Item2, await tasks[i]));
                    }

                    i++;
                }

                return datas;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Dictionary<int, byte[]>> GetFile(List<int> objId, ObjType objType)
        {
            try
            {
                Dictionary<int, byte[]> datas = new();

                Dictionary<int, string> paths = await GetFileNames(objId, objType);

                List<Task<byte[]>> tasks = paths.Select(x => IOUtils.ReadAllBytesAsync(x.Value)).ToList();

                await Task.WhenAll(tasks);

                int i = 0;

                foreach (var item in paths)
                {
                    datas.Add(item.Key, await tasks[i]);

                    i++;
                }

                try
                {
                    return datas;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Tuple<int, string, byte[]>>> GetFileWithNames(List<int> objId, ObjType objType)
        {
            try
            {
                List<Tuple<int, string, byte[]>> datas = new();

                List<Tuple<int, string, string>> paths = await GetFileNamesWithNames(objId, objType);

                List<Task<byte[]>> tasks = paths.Select(x => IOUtils.ReadAllBytesAsync(x.Item2)).ToList();

                await Task.WhenAll(tasks);

                int i = 0;

                foreach (var item in paths)
                {
                    datas.Add(new Tuple<int, string, byte[]>(item.Item1, item.Item3, await tasks[i]));

                    i++;
                }

                try
                {
                    return datas;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<dynamic> GetFile(int objId, ObjType objType, bool FileList = false)
        {
            try
            {
                if (!FileList)
                {
                    string fileName = await GetFileName(objId, objType);

                    return await IOUtils.ReadAllBytesAsync(fileName);
                }
                else
                {
                    return await GetFile(objId, objType);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<dynamic> GetObject(int objId, ObjType objType, bool FileList = false)
        {
            try
            {
                if (!FileList)
                    return await GetFileObject(objId, objType);
                else
                {
                    return await GetFileObjects(objId, objType);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<byte[]> GetFile(int imgId)
        {
            string targetPath = Path.Combine(targetFolder, imgId.ToString());

            return IOUtils.ReadAllBytesAsync(targetPath);
        }

        public async Task<List<byte[]>> GetFile(int objId, ObjType objType)
        {
            List<byte[]> datas = new();

            List<string> paths = await GetFileNames(objId, objType);


            List<Task<byte[]>> tasks = paths.Select(IOUtils.ReadAllBytesAsync).ToList();

            await Task.WhenAll(tasks);

            int i = 0;

            foreach (var item in paths)
            {
                datas.Add(await tasks[i]);

                i++;
            }

            try
            {
                return datas;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task RemoveFile(int imgId)
        {
            FileModel file = new() { Id = imgId };

            Attach(file);

            Delete(file);

            await CommitAsync();

            await IOUtils.RemoveFileAsync(GetFilePath(imgId));
        }

        public Task RemoveFilelocal(int imgId)
        {
            return IOUtils.RemoveFileAsync(GetFilePath(imgId));
        }

        public Task RemoveFilesLocal(IEnumerable<FileModel> files)
        {
            var ids = files.Select(f => f.Id);

            return RemoveFilesLocal(ids);
        }

        public Task RemoveFilesLocal(IEnumerable<int> fileIds)
        {
            var result = Task.CompletedTask;

            if (fileIds.Any())
            {
                var tasks = fileIds.Select(f => RemoveFilelocal(f));

                result = Task.WhenAll(tasks);
            }

            return result;
        }

        public Task AddFilelocal(int imgId, byte[] data)
        {
            return IOUtils.WriteAllBytesAsync(GetFilePath(imgId), data);
        }

        public Task AddFilesLocal<TEntity>(TEntity entity) where TEntity : IHasFiles
        {
            var result = Task.CompletedTask;

            if (entity.Files.Any())
            {
                var tasks = entity.Files.Select(f => AddFilelocal(f.Id, f.Data));

                result = Task.WhenAll(tasks);
            }

            return result;
        }

        public async Task RemoveFile(List<int> imgIds)
        {
            List<Task> tasks = imgIds.Select(x => IOUtils.RemoveFileAsync(GetFilePath(x))).ToList();

            foreach (int item in imgIds)
            {
                FileModel al = new() { Id = item };

                Attach(al);

                Delete(al);
            }

            await CommitAsync();

            await Task.WhenAll(tasks);
        }

        public async Task<List<string>> GetFileNames(int objId, ObjType objType)
        {
            List<FileModel> File = await FindAll().Where(f => f.RefId == objId && f.RefGid == objType).ToListAsync();

            List<string> targetPaths = new();

            foreach (var item in File)
            {
                targetPaths.Add(Path.Combine(targetFolder, item.Id.ToString()));
            }

            return targetPaths;
        }

        public async Task<Dictionary<int, string>> GetFileNames(ObjType objType)
        {
            List<FileModel> File = await FindAll().Where(f => f.RefGid == objType).GroupBy(x => x.RefId).Select(x => x.OrderByDescending(a => a.Id).FirstOrDefault()).ToListAsync();

            Dictionary<int, string> targetPaths = new();

            foreach (var item in File)
            {
                targetPaths.Add(item.RefId, Path.Combine(targetFolder, item.Id.ToString()));
            }

            return targetPaths;
        }

        public async Task<List<Tuple<int, int, string>>> GetFileNames2(ObjType objType)
        {
            List<FileModel> File = await FindAll().Where(f => f.RefGid == objType).GroupBy(x => x.RefId).Select(x => x.OrderByDescending(a => a.Id).FirstOrDefault()).ToListAsync();

            List<Tuple<int, int, string>> targetPaths = new();

            foreach (var item in File)
            {
                targetPaths.Add(new Tuple<int, int, string>(item.Id, item.RefId, Path.Combine(targetFolder, item.Id.ToString())));
            }

            return targetPaths;
        }

        public async Task<Dictionary<int, string>> GetFileNames(List<int> ids, ObjType objType)
        {
            List<FileModel> File = await FindAll().Where(f => f.RefGid == objType && ids.Contains(f.RefId)).GroupBy(x => x.RefId).Select(x => x.OrderByDescending(a => a.Id).FirstOrDefault()).ToListAsync();

            Dictionary<int, string> targetPaths = new();

            foreach (var item in File)
            {
                targetPaths.Add(item.RefId, Path.Combine(targetFolder, item.Id.ToString()));
            }

            return targetPaths;
        }

        public async Task<List<FileModel>> GetFileObjects(int refId, ObjType objType)
        {
            List<FileModel> File = await FindAll().OrderByDescending(x => x.Id).Where(f => f.RefGid == objType && f.RefId == refId).ToListAsync();

            foreach (var item in File)
            {
                item.Data = await IOUtils.ReadAllBytesAsync(Path.Combine(targetFolder, item.Id.ToString()));
            }

            return File;
        }

        public async Task<FileModel> GetFileObject(int refId, ObjType objType)
        {
            FileModel file = await base.FindAll().OrderByDescending(x => x.Id).FirstOrDefaultAsync(f => f.RefGid == objType && f.RefId == refId);

            file.Data = await IOUtils.ReadAllBytesAsync(Path.Combine(targetFolder, file.Id.ToString()));

            return file;
        }

        public async Task<FileModel> GetFileObject(int fileId)
        {
            FileModel file = await FindByIdAsync(fileId);

            file.Data = await IOUtils.ReadAllBytesAsync(Path.Combine(targetFolder, file.Id.ToString()));

            return file;
        }

        public async Task<List<Tuple<int, string, string>>> GetFileNamesWithNames(List<int> ids, ObjType objType)
        {
            List<FileModel> File = await FindAll().Where(f => f.RefGid == objType && ids.Contains(f.RefId)).GroupBy(x => x.RefId).Select(x => x.OrderByDescending(a => a.Id).FirstOrDefault()).ToListAsync();

            List<Tuple<int, string, string>> targetPaths = new();

            foreach (var item in File)
            {
                targetPaths.Add(new Tuple<int, string, string>(item.RefId, Path.Combine(targetFolder, item.Id.ToString()), item.Name));
            }

            return targetPaths;
        }
    }
}
