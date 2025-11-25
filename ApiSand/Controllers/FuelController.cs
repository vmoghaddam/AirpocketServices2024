using ApiSand.Models;
using ApiSand.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Novacode;
using System.IO;
using System.Text.RegularExpressions;
using Tesseract;

namespace ApiSand.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FuelController : ApiController
    {
        //[Route("api/fuel/report")]
        //public async Task<IHttpActionResult> GetFuelReport(DateTime dfrom, DateTime dto)
        //{
        //    try
        //    {
        //        dfrom = dfrom.Date;
        //        dto = dto.Date.AddDays(1);
        //        ppa_entities context = new ppa_entities();
        //        var query = from x in context.AppFuels
        //                    where x.STDDay >= dfrom && x.STDDay <= dto
        //                    select x;

        //        var result = await query.OrderBy(q => q.STD).ToListAsync();


        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "   " + ex.InnerException.Message;
        //        return Ok(msg);
        //    }

        //}
        static string ExtractField(string text, string pattern)
        {
            Match match = Regex.Match(text, pattern);
            return match.Success ? match.Groups[1].Value.Trim() : "Not Found";
        }
        public static DateTime? ConvertPersianDateToGregorian(string input)
        {
            // استفاده از Regex برای پیدا کردن تاریخ شمسی در فرمت yyyy/MM/dd
            var match = Regex.Match(input, @"\d{4}/\d{2}/\d{2}");
            if (match.Success)
            {
                string persianDate = match.Value;
                string[] parts = persianDate.Split('/');
                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);

                // استفاده از PersianCalendar برای تبدیل
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            }

            // اگر تاریخ پیدا نشد، مقدار null برمی‌گردد
            return null;
        }
        public static DateTime? ConvertPersianDateToGregorian2(string input)
        {
            // استفاده از Regex برای پیدا کردن تاریخ شمسی در فرمت yyyy/MM/dd
            var match = Regex.Match(input, @"\d{4}-\d{2}-\d{2}");
            if (match.Success)
            {
                string persianDate = match.Value;
                string[] parts = persianDate.Split('-');
                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);

                // استفاده از PersianCalendar برای تبدیل
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            }

            // اگر تاریخ پیدا نشد، مقدار null برمی‌گردد
            return null;
        }
        public class Student2
        {
            public int No { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }

            public string key { get; set; }
        }
        public class Session
        {
            public string Date { get; set; }
            public string Start { get; set; }
            public string End { get; set; }

            public DateTime? DateStart { get; set; }
            public DateTime? DateEnd { get; set; }
            public string key { get; set; }

            public override string ToString()
            {
                return $"{{ date = \"{Date}\", start = \"{Start}\", end = \"{End}\" }}";
            }
        }
        public class fly_course
        {
            public string title { get; set; }
            public DateTime? date_start { get; set; }
            public DateTime? date_end { get; set; }

            public int? duration { get; set; }
            public int? days { get; set; }
            public string instructors { get; set; }
            public List<Session> sessions { get; set; }
            public List<Student2> students { get; set; }
            public string key { get; set; }
        }

        [Route("api/folders")]
        public async Task<DataResponse> get_folder_names()
        {
            string root = @"C:\inetpub\vhosts\aerotango.app\httpdocs\AVA\Documents\Crew";
            ppa_entities context = new ppa_entities();


            // آدرس کامل فولدرهای مستقیم داخل root
            string[] dirPaths = Directory.GetDirectories(root);

            // فقط اسم فولدر (بدون مسیر کامل)
            var dirNames = dirPaths.Select(Path.GetFileName);

            foreach (var name in dirNames)
            {
                context.ava_folder.Add(new ava_folder()
                {
                    folder_name = name,

                });
            }
            context.SaveChanges();
            return new DataResponse()
            {
                Data = dirNames,
                IsSuccess = true
            };
        }


        [Route("api/folders/rename")]
        public async Task<DataResponse> get_folder_renames()
        {
            string root = @"C:\inetpub\vhosts\aerotango.app\httpdocs\AVA\Documents\Crew";
            ppa_entities context = new ppa_entities();
            var db_names = context.ava_folder.ToList();

            foreach (var dirPath in Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly))
            {
                string oldName = Path.GetFileName(dirPath);
                var rec = db_names.Where(q => q.folder_name == oldName).FirstOrDefault();
                if (rec != null)
                {
                    string newName = rec.nid; //+ "_" + oldName;

                    // --- قاعده‌ی تغییر نام (هرطور می‌خوای تنظیم کن) ---
                    // newName = newName.ToLowerInvariant();                // مثال: همه حروف کوچک
                    // newName = newName.Replace(" ", "_");                 // مثال: فاصله -> _
                    // newName = Regex.Replace(newName, @"\s+", "-");       // مثال: فاصله‌ها -> -
                    // newName = "PREFIX_" + newName;                       // مثال: افزودن پیشوند
                    // newName = newName.Trim();                            // مثال: حذف فاصله‌های ابتدا/انتها
                    // نمونه ترکیبی پیشنهادی:
                    //newName = Regex.Replace(newName.Trim(), @"\s+", "-").ToLowerInvariant();
                    // ----------------------------------------------------

                    string targetPath = Path.Combine(root, newName);
                    Directory.Move(dirPath, targetPath);
                    //  Console.WriteLine($"Renamed: \"{oldName}\" -> \"{newName}\"");
                }

            }


            return new DataResponse()
            {
                Data = db_names,
                IsSuccess = true
            };
        }

        [Route("api/folders/rename/x")]
        public async Task<DataResponse> get_folder_renamesx()
        {
            string root = @"C:\Users\vahid\Desktop\ava\crew documents\source";


            foreach (var dirPath in Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly))
            {
                string oldName = Path.GetFileName(dirPath);


                {
                    string newName = oldName.Split('_')[1];

                    // --- قاعده‌ی تغییر نام (هرطور می‌خوای تنظیم کن) ---
                    // newName = newName.ToLowerInvariant();                // مثال: همه حروف کوچک
                    // newName = newName.Replace(" ", "_");                 // مثال: فاصله -> _
                    // newName = Regex.Replace(newName, @"\s+", "-");       // مثال: فاصله‌ها -> -
                    // newName = "PREFIX_" + newName;                       // مثال: افزودن پیشوند
                    // newName = newName.Trim();                            // مثال: حذف فاصله‌های ابتدا/انتها
                    // نمونه ترکیبی پیشنهادی:
                    //newName = Regex.Replace(newName.Trim(), @"\s+", "-").ToLowerInvariant();
                    // ----------------------------------------------------

                    string targetPath = Path.Combine(root, newName);
                    Directory.Move(dirPath, targetPath);
                    //  Console.WriteLine($"Renamed: \"{oldName}\" -> \"{newName}\"");
                }

            }


            return new DataResponse()
            {
                Data = true,
                IsSuccess = true
            };
        }


        [Route("api/folders/rename/sub")]
        public async Task<DataResponse> get_folder_renames_sub()
        {
            string root = @"C:\Users\vahid\Desktop\ava\crew documents";
            // گرفتن همه‌ی ساب‌فولدرها
            string[] subDirs = Directory.GetDirectories(root);

            foreach (var dir in subDirs)
            {
                string dirName = Path.GetFileName(dir); // مثل "0019061961_نگار هاشمی"
                string[] parts = dirName.Split('_');

                if (parts.Length > 0)
                {
                    string newName = parts[0]; // فقط قسمت اول
                    string newPath = Path.Combine(root, newName);

                    // اگر پوشه جدید از قبل وجود ندارد، تغییر نام بده
                    if (!Directory.Exists(newPath))
                    {
                        Directory.Move(dir, newPath);
                        Console.WriteLine($"Renamed: {dirName} -> {newName}");
                    }
                    else
                    {
                        Console.WriteLine($"Skipped (already exists): {newName}");
                    }
                }
            }
            //ppa_entities context = new ppa_entities();
            //var subs = context.ava_sub.ToList();
            //foreach (var sub in subs)
            //{
            //    try
            //    {
            //        string path = sub.fullpath;
            //        string oldName = sub.title;
            //        string newName = sub.title2;
            //        string parentDir = Path.GetDirectoryName(path);

            //        // مسیر جدید با نام جدید
            //        string newPath = Path.Combine(parentDir, newName);

            //        // تغییر نام دایرکتوری
            //        Directory.Move(path, newPath);
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //}




            return new DataResponse()
            {
                Data = true,
                IsSuccess = true
            };
        }



        [Route("api/folders/a")]
        public async Task<DataResponse> get_folder_a()
        {
            string root = @"C:\Users\vahid\Desktop\ava\crew documents"; // مسیر root خودت
            ppa_entities context = new ppa_entities();
            List<string> names = new List<string>();
            // سطح اول: پوشه‌های مستقیم داخل root
            //foreach (var level1Path in Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly))
            //{
            //    string level1Name = Path.GetFileName(level1Path);


            //    // سطح دوم: فولدرهای داخل هر ساب‌فولدر سطح اول
            //    foreach (var level2Path in Directory.EnumerateDirectories(level1Path, "*", SearchOption.TopDirectoryOnly))
            //    {
            //        string level2Name = Path.GetFileName(level2Path);
            //        names.Add(level2Name);
            //        context.ava_sub.Add(new ava_sub() {  title=level2Name});


            //    }


            //}
            foreach (var level1Path in Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly))
            {
                //Console.WriteLine($"[L1] {level1Path}"); // مسیر کامل فولدر سطح اول
                string level1Name = Path.GetFileName(level1Path);
                // ساب‌فولدرهای سطح دوم (مسیر کامل)
                foreach (var level2Path in Directory.EnumerateDirectories(level1Path, "*", SearchOption.TopDirectoryOnly))
                {
                    // Console.WriteLine($"  [L2] {level2Path}");
                    string level2Name = Path.GetFileName(level2Path);
                    var ava_sub = new ava_sub()
                    {
                        title = level2Name,
                        fullpath = level2Path,
                        nid = level1Name.Split('_')[0],
                        remark = level1Name.Split('_')[1],
                    };
                    context.ava_sub.Add(ava_sub);
                    // فایل‌های داخل هر ساب‌فولدر سطح دوم (مسیر کامل)
                    foreach (var filePath in Directory.EnumerateFiles(level2Path, "*", SearchOption.TopDirectoryOnly))
                    {
                        //Console.WriteLine($"    [FILE] {filePath}");
                        string fileName = Path.GetFileName(filePath);
                        ava_sub.ava_sub_file.Add(new ava_sub_file()
                        {
                            fullpath = filePath,
                            title = fileName,

                        });

                    }
                }

                //Console.WriteLine();
            }

            context.SaveChanges();
            return new DataResponse()
            {
                Data = names,
                IsSuccess = true
            };
        }

        [Route("api/doc")]
        public async Task<DataResponse> get_doc()
        {
            string mainPath = @"C:\Users\vahid\Desktop\ava\camo";  // مسیر فولدر مورد نظر رو وارد کن

            // گرفتن فقط فولدرهای سطح اول داخل فولدر
            string[] subDirectories = Directory.GetDirectories(mainPath, "*", SearchOption.AllDirectories); //Directory.GetDirectories(mainPath);

            // تبدیل آرایه به لیست
            List<string> subFolderList = new List<string>(subDirectories);
            List<fly_course> courses = new List<fly_course>();
            List<string> errors = new List<string>();
            List<string> errors2 = new List<string>();
            foreach (var _folderPath in subFolderList)
            {
                string folderPath = _folderPath; //@"C:\Users\vahid\Desktop\ava\Hozor Ghiab\initial_cabin_1\test";
                string[] docxFiles = Directory.GetFiles(folderPath, "*.docx");

                Console.WriteLine("فایل‌های موجود در پوشه:");

                foreach (string file in docxFiles)
                {
                    if (!file.Contains("Type 310 5"))
                    {
                        // continue;
                    }
                    // Console.WriteLine(Path.GetFileName(file)); // فقط نام فایل بدون مسیر
                    //string filePath = @"C:\Users\vahid\Desktop\ava\flykish\" + "فرم  حضورغیاب fly kish Annoucment" + ".docx";
                    string filePath = (file);
                    List<Student2> students = new List<Student2>();
                    // List<Session> sessions = new List<Session>();
                    List<string> days = new List<string>();
                    List<string> sessions = new List<string>();
                    List<string> day_sessions = new List<string>();
                    List<Session> c_sessions = new List<Session>();
                    fly_course course = new fly_course() { key = filePath };
                    try
                    {
                        using (DocX document = DocX.Load(filePath))
                        {
                            string text = document.Text;

                            string courseTitle = ExtractField(text, @"Course Title:\s*(.+?)\s*Department");
                            string startingDate = ExtractField(text, @"Starting Date:\s*([^\s]+)");
                            string endingDate = ExtractField(text, @"Ending Date:\s*([^\s]+)");
                            string duration = ExtractField(text, @"Duration:\s*(.+?)\s*Hrs");  //……20……/……3……
                            string instructor = ExtractField(text, @"Instructor's Name:\s*(.+)");

                            course.date_start = ConvertPersianDateToGregorian(startingDate);
                            course.date_end = ConvertPersianDateToGregorian(endingDate);
                            var d_prts = duration.Replace(".", "").Replace(" ", "").Split('/');
                            course.duration = Convert.ToInt32(duration.Replace("…", "").Replace(".", "").Replace(" ", "").Split('/')[0]);
                            course.days = Convert.ToInt32(duration.Replace("…", "").Replace(".", "").Split('/')[1]);
                            course.instructors = String.Join(", ", instructor.Split('&').Select(q => q.ToUpper()).ToList());

                            Console.WriteLine("Course Title: " + courseTitle);
                            Console.WriteLine("Starting Date: " + startingDate);
                            Console.WriteLine("Ending Date: " + endingDate);
                            Console.WriteLine("Duration: " + duration);
                            Console.WriteLine("Instructor's Name: " + instructor);
                            foreach (var table in document.Tables)
                            {
                                // بررسی اینکه جدول حداقل 3 ستون دارد (No, Name (per), Name (eng))
                                int _r = 0;
                                foreach (var row in table.Rows)
                                {
                                    if (row.Cells.Count >= 3 &&
                                        int.TryParse(row.Cells[0].Paragraphs[0].Text.Trim(), out int no))
                                    {
                                        string namePer = row.Cells[1].Paragraphs[0].Text.Trim();
                                        string nameEng = row.Cells[2].Paragraphs[0].Text.Trim();

                                        students.Add(new Student2
                                        {
                                            No = no,
                                            first_name = namePer,
                                            last_name = nameEng,
                                            key = filePath
                                        });

                                        var hrow = table.Rows[_r - 2];
                                        var srow = table.Rows[_r - 1];
                                        foreach (var c in hrow.Cells)
                                        {
                                            string str = c.Paragraphs[0].Text;
                                            if (str != "No" && str != "Name" && !string.IsNullOrEmpty(str))
                                                days.Add(str);
                                        }
                                        if (day_sessions.Count == 0)
                                        {
                                            foreach (var c in srow.Cells)
                                            {
                                                string str = c.Paragraphs[0].Text;
                                                if (!string.IsNullOrEmpty(str) && str != "Signature")
                                                {
                                                    var strs = str.Split('-');
                                                    if (!strs[0].Contains(":"))
                                                        strs[0] += ":00";
                                                    if (strs[0].Length < 5)
                                                        strs[0] = "0" + strs[0];
                                                    if (strs.Length > 1)
                                                    {
                                                        if (!strs[1].Contains(":"))
                                                            strs[1] += ":00";
                                                        if (strs[1].Length < 5)
                                                            strs[1] = "0" + strs[1];
                                                        sessions.Add(strs[0] + "-" + strs[1]);
                                                    }
                                                    else
                                                    {
                                                        sessions.Add(strs[0] + "-" + strs[0]);
                                                    }



                                                }
                                            }
                                            // List<List<string>> result_sessions = SplitListBySig(sessions);

                                            int _di = 0;
                                            int _si = 0;
                                            Int64 _cs = -1;
                                            sessions = sessions.Select(q => q.Replace(" ", "").Trim()).ToList();
                                            foreach (var session in sessions)
                                            {
                                                var _i6 = Convert.ToInt64(session.Replace("-", "").Replace(":", "").Replace(" ", ""));
                                                if (_cs >= _i6)
                                                    _di++;
                                                var _day = days[_di];
                                                day_sessions.Add(_day + " " + session);
                                                var c_session = (new Session()
                                                {
                                                    DateStart = ConvertPersianDateToGregorian(_day),
                                                    DateEnd = ConvertPersianDateToGregorian(_day),
                                                });

                                                if (c_session.DateStart == null)
                                                {
                                                    c_session.DateStart = ConvertPersianDateToGregorian2(_day);
                                                    c_session.DateEnd = ConvertPersianDateToGregorian2(_day);
                                                }
                                                if (c_session.DateStart != null)
                                                {
                                                    var _ss = session.Split('-')[0];
                                                    var _se = session.Split('-')[1];
                                                    c_session.DateStart = ((DateTime)c_session.DateStart).AddHours(Convert.ToInt32(_ss.Split(':')[0]))
                                                        .AddMinutes(Convert.ToInt32(_ss.Split(':')[1]));

                                                    c_session.DateEnd = ((DateTime)c_session.DateEnd).AddHours(Convert.ToInt32(_se.Split(':')[0]))
                                                        .AddMinutes(Convert.ToInt32(_se.Split(':')[1]));
                                                    c_session.key = filePath;
                                                    c_sessions.Add(c_session);

                                                }
                                                _cs = Convert.ToInt64(session.Replace("-", "").Replace(":", ""));

                                            }
                                        }


                                    }
                                    _r++;
                                }
                            }

                            course.sessions = c_sessions;
                            course.students = students;
                            courses.Add(course);

                            //foreach (var table in document.Tables)
                            //{
                            //    // فرض: ردیف اول شامل تاریخ‌ها است، ردیف دوم شامل ساعت‌ها
                            //    if (table.RowCount >= 3)
                            //    {
                            //        var dateRow = table.Rows[0]; // یا 1 اگر سطر اول عنوان است
                            //        var timeRow = table.Rows[1];

                            //        string currentDate = null;

                            //        for (int i = 0; i < dateRow.Cells.Count; i++)
                            //        {
                            //            var dateText = dateRow.Cells[i].Paragraphs[0].Text.Trim();
                            //            if (Regex.IsMatch(dateText, @"\d{4}-\d{2}-\d{2}")) // تشخیص تاریخ
                            //            {
                            //                currentDate = dateText;
                            //            }

                            //            var timeText = timeRow.Cells[i].Paragraphs[0].Text.Trim();
                            //            var match = Regex.Match(timeText, @"(\d{2}:\d{2})-(\d{2}:\d{2})");
                            //            if (currentDate != null && match.Success)
                            //            {
                            //                sessions.Add(new Session
                            //                {
                            //                    Date = currentDate,
                            //                    Start = match.Groups[1].Value,
                            //                    End = match.Groups[2].Value
                            //                });
                            //            }
                            //        }

                            //        break; // اولین جدول کافی است
                            //    }
                            //}




                        }
                    }
                    catch (Exception ex)
                    {
                        var msg = filePath + "    " + ex.Message;
                        if (ex.InnerException != null)
                            msg += "    " + ex.InnerException.Message;
                        errors.Add(msg);
                        errors.Add("           ");
                        errors2.Add(filePath);
                    }

                }
            }

            ppa_entities context = new ppa_entities();
            foreach (var c in courses)
            {
                context.fly_course.Add(new Models.fly_course()
                {
                    date_end = c.date_end,
                    date_start = c.date_start,
                    days = c.days,
                    duration = c.duration,
                    instructors = c.instructors,
                    key = c.key,
                    title = c.title,
                });
                foreach (var s in c.sessions)
                {
                    context.fly_course_session.Add(new fly_course_session()
                    {
                        date_end = s.DateEnd,
                        date_start = s.DateStart,
                        key = s.key,
                    });
                }
                foreach (var s in c.students)
                {
                    context.fly_course_student.Add(new fly_course_student()
                    {
                        key = s.key,
                        first_name = s.first_name,
                        last_name = s.last_name,
                        row_no = s.No.ToString(),
                    });
                }
            }
            context.SaveChanges();
            File.WriteAllLines(@"C:\Users\vahid\Desktop\ava\camo\" + "errors.txt", errors);
            File.WriteAllLines(@"C:\Users\vahid\Desktop\ava\camo\" + "errors2.txt", errors2);
            return new DataResponse()
            {
                Data = new
                {
                    courses,
                    errors
                },
                IsSuccess = true
            };
        }



        [Route("api/doc2")]
        public async Task<DataResponse> get_doc2()
        {
            DocxToJson.Program cls = new DocxToJson.Program();
            cls.execute();
            return new DataResponse()
            {
                Data = true,
                IsSuccess = true
            };
        }


        [Route("api/doc2/sessions")]
        public async Task<DataResponse> get_doc2_sessions()
        {
            ppa_entities context = new ppa_entities();
            var ava_courses = context.ava_course.Where(q => q.remark.StartsWith("grh")).ToList();
            var ava_crs_ids = ava_courses.Select(q => (Nullable<int>)q.id).ToList();
            var ava_sessions = context.ava_session.Where(q => ava_crs_ids.Contains(q.course_id)).ToList();
            var courses = context.Courses.Where(q => ava_crs_ids.Contains(q.ext_id)).ToList();
            foreach (var crs in courses)
            {
                var i_sessions = ava_sessions.Where(q => q.course_id == crs.ext_id).ToList();
                var start_date = crs.DateStart.Date;
                var end_date = ((DateTime)crs.DateEnd).Date;
                while (start_date <= end_date)
                {
                    foreach (var s in i_sessions)
                    {
                        var prts = s.remark.Replace(" ", "").Split('-');
                        var s_h = Convert.ToInt32(prts[0].Split(':')[0]);
                        var s_m = Convert.ToInt32(prts[0].Split(':')[1]);
                        var e_h = Convert.ToInt32(prts[1].Split(':')[0]);
                        var e_m = Convert.ToInt32(prts[1].Split(':')[1]);
                        var s_start = start_date.AddHours(s_h).AddMinutes(s_m);
                        var s_end = start_date.AddHours(e_h).AddMinutes(e_m);
                        //2023-11-28-08-30-16-30
                        var s_key = start_date.ToString("yyyy-MM-dd-") + s_start.ToString("HH-mm-") + s_end.ToString("HH-mm");
                        var db_session = new CourseSession()
                        {
                            CourseId = crs.Id,
                            DateEnd = s_end,
                            DateStart = s_start,
                            DateEndUtc = s_end.AddMinutes(-210),
                            DateStartUtc = s_start.AddMinutes(-210),
                            Key = s_key,
                            Done = false,
                            Remark = crs.Remark /*+ " " + "import_20251029_" + crs.Id + "_" + crs.ext_id*/,
                        };
                        context.CourseSessions.Add(db_session);

                    }
                    start_date = start_date.AddDays(1);
                }
            }
            context.SaveChanges();
            return new DataResponse()
            {
                Data = true,
                IsSuccess = true
            };
        }


        [Route("api/ocr")]
        public async Task<DataResponse> get_ocr()
        {
            string tessDataPath = @"C:\tessdata";
            string imagePath = @"C:\Users\vahid\Desktop\ava\ftp_crew_documents\Documents\TRG\1-2 2027-09-23\2027-10-23 TRG 1-2 AHMADREZA BAGHERI.jpg";
            var reader = new TesseractOCRReader(tessDataPath);
            string englishText = reader.ReadEnglishText(imagePath);
            return new DataResponse()
            {
                Data = true,
                IsSuccess = true
            };
        }

        [Route("api/create/profile/folders")]
        public async Task<DataResponse> get_create_folders()
        {
            ppa_entities context = new ppa_entities();

            var nids = context.ViewProfiles.Where(q => q.GroupId == 1047 || q.GroupId == 1052).Select(q => q.NID).ToList();
            string basePath = @"C:\Users\vahid\Desktop\ava\profile_folders";
            string[] subfolders =
            {
            "LINE CHECK RECORDS",
            "LICENSES",
            "GENERAL DOCUMENTS",
            "CERTIFICATES",
            "LOGBOOK RECORDS",
            "MEDICAL RECORDS",
            "OFFICIAL RECORDS",
            "SIMULATOR RECORDS"
            };
            foreach (var nid in nids)
            {
                // فولدر اصلی برای هر NID
                string nidFolderPath = Path.Combine(basePath, nid);
                Directory.CreateDirectory(nidFolderPath);

                // ایجاد ساب‌فولدرها
                foreach (var sub in subfolders)
                {
                    string subFolderPath = Path.Combine(nidFolderPath, sub);
                    Directory.CreateDirectory(subFolderPath);
                }
            }

            return new DataResponse()
            {
                Data = true,
                IsSuccess = true
            };
        }


    }


    public class DataResponse
    {
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
    }

    public class TesseractOCRReader
    {
        private string tessDataPath;

        public TesseractOCRReader(string tessDataPath)
        {
            this.tessDataPath = tessDataPath;
        }

        /// <summary>
        /// خواندن متن از تصویر - فارسی
        /// </summary>
        public string ReadPersianText(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(tessDataPath, "fas", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("خطا در خواندن تصویر: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// خواندن متن از تصویر - انگلیسی
        /// </summary>
        public string ReadEnglishText(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.TesseractAndLstm))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("خطا در خواندن تصویر: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// خواندن متن از تصویر - چند زبانه (فارسی و انگلیسی)
        /// </summary>
        public string ReadMixedText(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(tessDataPath, "fas+eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("خطا در خواندن تصویر: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// خواندن متن با جزئیات (شامل موقعیت و اطمینان)
        /// </summary>
        //public void ReadTextWithDetails(string imagePath)
        //{
        //    try
        //    {
        //        using (var engine = new TesseractEngine(tessDataPath, "fas+eng", EngineMode.Default))
        //        {
        //            using (var img = Pix.LoadFromFile(imagePath))
        //            {
        //                using (var page = engine.Process(img))
        //                {
        //                    Console.WriteLine("متن کامل:");
        //                    Console.WriteLine(page.GetText());
        //                    Console.WriteLine();
        //                    Console.WriteLine("میزان اطمینان: " + page.GetMeanConfidence());
        //                    Console.WriteLine();

        //                    // خواندن کلمه به کلمه
        //                    using (var iter = page.GetIterator())
        //                    {
        //                        iter.Begin();
        //                        do
        //                        {
        //                            var word = iter.GetText(PageIteratorLevel.Word);
        //                            var confidence = iter.GetConfidence(PageIteratorLevel.Word);
        //                            var bounds = iter.GetBoundingBox(PageIteratorLevel.Word);

        //                            if (bounds != null)
        //                            {
        //                                Console.WriteLine("کلمه: {0}, اطمینان: {1:F2}%, مختصات: ({2},{3})-({4},{5})",
        //                                    word, confidence,
        //                                    bounds.Value.X1, bounds.Value.Y1,
        //                                    bounds.Value.X2, bounds.Value.Y2);
        //                            }
        //                        } while (iter.Next(PageIteratorLevel.Word));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("خطا: " + ex.Message);
        //    }
        //}
    }





}
