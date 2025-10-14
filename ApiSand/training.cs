// Docx → JSON extractor
// Target: C# 7.3, .NET Framework 4.6.1+ (or .NET Core 2.0+)
// NuGet packages (free):
//   - DocumentFormat.OpenXml  (Open XML SDK)
//   - Newtonsoft.Json
// Usage:
//   DocxToJson.exe "C:\InputFolder" "C:\OutputFolder"
// or DocxToJson.exe "C:\InputFolder"   (outputs JSON next to each DOCX)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ApiSand.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;

namespace DocxToJson
{
    // --- Models ---
    public class CourseRecord
    {
        public string CourseTitle { get; set; }
        public string Department { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Duration Duration { get; set; }
        public string Instructor { get; set; }
        public List<string> Sessions { get; set; }
        public List<Participant> Participants { get; set; }
        [JsonProperty("filename")] public string FileName { get; set; }
        [JsonProperty("filepath")] public string FilePath { get; set; }
        [JsonProperty("participants_count")] public int ParticipantsCount { get; set; }
        [JsonProperty("sessions_count")] public int SessionsCount { get; set; }
        public Dictionary<string, string> Raw { get; set; } // Anything we couldn't confidently map
    }

    public class Duration
    {
        public int? Hours { get; set; }
        public int? Days { get; set; }
    }

    public class Participant
    {
        public int? No { get; set; }
        public string NameEn { get; set; }
        public string NameFa { get; set; }
        public string Attendance { get; set; }



    }


