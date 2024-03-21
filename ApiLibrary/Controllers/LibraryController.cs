using ApiLibrary.Models;
using ApiLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ApiLibrary.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LibraryController : ApiController
    {
        [Route("api/library/paths")]
        public async Task<IHttpActionResult> GetPaths()
        {
            
            ppa_entities context = new ppa_entities();
            var query = from x in context.ViewBookFiles
                         
                        select x;

            var result = await query.OrderBy(q => q.FilePath).ToListAsync();


            return Ok(result);
        }
        [Route("api/library/search/{str}/{eid}")]
        public async Task<IHttpActionResult> GetSearch(string str,int eid)
        {
            str = str.ToLower();
            if (str.Length < 2)
                return Ok(new List<ViewBookFile>());
            ppa_entities context = new ppa_entities();
            var query = from x in context.ViewBookFiles
                        //where x.FilePath.ToLower().Contains(str)
                        select x;
            var prts = str.Split(' ').ToList();
            foreach (var prt in prts)
            {
                query = query.Where(q => q.FilePath.ToLower().Contains(prt));
            }

            var access = await context.HelperBookApplicableEmployees.Where(q => q.EmployeeId == eid).Select(q=>q.BookId).ToListAsync();

            var result = await query.Where(q=>access.Contains(q.BookId)).OrderBy(q => q.FilePath).ToListAsync();


            return Ok(result);
        }


        [Route("api/library/employee/folder/{eid}/{fid}/{pid}")]
       
        // [Authorize]
        public async Task<IHttpActionResult> GetLibraryEmployeeFolder(int pid, int fid, int eid)
        {
            try
            {
                ppa_entities context = new ppa_entities();
                //hook
                int? parentId = pid == -1 ? null : (Nullable<int>)pid;
                //var folders = unitOfWork.ViewFolderApplicableRepository.GetQuery().Where(q => q.EmployeeId == eid && q.ParentId == parentId).OrderBy(q => q.FullCode).ToList();
                var _folders = await context.ViewFolderApplicables.Where(q => q.EmployeeId == eid).ToListAsync();
                var folders = _folders.Where(q => q.ParentId == pid).OrderBy(q => q.FullCode).ToList();
                //var items = unitOfWork.BookRepository.GetViewBookApplicableEmployee().Where(q => q.FolderId == fid && q.EmployeeId == eid).OrderBy(q => q.Title).ToList();
                var items = await context.ViewBookApplicableEmployees.Where(q => q.FolderId == fid && q.EmployeeId == eid).OrderBy(q => q.Title).ToListAsync();
                var ids = items.Select(q => q.BookId).ToList();
                //var files = await unitOfWork.BookRepository.GetBookFiles(ids, eid);
                var files= await  context.ViewBookFileVisiteds.Where(q => q.EmployeeId == eid && ids.Contains(q.BookId)).ToListAsync();
                var _ids = ids.Select(q => (Nullable<int>)q).ToList();
                var chapters= await  context.ViewBookChapters.Where(q => _ids.Contains(q.BookId)).ToListAsync();
                //var chapters = await unitOfWork.BookRepository.GetBookChapters(ids);


                var result = new
                {
                    folders,
                    items,
                    files,
                    chapters
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

        }

        public class FileItemModel
        {
            public bool IsDirectory { get; set; }
            public DateTime LastModified { get; set; }
            public long Size { get; set; }
            public bool HasSubfolder { get; set; }
            public string Name { get; set; }
            public string Ext { get; set; }
            public string Path { get; set; }
        }

        bool HasSubfolder(string path)
        {
            try
            {
                IEnumerable<string> subfolders = Directory.EnumerateDirectories(path);
                return subfolders != null && subfolders.Any();
            }
            catch
            {
                return false;
            }
        }
        List<FileItemModel> GetChildren(string path)
        {
            List<FileItemModel> files = new List<FileItemModel>();

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (DirectoryInfo dc in di.GetDirectories())
            {
                files.Add(new FileItemModel()
                {
                    Name = dc.Name,
                    Path = dc.FullName,
                    IsDirectory = true,
                    HasSubfolder = HasSubfolder(dc.FullName),
                    LastModified = dc.LastWriteTime
                });
            }

            foreach (FileInfo fi in di.GetFiles())
            {
                files.Add(new FileItemModel()
                {
                    Name = fi.Name,
                    Ext = fi.Extension.Length > 1 ? fi.Extension.Substring(1).ToLower() : "",
                    Path = fi.FullName,
                    IsDirectory = false,
                    LastModified = fi.LastWriteTime,
                    Size = fi.Length
                });
            }
            return files;
        }


        [Route("api/FileExplorer/GetPath")]
        public IHttpActionResult GetPath(string path)
        {
            path = HttpContext.Current.Server.UrlDecode(path);
            path = path.Replace("|", ":");
            List<dynamic> result = new List<dynamic>();
            if (path == "/")
            {
                var allDrives = DriveInfo.GetDrives();
                foreach (var drive in allDrives)
                {
                    result.Add(new
                    {
                        label = drive.Name,
                        path = drive.RootDirectory.FullName.Replace(":", "|"),
                        isDrive = true,
                        isFolder = true,
                        hasSubfolder = HasSubfolder(drive.RootDirectory.FullName),
                        subitems = new string[] { "Total: " + drive.TotalSize.ToString("n0"), "Free: " + drive.AvailableFreeSpace.ToString("n0") }
                    });
                }
            }
            else if (Directory.Exists(path))
            {
                List<FileItemModel> content = GetChildren(path);
                foreach (var item in content)
                {
                    result.Add(new
                    {
                        label = item.Name,
                        path = item.Path.Replace(":", "|"),
                        ext = item.Ext,
                        isFolder = item.IsDirectory,
                        hasSubfolder = item.HasSubfolder,
                        subitems = new string[] { item.LastModified.ToString(), item.IsDirectory ? "" : item.Size.ToString("n0") }
                    });
                }
            }
            //else if (System.IO.File.Exists(path))
            //{
            //    try
            //    {
            //        var rules = System.IO.File.GetAccessControl(path).GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            //        foreach (FileSystemAccessRule rule in rules)
            //        {
            //            if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
            //            {
            //                return File(path, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(path));
            //            }
            //        }
            //        return PartialView("Error", "Access Denied: " + path);
            //    }
            //    catch (Exception ex)
            //    {
            //        return PartialView("Error", "Access Denied: " + path);
            //    }
            //}

            // HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            // return Json(result, JsonRequestBehavior.AllowGet);
            return Ok(result);
        }


        //////end of cpntroller
    }

    



   






}
