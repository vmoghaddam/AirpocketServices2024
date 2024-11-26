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
        [HttpPost]
        [Route("api/library/visit/file/{employeeId}/{bookfileId}")]
        public async Task<IHttpActionResult> PostVisitFile(int employeeId, int bookfileId)
        {

            ppa_entities context = new ppa_entities();
            var status = await  context.BookFileVisits.Where(q => q.EmployeeId == employeeId && q.BookFileId == bookfileId).FirstOrDefaultAsync();
            if (status == null)
            {
                status = new BookFileVisit()
                {
                    EmployeeId = employeeId,
                    BookFileId = bookfileId,
                    DateVisited = DateTime.Now,

                };

                context.BookFileVisits.Add(status);

                await context.SaveChangesAsync();
            }
            return Ok(true);
        }
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
        public async Task<IHttpActionResult> GetSearch(string str, int eid)
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

            var access = await context.HelperBookApplicableEmployees.Where(q => q.EmployeeId == eid).Select(q => q.BookId).ToListAsync();

            var result = await query.Where(q => access.Contains(q.BookId)).OrderBy(q => q.FilePath).ToListAsync();


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
                var files = await context.ViewBookFileVisiteds.Where(q => q.EmployeeId == eid && ids.Contains(q.BookId)).ToListAsync();
                var _ids = ids.Select(q => (Nullable<int>)q).ToList();
                var chapters = await context.ViewBookChapters.Where(q => _ids.Contains(q.BookId)).ToListAsync();
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

        public class Item
        {
            public string Id { get; set; }
            public string ParentId { get; set; }
            public string Text { get; set; }
            public bool IsFile { get; set; }
            public int FilesCount { get; set; }


            public bool FileVisible { get; set; }
            public string Url { get; set; }

            public int Version { get; set; }
            public string FileId { get; set; }
            public string BookId { get; set; }

        }

        [HttpGet]
        [Route("api/library/employee/{eid}")]
        public async Task<IHttpActionResult> GetLibraryEmployee(int eid)
        {
            try
            {

                ppa_entities context = new ppa_entities();
                List<Item> result = new List<Item>();
                //var _folders = await context.ViewFolderApplicables.Where(q => q.EmployeeId == eid).ToListAsync();
                //var books = await context.ViewBookApplicableEmployees.Where(q => q.EmployeeId == eid).ToListAsync();

                var qu = from x in context.ViewFolderApplicables
                         where x.EmployeeId == eid
                         select new
                         {
                             x.Id,
                             //x.EmployeeId,
                             x.ParentId,
                             Text = x.Title,
                             IsFile = false,
                             Url = "",
                             Version = -1,
                             BookId = -1,
                             FileVisible = false,
                             FileId = -1,
                             FilesCount = 2,
                         };

                var query = from x in context.ViewBookApplicableEmployees
                            where x.EmployeeId == eid
                            select new
                            {
                                Id = x.IDNo,
                                //x.EmployeeId,
                                ParentId = x.FolderId,
                                Text = x.Title,
                                IsFile = true,
                                Url = x.FileUrl,
                                x.BookId,
                                Version = -1,
                                FileVisible = false,
                                FileId = -1,
                                FilesCount = 0,
                            };

                foreach (var item in query)
                {
                    var test = new Item();
                    test.Id = item.Id;
                    test.Text = item.Text;
                    test.Url = item.Url;
                    test.Version = item.Version;
                    test.IsFile = item.IsFile;
                    test.FileVisible = item.FileVisible;
                    test.ParentId = item.ParentId.ToString();
                    test.BookId = item.BookId.ToString();
                    test.FileId = item.FileId.ToString();
                    test.FilesCount = item.FilesCount;
                    result.Add(test);
                }

                foreach (var item2 in query)
                {
                    var test2 = new Item();
                    test2.Id = item2.Id;
                    test2.Text = item2.Text;
                    test2.Url = item2.Url;
                    test2.Version = item2.Version;
                    test2.IsFile = item2.IsFile;
                    test2.FileVisible = item2.FileVisible;
                    test2.ParentId = item2.ParentId.ToString();
                    test2.BookId = item2.BookId.ToString();
                    test2.FileId = item2.FileId.ToString();
                    test2.FilesCount = item2.FilesCount;
                    result.Add(test2);
                }



                //var result = new { _folders, query };
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

        public class _bookgrp
        {
            public string title { get; set; }
            public string code { get; set; }
            public string code2 { get; set; }
            public string code3 { get; set; }
            public bool selected { get; set; }
        }
        public class dto_pif
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string ISBN { get; set; }
            public DateTime? DateRelease { get; set; }
            public DateTime? DateExposure { get; set; }
            public DateTime? DateCreate { get; set; }
            public DateTime? DatePublished { get; set; }
            public int? PublisherId { get; set; }
            public int? FolderId { get; set; }
            public string ISSNPrint { get; set; }
            public string ISSNElectronic { get; set; }
            public string DOI { get; set; }
            public string Pages { get; set; }
            public int CategoryId { get; set; }
            public int? CustomerId { get; set; }
            public string Abstract { get; set; }
            public string ImageUrl { get; set; }
            public bool? IsExposed { get; set; }
            public Nullable<System.DateTime> DateDeadline { get; set; }
            public string Duration { get; set; }
            public Nullable<int> LanguageId { get; set; }
            public string Language { get; private set; }
            public string ExternalUrl { get; set; }
            public Nullable<int> NumberOfLessens { get; set; }
            public int TypeId { get; set; }
            public int? Issue { get; set; }
            public Nullable<int> JournalId { get; set; }
            public string Journal { get; private set; }
            public string Conference { get; set; }
            public Nullable<int> ConferenceLocationId { get; set; }
            public string DateConference { get; set; }
            public string Sender { get; set; }
            public string No { get; set; }
            public string PublishedIn { get; set; }
            public string INSPECAccessionNumber { get; set; }
            public string Edition { get; set; }
            public string DateEffective { get; set; }

            public bool? IsVisited { get; set; }
            public bool? IsDownloaded { get; set; }

            public DateTime? DateVisit { get; set; }

            public DateTime? DateDownload { get; set; }

            public string Authors { get; set; }
            public string Keywords { get; set; }
            public string Publisher { get; set; }
            public string Category { get; private set; }
            public string BookKey { get; set; }
            public Nullable<int> CourseId { get; set; }

            public DateTime? DateValidUntil { get; set; }
            public DateTime? DeadLine { get; set; }

            public string SysUrlX { get; set; }
            public string FileUrlX { get; set; }

            public List<string> BookGrps { get; set; }
            //List<EmployeeView> bookRelatedEmployees = null;
            //public List<EmployeeView> BookRelatedEmployees
            //{
            //    get
            //    {
            //        if (bookRelatedEmployees == null)
            //            bookRelatedEmployees = new List<EmployeeView>();
            //        return bookRelatedEmployees;

            //    }
            //    set { bookRelatedEmployees = value; }
            //}
            List<_bookgrp> bookGroups = null;
            public List<_bookgrp> BookGroups
            {
                get
                {
                    if (bookGroups == null)
                        bookGroups = new List<_bookgrp>();
                    return bookGroups;

                }
                set { bookGroups = value; }
            }

        }
        //[HttpPost]
        //[Route("api/dc/pif/save")]
        //public async Task<IHttpActionResult> PostPIFSave(dto_pif dto)
        //{

        //    ppa_entities context = new ppa_entities();

        //    var book = await context.Books.FirstOrDefaultAsync(q => q.Id == dto.Id);
        //    if (book == null)
        //    {
        //        book = new Book()
        //        {
        //            DateCreate = DateTime.Now
        //        };
        //        context.Books.Add(book);
        //    }

        //    book.Title = dto.Title;
        //    book.Abstract = dto.Abstract;
        //    book.Sender = dto.Sender;
        //    book.CategoryId = dto.CategoryId;
        //    book.No = dto.No;
        //    book.DateRelease = dto.DateRelease;
        //    book.DateDeadline = dto.DateDeadline;
        //    book.DateEffective = dto.DateEffective;
        //    book.DatePublished = dto.DatePublished;
        //    book.DateValidUntil = dto.DateValidUntil;
        //    book.FileUrlX = dto.FileUrlX;
        //    book.SysUrlX = dto.SysUrlX;

        //    if (dto.Id != -1)
        //    {
        //        var existing_grps = await context.BookRelatedGroups.Where(q => q.BookId == dto.Id).ToListAsync();
        //        if (existing_grps != null && existing_grps.Count > 0)
        //            context.BookRelatedGroups.RemoveRange(existing_grps);
        //    }
        //    var _grps = new List<Models.JobGroup>();
        //    var qry = from q in context.JobGroups
        //              select q;
        //    if (dto.CategoryId == 10007)
        //    { qry = qry.Where(q => q.FullCode.StartsWith("00101")); _grps = qry.ToList(); }
        //    //10008
        //    else if (dto.CategoryId == 10008)
        //    { qry = qry.Where(q => q.FullCode.StartsWith("00102")); _grps = qry.ToList(); }
        //    //dif
        //    //95
        //    else if (dto.CategoryId == 95)
        //    { qry = qry.Where(q => q.FullCode.StartsWith("00103")); _grps = qry.ToList(); }

        //    else
        //    {

        //        foreach (var x in dto.BookGroups)
        //        {
        //            var qry2 = from q in context.JobGroups
        //                       select q;
        //            if (x.code == "-1")
        //            {
        //                //var grps = this.context.JobGroups.Where(q => q.FullCode.StartsWith(x)).ToList();
        //                var grps =  context.JobGroups.ToList();
        //                _grps = _grps.Concat(grps).ToList();
        //            }
        //            else
        //            {
        //                qry = qry.Where(q => q.FullCode.StartsWith(x.code));
        //                var grps = qry.ToList();
        //                _grps = _grps.Concat(grps).ToList();

        //                if (!string.IsNullOrEmpty(x.code2))
        //                {
        //                    _grps = _grps.Concat(this.context.JobGroups.Where(w => w.FullCode == x.code2)).ToList();
        //                }

        //            }
        //        }
        //    }

        //    foreach (var x in _grps)
        //        this.context.BookRelatedGroups.Add(new Models.BookRelatedGroup()
        //        {
        //            Book = entity,
        //            GroupId = x.Id,
        //            TypeId = null,// x.TypeId,

        //        });

        //    await context.SaveChangesAsync();

        //    dto.Id = entity.Id;
        //    return Ok(dto);
        //}


        //////end of cpntroller
    }












}