    public class Program
    {
        public int execute()
        {
            try
            {
                ppa_entities context = new ppa_entities();

                var input = @"C:\Users\vahid\Desktop\ava\camo";
                var outputFolder = @"C:\Users\vahid\Desktop\ava\camo\___json";

                var files = new List<string>();
                //if (Directory.Exists(input))
                //{

                //    files.AddRange(Directory.GetFiles(input, "*.docx", SearchOption.AllDirectories));
                //}
                if (Directory.Exists(input))
                {
                    var q = Directory.EnumerateFiles(input, "*.docx", SearchOption.AllDirectories)
                                     .Where(p => !Path.GetFileName(p).StartsWith("~$", StringComparison.OrdinalIgnoreCase));
                    files.AddRange(q);
                }
                else if (File.Exists(input) && input.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                {
                    files.Add(input);
                }
                else
                {
                    Console.WriteLine("Input must be a .docx file or a folder containing .docx files.");
                    return 2;
                }

                if (files.Count == 0)
                {
                    Console.WriteLine("No .docx files found.");
                    return 0;
                }

                int ok = 0, fail = 0;
                foreach (var path in files)
                {
                    try
                    {
                        var rec = ParseDocx(path);
                        var json = JsonConvert.SerializeObject(rec, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                        var outDir = outputFolder ?? Path.GetDirectoryName(path);
                        Directory.CreateDirectory(outDir);
                        var outPath = Path.Combine(outDir, Path.GetFileNameWithoutExtension(path) + ".json");
                        File.WriteAllText(outPath, json, new UTF8Encoding(false));
                        Console.WriteLine($"OK  → {Path.GetFileName(path)}  → {Path.GetFileName(outPath)}");
                        ok++;
                    }
                    catch (Exception ex)
                    {
                        ava_error err = new ava_error()
                        {
                             file_path=path,
                              description=ex.Message,
                              file_name= Path.GetFileName(path),
                        };
                        context.ava_error.Add(err);
                        Console.WriteLine($"ERR → {Path.GetFileName(path)} :: {ex.Message}");
                        fail++;
                    }
                }
                context.SaveChanges();
                Console.WriteLine($"Done. Success: {ok}, Failed: {fail}");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 99;
            }
        }
        private static List<int> FindAllColumns(List<List<string>> grid, int headerRowsToScan, string[] keywords)
        {
            var cols = new List<int>();
            for (int c = 0; c < MaxColumns(grid, headerRowsToScan); c++)
            {
                for (int r = 0; r < headerRowsToScan && r < grid.Count; r++)
                {
                    var cell = GetCell(grid, r, c);
                    if (string.IsNullOrWhiteSpace(cell)) continue;
                    foreach (var k in keywords)
                        if (cell.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0)
                        { cols.Add(c); break; }
                }
            }
            return cols.Distinct().OrderBy(x => x).ToList();
        }

        public static CourseRecord ParseDocx(string filePath)
        {
            ppa_entities context = new ppa_entities();
            using (var doc = WordprocessingDocument.Open(filePath, false))
            {
                var body = doc.MainDocumentPart.Document.Body;

                // Text anywhere (headers/meta) — many docs keep text inside tables
                var paragraphs = body.Descendants<Paragraph>().ToList(); // include paragraphs inside tables too
                var lines = new List<string>();
                foreach (var p in paragraphs)
                {
                    var t = GetParagraphText(p);
                    if (!string.IsNullOrWhiteSpace(t)) lines.Add(NormalizeSpaces(t));
                }
                var all = string.Join("\n", lines);

                // Extract top fields with resilient regexes
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                TryAdd(map, "CourseTitle", MatchAfter(all, @"Course\s*Title\s*:\s*(.*?)(?=\s+(?:Department|Starting\s*Date|Ending\s*Date|Date|Instructor.?s?\s*Name|Duration)\s*:|$)"));
                TryAdd(map, "Department", MatchAfter(all, @"Department\s*:\s*(.+?)(?:\s{2,}|$)"));
                TryAdd(map, "StartingDate", MatchAfter(all, @"Starting\s*Date\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
                TryAdd(map, "EndingDate", MatchAfter(all, @"Ending\s*Date\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
                TryAdd(map, "Date", MatchAfter(all, @"\bDate\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
                TryAdd(map, "Duration", MatchAfter(all, @"Duration\s*:\s*([^\n]+)"));
                TryAdd(map, "Instructor", MatchAfter(all, @"Instructor.?s?\s*Name\s*:\s*(.+)"));

                // Build record
                var rec = new CourseRecord();
                rec.CourseTitle = Get(map, "CourseTitle");
                rec.Department = Get(map, "Department");
                rec.Instructor = Get(map, "Instructor");
                rec.Duration = ParseDuration(Get(map, "Duration"));

                // File info
                rec.FileName = Path.GetFileName(filePath);
                rec.FilePath = Path.GetFullPath(filePath);

                // Dates (support Persian/Jalali too)
                var start = Get(map, "StartingDate");
                var end = Get(map, "EndingDate");
                var singleDate = Get(map, "Date");
                if (!string.IsNullOrEmpty(start)) rec.StartDate = ParseFlexibleDate(start);
                if (!string.IsNullOrEmpty(end)) rec.EndDate = ParseFlexibleDate(end);
                if (rec.StartDate == null && rec.EndDate == null && !string.IsNullOrEmpty(singleDate))
                {
                    var d = ParseFlexibleDate(singleDate);
                    rec.StartDate = d; rec.EndDate = d;
                }

                // Attendance table(s)
                var sessions = new List<string>();
                var participants = new List<Participant>();

                foreach (var table in body.Descendants<Table>())
                {
                    var grid = ReadTable(table); // rows of cell texts
                    if (grid.Count == 0) continue;

                    // Collect possible session headers (time ranges like 10:00-12:00 etc) from top few rows
                    var timeRegex = new Regex(@"\b\d{1,2}:\d{2}\s*[-–]\s*\d{1,2}:\d{2}\b");
                    var headerRowsToScan = Math.Min(10, grid.Count); // scan a few more rows for headers
                    var candidateSessionCols = new List<int>();
                    var candidateSessionLabels = new List<string>();

                    for (int r = 0; r < headerRowsToScan; r++)
                    {
                        var row = grid[r];
                        for (int c = 0; c < row.Count; c++)
                        {
                            var cell = row[c];
                            if (string.IsNullOrWhiteSpace(cell)) continue;
                            if (timeRegex.IsMatch(cell) && !candidateSessionCols.Contains(c))
                            {
                                candidateSessionCols.Add(c);
                                candidateSessionLabels.Add(FirstMatch(cell, timeRegex));
                            }
                        }
                    }

                    if (candidateSessionCols.Count == 0)
                    {
                        // Not an attendance table
                        continue;
                    }

                    // Try to find columns for No / Name EN / Name FA from header-ish area
                    var noCol = FindColumnIndex(grid, headerRowsToScan, new[] { "No", "NO", "#" });
                    var nameEnCol = FindColumnIndex(grid, headerRowsToScan, new[] { "Name", "First Name", "Full Name", "نام انگلیسی" });
                    // For Persian, look for a cell that literally contains Persian letters near name header
                    var nameFaCol = FindPersianNameColumn(grid, headerRowsToScan);
                    // If headers have two "Name" columns, assume the latter is Persian
                    if (nameFaCol < 0)
                    {
                        var headerNameCols = FindAllColumns(grid, headerRowsToScan, new[] { "Name" });
                        if (headerNameCols.Count >= 2)
                        {
                            nameEnCol = headerNameCols[0];
                            nameFaCol = headerNameCols[headerNameCols.Count - 1];
                        }
                        else if (nameEnCol >= 0)
                        {
                            // Fallback: probe first 10 data rows for a column with Persian text
                            int probeStart = DetectDataStartRow(grid, headerRowsToScan);
                            nameFaCol = FindPersianRichColumn(grid, probeStart, maxRows: 10);
                        }
                    }

                    // Build sessions list (ordered by column index)
                    var ordered = candidateSessionCols
                        .Select((col, idx) => new { Col = col, Label = candidateSessionLabels[idx] })
                        .OrderBy(x => x.Col)
                        .ToList();
                    sessions = ordered.Select(x => x.Label).Distinct().ToList();

                    // Decide where data rows actually start: first row whose first non-empty cell looks like a number (row no.)
                    int dataStart = DetectDataStartRow(grid, headerRowsToScan);
                    for (int r = dataStart; r < grid.Count; r++)
                    {
                        var row = grid[r];
                        if (IsMostlyEmpty(row)) continue;

                        var p = new Participant();
                        var attFlags = new List<bool>();

                        // No
                        if (noCol >= 0 && noCol < row.Count)
                        {
                            int n;
                            p.No = TryParseInt(row[noCol], out n) ? (int?)n : null;
                        }
                        else
                        {
                            // try first non-empty cell as number
                            int n;
                            int firstTextCol = FirstNonEmptyIndex(row);
                            if (firstTextCol == 0 && TryParseInt(row[0], out n)) p.No = n;
                        }

                        // Names (best effort)
                        var en = nameEnCol >= 0 && nameEnCol < row.Count ? row[nameEnCol] : null;
                        var fa = nameFaCol >= 0 && nameFaCol < row.Count ? row[nameFaCol] : null;

                        // If not found, guess from left side after No
                        if (string.IsNullOrWhiteSpace(en) && string.IsNullOrWhiteSpace(fa))
                        {
                            int startIdx = 0;
                            if (p.No.HasValue) startIdx = Math.Min(row.Count - 1, 1);
                            for (int c = startIdx; c < row.Count; c++)
                            {
                                var txt = row[c];
                                if (string.IsNullOrWhiteSpace(txt)) continue;
                                if (ContainsPersian(txt)) { fa = txt; if (!string.IsNullOrWhiteSpace(en)) break; }
                                else { if (string.IsNullOrWhiteSpace(en)) en = txt; }
                            }
                        }

                        p.NameEn = CleanName(en);
                        p.NameFa = CleanName(fa);

                        // Attendance across session columns
                        foreach (var s in ordered)
                        {
                            bool attended = false;
                            if (s.Col < row.Count)
                            {
                                var v = row[s.Col];
                                attended = EvaluateAttendance(v);
                            }
                            attFlags.Add(attended);
                        }


                        p.Attendance = string.Join(",", attFlags.Select(b => b ? "true" : "false"));
                        // Consider it a participant row if it has a name
                        if (!string.IsNullOrWhiteSpace(p.NameEn) || !string.IsNullOrWhiteSpace(p.NameFa))
                        {
                            participants.Add(p);
                        }
                    }

                    // Prefer the first table that looked like attendance
                    if (participants.Count > 0) break;
                }

                rec.Sessions = sessions;
                rec.Participants = participants;
                // Counts
                rec.SessionsCount = rec.Sessions != null ? rec.Sessions.Count : 0;
                rec.ParticipantsCount = rec.Participants != null ? rec.Participants.Count : 0;

                // Stash raw unmatched bits to help debugging
                rec.Raw = new Dictionary<string, string>();
                foreach (var kv in map)
                {
                    if (kv.Key != "CourseTitle" && kv.Key != "Department" && kv.Key != "StartingDate" && kv.Key != "EndingDate" && kv.Key != "Date" && kv.Key != "Duration" && kv.Key != "Instructor")
                    {
                        rec.Raw[kv.Key] = kv.Value;
                    }
                }

                var _course = new ava_course();
                _course.filepath = rec.FilePath;
                _course.filename = rec.FileName;
                _course.EndDate = rec.EndDate;
                _course.StartDate = rec.StartDate;
                _course.Instructor = rec.Instructor;
                _course.CourseTitle = rec.CourseTitle;
                _course.sessions_count = rec.SessionsCount;
                _course.participants_count = rec.ParticipantsCount;
                _course.Duration_Days = rec.Duration.Days;
                _course.Duration_Hours= rec.Duration.Hours;
                foreach (var s in rec.Sessions)
                    _course.ava_session.Add(new ava_session()
                    {
                        remark = s,
                    });

                foreach (var p in rec.Participants)
                {
                    _course.ava_student.Add(new ava_student()
                    {
                        name = p.NameEn,
                        name_fa = p.NameFa,
                        attendance = p.Attendance,
                    });
                }
                context.ava_course.Add(_course);
                context.SaveChanges();

                return rec;
            }
        }
        private static bool EvaluateAttendance(string cell)
        {
            if (string.IsNullOrWhiteSpace(cell)) return false;
            cell = NormalizeSpaces(cell);

            // Marks meaning present
            string[] presentMarks = { "✓", "✔", "√", "☑", "✅", "🗸", "ü" };
            for (int i = 0; i < presentMarks.Length; i++)
                if (cell.IndexOf(presentMarks[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

            // Explicit present keywords
            var lower = cell.ToLowerInvariant();
            if (lower == "p" || lower == "present" || lower == "yes" || lower == "y" || lower == "حاضر")
                return true;

            // Explicit absent marks/keywords
            string[] absentMarks = { "✗", "✘", "×", "x", "☒", "❌", "غایب", "absent" };
            for (int i = 0; i < absentMarks.Length; i++)
                if (cell.IndexOf(absentMarks[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return false;

            // Default: only tick = present; everything else absent
            return false;
        }
        //public static CourseRecord ParseDocx4(string filePath)
        //{
        //    using (var doc = WordprocessingDocument.Open(filePath, false))
        //    {
        //        var body = doc.MainDocumentPart.Document.Body;

        //        // Text anywhere (headers/meta) — many docs keep text inside tables
        //        var paragraphs = body.Descendants<Paragraph>().ToList(); // include paragraphs inside tables too
        //        var lines = new List<string>();
        //        foreach (var p in paragraphs)
        //        {
        //            var t = GetParagraphText(p);
        //            if (!string.IsNullOrWhiteSpace(t)) lines.Add(NormalizeSpaces(t));
        //        }
        //        var all = string.Join("\n", lines);

        //        // Extract top fields with resilient regexes
        //        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        //        TryAdd(map, "CourseTitle", MatchAfter(all, @"Course\s*Title\s*:\s*(.*?)(?=\s+(?:Department|Starting\s*Date|Ending\s*Date|Date|Instructor.?s?\s*Name|Duration)\s*:|$)"));
        //        TryAdd(map, "Department", MatchAfter(all, @"Department\s*:\s*(.+?)(?:\s{2,}|$)"));
        //        TryAdd(map, "StartingDate", MatchAfter(all, @"Starting\s*Date\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //        TryAdd(map, "EndingDate", MatchAfter(all, @"Ending\s*Date\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //        TryAdd(map, "Date", MatchAfter(all, @"\bDate\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //        TryAdd(map, "Duration", MatchAfter(all, @"Duration\s*:\s*([^\n]+)"));
        //        TryAdd(map, "Instructor", MatchAfter(all, @"Instructor.?s?\s*Name\s*:\s*(.+)"));

        //        // Build record
        //        var rec = new CourseRecord();
        //        rec.CourseTitle = Get(map, "CourseTitle");
        //        rec.Department = Get(map, "Department");
        //        rec.Instructor = Get(map, "Instructor");
        //        rec.Duration = ParseDuration(Get(map, "Duration"));

        //        // File info
        //        rec.FileName = Path.GetFileName(filePath);
        //        rec.FilePath = Path.GetFullPath(filePath);

        //        // Dates (support Persian/Jalali too)
        //        var start = Get(map, "StartingDate");
        //        var end = Get(map, "EndingDate");
        //        var singleDate = Get(map, "Date");
        //        if (!string.IsNullOrEmpty(start)) rec.StartDate = ParseFlexibleDate(start);
        //        if (!string.IsNullOrEmpty(end)) rec.EndDate = ParseFlexibleDate(end);
        //        if (rec.StartDate == null && rec.EndDate == null && !string.IsNullOrEmpty(singleDate))
        //        {
        //            var d = ParseFlexibleDate(singleDate);
        //            rec.StartDate = d; rec.EndDate = d;
        //        }

        //        // Attendance table(s)
        //        var sessions = new List<string>();
        //        var participants = new List<Participant>();

        //        foreach (var table in body.Descendants<Table>())
        //        {
        //            var grid = ReadTable(table); // rows of cell texts
        //            if (grid.Count == 0) continue;

        //            // Collect possible session headers (time ranges like 10:00-12:00 etc) from top few rows
        //            var timeRegex = new Regex(@"\b\d{1,2}:\d{2}\s*[-–]\s*\d{1,2}:\d{2}\b");
        //            var headerRowsToScan = Math.Min(10, grid.Count); // scan a few more rows for headers
        //            var candidateSessionCols = new List<int>();
        //            var candidateSessionLabels = new List<string>();

        //            for (int r = 0; r < headerRowsToScan; r++)
        //            {
        //                var row = grid[r];
        //                for (int c = 0; c < row.Count; c++)
        //                {
        //                    var cell = row[c];
        //                    if (string.IsNullOrWhiteSpace(cell)) continue;
        //                    if (timeRegex.IsMatch(cell) && !candidateSessionCols.Contains(c))
        //                    {
        //                        candidateSessionCols.Add(c);
        //                        candidateSessionLabels.Add(FirstMatch(cell, timeRegex));
        //                    }
        //                }
        //            }

        //            if (candidateSessionCols.Count == 0)
        //            {
        //                // Not an attendance table
        //                continue;
        //            }

        //            // Try to find columns for No / Name EN / Name FA from header-ish area
        //            var noCol = FindColumnIndex(grid, headerRowsToScan, new[] { "No", "NO", "#" });
        //            var nameEnCol = FindColumnIndex(grid, headerRowsToScan, new[] { "Name", "First Name", "Full Name", "نام انگلیسی" });
        //            // For Persian, look for a cell that literally contains Persian letters near name header
        //            var nameFaCol = FindPersianNameColumn(grid, headerRowsToScan);
        //            // If headers have two "Name" columns, assume the latter is Persian
        //            if (nameFaCol < 0)
        //            {
        //                var headerNameCols = FindAllColumns(grid, headerRowsToScan, new[] { "Name" });
        //                if (headerNameCols.Count >= 2)
        //                {
        //                    nameEnCol = headerNameCols[0];
        //                    nameFaCol = headerNameCols[headerNameCols.Count - 1];
        //                }
        //                else if (nameEnCol >= 0)
        //                {
        //                    // Fallback: probe first 10 data rows for a column with Persian text
        //                    int probeStart = DetectDataStartRow(grid, headerRowsToScan);
        //                    nameFaCol = FindPersianRichColumn(grid, probeStart, maxRows: 10);
        //                }
        //            }

        //            // Build sessions list (ordered by column index)
        //            var ordered = candidateSessionCols
        //                .Select((col, idx) => new { Col = col, Label = candidateSessionLabels[idx] })
        //                .OrderBy(x => x.Col)
        //                .ToList();
        //            sessions = ordered.Select(x => x.Label).Distinct().ToList();

        //            // Decide where data rows actually start: first row whose first non-empty cell looks like a number (row no.)
        //            int dataStart = DetectDataStartRow(grid, headerRowsToScan);
        //            for (int r = dataStart; r < grid.Count; r++)
        //            {
        //                var row = grid[r];
        //                if (IsMostlyEmpty(row)) continue;

        //                var p = new Participant();
        //                p.Attendance = new List<bool>();

        //                // No
        //                if (noCol >= 0 && noCol < row.Count)
        //                {
        //                    int n;
        //                    p.No = TryParseInt(row[noCol], out n) ? (int?)n : null;
        //                }
        //                else
        //                {
        //                    // try first non-empty cell as number
        //                    int n;
        //                    int firstTextCol = FirstNonEmptyIndex(row);
        //                    if (firstTextCol == 0 && TryParseInt(row[0], out n)) p.No = n;
        //                }

        //                // Names (best effort)
        //                var en = nameEnCol >= 0 && nameEnCol < row.Count ? row[nameEnCol] : null;
        //                var fa = nameFaCol >= 0 && nameFaCol < row.Count ? row[nameFaCol] : null;

        //                // If not found, guess from left side after No
        //                if (string.IsNullOrWhiteSpace(en) && string.IsNullOrWhiteSpace(fa))
        //                {
        //                    int startIdx = 0;
        //                    if (p.No.HasValue) startIdx = Math.Min(row.Count - 1, 1);
        //                    for (int c = startIdx; c < row.Count; c++)
        //                    {
        //                        var txt = row[c];
        //                        if (string.IsNullOrWhiteSpace(txt)) continue;
        //                        if (ContainsPersian(txt)) { fa = txt; if (!string.IsNullOrWhiteSpace(en)) break; }
        //                        else { if (string.IsNullOrWhiteSpace(en)) en = txt; }
        //                    }
        //                }

        //                p.NameEn = CleanName(en);
        //                p.NameFa = CleanName(fa);

        //                // Attendance across session columns
        //                foreach (var s in ordered)
        //                {
        //                    bool attended = false;
        //                    if (s.Col < row.Count)
        //                    {
        //                        var v = row[s.Col];
        //                        attended = !string.IsNullOrWhiteSpace(v);
        //                    }
        //                    p.Attendance.Add(attended);
        //                }

        //                // Consider it a participant row if it has a name
        //                if (!string.IsNullOrWhiteSpace(p.NameEn) || !string.IsNullOrWhiteSpace(p.NameFa))
        //                {
        //                    participants.Add(p);
        //                }
        //            }

        //            // Prefer the first table that looked like attendance
        //            if (participants.Count > 0) break;
        //        }

        //        rec.Sessions = sessions;
        //        rec.Participants = participants;
        //        // Counts
        //        rec.SessionsCount = rec.Sessions != null ? rec.Sessions.Count : 0;
        //        rec.ParticipantsCount = rec.Participants != null ? rec.Participants.Count : 0;

        //        // Stash raw unmatched bits to help debugging
        //        rec.Raw = new Dictionary<string, string>();
        //        foreach (var kv in map)
        //        {
        //            if (kv.Key != "CourseTitle" && kv.Key != "Department" && kv.Key != "StartingDate" && kv.Key != "EndingDate" && kv.Key != "Date" && kv.Key != "Duration" && kv.Key != "Instructor")
        //            {
        //                rec.Raw[kv.Key] = kv.Value;
        //            }
        //        }

        //        return rec;
        //    }
        //}
        //public static CourseRecord ParseDocx3(string filePath)
        //{
        //    using (var doc = WordprocessingDocument.Open(filePath, false))
        //    {
        //        var body = doc.MainDocumentPart.Document.Body;

        //        // Text anywhere (headers/meta) — many docs keep text inside tables
        //        var paragraphs = body.Descendants<Paragraph>().ToList(); // include paragraphs inside tables too
        //        var lines = new List<string>();
        //        foreach (var p in paragraphs)
        //        {
        //            var t = GetParagraphText(p);
        //            if (!string.IsNullOrWhiteSpace(t)) lines.Add(NormalizeSpaces(t));
        //        }
        //        var all = string.Join("\n", lines);

        //        // Extract top fields with resilient regexes
        //        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        //        TryAdd(map, "CourseTitle", MatchAfter(all, @"Course\s*Title\s*:\s*(.*?)(?=\s+(?:Department|Starting\s*Date|Ending\s*Date|Date|Instructor.?s?\s*Name|Duration)\s*:|$)"));
        //        TryAdd(map, "Department", MatchAfter(all, @"Department\s*:\s*(.+?)(?:\s{2,}|$)"));
        //        TryAdd(map, "StartingDate", MatchAfter(all, @"Starting\s*Date\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //        TryAdd(map, "EndingDate", MatchAfter(all, @"Ending\s*Date\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //        TryAdd(map, "Date", MatchAfter(all, @"\bDate\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //        TryAdd(map, "Duration", MatchAfter(all, @"Duration\s*:\s*([^\n]+)"));
        //        TryAdd(map, "Instructor", MatchAfter(all, @"Instructor.?s?\s*Name\s*:\s*(.+)"));

        //        // Build record
        //        var rec = new CourseRecord();
        //        rec.CourseTitle = Get(map, "CourseTitle");
        //        rec.Department = Get(map, "Department");
        //        rec.Instructor = Get(map, "Instructor");
        //        rec.Duration = ParseDuration(Get(map, "Duration"));

        //        // Dates (support Persian/Jalali too)
        //        var start = Get(map, "StartingDate");
        //        var end = Get(map, "EndingDate");
        //        var singleDate = Get(map, "Date");
        //        if (!string.IsNullOrEmpty(start)) rec.StartDate = ParseFlexibleDate(start);
        //        if (!string.IsNullOrEmpty(end)) rec.EndDate = ParseFlexibleDate(end);
        //        if (rec.StartDate == null && rec.EndDate == null && !string.IsNullOrEmpty(singleDate))
        //        {
        //            var d = ParseFlexibleDate(singleDate);
        //            rec.StartDate = d; rec.EndDate = d;
        //        }

        //        // Attendance table(s)
        //        var sessions = new List<string>();
        //        var participants = new List<Participant>();

        //        foreach (var table in body.Descendants<Table>())
        //        {
        //            var grid = ReadTable(table); // rows of cell texts
        //            if (grid.Count == 0) continue;

        //            // Collect possible session headers (time ranges like 10:00-12:00 etc) from top few rows
        //            var timeRegex = new Regex(@"\b\d{1,2}:\d{2}\s*[-–]\s*\d{1,2}:\d{2}\b");
        //            var headerRowsToScan = Math.Min(10, grid.Count); // scan a few more rows for headers
        //            var candidateSessionCols = new List<int>();
        //            var candidateSessionLabels = new List<string>();

        //            for (int r = 0; r < headerRowsToScan; r++)
        //            {
        //                var row = grid[r];
        //                for (int c = 0; c < row.Count; c++)
        //                {
        //                    var cell = row[c];
        //                    if (string.IsNullOrWhiteSpace(cell)) continue;
        //                    if (timeRegex.IsMatch(cell) && !candidateSessionCols.Contains(c))
        //                    {
        //                        candidateSessionCols.Add(c);
        //                        candidateSessionLabels.Add(FirstMatch(cell, timeRegex));
        //                    }
        //                }
        //            }

        //            if (candidateSessionCols.Count == 0)
        //            {
        //                // Not an attendance table
        //                continue;
        //            }

        //            // Try to find columns for No / Name EN / Name FA from header-ish area
        //            var noCol = FindColumnIndex(grid, headerRowsToScan, new[] { "No", "NO", "#" });
        //            var nameEnCol = FindColumnIndex(grid, headerRowsToScan, new[] { "Name", "First Name", "Full Name", "نام انگلیسی" });
        //            // For Persian, look for a cell that literally contains Persian letters near name header
        //            var nameFaCol = FindPersianNameColumn(grid, headerRowsToScan);
        //            // If headers have two "Name" columns, assume the latter is Persian
        //            if (nameFaCol < 0)
        //            {
        //                var headerNameCols = FindAllColumns(grid, headerRowsToScan, new[] { "Name" });
        //                if (headerNameCols.Count >= 2)
        //                {
        //                    nameEnCol = headerNameCols[0];
        //                    nameFaCol = headerNameCols[headerNameCols.Count - 1];
        //                }
        //                else if (nameEnCol >= 0)
        //                {
        //                    // Fallback: probe first 10 data rows for a column with Persian text
        //                    int probeStart = DetectDataStartRow(grid, headerRowsToScan);
        //                    nameFaCol = FindPersianRichColumn(grid, probeStart, maxRows: 10);
        //                }
        //            }

        //            // Build sessions list (ordered by column index)
        //            var ordered = candidateSessionCols
        //                .Select((col, idx) => new { Col = col, Label = candidateSessionLabels[idx] })
        //                .OrderBy(x => x.Col)
        //                .ToList();
        //            sessions = ordered.Select(x => x.Label).Distinct().ToList();

        //            // Decide where data rows actually start: first row whose first non-empty cell looks like a number (row no.)
        //            int dataStart = DetectDataStartRow(grid, headerRowsToScan);
        //            for (int r = dataStart; r < grid.Count; r++)
        //            {
        //                var row = grid[r];
        //                if (IsMostlyEmpty(row)) continue;

        //                var p = new Participant();
        //                p.Attendance = new List<bool>();

        //                // No
        //                if (noCol >= 0 && noCol < row.Count)
        //                {
        //                    int n;
        //                    p.No = TryParseInt(row[noCol], out n) ? (int?)n : null;
        //                }
        //                else
        //                {
        //                    // try first non-empty cell as number
        //                    int n;
        //                    int firstTextCol = FirstNonEmptyIndex(row);
        //                    if (firstTextCol == 0 && TryParseInt(row[0], out n)) p.No = n;
        //                }

        //                // Names (best effort)
        //                var en = nameEnCol >= 0 && nameEnCol < row.Count ? row[nameEnCol] : null;
        //                var fa = nameFaCol >= 0 && nameFaCol < row.Count ? row[nameFaCol] : null;

        //                // If not found, guess from left side after No
        //                if (string.IsNullOrWhiteSpace(en) && string.IsNullOrWhiteSpace(fa))
        //                {
        //                    int startIdx = 0;
        //                    if (p.No.HasValue) startIdx = Math.Min(row.Count - 1, 1);
        //                    for (int c = startIdx; c < row.Count; c++)
        //                    {
        //                        var txt = row[c];
        //                        if (string.IsNullOrWhiteSpace(txt)) continue;
        //                        if (ContainsPersian(txt)) { fa = txt; if (!string.IsNullOrWhiteSpace(en)) break; }
        //                        else { if (string.IsNullOrWhiteSpace(en)) en = txt; }
        //                    }
        //                }

        //                p.NameEn = CleanName(en);
        //                p.NameFa = CleanName(fa);

        //                // Attendance across session columns
        //                foreach (var s in ordered)
        //                {
        //                    bool attended = false;
        //                    if (s.Col < row.Count)
        //                    {
        //                        var v = row[s.Col];
        //                        attended = !string.IsNullOrWhiteSpace(v);
        //                    }
        //                    p.Attendance.Add(attended);
        //                }

        //                // Consider it a participant row if it has a name
        //                if (!string.IsNullOrWhiteSpace(p.NameEn) || !string.IsNullOrWhiteSpace(p.NameFa))
        //                {
        //                    participants.Add(p);
        //                }
        //            }

        //            // Prefer the first table that looked like attendance
        //            if (participants.Count > 0) break;
        //        }

        //        rec.Sessions = sessions;
        //        rec.Participants = participants;

        //        // Stash raw unmatched bits to help debugging
        //        rec.Raw = new Dictionary<string, string>();
        //        foreach (var kv in map)
        //        {
        //            if (kv.Key != "CourseTitle" && kv.Key != "Department" && kv.Key != "StartingDate" && kv.Key != "EndingDate" && kv.Key != "Date" && kv.Key != "Duration" && kv.Key != "Instructor")
        //            {
        //                rec.Raw[kv.Key] = kv.Value;
        //            }
        //        }

        //        return rec;
        //    }
        //}
        private static int DetectDataStartRow(List<List<string>> grid, int headerRowsToScan)
        {
            for (int r = 0; r < grid.Count; r++)
            {
                if (r < headerRowsToScan)
                {
                    int n; if (grid[r].Count > 0 && TryParseInt(grid[r][0], out n)) return r;
                    continue;
                }
                var row = grid[r];
                int n2; if (row.Count > 0 && TryParseInt(row[0], out n2)) return r;
                int nonEmpty = row.Count(c => !string.IsNullOrWhiteSpace(c));
                if (nonEmpty >= 3) return r;
            }
            return headerRowsToScan;
        }
        private static int FindPersianRichColumn(List<List<string>> grid, int dataStart, int maxRows)
        {
            if (grid.Count == 0) return -1;
            int cols = grid.Max(r => r.Count);
            int bestCol = -1; int bestScore = 0;
            for (int c = 0; c < cols; c++)
            {
                int score = 0; int checkedRows = 0;
                for (int r = dataStart; r < grid.Count && checkedRows < maxRows; r++)
                {
                    var cell = GetCell(grid, r, c);
                    if (string.IsNullOrWhiteSpace(cell)) continue;
                    checkedRows++;
                    if (ContainsPersian(cell)) score++;
                }
                if (score > bestScore)
                { bestScore = score; bestCol = c; }
            }
            return bestCol;
        }
        // --- Core parser ---
        //   public static CourseRecord ParseDocx2(string filePath)
        //   {
        //       using (var doc = WordprocessingDocument.Open(filePath, false))
        //       {
        //           var body = doc.MainDocumentPart.Document.Body;

        //           // Text outside tables (headers/meta)
        //           //var paragraphs = body.Descendants<Paragraph>().Where(p => p.Parent is Body).ToList();
        //           var paragraphs = body.Descendants<Paragraph>().ToList(); // include paragraphs inside tables too

        //           var lines = new List<string>();
        //           foreach (var p in paragraphs)
        //           {
        //               var t = GetParagraphText(p);
        //               if (!string.IsNullOrWhiteSpace(t)) lines.Add(NormalizeSpaces(t));
        //           }
        //           var all = string.Join("\n", lines);

        //           // Extract top fields with resilient regexes
        //           var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        //           TryAdd(map, "CourseTitle",
        //MatchAfter(all,
        //@"Course\s*Title\s*:\s*(.*?)(?=\s+(?:Department|Starting\s*Date|Ending\s*Date|Date|Instructor.?s?\s*Name|Duration)\s*:|$)"));

        //           TryAdd(map, "Department", MatchAfter(all, @"Department\s*:\s*(.+?)(?:\s{2,}|$)"));
        //           TryAdd(map, "StartingDate", MatchAfter(all, @"Starting\s*Date\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //           TryAdd(map, "EndingDate", MatchAfter(all, @"Ending\s*Date\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //           TryAdd(map, "Date", MatchAfter(all, @"\bDate\s*:\s*([\p{Nd}0-9/\\\-\.]+)"));
        //           TryAdd(map, "Duration", MatchAfter(all, @"Duration\s*:\s*([^\n]+)"));
        //           TryAdd(map, "Instructor", MatchAfter(all, @"Instructor.?s?\s*Name\s*:\s*(.+)"));

        //           // Build record
        //           var rec = new CourseRecord();
        //           rec.CourseTitle = Get(map, "CourseTitle");
        //           rec.Department = Get(map, "Department");
        //           rec.Instructor = Get(map, "Instructor");
        //           rec.Duration = ParseDuration(Get(map, "Duration"));

        //           // Dates (support Persian/Jalali too)
        //           var start = Get(map, "StartingDate");
        //           var end = Get(map, "EndingDate");
        //           var singleDate = Get(map, "Date");
        //           if (!string.IsNullOrEmpty(start)) rec.StartDate = ParseFlexibleDate(start);
        //           if (!string.IsNullOrEmpty(end)) rec.EndDate = ParseFlexibleDate(end);
        //           if (rec.StartDate == null && rec.EndDate == null && !string.IsNullOrEmpty(singleDate))
        //           {
        //               var d = ParseFlexibleDate(singleDate);
        //               rec.StartDate = d; rec.EndDate = d;
        //           }

        //           // Attendance table(s)
        //           var sessions = new List<string>();
        //           var participants = new List<Participant>();

        //           foreach (var table in body.Elements<Table>())
        //           {
        //               var grid = ReadTable(table); // rows of cell texts
        //               if (grid.Count == 0) continue;

        //               // Collect possible session headers (time ranges like 10:00-12:00 etc) from top few rows
        //               var timeRegex = new Regex(@"\b\d{1,2}:\d{2}\s*[-–]\s*\d{1,2}:\d{2}\b");
        //               var headerRowsToScan = Math.Min(6, grid.Count);
        //               var candidateSessionCols = new List<int>();
        //               var candidateSessionLabels = new List<string>();

        //               for (int r = 0; r < headerRowsToScan; r++)
        //               {
        //                   var row = grid[r];
        //                   for (int c = 0; c < row.Count; c++)
        //                   {
        //                       var cell = row[c];
        //                       if (string.IsNullOrWhiteSpace(cell)) continue;
        //                       if (timeRegex.IsMatch(cell) && !candidateSessionCols.Contains(c))
        //                       {
        //                           candidateSessionCols.Add(c);
        //                           candidateSessionLabels.Add(FirstMatch(cell, timeRegex));
        //                       }
        //                   }
        //               }

        //               if (candidateSessionCols.Count == 0)
        //               {
        //                   // Not an attendance table
        //                   continue;
        //               }

        //               // Try to find columns for No / Name EN / Name FA from header-ish area
        //               var noCol = FindColumnIndex(grid, headerRowsToScan, new[] { "No", "NO", "#" });
        //               var nameEnCol = FindColumnIndex(grid, headerRowsToScan, new[] { "Name", "First Name", "Full Name" });
        //               // For Persian, look for a cell that literally contains Persian letters near name header
        //               var nameFaCol = FindPersianNameColumn(grid, headerRowsToScan);

        //               // Build sessions list (ordered by column index)
        //               var ordered = candidateSessionCols
        //                   .Select((col, idx) => new { Col = col, Label = candidateSessionLabels[idx] })
        //                   .OrderBy(x => x.Col)
        //                   .ToList();
        //               sessions = ordered.Select(x => x.Label).Distinct().ToList();

        //               // Data rows start after headerRowsToScan (heuristic) — but also include rows that clearly start with a row number.
        //               for (int r = headerRowsToScan; r < grid.Count; r++)
        //               {
        //                   var row = grid[r];
        //                   if (IsMostlyEmpty(row)) continue;

        //                   var p = new Participant();
        //                   p.Attendance = new List<bool>();

        //                   // No
        //                   if (noCol >= 0 && noCol < row.Count)
        //                   {
        //                       int n;
        //                       p.No = TryParseInt(row[noCol], out n) ? (int?)n : null;
        //                   }
        //                   else
        //                   {
        //                       // try first non-empty cell as number
        //                       int n;
        //                       int firstTextCol = FirstNonEmptyIndex(row);
        //                       if (firstTextCol == 0 && TryParseInt(row[0], out n)) p.No = n;
        //                   }

        //                   // Names (best effort)
        //                   var en = nameEnCol >= 0 && nameEnCol < row.Count ? row[nameEnCol] : null;
        //                   var fa = nameFaCol >= 0 && nameFaCol < row.Count ? row[nameFaCol] : null;

        //                   // If not found, guess from left side after No
        //                   if (string.IsNullOrWhiteSpace(en) && string.IsNullOrWhiteSpace(fa))
        //                   {
        //                       int startIdx = 0;
        //                       if (p.No.HasValue) startIdx = Math.Min(row.Count - 1, 1);
        //                       for (int c = startIdx; c < row.Count; c++)
        //                       {
        //                           var txt = row[c];
        //                           if (string.IsNullOrWhiteSpace(txt)) continue;
        //                           if (ContainsPersian(txt)) { fa = txt; if (!string.IsNullOrWhiteSpace(en)) break; }
        //                           else { if (string.IsNullOrWhiteSpace(en)) en = txt; }
        //                       }
        //                   }

        //                   p.NameEn = CleanName(en);
        //                   p.NameFa = CleanName(fa);

        //                   // Attendance across session columns
        //                   foreach (var s in ordered)
        //                   {
        //                       bool attended = false;
        //                       if (s.Col < row.Count)
        //                       {
        //                           var v = row[s.Col];
        //                           attended = !string.IsNullOrWhiteSpace(v);
        //                       }
        //                       p.Attendance.Add(attended);
        //                   }

        //                   // Consider it a participant row if it has a name
        //                   if (!string.IsNullOrWhiteSpace(p.NameEn) || !string.IsNullOrWhiteSpace(p.NameFa))
        //                   {
        //                       participants.Add(p);
        //                   }
        //               }

        //               // Prefer the first table that looked like attendance
        //               if (participants.Count > 0) break;
        //           }

        //           rec.Sessions = sessions;
        //           rec.Participants = participants;

        //           // Stash raw unmatched bits to help debugging
        //           rec.Raw = new Dictionary<string, string>();
        //           foreach (var kv in map)
        //           {
        //               if (kv.Key != "CourseTitle" && kv.Key != "Department" && kv.Key != "StartingDate" && kv.Key != "EndingDate" && kv.Key != "Date" && kv.Key != "Duration" && kv.Key != "Instructor")
        //               {
        //                   rec.Raw[kv.Key] = kv.Value;
        //               }
        //           }

        //           return rec;
        //       }
        //   }

        // --- Helpers ---
        private static string GetParagraphText(Paragraph p)
        {
            var sb = new StringBuilder();
            foreach (var t in p.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                sb.Append(t.Text);
            return sb.ToString();
        }

        private static List<List<string>> ReadTable(Table t)
        {
            var rows = new List<List<string>>();
            foreach (var tr in t.Elements<TableRow>())
            {
                var row = new List<string>();
                foreach (var tc in tr.Elements<TableCell>())
                {
                    var txt = string.Concat(tc.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>().Select(x => x.Text));
                    row.Add(NormalizeSpaces(txt));
                }
                rows.Add(row);
            }
            return rows;
        }

        private static int FindColumnIndex(List<List<string>> grid, int headerRowsToScan, string[] keywords)
        {
            for (int c = 0; c < MaxColumns(grid, headerRowsToScan); c++)
            {
                for (int r = 0; r < headerRowsToScan && r < grid.Count; r++)
                {
                    var cell = GetCell(grid, r, c);
                    if (string.IsNullOrWhiteSpace(cell)) continue;
                    for (int k = 0; k < keywords.Length; k++)
                    {
                        if (cell.IndexOf(keywords[k], StringComparison.OrdinalIgnoreCase) >= 0)
                            return c;
                    }
                }
            }
            return -1;
        }

        private static int FindPersianNameColumn(List<List<string>> grid, int headerRowsToScan)
        {
            for (int c = 0; c < MaxColumns(grid, headerRowsToScan); c++)
            {
                for (int r = 0; r < headerRowsToScan && r < grid.Count; r++)
                {
                    var cell = GetCell(grid, r, c);
                    if (string.IsNullOrWhiteSpace(cell)) continue;
                    if (ContainsPersian(cell) && (cell.Contains("نام") || cell.Contains("Name")))
                        return c;
                }
            }
            return -1;
        }

        private static string GetCell(List<List<string>> grid, int r, int c)
        {
            if (r < 0 || r >= grid.Count) return null;
            var row = grid[r];
            if (c < 0 || c >= row.Count) return null;
            return row[c];
        }

        private static int MaxColumns(List<List<string>> grid, int rows)
        {
            var max = 0;
            for (int r = 0; r < rows && r < grid.Count; r++)
                if (grid[r].Count > max) max = grid[r].Count;
            return max;
        }

        private static bool IsMostlyEmpty(List<string> row)
        {
            int nonEmpty = 0;
            for (int i = 0; i < row.Count; i++) if (!string.IsNullOrWhiteSpace(row[i])) nonEmpty++;
            return nonEmpty <= 1; // allow only one filled cell as empty-ish
        }

        private static int FirstNonEmptyIndex(List<string> row)
        {
            for (int i = 0; i < row.Count; i++) if (!string.IsNullOrWhiteSpace(row[i])) return i;
            return -1;
        }

        private static string NormalizeSpaces(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = s.Replace('\u00A0', ' ');
            s = ConvertPersianDigitsToAscii(s);
            return Regex.Replace(s, "\\s+", " ").Trim();
        }

        private static string FirstMatch(string input, Regex re)
        {
            var m = re.Match(input ?? string.Empty);
            return m.Success ? m.Value : null;
        }

        private static void TryAdd(Dictionary<string, string> map, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) map[key] = value.Trim();
        }

        private static string Get(Dictionary<string, string> map, string key)
        {
            string v; return map.TryGetValue(key, out v) ? v : null;
        }

        private static string MatchAfter(string input, string pattern)
        {
            if (string.IsNullOrEmpty(input)) return null;
            var m = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            if (!m.Success) return null;
            if (m.Groups.Count > 1) return m.Groups[1].Value.Trim();
            return m.Value.Trim();
        }

        private static Duration ParseDuration(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            // Try formats like "06 / 1 Hrs / days" or "6 hours" "1 day"
            // Extract first two integers as hours and days in either order based on keywords
            var hours = (int?)null; var days = (int?)null;
            var nums = Regex.Matches(s, @"\d+").Cast<Match>().Select(m => SafeToInt(m.Value)).ToList();
            var lower = s.ToLowerInvariant();
            if (lower.Contains("hr"))
            {
                var m = Regex.Match(s, @"(\d+)\s*hr");
                if (m.Success) hours = SafeToInt(m.Groups[1].Value);
            }
            if (lower.Contains("day"))
            {
                var m = Regex.Match(s, @"(\d+)\s*day");
                if (m.Success) days = SafeToInt(m.Groups[1].Value);
            }
            if (hours == null && nums.Count > 0) hours = nums[0];
            if (days == null && nums.Count > 1) days = nums[1];
            return new Duration { Hours = hours, Days = days };
        }

        private static int SafeToInt(string s)
        {
            int n; return int.TryParse(ConvertPersianDigitsToAscii(s), out n) ? n : 0;
        }

        private static bool TryParseInt(string s, out int n)
        {
            return int.TryParse(ConvertPersianDigitsToAscii(NormalizeSpaces(s)), out n);
        }

        private static string CleanName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            // Remove stray punctuation used for layout
            s = s.Replace("……", " ").Replace("__", " ").Replace("..", " ");
            return NormalizeSpaces(s);
        }

        private static bool ContainsPersian(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char ch in s)
            {
                if ((ch >= '\u0600' && ch <= '\u06FF') || (ch >= '\u0750' && ch <= '\u077F') || (ch >= '\u08A0' && ch <= '\u08FF'))
                    return true;
            }
            return false;
        }

        private static DateTime? ParseFlexibleDate(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = NormalizeSpaces(s);

            // If looks like yyyy/MM/dd with year 13xx/14xx => treat as Persian calendar
            var m = Regex.Match(s, @"(\d{3,4})[/-](\d{1,2})[/-](\d{1,2})");
            if (m.Success)
            {
                int y = SafeToInt(m.Groups[1].Value);
                int mo = SafeToInt(m.Groups[2].Value);
                int d = SafeToInt(m.Groups[3].Value);
                if (y >= 1200 && y <= 1599) // Jalali year range
                {
                    try
                    {
                        var pc = new PersianCalendar();
                        return pc.ToDateTime(y, mo, d, 0, 0, 0, 0);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        return new DateTime(y, mo, d);
                    }
                    catch { }
                }
            }

            // Try normal parse
            DateTime dt;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt;

            return null;
        }

        private static string ConvertPersianDigitsToAscii(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var sb = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                // Persian digits ۰۱۲۳۴۵۶۷۸۹ U+06F0..U+06F9
                if (ch >= '\u06F0' && ch <= '\u06F9')
                {
                    sb.Append((char)('0' + (ch - '\u06F0')));
                }
                // Arabic-Indic digits ٠١٢٣٤٥٦٧٨٩ U+0660..U+0669
                else if (ch >= '\u0660' && ch <= '\u0669')
                {
                    sb.Append((char)('0' + (ch - '\u0660')));
                }
                else sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}
