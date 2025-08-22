using AirpocketTRN.Models;
using AirpocketTRN.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AirpocketTRN.Services
{
    public interface ICourseService
    {

    }
    public class CourseService : ICourseService
    {
        FLYEntities context = null;
        public CourseService()
        {
            context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
        }
        public async Task<DataResponse> GetCourseTypes()
        {
            var result = await context.ViewCourseTypes.OrderBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourseTypeGroupsProfile()
        {
            var result = await context.view_trn_course_type_group.Where(q => q.show_in_profile == true).OrderBy(q => q.title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetGRPCTExpiring()
        {
            var result = await context.GRPCourseTypeExpirings.OrderByDescending(q => q.ExpiredCount).ThenByDescending(q => q.ExpiringCount).ThenBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetCourseRemaining(int dd)
        {
            var result = await context.ViewCourseRemainings.Where(q => q.Remaining == dd).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetGRPCTExpiringByMainGroup(string main)
        {
            var result = await context.GRPCourseTypeExpiringMainGroups.Where(q => q.JobGroupMainCode == main).OrderByDescending(q => q.ExpiredCount).ThenByDescending(q => q.ExpiringCount).ThenBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetGRPCTExpiringMainGroups(int typeId)
        {
            var query = from x in context.GRPCourseTypeExpiringMainGroups
                        select x;
            if (typeId != -1)
                query = query.Where(q => q.TypeId == typeId);
            var result = await query.OrderByDescending(q => q.ExpiredCount).ThenByDescending(q => q.ExpiringCount).ThenBy(q => q.JobGroupMain).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetGRPMainGroupsExpiring(string code)
        {
            var query = from x in context.GRPMainGroupsExpirings
                        select x;
            if (code != "-1")
                query = query.Where(q => q.JobGroupMainCode == code);
            var result = await query.OrderByDescending(q => q.ExpiredCount).ThenByDescending(q => q.ExpiringCount).ThenBy(q => q.JobGroupMain).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetGRPGroupsExpiring(string main, string code)
        {
            var query = from x in context.GRPGroupsExpirings
                        select x;
            if (main != "-1")
                query = query.Where(q => q.JobGroupMainCode == main);
            if (code != "-1")
                query = query.Where(q => q.JobGroupCode2 == code);
            var result = await query.OrderByDescending(q => q.ExpiredCount).ThenByDescending(q => q.ExpiringCount).ThenBy(q => q.JobGroupMain).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetGRPGroupsExpiringCourseTypes(string main, string code, int type)
        {
            var query = from x in context.GRPGroupsCourseTypeExpirings
                        select x;
            if (main != "-1")
                query = query.Where(q => q.JobGroupMainCode == main);
            if (code != "-1")
                query = query.Where(q => q.JobGroupCode2 == code);
            if (type != -1)
                query = query.Where(q => q.TypeId == type);
            var result = await query.OrderByDescending(q => q.ExpiredCount).ThenByDescending(q => q.ExpiringCount).ThenBy(q => q.JobGroupMain).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetGRPCTExpiringGroups(string code, int type)
        {
            var query = from x in context.GRPCourseTypeExpiringGroups
                        select x;
            if (code != "-1")
                query = query.Where(q => q.JobGroupMainCode == code);
            if (type != -1)
                query = query.Where(q => q.TypeId == type);
            var result = await query.OrderByDescending(q => q.ExpiredCount).ThenByDescending(q => q.ExpiringCount).ThenBy(q => q.JobGroupMain).ThenBy(q => q.JobGroup).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public class _title_count
        {

        }
        public class _expiring_group
        {
            public int GroupId { get; set; }
            public string GroupTitle { get; set; }
            public string GroupCode { get; set; }

            public int IsExpiring { get; set; }

            public int IsExpired { get; set; }
            public string CertificateType { get; set; }
            public int EmployeesCount { get; set; }
            public double IndexExpiring { get; set; }
            public double IndexExpired { get; set; }

            public List<_expiring_group> Children { get; set; }

            public int GroupLevel { get; set; }
            public dynamic Dates { get; set; }
        }
        public dynamic getCalendar(List<ViewCertificateHistoryRanked> rows, string code)
        {
            var expiring = new List<ViewCertificateHistoryRanked>();
            var expiring_query = rows.Where(q => q.IsExpiring == 1);
            if (!string.IsNullOrEmpty(code))
                expiring_query = expiring_query.Where(q => q.JobGroupCode2.StartsWith(code));
            expiring = expiring_query.ToList();
            var query = from x in expiring
                        group x by new { DateExpire = ((DateTime)x.DateExpire).Date, x.CertificateType } into grp
                        select new
                        {
                            grp.Key.DateExpire,
                            grp.Key.CertificateType,
                            Count = grp.Count(),
                            Items = (
                               from y in grp
                               group y by new { y.L1Title } into grp2
                               select new
                               {
                                   main_group = grp2.Key.L1Title,
                                   employees = grp2.OrderBy(q => q.JobGroup).ThenBy(q => q.LastName).ToList()
                               }
                            ).ToList()
                        };
            var query2 = (from x in query
                          group x by new { x.DateExpire } into grp
                          select new
                          {
                              Date = grp.Key.DateExpire,
                              Month = grp.Key.DateExpire.ToString("MMM"),
                              Items = grp.OrderByDescending(q => q.Count).ToList()
                          }).OrderBy(q => q.Date).ToList();
            var month_names = query2.Select(q => q.Month).Distinct().ToList();
            return new { Items = query2, Monthes = month_names };

        }

        public dynamic getCalendarExpired(List<ViewCertificateHistoryRanked> rows, string code)
        {
            var expiring = new List<ViewCertificateHistoryRanked>();
            var expiring_query = rows.AsEnumerable();
            if (!string.IsNullOrEmpty(code))
                expiring_query = expiring_query.Where(q => q.JobGroupCode2.StartsWith(code));
            expiring = expiring_query.ToList();
            var query = from x in expiring
                        group x by new { DateExpire = ((DateTime)x.DateExpire).Date, x.CertificateType } into grp
                        select new
                        {
                            grp.Key.DateExpire,
                            grp.Key.CertificateType,
                            Count = grp.Count(),
                            Items = (
                               from y in grp
                               group y by new { y.L1Title } into grp2
                               select new
                               {
                                   main_group = grp2.Key.L1Title,
                                   employees = grp2.OrderBy(q => q.JobGroup).ThenBy(q => q.LastName).ToList()
                               }
                            ).ToList()
                        };
            var query2 = (from x in query
                          group x by new { x.DateExpire } into grp
                          select new
                          {
                              Date = grp.Key.DateExpire,
                              Month = grp.Key.DateExpire.ToString("MMM"),
                              Items = grp.OrderByDescending(q => q.Count).ToList()
                          }).OrderBy(q => q.Date).ToList();
            var month_names = query2.Select(q => q.Month).Distinct().ToList();
            return new { Items = query2, Monthes = month_names };

        }
        public async Task<DataResponse> GetMonitoringExpiringGroups()
        {

            var l1_length = 3;
            var lo_length = 2;
            var jobgroups = await context.JobGroups.ToListAsync();
            var _exg = new List<string>() { "TRE", "TRI", "P1", "P2", "ISCCM", "SCCM", "CCM", "CCE", "CCI" };
            var expiring_employees = await context.ViewCertificateHistoryRankeds.Where(q => /*q.InActive == false &&*/ q.IsCritical == 1 /*&& !_exg.Contains(q.JobGroup)*/).ToListAsync();


            //condition: inactive=false
            var expiring_employees_groups = await context.ViewEmployees.Select(q => new { q.Id, q.JobGroupMainCode }).ToListAsync();

            foreach (var emp in expiring_employees)
            {
                var jobgroup_code = emp.JobGroupCode2;
                var l1_code = jobgroup_code.Substring(0, l1_length);
                var l1_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l1_code);
                emp.L1Id = l1_group.Id;
                emp.L1Code = l1_group.FullCode2;
                emp.L1Title = l1_group.Title;



                if (jobgroup_code.Length > l1_length)
                {
                    var l2_code = jobgroup_code.Substring(0, l1_length + lo_length);
                    var l2_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l2_code);

                    emp.L2Id = l2_group.Id;
                    emp.L2Code = l2_group.FullCode2;
                    emp.L2Title = l2_group.Title;
                }
                if (jobgroup_code.Length > l1_length + lo_length * 1)
                {
                    var l3_code = jobgroup_code.Substring(0, l1_length + 2 * lo_length);
                    var l3_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l3_code);

                    emp.L3Id = l3_group.Id;
                    emp.L3Code = l3_group.FullCode2;
                    emp.L3Title = l3_group.Title;
                }
                if (jobgroup_code.Length > l1_length + lo_length * 2)
                {
                    var l4_code = jobgroup_code.Substring(0, l1_length + 3 * lo_length);
                    var l4_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l4_code);

                    emp.L4Id = l4_group.Id;
                    emp.L4Code = l4_group.FullCode2;
                    emp.L4Title = l4_group.Title;
                }
                if (jobgroup_code.Length > l1_length + lo_length * 3)
                {
                    var l5_code = jobgroup_code.Substring(0, l1_length + 4 * lo_length);
                    var l5_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l5_code);

                    emp.L5Id = l5_group.Id;
                    emp.L5Code = l5_group.FullCode2;
                    emp.L5Title = l5_group.Title;
                }
            }


            var dates = getCalendar(expiring_employees, null);
            var query_l1 = (from x in expiring_employees
                            group x by new { x.L1Code, x.L1Id, x.L1Title } into grp
                            select new _expiring_group()
                            {
                                GroupCode = grp.Key.L1Code,
                                GroupTitle = grp.Key.L1Title,
                                GroupId = (int)grp.Key.L1Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 1,
                                EmployeesCount = expiring_employees_groups.Where(q => q.JobGroupMainCode.StartsWith(grp.Key.L1Code)).Count(),
                                Dates = getCalendar(expiring_employees, grp.Key.L1Code)


                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();
            var query_l2 = (from x in expiring_employees
                            group x by new { x.L2Code, x.L2Id, x.L2Title } into grp
                            select new _expiring_group()
                            {
                                GroupCode = grp.Key.L2Code,
                                GroupTitle = grp.Key.L2Title,
                                GroupId = (int)grp.Key.L2Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 2,
                                EmployeesCount = expiring_employees_groups.Where(q => q.JobGroupMainCode.StartsWith(grp.Key.L2Code)).Count(),
                                Dates = getCalendar(expiring_employees, grp.Key.L2Code)
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();


            List<_expiring_group> query_l3 = new List<_expiring_group>();
            List<_expiring_group> query_l4 = new List<_expiring_group>();
            List<_expiring_group> query_l5 = new List<_expiring_group>();

            if (expiring_employees.Where(q => q.JobGroupCode2.Length >= l1_length + 2 * lo_length).Count() > 0)
            {
                //l3
                query_l3 = (from x in expiring_employees
                            group x by new { x.L3Code, x.L3Id, x.L3Title } into grp
                            select new _expiring_group()
                            {
                                GroupCode = grp.Key.L3Code,
                                GroupTitle = grp.Key.L3Title,
                                GroupId = (int)grp.Key.L3Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 3,
                                EmployeesCount = expiring_employees_groups.Where(q => q.JobGroupMainCode.StartsWith(grp.Key.L3Code)).Count(),
                                Dates = getCalendar(expiring_employees, grp.Key.L3Code)
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();
            }

            if (expiring_employees.Where(q => q.JobGroupCode2.Length >= l1_length + 3 * lo_length).Count() > 0)
            {
                //l4
                query_l4 = (from x in expiring_employees
                            group x by new { x.L4Code, x.L4Id, x.L4Title } into grp
                            select new _expiring_group()
                            {
                                GroupCode = grp.Key.L4Code,
                                GroupTitle = grp.Key.L4Title,
                                GroupId = (int)grp.Key.L4Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 4,
                                EmployeesCount = expiring_employees_groups.Where(q => q.JobGroupMainCode.StartsWith(grp.Key.L4Code)).Count(),
                                Dates = getCalendar(expiring_employees, grp.Key.L4Code)
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();
            }

            if (expiring_employees.Where(q => q.JobGroupCode2.Length >= l1_length + 4 * lo_length).Count() > 0)
            {
                //l5
                query_l5 = (from x in expiring_employees
                            group x by new { x.L5Code, x.L5Id, x.L5Title } into grp
                            select new _expiring_group()
                            {
                                GroupCode = grp.Key.L5Code,
                                GroupTitle = grp.Key.L5Title,
                                GroupId = (int)grp.Key.L5Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 5,
                                EmployeesCount = expiring_employees_groups.Where(q => q.JobGroupMainCode.StartsWith(grp.Key.L5Code)).Count(),
                                Dates = getCalendar(expiring_employees, grp.Key.L5Code)
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();
            }

            foreach (var grp in query_l1)
            {
                grp.Children = query_l2.Where(q => q.GroupCode.StartsWith(grp.GroupCode)).ToList();
                grp.IndexExpired = Math.Round(grp.IsExpired * 1.0 / grp.EmployeesCount, 1);
                grp.IndexExpiring = Math.Round(grp.IsExpiring * 1.0 / grp.EmployeesCount, 1);
            }
            foreach (var grp in query_l2)
            {
                grp.Children = query_l3.Where(q => q.GroupCode.StartsWith(grp.GroupCode)).ToList();
                grp.IndexExpired = Math.Round(grp.IsExpired * 1.0 / grp.EmployeesCount, 1);
                grp.IndexExpiring = Math.Round(grp.IsExpiring * 1.0 / grp.EmployeesCount, 1);
            }
            foreach (var grp in query_l3)
            {
                grp.Children = query_l4.Where(q => q.GroupCode.StartsWith(grp.GroupCode)).ToList();
                grp.IndexExpired = Math.Round(grp.IsExpired * 1.0 / grp.EmployeesCount, 1);
                grp.IndexExpiring = Math.Round(grp.IsExpiring * 1.0 / grp.EmployeesCount, 1);
            }

            foreach (var grp in query_l4)
            {
                grp.Children = query_l5.Where(q => q.GroupCode.StartsWith(grp.GroupCode)).ToList();
                grp.IndexExpired = Math.Round(grp.IsExpired * 1.0 / grp.EmployeesCount, 1);
                grp.IndexExpiring = Math.Round(grp.IsExpiring * 1.0 / grp.EmployeesCount, 1);
            }

            var result = new
            {
                employees = expiring_employees,
                Dates = dates,
                groups = query_l1,
            };

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetMonitoringExpiringCertificateTypes()
        {

            var l1_length = 3;
            var lo_length = 2;
            var jobgroups = await context.JobGroups.ToListAsync();
            var expiring_employees = await context.ViewCertificateHistoryRankeds.Where(q => q.InActive == false && q.IsCritical == 1).ToListAsync();

            foreach (var emp in expiring_employees)
            {
                var jobgroup_code = emp.JobGroupCode2;
                var l1_code = jobgroup_code.Substring(0, l1_length);
                var l1_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l1_code);
                emp.L1Id = l1_group.Id;
                emp.L1Code = l1_group.FullCode2;
                emp.L1Title = l1_group.Title;

                if (jobgroup_code.Length > l1_length)
                {
                    var l2_code = jobgroup_code.Substring(0, l1_length + lo_length);
                    var l2_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l2_code);

                    emp.L2Id = l2_group.Id;
                    emp.L2Code = l2_group.FullCode2;
                    emp.L2Title = l2_group.Title;
                }
                if (jobgroup_code.Length > l1_length + lo_length * 1)
                {
                    var l3_code = jobgroup_code.Substring(0, l1_length + 2 * lo_length);
                    var l3_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l3_code);

                    emp.L3Id = l3_group.Id;
                    emp.L3Code = l3_group.FullCode2;
                    emp.L3Title = l3_group.Title;
                }
                if (jobgroup_code.Length > l1_length + lo_length * 2)
                {
                    var l4_code = jobgroup_code.Substring(0, l1_length + 3 * lo_length);
                    var l4_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l4_code);

                    emp.L4Id = l4_group.Id;
                    emp.L4Code = l4_group.FullCode2;
                    emp.L4Title = l4_group.Title;
                }
                if (jobgroup_code.Length > l1_length + lo_length * 3)
                {
                    var l5_code = jobgroup_code.Substring(0, l1_length + 4 * lo_length);
                    var l5_group = jobgroups.FirstOrDefault(q => q.FullCode2 == l5_code);

                    emp.L5Id = l5_group.Id;
                    emp.L5Code = l5_group.FullCode2;
                    emp.L5Title = l5_group.Title;
                }
            }

            var query_root = (from x in expiring_employees
                              group x by new { x.CertificateType } into grp
                              select new _expiring_group()
                              {
                                  CertificateType = grp.Key.CertificateType,
                                  IsExpired = grp.Sum(q => q.IsExpired),
                                  IsExpiring = grp.Sum(q => q.IsExpiring),
                                  GroupLevel = 0,
                              }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();

            var query_l1 = (from x in expiring_employees
                            group x by new { x.CertificateType, x.L1Code, x.L1Id, x.L1Title } into grp
                            select new _expiring_group()
                            {
                                CertificateType = grp.Key.CertificateType,
                                GroupCode = grp.Key.L1Code,
                                GroupTitle = grp.Key.L1Title,
                                GroupId = (int)grp.Key.L1Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 1,
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();
            var query_l2 = (from x in expiring_employees
                            group x by new { x.CertificateType, x.L2Code, x.L2Id, x.L2Title } into grp
                            select new _expiring_group()
                            {
                                CertificateType = grp.Key.CertificateType,
                                GroupCode = grp.Key.L2Code,
                                GroupTitle = grp.Key.L2Title,
                                GroupId = (int)grp.Key.L2Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 2,
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();


            List<_expiring_group> query_l3 = new List<_expiring_group>();
            List<_expiring_group> query_l4 = new List<_expiring_group>();
            List<_expiring_group> query_l5 = new List<_expiring_group>();

            if (expiring_employees.Where(q => q.JobGroupCode2.Length >= l1_length + 2 * lo_length).Count() > 0)
            {
                //l3
                query_l3 = (from x in expiring_employees
                            group x by new { x.CertificateType, x.L3Code, x.L3Id, x.L3Title } into grp
                            select new _expiring_group()
                            {
                                CertificateType = grp.Key.CertificateType,
                                GroupCode = grp.Key.L3Code,
                                GroupTitle = grp.Key.L3Title,
                                GroupId = (int)grp.Key.L3Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 3,
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();
            }

            if (expiring_employees.Where(q => q.JobGroupCode2.Length >= l1_length + 3 * lo_length).Count() > 0)
            {
                //l4
                query_l4 = (from x in expiring_employees
                            group x by new { x.CertificateType, x.L4Code, x.L4Id, x.L4Title } into grp
                            select new _expiring_group()
                            {
                                CertificateType = grp.Key.CertificateType,
                                GroupCode = grp.Key.L4Code,
                                GroupTitle = grp.Key.L4Title,
                                GroupId = (int)grp.Key.L4Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 4,
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();
            }

            if (expiring_employees.Where(q => q.JobGroupCode2.Length >= l1_length + 4 * lo_length).Count() > 0)
            {
                //l5
                query_l5 = (from x in expiring_employees
                            group x by new { x.CertificateType, x.L5Code, x.L5Id, x.L5Title } into grp
                            select new _expiring_group()
                            {
                                CertificateType = grp.Key.CertificateType,
                                GroupCode = grp.Key.L5Code,
                                GroupTitle = grp.Key.L5Title,
                                GroupId = (int)grp.Key.L5Id,
                                IsExpired = grp.Sum(q => q.IsExpired),
                                IsExpiring = grp.Sum(q => q.IsExpiring),
                                GroupLevel = 5,
                            }).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.GroupTitle).ToList();
            }
            foreach (var grp in query_root)
            {
                grp.Children = query_l1.Where(q => q.CertificateType == grp.CertificateType).ToList();
            }
            foreach (var grp in query_l1)
            {
                grp.Children = query_l2.Where(q => q.CertificateType == grp.CertificateType && q.GroupCode.StartsWith(grp.GroupCode)).ToList();
            }
            foreach (var grp in query_l2)
            {
                grp.Children = query_l3.Where(q => q.CertificateType == grp.CertificateType && q.GroupCode.StartsWith(grp.GroupCode)).ToList();
            }
            foreach (var grp in query_l3)
            {
                grp.Children = query_l4.Where(q => q.CertificateType == grp.CertificateType && q.GroupCode.StartsWith(grp.GroupCode)).ToList();
            }

            foreach (var grp in query_l4)
            {
                grp.Children = query_l5.Where(q => q.CertificateType == grp.CertificateType && q.GroupCode.StartsWith(grp.GroupCode)).ToList();
            }

            var result = new
            {
                employees = expiring_employees,
                groups = query_root,
            };

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetMonitoringExpiringMain()
        {
            var query = from x in context.ViewCertificateHistoryRankeds
                        where x.InActive == false && x.IsCritical == 1
                        group x by new { x.JobGroupMain, x.JobGroupMainCode, x.JobGroupMainId } into grp
                        select new
                        {
                            Group = grp.Key.JobGroupMain,
                            GroupId = grp.Key.JobGroupMainId,
                            GroupCode = grp.Key.JobGroupMainCode,
                            Count = grp.Count(),
                            IsExpiring = grp.Sum(q => q.IsExpiring),
                            IsExpired = grp.Sum(q => q.IsExpired),
                            Employees = grp.OrderBy(q => q.JobGroup).ThenBy(q => q.CertificateType).ToList()

                        };
            var result = (await query.ToListAsync()).OrderBy(q => q.IsExpired).ThenBy(q => q.IsExpiring).ThenBy(q => q.Group).ToList();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public class _JobGroupKey
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Code { get; set; }
        }
        public async Task<DataResponse> GetMonitoringExpiringMainByParent(int parentid)
        {
            var parentgoup = await context.JobGroups.Where(q => q.Id == parentid).FirstOrDefaultAsync();

            var jobgroups = (await context.JobGroups.Where(q => q.ParentId == parentid).ToListAsync()).OrderBy(q => q.FullCode2).ToList();
            var fullcodes = jobgroups.Select(q => q.FullCode2).ToList();
            var query = from x in context.ViewCertificateHistoryRankeds
                        where x.InActive == false && x.IsCritical == 1 && x.JobGroupCode2.StartsWith(parentgoup.FullCode2)
                        group x by new { x.JobGroupCode2, x.JobGroup, x.JobGroupMain, x.JobGroupId } into grp
                        select new
                        {
                            MainGroup = grp.Key.JobGroupMain,
                            Group = grp.Key.JobGroup,
                            GroupCode = grp.Key.JobGroupCode2,
                            GroupId = grp.Key.JobGroupId,
                            Count = grp.Count(),
                            IsExpiring = grp.Sum(q => q.IsExpiring),
                            IsExpired = grp.Sum(q => q.IsExpired),
                            Employees = grp.OrderBy(q => q.JobGroup).ThenBy(q => q.CertificateType).ToList()

                        };
            var query_result = await query.ToListAsync();
            var result = new List<Object>(); //new Dictionary<string, List<object>>();
            foreach (var jg in jobgroups)
            {
                var items = query_result.Where(q => q.GroupCode.StartsWith(jg.FullCode2)).OrderBy(q => q.GroupCode).ToList();
                if (items.Count > 0)
                {
                    //result.Add(jg.Id+"_"+jg.Id+"_"+jg.Title, items.Select(q=>(object)q).ToList());
                    result.Add(
                        new
                        {
                            jg.Id,
                            Code = jg.Id,
                            jg.Title,
                            Items = items
                        }
                    );
                }
            }



            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetTrainingExpiredCertificateTypes(int year, int certificate_type_id, int mng_id)
        {
            //var expiring_employees = await context.ViewCertificateHistoryRankeds.Where(q => /*q.InActive == false &&*/  q.ExpireYear == year && q.ExpireMonth == month).ToListAsync();
            Manager mng = null;
            List<string> mng_grps = null;
            List<int> filter_employee_ids = null;
            if (mng_id != -1)
            {
                mng = await context.Managers.FirstOrDefaultAsync(q => q.EmployeeId == mng_id);
                mng_grps = await context.ManagerGroups.Where(q => q.ManagerId == mng_id).Select(q => q.ProfileGroup).ToListAsync();


                filter_employee_ids = await context.ViewProfiles.Where(q => mng_grps.Contains(q.JobGroupRoot)).Select(q => q.PersonId).ToListAsync();
            }

            var query = from x in context.ViewCertificateHistoryRankeds
                        where x.ExpireYear == year
                        select x;
            if (filter_employee_ids != null)
                query = query.Where(q => filter_employee_ids.Contains(q.PersonId));
            if (certificate_type_id != -1)
                query = query.Where(q => q.CertificateTypeId == certificate_type_id);
            var ds = await query.ToListAsync();
            var result = (from x in ds
                          group x by new { Year = x.ExpireYear, Month = x.ExpireMonth, TypeId = x.CertificateTypeId, Type = x.CertificateType } into grp
                          select new
                          {
                              grp.Key.Year,
                              grp.Key.Month,
                              grp.Key.Type,
                              grp.Key.TypeId,

                              Count = grp.Count(),
                              Items = grp.OrderBy(q => q.ExpireMonth).ThenBy(q => q.CertificateType).ToList(),
                              GroupedItems = (
                                   from z in grp
                                   group z by new { z.JobGroup } into grpz
                                   select new
                                   {
                                       grp.Key.Type,
                                       grp.Key.Year,
                                       grp.Key.Month,
                                       Items = grpz.OrderBy(q => q.JobGroup).ToList(),
                                       Count = grpz.Count(),
                                       Group = grpz.Key.JobGroup
                                   }
                                ).OrderBy(q => q.Count).ToList()
                          }).OrderBy(q => q.Month).ThenByDescending(q => q.Count).ThenBy(q => q.Type).ToList();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetTrainingExpiredCertificateTypes(int year, int month, int certificate_type_id, int mng_id)
        {
            //var expiring_employees = await context.ViewCertificateHistoryRankeds.Where(q => /*q.InActive == false &&*/  q.ExpireYear == year && q.ExpireMonth == month).ToListAsync();
            var query = from x in context.ViewCertificateHistoryRankeds
                        where x.ExpireYear == year && x.ExpireMonth == month
                        select x;

            if (mng_id != -1)
            {
                var mng = await context.Managers.FirstOrDefaultAsync(q => q.EmployeeId == mng_id);
                var mng_grps = await context.ManagerGroups.Where(q => q.ManagerId == mng_id).Select(q => q.ProfileGroup).ToListAsync();
                var filter_employee_ids = await context.ViewProfiles.Where(q => mng_grps.Contains(q.JobGroupRoot)).Select(q => q.PersonId).ToListAsync();
                query = query.Where(q => filter_employee_ids.Contains(q.PersonId));
            }


            if (certificate_type_id != -1)
                query = query.Where(q => q.CertificateTypeId == certificate_type_id);
            var ds = await query.ToListAsync();
            var result = (from x in ds
                          group x by new { Date = ((DateTime)x.DateExpire).Date, Year = x.ExpireYear, Month = x.ExpireMonth, TypeId = x.CertificateTypeId, Type = x.CertificateType } into grp
                          select new
                          {
                              grp.Key.Date,
                              grp.Key.Year,
                              grp.Key.Month,
                              grp.Key.Type,
                              grp.Key.TypeId,

                              Count = grp.Count(),
                              Items = grp.OrderBy(q => q.ExpireMonth).ThenBy(q => q.CertificateType).ToList(),
                              GroupedItems = (
                                   from z in grp
                                   group z by new { z.JobGroup } into grpz
                                   select new
                                   {
                                       grp.Key.Type,
                                       grp.Key.Year,
                                       grp.Key.Month,
                                       grp.Key.Date,
                                       Items = grpz.OrderBy(q => q.JobGroup).ToList(),
                                       Count = grpz.Count(),
                                       Group = grpz.Key.JobGroup
                                   }
                                ).OrderBy(q => q.Count).ToList()
                          }).OrderBy(q => q.Month).ThenByDescending(q => q.Count).ThenBy(q => q.Type).ToList();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetTrainingExpiredCertificateTypesByPerson(int year, int pid)
        {
            //var expiring_employees = await context.ViewCertificateHistoryRankeds.Where(q => /*q.InActive == false &&*/  q.ExpireYear == year && q.ExpireMonth == month).ToListAsync();
            var query = from x in context.ViewCertificateHistoryRankeds
                        where x.ExpireYear == year && x.PersonId == pid
                        select x;
            var ds = await query.ToListAsync();
            var result = (from x in ds
                          group x by new { Year = x.ExpireYear, Month = x.ExpireMonth } into grp
                          select new
                          {
                              grp.Key.Year,
                              grp.Key.Month,

                              Count = grp.Count(),
                              Items = grp.OrderBy(q => q.ExpireMonth).ThenBy(q => q.CertificateType).ToList(),

                          }).OrderBy(q => q.Month).ThenByDescending(q => q.Count).ToList();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetTrainingExpiredCertificateTypesByPerson(int year, int month, int pid)
        {
            //var expiring_employees = await context.ViewCertificateHistoryRankeds.Where(q => /*q.InActive == false &&*/  q.ExpireYear == year && q.ExpireMonth == month).ToListAsync();
            var query = from x in context.ViewCertificateHistoryRankeds
                        where x.ExpireYear == year && x.ExpireMonth == month && x.PersonId == pid
                        select x;
            var ds = await query.ToListAsync();
            var result = (from x in ds
                          group x by new { Date = ((DateTime)x.DateExpire).Date, Year = x.ExpireYear, Month = x.ExpireMonth } into grp
                          select new
                          {
                              grp.Key.Date,
                              grp.Key.Year,
                              grp.Key.Month,

                              Count = grp.Count(),
                              Items = grp.OrderBy(q => q.ExpireMonth).ThenBy(q => q.CertificateType).ToList(),

                          }).OrderBy(q => q.Month).ThenByDescending(q => q.Count).ToList();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        //2024-01-09
        public async Task<DataResponse> GetProfilesAbs(string grp)
        {


            var query = context.ViewProfiles.Where(q => q.InActive != true);

            grp = grp.Replace('x', '/');
            if (grp != "-1")
                query = query.Where(q => q.JobGroupRoot == grp || q.PostRoot == grp);
            var profiles = await query.Select(q => new
            {
                q.Id,
                q.JobGroup,
                q.JobGroupRoot,
                q.NID,
                q.Mobile,
                q.FirstName,
                q.LastName,
                q.Name,
                q.PersonId,
                q.PID
            }).ToListAsync();

            return new DataResponse()
            {
                Data = profiles,
                IsSuccess = true,
            };

        }


        public async Task<DataResponse> GetTrainingSchedule(int year, int month, int mng_id)
        {
            Manager mng = null;
            List<string> mng_grps = null;
            List<int> filter_course_ids = null;
            List<int> filter_employee_ids = null;
            if (mng_id != -1)
            {
                mng = await context.Managers.FirstOrDefaultAsync(q => q.EmployeeId == mng_id);
                mng_grps = await context.ManagerGroups.Where(q => q.ManagerId == mng_id).Select(q => q.ProfileGroup).ToListAsync();

                var course_people_query = from x in context.ViewCoursePeoples
                                          where mng_grps.Contains(x.JobGroupRoot) && x.Year == year
                                          select x;

                filter_course_ids = await course_people_query.Select(q => q.CourseId).Distinct().ToListAsync();
                filter_employee_ids = await context.ViewProfiles.Where(q => mng_grps.Contains(q.JobGroupRoot)).Select(q => q.PersonId).ToListAsync();
            }

            var session_query = from x in context.ViewCourseSessions
                                where x.Year == year && x.Month == month && x.ParentId == null
                                select x;
            if (mng_id != -1)
                session_query = session_query.Where(q => filter_course_ids.Contains(q.CourseId));

            var session_query_list = await (session_query).ToListAsync();
            var sessions = (from x in session_query_list
                            group x by new { ((DateTime)x.DateStart).Date } into grp
                            let _ds = grp.Select(q => new
                            {
                                q.CourseId,
                                q.DateStart,
                                q.DateEnd,
                                q.Instructor,
                                q.No,
                                q.Organization,
                                q.Title,
                                q.Location,
                                q.Status,
                                q.StatusId
                            })
                            select new
                            {
                                Date = grp.Key.Date,
                                Items = _ds.OrderBy(w => w.DateStart).ThenBy(w => w.Title).ToList(),
                                courses = (from y in _ds
                                           group y by new { y.CourseId, y.Title, y.Instructor, y.Status, y.No } into grp2
                                           select new
                                           {
                                               grp2.Key.Title,
                                               grp2.Key.CourseId,
                                               grp2.Key.Instructor,
                                               grp2.Key.Status,
                                               grp2.Key.No,
                                               DateStart = _ds.Min(q => q.DateStart),
                                               DateEnd = _ds.Max(q => q.DateEnd)
                                           }).OrderBy(q => q.DateStart).ThenBy(q => q.Title).ToList()

                            }).OrderBy(q => q.Date).ToList();
            //var courses=  from y in session_query_list
            //              group y by new { y.CourseId, y.Title, y.Instructor, y.Status, y.No } into grp2
            //                       select new
            //                       {
            //                           grp2.Key.Title,
            //                           grp2.Key.CourseId,
            //                           grp2.Key.Instructor,
            //                           grp2.Key.Status,
            //                           grp2.Key.No,
            //                           DateStart = grp2.Where(q => q.Key.)
            //                       }


            var expiring_employees_query = from q in context.ViewCertificateHistoryRankeds
                                           where q.ExpireYear == year && q.ExpireMonth == month
                                           select q;
            if (mng_id != -1)
                expiring_employees_query = expiring_employees_query.Where(q => filter_employee_ids.Contains(q.PersonId));

            var expiring_employees = await expiring_employees_query.ToListAsync();
            //kosu
            var dates = getCalendarExpired(expiring_employees, null);


            var result = new
            {
                sessions,
                expired = dates
            };
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };

        }


        public async Task<DataResponse> GetTrainingSchedule(int year, int mng_id)
        {
            List<ViewCoursePeople> course_people = new List<ViewCoursePeople>();
            Manager mng = null;
            List<string> mng_grps = null;
            List<int> filter_course_ids = null;
            List<int> filter_employee_ids = null;
            if (mng_id != -1)
            {
                mng = await context.Managers.FirstOrDefaultAsync(q => q.EmployeeId == mng_id);
                mng_grps = await context.ManagerGroups.Where(q => q.ManagerId == mng_id).Select(q => q.ProfileGroup).ToListAsync();

                var course_people_query = from x in context.ViewCoursePeoples
                                          where mng_grps.Contains(x.JobGroupRoot) && x.Year == year
                                          select x;
                course_people = await course_people_query.ToListAsync();
                filter_course_ids = course_people.Select(q => q.CourseId).Distinct().ToList();
                filter_employee_ids = await context.ViewProfiles.Where(q => mng_grps.Contains(q.JobGroupRoot)).Select(q => q.PersonId).ToListAsync();
            }

            var course_query = (from x in context.ViewCourseNews
                                where x.Year == year
                                select x);
            if (filter_course_ids != null)
                course_query = course_query.Where(q => filter_course_ids.Contains(q.Id));

            var course_query_list = await course_query.ToListAsync();

            var courses = (from x in course_query_list
                           group x by new { x.Year, ((DateTime)x.DateStart).Month, x.CourseType } into grp
                           select new
                           {
                               Year = grp.Key.Year,
                               grp.Key.Month,
                               Type = grp.Key.CourseType,
                               Scheduled = grp.Where(q => q.StatusId == 1).Count(),
                               InProgress = grp.Where(q => q.StatusId == 2).Count(),
                               Done = grp.Where(q => q.StatusId == 3).Count(),
                               Canceled = grp.Where(q => q.StatusId == 4).Count(),

                               Items = grp.Select(q => new
                               {
                                   q.Id,
                                   q.DateStart,
                                   q.DateEnd,
                                   q.Instructor,
                                   q.No,
                                   q.Organization,
                                   q.Title,
                                   q.Location,
                                   q.Status,
                                   q.StatusId
                               }).OrderBy(w => w.DateStart).ThenBy(w => w.Title).ToList()
                           }).OrderBy(q => q.Month).ToList();

            var expiring_employees_query = from x in context.ViewCertificateHistoryRankeds
                                           where x.ExpireYear == year
                                           select x;
            if (filter_employee_ids != null)
                expiring_employees_query = expiring_employees_query.Where(q => filter_employee_ids.Contains(q.PersonId));

            var expiring_employees = await expiring_employees_query.ToListAsync();

            var dates = getCalendarExpired(expiring_employees, null);


            var result = new
            {
                courses,
                expired = dates
            };
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };

        }



        public async Task<DataResponse> GetRptCourseType(DateTime df, DateTime dt)
        {
            df = df.Date;
            dt = dt.Date.AddDays(1).Date;

            var courses = await (from x in context.ViewCourseNews

                                 where x.DateStart >= df && x.DateStart < dt
                                 group x by new { x.CourseType } into grp
                                 select new
                                 {

                                     Type = grp.Key.CourseType,
                                     Scheduled = grp.Where(q => q.StatusId == 1).Count(),
                                     InProgress = grp.Where(q => q.StatusId == 2).Count(),
                                     Done = grp.Where(q => q.StatusId == 3).Count(),
                                     Canceled = grp.Where(q => q.StatusId == 4).Count(),
                                     Duration = grp.Where(q => q.StatusId != 4).Sum(q => q.Duration),


                                     Items = grp.Select(q => new
                                     {
                                         q.Id,
                                         q.DateStart,
                                         q.DateEnd,
                                         q.Instructor,
                                         q.No,
                                         q.Organization,
                                         q.Title,
                                         q.Location,
                                         q.Status,
                                         q.StatusId
                                     }).OrderBy(w => w.DateStart).ThenBy(w => w.Title).ToList()
                                 }).ToListAsync();



            var result = new
            {
                courses,

            };
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };

        }

        //2023-07-31
        public async Task<DataResponse> GetRptCourseJobGroup(DateTime df, DateTime dt, int ct, string jg)
        {
            if (jg == "-1")
                jg = "";
            df = df.Date;
            dt = dt.Date.AddDays(1).Date;


            int n = 2;
            if (jg == "")
                n = 3;

            var query_people = from x in context.ViewCoursePeoples
                               where x.JobGroupCode2.StartsWith(jg) && x.StatusId != 4 && x.DateStart >= df && x.DateStart < dt
                               group x by new { Group = x.JobGroupCode2.Substring(0, jg.Length + n), x.CourseType, x.CourseTypeId } into grp
                               join y in context.ViewJobGroups on grp.Key.Group equals y.FullCode2
                               select new
                               {
                                   grp.Key.Group,
                                   GroupTitle = y.Title,
                                   grp.Key.CourseType,
                                   grp.Key.CourseTypeId,
                                   Duration = grp.Where(q => q.StatusId != 4).Sum(q => q.Duration),
                                   Passed = grp.Where(q => q.CoursePeopleStatusId == 1).Count(),
                                   Failed = grp.Where(q => q.CoursePeopleStatusId == 0).Count(),
                                   Unknown = grp.Where(q => q.CoursePeopleStatusId == -1).Count(),
                                   Count = grp.Count(),
                                   Items = grp.Select(q => new
                                   {
                                       q.JobGroup,
                                       q.FirstName,
                                       q.LastName,
                                       q.NID,
                                       q.Mobile,
                                       q.CoursePeopleStatus,
                                       q.PersonId,
                                       q.EmployeeId,
                                       q.CourseId,
                                       q.DateIssue,
                                       q.DateExpire,
                                       q.ImgUrl,
                                   })

                               };
            var people = await query_people.OrderBy(q => q.Group).ToListAsync();
            var summary = (from x in people
                           group x by new { x.Group, x.GroupTitle } into grp
                           select new
                           {
                               grp.Key.Group,
                               grp.Key.GroupTitle,
                               Duration = grp.Sum(q => q.Duration),
                               Count = grp.Sum(q => q.Count),
                           }).OrderByDescending(q => q.Count).ToList();





            var result = new
            {
                people,
                summary

            };
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };



        }

        public async Task<DataResponse> GetRptCoursePerson(DateTime df, DateTime dt, int pid)
        {
            df = df.Date;
            dt = dt.Date.AddDays(1).Date;
            var query_people = from x in context.ViewCoursePeoples
                               where x.PersonId == pid && x.StatusId == 3 && x.DateStart >= df && x.DateStart < dt
                               group x by new { x.CourseType, x.CourseTypeId } into grp

                               select new
                               {


                                   grp.Key.CourseType,
                                   grp.Key.CourseTypeId,
                                   Duration = grp.Sum(q => q.Duration),
                                   Passed = grp.Where(q => q.CoursePeopleStatusId == 1).Count(),
                                   Failed = grp.Where(q => q.CoursePeopleStatusId == 0).Count(),
                                   Unknown = grp.Where(q => q.CoursePeopleStatusId == -1).Count(),
                                   Count = grp.Count(),
                                   Items = grp.Select(q => new
                                   {
                                       q.JobGroup,
                                       q.FirstName,
                                       q.LastName,
                                       q.NID,
                                       q.Mobile,
                                       q.CoursePeopleStatus,
                                       q.PersonId,
                                       q.EmployeeId,
                                       q.CourseId,
                                       q.DateIssue,
                                       q.DateExpire,
                                       q.ImgUrl,
                                       q.Title
                                   })

                               };
            var people = await query_people.OrderBy(q => q.CourseType).ToListAsync();




            var result = new
            {
                people,

            };
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetRptSummary(DateTime df, DateTime dt)
        {
            df = df.Date;
            dt = dt.Date.AddDays(1).Date;
            var query_passed = from x in context.ViewCoursePeoples
                               where x.DateStart >= df && x.DateStart < dt && x.StatusId != 4
                               group x by new { x.CoursePeopleStatus } into grp
                               select new
                               {
                                   Status = grp.Key.CoursePeopleStatus,
                                   Count = grp.Count(),
                               };
            var passed = await query_passed.OrderBy(q => q.Count).ToListAsync();

            var query_status = from x in context.ViewCourseNews
                               where x.DateStart >= df && x.DateStart < dt
                               group x by new { x.Status, x.StatusId } into grp
                               select new
                               {
                                   grp.Key.Status,
                                   grp.Key.StatusId,
                                   Count = grp.Count(),

                               };
            var status = await query_status.OrderBy(q => q.Count).ToListAsync();

            var query_type = from x in context.ViewCourseNews
                             where x.DateStart >= df && x.DateStart < dt && (x.StatusId != 4)
                             group x by new { x.CourseType, x.CourseTypeId } into grp
                             select new
                             {
                                 grp.Key.CourseType,
                                 grp.Key.CourseTypeId,
                                 Count = grp.Count(),
                                 Duration = grp.Sum(q => q.Duration)

                             };
            var type = await query_type.OrderByDescending(q => q.Count).Take(20).ToListAsync();


            var query_group = from x in context.ViewCoursePeoples
                              where x.DateStart >= df && x.DateStart < dt && x.StatusId != 4
                              //group x by new { Group = x.JobGroupCode2.Substring(0, 3) } into grp
                              //FLY
                              group x by new { Group = x.JobGroupCode2.Substring(0, 7) } into grp
                              join y in context.ViewJobGroups on grp.Key.Group equals y.FullCode2
                              select new
                              {
                                  grp.Key.Group,
                                  Title = y.Title,
                                  Count = grp.Count(),
                                  Duration = grp.Sum(q => q.Duration)
                              };
            var group = await query_group.OrderBy(q => q.Count).ToListAsync();




            var result = new
            {
                status,
                passed,
                type,
                group

            };
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourseTypeJobGroups(int cid)
        {
            var result = await context.ViewCourseTypeJobGroups.Where(q => q.CourseTypeId == cid).OrderBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourseTypeJobGroupsByGroup(int gid)
        {
            var result = await context.ViewCourseTypeJobGroups.Where(q => q.Id == gid).OrderBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCertificateTypes()
        {
            var result = await context.CertificateTypes.OrderBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetCoursesByType(int tid, int sid)
        {
            var query = context.ViewCourseNews.Where(q => q.CourseTypeId == tid);
            if (sid != -1)
                query = query.Where(q => q.StatusId == sid);
            var result = await query.OrderBy(q => q.CourseType).ThenByDescending(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCoursesByTypeOutSide(int tid, int sid)
        {
            var query = context.ViewCourseNews.Where(q => q.CourseTypeId == tid);
            if (sid != -1)
                query = query.Where(q => q.StatusId == sid && q.IsInside == false);
            var result = await query.OrderBy(q => q.CourseType).ThenByDescending(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetCourseTypeSubjects(int id)
        {
            //var query = from x in context.ViewEmployeeTrainings select x;
            //if (root != "000")
            //    query = query.Where(q => q.JobGroupMainCode == root);
            //var result = await query.OrderByDescending(q => q.MandatoryExpired).ThenBy(q => q.JobGroup).ThenBy(q => q.LastName).ToListAsync();
            var obj = await context.view_course_type_subject.Where(q => q.parent_id == id).ToListAsync();

            return new DataResponse()
            {
                Data = obj,
                IsSuccess = true,
            };

        }

        public async Task<DataResponse> GetPersonCourses(int pid)
        {
            var result = await context.ViewCoursePeopleRankeds.Where(q => q.PersonId == pid).OrderByDescending(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetPersonMandatoryCourses(int pid)
        {
            var result = await context.ViewMandatoryCourseEmployees.Where(q => q.Id == pid).OrderBy(q => q.CourseType).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetPersonMandatoryCoursesByType(int type, int group)
        {
            var result = await context.ViewMandatoryCourseEmployees.Where(q => q.CourseTypeId == type && q.GroupId == group).OrderBy(q => q.ValidStatus).ThenBy(q => q.Remains).ThenBy(q => q.Name).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        //2024-09-15
        public async Task<DataResponse> GetCertificateHistoryByTypeGroup(int type, int group)
        {
            //var result = await context.ViewMandatoryCourseEmployees.Where(q => q.CourseTypeId == type && q.GroupId == group).OrderBy(q => q.ValidStatus).ThenBy(q => q.Remains).ThenBy(q => q.Name).ToListAsync();
            var result = await context.ViewCertificateHistories.Where(q => q.RankOrder == 1 && q.InActive == false && q.CertificateTypeId == type && q.JobGroupId == group)
                .OrderBy(q => q.Remain).ThenBy(q => q.LastName).ToListAsync();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCertificateHistoryByType(int type)
        {
            //var result = await context.ViewMandatoryCourseEmployees.Where(q => q.CourseTypeId == type && q.GroupId == group).OrderBy(q => q.ValidStatus).ThenBy(q => q.Remains).ThenBy(q => q.Name).ToListAsync();
            var course_type = await context.CourseTypes.FirstOrDefaultAsync(q => q.Id == type);
            if (course_type == null || course_type.CertificateTypeId == null)
                return new DataResponse()
                {
                    Data = null,
                    IsSuccess = true,
                };
            var result = await context.ViewCertificateHistories.Where(q => q.RankOrder == 1 && q.InActive == false && q.CertificateTypeId == course_type.CertificateTypeId)
                .OrderBy(q => q.Remain).ThenBy(q => q.LastName).ToListAsync();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public class expiring_grp
        {
            public string title { get; set; }
            public int? id { get; set; }
            public int expired { get; set; }
            public int expiring { get; set; }
        }
        public async Task<DataResponse> GetCertificateHistory_Expiring_People(string type)
        {
            var query = from x in context.view_trn_expiring
                        where x.CertificateType == type
                           && x.Remain <= 45
                        orderby x.Remain
                        select x;
            var result = await query.ToListAsync();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };

        }
        public async Task<DataResponse> GetStat()
        {
            var query = await (from x in context.C_view_trncard
                               where x.Remain <= 30 && x.Remain != -10000
                               select x).ToListAsync();
            var query_grp_type = (from x in query
                                  group x by new { x.title, x.trncard_title/*, is_expired = Math.Sign(Convert.ToInt32(x.Remain))*/ } into grp
                                  select new
                                  {
                                      grp.Key.title,
                                      title_card = grp.Key.trncard_title,
                                      //grp.Key.is_expired,
                                      rows = grp.ToList(),
                                      count = grp.Count(),
                                      has_expired = grp.Where(q => q.Remain <= 0).Any(),
                                      is_expired = grp.Where(q => q.Remain <= 0).Count(),
                                      is_expiring = grp.Where(q => q.Remain > 0).Count(),
                                      items = from q in grp
                                              group q by new { q.jobgroup_root, q.jobgroup } into grp2
                                              select new
                                              {
                                                  grp.Key.title,
                                                  title_card = grp.Key.trncard_title,
                                                  jobgroup_root = grp2.Key.jobgroup_root,
                                                  jobgroup = grp2.Key.jobgroup,
                                                  count = grp2.Count(),
                                                  rows = grp.ToList(),
                                                  is_expired = grp2.Where(q => q.Remain <= 0).Count(),
                                                  is_expiring = grp2.Where(q => q.Remain > 0).Count(),
                                                  items = from w in grp2
                                                          group w by new { caption = Math.Sign(Convert.ToInt32(w.Remain)) < 0 ? "Expired" : "Expiring" } into grp3
                                                          select new
                                                          {
                                                              grp.Key.title,
                                                              title_card = grp.Key.trncard_title,
                                                              jobgroup_root = grp2.Key.jobgroup_root,
                                                              jobgroup = grp2.Key.jobgroup,
                                                              grp3.Key.caption,
                                                              is_expired = grp3.Where(q => q.Remain <= 0).Count(),
                                                              is_expiring = grp3.Where(q => q.Remain > 0).Count(),
                                                              count = grp3.Count(),
                                                              items = grp3.ToList(),
                                                          }
                                              }
                                  }).ToList();

            var query_grp_people = (from x in query
                                    group x by new { x.name, x.nid, x.person_id, x.profile_id, x.jobgroup, x.jobgroup_root } into grp
                                    select new
                                    {
                                        grp.Key.person_id,
                                        grp.Key.profile_id,
                                        grp.Key.name,
                                        grp.Key.nid,
                                        grp.Key.jobgroup_root,
                                        grp.Key.jobgroup,
                                        items = grp.ToList(),
                                    }).ToList();

            var query_grp_group = (from x in query
                                   group x by new { x.jobgroup, x.jobgroup_root } into grp
                                   select new
                                   {
                                       //grp.Key.person_id,
                                       // grp.Key.profile_id,
                                       //grp.Key.name,
                                       //grp.Key.nid,
                                       grp.Key.jobgroup_root,
                                       grp.Key.jobgroup,
                                       rows = grp.ToList(),
                                       items = from q in grp
                                               group q by new { q.name, q.nid, q.person_id, q.profile_id, } into grp2
                                               select new
                                               {
                                                   grp2.Key.person_id,
                                                   grp2.Key.profile_id,
                                                   grp2.Key.name,
                                                   grp2.Key.nid,
                                                   items = grp2.ToList(),
                                               }
                                   }).ToList();

            return new DataResponse()
            {
                Data = new
                {
                    query,
                    query_grp_group,
                    query_grp_people,
                    query_grp_type
                },
                IsSuccess = true,
            };

        }
        //2025-01-11
        public async Task<DataResponse> GetCertificateHistory_Expiring()
        {

            var query_expiring = await (from x in context.view_trn_expiring
                                        where x.Remain > 0 && x.Remain <= 45 /*&& x.InActive == false && x.RankOrder == 1*/
                                        group x by new { x.CertificateType } into grp
                                        select new
                                        {

                                            grp.Key.CertificateType,
                                            cnt = grp.Count(),
                                        }).ToListAsync();
            var query_expired = await (from x in context.view_trn_expiring
                                       where x.Remain <= 0
                                       group x by new { x.CertificateType } into grp
                                       select new
                                       {

                                           grp.Key.CertificateType,
                                           cnt = grp.Count(),
                                       }).ToListAsync();
            var cts = query_expired.Select(q => q.CertificateType).Concat(query_expiring.Select(q => q.CertificateType)).Distinct().ToList();
            var result = new List<expiring_grp>();
            foreach (var x in cts)
            {
                var _expired = query_expired.Where(q => q.CertificateType == x).FirstOrDefault();
                var _expiring = query_expiring.Where(q => q.CertificateType == x).FirstOrDefault();
                result.Add(new expiring_grp()
                {
                    id = -1,
                    title = _expired == null ? _expiring.CertificateType : _expired.CertificateType,
                    expired = _expired == null ? 0 : _expired.cnt,
                    expiring = _expiring == null ? 0 : _expiring.cnt
                });
            }


            //var result = await context.ViewMandatoryCourseEmployees.Where(q => q.CourseTypeId == type && q.GroupId == group).OrderBy(q => q.ValidStatus).ThenBy(q => q.Remains).ThenBy(q => q.Name).ToListAsync();
            //var course_type = await context.CourseTypes.FirstOrDefaultAsync(q => q.Id == type);
            //if (course_type == null || course_type.CertificateTypeId == null)
            //    return new DataResponse()
            //    {
            //        Data = null,
            //        IsSuccess = true,
            //    };
            //var result = await context.ViewCertificateHistories.Where(q => q.RankOrder == 1 && q.InActive == false && q.CertificateTypeId == course_type.CertificateTypeId)
            //    .OrderBy(q => q.Remain).ThenBy(q => q.LastName).ToListAsync();
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCoursePeople(int cid)
        {
            var result = await context.ViewCoursePeoples.OrderBy(q => q.CourseId == cid).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCoursePeopleNames(int cid)
        {
            var result = await context.ViewCoursePeoples.Where(q => q.CourseId == cid).Select(q => new
            {
                 q.Name,
                 q.FirstName,
                 q.LastName,
                 q.JobGroup
            }).OrderBy(q=>q.JobGroup).ThenBy(q=>q.Name).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourseTypeNotApplicable(int ctid)
        {
            var result = await context.CourseTypeApplicables.Where(q => q.IsApplicable == false && q.CourseTypeId == ctid).Select(q => q.TrainingGroup).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourseView(int cid)
        {
            var result = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();


            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public class dto_trn_exam
        {
            public int id { get; set; }
            public int course_id { get; set; }
            public DateTime? exam_date { get; set; }
            public string exam_date_persian { get; set; }
            public string location_title { get; set; }
            public string location_address { get; set; }
            public string location_phone { get; set; }
            public string remark { get; set; }
            public int? status_id { get; set; }
            public int? created_by { get; set; }
            public int? confirmed_by { get; set; }
            public DateTime? created_date { get; set; }
            public DateTime? confirmed_date { get; set; }
            public int? exam_type_id { get; set; }
            public DateTime? signed_by_ins1_date { get; set; }
            public DateTime? signed_by_ins2_date { get; set; }
            public DateTime? signed_by_director_date { get; set; }
            public DateTime? signed_by_staff_date { get; set; }
            public int? duration { get; set; }
            public DateTime? date_start { get; set; }
            public DateTime? date_end_scheduled { get; set; }
            public DateTime? date_end_actual { get; set; }
            public DateTime? date_start_scheduled { get; set; }
            public bool? is_negetive_point { get; set; }

        }
        //public async Task<DataResponse> GetCourseViewObjectApp(int cid)
        //{
        //    var course = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();
        //    var subjects = await context.ViewCourseNews.Where(q => q.ParentId == cid).ToListAsync();
        //}
        public async Task<DataResponse> GetCourseViewObject(int cid)
        {
            var course = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();
            var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();


            //var syllabi = await context.ViewSyllabus.Where(q => q.CourseId == cid).ToListAsync();
            var syllabi = await context.ViewCourseNewAlls.Where(q => q.ParentId == cid).ToListAsync();
            var syllabi_ids = syllabi.Select(q => q.Id).ToList();
            var syllabi_session = await context.CourseSessions.Where(q => syllabi_ids.Contains(q.CourseId)).ToListAsync();

            List<ViewSyllabiDto> subjects = new List<ViewSyllabiDto>();
            foreach (var x in syllabi)
            {
                var sbj = new ViewSyllabiDto()
                {
                    Id = x.Id,
                    CertificateType = x.CertificateType,
                    CertificateTypeId = x.CertificateTypeId,
                    CourseTypeId = x.CourseTypeId,
                    CurrencyId = x.CurrencyId,
                    DateEnd = x.DateEnd,
                    DateStart = x.DateStart,
                    Date_Exam_Sign_Ins1 = x.Date_Exam_Sign_Ins1,
                    Date_Exam_Sign_Ins2 = x.Date_Exam_Sign_Ins2,
                    Date_Sessions_Instructor_Synced = x.Date_Sessions_Instructor_Synced,
                    Date_Sessions_Synced = x.Date_Sessions_Synced,
                    Date_Sign_Director = x.Date_Sign_Director,
                    Date_Sign_Ins1 = x.Date_Sign_Ins1,
                    Date_Sign_Ins2 = x.Date_Sign_Ins2,
                    Date_Sign_OPS = x.Date_Sign_OPS,
                    Date_Sign_Staff = x.Date_Sign_Staff,
                    Duration = x.Duration,
                    DurationUnitId = x.DurationUnitId,
                    Instructor = x.Instructor,
                    Instructor2 = x.Instructor,
                    Instructor2Id = x.Instructor2Id,
                    ParentId = x.ParentId,
                    ParentTitle = x.ParentTitle,
                    PLCount = x.PLCount,
                    PLRemain = x.PLRemain,
                    PLStatus = x.PLStatus,
                    PLTotal = x.PLTotal,
                    Remark = x.Remark,
                    SesstionsCount = x.SesstionsCount,
                    Status = x.Status,
                    StatusId = x.StatusId,
                    Synced = x.Synced,
                    SyncedErrors = x.SyncedErrors,
                    Title = x.Title,

                };
                sbj.Sessions = syllabi_session.Where(q => q.CourseId == sbj.Id).OrderBy(q => q.DateStart).ToList();
                subjects.Add(sbj);
            }



            //var exams =await context.trn_exam.Where(q=>q.course_id==cid).ToListAsync();
            var exams = await (from e in context.trn_exam
                               where e.course_id == cid
                               select new dto_trn_exam()
                               {
                                   id = e.id,
                                   course_id = e.course_id,
                                   exam_date = e.exam_date,
                                   exam_date_persian = e.exam_date_persian,
                                   location_title = e.location_title,
                                   location_address = e.location_address,
                                   location_phone = e.location_phone,
                                   remark = e.remark,
                                   status_id = e.status_id,
                                   created_by = e.created_by,
                                   confirmed_by = e.confirmed_by,
                                   created_date = e.created_date,
                                   confirmed_date = e.confirmed_date,
                                   exam_type_id = e.exam_type_id,
                                   signed_by_ins1_date = e.signed_by_ins1_date,
                                   signed_by_ins2_date = e.signed_by_ins2_date,
                                   signed_by_director_date = e.signed_by_director_date,
                                   signed_by_staff_date = e.signed_by_staff_date,
                                   duration = e.duration,
                                   date_start = e.date_start,
                                   date_end_scheduled = e.date_end_scheduled,
                                   date_end_actual = e.date_end_actual,
                                   date_start_scheduled = e.date_start_scheduled,
                                   is_negetive_point = e.is_negetive_point

                               }).ToListAsync();

            var exam_ids = exams.Select(q => q.id).ToList();
            var groups = await context.trn_exam_group.Where(q => exam_ids.Contains(q.exam_id)).ToListAsync();
            var people = await context.trn_exam_person.Where(q => exam_ids.Contains(q.exam_id)).ToListAsync();
            var templates = await context.view_trn_exam_question_template.Where(q => exam_ids.Contains(q.exam_id)).ToListAsync();
            var _exams = new List<ExamViewModel>();
            foreach (var exam in exams)
            {
                var _exam = JsonConvert.DeserializeObject<ExamViewModel>(JsonConvert.SerializeObject(exam));
                _exam.groups = groups.Where(q => q.exam_id == exam.id).Select(q => q.group_id).ToList();
                _exam.people = people.Where(q => q.exam_id == exam.id).Select(q => q.person_id).ToList();
                _exam.template = templates.Where(q => q.exam_id == exam.id).ToList();
                _exams.Add(_exam);
            }

            return new DataResponse()
            {
                Data = new
                {
                    course,
                    sessions,
                    syllabi = subjects,
                    exams = _exams,

                },
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourse(int cid)
        {
            var result = await context.Courses.Where(q => q.Id == cid).FirstOrDefaultAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        //zamani
        public async Task<DataResponse> GetCertificatesHistory(int pid)
        {
            var result = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.RankLast == 1).OrderBy(q => q.DateExpire).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        //09-11


        public C_view_trncard get_max_sms(C_view_trncard l1, C_view_trncard l2, C_view_trncard l3)
        {
            if (l1 == null)
                l1 = new C_view_trncard() { Remain = -10000, trncard_title = "SMS-L1" };
            if (l2 == null)
                l2 = new C_view_trncard() { Remain = -10000, trncard_title = "SMS-L2" };
            if (l3 == null)
                l3 = new C_view_trncard() { Remain = -10000, trncard_title = "SMS-L3" };

            C_view_trncard sms = l3;
            if (l2.Remain > sms.Remain)
                sms = l2;
            if (l1.Remain > sms.Remain)
                sms = l1;

            if (sms.ExpireDate == null)
                return null;
            else return sms;



        }
        //2025-01-11
        public async Task<DataResponse> GetTrainingCard(int pid)
        {
            pid = (pid - 1237) / 2;
            var employee = await context.ViewEmployees.Where(q => q.PersonId == pid).FirstOrDefaultAsync();
            var profile = await context.ViewProfiles.Where(q => q.PersonId == pid).FirstOrDefaultAsync();
            var person = await context.People.Where(q => q.Id == pid).FirstOrDefaultAsync();

            var result = new List<ViewCoursePeoplePassedRanked>();
            var trn_ds = await context.C_view_trncard.Where(q => q.person_id == pid).OrderBy(q => q.trncard_title).ToListAsync();
            var trn_ds_no_sms_l2_l3 = trn_ds.Where(q => q.trncard_title != "SMS-L2" && q.trncard_title != "SMS-L3" && q.trncard_title != "SMS-L1" && q.trncard_title != "SMS").ToList();
            var sms_l1 = trn_ds.FirstOrDefault(q => q.trncard_title == "SMS-L1");
            if (sms_l1 == null)
                sms_l1 = trn_ds.FirstOrDefault(q => q.trncard_title == "SMS");
            var sms_l2 = trn_ds.FirstOrDefault(q => q.trncard_title == "SMS-L2");
            var sms_l3 = trn_ds.FirstOrDefault(q => q.trncard_title == "SMS-L3");
            foreach (var x in trn_ds_no_sms_l2_l3)
            {
                result.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = x.first_name,
                    LastName = x.last_name,
                    JobGroup = x.jobgroup,
                    JobGroupRoot = x.jobgroup_root,
                    NID = x.nid,
                    Title = x.trncard_title,
                    DateIssue = x.IssueDate,
                    DateExpire = x.ExpireDate,
                    Interval = x.interval_month,
                    ImageUrl = x.imageurl,

                    PersonId = x.person_id,
                    Remaining = x.Remain,
                });
            }

            if (employee.JobGroupRoot == "Cockpit" || employee.JobGroupRoot == "Cabin")
            {
                //  result = result.Where(q =>q.Remaining>=0 || ( q.Remaining < 0 && (q.Title == "SMS-L2" || q.Title == "SMS-L3"))).ToList();
                //  var sms = new List<string>() { "SMS-L2", "SMS-L3" };
                //   var sms_rows = result.Where(q => sms.IndexOf(q.Title) != -1 && (q.Remaining < 0)).Select(q => q.Title).ToList();
                //  result = result.Where(q => sms_rows.IndexOf(q.Title) == -1).ToList();
                result.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    JobGroup = employee.JobGroup,
                    JobGroupRoot = employee.JobGroupRoot,
                    NID = employee.NID,
                    Title = "SMS",
                    DateIssue = sms_l1 != null ? sms_l1.IssueDate : null,
                    DateExpire = sms_l1 != null ? sms_l1.ExpireDate : null,
                    Interval = sms_l1 != null ? sms_l1.interval_month : null,
                    ImageUrl = employee.ImageUrl,

                    PersonId = employee.PersonId,
                    Remaining = sms_l1 != null ? sms_l1.Remain : null,
                });

                if (profile.PostRoot == "MANAGEMENT")
                {
                    var _sms = get_max_sms(null, sms_l2, sms_l3);

                    if (_sms == null)
                        _sms = new C_view_trncard() { title = "SMS-L2", trncard_title = "SMS-L2", interval_month = 24 };

                    result.Add(new ViewCoursePeoplePassedRanked()
                    {
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        JobGroup = employee.JobGroup,
                        JobGroupRoot = employee.JobGroupRoot,
                        NID = employee.NID,
                        Title = _sms.trncard_title,
                        DateIssue = _sms.IssueDate != null ? _sms.IssueDate : null,
                        DateExpire = _sms.ExpireDate != null ? _sms.ExpireDate : null,
                        Interval = _sms.interval_month != null ? _sms.interval_month : null,
                        ImageUrl = employee.ImageUrl,

                        PersonId = employee.PersonId,
                        Remaining = _sms.Remain != null ? _sms.Remain : null,
                    });
                }

            }
            else
            if (employee.JobGroupRoot == "QA")
            {

                result.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    JobGroup = employee.JobGroup,
                    JobGroupRoot = employee.JobGroupRoot,
                    NID = employee.NID,
                    Title = "SMS-L3",
                    DateIssue = sms_l3 != null ? sms_l3.IssueDate : null,
                    DateExpire = sms_l3 != null ? sms_l3.ExpireDate : null,
                    Interval = sms_l3 != null ? sms_l3.interval_month : null,
                    ImageUrl = employee.ImageUrl,

                    PersonId = employee.PersonId,
                    Remaining = sms_l3 != null ? sms_l3.Remain : null,
                });



            }

            else
            {
                var _sms = get_max_sms(sms_l1, sms_l2, sms_l3);

                if (_sms == null)
                {
                    _sms = new C_view_trncard() { title = "SMS-L1", interval_month = 24, trncard_title = "SMS-L1" };
                    if (profile.PostRoot == "MANAGEMENT")
                        _sms = new C_view_trncard() { title = "SMS-L2", interval_month = 24, trncard_title = "SMS-L2" };
                }
                result.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    JobGroup = employee.JobGroup,
                    JobGroupRoot = employee.JobGroupRoot,
                    NID = employee.NID,
                    Title = _sms.trncard_title,
                    DateIssue = _sms.IssueDate != null ? _sms.IssueDate : null,
                    DateExpire = _sms.ExpireDate != null ? _sms.ExpireDate : null,
                    Interval = _sms.interval_month != null ? _sms.interval_month : null,
                    ImageUrl = employee.ImageUrl,

                    PersonId = employee.PersonId,
                    Remaining = _sms.Remain != null ? _sms.Remain : null,
                });
            }
            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };

        }
        public async Task<DataResponse> GetTrainingCard_OLD(int pid)
        {
            pid = (pid - 1237) / 2;
            var person = await context.People.Where(q => q.Id == pid).FirstOrDefaultAsync();
            var employee = await context.ViewEmployees.Where(q => q.PersonId == pid).FirstOrDefaultAsync();
            var result = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.RankLast == 1).OrderBy(q => q.DateExpire).ToListAsync();
            var trg02 = result.Where(q => q.CourseType == "AVSEC-TRG-02").FirstOrDefault();
            var ds = new List<ViewCoursePeoplePassedRanked>();
            if (employee.JobGroup == "TRE" || employee.JobGroup == "TRI" || employee.JobGroup == "LTC" || employee.JobGroup == "P1" || employee.JobGroup == "P2")
            {
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "LPC",
                    DateIssue = person.ProficiencyCheckDate,
                    DateExpire = person.ProficiencyValidUntil,
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,

                    PersonId = employee.PersonId,
                });
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "OPC",
                    DateIssue = person.ProficiencyCheckDateOPC,
                    DateExpire = person.ProficiencyValidUntilOPC,
                    Interval = 6,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "AVSEC-TRG-02",
                    DateIssue = trg02 != null ? trg02.DateIssue : null,
                    DateExpire = trg02 != null ? trg02.DateExpire : null,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });


                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "AVSEC-TRG-07B",
                    DateIssue = person.AviationSecurityIssueDate,
                    DateExpire = person.AviationSecurityExpireDate,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "SMS",
                    DateIssue = person.SMSIssueDate,
                    DateExpire = person.SMSExpireDate,
                    Interval = 24,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "SEPT-P",
                    DateIssue = person.SEPTPIssueDate,
                    DateExpire = person.SEPTPExpireDate,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "ESET",
                    DateIssue = person.SEPTIssueDate,
                    DateExpire = person.SEPTExpireDate,
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "DGR-Function 7",
                    DateIssue = person.DangerousGoodsIssueDate,
                    DateExpire = person.DangerousGoodsExpireDate,
                    Interval = 24,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "CRM",
                    DateIssue = person.UpsetRecoveryTrainingIssueDate,
                    DateExpire = person.UpsetRecoveryTrainingExpireDate,
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                //comment for fucking karun
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "CCRM",
                    DateIssue = person.CCRMIssueDate,
                    DateExpire = person.CCRMExpireDate,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "HOT-WX",
                    DateIssue = person.HotWeatherOperationIssueDate,
                    DateExpire = person.HotWeatherOperationExpireDate,
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "COLD-WX",
                    DateIssue = person.ColdWeatherOperationIssueDate,
                    DateExpire = person.ColdWeatherOperationExpireDate,
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "FMT",
                    DateIssue = person.EGPWSIssueDate,
                    DateExpire = person.EGPWSExpireDate,
                    Interval = 24,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });
                //comment for fucking karun
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "GRT",
                    DateIssue = person.DateCaoCardIssue,
                    DateExpire = person.DateCaoCardExpire,
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "LINE CHECK",
                    //DateIssue = person.DateIssueNDT,
                    //DateExpire = person.DateIssueNDT == null ? null : (Nullable<DateTime>)((DateTime)person.DateIssueNDT).AddYears(1),

                    DateIssue = person.LineIssueDate,
                    //  DateExpire = person.LineIssueDate == null ? null : (Nullable<DateTime>)((DateTime)person.LineIssueDate).AddYears(1),
                    DateExpire = person.LineExpireDate == null ? null : (Nullable<DateTime>)((DateTime)person.LineExpireDate),
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });
            }
            else
            {
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "AVSEC-TRG-02",
                    DateIssue = trg02 != null ? trg02.DateIssue : null,
                    DateExpire = trg02 != null ? trg02.DateExpire : null,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });


                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "AVSEC-TRG-07B",
                    DateIssue = person.AviationSecurityIssueDate,
                    DateExpire = person.AviationSecurityExpireDate,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "SMS",
                    DateIssue = person.SMSIssueDate,
                    DateExpire = person.SMSExpireDate,
                    Interval = 24,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "SEPT-P",
                    DateIssue = person.SEPTPIssueDate,
                    DateExpire = person.SEPTPExpireDate,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "DGR-Function 9",
                    DateIssue = person.DangerousGoodsIssueDate,
                    DateExpire = person.DangerousGoodsExpireDate,
                    Interval = 24,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "CRM",
                    DateIssue = person.UpsetRecoveryTrainingIssueDate,
                    DateExpire = person.UpsetRecoveryTrainingExpireDate,
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });


                //comment for fucking karun
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "CCRM",
                    DateIssue = person.CCRMIssueDate,
                    DateExpire = person.CCRMExpireDate,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "FIRST-AID",
                    DateIssue = person.FirstAidIssueDate,
                    DateExpire = person.FirstAidExpireDate,
                    Interval = 36,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "FMT",
                    DateIssue = person.EGPWSIssueDate,
                    DateExpire = person.EGPWSExpireDate,
                    Interval = 24,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "TYPE RECURRENT",
                    DateIssue = person.RecurrentIssueDate,
                    DateExpire = person.RecurrentIssueDate == null ? null : person.RecurrentExpireDate,//(Nullable<DateTime>)((DateTime)person.RecurrentIssueDate).AddYears(1),
                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });
                ds.Add(new ViewCoursePeoplePassedRanked()
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    JobGroup = employee.JobGroup,
                    NID = person.NID,
                    Title = "LINE CHECK",
                    //DateIssue = person.DateIssueNDT,
                    // DateExpire = person.DateIssueNDT == null ? null : (Nullable<DateTime>)((DateTime)person.DateIssueNDT).AddYears(1),
                    DateIssue = person.LineIssueDate,
                    DateExpire = person.LineIssueDate == null ? null : person.LineExpireDate, //(Nullable<DateTime>)((DateTime)person.LineIssueDate).AddYears(1),

                    Interval = 12,
                    ImageUrl = (result == null || result.Count == 0) ? "" : result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

            }






            return new DataResponse()
            {
                Data = ds,//result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCoursesPassedHistory(int pid)
        {
            var result = await context.ViewCoursePeopleRankeds.Where(q => q.PersonId == pid && q.CoursePeopleStatusId == 1).OrderBy(q => q.CertificateType).ThenBy(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetMainJobGroups()
        {
            var result = await context.ViewJobGroupMains.OrderBy(q => q.FullCode2).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetEmployees(string root)
        {
            var query = from x in context.ViewEmployeeTrainings where x.InActive == false select x;
            if (root != "000")
                query = query.Where(q => q.JobGroupMainCode == root);
            var result = await query.OrderByDescending(q => q.MandatoryExpired).ThenBy(q => q.JobGroup).ThenBy(q => q.LastName).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        //GetCertificateAll
        public async Task<DataResponse> GetCertificateAll()
        {
            //var query = from x in context.ViewEmployeeTrainings select x;
            //if (root != "000")
            //    query = query.Where(q => q.JobGroupMainCode == root);
            //var result = await query.OrderByDescending(q => q.MandatoryExpired).ThenBy(q => q.JobGroup).ThenBy(q => q.LastName).ToListAsync();
            var obj = context.ViewCoursePeoplePassedRankeds.Where(q => q.ImgUrl == "FLY KISH" /*&& (q.Instructor.Contains("TALEBI") || q.Instructor.Contains("SHAFIEI"))*/ ).Select(q =>

           new
           {
               Id = q.Id,
               Name = q.LastName + " " + q.FirstName,
               q.Title,
           }

            ).ToList();
            if (obj != null)
                return new DataResponse()
                {
                    Data = obj,
                    IsSuccess = true,
                };
            else
                return new DataResponse()
                {
                    Data = new ViewCoursePeople() { Id = -1, },
                    IsSuccess = true,
                };
        }

        public async Task<DataResponse> GetCertificate(int id)
        {
            //var query = from x in context.ViewEmployeeTrainings select x;
            //if (root != "000")
            //    query = query.Where(q => q.JobGroupMainCode == root);
            //var result = await query.OrderByDescending(q => q.MandatoryExpired).ThenBy(q => q.JobGroup).ThenBy(q => q.LastName).ToListAsync();
            var obj = context.ViewCoursePeoplePassedRankeds.FirstOrDefault(q => q.Id == id);
            if (obj != null)
                return new DataResponse()
                {
                    Data = obj,
                    IsSuccess = true,
                };
            else
                return new DataResponse()
                {
                    Data = new ViewCoursePeople() { Id = -1, },
                    IsSuccess = true,
                };
        }





        public async Task<DataResponse> DeleteCourseType(int id)
        {
            var view = await context.ViewCourseTypes.Where(q => q.Id == id).FirstOrDefaultAsync();
            if (view.CoursesCount > 0)
            {
                return new DataResponse()
                {
                    Data = null,
                    IsSuccess = false,
                    Errors = new List<string>() { "Please remove related course(s)." }
                };
            }
            var obj = await context.CourseTypes.Where(q => q.Id == id).FirstOrDefaultAsync();
            context.CourseTypes.Remove(obj);
            var saveResult = await context.SaveAsync();
            if (!saveResult.IsSuccess)
                return saveResult;

            return new DataResponse()
            {
                IsSuccess = true,
                Data = obj,
            };
        }

        public async Task<DataResponse> DeleteCourse(int id)
        {
            var view = await context.CoursePeoples.Where(q => q.CourseId == id).FirstOrDefaultAsync();
            if (view != null)
            {
                return new DataResponse()
                {
                    Data = null,
                    IsSuccess = false,
                    Errors = new List<string>() { "Please remove related People." }
                };
            }
            var obj = await context.Courses.Where(q => q.Id == id).FirstOrDefaultAsync();
            context.Courses.Remove(obj);
            var saveResult = await context.SaveAsync();
            if (!saveResult.IsSuccess)
                return saveResult;

            return new DataResponse()
            {
                IsSuccess = true,
                Data = obj,
            };
        }

        public async Task<DataResponse> DeleteCoursePeople(int pid, int cid)
        {

            var obj = await context.CoursePeoples.Where(q => q.CourseId == cid && q.PersonId == pid).FirstOrDefaultAsync();
            var employee = await context.ViewEmployeeAbs.Where(q => q.PersonId == pid).Select(q => q.Id).FirstOrDefaultAsync();
            context.CoursePeoples.Remove(obj);
            var sessionFdps = await context.CourseSessionFDPs.Where(q => q.CourseId == cid && q.EmployeeId == employee).Select(q => q.FDPId).ToListAsync();
            var fdps = await context.FDPs.Where(q => sessionFdps.Contains(q.Id)).ToListAsync();
            context.FDPs.RemoveRange(fdps);

            var saveResult = await context.SaveAsync();
            if (!saveResult.IsSuccess)
                return saveResult;

            return new DataResponse()
            {
                IsSuccess = true,
                Data = obj,
            };
        }

        public async Task<DataResponse> SaveCourseType(ViewModels.CourseTypeViewModel dto)
        {
            CourseType entity = null;
            var _t = dto.Title.Replace(" ", "").Replace("-", "").Replace("/", "").Trim().ToLower();
            var _exist = await context.CourseTypes.FirstOrDefaultAsync(q => q.Id != dto.Id && q.Title.Replace(" ", "").Replace("-", "").Replace("/", "").Trim().ToLower() == _t);
            if (_exist != null)
            {
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = false,
                    Errors = new List<string>() { "Duplicated Title Found" }
                };
            }

            if (dto.Id == -1)
            {
                entity = new CourseType();
                context.CourseTypes.Add(entity);
            }

            else
            {
                entity = await context.CourseTypes.FirstOrDefaultAsync(q => q.Id == dto.Id);

            }

            if (entity == null)
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = false,
                    Errors = new List<string>() { "entity not found" }
                };

            //ViewModels.Location.Fill(entity, dto);
            ViewModels.CourseTypeViewModel.Fill(entity, dto);

            if (dto.Id != -1)
            {
                var djgs = await context.CourseTypeJobGroups.Where(q => q.CourseTypeId == entity.Id).ToListAsync();
                context.CourseTypeJobGroups.RemoveRange(djgs);
            }

            var jgsIds = dto.JobGroups.Select(q => q.Id);
            var jgs = await context.JobGroups.Where(q => jgsIds.Contains(q.Id)).ToListAsync();
            foreach (var x in jgs)
            {
                entity.CourseTypeJobGroups.Add(new CourseTypeJobGroup()
                {
                    JobGroupId = x.Id,
                    GroupCode = x.FullCode,
                });
            }

            await context.SaveChangesAsync();
            dto.Id = entity.Id;

            await SaveCourseTypeNotApplicable(new course_type_notapplicable_viewmodel()
            {
                groups = dto.not_applicables,
                type_id = dto.Id
            });

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public class course_type_notapplicable_viewmodel
        {
            public int type_id { get; set; }
            public List<string> groups { get; set; }

        }

        public async Task<DataResponse> SaveCourseTypeNotApplicable(course_type_notapplicable_viewmodel dto)
        {
            var type_id = dto.type_id;
            var exist = context.CourseTypeApplicables.Where(q => q.CourseTypeId == type_id).ToList();
            context.CourseTypeApplicables.RemoveRange(exist);
            if (dto.groups.Count == 0)
            {
                await context.SaveChangesAsync();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = dto,
                };
            }

            foreach (var item in dto.groups)
            {
                context.CourseTypeApplicables.Add(new CourseTypeApplicable()
                {
                    CourseTypeId = type_id,
                    TrainingGroup = item,
                    IsApplicable = false
                });
            }

            // var not_applicables = dto.Where(q => q.applicable == false).ToList();

            var certificate_type = (from ct in context.CourseTypes
                                    join c in context.CertificateTypes on ct.CertificateTypeId equals c.Id
                                    where ct.Id == type_id
                                    select c).FirstOrDefault();
            var dto_groups = dto.groups;
            var profiles = context.ViewProfiles.Where(q => dto_groups.Contains(q.JobGroupRoot) || dto_groups.Contains(q.PostRoot)).ToList();
            var profile_ids = profiles.Select(q => q.PersonId).ToList();
            var last_history = context.ViewCertificateHistoryRankeds.Where(q => q.RankOrder == 1 && profile_ids.Contains(q.PersonId) && q.CertificateTypeId == certificate_type.Id).Select(q => q.Id).ToList();
            var history_whr = string.Join(",", last_history);
            context.Database.ExecuteSqlCommand(
                      "UPDATE CertificateHistory SET DateExpire = null WHERE Id in (" + history_whr + ")");

            await context.SaveChangesAsync();

            //return new DataResponse()
            //{
            //    Data = dto,
            //    IsSuccess = false,
            //    Errors = new List<string>() { "Duplicated Title Found" }
            //};

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public async Task<DataResponse> SaveCourseTypeJobGroup(int tid, int gid, int man, int sel)
        {
            CourseTypeJobGroup cj = null;
            if (sel == 0)
            {
                cj = await context.CourseTypeJobGroups.Where(q => q.CourseTypeId == tid && q.JobGroupId == gid).FirstOrDefaultAsync();
                if (cj != null)
                    context.CourseTypeJobGroups.Remove(cj);

                var childs = await context.JobGroups.Where(q => q.ParentId == gid).ToListAsync();
                var childIds = childs.Select(q => q.Id).ToList();

                var childscj = await context.CourseTypeJobGroups.Where(q => q.CourseTypeId == tid && childIds.Contains(q.JobGroupId)).ToListAsync();
                if (childscj != null && childscj.Count > 0)
                    context.CourseTypeJobGroups.RemoveRange(childscj);

            }
            else
            {
                var groupIds = new List<int>();

                var childs = await context.JobGroups.Where(q => q.ParentId == gid).ToListAsync();
                groupIds = childs.Select(q => q.Id).ToList();
                groupIds.Add(gid);

                foreach (var _gid in groupIds)
                {
                    cj = await context.CourseTypeJobGroups.Where(q => q.CourseTypeId == tid && q.JobGroupId == _gid).FirstOrDefaultAsync();
                    if (cj != null)
                    {
                        cj.Mandatory = man == 1;
                    }
                    else
                    {
                        cj = new CourseTypeJobGroup()
                        {
                            JobGroupId = _gid,
                            CourseTypeId = tid,
                            Mandatory = man == 1,

                        };
                        context.CourseTypeJobGroups.Add(cj);
                    }
                }



            }





            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = cj,
            };
        }

        public async Task<DataResponse> GetCourseDocs(int id)
        {
            var docs = await context.ViewCourseDocuments.Where(q => q.CourseId == id).ToListAsync();

            return new DataResponse()
            {
                Data = docs,
                IsSuccess = true,
            };
        }
        //07-13
       
        public async Task<DataResponse> SaveExamScore(exam_score dto)
        {
            //int id = Convert.ToInt32(dto.id);
            //int score = Convert.ToInt32(dto.score);
            var course_people = await context.CoursePeoples.FirstOrDefaultAsync(q => q.Id == dto.id);
            if (course_people != null)
            {
                course_people.ExamResult= dto.score;
            }
            await context.SaveChangesAsync();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<DataResponse> SaveCourse(ViewModels.CourseViewModel dto)
        {
            //2024-12-30
            try
            {
                Course entity = null;

                if (dto.Id == -1)
                {
                    entity = new Course();
                    context.Courses.Add(entity);
                }

                else
                {
                    entity = await context.Courses.FirstOrDefaultAsync(q => q.Id == dto.Id);

                }

                if (entity == null)
                    return new DataResponse()
                    {
                        Data = dto,
                        IsSuccess = false,
                        Errors = new List<string>() { "entity not found" }
                    };

                //ViewModels.Location.Fill(entity, dto);
                entity.CourseTypeId = dto.CourseTypeId;
                entity.DateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
                entity.DateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;
                entity.Instructor = dto.Instructor;
                entity.CurrencyId = dto.CurrencyId;
                entity.Location = dto.Location;
                entity.OrganizationId = dto.OrganizationId;
                entity.Duration = dto.Duration;
                entity.DurationUnitId = dto.DurationUnitId;
                entity.Remark = dto.Remark;
                entity.TrainingDirector = dto.TrainingDirector;
                entity.Title = dto.Title;
                entity.Recurrent = dto.Recurrent;
                entity.Interval = dto.Interval;
                entity.CalanderTypeId = dto.CalanderTypeId;
                entity.IsGeneral = dto.IsGeneral;
                entity.CustomerId = dto.CustomerId;
                entity.No = dto.No;
                entity.IsNotificationEnabled = dto.IsNotificationEnabled;
                entity.StatusId = dto.StatusId;
                entity.HoldingType = dto.HoldingType;
                entity.Instructor2 = dto.Instructor2Id;
                entity.Cost = dto.Cost;
                entity.HoldingType = dto.HoldingType;
                entity.SendLetter = dto.SendLetter;
                entity.Financial = dto.Financial;
                entity.InForm = dto.InForm;
                entity.Certificate = dto.Certificate;
                entity.ExamType = dto.ExamType;



                if (dto.Id == -1)
                {
                    if (dto.Sessions.Count > 0)
                    {
                        foreach (var s in dto.Sessions)
                        {
                            var dtobj = DateObject.ConvertToDateTimeSession(s);
                            entity.CourseSessions.Add(new CourseSession()
                            {
                                Done = false,
                                Key = s,
                                DateStart = dtobj[0].Date,
                                DateStartUtc = dtobj[0].DateUtc,
                                DateEnd = dtobj[1].Date,
                                DateEndUtc = dtobj[1].DateUtc,
                            });
                        }
                    }

                    if (dto.Syllabi.Count > 0)
                    {
                        foreach (var subject in dto.Syllabi)
                        {
                            //entity.CourseSyllabus.Add(new CourseSyllabu()
                            //{
                            //    Duration = x.Duration,
                            //    Title = x.Title,
                            //    CourseTypeId = x.TypeId,
                            //    InstructorId = x.InstructorId,

                            //});

                            var crs_subject = new Course();
                            entity.Course1.Add(crs_subject);

                            crs_subject.CourseTypeId = subject.CourseTypeId;
                            // entity.DateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
                            // entity.DateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;

                            crs_subject.CurrencyId = subject.CurrencyId;
                            crs_subject.Location = entity.Location;
                            crs_subject.OrganizationId = entity.OrganizationId;
                            crs_subject.Duration = subject.Duration;
                            crs_subject.DurationUnitId = entity.DurationUnitId;
                            crs_subject.Remark = subject.Remark;
                            crs_subject.TrainingDirector = entity.TrainingDirector;
                            crs_subject.Title = subject.Title;

                            crs_subject.Interval = subject.Interval;
                            crs_subject.CalanderTypeId = entity.CalanderTypeId;
                            crs_subject.IsGeneral = entity.IsGeneral;
                            crs_subject.CustomerId = entity.CustomerId;

                            crs_subject.IsNotificationEnabled = entity.IsNotificationEnabled;
                            crs_subject.StatusId = entity.StatusId;
                            crs_subject.HoldingType = entity.HoldingType;

                            crs_subject.Cost = entity.Cost;
                            crs_subject.HoldingType = entity.HoldingType;
                            crs_subject.SendLetter = entity.SendLetter;
                            crs_subject.Financial = entity.Financial;
                            crs_subject.InForm = entity.InForm;
                            crs_subject.Certificate = entity.Certificate;


                            var _subject_sessions = subject.Sessions;
                            List<CourseSession> _temp_sessions = new List<CourseSession>();
                            foreach (var s in _subject_sessions)
                            {
                                //12-29
                                var dtobj = DateObject.ConvertToDateTimeSession(s);
                                var _cs = new CourseSession()
                                {
                                    Done = false,
                                    Key = s,
                                    DateStart = dtobj[0].Date,
                                    DateStartUtc = dtobj[0].DateUtc,
                                    DateEnd = dtobj[1].Date,
                                    DateEndUtc = dtobj[1].DateUtc,
                                };
                                crs_subject.CourseSessions.Add(_cs);
                                _temp_sessions.Add(_cs);
                            }

                            if (_temp_sessions.Count > 0)
                            {
                                crs_subject.DateStart = (DateTime)_temp_sessions.OrderBy(q => q.DateStart).First().DateStart;
                                crs_subject.DateEnd = _temp_sessions.OrderByDescending(q => q.DateEnd).First().DateEnd;

                            }
                            else
                            {
                                crs_subject.DateStart = entity.DateStart;
                                crs_subject.DateEnd = entity.DateEnd;
                            }




                        }
                    }
                }
                else
                {

                    ///SESSIONS 
                    var _sessions = await context.CourseSessions.Where(q => q.CourseId == dto.Id).ToListAsync();
                    var _sessionKeys = _sessions.Select(q => q.Key).ToList();


                    var _deleted = _sessions.Where(q => !dto.Sessions.Contains(q.Key)).ToList();
                    var _deletedKeys = _deleted.Select(q => q.Key).ToList();

                    var sessionFdps = await context.CourseSessionFDPs.Where(q => q.CourseId == dto.Id && _deletedKeys.Contains(q.SessionKey)).ToListAsync();
                    var fdpIds = sessionFdps.Select(q => q.FDPId).ToList();
                    var fdps = await context.FDPs.Where(q => fdpIds.Contains(q.Id)).ToListAsync();
                    context.FDPs.RemoveRange(fdps);


                    context.CourseSessions.RemoveRange(_deleted);

                    var _newSessions = dto.Sessions.Where(q => !_sessionKeys.Contains(q)).ToList();
                    foreach (var s in _newSessions)
                    {
                        //12-29
                        var dtobj = DateObject.ConvertToDateTimeSession(s);
                        entity.CourseSessions.Add(new CourseSession()
                        {
                            Done = false,
                            Key = s,
                            DateStart = dtobj[0].Date,
                            DateStartUtc = dtobj[0].DateUtc,
                            DateEnd = dtobj[1].Date,
                            DateEndUtc = dtobj[1].DateUtc,
                        });
                    }
                    /////////////////////////////////////////////
                    //var _syllabi = await context.CourseSyllabus.Where(q => q.CourseId == dto.Id).ToListAsync();
                    //var _syllabiIds = _syllabi.Select(q => q.Id).ToList();
                    //var _dtoIds = dto.Syllabi.Select(q => q.Id).ToList();
                    //var _deletedSyl = _syllabi.Where(q => !_dtoIds.Contains(q.Id)).ToList();
                    //context.CourseSyllabus.RemoveRange(_deletedSyl);

                    //var newSyllabi = dto.Syllabi.Where(q => q.Id < 0).ToList();
                    //foreach (var x in newSyllabi)
                    //{
                    //    entity.CourseSyllabus.Add(new CourseSyllabu() { Duration = x.Duration, Title = x.Title, CourseTypeId = x.TypeId, InstructorId = x.InstructorId });
                    //}

                    var _dtoIds = dto.Syllabi.Select(q => q.Id).ToList();
                    var _syllabi = await context.Courses.Where(q => q.ParentId == dto.Id).ToListAsync();
                    var _syllabi_ids = _syllabi.Select(q => q.Id).ToList();
                    var _deletedSyl = _syllabi.Where(q => !_dtoIds.Contains(q.Id)).ToList();
                    var _deletedSyl_ids = _deletedSyl.Select(q => q.Id).ToList();


                    //context.CourseSyllabus.RemoveRange(_deletedSyl);
                    var _syllabi_sessions = await context.CourseSessions.Where(q => _syllabi_ids.Contains(q.CourseId)).ToListAsync();
                    var _syllabi_sessionKeys = _syllabi_sessions.Select(q => q.Key).ToList();
                    List<string> _sylabi_dto_sessions = new List<string>();
                    foreach (var x in dto.Syllabi)
                    {
                        _sylabi_dto_sessions = _sylabi_dto_sessions.Concat(x.Sessions).ToList();
                    }

                    var _syllabi_deleted_sessions_keys = _syllabi_sessionKeys.Where(q => !_sylabi_dto_sessions.Contains(q)).ToList();
                    var _syllabi_deleted_sessions = _syllabi_sessions.Where(q => _syllabi_deleted_sessions_keys.Contains(q.Key)).ToList();

                    var _syllabi_sessionFdps = await context.CourseSessionFDPs.Where(q => _syllabi_ids.Contains(q.CourseId) && _syllabi_deleted_sessions_keys.Contains(q.SessionKey)).ToListAsync();
                    var _syllabi_fdpIds = _syllabi_sessionFdps.Select(q => q.FDPId).ToList();
                    var _syllabi_fdps = await context.FDPs.Where(q => _syllabi_fdpIds.Contains(q.Id)).ToListAsync();
                    context.FDPs.RemoveRange(_syllabi_fdps);


                    context.CourseSessions.RemoveRange(_syllabi_deleted_sessions);

                    context.Courses.RemoveRange(_deletedSyl);

                    foreach (var subject in dto.Syllabi)
                    {
                        var crs_subject = _syllabi.FirstOrDefault(q => q.Id == subject.Id);
                        if (crs_subject == null)
                        {
                            crs_subject = new Course();
                            entity.Course1.Add(crs_subject);
                        }
                        crs_subject.CourseTypeId = subject.CourseTypeId;
                        // entity.DateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
                        // entity.DateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;

                        crs_subject.CurrencyId = subject.CurrencyId;
                        crs_subject.Location = entity.Location;
                        crs_subject.OrganizationId = entity.OrganizationId;
                        crs_subject.Duration = subject.Duration;
                        crs_subject.DurationUnitId = entity.DurationUnitId;
                        crs_subject.Remark = subject.Remark;
                        crs_subject.TrainingDirector = entity.TrainingDirector;
                        crs_subject.Title = subject.Title;

                        crs_subject.Interval = subject.Interval;
                        crs_subject.CalanderTypeId = entity.CalanderTypeId;
                        crs_subject.IsGeneral = entity.IsGeneral;
                        crs_subject.CustomerId = entity.CustomerId;

                        crs_subject.IsNotificationEnabled = entity.IsNotificationEnabled;
                        crs_subject.StatusId = entity.StatusId;
                        crs_subject.HoldingType = entity.HoldingType;

                        crs_subject.Cost = entity.Cost;
                        crs_subject.HoldingType = entity.HoldingType;
                        crs_subject.SendLetter = entity.SendLetter;
                        crs_subject.Financial = entity.Financial;
                        crs_subject.InForm = entity.InForm;
                        crs_subject.Certificate = entity.Certificate;

                        var _subject_db_sessions = _syllabi_sessions.Where(q => q.CourseId == subject.Id).Select(q => q.Key).ToList();
                        var _subject_sessions = subject.Sessions.Where(q => !_subject_db_sessions.Contains(q)).ToList();
                        var _subject_sessions_deleted_key = _subject_db_sessions.Where(q => !subject.Sessions.Contains(q)).ToList();
                        var _subject_sessions_deleted_obj = _syllabi_sessions.Where(q => _subject_sessions_deleted_key.Contains(q.Key) && q.CourseId == crs_subject.Id).ToList();
                        context.CourseSessions.RemoveRange(_subject_sessions_deleted_obj);
                        List<CourseSession> _temp_sessions = new List<CourseSession>();
                        foreach (var s in subject.Sessions)
                        {
                            var dtobj = DateObject.ConvertToDateTimeSession(s);
                            _temp_sessions.Add(new CourseSession()
                            {
                                Done = false,
                                Key = s,
                                DateStart = dtobj[0].Date,
                                DateStartUtc = dtobj[0].DateUtc,
                                DateEnd = dtobj[1].Date,
                                DateEndUtc = dtobj[1].DateUtc,
                            });
                        }
                        foreach (var s in _subject_sessions)
                        {
                            //12-29
                            var dtobj = DateObject.ConvertToDateTimeSession(s);
                            crs_subject.CourseSessions.Add(new CourseSession()
                            {
                                Done = false,
                                Key = s,
                                DateStart = dtobj[0].Date,
                                DateStartUtc = dtobj[0].DateUtc,
                                DateEnd = dtobj[1].Date,
                                DateEndUtc = dtobj[1].DateUtc,
                            });
                        }
                        if (_temp_sessions.Count > 0)
                        {
                            crs_subject.DateStart = (DateTime)_temp_sessions.OrderBy(q => q.DateStart).First().DateStart;
                            crs_subject.DateEnd = _temp_sessions.OrderByDescending(q => q.DateEnd).First().DateEnd;
                        }
                        else
                        {
                            crs_subject.DateStart = entity.DateStart;
                            crs_subject.DateEnd = entity.DateEnd;
                        }

                    }




                }

                var exiting_exams = await context.trn_exam.Where(q => q.course_id == dto.Id).ToListAsync();
                var exam_ids = exiting_exams.Select(q => q.id).ToList();
                var existing_templates = await context.trn_exam_question_template.Where(q => exam_ids.Contains(q.exam_id)).ToListAsync();

                if (dto.exams != null && dto.exams.Count > 0)
                {

                    var dto_exam = dto.exams.First();
                    if (dto_exam.date_start != null)
                    {
                        var db_exam = await context.trn_exam.FirstOrDefaultAsync(q => q.course_id == dto.Id);
                        if (db_exam == null)
                        {
                            db_exam = new trn_exam();
                            entity.trn_exam.Add(db_exam);
                        }
                        db_exam.date_start = dto_exam.date_start;
                        db_exam.date_start_scheduled = dto_exam.date_start_scheduled;
                        db_exam.duration = dto_exam.duration;
                        db_exam.exam_date = dto_exam.exam_date;
                        db_exam.location_title = dto_exam.location_title;
                        db_exam.location_address = dto_exam.location_address;
                        db_exam.location_phone = dto_exam.location_phone;
                        db_exam.status_id = 0;
                        dto_exam.template = dto_exam.template.Where(q => q.total != null).ToList();


                        foreach (var temp in dto_exam.template)
                        {
                            var db_temp = existing_templates.FirstOrDefault(q => q.exam_id == db_exam.id && q.question_category_id == temp.category_id);
                            if (db_temp == null)
                            {
                                db_temp = new trn_exam_question_template();
                                db_exam.trn_exam_question_template.Add(db_temp);
                            }
                            db_temp.question_category_id = temp.category_id;
                            db_temp.total = temp.total;
                        }

                        var existing_exam_grps = await context.trn_exam_group.Where(q => q.exam_id == db_exam.id).ToListAsync();
                        var existing_exam_people = await context.trn_exam_person.Where(q => q.exam_id == db_exam.id).ToListAsync();
                        if (existing_exam_grps != null && existing_exam_grps.Count > 0)
                            context.trn_exam_group.RemoveRange(existing_exam_grps);
                        if (existing_exam_people != null && existing_exam_people.Count > 0)
                            context.trn_exam_person.RemoveRange(existing_exam_people);
                        // var grps = await context.JobGroups.Where(q => dto_exam.groups.Contains(q.Id)).ToListAsync();
                        foreach (var g in dto_exam.groups)
                        {
                            db_exam.trn_exam_group.Add(new trn_exam_group() { group_id = g });
                        }
                        foreach (var p in dto_exam.people)
                            db_exam.trn_exam_person.Add(new trn_exam_person() { person_id = p });
                    }

                }


                //pasco
                /* var docs = await context.CourseDocuments.Where(q => q.CourseId == dto.Id).ToListAsync();
                 var docids = dto.Documents.Where(q => q.Id > 0).Select(q => q.Id).ToList();
                 var deleted = docs.Where(q => !docids.Contains(q.Id)).ToList();
                 context.CourseDocuments.RemoveRange(deleted);

                 var newdocs = dto.Documents.Where(q => q.Id < 0).ToList();

                 foreach (var x in newdocs)
                     entity.CourseDocuments.Add(new CourseDocument()
                     {
                         FileUrl = x.FileUrl,
                         Remark = x.Remark,
                         TypeId = x.TypeId,
                     });*/
                /////////////////////////////////////////


                await context.SaveChangesAsync();
                if (dto.session_changed == 1 && entity.Date_Sessions_Synced != null)
                {

                    try
                    {
                        await SyncSessionsToRosterByDate(entity.Id);
                        // entity.Date_Sessions_Instructor_Synced = DateTime.Now;
                        //  entity.Date_Sessions_Synced = DateTime.Now;
                        await context.SaveChangesAsync();


                    }
                    catch (Exception ex)
                    {
                        entity.Date_Sessions_Instructor_Synced = null;
                        entity.Date_Sessions_Synced = null;
                        await context.SaveChangesAsync();
                    }

                }
                dto.Id = entity.Id;
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = dto,
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = dto,
                };
            }
        }

        public async Task<DataResponse> SaveCertificate(ViewModels.CertificateViewModel dto)
        {
            var _dateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
            var _dateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;

            Course entity = await context.Courses.Where(q => q.DateStart == _dateStart && q.DateEnd == _dateEnd && q.CourseTypeId == dto.CourseTypeId
            && q.OrganizationId == dto.OrganizationId && q.IsInside == false).FirstOrDefaultAsync();

            if (entity == null)
            {
                entity = new Course();
                context.Courses.Add(entity);
                entity.CourseTypeId = dto.CourseTypeId;
                entity.DateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
                entity.DateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;
                entity.Instructor = dto.Instructor;
                entity.Location = dto.Location;
                entity.OrganizationId = dto.OrganizationId;
                entity.Duration = dto.Duration;
                entity.DurationUnitId = dto.DurationUnitId;
                entity.Remark = dto.Remark;
                entity.TrainingDirector = dto.TrainingDirector;
                entity.Title = dto.Title;
                entity.Recurrent = dto.Recurrent;
                entity.Interval = dto.Interval;
                entity.CalanderTypeId = dto.CalanderTypeId;
                entity.IsGeneral = dto.IsGeneral;
                entity.CustomerId = dto.CustomerId;
                entity.No = dto.No;
                entity.IsNotificationEnabled = dto.IsNotificationEnabled;
                entity.StatusId = 3;
                entity.IsInside = false;
            }
            if (dto.PersonId != null)
            {
                var cp = await context.CoursePeoples.Where(q => q.PersonId == dto.PersonId && q.CourseId == entity.Id).FirstOrDefaultAsync();
                if (cp == null)
                {
                    cp = new CoursePeople()
                    {
                        PersonId = dto.PersonId,
                        StatusId = 1,
                        DateStatus = DateTime.Now,
                        DateExpire = (DateTime)DateObject.ConvertToDate(dto.DateExpire).Date,
                        DateIssue = (DateTime)DateObject.ConvertToDate(dto.DateIssue).Date,
                        CertificateNo = dto.CertificateNo,
                    };
                    entity.CoursePeoples.Add(cp);
                }
                else
                {
                    cp.DateExpire = (DateTime)DateObject.ConvertToDate(dto.DateExpire).Date;
                    cp.DateIssue = (DateTime)DateObject.ConvertToDate(dto.DateIssue).Date;
                    cp.CertificateNo = dto.CertificateNo;
                    cp.StatusId = 1;
                }
                //////////////////////////
                var person = await context.People.Where(q => q.Id == cp.PersonId).FirstOrDefaultAsync();
                var ct = await context.ViewCourseTypes.Where(q => q.Id == dto.CourseTypeId).FirstOrDefaultAsync();
                //12-03
                if (ct != null && 1 == 3)
                {
                    switch (ct.CertificateType)
                    {


                        //CYBER SECURITY
                        case "CYBER SECURITY":
                            if ((DateTime)cp.DateExpire > person.ExpireDate2 || person.ExpireDate2 == null)
                            {
                                person.ExpireDate2 = cp.DateExpire;
                                person.IssueDate2 = cp.DateIssue;
                            }
                            break;

                        //ERP
                        case "ERP":
                            if ((DateTime)cp.DateExpire > person.ERPExpireDate || person.ERPExpireDate == null)
                            {
                                person.ERPExpireDate = cp.DateExpire;
                                person.ERPIssueDate = cp.DateIssue;
                            }
                            break;
                        //HF
                        case "HF":
                            if ((DateTime)cp.DateExpire > person.HFExpireDate || person.HFExpireDate == null)
                            {
                                person.HFExpireDate = cp.DateExpire;
                                person.HFIssueDate = cp.DateIssue;
                            }
                            break;
                        //MEL/CDL
                        case "MEL/CDL":
                            if ((DateTime)cp.DateExpire > person.MELExpireDate || person.MELExpireDate == null)
                            {
                                person.MELExpireDate = cp.DateExpire;
                                person.MELIssueDate = cp.DateIssue;
                            }
                            break;
                        //METEOROLOGY
                        case "METEOROLOGY":
                            if ((DateTime)cp.DateExpire > person.METExpireDate || person.METExpireDate == null)
                            {
                                person.METExpireDate = cp.DateExpire;
                                person.METIssueDate = cp.DateIssue;
                            }
                            break;
                        //PERFORMANCE
                        case "PERFORMANCE":
                            if ((DateTime)cp.DateExpire > person.PERExpireDate || person.PERExpireDate == null)
                            {
                                person.PERExpireDate = cp.DateExpire;
                                person.PERIssueDate = cp.DateIssue;
                            }
                            break;
                        //RADIO COMMUNICATION
                        case "RADIO COMMUNICATION":
                            if ((DateTime)cp.DateExpire > person.LRCExpireDate || person.LRCExpireDate == null)
                            {
                                person.LRCExpireDate = cp.DateExpire;
                                person.LRCIssueDate = cp.DateIssue;
                            }
                            break;
                        //SITA
                        case "SITA":
                            if ((DateTime)cp.DateExpire > person.RSPExpireDate || person.RSPExpireDate == null)
                            {
                                person.RSPExpireDate = cp.DateExpire;
                                person.RSPIssueDate = cp.DateIssue;
                            }
                            break;
                        //WEIGHT AND BALANCE
                        case "WEIGHT AND BALANCE":
                            if ((DateTime)cp.DateExpire > person.MBExpireDate || person.MBExpireDate == null)
                            {
                                person.MBExpireDate = cp.DateExpire;
                                person.MBIssueDate = cp.DateIssue;
                            }
                            break;
                        //AIRSIDE SAFETY AND DRIVING
                        case "AIRSIDE SAFETY AND DRIVING":
                            if ((DateTime)cp.DateExpire > person.ASDExpireDate || person.ASDExpireDate == null)
                            {
                                person.ASDExpireDate = cp.DateExpire;
                                person.ASDIssueDate = cp.DateIssue;
                            }
                            break;
                        //AIRSIDE SAFETY AND DRIVING (IKA)
                        case "AIRSIDE SAFETY AND DRIVING (IKA)":
                            if ((DateTime)cp.DateExpire > person.ExpireDate1 || person.ExpireDate1 == null)
                            {
                                person.ExpireDate1 = cp.DateExpire;
                                person.IssueDate1 = cp.DateIssue;
                            }
                            break;
                        //GOM
                        case "GOM":
                            if ((DateTime)cp.DateExpire > person.GOMExpireDate || person.GOMExpireDate == null)
                            {
                                person.GOMExpireDate = cp.DateExpire;
                                person.GOMIssueDate = cp.DateIssue;
                            }
                            break;
                        //AIRPORT SERVICE FAMILIARIZATION
                        case "AIRPORT SERVICE FAMILIARIZATION":
                            if ((DateTime)cp.DateExpire > person.ASFExpireDate || person.ASFExpireDate == null)
                            {
                                person.ASFExpireDate = cp.DateExpire;
                                person.ASFIssueDate = cp.DateIssue;
                            }
                            break;
                        //CUSTOMER CARE
                        case "CUSTOMER CARE":
                            if ((DateTime)cp.DateExpire > person.CCExpireDate || person.CCExpireDate == null)
                            {
                                person.CCExpireDate = cp.DateExpire;
                                person.CCIssueDate = cp.DateIssue;
                            }
                            break;
                        //LOAD SHEET
                        case "LOAD SHEET":
                            if ((DateTime)cp.DateExpire > person.MBExpireDate || person.MBExpireDate == null)
                            {
                                person.MBExpireDate = cp.DateExpire;
                                person.MBIssueDate = cp.DateIssue;
                            }
                            break;
                        //PASSENGER SERVICE
                        case "PASSENGER SERVICE":
                            if ((DateTime)cp.DateExpire > person.PSExpireDate || person.PSExpireDate == null)
                            {
                                person.PSExpireDate = cp.DateExpire;
                                person.PSIssueDate = cp.DateIssue;
                            }
                            break;

                        //DRM
                        case "DRM":
                            if ((DateTime)cp.DateExpire > person.DRMExpireDate || person.DRMExpireDate == null)
                            {
                                person.DRMExpireDate = cp.DateExpire;
                                person.DRMIssueDate = cp.DateIssue;
                            }
                            break;
                        //ANNEX
                        case "ANNEX":
                            if ((DateTime)cp.DateExpire > person.ANNEXExpireDate || person.ANNEXExpireDate == null)
                            {
                                person.ANNEXExpireDate = cp.DateExpire;
                                person.ANNEXIssueDate = cp.DateIssue;
                            }
                            break;
                        //FRMS
                        case "FRMS":
                            if ((DateTime)cp.DateExpire > person.TypeAirbusExpireDate || person.TypeAirbusExpireDate == null)
                            {
                                person.TypeAirbusExpireDate = cp.DateExpire;
                                person.TypeAirbusIssueDate = cp.DateIssue;
                            }
                            break;
                        //DANGEROUS GOODS
                        case "DANGEROUS GOODS":
                            if ((DateTime)cp.DateExpire > person.DangerousGoodsExpireDate || person.DangerousGoodsExpireDate == null)
                            {
                                person.DangerousGoodsExpireDate = cp.DateExpire;
                                person.DangerousGoodsIssueDate = cp.DateIssue;
                            }
                            break;
                        //1	SEPT-P
                        case "SEPT":
                            if ((DateTime)cp.DateExpire > person.SEPTPExpireDate || person.SEPTPExpireDate == null)
                            {
                                person.SEPTPExpireDate = cp.DateExpire;
                                person.SEPTPIssueDate = cp.DateIssue;
                            }
                            break;
                        //2   SEPT - T
                        case "ANNUAL SEPT":
                            if ((DateTime)cp.DateExpire > person.SEPTExpireDate || person.SEPTExpireDate == null)
                            {
                                person.SEPTExpireDate = cp.DateExpire;
                                person.SEPTIssueDate = cp.DateIssue;
                            }
                            break;
                        //4	CRM
                        case "CRM":
                            if ((DateTime)cp.DateExpire > person.UpsetRecoveryTrainingExpireDate || person.UpsetRecoveryTrainingExpireDate == null)
                            {
                                person.UpsetRecoveryTrainingExpireDate = cp.DateExpire;
                                person.UpsetRecoveryTrainingIssueDate = cp.DateIssue;
                            }
                            break;
                        //5	CCRM
                        case "CCRM":
                            if ((DateTime)cp.DateExpire > person.CCRMExpireDate || person.CCRMExpireDate == null)
                            {
                                person.CCRMExpireDate = cp.DateExpire;
                                person.CCRMIssueDate = cp.DateIssue;
                            }
                            break;
                        //6	SMS
                        case "SMS":
                            if ((DateTime)cp.DateExpire > person.SMSExpireDate || person.SMSExpireDate == null)
                            {
                                person.SMSExpireDate = cp.DateExpire;
                                person.SMSIssueDate = cp.DateIssue;
                            }
                            break;
                        //7	AV-SEC
                        case "AVIATION SECURITY":
                            if ((DateTime)cp.DateExpire > person.AviationSecurityExpireDate || person.AviationSecurityExpireDate == null)
                            {
                                person.AviationSecurityExpireDate = cp.DateExpire;
                                person.AviationSecurityIssueDate = cp.DateIssue;
                            }
                            break;
                        //8	COLD-WX
                        case "COLD WEATHER OPERATION":
                            if ((DateTime)cp.DateExpire > person.ColdWeatherOperationExpireDate || person.ColdWeatherOperationExpireDate == null)
                            {
                                person.ColdWeatherOperationExpireDate = cp.DateExpire;
                                person.ColdWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //9	HOT-WX
                        case "HOT WEATHER OPERATION":
                            if ((DateTime)cp.DateExpire > person.HotWeatherOperationExpireDate || person.HotWeatherOperationExpireDate == null)
                            {
                                person.HotWeatherOperationExpireDate = cp.DateExpire;
                                person.HotWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //10	FIRSTAID
                        case "FIRST AID":
                            if ((DateTime)cp.DateExpire > person.FirstAidExpireDate || person.FirstAidExpireDate == null)
                            {
                                person.FirstAidExpireDate = cp.DateExpire;
                                person.FirstAidIssueDate = cp.DateIssue;
                            }
                            break;
                        ////lpc
                        //case 100:
                        //    if ((DateTime)cp.DateExpire > person.ProficiencyValidUntil || person.ProficiencyValidUntil == null)
                        //    {
                        //        person.ProficiencyValidUntil = cp.DateExpire;
                        //        person.ProficiencyCheckDate = cp.DateIssue;
                        //    }
                        //    break;
                        ////opc
                        //case 101:
                        //    if ((DateTime)cp.DateExpire > person.ProficiencyValidUntilOPC || person.ProficiencyValidUntilOPC == null)
                        //    {
                        //        person.ProficiencyValidUntilOPC = cp.DateExpire;
                        //        person.ProficiencyCheckDateOPC = cp.DateIssue;
                        //    }
                        //    break;
                        ////lpr
                        //case 102:
                        //    if ((DateTime)cp.DateExpire > person.ICAOLPRValidUntil || person.ICAOLPRValidUntil == null)
                        //    {
                        //        person.ICAOLPRValidUntil = cp.DateExpire;
                        //        // person.ProficiencyCheckDateOPC = cp.DateIssue;
                        //    }
                        //    break;

                        //grt
                        case "GRT":
                            if ((DateTime)cp.DateExpire > person.DateCaoCardExpire || person.DateCaoCardExpire == null)
                            {
                                person.DateCaoCardExpire = cp.DateExpire;
                                person.DateCaoCardIssue = cp.DateIssue;
                            }
                            break;
                        //recurrent
                        case "RECURRENT 737":
                            if ((DateTime)cp.DateExpire > person.Type737ExpireDate || person.Type737ExpireDate == null)
                            {
                                person.Type737ExpireDate = cp.DateExpire;
                                person.Type737IssueDate = cp.DateIssue;
                            }
                            break;
                        //fmt
                        case "FMT":
                            if ((DateTime)cp.DateExpire > person.EGPWSExpireDate || person.EGPWSExpireDate == null)
                            {
                                person.EGPWSExpireDate = cp.DateExpire;
                                person.EGPWSIssueDate = cp.DateIssue;
                            }
                            break;
                        case "FMTD":
                            if ((DateTime)cp.DateExpire > person.FMTDExpireDate || person.FMTDExpireDate == null)
                            {
                                person.FMTDExpireDate = cp.DateExpire;
                                person.FMTDIssueDate = cp.DateIssue;
                            }
                            break;
                        case "LINE":
                            if ((DateTime)cp.DateExpire > person.LineExpireDate || person.LineExpireDate == null)
                            {
                                person.LineExpireDate = cp.DateExpire;
                                person.LineIssueDate = cp.DateIssue;
                            }
                            break;
                        default:
                            break;
                    }
                    /*switch (ct.CertificateTypeId)
                    {
                        //dg
                        case 3:
                            if ((DateTime)cp.DateExpire > person.DangerousGoodsExpireDate)
                            {
                                person.DangerousGoodsExpireDate = cp.DateExpire;
                                person.DangerousGoodsIssueDate = cp.DateIssue;
                            }
                            break;
                        //1	SEPT-P
                        case 1:
                            if ((DateTime)cp.DateExpire > person.SEPTPExpireDate)
                            {
                                person.SEPTPExpireDate = cp.DateExpire;
                                person.SEPTPIssueDate = cp.DateIssue;
                            }
                            break;
                        //2   SEPT - T
                        case 2:
                            if ((DateTime)cp.DateExpire > person.SEPTExpireDate)
                            {
                                person.SEPTExpireDate = cp.DateExpire;
                                person.SEPTIssueDate = cp.DateIssue;
                            }
                            break;
                        //4	CRM
                        case 4:
                            if ((DateTime)cp.DateExpire > person.UpsetRecoveryTrainingExpireDate)
                            {
                                person.UpsetRecoveryTrainingExpireDate = cp.DateExpire;
                                person.UpsetRecoveryTrainingIssueDate = cp.DateIssue;
                            }
                            break;
                        //5	CCRM
                        case 5:
                            if ((DateTime)cp.DateExpire > person.CCRMExpireDate)
                            {
                                person.CCRMExpireDate = cp.DateExpire;
                                person.CCRMIssueDate = cp.DateIssue;
                            }
                            break;
                        //6	SMS
                        case 6:
                            if ((DateTime)cp.DateExpire > person.SMSExpireDate)
                            {
                                person.SMSExpireDate = cp.DateExpire;
                                person.SMSIssueDate = cp.DateIssue;
                            }
                            break;
                        //7	AV-SEC
                        case 7:
                            if ((DateTime)cp.DateExpire > person.AviationSecurityExpireDate)
                            {
                                person.AviationSecurityExpireDate = cp.DateExpire;
                                person.AviationSecurityIssueDate = cp.DateIssue;
                            }
                            break;
                        //8	COLD-WX
                        case 8:
                            if ((DateTime)cp.DateExpire > person.ColdWeatherOperationExpireDate)
                            {
                                person.ColdWeatherOperationExpireDate = cp.DateExpire;
                                person.ColdWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //9	HOT-WX
                        case 9:
                            if ((DateTime)cp.DateExpire > person.HotWeatherOperationExpireDate)
                            {
                                person.HotWeatherOperationExpireDate = cp.DateExpire;
                                person.HotWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //10	FIRSTAID
                        case 10:
                            if ((DateTime)cp.DateExpire > person.FirstAidExpireDate)
                            {
                                person.FirstAidExpireDate = cp.DateExpire;
                                person.FirstAidIssueDate = cp.DateIssue;
                            }
                            break;
                        case 105:
                            if ((DateTime)cp.DateExpire > person.LineExpireDate)
                            {
                                person.LineExpireDate = cp.DateExpire;
                                person.LineIssueDate = cp.DateIssue;
                            }
                            break;
                        default:
                            break;
                    }*/
                }


                /////////////////////////


            }




            await context.SaveChangesAsync();
            dto.Id = entity.Id;
            var result = await context.ViewCoursePeopleRankeds.Where(q => q.PersonId == dto.PersonId && q.CourseId == entity.Id).FirstOrDefaultAsync();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result,
            };
        }
        public async Task<DataResponse> CopyCoursePeople(dynamic dto)
        {
            int source_id = Convert.ToInt32(dto.source_id);
            int destination_id = Convert.ToInt32(dto.destination_id);

            var source_people = await context.CoursePeoples.Where(q => q.CourseId == source_id).ToListAsync();
            var destination_people = await context.CoursePeoples.Where(q => q.CourseId == destination_id).ToListAsync();
            var exam = await context.trn_exam.Where(q => q.course_id == destination_id).FirstOrDefaultAsync();
            foreach (var x in source_people)
            {
                var exist = destination_people.FirstOrDefault(q => q.PersonId == x.PersonId);
                if (exist == null)
                {
                    context.CoursePeoples.Add(new CoursePeople
                    {
                        CourseId = destination_id,
                        PersonId = x.PersonId,
                        StatusId = -1,
                    });
                    if (exam != null)
                        context.trn_person_exam.Add(new trn_person_exam()
                        {
                            person_id = x.PersonId,
                            course_id = destination_id,
                            exam_date = exam.exam_date,
                            location_title = exam.location_title,
                            date_start = exam.date_start,
                            duration = exam.duration,
                            main_exam_id = exam.id,
                            location_address = exam.location_address,
                            exam_type_id = exam.exam_type_id,
                            location_phone = exam.location_phone,
                            status_id = exam.status_id,
                            created_by = exam.created_by,
                            date_start_scheduled = exam.date_start_scheduled,
                            date_end_scheduled = exam.date_end_scheduled,
                            created_date = exam.created_date,


                        });
                }
            }

            await context.SaveChangesAsync();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };

        }
        public async Task<DataResponse> SaveCoursePeople(dynamic dto)
        {
            int courseId = Convert.ToInt32(dto.Id);
            var subjects = await context.Courses.Where(q => q.ParentId == courseId).ToListAsync();

            var subject_ids = subjects.Select(q => q.Id).ToList();


            string pid = Convert.ToString(dto.pid);
            // string eid = Convert.ToString(dto.eid);

            var personIds = pid.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            // var employeeIds = eid.Split('-').Select(q => Convert.ToInt32(q)).ToList();

            var exists = await context.CoursePeoples.Where(q => q.CourseId == courseId).Select(q => q.PersonId).ToListAsync();
            var newids = personIds.Where(q => !exists.Contains(q)).ToList();
            var exam = await context.trn_exam.Where(q => q.course_id == courseId).FirstOrDefaultAsync();
            foreach (var id in newids)
            {
                context.CoursePeoples.Add(new CoursePeople()
                {
                    CourseId = courseId,
                    PersonId = id,
                    StatusId = -1,
                });
                foreach (var sid in subject_ids)
                {
                    context.CoursePeoples.Add(new CoursePeople()
                    {
                        CourseId = sid,
                        PersonId = id,
                        StatusId = -1,
                    });
                }
                if (exam != null)
                    context.trn_person_exam.Add(new trn_person_exam()
                    {
                        person_id = id,
                        course_id = courseId,
                        exam_date = exam.exam_date,
                        location_title = exam.location_title,
                        date_start = exam.date_start,
                        duration = exam.duration,
                        main_exam_id = exam.id,
                        location_address = exam.location_address,
                        exam_type_id = exam.exam_type_id,
                        location_phone = exam.location_phone,
                        status_id = exam.status_id,
                        created_by = exam.created_by,
                        date_start_scheduled = exam.date_start_scheduled,
                        date_end_scheduled = exam.date_end_scheduled,
                        created_date = exam.created_date,


                    });
            }

            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }

        public async Task<DataResponse> SaveSyllabus(dynamic dto)
        {
            int Id = Convert.ToInt32(dto.Id);
            string Remark = Convert.ToString(dto.Remark);
            int Done = Convert.ToInt32(dto.Done);
            int Instructor = Convert.ToInt32(dto.Instructor);
            string Session = Convert.ToString(dto.Session);
            int TypeId = Convert.ToInt32(dto.TypeId);
            int InsId = Convert.ToInt32(dto.InstructorId);
            var syllabus = await context.CourseSyllabus.Where(q => q.Id == Id).FirstOrDefaultAsync();
            syllabus.Remark = Remark;
            syllabus.Status = Done;
            syllabus.InstructorId = Instructor;
            syllabus.SessionKey = Session;
            syllabus.CourseTypeId = TypeId;
            syllabus.InstructorId = InsId;


            await context.SaveChangesAsync();
            var syll = await context.ViewSyllabus.Where(q => q.Id == Id).FirstOrDefaultAsync();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = syll,
            };
        }


        public class cspg_dto
        {
            public int cid { get; set; }
            public string sid { get; set; }
            public List<int?> pid { get; set; }

            public bool value { get; set; }
        }
        public async Task<DataResponse> SaveCourseSessionPresenceGroup(cspg_dto dto)
        {
            int courseId = dto.cid;
            List<int?> pid = dto.pid;
            string sid = dto.sid;
            sid = sid.Replace("Session", "");



            var exists = await context.CourseSessionPresences.Where(q => q.CourseId == courseId && pid.Contains(q.PersonId) && q.SessionKey == sid).ToListAsync();

            if (exists != null && exists.Count > 0)
            {
                context.CourseSessionPresences.RemoveRange(exists);
            }
            else
            {
                foreach (var p in pid)
                {
                    context.CourseSessionPresences.Add(new CourseSessionPresence()
                    {
                        PersonId = p,
                        SessionKey = sid,
                        CourseId = courseId,
                        Date = DateTime.Now
                    });
                }

            }

            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public async Task<DataResponse> SaveCourseSessionPresenceAll(cspg_dto dto)
        {
            int courseId = dto.cid;
            var subjects = await context.Courses.Where(q => q.ParentId == dto.cid).Select(q =>(Nullable<int>) q.Id).ToListAsync();
            subjects.Add(courseId);
            var query = from x in context.CourseSessionPresences
                        where subjects.Contains(x.CourseId)
                        select x;
            var rows = await query.ToListAsync();
            context.CourseSessionPresences.RemoveRange(rows);
            if (dto.value)
            {
                if (subjects.Count > 1)
                      subjects.Remove(courseId);
                foreach(var cid in subjects)
                {
                    var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).ToListAsync();
                    var people = await context.CoursePeoples.Where(q => q.CourseId == cid).ToListAsync();
                    foreach (var p in people)
                    {
                        foreach (var s in sessions)
                        {
                            context.CourseSessionPresences.Add(new CourseSessionPresence()
                            {
                                PersonId = p.PersonId,
                                SessionKey = s.Key,
                                CourseId = cid,
                                Date = DateTime.Now
                            });
                        }
                    }
                }
               
            }


            

            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public async Task<DataResponse> SaveCourseSessionPresence(dynamic dto)
        {
            int courseId = Convert.ToInt32(dto.cid);
            int pid = Convert.ToInt32(dto.pid);
            string sid = Convert.ToString(dto.sid);
            sid = sid.Replace("Session", "");



            var exists = await context.CourseSessionPresences.Where(q => q.CourseId == courseId && q.PersonId == pid && q.SessionKey == sid).FirstOrDefaultAsync();

            if (exists != null)
            {
                context.CourseSessionPresences.Remove(exists);
            }
            else
            {
                context.CourseSessionPresences.Add(new CourseSessionPresence()
                {
                    PersonId = pid,
                    SessionKey = sid,
                    CourseId = courseId,
                    Date = DateTime.Now
                });
            }

            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }

        //06-13
        //10-14
        public async Task<DataResponse> UpdateExamResult(dto_exam_result dto)
        {
            var course_people = await context.CoursePeoples.Where(q => q.CourseId == dto.course_id).ToListAsync();
            foreach (var x in course_people)
            {
                var sc = dto.scores.FirstOrDefault(q => q.person_id == x.PersonId);
                if (sc != null)
                {
                    x.ExamResult = sc.score;
                    x.ExamStatus = sc.status_id;
                }
            }

            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }

        public async Task<DataResponse> UpdateExamSign(dto_exam_sign dto)
        {
            var course = await context.Courses.Where(q => q.Id == dto.course_id).FirstAsync();
            if (course != null)
            {
                course.Date_Exam_Sign_Ins1 = DateTime.Now;

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<DataResponse> UpdateCourseSign(dto_exam_sign dto)
        {
            var course = await context.Courses.Where(q => q.Id == dto.course_id).FirstAsync();
            if (course != null)
            {
                course.Date_Sign_Ins1 = DateTime.Now;

                if (course.ParentId != null)
                {
                    var subjects = await context.Courses.Where(q => q.ParentId == course.ParentId && q.Date_Sign_Ins1 == null).CountAsync();
                    if (subjects == 0)
                    {
                        var parent = await context.Courses.FirstOrDefaultAsync(q => q.Id == course.ParentId);
                        parent.Date_Sign_Ins1 = DateTime.Now;
                    }
                }

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<DataResponse> UpdateCourseSignDirector(dto_exam_sign dto)
        {
            var course = await context.Courses.Where(q => q.Id == dto.course_id).FirstAsync();
            if (course != null)
            {
                course.Date_Sign_Director = DateTime.Now;

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<DataResponse> UpdateCourseSignOPS(dto_exam_sign dto)
        {
            var course = await context.Courses.Where(q => q.Id == dto.course_id).FirstAsync();
            if (course != null)
            {
                course.Date_Sign_OPS = DateTime.Now;

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<DataResponse> UpdateCourseSignStaff(dto_exam_sign dto)
        {
            var course = await context.Courses.Where(q => q.Id == dto.course_id).FirstAsync();
            if (course != null)
            {
                course.Date_Sign_Staff = DateTime.Now;

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public async Task<DataResponse> UpdateCoursePeopleSign(dto_exam_sign dto)
        {
            var cp = await context.CoursePeoples.Where(q => q.CourseId == dto.course_id && q.PersonId == dto.persin_id).FirstAsync();
            if (cp != null)
            {
                cp.Date_Sign_Ins1 = DateTime.Now;

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }

        public async Task<DataResponse> UpdateCoursePeopleSignOPS(dto_exam_sign dto)
        {
            var cp = await context.CoursePeoples.Where(q => q.CourseId == dto.course_id && q.PersonId == dto.persin_id).FirstAsync();
            if (cp != null)
            {
                cp.Date_Sign_OPS = DateTime.Now;

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<DataResponse> UpdateCoursePeopleSignStaff(dto_exam_sign dto)
        {
            var cp = await context.CoursePeoples.Where(q => q.CourseId == dto.course_id && q.PersonId == dto.persin_id).FirstAsync();
            if (cp != null)
            {
                cp.Date_Sign_Staff = DateTime.Now;

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<DataResponse> UpdateCoursePeopleSignDirector(dto_exam_sign dto)
        {
            var cp = await context.CoursePeoples.Where(q => q.CourseId == dto.course_id && q.PersonId == dto.persin_id).FirstAsync();
            if (cp != null)
            {
                cp.Date_Sign_Director = DateTime.Now;

            }
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }
        public async Task<DataResponse> UpdateCoursePeopleStatus(CoursePeopleStatusViewModel dto)
        {
            CoursePeople cp = null;




            if (dto.Id != -1)
                cp = await context.CoursePeoples.Where(q => q.Id == dto.Id).FirstOrDefaultAsync();
            else
                cp = await context.CoursePeoples.Where(q => q.PersonId == dto.PersonId && q.CourseId == dto.CourseId).FirstOrDefaultAsync();
            if (dto.StatusId != cp.StatusId)
                cp.DateStatus = DateTime.Now;

            var course = await context.ViewCourseNews.Where(q => q.Id == cp.CourseId).FirstOrDefaultAsync();
            var profile = context.ViewProfiles.Where(q => q.PersonId == dto.PersonId).FirstOrDefault();
            var not_applicable = context.CourseTypeApplicables.Where(q => q.IsApplicable == false && (q.TrainingGroup == profile.JobGroupRoot || q.TrainingGroup == profile.PostRoot)
                  && q.CourseTypeId == course.CourseTypeId).FirstOrDefault();
            var history = await context.CertificateHistories.Where(q => q.PersonId == dto.PersonId && q.CourseId == dto.CourseId).FirstOrDefaultAsync();

            var certificate_type = await context.CertificateTypes.Where(q => q.Id == course.CertificateTypeId).FirstOrDefaultAsync();
            // var field_map = await (from m in context.TrnDbAppFieldMappings
            //                        join ct in context.CertificateTypes on m.CertificateTypeId equals ct.TypeId
            //                        where ct.Id == course.CertificateTypeId //&& dto.Group.StartsWith(m.GroupCode)
            //                        select m).FirstOrDefaultAsync();
            if (history != null)
                context.CertificateHistories.Remove(history);
            //-1: UnKnown 0:Failed 1:Passed
            if (dto.StatusId != 1)
            {
                cp.DateIssue = null;
                cp.DateExpire = null;
                cp.CertificateNo = null;

                //remove record from history


            }
            else
            {
                cp.DateExpire = string.IsNullOrEmpty(dto.Expire) ? null : DateObject.ConvertToDate(dto.Expire).Date;
                cp.DateIssue = string.IsNullOrEmpty(dto.Issue) ? null : DateObject.ConvertToDate(dto.Issue).Date;
                cp.CertificateNo = dto.No;
                if (string.IsNullOrEmpty(cp.CertificateNo))
                    cp.CertificateNo = "FPC-" + cp.Id;

                //add record to history
                context.CertificateHistories.Add(new CertificateHistory()
                {
                    CourseId = dto.CourseId,
                    CertificateType = certificate_type != null ? (!string.IsNullOrEmpty(certificate_type.IssueField) ? certificate_type.IssueField : certificate_type.ExpireField) : "", //field_map.Identifier,
                    DateCreate = DateTime.Now,
                    DateExpire = not_applicable == null ? cp.DateExpire : null,
                    DateIssue = cp.DateIssue,
                    PersonId = (int)cp.PersonId,
                    Remark = "Update Course Result"

                });
            }



            cp.StatusId = dto.StatusId;
            cp.StatusRemark = dto.Remark;

            if (dto.StatusId == 1 && (cp.DateIssue != null || cp.DateExpire != null) && /*field_map.DbFieldIssue!=null*/certificate_type != null)
            {
                var cmd_upd_part = new List<string>();

                if (cp.DateIssue != null && !string.IsNullOrEmpty(certificate_type.IssueField))
                    cmd_upd_part.Add(/*field_map.DbFieldIssue*/certificate_type.IssueField + "='" + ((DateTime)cp.DateIssue).ToString("yyyy-MM-dd") + "' ");
                if (cp.DateExpire != null && !string.IsNullOrEmpty(certificate_type.IssueField))
                    cmd_upd_part.Add(/*field_map.DbFieldExpire*/certificate_type.ExpireField + "='" + ((DateTime)cp.DateExpire).ToString("yyyy-MM-dd") + "' ");
                if (cp.DateExpire == null && !string.IsNullOrEmpty(certificate_type.IssueField))
                    cmd_upd_part.Add(/*field_map.DbFieldExpire*/certificate_type.ExpireField + "=null ");

                var cmd_upd = "Update Person set "
                    + string.Join(",", cmd_upd_part)
                    + " Where id=" + dto.PersonId;

                var i = context.Database.ExecuteSqlCommand(cmd_upd);

            }

            if (1 == 2 && dto.StatusId == 1 && cp.DateIssue != null && cp.DateExpire != null && !string.IsNullOrEmpty(cp.CertificateNo))
            {



                var person = await context.People.Where(q => q.Id == cp.PersonId).FirstOrDefaultAsync();





                switch (course.CertificateType)
                {


                    //CYBER SECURITY
                    case "CYBER SECURITY":
                        if ((DateTime)cp.DateExpire > person.ExpireDate2 || person.ExpireDate2 == null)
                        {
                            person.ExpireDate2 = cp.DateExpire;
                            person.IssueDate2 = cp.DateIssue;
                        }
                        break;

                    //ERP
                    case "ERP":
                        if ((DateTime)cp.DateExpire > person.ERPExpireDate || person.ERPExpireDate == null)
                        {
                            person.ERPExpireDate = cp.DateExpire;
                            person.ERPIssueDate = cp.DateIssue;
                        }
                        break;
                    //HF
                    case "HF":
                        if ((DateTime)cp.DateExpire > person.HFExpireDate || person.HFExpireDate == null)
                        {
                            person.HFExpireDate = cp.DateExpire;
                            person.HFIssueDate = cp.DateIssue;
                        }
                        break;
                    //MEL/CDL
                    case "MEL/CDL":
                        if ((DateTime)cp.DateExpire > person.MELExpireDate || person.MELExpireDate == null)
                        {
                            person.MELExpireDate = cp.DateExpire;
                            person.MELIssueDate = cp.DateIssue;
                        }
                        break;
                    //METEOROLOGY
                    case "METEOROLOGY":
                        if ((DateTime)cp.DateExpire > person.METExpireDate || person.METExpireDate == null)
                        {
                            person.METExpireDate = cp.DateExpire;
                            person.METIssueDate = cp.DateIssue;
                        }
                        break;
                    //PERFORMANCE
                    case "PERFORMANCE":
                        if ((DateTime)cp.DateExpire > person.PERExpireDate || person.PERExpireDate == null)
                        {
                            person.PERExpireDate = cp.DateExpire;
                            person.PERIssueDate = cp.DateIssue;
                        }
                        break;
                    //RADIO COMMUNICATION
                    case "RADIO COMMUNICATION":
                        if ((DateTime)cp.DateExpire > person.LRCExpireDate || person.LRCExpireDate == null)
                        {
                            person.LRCExpireDate = cp.DateExpire;
                            person.LRCIssueDate = cp.DateIssue;
                        }
                        break;
                    //SITA
                    case "SITA":
                        if ((DateTime)cp.DateExpire > person.RSPExpireDate || person.RSPExpireDate == null)
                        {
                            person.RSPExpireDate = cp.DateExpire;
                            person.RSPIssueDate = cp.DateIssue;
                        }
                        break;
                    //WEIGHT AND BALANCE
                    case "WEIGHT AND BALANCE":
                        if ((DateTime)cp.DateExpire > person.MBExpireDate || person.MBExpireDate == null)
                        {
                            person.MBExpireDate = cp.DateExpire;
                            person.MBIssueDate = cp.DateIssue;
                        }
                        break;
                    //AIRSIDE SAFETY AND DRIVING
                    case "AIRSIDE SAFETY AND DRIVING":
                        if ((DateTime)cp.DateExpire > person.ASDExpireDate || person.ASDExpireDate == null)
                        {
                            person.ASDExpireDate = cp.DateExpire;
                            person.ASDIssueDate = cp.DateIssue;
                        }
                        break;
                    //AIRSIDE SAFETY AND DRIVING (IKA)
                    case "AIRSIDE SAFETY AND DRIVING (IKA)":
                        if ((DateTime)cp.DateExpire > person.ExpireDate1 || person.ExpireDate1 == null)
                        {
                            person.ExpireDate1 = cp.DateExpire;
                            person.IssueDate1 = cp.DateIssue;
                        }
                        break;
                    //GOM
                    case "GOM":
                        if ((DateTime)cp.DateExpire > person.GOMExpireDate || person.GOMExpireDate == null)
                        {
                            person.GOMExpireDate = cp.DateExpire;
                            person.GOMIssueDate = cp.DateIssue;
                        }
                        break;
                    //AIRPORT SERVICE FAMILIARIZATION
                    case "AIRPORT SERVICE FAMILIARIZATION":
                        if ((DateTime)cp.DateExpire > person.ASFExpireDate || person.ASFExpireDate == null)
                        {
                            person.ASFExpireDate = cp.DateExpire;
                            person.ASFIssueDate = cp.DateIssue;
                        }
                        break;
                    //CUSTOMER CARE
                    case "CUSTOMER CARE":
                        if ((DateTime)cp.DateExpire > person.CCExpireDate || person.CCExpireDate == null)
                        {
                            person.CCExpireDate = cp.DateExpire;
                            person.CCIssueDate = cp.DateIssue;
                        }
                        break;
                    //LOAD SHEET
                    case "LOAD SHEET":
                        if ((DateTime)cp.DateExpire > person.MBExpireDate || person.MBExpireDate == null)
                        {
                            person.MBExpireDate = cp.DateExpire;
                            person.MBIssueDate = cp.DateIssue;
                        }
                        break;
                    //PASSENGER SERVICE
                    case "PASSENGER SERVICE":
                        if ((DateTime)cp.DateExpire > person.PSExpireDate || person.PSExpireDate == null)
                        {
                            person.PSExpireDate = cp.DateExpire;
                            person.PSIssueDate = cp.DateIssue;
                        }
                        break;

                    //DRM
                    case "DRM":
                        if ((DateTime)cp.DateExpire > person.DRMExpireDate || person.DRMExpireDate == null)
                        {
                            person.DRMExpireDate = cp.DateExpire;
                            person.DRMIssueDate = cp.DateIssue;
                        }
                        break;
                    //ANNEX
                    case "ANNEX":
                        if ((DateTime)cp.DateExpire > person.ANNEXExpireDate || person.ANNEXExpireDate == null)
                        {
                            person.ANNEXExpireDate = cp.DateExpire;
                            person.ANNEXIssueDate = cp.DateIssue;
                        }
                        break;
                    //FRMS
                    case "FRMS":
                        if ((DateTime)cp.DateExpire > person.TypeAirbusExpireDate || person.TypeAirbusExpireDate == null)
                        {
                            person.TypeAirbusExpireDate = cp.DateExpire;
                            person.TypeAirbusIssueDate = cp.DateIssue;
                        }
                        break;
                    //DANGEROUS GOODS
                    case "DANGEROUS GOODS":
                        if ((DateTime)cp.DateExpire > person.DangerousGoodsExpireDate || person.DangerousGoodsExpireDate == null)
                        {
                            person.DangerousGoodsExpireDate = cp.DateExpire;
                            person.DangerousGoodsIssueDate = cp.DateIssue;
                        }
                        break;
                    //1	SEPT-P
                    case "SEPT":
                        if ((DateTime)cp.DateExpire > person.SEPTPExpireDate || person.SEPTPExpireDate == null)
                        {
                            person.SEPTPExpireDate = cp.DateExpire;
                            person.SEPTPIssueDate = cp.DateIssue;
                        }
                        break;
                    //2   SEPT - T
                    case "ANNUAL SEPT":
                        if ((DateTime)cp.DateExpire > person.SEPTExpireDate || person.SEPTExpireDate == null)
                        {
                            person.SEPTExpireDate = cp.DateExpire;
                            person.SEPTIssueDate = cp.DateIssue;
                        }
                        break;
                    //4	CRM
                    case "CRM":
                        if ((DateTime)cp.DateExpire > person.UpsetRecoveryTrainingExpireDate || person.UpsetRecoveryTrainingExpireDate == null)
                        {
                            person.UpsetRecoveryTrainingExpireDate = cp.DateExpire;
                            person.UpsetRecoveryTrainingIssueDate = cp.DateIssue;
                        }
                        break;
                    //5	CCRM
                    case "CCRM":
                        if ((DateTime)cp.DateExpire > person.CCRMExpireDate || person.CCRMExpireDate == null)
                        {
                            person.CCRMExpireDate = cp.DateExpire;
                            person.CCRMIssueDate = cp.DateIssue;
                        }
                        break;
                    //6	SMS
                    case "SMS":
                        if ((DateTime)cp.DateExpire > person.SMSExpireDate || person.SMSExpireDate == null)
                        {
                            person.SMSExpireDate = cp.DateExpire;
                            person.SMSIssueDate = cp.DateIssue;
                        }
                        break;
                    //7	AV-SEC
                    case "AVIATION SECURITY":
                        if ((DateTime)cp.DateExpire > person.AviationSecurityExpireDate || person.AviationSecurityExpireDate == null)
                        {
                            person.AviationSecurityExpireDate = cp.DateExpire;
                            person.AviationSecurityIssueDate = cp.DateIssue;
                        }
                        break;
                    //8	COLD-WX
                    case "COLD WEATHER OPERATION":
                        if ((DateTime)cp.DateExpire > person.ColdWeatherOperationExpireDate || person.ColdWeatherOperationExpireDate == null)
                        {
                            person.ColdWeatherOperationExpireDate = cp.DateExpire;
                            person.ColdWeatherOperationIssueDate = cp.DateIssue;
                        }
                        break;
                    //9	HOT-WX
                    case "HOT WEATHER OPERATION":
                        if ((DateTime)cp.DateExpire > person.HotWeatherOperationExpireDate || person.HotWeatherOperationExpireDate == null)
                        {
                            person.HotWeatherOperationExpireDate = cp.DateExpire;
                            person.HotWeatherOperationIssueDate = cp.DateIssue;
                        }
                        break;
                    //10	FIRSTAID
                    case "FIRST AID":
                        if ((DateTime)cp.DateExpire > person.FirstAidExpireDate || person.FirstAidExpireDate == null)
                        {
                            person.FirstAidExpireDate = cp.DateExpire;
                            person.FirstAidIssueDate = cp.DateIssue;
                        }
                        break;
                    ////lpc
                    //case 100:
                    //    if ((DateTime)cp.DateExpire > person.ProficiencyValidUntil || person.ProficiencyValidUntil == null)
                    //    {
                    //        person.ProficiencyValidUntil = cp.DateExpire;
                    //        person.ProficiencyCheckDate = cp.DateIssue;
                    //    }
                    //    break;
                    ////opc
                    //case 101:
                    //    if ((DateTime)cp.DateExpire > person.ProficiencyValidUntilOPC || person.ProficiencyValidUntilOPC == null)
                    //    {
                    //        person.ProficiencyValidUntilOPC = cp.DateExpire;
                    //        person.ProficiencyCheckDateOPC = cp.DateIssue;
                    //    }
                    //    break;
                    ////lpr
                    //case 102:
                    //    if ((DateTime)cp.DateExpire > person.ICAOLPRValidUntil || person.ICAOLPRValidUntil == null)
                    //    {
                    //        person.ICAOLPRValidUntil = cp.DateExpire;
                    //        // person.ProficiencyCheckDateOPC = cp.DateIssue;
                    //    }
                    //    break;

                    //grt
                    case "GRT":
                        if ((DateTime)cp.DateExpire > person.DateCaoCardExpire || person.DateCaoCardExpire == null)
                        {
                            person.DateCaoCardExpire = cp.DateExpire;
                            person.DateCaoCardIssue = cp.DateIssue;
                        }
                        break;
                    //recurrent
                    case "RECURRENT 737":
                        if ((DateTime)cp.DateExpire > person.Type737ExpireDate || person.Type737ExpireDate == null)
                        {
                            person.Type737ExpireDate = cp.DateExpire;
                            person.Type737IssueDate = cp.DateIssue;
                        }
                        break;
                    //fmt
                    case "FMT":
                        if ((DateTime)cp.DateExpire > person.EGPWSExpireDate || person.EGPWSExpireDate == null)
                        {
                            person.EGPWSExpireDate = cp.DateExpire;
                            person.EGPWSIssueDate = cp.DateIssue;
                        }
                        break;
                    case "FMTD":
                        if ((DateTime)cp.DateExpire > person.FMTDExpireDate || person.FMTDExpireDate == null)
                        {
                            person.FMTDExpireDate = cp.DateExpire;
                            person.FMTDIssueDate = cp.DateIssue;
                        }
                        break;
                    case "LINE":
                        if ((DateTime)cp.DateExpire > person.LineExpireDate || person.LineExpireDate == null)
                        {
                            person.LineExpireDate = cp.DateExpire;
                            person.LineIssueDate = cp.DateIssue;
                        }
                        break;
                    default:
                        break;
                }
            }





            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }

        //2025-01-05
        public async Task<DataResponse> UpdateCoursePeopleStatusAll(CoursePeopleStatusViewModel dto)
        {


            var course = await context.ViewCourseNews.Where(q => q.Id == dto.CourseId).FirstOrDefaultAsync();
            var _crs = await context.Courses.FirstOrDefaultAsync(q => q.Id == dto.CourseId);
            List<ViewCourseNew> subjects_views = await context.ViewCourseNews.Where(q => q.ParentId == dto.CourseId).ToListAsync();
            List<Course> subjects = await context.Courses.Where(q => q.ParentId == dto.CourseId).ToListAsync();
            var subject_ids = subjects.Select(q => (Nullable<int>)q.Id).ToList();
            var subjects_cer_type_ids = subjects_views.Select(q => q.CertificateTypeId).ToList();





            _crs.StatusId = 3;
            foreach (var s in subjects)
                s.StatusId = 3;

            List<ViewCoursePeople> view_cps = new List<ViewCoursePeople>();
            List<CoursePeople> cps = new List<CoursePeople>();
            List<ViewCoursePeople> view_cps_subjects = new List<ViewCoursePeople>();
            List<CoursePeople> cps_subjects = new List<CoursePeople>();
            if (dto.PersonIds != null && dto.PersonIds.Any())
            {
                view_cps = await context.ViewCoursePeoples.Where(q => q.CourseId == dto.CourseId && dto.PersonIds.Contains(q.PersonId)).ToListAsync();
                cps = await context.CoursePeoples.Where(q => q.CourseId == dto.CourseId && dto.PersonIds.Contains(q.PersonId)).ToListAsync();
                if (subjects.Count > 0)
                {
                    view_cps_subjects = await context.ViewCoursePeoples.Where(q => q.ParentId == dto.CourseId && dto.PersonIds.Contains(q.PersonId)).ToListAsync();
                    var _ids = view_cps_subjects.Select(q => q.Id).ToList();
                    cps_subjects = await context.CoursePeoples.Where(q => _ids.Contains(q.Id) && dto.PersonIds.Contains(q.PersonId)).ToListAsync();

                }

            }
            else
            {
                view_cps = await context.ViewCoursePeoples.Where(q => q.CourseId == dto.CourseId).ToListAsync();
                cps = await context.CoursePeoples.Where(q => q.CourseId == dto.CourseId).ToListAsync();
                if (subjects.Count > 0)
                {
                    view_cps_subjects = await context.ViewCoursePeoples.Where(q => q.ParentId == dto.CourseId).ToListAsync();
                    var _ids = view_cps_subjects.Select(q => q.Id).ToList();
                    cps_subjects = await context.CoursePeoples.Where(q => _ids.Contains(q.Id)).ToListAsync();
                }

            }

            var person_ids = cps.Select(q => (int)q.PersonId).ToList();

            var profiles = await context.ViewProfiles.Where(q => person_ids.Contains(q.PersonId)).ToListAsync();

            var certificate_histories = await context.CertificateHistories.Where(q => q.CourseId == dto.CourseId).ToListAsync();
            var certificate_histories_last = await context.ViewCertificateHistoryRankeds.Where(q => person_ids.Contains(q.PersonId) && q.CertificateTypeId != null && q.CertificateTypeId == course.CertificateTypeId).ToListAsync();
            var subjects_certificate_histories = await context.CertificateHistories.Where(q => subject_ids.Contains(q.CourseId)).ToListAsync();
            var subjects_certificate_histories_last = await context.ViewCertificateHistoryRankeds.Where(q =>
               q.CertificateTypeId != null && person_ids.Contains(q.PersonId) && subjects_cer_type_ids.Contains(q.CertificateTypeId)).ToListAsync();
            var certificate_type = await context.CertificateTypes.Where(q => q.Id == course.CertificateTypeId).FirstOrDefaultAsync();
            var subjects_cer_types = await context.CertificateTypes.Where(q => subjects_cer_type_ids.Contains(q.Id)).ToListAsync();

            var _date_issue = course.Date_Sign_Director != null ? ((DateTime)course.Date_Sign_Director).Date : ((DateTime)course.DateEnd).Date;
            var _interval = course.Interval;
            if (_interval == null)
                _interval = 0;
            if (course.Continual == true)
                _interval = 1200;



            //Main Course
            foreach (var cp in cps)
            {

                var _date_exire = _date_issue.AddMonths((int)_interval).AddMonths(1);

                _date_exire = new DateTime(_date_exire.Year, _date_exire.Month, 1);
                _date_exire = _date_exire.AddDays(-1);
                if (_interval != 1200)
                {
                    var last_history = certificate_histories_last.Where(q => q.PersonId == cp.PersonId).FirstOrDefault();
                    if (last_history != null && last_history.DateExpire != null && last_history.DateExpire > _date_exire)
                    {
                        var lh = (DateTime)last_history.DateExpire;
                        _date_exire = new DateTime(lh.Year, lh.Month, 1);
                        _date_exire = _date_exire.AddDays(-1);
                    }
                }

                var vcp = view_cps.FirstOrDefault(q => q.Id == cp.Id);
                var cp_subjects = view_cps_subjects.Where(q => q.PersonId == cp.PersonId).ToList();
                int statusId = 0;
                if (cp_subjects.Any())
                {
                    statusId = cp_subjects.Where(q => q.Presence != 100).Any() ? 0 : 1;
                }
                else
                    statusId = vcp.Presence == 100 ? 1 : 0;
                var profile = profiles.FirstOrDefault(q => q.PersonId == cp.PersonId);
                var not_applicable = context.CourseTypeApplicables.Where(q => q.IsApplicable == false && (q.TrainingGroup == profile.JobGroupRoot || q.TrainingGroup == profile.PostRoot)
                 && q.CourseTypeId == course.CourseTypeId).FirstOrDefault();
                var history = certificate_histories.FirstOrDefault(q => q.PersonId == cp.PersonId);

                if (history != null)
                    context.CertificateHistories.Remove(history);

                if (statusId != 1)
                {
                    cp.DateIssue = null;
                    cp.DateExpire = null;
                    cp.CertificateNo = null;

                    //remove record from history



                }
                else
                {
                    cp.DateExpire = _interval == 1200 ? null : (Nullable<DateTime>)_date_exire;
                    cp.DateIssue = _date_issue;
                    cp.CertificateNo = cp.Id.ToString();


                    //add record to history
                    context.CertificateHistories.Add(new CertificateHistory()
                    {
                        CourseId = dto.CourseId,
                        CertificateType = certificate_type != null ? (!string.IsNullOrEmpty(certificate_type.IssueField) ? certificate_type.IssueField : certificate_type.ExpireField) : "", //field_map.Identifier,
                        DateCreate = DateTime.Now,
                        DateExpire = not_applicable == null ? cp.DateExpire : null,
                        DateIssue = cp.DateIssue,
                        PersonId = (int)cp.PersonId,
                        Remark = "Update Course Result"

                    });
                }

                cp.StatusId = statusId;
                //cp.StatusRemark = dto.Remark;
                if (statusId == 1 && (cp.DateIssue != null || cp.DateExpire != null) && /*field_map.DbFieldIssue!=null*/certificate_type != null)
                {
                    var cmd_upd_part = new List<string>();

                    if (cp.DateIssue != null && !string.IsNullOrEmpty(certificate_type.IssueField))
                        cmd_upd_part.Add(/*field_map.DbFieldIssue*/certificate_type.IssueField + "='" + ((DateTime)cp.DateIssue).ToString("yyyy-MM-dd") + "' ");
                    if (cp.DateExpire != null && !string.IsNullOrEmpty(certificate_type.IssueField))
                        cmd_upd_part.Add(/*field_map.DbFieldExpire*/certificate_type.ExpireField + "='" + ((DateTime)cp.DateExpire).ToString("yyyy-MM-dd") + "' ");
                    if (cp.DateExpire == null && !string.IsNullOrEmpty(certificate_type.IssueField))
                        cmd_upd_part.Add(/*field_map.DbFieldExpire*/certificate_type.ExpireField + "=null ");

                    var cmd_upd = "Update Person set "
                        + string.Join(",", cmd_upd_part)
                        + " Where id=" + cp.PersonId;

                    var i = context.Database.ExecuteSqlCommand(cmd_upd);

                }


            }


            //subjects
            foreach (var scp in cps_subjects)
            {
                var cp = cps.FirstOrDefault(q => q.PersonId == scp.PersonId);
                if (cp.StatusId != 1)
                    continue;

                var sbj = subjects_views.FirstOrDefault(q => q.Id == scp.CourseId);
                var vscp = view_cps_subjects.FirstOrDefault(q => q.Id == scp.Id);
                var sbj_certificate_type = subjects_cer_types.FirstOrDefault(q => q.Id == sbj.CertificateTypeId);
                var statusId = vscp.Presence == 100 ? 1 : 0;
                var profile = profiles.FirstOrDefault(q => q.PersonId == scp.PersonId);
                var not_applicable = context.CourseTypeApplicables.Where(q => q.IsApplicable == false && (q.TrainingGroup == profile.JobGroupRoot || q.TrainingGroup == profile.PostRoot)
                 && q.CourseTypeId == sbj.CourseTypeId).FirstOrDefault();
                var history = subjects_certificate_histories.FirstOrDefault(q => q.PersonId == scp.PersonId && q.CourseId == sbj.Id);


                if (history != null)
                    context.CertificateHistories.Remove(history);

                if (statusId != 1)
                {
                    scp.DateIssue = null;
                    scp.DateExpire = null;
                    scp.CertificateNo = null;

                    //remove record from history


                }
                else
                {
                    var sbj_interval = sbj.Interval;
                    if (sbj_interval == null)
                        sbj_interval = 0;
                    if (sbj.Continual == true)
                        sbj_interval = 1200;
                    var sbj_date_exire = _date_issue.AddMonths((int)sbj_interval).AddMonths(1);

                    sbj_date_exire = new DateTime(sbj_date_exire.Year, sbj_date_exire.Month, 1);
                    sbj_date_exire = sbj_date_exire.AddDays(-1);

                    /////////////////////////////////
                    ///var _date_exire = _date_issue.AddMonths((int)_interval).AddMonths(1);


                    if (_interval != 1200)
                    {
                        var last_history = subjects_certificate_histories_last.Where(q => q.PersonId == scp.PersonId && q.CertificateTypeId == sbj.CertificateTypeId && q.DateIssue != _date_issue).FirstOrDefault();

                        if (last_history != null && last_history.DateExpire != null && last_history.DateExpire > _date_issue && last_history.DateIssue != null
                            && ((DateTime)last_history.DateIssue).Date != _date_issue.Date)
                        {
                            var dif = ((DateTime)last_history.DateExpire).Subtract(_date_issue).Days;


                            sbj_date_exire = sbj_date_exire.AddDays(dif);
                        }
                    }

                    /////////////////////////////

                    scp.DateExpire = _interval == 1200 ? null : (Nullable<DateTime>)sbj_date_exire;
                    scp.DateIssue = _date_issue;
                    scp.CertificateNo = vscp.Id.ToString();


                    //add record to history
                    context.CertificateHistories.Add(new CertificateHistory()
                    {
                        CourseId = sbj.Id,
                        CertificateType = sbj_certificate_type != null ? (!string.IsNullOrEmpty(sbj_certificate_type.IssueField) ? sbj_certificate_type.IssueField : sbj_certificate_type.ExpireField) : "", //field_map.Identifier,
                        DateCreate = DateTime.Now,
                        DateExpire = not_applicable == null ? vscp.DateExpire : null,
                        DateIssue = vscp.DateIssue,
                        PersonId = (int)vscp.PersonId,
                        Remark = "Update Course Result"

                    });
                }

                scp.StatusId = statusId;
                //cp.StatusRemark = dto.Remark;
                if (statusId == 1 && (scp.DateIssue != null || scp.DateExpire != null) && /*field_map.DbFieldIssue!=null*/sbj_certificate_type != null)
                {
                    var cmd_upd_part = new List<string>();

                    if (scp.DateIssue != null && !string.IsNullOrEmpty(sbj_certificate_type.IssueField))
                        cmd_upd_part.Add(/*field_map.DbFieldIssue*/sbj_certificate_type.IssueField + "='" + ((DateTime)scp.DateIssue).ToString("yyyy-MM-dd") + "' ");
                    if (scp.DateExpire != null && !string.IsNullOrEmpty(sbj_certificate_type.IssueField))
                        cmd_upd_part.Add(/*field_map.DbFieldExpire*/sbj_certificate_type.ExpireField + "='" + ((DateTime)scp.DateExpire).ToString("yyyy-MM-dd") + "' ");
                    if (scp.DateExpire == null && !string.IsNullOrEmpty(sbj_certificate_type.IssueField))
                        cmd_upd_part.Add(/*field_map.DbFieldExpire*/sbj_certificate_type.ExpireField + "=null ");

                    var cmd_upd = "Update Person set "
                        + string.Join(",", cmd_upd_part)
                        + " Where id=" + scp.PersonId;

                    var i = context.Database.ExecuteSqlCommand(cmd_upd);

                }



            }













            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }



        public async Task<DataResponse> SaveCertificateAtlas(ViewModels.CertificateViewModel dto)
        {
            var _dateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
            var _dateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;

            Course entity = await context.Courses.Where(q => q.DateStart == _dateStart && q.DateEnd == _dateEnd && q.CourseTypeId == dto.CourseTypeId
            && q.OrganizationId == dto.OrganizationId).FirstOrDefaultAsync();

            if (entity == null)
            {
                entity = new Course();
                context.Courses.Add(entity);
                entity.CourseTypeId = dto.CourseTypeId;
                entity.DateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
                entity.DateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;
                entity.Instructor = dto.Instructor;
                entity.Location = dto.Location;
                entity.OrganizationId = dto.OrganizationId;
                entity.Duration = dto.Duration;
                entity.DurationUnitId = dto.DurationUnitId;
                entity.Remark = dto.Remark;
                entity.TrainingDirector = dto.TrainingDirector;
                entity.Title = dto.Title;
                entity.Recurrent = dto.Recurrent;
                entity.Interval = dto.Interval;
                entity.CalanderTypeId = dto.CalanderTypeId;
                entity.IsGeneral = dto.IsGeneral;
                entity.CustomerId = dto.CustomerId;
                entity.No = dto.No;
                entity.IsNotificationEnabled = dto.IsNotificationEnabled;
                entity.StatusId = 3;
            }
            if (dto.PersonId != null)
            {
                var cp = await context.CoursePeoples.Where(q => q.PersonId == dto.PersonId && q.CourseId == entity.Id).FirstOrDefaultAsync();
                if (cp == null)
                {
                    cp = new CoursePeople()
                    {
                        PersonId = dto.PersonId,
                        StatusId = 1,
                        DateStatus = DateTime.Now,
                        DateExpire = (DateTime)DateObject.ConvertToDate(dto.DateExpire).Date,
                        DateIssue = (DateTime)DateObject.ConvertToDate(dto.DateIssue).Date,
                        CertificateNo = dto.CertificateNo,
                    };
                    entity.CoursePeoples.Add(cp);
                }
                else
                {
                    cp.DateExpire = (DateTime)DateObject.ConvertToDate(dto.DateExpire).Date;
                    cp.DateIssue = (DateTime)DateObject.ConvertToDate(dto.DateIssue).Date;
                    cp.CertificateNo = dto.CertificateNo;
                    cp.StatusId = 1;
                }
                //////////////////////////
                var person = await context.People.Where(q => q.Id == cp.PersonId).FirstOrDefaultAsync();
                var ct = await context.ViewCourseTypes.Where(q => q.Id == dto.CourseTypeId).FirstOrDefaultAsync();
                //12-03
                if (ct != null)
                {
                    switch (ct.CertificateType)
                    {


                        //CYBER SECURITY
                        case "RECURRENT":
                            if ((DateTime)cp.DateExpire > person.ExpireDate2 || person.ExpireDate2 == null)
                            {
                                person.ExpireDate2 = cp.DateExpire;
                                person.IssueDate2 = cp.DateIssue;
                            }
                            break;

                        case "OM-A":
                            if ((DateTime)cp.DateExpire > person.OMA1ExpireDate || person.OMA1ExpireDate == null)
                            {
                                person.OMA1ExpireDate = cp.DateExpire;
                                person.OMA1IssueDate = cp.DateIssue;
                            }
                            break;
                        //OM-B AN-26
                        case "OM-B AN-26":
                            if ((DateTime)cp.DateExpire > person.OMB1ExpireDate || person.OMB1ExpireDate == null)
                            {
                                person.OMB1ExpireDate = cp.DateExpire;
                                person.OMB1IssueDate = cp.DateIssue;
                            }
                            break;
                        //OM-C AN-26
                        case "OM-C AN-26":
                            if ((DateTime)cp.DateExpire > person.OMC1ExpireDate || person.OMC1ExpireDate == null)
                            {
                                person.OMC1ExpireDate = cp.DateExpire;
                                person.OMC1IssueDate = cp.DateIssue;
                            }
                            break;
                        case "OM-C MEP":
                            if ((DateTime)cp.DateExpire > person.OMC2ExpireDate || person.OMC2ExpireDate == null)
                            {
                                person.OMC2ExpireDate = cp.DateExpire;
                                person.OMC2IssueDate = cp.DateIssue;
                            }
                            break;
                        case "OM-C":
                            if ((DateTime)cp.DateExpire > person.OMC1ExpireDate || person.OMC1ExpireDate == null)
                            {
                                person.OMC1ExpireDate = cp.DateExpire;
                                person.OMC1IssueDate = cp.DateIssue;
                            }
                            break;
                        case "UPRT":
                            if ((DateTime)cp.DateExpire > person.UPRTExpireDate || person.UPRTExpireDate == null)
                            {
                                person.UPRTExpireDate = cp.DateExpire;
                                person.UPRTIssueDate = cp.DateIssue;
                            }
                            break;
                        case "RAMP INSPECTION":
                            if ((DateTime)cp.DateExpire > person.RampExpireDate || person.RampExpireDate == null)
                            {
                                person.RampExpireDate = cp.DateExpire;
                                person.RampIssueDate = cp.DateIssue;
                            }
                            break;

                        case "A/C SYSTEM REVIEW MEP":
                            if ((DateTime)cp.DateExpire > person.ACExpireDate || person.ACExpireDate == null)
                            {
                                person.ACExpireDate = cp.DateExpire;
                                person.ACIssueDate = cp.DateIssue;
                            }
                            break;

                        case "AIR CREW REGULATION":
                            if ((DateTime)cp.DateExpire > person.AirCrewExpireDate || person.AirCrewExpireDate == null)
                            {
                                person.AirCrewExpireDate = cp.DateExpire;
                                person.AirCrewIssueDate = cp.DateIssue;
                            }
                            break;
                        case "AIROPS REGULATION":
                            if ((DateTime)cp.DateExpire > person.AirOpsExpireDate || person.AirOpsExpireDate == null)
                            {
                                person.AirOpsExpireDate = cp.DateExpire;
                                person.AirOpsIssueDate = cp.DateIssue;
                            }
                            break;
                        case "SOP MEP":
                            if ((DateTime)cp.DateExpire > person.SOPExpireDate || person.SOPExpireDate == null)
                            {
                                person.SOPExpireDate = cp.DateExpire;
                                person.SOPIssueDate = cp.DateIssue;
                            }
                            break;
                        case "DIFF TRAINING PA31":
                            if ((DateTime)cp.DateExpire > person.Diff31ExpireDate || person.Diff31ExpireDate == null)
                            {
                                person.Diff31ExpireDate = cp.DateExpire;
                                person.Diff31IssueDate = cp.DateIssue;
                            }
                            break;
                        case "DIFF TRAINING PA34":
                            if ((DateTime)cp.DateExpire > person.Diff34ExpireDate || person.Diff34ExpireDate == null)
                            {
                                person.Diff34ExpireDate = cp.DateExpire;
                                person.Diff34IssueDate = cp.DateIssue;
                            }
                            break;

                        case "AERIAL MAPPING":
                            if ((DateTime)cp.DateExpire > person.MapExpireDate || person.MapExpireDate == null)
                            {
                                person.MapExpireDate = cp.DateExpire;
                                person.MapIssueDate = cp.DateIssue;
                            }
                            break;
                        case "COM RES":
                            if ((DateTime)cp.DateExpire > person.ComResExpireDate || person.ComResExpireDate == null)
                            {
                                person.ComResExpireDate = cp.DateExpire;
                                person.ComResIssueDate = cp.DateIssue;
                            }
                            break;

                        case "MEL":
                            if ((DateTime)cp.DateExpire > person.MELExpireDate || person.MELExpireDate == null)
                            {
                                person.MELExpireDate = cp.DateExpire;
                                person.MELIssueDate = cp.DateIssue;
                            }
                            break;

                        //ERP
                        case "ERP":
                            if ((DateTime)cp.DateExpire > person.ERPExpireDate || person.ERPExpireDate == null)
                            {
                                person.ERPExpireDate = cp.DateExpire;
                                person.ERPIssueDate = cp.DateIssue;
                            }
                            break;
                        //HF
                        case "HF":
                            if ((DateTime)cp.DateExpire > person.HFExpireDate || person.HFExpireDate == null)
                            {
                                person.HFExpireDate = cp.DateExpire;
                                person.HFIssueDate = cp.DateIssue;
                            }
                            break;
                        //MEL/CDL
                        case "MEL/CDL":
                            if ((DateTime)cp.DateExpire > person.MELExpireDate || person.MELExpireDate == null)
                            {
                                person.MELExpireDate = cp.DateExpire;
                                person.MELIssueDate = cp.DateIssue;
                            }
                            break;
                        //METEOROLOGY
                        case "METEOROLOGY":
                            if ((DateTime)cp.DateExpire > person.METExpireDate || person.METExpireDate == null)
                            {
                                person.METExpireDate = cp.DateExpire;
                                person.METIssueDate = cp.DateIssue;
                            }
                            break;
                        //PERFORMANCE
                        case "PERFORMANCE":
                            if ((DateTime)cp.DateExpire > person.PERExpireDate || person.PERExpireDate == null)
                            {
                                person.PERExpireDate = cp.DateExpire;
                                person.PERIssueDate = cp.DateIssue;
                            }
                            break;
                        //RADIO COMMUNICATION
                        case "RADIO COMMUNICATION":
                            if ((DateTime)cp.DateExpire > person.LRCExpireDate || person.LRCExpireDate == null)
                            {
                                person.LRCExpireDate = cp.DateExpire;
                                person.LRCIssueDate = cp.DateIssue;
                            }
                            break;
                        //SITA
                        case "SITA":
                            if ((DateTime)cp.DateExpire > person.RSPExpireDate || person.RSPExpireDate == null)
                            {
                                person.RSPExpireDate = cp.DateExpire;
                                person.RSPIssueDate = cp.DateIssue;
                            }
                            break;
                        //WEIGHT AND BALANCE
                        case "WEIGHT AND BALANCE":
                            if ((DateTime)cp.DateExpire > person.MBExpireDate || person.MBExpireDate == null)
                            {
                                person.MBExpireDate = cp.DateExpire;
                                person.MBIssueDate = cp.DateIssue;
                            }
                            break;
                        //AIRSIDE SAFETY AND DRIVING
                        case "AIRSIDE SAFETY AND DRIVING":
                            if ((DateTime)cp.DateExpire > person.ASDExpireDate || person.ASDExpireDate == null)
                            {
                                person.ASDExpireDate = cp.DateExpire;
                                person.ASDIssueDate = cp.DateIssue;
                            }
                            break;
                        //EFB
                        case "EFB":
                            if ((DateTime)cp.DateExpire > person.ExpireDate1 || person.ExpireDate1 == null)
                            {
                                person.ExpireDate1 = cp.DateExpire;
                                person.IssueDate1 = cp.DateIssue;
                            }
                            break;
                        //GOM
                        case "GOM":
                            if ((DateTime)cp.DateExpire > person.GOMExpireDate || person.GOMExpireDate == null)
                            {
                                person.GOMExpireDate = cp.DateExpire;
                                person.GOMIssueDate = cp.DateIssue;
                            }
                            break;
                        //AIRPORT SERVICE FAMILIARIZATION
                        case "AIRPORT SERVICE FAMILIARIZATION":
                            if ((DateTime)cp.DateExpire > person.ASFExpireDate || person.ASFExpireDate == null)
                            {
                                person.ASFExpireDate = cp.DateExpire;
                                person.ASFIssueDate = cp.DateIssue;
                            }
                            break;
                        //CUSTOMER CARE
                        case "CUSTOMER CARE":
                            if ((DateTime)cp.DateExpire > person.CCExpireDate || person.CCExpireDate == null)
                            {
                                person.CCExpireDate = cp.DateExpire;
                                person.CCIssueDate = cp.DateIssue;
                            }
                            break;
                        //LOAD SHEET
                        case "LOAD SHEET":
                            if ((DateTime)cp.DateExpire > person.MBExpireDate || person.MBExpireDate == null)
                            {
                                person.MBExpireDate = cp.DateExpire;
                                person.MBIssueDate = cp.DateIssue;
                            }
                            break;
                        //PASSENGER SERVICE
                        case "PASSENGER SERVICE":
                            if ((DateTime)cp.DateExpire > person.PSExpireDate || person.PSExpireDate == null)
                            {
                                person.PSExpireDate = cp.DateExpire;
                                person.PSIssueDate = cp.DateIssue;
                            }
                            break;

                        //DRM
                        case "DRM":
                            if ((DateTime)cp.DateExpire > person.DRMExpireDate || person.DRMExpireDate == null)
                            {
                                person.DRMExpireDate = cp.DateExpire;
                                person.DRMIssueDate = cp.DateIssue;
                            }
                            break;
                        //ANNEX
                        case "ANNEX":
                            if ((DateTime)cp.DateExpire > person.ANNEXExpireDate || person.ANNEXExpireDate == null)
                            {
                                person.ANNEXExpireDate = cp.DateExpire;
                                person.ANNEXIssueDate = cp.DateIssue;
                            }
                            break;
                        //FRMS
                        case "FRMS":
                            if ((DateTime)cp.DateExpire > person.TypeAirbusExpireDate || person.TypeAirbusExpireDate == null)
                            {
                                person.TypeAirbusExpireDate = cp.DateExpire;
                                person.TypeAirbusIssueDate = cp.DateIssue;
                            }
                            break;
                        //DANGEROUS GOODS
                        case "DANGEROUS GOODS":
                            if ((DateTime)cp.DateExpire > person.DangerousGoodsExpireDate || person.DangerousGoodsExpireDate == null)
                            {
                                person.DangerousGoodsExpireDate = cp.DateExpire;
                                person.DangerousGoodsIssueDate = cp.DateIssue;
                            }
                            break;
                        //1	SEPT-P
                        case "SEPT":
                            if ((DateTime)cp.DateExpire > person.SEPTPExpireDate || person.SEPTPExpireDate == null)
                            {
                                person.SEPTPExpireDate = cp.DateExpire;
                                person.SEPTPIssueDate = cp.DateIssue;
                            }
                            break;
                        //2   SEPT - T
                        case "ANNUAL SEPT":
                            if ((DateTime)cp.DateExpire > person.SEPTExpireDate || person.SEPTExpireDate == null)
                            {
                                person.SEPTExpireDate = cp.DateExpire;
                                person.SEPTIssueDate = cp.DateIssue;
                            }
                            break;
                        //4	CRM
                        case "CRM":
                            if ((DateTime)cp.DateExpire > person.UpsetRecoveryTrainingExpireDate || person.UpsetRecoveryTrainingExpireDate == null)
                            {
                                person.UpsetRecoveryTrainingExpireDate = cp.DateExpire;
                                person.UpsetRecoveryTrainingIssueDate = cp.DateIssue;
                            }
                            break;
                        //5	CCRM
                        case "CCRM":
                            if ((DateTime)cp.DateExpire > person.CCRMExpireDate || person.CCRMExpireDate == null)
                            {
                                person.CCRMExpireDate = cp.DateExpire;
                                person.CCRMIssueDate = cp.DateIssue;
                            }
                            break;
                        //6	SMS
                        case "SMS":
                            if ((DateTime)cp.DateExpire > person.SMSExpireDate || person.SMSExpireDate == null)
                            {
                                person.SMSExpireDate = cp.DateExpire;
                                person.SMSIssueDate = cp.DateIssue;
                            }
                            break;
                        //7	AV-SEC
                        case "AVIATION SECURITY":
                            if ((DateTime)cp.DateExpire > person.AviationSecurityExpireDate || person.AviationSecurityExpireDate == null)
                            {
                                person.AviationSecurityExpireDate = cp.DateExpire;
                                person.AviationSecurityIssueDate = cp.DateIssue;
                            }
                            break;
                        //8	COLD-WX
                        case "COLD WEATHER OPERATION":
                            if ((DateTime)cp.DateExpire > person.ColdWeatherOperationExpireDate || person.ColdWeatherOperationExpireDate == null)
                            {
                                person.ColdWeatherOperationExpireDate = cp.DateExpire;
                                person.ColdWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //9	HOT-WX
                        case "HOT WEATHER OPERATION":
                            if ((DateTime)cp.DateExpire > person.HotWeatherOperationExpireDate || person.HotWeatherOperationExpireDate == null)
                            {
                                person.HotWeatherOperationExpireDate = cp.DateExpire;
                                person.HotWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //10	FIRSTAID
                        case "FIRST AID":
                            if ((DateTime)cp.DateExpire > person.FirstAidExpireDate || person.FirstAidExpireDate == null)
                            {
                                person.FirstAidExpireDate = cp.DateExpire;
                                person.FirstAidIssueDate = cp.DateIssue;
                            }
                            break;
                        ////lpc
                        //case 100:
                        //    if ((DateTime)cp.DateExpire > person.ProficiencyValidUntil || person.ProficiencyValidUntil == null)
                        //    {
                        //        person.ProficiencyValidUntil = cp.DateExpire;
                        //        person.ProficiencyCheckDate = cp.DateIssue;
                        //    }
                        //    break;
                        ////opc
                        //case 101:
                        //    if ((DateTime)cp.DateExpire > person.ProficiencyValidUntilOPC || person.ProficiencyValidUntilOPC == null)
                        //    {
                        //        person.ProficiencyValidUntilOPC = cp.DateExpire;
                        //        person.ProficiencyCheckDateOPC = cp.DateIssue;
                        //    }
                        //    break;
                        ////lpr
                        //case 102:
                        //    if ((DateTime)cp.DateExpire > person.ICAOLPRValidUntil || person.ICAOLPRValidUntil == null)
                        //    {
                        //        person.ICAOLPRValidUntil = cp.DateExpire;
                        //        // person.ProficiencyCheckDateOPC = cp.DateIssue;
                        //    }
                        //    break;

                        //grt
                        case "GRT":
                            if ((DateTime)cp.DateExpire > person.DateCaoCardExpire || person.DateCaoCardExpire == null)
                            {
                                person.DateCaoCardExpire = cp.DateExpire;
                                person.DateCaoCardIssue = cp.DateIssue;
                            }
                            break;
                        //recurrent
                        case "RECURRENT 737":
                            if ((DateTime)cp.DateExpire > person.Type737ExpireDate || person.Type737ExpireDate == null)
                            {
                                person.Type737ExpireDate = cp.DateExpire;
                                person.Type737IssueDate = cp.DateIssue;
                            }
                            break;
                        //fmt
                        case "FMT":
                            if ((DateTime)cp.DateExpire > person.EGPWSExpireDate || person.EGPWSExpireDate == null)
                            {
                                person.EGPWSExpireDate = cp.DateExpire;
                                person.EGPWSIssueDate = cp.DateIssue;
                            }
                            break;
                        case "FMTD":
                            if ((DateTime)cp.DateExpire > person.FMTDExpireDate || person.FMTDExpireDate == null)
                            {
                                person.FMTDExpireDate = cp.DateExpire;
                                person.FMTDIssueDate = cp.DateIssue;
                            }
                            break;
                        case "LINE":
                            if ((DateTime)cp.DateExpire > person.LineExpireDate || person.LineExpireDate == null)
                            {
                                person.LineExpireDate = cp.DateExpire;
                                person.LineIssueDate = cp.DateIssue;
                            }
                            break;
                        default:
                            break;
                    }
                    /*switch (ct.CertificateTypeId)
                    {
                        //dg
                        case 3:
                            if ((DateTime)cp.DateExpire > person.DangerousGoodsExpireDate)
                            {
                                person.DangerousGoodsExpireDate = cp.DateExpire;
                                person.DangerousGoodsIssueDate = cp.DateIssue;
                            }
                            break;
                        //1	SEPT-P
                        case 1:
                            if ((DateTime)cp.DateExpire > person.SEPTPExpireDate)
                            {
                                person.SEPTPExpireDate = cp.DateExpire;
                                person.SEPTPIssueDate = cp.DateIssue;
                            }
                            break;
                        //2   SEPT - T
                        case 2:
                            if ((DateTime)cp.DateExpire > person.SEPTExpireDate)
                            {
                                person.SEPTExpireDate = cp.DateExpire;
                                person.SEPTIssueDate = cp.DateIssue;
                            }
                            break;
                        //4	CRM
                        case 4:
                            if ((DateTime)cp.DateExpire > person.UpsetRecoveryTrainingExpireDate)
                            {
                                person.UpsetRecoveryTrainingExpireDate = cp.DateExpire;
                                person.UpsetRecoveryTrainingIssueDate = cp.DateIssue;
                            }
                            break;
                        //5	CCRM
                        case 5:
                            if ((DateTime)cp.DateExpire > person.CCRMExpireDate)
                            {
                                person.CCRMExpireDate = cp.DateExpire;
                                person.CCRMIssueDate = cp.DateIssue;
                            }
                            break;
                        //6	SMS
                        case 6:
                            if ((DateTime)cp.DateExpire > person.SMSExpireDate)
                            {
                                person.SMSExpireDate = cp.DateExpire;
                                person.SMSIssueDate = cp.DateIssue;
                            }
                            break;
                        //7	AV-SEC
                        case 7:
                            if ((DateTime)cp.DateExpire > person.AviationSecurityExpireDate)
                            {
                                person.AviationSecurityExpireDate = cp.DateExpire;
                                person.AviationSecurityIssueDate = cp.DateIssue;
                            }
                            break;
                        //8	COLD-WX
                        case 8:
                            if ((DateTime)cp.DateExpire > person.ColdWeatherOperationExpireDate)
                            {
                                person.ColdWeatherOperationExpireDate = cp.DateExpire;
                                person.ColdWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //9	HOT-WX
                        case 9:
                            if ((DateTime)cp.DateExpire > person.HotWeatherOperationExpireDate)
                            {
                                person.HotWeatherOperationExpireDate = cp.DateExpire;
                                person.HotWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //10	FIRSTAID
                        case 10:
                            if ((DateTime)cp.DateExpire > person.FirstAidExpireDate)
                            {
                                person.FirstAidExpireDate = cp.DateExpire;
                                person.FirstAidIssueDate = cp.DateIssue;
                            }
                            break;
                        case 105:
                            if ((DateTime)cp.DateExpire > person.LineExpireDate)
                            {
                                person.LineExpireDate = cp.DateExpire;
                                person.LineIssueDate = cp.DateIssue;
                            }
                            break;
                        default:
                            break;
                    }*/
                }


                /////////////////////////


            }




            await context.SaveChangesAsync();
            dto.Id = entity.Id;
            var result = await context.ViewCoursePeopleRankeds.Where(q => q.PersonId == dto.PersonId && q.CourseId == entity.Id).FirstOrDefaultAsync();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result,
            };
        }
        public async Task<DataResponse> UpdateCoursePeopleStatusAtlas(CoursePeopleStatusViewModel dto)
        {
            CoursePeople cp = null;

            if (dto.Id != -1)
                cp = await context.CoursePeoples.Where(q => q.Id == dto.Id).FirstOrDefaultAsync();
            else
                cp = await context.CoursePeoples.Where(q => q.PersonId == dto.PersonId && q.CourseId == dto.CourseId).FirstOrDefaultAsync();
            if (dto.StatusId != cp.StatusId)
                cp.DateStatus = DateTime.Now;

            //-1: UnKnown 0:Failed 1:Passed
            if (dto.StatusId != 1)
            {
                cp.DateIssue = null;
                cp.DateExpire = null;
                cp.CertificateNo = null;


            }
            else
            {
                cp.DateExpire = string.IsNullOrEmpty(dto.Expire) ? null : DateObject.ConvertToDate(dto.Expire).Date;
                cp.DateIssue = string.IsNullOrEmpty(dto.Issue) ? null : DateObject.ConvertToDate(dto.Issue).Date;
                cp.CertificateNo = dto.No;
                if (string.IsNullOrEmpty(cp.CertificateNo))
                    cp.CertificateNo = "FPC-" + cp.Id;

            }

            cp.StatusId = dto.StatusId;
            cp.StatusRemark = dto.Remark;

            if (dto.StatusId == 1 && cp.DateIssue != null && cp.DateExpire != null && !string.IsNullOrEmpty(cp.CertificateNo))
            {

                var person = await context.People.Where(q => q.Id == cp.PersonId).FirstOrDefaultAsync();
                var course = await context.ViewCourseNews.Where(q => q.Id == cp.CourseId).FirstOrDefaultAsync();
                switch (course.CertificateType)
                {


                    //CYBER SECURITY
                    case "RECURRENT":
                        if ((DateTime)cp.DateExpire > person.ExpireDate2 || person.ExpireDate2 == null)
                        {
                            person.ExpireDate2 = cp.DateExpire;
                            person.IssueDate2 = cp.DateIssue;
                        }
                        break;

                    case "OM-A":
                        if ((DateTime)cp.DateExpire > person.OMA1ExpireDate || person.OMA1ExpireDate == null)
                        {
                            person.OMA1ExpireDate = cp.DateExpire;
                            person.OMA1IssueDate = cp.DateIssue;
                        }
                        break;
                    //OM-B AN-26
                    case "OM-B AN-26":
                        if ((DateTime)cp.DateExpire > person.OMB1ExpireDate || person.OMB1ExpireDate == null)
                        {
                            person.OMB1ExpireDate = cp.DateExpire;
                            person.OMB1IssueDate = cp.DateIssue;
                        }
                        break;
                    //OM-C AN-26
                    case "OM-C AN-26":
                        if ((DateTime)cp.DateExpire > person.OMC1ExpireDate || person.OMC1ExpireDate == null)
                        {
                            person.OMC1ExpireDate = cp.DateExpire;
                            person.OMC1IssueDate = cp.DateIssue;
                        }
                        break;
                    case "OM-C MEP":
                        if ((DateTime)cp.DateExpire > person.OMC2ExpireDate || person.OMC2ExpireDate == null)
                        {
                            person.OMC2ExpireDate = cp.DateExpire;
                            person.OMC2IssueDate = cp.DateIssue;
                        }
                        break;
                    case "OM-C":
                        if ((DateTime)cp.DateExpire > person.OMC1ExpireDate || person.OMC1ExpireDate == null)
                        {
                            person.OMC1ExpireDate = cp.DateExpire;
                            person.OMC1IssueDate = cp.DateIssue;
                        }
                        break;
                    case "UPRT":
                        if ((DateTime)cp.DateExpire > person.UPRTExpireDate || person.UPRTExpireDate == null)
                        {
                            person.UPRTExpireDate = cp.DateExpire;
                            person.UPRTIssueDate = cp.DateIssue;
                        }
                        break;
                    case "RAMP INSPECTION":
                        if ((DateTime)cp.DateExpire > person.RampExpireDate || person.RampExpireDate == null)
                        {
                            person.RampExpireDate = cp.DateExpire;
                            person.RampIssueDate = cp.DateIssue;
                        }
                        break;

                    case "A/C SYSTEM REVIEW MEP":
                        if ((DateTime)cp.DateExpire > person.ACExpireDate || person.ACExpireDate == null)
                        {
                            person.ACExpireDate = cp.DateExpire;
                            person.ACIssueDate = cp.DateIssue;
                        }
                        break;

                    case "AIR CREW REGULATION":
                        if ((DateTime)cp.DateExpire > person.AirCrewExpireDate || person.AirCrewExpireDate == null)
                        {
                            person.AirCrewExpireDate = cp.DateExpire;
                            person.AirCrewIssueDate = cp.DateIssue;
                        }
                        break;
                    case "AIROPS REGULATION":
                        if ((DateTime)cp.DateExpire > person.AirOpsExpireDate || person.AirOpsExpireDate == null)
                        {
                            person.AirOpsExpireDate = cp.DateExpire;
                            person.AirOpsIssueDate = cp.DateIssue;
                        }
                        break;
                    case "SOP MEP":
                        if ((DateTime)cp.DateExpire > person.SOPExpireDate || person.SOPExpireDate == null)
                        {
                            person.SOPExpireDate = cp.DateExpire;
                            person.SOPIssueDate = cp.DateIssue;
                        }
                        break;
                    case "DIFF TRAINING PA31":
                        if ((DateTime)cp.DateExpire > person.Diff31ExpireDate || person.Diff31ExpireDate == null)
                        {
                            person.Diff31ExpireDate = cp.DateExpire;
                            person.Diff31IssueDate = cp.DateIssue;
                        }
                        break;
                    case "DIFF TRAINING PA34":
                        if ((DateTime)cp.DateExpire > person.Diff34ExpireDate || person.Diff34ExpireDate == null)
                        {
                            person.Diff34ExpireDate = cp.DateExpire;
                            person.Diff34IssueDate = cp.DateIssue;
                        }
                        break;

                    case "AERIAL MAPPING":
                        if ((DateTime)cp.DateExpire > person.MapExpireDate || person.MapExpireDate == null)
                        {
                            person.MapExpireDate = cp.DateExpire;
                            person.MapIssueDate = cp.DateIssue;
                        }
                        break;
                    case "COM RES":
                        if ((DateTime)cp.DateExpire > person.ComResExpireDate || person.ComResExpireDate == null)
                        {
                            person.ComResExpireDate = cp.DateExpire;
                            person.ComResIssueDate = cp.DateIssue;
                        }
                        break;

                    case "MEL":
                        if ((DateTime)cp.DateExpire > person.MELExpireDate || person.MELExpireDate == null)
                        {
                            person.MELExpireDate = cp.DateExpire;
                            person.MELIssueDate = cp.DateIssue;
                        }
                        break;

                    //ERP
                    case "ERP":
                        if ((DateTime)cp.DateExpire > person.ERPExpireDate || person.ERPExpireDate == null)
                        {
                            person.ERPExpireDate = cp.DateExpire;
                            person.ERPIssueDate = cp.DateIssue;
                        }
                        break;
                    //HF
                    case "HF":
                        if ((DateTime)cp.DateExpire > person.HFExpireDate || person.HFExpireDate == null)
                        {
                            person.HFExpireDate = cp.DateExpire;
                            person.HFIssueDate = cp.DateIssue;
                        }
                        break;
                    //MEL/CDL
                    case "MEL/CDL":
                        if ((DateTime)cp.DateExpire > person.MELExpireDate || person.MELExpireDate == null)
                        {
                            person.MELExpireDate = cp.DateExpire;
                            person.MELIssueDate = cp.DateIssue;
                        }
                        break;
                    //METEOROLOGY
                    case "METEOROLOGY":
                        if ((DateTime)cp.DateExpire > person.METExpireDate || person.METExpireDate == null)
                        {
                            person.METExpireDate = cp.DateExpire;
                            person.METIssueDate = cp.DateIssue;
                        }
                        break;
                    //PERFORMANCE
                    case "PERFORMANCE":
                        if ((DateTime)cp.DateExpire > person.PERExpireDate || person.PERExpireDate == null)
                        {
                            person.PERExpireDate = cp.DateExpire;
                            person.PERIssueDate = cp.DateIssue;
                        }
                        break;
                    //RADIO COMMUNICATION
                    case "RADIO COMMUNICATION":
                        if ((DateTime)cp.DateExpire > person.LRCExpireDate || person.LRCExpireDate == null)
                        {
                            person.LRCExpireDate = cp.DateExpire;
                            person.LRCIssueDate = cp.DateIssue;
                        }
                        break;
                    //SITA
                    case "SITA":
                        if ((DateTime)cp.DateExpire > person.RSPExpireDate || person.RSPExpireDate == null)
                        {
                            person.RSPExpireDate = cp.DateExpire;
                            person.RSPIssueDate = cp.DateIssue;
                        }
                        break;
                    //WEIGHT AND BALANCE
                    case "WEIGHT AND BALANCE":
                        if ((DateTime)cp.DateExpire > person.MBExpireDate || person.MBExpireDate == null)
                        {
                            person.MBExpireDate = cp.DateExpire;
                            person.MBIssueDate = cp.DateIssue;
                        }
                        break;
                    //AIRSIDE SAFETY AND DRIVING
                    case "AIRSIDE SAFETY AND DRIVING":
                        if ((DateTime)cp.DateExpire > person.ASDExpireDate || person.ASDExpireDate == null)
                        {
                            person.ASDExpireDate = cp.DateExpire;
                            person.ASDIssueDate = cp.DateIssue;
                        }
                        break;
                    //EFB
                    case "EFB":
                        if ((DateTime)cp.DateExpire > person.ExpireDate1 || person.ExpireDate1 == null)
                        {
                            person.ExpireDate1 = cp.DateExpire;
                            person.IssueDate1 = cp.DateIssue;
                        }
                        break;
                    //GOM
                    case "GOM":
                        if ((DateTime)cp.DateExpire > person.GOMExpireDate || person.GOMExpireDate == null)
                        {
                            person.GOMExpireDate = cp.DateExpire;
                            person.GOMIssueDate = cp.DateIssue;
                        }
                        break;
                    //AIRPORT SERVICE FAMILIARIZATION
                    case "AIRPORT SERVICE FAMILIARIZATION":
                        if ((DateTime)cp.DateExpire > person.ASFExpireDate || person.ASFExpireDate == null)
                        {
                            person.ASFExpireDate = cp.DateExpire;
                            person.ASFIssueDate = cp.DateIssue;
                        }
                        break;
                    //CUSTOMER CARE
                    case "CUSTOMER CARE":
                        if ((DateTime)cp.DateExpire > person.CCExpireDate || person.CCExpireDate == null)
                        {
                            person.CCExpireDate = cp.DateExpire;
                            person.CCIssueDate = cp.DateIssue;
                        }
                        break;
                    //LOAD SHEET
                    case "LOAD SHEET":
                        if ((DateTime)cp.DateExpire > person.MBExpireDate || person.MBExpireDate == null)
                        {
                            person.MBExpireDate = cp.DateExpire;
                            person.MBIssueDate = cp.DateIssue;
                        }
                        break;
                    //PASSENGER SERVICE
                    case "PASSENGER SERVICE":
                        if ((DateTime)cp.DateExpire > person.PSExpireDate || person.PSExpireDate == null)
                        {
                            person.PSExpireDate = cp.DateExpire;
                            person.PSIssueDate = cp.DateIssue;
                        }
                        break;

                    //DRM
                    case "DRM":
                        if ((DateTime)cp.DateExpire > person.DRMExpireDate || person.DRMExpireDate == null)
                        {
                            person.DRMExpireDate = cp.DateExpire;
                            person.DRMIssueDate = cp.DateIssue;
                        }
                        break;
                    //ANNEX
                    case "ANNEX":
                        if ((DateTime)cp.DateExpire > person.ANNEXExpireDate || person.ANNEXExpireDate == null)
                        {
                            person.ANNEXExpireDate = cp.DateExpire;
                            person.ANNEXIssueDate = cp.DateIssue;
                        }
                        break;
                    //FRMS
                    case "FRMS":
                        if ((DateTime)cp.DateExpire > person.TypeAirbusExpireDate || person.TypeAirbusExpireDate == null)
                        {
                            person.TypeAirbusExpireDate = cp.DateExpire;
                            person.TypeAirbusIssueDate = cp.DateIssue;
                        }
                        break;
                    //DANGEROUS GOODS
                    case "DANGEROUS GOODS":
                        if ((DateTime)cp.DateExpire > person.DangerousGoodsExpireDate || person.DangerousGoodsExpireDate == null)
                        {
                            person.DangerousGoodsExpireDate = cp.DateExpire;
                            person.DangerousGoodsIssueDate = cp.DateIssue;
                        }
                        break;
                    //1	SEPT-P
                    case "SEPT":
                        if ((DateTime)cp.DateExpire > person.SEPTPExpireDate || person.SEPTPExpireDate == null)
                        {
                            person.SEPTPExpireDate = cp.DateExpire;
                            person.SEPTPIssueDate = cp.DateIssue;
                        }
                        break;
                    //2   SEPT - T
                    case "ANNUAL SEPT":
                        if ((DateTime)cp.DateExpire > person.SEPTExpireDate || person.SEPTExpireDate == null)
                        {
                            person.SEPTExpireDate = cp.DateExpire;
                            person.SEPTIssueDate = cp.DateIssue;
                        }
                        break;
                    //4	CRM
                    case "CRM":
                        if ((DateTime)cp.DateExpire > person.UpsetRecoveryTrainingExpireDate || person.UpsetRecoveryTrainingExpireDate == null)
                        {
                            person.UpsetRecoveryTrainingExpireDate = cp.DateExpire;
                            person.UpsetRecoveryTrainingIssueDate = cp.DateIssue;
                        }
                        break;
                    //5	CCRM
                    case "CCRM":
                        if ((DateTime)cp.DateExpire > person.CCRMExpireDate || person.CCRMExpireDate == null)
                        {
                            person.CCRMExpireDate = cp.DateExpire;
                            person.CCRMIssueDate = cp.DateIssue;
                        }
                        break;
                    //6	SMS
                    case "SMS":
                        if ((DateTime)cp.DateExpire > person.SMSExpireDate || person.SMSExpireDate == null)
                        {
                            person.SMSExpireDate = cp.DateExpire;
                            person.SMSIssueDate = cp.DateIssue;
                        }
                        break;
                    //7	AV-SEC
                    case "AVIATION SECURITY":
                        if ((DateTime)cp.DateExpire > person.AviationSecurityExpireDate || person.AviationSecurityExpireDate == null)
                        {
                            person.AviationSecurityExpireDate = cp.DateExpire;
                            person.AviationSecurityIssueDate = cp.DateIssue;
                        }
                        break;
                    //8	COLD-WX
                    case "COLD WEATHER OPERATION":
                        if ((DateTime)cp.DateExpire > person.ColdWeatherOperationExpireDate || person.ColdWeatherOperationExpireDate == null)
                        {
                            person.ColdWeatherOperationExpireDate = cp.DateExpire;
                            person.ColdWeatherOperationIssueDate = cp.DateIssue;
                        }
                        break;
                    //9	HOT-WX
                    case "HOT WEATHER OPERATION":
                        if ((DateTime)cp.DateExpire > person.HotWeatherOperationExpireDate || person.HotWeatherOperationExpireDate == null)
                        {
                            person.HotWeatherOperationExpireDate = cp.DateExpire;
                            person.HotWeatherOperationIssueDate = cp.DateIssue;
                        }
                        break;
                    //10	FIRSTAID
                    case "FIRST AID":
                        if ((DateTime)cp.DateExpire > person.FirstAidExpireDate || person.FirstAidExpireDate == null)
                        {
                            person.FirstAidExpireDate = cp.DateExpire;
                            person.FirstAidIssueDate = cp.DateIssue;
                        }
                        break;
                    ////lpc
                    //case 100:
                    //    if ((DateTime)cp.DateExpire > person.ProficiencyValidUntil || person.ProficiencyValidUntil == null)
                    //    {
                    //        person.ProficiencyValidUntil = cp.DateExpire;
                    //        person.ProficiencyCheckDate = cp.DateIssue;
                    //    }
                    //    break;
                    ////opc
                    //case 101:
                    //    if ((DateTime)cp.DateExpire > person.ProficiencyValidUntilOPC || person.ProficiencyValidUntilOPC == null)
                    //    {
                    //        person.ProficiencyValidUntilOPC = cp.DateExpire;
                    //        person.ProficiencyCheckDateOPC = cp.DateIssue;
                    //    }
                    //    break;
                    ////lpr
                    //case 102:
                    //    if ((DateTime)cp.DateExpire > person.ICAOLPRValidUntil || person.ICAOLPRValidUntil == null)
                    //    {
                    //        person.ICAOLPRValidUntil = cp.DateExpire;
                    //        // person.ProficiencyCheckDateOPC = cp.DateIssue;
                    //    }
                    //    break;

                    //grt
                    case "GRT":
                        if ((DateTime)cp.DateExpire > person.DateCaoCardExpire || person.DateCaoCardExpire == null)
                        {
                            person.DateCaoCardExpire = cp.DateExpire;
                            person.DateCaoCardIssue = cp.DateIssue;
                        }
                        break;
                    //recurrent
                    case "RECURRENT 737":
                        if ((DateTime)cp.DateExpire > person.Type737ExpireDate || person.Type737ExpireDate == null)
                        {
                            person.Type737ExpireDate = cp.DateExpire;
                            person.Type737IssueDate = cp.DateIssue;
                        }
                        break;
                    //fmt
                    case "FMT":
                        if ((DateTime)cp.DateExpire > person.EGPWSExpireDate || person.EGPWSExpireDate == null)
                        {
                            person.EGPWSExpireDate = cp.DateExpire;
                            person.EGPWSIssueDate = cp.DateIssue;
                        }
                        break;
                    case "FMTD":
                        if ((DateTime)cp.DateExpire > person.FMTDExpireDate || person.FMTDExpireDate == null)
                        {
                            person.FMTDExpireDate = cp.DateExpire;
                            person.FMTDIssueDate = cp.DateIssue;
                        }
                        break;
                    case "LINE":
                        if ((DateTime)cp.DateExpire > person.LineExpireDate || person.LineExpireDate == null)
                        {
                            person.LineExpireDate = cp.DateExpire;
                            person.LineIssueDate = cp.DateIssue;
                        }
                        break;
                    default:
                        break;
                }
            }





            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public IQueryable<CertificateType> GetCertificateTypesQuery()
        {
            IQueryable<CertificateType> query = context.Set<CertificateType>().AsNoTracking();
            return query;
        }

        public IQueryable<ViewCourseType> GetCourseTypesQuery()
        {
            IQueryable<ViewCourseType> query = context.Set<ViewCourseType>().AsNoTracking();
            return query;
        }

        public IQueryable<ViewCourseNew> GetCourseQuery()
        {
            IQueryable<ViewCourseNew> query = context.Set<ViewCourseNew>().AsNoTracking();
            return query.Where(q => q.ParentId == null);
        }

        public IQueryable<ViewJobGroup> GetViewJobGroupQuery()
        {
            IQueryable<ViewJobGroup> query = context.Set<ViewJobGroup>().AsNoTracking();
            return query;
        }

        public async Task<DataResponse> GetCourseSessions(int cid)
        {
            var result = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetExamSummary(int exam_id)
        {

            var summary = await context.view_trn_exam_summary_details.Where(q => q.main_exam_id == exam_id).OrderBy(q => q.last_name).ThenBy(q => q.first_name).ToListAsync();
            return new DataResponse()
            {
                Data = summary,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetExamPeopleAnswers(int exam_id)
        {
            var summary = await context.view_trn_exam_question_person_details.Where(q => q.exam_id == exam_id)
                .OrderBy(q => q.last_name).ThenBy(q => q.first_name).ThenBy(q => q.category).ThenBy(q => q.question_id)
                .ToListAsync();
            return new DataResponse()
            {
                Data = summary,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetExamPeopleAnswersByPerson(int exam_id, int person_id)
        {
            var summary = await context.view_trn_exam_question_person_details.Where(q => q.exam_id == exam_id && q.person_id == person_id)
                .OrderBy(q => q.category).ThenBy(q => q.question_id)
                .ToListAsync();
            return new DataResponse()
            {
                Data = summary,
                IsSuccess = true,
            };
        }

        //public async Task<DataResponse> GetCoursePeopleAndSessions(int cid)
        //{
        //    var course = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();
        //    var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
        //    var people = await context.ViewCoursePeoples.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();

        //    //var press = await context.CourseSessionPresences.Where(q => q.CourseId == cid).ToListAsync();
        //    var press = await context.ViewCourseSessionPresences.Where(q => q.CourseId == cid).ToListAsync();
        //    var sessions_stats = (from x in press
        //                          group x by new { x.Id, x.SessionKey } into grp
        //                          select new
        //                          {
        //                              grp.Key.Id,
        //                              grp.Key.SessionKey,
        //                              present = grp.Where(q => q.IsPresent == 1).Count(),
        //                              total = people.Count(),
        //                              absent = people.Count() - grp.Where(q => q.IsPresent == 1).Count()
        //                          }).ToList();

        //    var syllabi = await context.ViewSyllabus.Where(q => q.CourseId == cid).ToListAsync();
        //    //var exams = new List<trn_exam>();
        //    var _exams = await context.trn_exam.Where(q => q.course_id == cid).ToListAsync();
        //    var exams = _exams.Select(q => JsonConvert.DeserializeObject<ExamViewModel>(JsonConvert.SerializeObject(q))).ToList();


        //    var exam_ids = exams.Select(q => q.id).ToList();
        //    var exam_ids_null = exams.Select(q => (Nullable<int>)q.id).ToList();
        //    var templates = await context.view_trn_exam_question_template.Where(q => exam_ids.Contains(q.exam_id)).ToListAsync();
        //    var questions = await context.view_trn_exam_question.Where(q => exam_ids.Contains(q.exam_id)).ToListAsync();
        //    var exam_summary = await context.view_trn_exam_summary_details.Where(q => exam_ids_null.Contains(q.main_exam_id)).ToListAsync();
        //    try
        //    {
        //        foreach (var x in exams)
        //        {
        //            x.date_end_scheduled = ((DateTime)x.exam_date).AddMinutes(Convert.ToDouble(x.duration));
        //            if (x.status_id != 0 && x.date_end_actual == null && x.date_start != null)
        //            {
        //                x.date_end_actual = ((DateTime)x.date_start).AddMinutes(Convert.ToDouble(x.duration));
        //            }
        //            x.questions = questions.Where(q => q.exam_id == x.id).OrderBy(q => q.category).ThenBy(q => q.question_id).ToList();
        //            x.template = templates.Where(q => q.exam_id == x.id).ToList();
        //            x.summary = exam_summary.Where(q => q.main_exam_id == x.id).OrderBy(q => q.last_name).ThenBy(q => q.first_name).ToList();
        //        }
        //    }
        //    catch (Exception exxx)
        //    {

        //    }





        //    return new DataResponse()
        //    {
        //        Data = new
        //        {
        //            course,
        //            sessions,
        //            sessions_stats,
        //            people,
        //            press,
        //            syllabi,
        //            exams
        //        },
        //        IsSuccess = true,
        //    };
        //}


        public async Task<DataResponse> GetCoursePeopleAndSessions(int cid)
        {
            var course = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();
            var subjects = await context.ViewCourseNews.Where(q => q.ParentId == cid).ToListAsync();
            var subject_ids = subjects.Select(q => q.Id).ToList();

            var main_sessions = await context.ViewCourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            List<ViewCourseSession> sessions = main_sessions.ToList();
            if (subjects.Count > 0)
            {

                sessions = await context.ViewCourseSessions.Where(q => subject_ids.Contains(q.CourseId)).ToListAsync();


            }

            var sessions_grps = (from x in sessions
                                 group x by new { x.Date, x.PDate } into grp
                                 select new
                                 {
                                     grp.Key.Date,
                                     grp.Key.PDate,
                                     instructors = (from item in grp
                                                    group item by new { item.Instructor, item.InstructorId } into grp2
                                                    select new
                                                    {
                                                        grp2.Key.InstructorId,
                                                        grp2.Key.Instructor,
                                                        courses = from w in grp2
                                                                  group w by new { w.Title, w.CT_Title } into grp3
                                                                  select new
                                                                  {
                                                                      grp3.Key.Title,
                                                                      grp3.Key.CT_Title,
                                                                      sessions = grp3.OrderBy(q => q.DateStart).ToList()
                                                                  }
                                                    }).ToList(),
                                     courses = from item in grp
                                               group item by new { item.Title, item.CT_Title, item.CourseId } into c_grp
                                               select new
                                               {
                                                   c_grp.Key.Title,
                                                   c_grp.Key.CT_Title,
                                                   c_grp.Key.CourseId,
                                                   sessions = c_grp.OrderBy(q => q.DateStart).ToList()
                                               }
                                 }).ToList();



            var people = await context.ViewCoursePeoples.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var participants = people.ToList();
            if (subjects.Count > 0)
            {
                people = await context.ViewCoursePeoples.Where(q => subject_ids.Contains(q.CourseId)).OrderBy(q => q.DateStart).ToListAsync();
            }



            var people_grps = (from x in people
                               group x by new { x.CourseId, x.Title, x.Instructor, x.Duration, x.Date_Sign_Ins1 } into grps
                               select new
                               {
                                   grps.Key.CourseId,
                                   grps.Key.Title,
                                   grps.Key.Instructor,
                                   grps.Key.Duration,
                                   grps.Key.Date_Sign_Ins1,
                                   people = grps.OrderByDescending(q => q.Presence).ThenBy(q => q.LastName).ThenBy(q => q.FirstName).ToList()

                               }).OrderBy(q => q.Title).ToList();

            //var press = await context.CourseSessionPresences.Where(q => q.CourseId == cid).ToListAsync();
            var press = await context.ViewCourseSessionPresences.Where(q => q.CourseId == cid).ToListAsync();
            if (subjects.Count > 0)
            {
                press = await context.ViewCourseSessionPresences.Where(q => subject_ids.Contains(q.CourseId)).ToListAsync();
            }
            //  var press_grps=from x in press
            //                 group x by new {x.PersonId,x.EmployeeId,x.FirstName,x.LastName,x.Name,x.NID,x.Mobile,x.JobGroup}
            var sessions_stats = (from x in press
                                  group x by new { x.Id, x.SessionKey, x.CourseId, x.Title, x.Instructor, } into grp
                                  select new
                                  {
                                      grp.Key.Id,
                                      grp.Key.SessionKey,
                                      grp.Key.Title,
                                      grp.Key.Instructor,
                                      grp.Key.CourseId,
                                      DateStart = sessions.Where(q => q.Key == grp.Key.SessionKey).FirstOrDefault().DateStart,
                                      DateEnd = sessions.Where(q => q.Key == grp.Key.SessionKey).FirstOrDefault().DateEnd,
                                      present = grp.Where(q => q.IsPresent == 1).Count(),
                                      total = people.Where(q => q.CourseId == grp.Key.CourseId).Count(),
                                      absent = people.Where(q => q.CourseId == grp.Key.CourseId).Count() - grp.Where(q => q.IsPresent == 1).Count(),
                                      people = grp.OrderByDescending(q => q.IsPresent).ThenBy(q => q.LastName).ThenBy(q => q.FirstName).ToList()
                                  }).ToList();

            var syllabi = await context.ViewSyllabus.Where(q => q.CourseId == cid).ToListAsync();
            //var exams = new List<trn_exam>();
            var _exams = await context.trn_exam.Where(q => q.course_id == cid).ToListAsync();
            var exams = _exams.Select(q => JsonConvert.DeserializeObject<ExamViewModel>(JsonConvert.SerializeObject(q))).ToList();


            var exam_ids = exams.Select(q => q.id).ToList();
            var exam_ids_null = exams.Select(q => (Nullable<int>)q.id).ToList();
            var templates = await context.view_trn_exam_question_template.Where(q => exam_ids.Contains(q.exam_id)).ToListAsync();
            var questions = await context.view_trn_exam_question.Where(q => exam_ids.Contains(q.exam_id)).ToListAsync();
            var exam_summary = await context.view_trn_exam_summary_details.Where(q => exam_ids_null.Contains(q.main_exam_id)).ToListAsync();
            try
            {
                foreach (var x in exams)
                {
                    x.date_end_scheduled = ((DateTime)x.exam_date).AddMinutes(Convert.ToDouble(x.duration));
                    if (x.status_id != 0 && x.date_end_actual == null && x.date_start != null)
                    {
                        x.date_end_actual = ((DateTime)x.date_start).AddMinutes(Convert.ToDouble(x.duration));
                    }
                    x.questions = questions.Where(q => q.exam_id == x.id).OrderBy(q => q.category).ThenBy(q => q.question_id).ToList();
                    x.template = templates.Where(q => q.exam_id == x.id).ToList();
                    x.summary = exam_summary.Where(q => q.main_exam_id == x.id).OrderBy(q => q.last_name).ThenBy(q => q.first_name).ToList();
                }
            }
            catch (Exception exxx)
            {

            }





            return new DataResponse()
            {
                Data = new
                {
                    course,
                    subjects,
                    sessions,
                    main_sessions,
                    sessions_grps,
                    people_grps,
                    sessions_stats,
                    people = participants,
                    press,
                    syllabi,
                    exams
                },
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetCoursePeopleAndSessionsByDate(DateTime dt, int pid)
        {
            dt = dt.Date;
            var d2 = dt.AddDays(1);
            var query = await (from x in context.CourseSessions
                               join y in context.ViewCoursePeoples on x.CourseId equals y.CourseId
                               where x.DateStart >= dt && x.DateEnd <= d2 && y.PersonId == pid
                               select new
                               {
                                   y.CourseId,
                                   y.Title,
                                   y.No,
                                   y.Status,
                                   y.Location,
                                   y.Instructor1,
                                   y.Instructor2,
                                   y.CoursePeopleStatus,
                                   x.DateStart,
                                   x.DateEnd,

                               }).ToListAsync();
            var grps = (from y in query
                        group y by new
                        {
                            y.CourseId,
                            y.Title,
                            y.No,
                            y.Status,
                            y.Location,
                            y.Instructor1,
                            y.Instructor2,
                            y.CoursePeopleStatus
                        }
                     into grp
                        select new
                        {
                            grp.Key.CourseId,
                            grp.Key.Title,
                            grp.Key.No,
                            grp.Key.Status,
                            grp.Key.Location,
                            grp.Key.Instructor1,
                            grp.Key.Instructor2,
                            grp.Key.CoursePeopleStatus,
                            Sessions = grp.OrderBy(q => q.DateStart).Select(q => new { q.DateStart, q.DateEnd }).ToList()
                        }).ToList();
            return new DataResponse()
            {
                Data = grps,
                IsSuccess = true,
            };

        }

        public async Task<DataResponse> NotifyCoursePeople(int cid, string recs)
        {
            try
            {
                var recIds = recs.Split('_').Select(q => (Nullable<int>)Convert.ToInt32(q)).ToList();
                var course = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();
                var people = await context.ViewCoursePeoples.Where(q => q.CourseId == cid && recIds.Contains(q.PersonId)).OrderBy(q => q.DateStart).ToListAsync();
                var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
                List<string> strs = new List<string>();
                strs.Add("COURSE NOTIFICATION");
                strs.Add(course.Title.ToUpper());
                if (!string.IsNullOrEmpty(course.Organization))
                    strs.Add(course.Organization);
                if (!string.IsNullOrEmpty(course.Location))
                    strs.Add(course.Location);
                if (!string.IsNullOrEmpty(course.HoldingType))
                    strs.Add(course.HoldingType);
                strs.Add(course.DateStart.ToString("ddd, dd MMM yyyy"));
                if (course.DateEnd != null)
                    strs.Add(((DateTime)course.DateEnd).ToString("ddd, dd MMM yyyy"));

                if (sessions.Count > 0)
                {
                    strs.Add("Sessions");
                    foreach (var x in sessions)
                    {
                        if (x.DateStart != null && x.DateEnd != null)
                        {
                            var dt = ((DateTime)x.DateStart).ToString("ddd, dd MMM yyyy");
                            strs.Add(dt + " " + ((DateTime)x.DateStart).ToString("HH:mm") + "-" + ((DateTime)x.DateEnd).ToString("HH:mm"));
                        }

                    }
                }
                strs.Add("TRAINING DEPARTMENT");

                var text = String.Join("\n", strs);
                Magfa m = new Magfa();

                var res = new List<long>();
                var hists = new List<CourseSMSHistory>();
                foreach (var p in people)
                {
                    var rs = m.enqueue(1, p.Mobile, text)[0];
                    var hist = new CourseSMSHistory()
                    {
                        CourseId = cid,
                        DateSent = DateTime.Now,
                        Mobil = p.Mobile,
                        Msg = text,
                        PersonId = p.EmployeeId,
                        PersonName = p.Name,
                        TypeId = 1,
                        RefId = rs,



                    };
                    hists.Add(hist);
                    context.CourseSMSHistories.Add(hist);
                    res.Add(rs);
                }
                await Task.Delay(10000);


                var sts = m.getStatus(res);
                int c = 0;
                foreach (var st in sts)
                {
                    hists[c].DateStatus = DateTime.Now;
                    hists[c].Statu = st;
                    c++;
                }
                var saveResult = await context.SaveAsync();
                return new DataResponse()
                {
                    Data = hists,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                var inner = ex.InnerException;
                var result = msg + "Inner" + inner;
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = false
                };
            }

        }


        public async Task<DataResponse> NotifyCourseTeachers(int cid)
        {
            var course = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();
            var crs = await context.Courses.Where(q => q.Id == cid).FirstOrDefaultAsync();
            Person ins1 = null;
            Person ins2 = null;
            if (course.CurrencyId != null)
                ins1 = await context.People.FirstOrDefaultAsync(q => q.Id == course.CurrencyId);
            if (course.Instructor2Id != null)
                ins2 = await context.People.FirstOrDefaultAsync(q => q.Id == course.Instructor2Id);

            List<Person> people = new List<Person>();
            if (ins1 != null)
                people.Add(ins1);
            if (ins2 != null)
                people.Add(ins2);
            var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            List<string> strs = new List<string>();
            strs.Add("COURSE NOTIFICATION");
            strs.Add(course.Title.ToUpper());
            if (!string.IsNullOrEmpty(course.Organization))
                strs.Add(course.Organization);
            if (!string.IsNullOrEmpty(course.Location))
                strs.Add(course.Location);
            if (!string.IsNullOrEmpty(course.HoldingType))
                strs.Add(course.HoldingType);
            strs.Add(course.DateStart.ToString("ddd, dd MMM yyyy"));
            if (course.DateEnd != null)
                strs.Add(((DateTime)course.DateEnd).ToString("ddd, dd MMM yyyy"));

            if (sessions.Count > 0)
            {
                strs.Add("Sessions");
                foreach (var x in sessions)
                {
                    if (x.DateStart != null && x.DateEnd != null)
                    {
                        var dt = ((DateTime)x.DateStart).ToString("ddd, dd MMM yyyy");
                        strs.Add(dt + " " + ((DateTime)x.DateStart).ToString("HH:mm") + "-" + ((DateTime)x.DateEnd).ToString("HH:mm"));
                    }

                }
            }
            strs.Add("TRAINING DEPARTMENT");

            var text = String.Join("\n", strs);
            Magfa m = new Magfa();

            var res = new List<long>();
            var hists = new List<CourseSMSHistory>();
            foreach (var p in people)
            {
                var rs = m.enqueue(1, p.Mobile, text)[0];
                var hist = new CourseSMSHistory()
                {
                    CourseId = cid,
                    DateSent = DateTime.Now,
                    Mobil = p.Mobile,
                    Msg = text,
                    PersonId = p.Id,
                    PersonName = p.FirstName + " " + p.LastName,
                    TypeId = 1,
                    RefId = rs,



                };
                hists.Add(hist);
                context.CourseSMSHistories.Add(hist);
                res.Add(rs);
            }
            await Task.Delay(10000);


            var sts = m.getStatus(res);
            int c = 0;
            foreach (var st in sts)
            {
                hists[c].DateStatus = DateTime.Now;
                hists[c].Statu = st;
                if (c == 0)
                {
                    crs.SMSIns1 = text;
                    crs.SMSIns1Status = st;
                    crs.SMSInsDate = DateTime.Now;
                }
                else
                {
                    crs.SMSIns2 = text;
                    crs.SMSIns2Status = st;
                }
                c++;
            }
            var saveResult = await context.SaveAsync();
            return new DataResponse()
            {
                Data = hists,
                IsSuccess = true,
            };

        }


        public async Task<DataResponse> NotifyCourseRemaining(int dd)
        {
            Magfa m = new Magfa();
            List<string> nos = new List<string>() { "09128070746", "09122106372", "09124449584" };
            var courses = await context.ViewCourseRemainings.Where(q => q.Remaining == dd).ToListAsync();
            var jgs = courses.Select(q => q.JobGroupCode).ToList();
            var jobgroups = await context.ViewJobGroups.Where(q => jgs.Contains(q.FullCode) || jgs.Contains(q.FullCode2)).ToListAsync();

            var sent = new List<CourseRemainingNotification>();
            var hises = new List<CourseRemainingNotification>();
            var refs = new List<Int64>();
            foreach (var x in courses)
            {
                List<string> strs = new List<string>();
                List<string> mngnos = new List<string>();
                //var _mng = jobgroups.FirstOrDefault(q => q.FullCode == x.JobGroupCode || q.FullCode2 == x.JobGroupCode);
                //if (_mng!=null && _mng.Manager != null)
                //{
                //      mngnos = await context.ViewEmployees.Where(q => q.GroupId == _mng.Manager || q.IntervalNDT==_mng.Manager).Select(q => q.Mobile).ToListAsync();
                //}
                strs.Add("EXPIRING NOTIFICATION");
                strs.Add("Dear " + x.Name + ",");
                strs.Add("Your certificate/licence will be expired in " + dd.ToString() + " day(s).");
                strs.Add(x.CourseType);
                strs.Add("Issued:" + ((DateTime)x.DateIssue).ToString("yyyy-MM-dd"));
                strs.Add("Expired:" + ((DateTime)x.DateExpire).ToString("yyyy-MM-dd"));
                strs.Add("TRAINING DEPARTMENT");
                var text = String.Join("\n", strs);
                var rs = m.enqueue(1, x.Mobile, text)[0];
                //foreach(var mo in nos)
                //{
                //    m.enqueue(1, /*x.Mobile*/mo, text) ;
                //}
                //foreach (var mo in mngnos)
                //{
                //    m.enqueue(1, /*x.Mobile*/mo, text);
                //}
                var his = new CourseRemainingNotification()
                {
                    CourseId = x.CourseId,
                    DateExpire = x.DateExpire,
                    DateIssue = x.DateIssue,
                    DateSent = DateTime.Now,
                    Message = text,
                    Mobile = x.Mobile,
                    Name = x.Name,
                    PersonId = x.PersonId,
                    Remaining = dd,
                    Title = x.Title,
                    RefId = rs,
                };
                hises.Add(his);
                //if (rs != -1)
                refs.Add(rs);
                context.CourseRemainingNotifications.Add(his);

            }


            await Task.Delay(5000);


            var sts = m.getStatus(refs);
            int c = 0;
            foreach (var st in sts)
            {

                hises[c].Status = st;

                c++;
            }
            var saveResult = await context.SaveAsync();
            return new DataResponse()
            {
                Data = hises,
                IsSuccess = true,
            };

        }


        public async Task<DataResponse> EmailCourseRemaining(int dd)
        {

            List<string> nos = new List<string>() { "09128070746", "09122106372", "09124449584" };
            var courses = await context.ViewCourseRemainings.Where(q => q.Remaining == dd).ToListAsync();
            var jgs = courses.Select(q => q.JobGroupCode).ToList();
            var jobgroups = await context.ViewJobGroups.Where(q => jgs.Contains(q.FullCode) || jgs.Contains(q.FullCode2)).ToListAsync();

            var sent = new List<CourseRemainingNotification>();
            var hises = new List<CourseRemainingNotification>();
            var refs = new List<Int64>();
            List<string> strs = new List<string>();
            strs.Add("<b>EXPIRING NOTIFICATION</b>");

            strs.Add("<div>The below certificates/licences will be expired in " + dd.ToString() + " day(s).</div>");
            foreach (var x in courses)
            {


                List<string> mngnos = new List<string>();
                // var _mng = jobgroups.FirstOrDefault(q => q.FullCode == x.JobGroupCode || q.FullCode2 == x.JobGroupCode);
                // if (_mng != null && _mng.Manager != null)
                // {
                //     mngnos = await context.ViewEmployees.Where(q => q.GroupId == _mng.Manager || q.IntervalNDT == _mng.Manager).Select(q => q.Mobile).ToListAsync();
                // }

                strs.Add("<div>" + x.CourseType + ", " + x.Name + ", " + "Issued:" + ((DateTime)x.DateIssue).ToString("yyyy-MM-dd") + ", " + "Expired:" + ((DateTime)x.DateExpire).ToString("yyyy-MM-dd") + "</di>");






            }
            var text = String.Join("", strs);
            var helper = new MailHelper();
            var result = helper.SendTest("v.moghaddam59@gmail.com", text, "EXPIRING NOTIFICATION", 25, 0);



            return new DataResponse()
            {
                Data = strs,
                IsSuccess = true,
            };

        }

        public async Task<DataResponse> GetCourseAttendance(int cid, int pid)
        {
            var attendance = await context.ViewCourseSessionPresenceDetails.Where(q => q.PersonId == pid && q.CourseId == cid).OrderBy(q => q.DateFrom).ToListAsync();

            return new DataResponse()
            {
                Data = attendance,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> SaveCourseAttendance(Attendance att)
        {
            var _dates = att.Date.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var _from = att.From.Split(':').Select(q => Convert.ToInt32(q)).ToList();
            var _to = att.To.Split(':').Select(q => Convert.ToInt32(q)).ToList();

            var dfrom = new DateTime(_dates[0], _dates[1], _dates[2], _from[0], _from[1], 0);
            var dto = new DateTime(_dates[0], _dates[1], _dates[2], _to[0], _to[1], 0);

            var entity = new CourseSessionPresenceDetail()
            {
                CourseId = att.CourseId,
                PersonId = att.PersonId,
                SessionKey = att.Key,
                DateFrom = dfrom,
                DateTo = dto,
                Remark = att.Remark,
                Date = DateTime.Now

            };
            context.CourseSessionPresenceDetails.Add(entity);

            var saveResult = await context.SaveAsync();

            var view = await this.context.ViewCourseSessionPresenceDetails.Where(q => q.Id == entity.Id).FirstOrDefaultAsync();
            return new DataResponse()
            {
                Data = view,
                IsSuccess = saveResult.IsSuccess,
            };


        }

        public async Task<DataResponse> DeleteCourseAttendance(int cid)
        {
            var obj = await this.context.CourseSessionPresenceDetails.Where(q => q.Id == cid).FirstOrDefaultAsync();
            if (obj != null)
            {
                context.CourseSessionPresenceDetails.Remove(obj);
                var saveResult = await context.SaveAsync();
            }
            return new DataResponse()
            {
                Data = true,
                IsSuccess = true,
            };
        }
        //12-02
        public async Task<DataResponse> SyncSessionsToRoster(int cid)
        {
            var sessions = await context.ViewCourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var cps = await context.CoursePeoples.Where(q => q.CourseId == cid).ToListAsync();
            var personIds = cps.Select(q => q.PersonId).ToList();
            var fltcrew = new List<string>() { "P1", "P2", "ISCCM", "SCCM", "CCM", "TRE", "TRI", "LTC", "CCE", "CCI" };
            var employees = await context.ViewEmployeeAbs.Where(q => personIds.Contains(q.PersonId) && fltcrew.Contains(q.JobGroup)).ToListAsync();
            var currents = await context.CourseSessionFDPs.Where(q => q.CourseId == cid).ToListAsync();
            var fdps = new List<FDP>();
            var errors = new List<object>();
            foreach (var session in sessions)
            {
                foreach (var emp in employees)
                {
                    var exist = currents.Where(q => q.EmployeeId == emp.Id && q.SessionKey == session.Key).FirstOrDefault();
                    if (exist == null)
                    {

                        //var ofdp = (from x in context.ViewFDPIdeas.AsNoTracking()
                        //            where x.CrewId == emp.Id && x.DutyType == 1165
                        //            && (
                        //              (session.DateStartUtc >= x.DutyStart && session.DateStartUtc <= x.RestUntil) || (session.DateEndUtc >= x.DutyStart && session.DateEndUtc <= x.RestUntil)
                        //              || (x.DutyStart >= session.DateStartUtc && x.DutyStart <= session.DateEndUtc) || (x.RestUntil >= session.DateStartUtc && x.RestUntil <= session.DateEndUtc)
                        //              )
                        //            select x).FirstOrDefault();
                        //var pre_fpd=context.ViewFDPIdeas.Where(q=>q.CrewId==emp.Id && q.DutyType==1165 && q.DutyStart<session.DateStartUtc).OrderByDescending(q=>q.DutyStart).FirstOrDefault();
                        var post_fdp = context.ViewFDPIdeas.Where(q => q.CrewId == emp.Id && q.DutyType == 1165 && q.DutyStart > session.DateEndUtc).OrderBy(q => q.DutyStart).FirstOrDefault();
                        var in_fdp = context.ViewFDPIdeas.Where(q => q.CrewId == emp.Id && q.DutyType == 1165 &&
                                 (
                                    (q.DutyStart >= session.DateStartUtc && q.DutyEnd <= session.DateEndUtc) ||
                                    (q.DutyStart >= session.DateStartUtc && q.DutyStart <= session.DateEndUtc) ||
                                    (q.DutyEnd >= session.DateStartUtc && q.DutyEnd <= session.DateEndUtc) ||
                                    (session.DateStartUtc >= q.DutyStart && session.DateStartUtc <= q.DutyEnd)
                                 )

                            ).OrderBy(q => q.DutyStart).FirstOrDefault();
                        var ofdp = in_fdp;
                        if (ofdp == null && post_fdp != null)
                        {
                            if (((DateTime)post_fdp.DutyStart - (DateTime)session.DateStartUtc).TotalMinutes < 12 * 60)
                                ofdp = post_fdp;
                        }

                        if (ofdp == null)
                        {
                            var duty = new FDP();
                            duty.DateStart = session.DateStartUtc;
                            duty.DateEnd = session.DateEndUtc;

                            duty.CrewId = emp.Id;
                            duty.DutyType = 5000;
                            duty.GUID = Guid.NewGuid();
                            duty.IsTemplate = false;
                            duty.Remark = session.CT_Title + "\r\n" + session.Title;
                            duty.UPD = 1;

                            duty.InitStart = duty.DateStart;
                            duty.InitEnd = duty.DateEnd;
                            duty.DateConfirmed = DateTime.Now;
                            duty.ConfirmedBy = "TRAINING";
                            var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003 };
                            duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
                            //rec.FDP = duty;
                            var csf = new CourseSessionFDP()
                            {
                                FDP = duty,
                                CourseId = session.CourseId,
                                SessionKey = session.Key,
                                EmployeeId = emp.Id,

                            };
                            context.FDPs.Add(duty);
                            context.CourseSessionFDPs.Add(csf);


                            fdps.Add(duty);
                        }
                        else
                        {
                            errors.Add(new
                            {
                                FDPId = ofdp.Id,
                                EmployeeId = emp.Id,
                                SessionItemId = session.Id,
                                Name = emp.Name,
                                Flights = ofdp.InitFlts,
                                Route = ofdp.InitRoute,
                                // DutyEnd = ofdp.DutyEndLocal,
                                DutyStart = ofdp.DutyStart,
                                RestUntil = ofdp.RestUntil,
                                CourseCode = session.CT_Title,
                                CourseTitle = session.Title,
                                SessionDateFrom = session.DateStart,
                                SessionDateTo = session.DateEnd,
                                DateCreate = DateTime.Now
                            });
                        }
                    }

                }
            }
            var saveResult = await context.SaveAsync();
            return new DataResponse()
            {
                Data = new
                {
                    fdps = fdps.Select(q => new
                    {
                        q.Id,
                        q.CrewId
                    }).ToList(),
                    errors,
                    saveErrors = saveResult.Errors,
                },
                IsSuccess = saveResult.IsSuccess,
            };
        }


        //12-03

        public async Task<sync_result> SyncSessionsToRosterByDateTeachers_Subjects(int cid)
        {
            var course = await context.Courses.FirstOrDefaultAsync(q => q.Id == cid);


            var sessions = await context.ViewCourseSessions.Where(q => q.CourseId == cid /*|| q.ParentId == cid*/).OrderBy(q => q.DateStart).ToListAsync();

            var grp_sessions = (from x in sessions
                                group x by new { ((DateTime)x.DateStart).Date, x.Title, x.CourseId } into grp
                                select new
                                {
                                    grp.Key.Date,
                                    grp.Key.Title,
                                    grp.Key.CourseId,

                                    items = grp.OrderBy(q => q.DateStart).ToList(),
                                    start = ((DateTime)grp.Min(q => q.DateStart)),
                                    end = ((DateTime)grp.Max(q => q.DateEnd)),
                                    start_utc = ((DateTime)grp.Min(q => q.DateStartUtc)),
                                    end_utc = ((DateTime)grp.Max(q => q.DateEndUtc)),

                                }).ToList();
            //var course_ids = new List<int>() { cid };
            //course_ids = course_ids.Concat(grp_sessions.Select(q => q.CourseId).ToList()).ToList();
            var personIds = new List<int>();

            if (course.CurrencyId != null)
                personIds.Add((int)course.CurrencyId);
            if (course.Instructor2 != null)
                personIds.Add((int)course.Instructor2);

            //  var ins_ids = grp_sessions.Where(q => q.InstructorId != null).Select(q => q.InstructorId).Distinct().ToList();
            // personIds = personIds.Concat(ins_ids).ToList();


            var fltcrew = new List<string>() { "P1", "P2", "ISCCM", "SCCM", "CCM", "TRE", "TRI", "LTC", "CCE", "CCI" };
            var employees = await context.ViewEmployeeAbs.Where(q => personIds.Contains(q.PersonId) && fltcrew.Contains(q.JobGroup)).ToListAsync();
            var eids = employees.Select(q => (Nullable<int>)q.Id).ToList();




            var current_fdp_ids = await context.CourseSessionFDPs.Where(q => q.CourseId == cid && eids.Contains(q.EmployeeId)).Select(q => q.FDPId).ToListAsync();
            //var currents = await context.CourseSessionFDPs.Where(q =>course_ids.Contains( q.CourseId)).ToListAsync();
            //context.CourseSessionFDPs.RemoveRange(currents);
            var currents = await context.FDPs.Where(q => current_fdp_ids.Contains(q.Id)).ToListAsync();
            context.FDPs.RemoveRange(currents);

            var fdps = new List<FDP>();
            var errors = new List<object>();
            var errors_people = new List<int?>();
            //course.Date_Sessions_Synced = DateTime.Now;
            course.Date_Sessions_Instructor_Synced = DateTime.Now;
            foreach (var session in grp_sessions)
            {
                foreach (var emp in employees)
                {
                    var _proceed = true;

                    if (_proceed)
                    {

                        var post_fdp = context.ViewFDPIdeas.Where(q => q.CrewId == emp.Id && q.DutyType == 1165 && q.DutyStart > session.end_utc).OrderBy(q => q.DutyStart).FirstOrDefault();
                        var in_fdp = context.ViewFDPIdeas.Where(q => q.CrewId == emp.Id && q.DutyType == 1165 &&
                                 (
                                    (q.DutyStart >= session.start_utc && q.DutyEnd <= session.end_utc) ||
                                    (q.DutyStart >= session.start_utc && q.DutyStart <= session.end_utc) ||
                                    (q.DutyEnd >= session.start_utc && q.DutyEnd <= session.end_utc) ||
                                    (session.start_utc >= q.DutyStart && session.end_utc <= q.DutyEnd)
                                 )

                            ).OrderBy(q => q.DutyStart).FirstOrDefault();
                        var ofdp = in_fdp;
                        if (ofdp == null && post_fdp != null)
                        {
                            if (((DateTime)post_fdp.DutyStart - (DateTime)session.end_utc).TotalMinutes < 12 * 60)
                                ofdp = post_fdp;
                        }

                        if (ofdp == null)
                        {
                            var duty = new FDP();
                            duty.DateStart = session.start_utc;
                            duty.DateEnd = session.end_utc;

                            duty.CrewId = emp.Id;
                            duty.DutyType = 5000;
                            duty.GUID = Guid.NewGuid();
                            duty.IsTemplate = false;
                            duty.Remark = session.Title;
                            duty.UPD = 1;

                            duty.InitStart = duty.DateStart;
                            duty.InitEnd = duty.DateEnd;
                            duty.DateConfirmed = DateTime.Now;
                            duty.ConfirmedBy = "TRAINING";
                            var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003 };
                            duty.InitRestTo = duty.InitEnd; //rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
                                                            //rec.FDP = duty;
                            var csf = new CourseSessionFDP()
                            {
                                FDP = duty,
                                CourseId = session.CourseId,
                                SessionKey = String.Join("*", session.items.Select(q => q.Key)),
                                EmployeeId = emp.Id,
                                Date = session.Date,

                            };
                            foreach (var item in session.items)
                            {
                                csf.CourseSessionFDPItems.Add(new CourseSessionFDPItem()
                                {
                                    session_key = item.Key,
                                });
                            }
                            context.FDPs.Add(duty);
                            context.CourseSessionFDPs.Add(csf);



                            fdps.Add(duty);
                        }
                        else
                        {
                            errors.Add(new
                            {
                                PersonId = emp.PersonId,
                                FDPId = ofdp.Id,
                                EmployeeId = emp.Id,
                                // SessionItemId = session.Id,
                                Name = emp.Name,
                                Flights = ofdp.InitFlts,
                                Route = ofdp.InitRoute,
                                // DutyEnd = ofdp.DutyEndLocal,
                                DutyStart = ofdp.DutyStart,
                                RestUntil = ofdp.RestUntil,
                                CourseCode = session.Title,
                                CourseTitle = session.Title,
                                SessionDateFrom = session.start,
                                SessionDateTo = session.end,
                                DateCreate = DateTime.Now
                            });
                            errors_people.Add(emp.PersonId);
                        }

                    }
                }
            }

            var saveResult = await context.SaveAsync();

            var _sync_result = new sync_result()
            {
                errors = errors,
                fdps = fdps.Select(q => new sync_result_fdp()
                {
                    Id = q.Id,
                    CrewId = q.CrewId
                }).ToList(),
                saveErrors = saveResult.Errors
            };
            return _sync_result;
        }

        public async Task<DataResponse> SyncSessionsToRosterByDate(int cid)
        {
            var course = await context.Courses.FirstOrDefaultAsync(q => q.Id == cid);
            var subjects = await context.Courses.Where(q => q.ParentId == cid).ToListAsync();
            //List<ViewCourseSession> sessions = null;

            var sessions = await context.ViewCourseSessions.Where(q => q.CourseId == cid /*|| q.ParentId == cid*/).OrderBy(q => q.DateStart).ToListAsync();
            //if (sessions.Where(q => q.ParentId == cid).FirstOrDefault() != null)
            //{
            //    sessions = sessions.Where(q => q.ParentId == cid).OrderBy(q => q.DateStart).ToList();
            //}
            var grp_sessions = (from x in sessions
                                group x by new { ((DateTime)x.DateStart).Date, x.Title, x.CourseId/*, x.InstructorId*/ } into grp
                                select new
                                {
                                    grp.Key.Date,
                                    grp.Key.Title,
                                    grp.Key.CourseId,
                                    //  grp.Key.InstructorId,
                                    items = grp.OrderBy(q => q.DateStart).ToList(),
                                    start = ((DateTime)grp.Min(q => q.DateStart)),
                                    end = ((DateTime)grp.Max(q => q.DateEnd)),
                                    start_utc = ((DateTime)grp.Min(q => q.DateStartUtc)),
                                    end_utc = ((DateTime)grp.Max(q => q.DateEndUtc)),

                                }).ToList();
            var course_ids = new List<int>() { cid };
            course_ids = course_ids.Concat(grp_sessions.Select(q => q.CourseId).ToList()).ToList();
            var cps = await context.CoursePeoples.Where(q => q.CourseId == cid).ToListAsync();
            var personIds = cps.Select(q => q.PersonId).ToList();

            if (course.CurrencyId != null)
                personIds.Add((int)course.CurrencyId);
            if (course.Instructor2 != null)
                personIds.Add((int)course.Instructor2);

            List<int?> ins_ids = new List<int?>();
            if (course.CurrencyId != null)
                ins_ids.Add(course.CurrencyId);
            if (course.Instructor2 != null)
                ins_ids.Add(course.Instructor2);
            //var ins_ids = grp_sessions.Where(q => q.InstructorId != null).Select(q => q.InstructorId).Distinct().ToList();
            personIds = personIds.Concat(ins_ids).ToList();


            var fltcrew = new List<string>() { "P1", "P2", "ISCCM", "SCCM", "CCM", "TRE", "TRI", "LTC", "CCE", "CCI" };
            var employees = await context.ViewEmployeeAbs.Where(q => personIds.Contains(q.PersonId) && fltcrew.Contains(q.JobGroup)).ToListAsync();
            var eids = employees.Select(q => (Nullable<int>)q.PersonId).ToList();



            var course_people = await context.CoursePeoples.Where(q => eids.Contains(q.PersonId)).ToListAsync();
            var current_fdp_ids = await context.CourseSessionFDPs.Where(q => course_ids.Contains(q.CourseId)).Select(q => q.FDPId).ToListAsync();
            //var currents = await context.CourseSessionFDPs.Where(q =>course_ids.Contains( q.CourseId)).ToListAsync();
            //context.CourseSessionFDPs.RemoveRange(currents);
            var currents = await context.FDPs.Where(q => current_fdp_ids.Contains(q.Id)).ToListAsync();
            context.FDPs.RemoveRange(currents);

            var fdps = new List<FDP>();
            var errors = new List<object>();
            var errors_people = new List<int?>();
            course.Date_Sessions_Synced = DateTime.Now;
            course.Date_Sessions_Instructor_Synced = course.Date_Sessions_Synced;
            foreach (var session in grp_sessions)
            {
                foreach (var emp in employees)
                {
                    var _proceed = true;

                    if (_proceed)
                    {

                        var post_fdp = context.ViewFDPIdeas.Where(q => q.CrewId == emp.Id && q.DutyType == 1165 && q.DutyStart > session.end_utc).OrderBy(q => q.DutyStart).FirstOrDefault();
                        var in_fdp = context.ViewFDPIdeas.Where(q => q.CrewId == emp.Id && q.DutyType == 1165 &&
                                 (
                                    (q.DutyStart >= session.start_utc && q.DutyEnd <= session.end_utc) ||
                                    (q.DutyStart >= session.start_utc && q.DutyStart <= session.end_utc) ||
                                    (q.DutyEnd >= session.start_utc && q.DutyEnd <= session.end_utc) ||
                                    (session.start_utc >= q.DutyStart && session.end_utc <= q.DutyEnd)
                                 )

                            ).OrderBy(q => q.DutyStart).FirstOrDefault();
                        var ofdp = in_fdp;
                        if (ofdp == null && post_fdp != null)
                        {
                            if (((DateTime)post_fdp.DutyStart - (DateTime)session.end_utc).TotalMinutes < 12 * 60)
                                ofdp = post_fdp;
                        }

                        if (ofdp == null)
                        {
                            var duty = new FDP();
                            duty.DateStart = session.start_utc;
                            duty.DateEnd = session.end_utc;

                            duty.CrewId = emp.Id;
                            duty.DutyType = 5000;
                            duty.GUID = Guid.NewGuid();
                            duty.IsTemplate = false;
                            duty.Remark = session.Title;
                            duty.UPD = 1;

                            duty.InitStart = duty.DateStart;
                            duty.InitEnd = duty.DateEnd;
                            duty.DateConfirmed = DateTime.Now;
                            duty.ConfirmedBy = "TRAINING";
                            var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003 };
                            duty.InitRestTo = duty.InitEnd; //rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
                                                            //rec.FDP = duty;
                            var csf = new CourseSessionFDP()
                            {
                                FDP = duty,
                                CourseId = session.CourseId,
                                SessionKey = String.Join("*", session.items.Select(q => q.Key)),
                                EmployeeId = emp.Id,
                                Date = session.Date,

                            };
                            foreach (var item in session.items)
                            {
                                csf.CourseSessionFDPItems.Add(new CourseSessionFDPItem()
                                {
                                    session_key = item.Key,
                                });
                            }
                            context.FDPs.Add(duty);
                            context.CourseSessionFDPs.Add(csf);



                            fdps.Add(duty);
                        }
                        else
                        {
                            errors.Add(new
                            {
                                PersonId = emp.PersonId,
                                FDPId = ofdp.Id,
                                EmployeeId = emp.Id,
                                // SessionItemId = session.Id,
                                Name = emp.Name,
                                Flights = ofdp.InitFlts,
                                Route = ofdp.InitRoute,
                                // DutyEnd = ofdp.DutyEndLocal,
                                DutyStart = ofdp.DutyStart,
                                RestUntil = ofdp.RestUntil,
                                CourseCode = session.Title,
                                CourseTitle = session.Title,
                                SessionDateFrom = session.start,
                                SessionDateTo = session.end,
                                DateCreate = DateTime.Now
                            });
                            errors_people.Add(emp.PersonId);
                        }

                    }
                }
            }
            foreach (var cp in cps)
            {
                cp.IsSessionsSynced = false;
                var err = errors_people.FirstOrDefault(q => q == cp.PersonId);
                if (err == null)
                    cp.IsSessionsSynced = true;
            }
            var saveResult = await context.SaveAsync();

            var _sync_result = new sync_result()
            {
                errors = errors,
                fdps = fdps.Select(q => new sync_result_fdp()
                {
                    Id = q.Id,
                    CrewId = q.CrewId
                }).ToList(),
                saveErrors = saveResult.Errors
            };
            foreach (var subject in subjects)
            {
                var subj_result = await SyncSessionsToRosterByDateTeachers_Subjects(subject.Id);
                if (subj_result.fdps != null)
                    _sync_result.fdps = _sync_result.fdps.Concat(subj_result.fdps).ToList();
                if (subj_result.errors != null)
                    _sync_result.errors = _sync_result.errors.Concat(subj_result.errors).ToList();
                if (subj_result.saveErrors != null)
                    _sync_result.saveErrors = _sync_result.saveErrors.Concat(subj_result.saveErrors).ToList();
            }

            return new DataResponse()
            {
                Data = _sync_result,
                IsSuccess = saveResult.IsSuccess,
            };
        }

        public class sync_result_fdp
        {
            public int? Id { get; set; }
            public int? CrewId { get; set; }
        }
        public class sync_result
        {
            public List<object> errors { get; set; }
            public List<string> saveErrors { get; set; }
            public List<sync_result_fdp> fdps { get; set; }
        }
        //12-02
        public async Task<DataResponse> SyncSessionsToRosterTeachers(int cid)
        {
            var sessions = await context.ViewCourseSessions.Where(q => q.CourseId == cid || q.ParentId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var has_child = sessions.Where(q => q.ParentId == cid).FirstOrDefault() != null;
            if (has_child)
                sessions = sessions.Where(q => q.ParentId == cid).OrderBy(q => q.DateStart).ToList();
            var crs = await context.Courses.Where(q => q.Id == cid).FirstOrDefaultAsync();
            List<int> emps = new List<int>();
            if (!has_child)
            {
                if (crs.CurrencyId != null)
                    emps.Add((int)crs.CurrencyId);
                if (crs.Instructor2 != null)
                    emps.Add((int)crs.Instructor2);
            }
            else
            {
                emps = sessions.Where(q => q.InstructorId != null).Select(q => (int)q.InstructorId).Distinct().ToList();
            }

            // var cps = await context.CoursePeoples.Where(q => q.CourseId == cid).ToListAsync();
            //var personIds = cps.Select(q => q.PersonId).ToList();
            var fltcrew = new List<string>() { "P1", "P2", "ISCCM", "SCCM", "CCM", "TRE", "TRI", "LTC", "CCE", "CCI" };
            var employees = await context.ViewEmployeeAbs.Where(q => emps.Contains(q.PersonId) && fltcrew.Contains(q.JobGroup)).ToListAsync();
            var currents = await context.CourseSessionFDPs.Where(q => q.CourseId == cid).ToListAsync();
            var fdps = new List<FDP>();
            var errors = new List<object>();
            foreach (var session in sessions)
            {
                foreach (var emp in employees)
                {
                    var exist = currents.Where(q => q.EmployeeId == emp.Id && q.SessionKey == session.Key).FirstOrDefault();
                    if (exist == null)
                    {

                        //var ofdp = (from x in context.ViewFDPIdeas.AsNoTracking()
                        //            where x.CrewId == emp.Id && x.DutyType == 1165
                        //            && (
                        //              (session.DateStartUtc >= x.DutyStart && session.DateStartUtc <= x.RestUntil) || (session.DateEndUtc >= x.DutyStart && session.DateEndUtc <= x.RestUntil)
                        //              || (x.DutyStart >= session.DateStartUtc && x.DutyStart <= session.DateEndUtc) || (x.RestUntil >= session.DateStartUtc && x.RestUntil <= session.DateEndUtc)
                        //              )
                        //            select x).FirstOrDefault();

                        var post_fdp = context.ViewFDPIdeas.Where(q => q.CrewId == emp.Id && q.DutyType == 1165 && q.DutyStart > session.DateEndUtc).OrderBy(q => q.DutyStart).FirstOrDefault();
                        var in_fdp = context.ViewFDPIdeas.Where(q => q.CrewId == emp.Id && q.DutyType == 1165 &&
                                 (
                                    (q.DutyStart >= session.DateStartUtc && q.DutyEnd <= session.DateEndUtc) ||
                                    (q.DutyStart >= session.DateStartUtc && q.DutyStart <= session.DateEndUtc) ||
                                    (q.DutyEnd >= session.DateStartUtc && q.DutyEnd <= session.DateEndUtc) ||
                                    (session.DateStartUtc >= q.DutyStart && session.DateStartUtc <= q.DutyEnd)
                                 )

                            ).OrderBy(q => q.DutyStart).FirstOrDefault();
                        var ofdp = in_fdp;
                        if (ofdp == null && post_fdp != null)
                        {
                            if (((DateTime)post_fdp.DutyStart - (DateTime)session.DateStartUtc).TotalMinutes < 12 * 60)
                                ofdp = post_fdp;
                        }



                        if (ofdp == null)
                        {
                            var duty = new FDP();
                            duty.DateStart = session.DateStartUtc;
                            duty.DateEnd = session.DateEndUtc;

                            duty.CrewId = emp.Id;
                            duty.DutyType = 5000;
                            duty.GUID = Guid.NewGuid();
                            duty.IsTemplate = false;
                            duty.Remark = session.CT_Title + "\r\n" + session.Title;
                            duty.UPD = 1;
                            duty.DateConfirmed = DateTime.Now;
                            duty.ConfirmedBy = "TRAINING";

                            duty.InitStart = duty.DateStart;
                            duty.InitEnd = duty.DateEnd;
                            var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003 };
                            duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
                            //rec.FDP = duty;
                            var csf = new CourseSessionFDP()
                            {
                                FDP = duty,
                                CourseId = session.CourseId,
                                SessionKey = session.Key,
                                EmployeeId = emp.Id,

                            };
                            context.FDPs.Add(duty);
                            context.CourseSessionFDPs.Add(csf);


                            fdps.Add(duty);
                        }
                        else
                        {
                            errors.Add(new
                            {
                                FDPId = ofdp.Id,
                                EmployeeId = emp.Id,
                                SessionItemId = session.Id,
                                Name = emp.Name,
                                Flights = ofdp.InitFlts,
                                Route = ofdp.InitRoute,
                                // DutyEnd = ofdp.DutyEndLocal,
                                DutyStart = ofdp.DutyStart,
                                RestUntil = ofdp.RestUntil,
                                CourseCode = session.CT_Title,
                                CourseTitle = session.Title,
                                SessionDateFrom = session.DateStart,
                                SessionDateTo = session.DateEnd,
                                DateCreate = DateTime.Now
                            });
                        }
                    }

                }
            }
            var saveResult = await context.SaveAsync();
            return new DataResponse()
            {
                Data = new
                {
                    fdps = fdps.Select(q => new
                    {
                        q.Id,
                        q.CrewId
                    }).ToList(),
                    errors,
                    saveErrors = saveResult.Errors,
                },
                IsSuccess = saveResult.IsSuccess,
            };
        }

        public async Task<DataResponse> GetEmployeeCertificates(int id)
        {
            var certs = await context.AppCertificates.Where(q =>
                   q.CrewId == id

           ).OrderBy(q => q.StatusId).ThenBy(q => q.Remain).ToListAsync();



            return new DataResponse()
            {
                Data = certs,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetTeacherCourses(int id)
        {
            var certs = await context.ViewTeacherCourses.Where(q =>
                   q.Id == id

           ).OrderBy(q => q.DateStart).ToListAsync();



            return new DataResponse()
            {
                Data = certs,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetTeacherActiveCourses(int id)
        {
            var certs = await context.ViewTeacherCourses.Where(q =>
                   q.Id == id
                   && (q.Date_Sign_Ins1 == null || q.Date_Exam_Sign_Ins1 == null)

           ).OrderByDescending(q => q.DateStart).ToListAsync();



            return new DataResponse()
            {
                Data = certs,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetTeacherArchivedCourses(int id)
        {
            var certs = await context.ViewTeacherCourses.Where(q =>
                   q.Id == id
                   && (q.Date_Sign_Ins1 != null && q.Date_Exam_Sign_Ins1 != null)

           ).OrderByDescending(q => q.DateStart).ToListAsync();



            return new DataResponse()
            {
                Data = certs,
                IsSuccess = true,
            };
        }

        //2025-01-04
        public async Task<DataResponse> GetDirectorActiveCourses(int id)
        {
            //var certs = await context.ViewTeacherCourses.Where(q =>
            var certs = await context.ViewCourseNews.Where(q =>
                    //q.Id == id
                    (q.Date_Sign_Ins1 != null /*&& q.Date_Exam_Sign_Ins1 != null*/)
           // && q.Date_Sign_Director == null

           ).OrderBy(q => q.Date_Sign_Director).OrderByDescending(q => q.DateStart).ToListAsync();



            return new DataResponse()
            {
                Data = certs,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetDirectorArchivedCourses(int id)
        {
            var certs = await context.ViewTeacherCourses.Where(q =>
                    //q.Id == id
                    (q.Date_Sign_Ins1 != null && q.Date_Exam_Sign_Ins1 != null)
                    && q.Date_Sign_Director != null

           ).OrderByDescending(q => q.DateStart).ToListAsync();



            return new DataResponse()
            {
                Data = certs,
                IsSuccess = true,
            };
        }
        //UpdateExamStudentAnswer
        public async Task<DataResponse> UpdateExamStudentAnswer(dto_exam_student_answer dto)
        {
            var entity = await context.trn_exam_student_answer.Where(q =>
                   q.person_id == dto.person_id
                   && q.question_id == dto.question_id

           ).FirstOrDefaultAsync();
            if (entity == null)
            {
                entity = new trn_exam_student_answer()
                {
                    person_id = dto.person_id,
                    question_id = dto.question_id,


                };

                context.trn_exam_student_answer.Add(entity);
            }

            entity.date_sent = DateTime.Now;
            entity.answer_id = dto.answer_id;
            await context.SaveChangesAsync();

            return new DataResponse()
            {
                Data = entity.id,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCertificateUrl(int person, int type)
        {
            var cp = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == person && q.CertificateTypeId == type && q.RankLast == 1).FirstOrDefaultAsync();
            if (cp == null)
                return new DataResponse()
                {
                    Data = new ViewCoursePeoplePassedRanked() { Id = -1 },
                    IsSuccess = true,
                };
            if (string.IsNullOrEmpty(cp.ImgUrl))
                return new DataResponse()
                {
                    Data = new ViewCoursePeoplePassedRanked() { Id = -1 },
                    IsSuccess = true,
                };
            return new DataResponse()
            {
                Data = cp,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetPersonDocumentFile(int pid, int tid)
        {
            //var query = from x in context.ViewEmployeeTrainings select x;
            //if (root != "000")
            //    query = query.Where(q => q.JobGroupMainCode == root);
            //var result = await query.OrderByDescending(q => q.MandatoryExpired).ThenBy(q => q.JobGroup).ThenBy(q => q.LastName).ToListAsync();
            var obj = context.ViewPersonDocumentFileXes.Where(q => q.PersonId == pid && q.DocumentTypeId == tid).OrderByDescending(q => q.Id).FirstOrDefaultAsync();
            if (obj != null)
                return new DataResponse()
                {
                    Data = obj,
                    IsSuccess = true,
                };
            else
                return new DataResponse()
                {
                    Data = new ViewPersonDocumentFileX() { Id = -1, },
                    IsSuccess = true,
                };
        }

        public async Task<DataResponse> GetPersonCertificateDocument(int pid, int tid, string type)
        {
            //var query = from x in context.ViewEmployeeTrainings select x;
            //if (root != "000")
            //    query = query.Where(q => q.JobGroupMainCode == root);
            //var result = await query.OrderByDescending(q => q.MandatoryExpired).ThenBy(q => q.JobGroup).ThenBy(q => q.LastName).ToListAsync();
            //var obj =await context.ViewPersonDocumentFileXes.Where(q => q.PersonId == pid && q.DocumentTypeId == tid).OrderByDescending(q => q.Id).FirstOrDefaultAsync();
            string fileUrl = null;
            var pdoc = await context.EmployeeDocuments.FirstOrDefaultAsync(q => q.PersonId == pid && q.Type == type);
            if (pdoc == null)
            {
                var obj = await context.ViewPersonDocumentFileXes.Where(q => q.PersonId == pid && q.DocumentTypeId == tid).OrderByDescending(q => q.Id).FirstOrDefaultAsync();
                if (obj != null)
                    fileUrl = obj.FileUrl;
            }
            else
                fileUrl = pdoc.Url;

            //SMSL2L3
            if (tid == -1000)
            {
                var cp_l2 = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.CertificateTypeId == 197 && q.RankLast == 1).FirstOrDefaultAsync();
                var cp_l3 = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.CertificateTypeId == 198 && q.RankLast == 1).FirstOrDefaultAsync();
                var emp = await context.ViewEmployees.Where(q => q.PersonId == pid).FirstOrDefaultAsync();
                ViewCoursePeoplePassedRanked cp = cp_l2;
                if (cp_l2 == null)
                    cp = cp_l3;
                else
                {
                    if (cp_l3 != null && cp_l3.DateExpire >= cp_l2.DateExpire)
                    {
                        cp = cp_l3;
                    }
                }
                return new DataResponse()
                {
                    Data = new { certificate = cp, document = new { FileUrl = fileUrl }, employee = emp },
                    IsSuccess = true,
                };
            }
            else if (tid == -1001)
            {
                var cp_l1 = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.CertificateTypeId == 6 && q.RankLast == 1).FirstOrDefaultAsync();
                var cp_l2 = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.CertificateTypeId == 197 && q.RankLast == 1).FirstOrDefaultAsync();
                var cp_l3 = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.CertificateTypeId == 198 && q.RankLast == 1).FirstOrDefaultAsync();

                var cp_l1_exp = cp_l1 == null || cp_l1.DateExpire == null ? new DateTime(1900, 1, 1) : cp_l1.DateExpire;

                var cp_l2_exp = cp_l2 == null || cp_l2.DateExpire == null ? new DateTime(1900, 1, 1) : cp_l2.DateExpire;
                var cp_l3_exp = cp_l3 == null || cp_l3.DateExpire == null ? new DateTime(1900, 1, 1) : cp_l3.DateExpire;

                var emp = await context.ViewEmployees.Where(q => q.PersonId == pid).FirstOrDefaultAsync();

                DateTime? cp_date = cp_l1_exp;


                ViewCoursePeoplePassedRanked cp = cp_l1;

                if (cp_l2_exp >= cp_date)
                {
                    cp = cp_l2;
                    cp_date = cp_l2_exp;

                }
                if (cp_l3_exp >= cp_date)
                {
                    cp = cp_l3;
                    cp_date = cp_l3_exp;
                }


                return new DataResponse()
                {
                    Data = new { certificate = cp, document = new { FileUrl = fileUrl }, employee = emp },
                    IsSuccess = true,
                };
            }
            else
            {
                var cp = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.CertificateTypeId == tid && q.RankLast == 1).FirstOrDefaultAsync();
                var emp = await context.ViewEmployees.Where(q => q.PersonId == pid).FirstOrDefaultAsync();
                return new DataResponse()
                {
                    Data = new { certificate = cp, document = new { FileUrl = fileUrl }, employee = emp },
                    IsSuccess = true,
                };
            }


        }


        public async Task<DataResponse> GetTrnStatCoursePeople(DateTime df, DateTime dt, int? ct, int? status, int? cstatus, string cls, int? pid, int? inst1, int? inst2, int? rank, int? active, string grp, string dep)
        {
            var _df = df.Date;
            var _dt = dt.Date.AddDays(1);
            var query = from x in context.ViewCoursePeopleRankedByStarts
                        where x.DateStart >= _df && x.DateStart <= _dt
                        // && x.PersonId== 3366
                        select x;
            if (ct != -1)
            {
                query = query.Where(q => q.CourseTypeId == ct);
            }
            if (status != -2)
            {
                query = query.Where(q => q.CoursePeopleStatusId == status);
            }
            if (cstatus != -1)
            {
                query = query.Where(q => q.StatusId == cstatus);
            }
            if (cls != "-1")
            {
                query = query.Where(q => q.No == cls);
            }
            if (inst1 != -1)
            {
                query = query.Where(q => q.Instructor1Id == inst1);
            }
            if (inst2 != -1)
            {
                query = query.Where(q => q.Instructor2Id == inst2);
            }
            if (rank != -1)
            {
                query = query.Where(q => q.RankLast == 1);
            }
            if (active != -1)
            {
                query = query.Where(q => q.CustomerId == 0);
            }
            else query = query.Where(q => q.CustomerId == 1);

            if (grp != "-1")
            {
                if (dep == "-1")
                    query = query.Where(q => q.JobGroupCode.StartsWith(grp));
                else
                    query = query.Where(q => q.ProfileGroup == grp);

            }


            var result = await query.OrderBy(q => q.LastName).ThenBy(q => q.FirstName).ThenBy(q => q.CourseType).ThenBy(q => q.RankLast).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> SavePersonDoc(int personId, string type, string url)
        {
            EmployeeDocument doc = await context.EmployeeDocuments.FirstOrDefaultAsync(q => q.PersonId == personId && q.Type == type);
            if (doc == null)
            {
                doc = new EmployeeDocument() { PersonId = personId, Type = type };
                context.EmployeeDocuments.Add(doc);
            }
            doc.Url = url;


            var saveResult = await context.SaveAsync();
            return new DataResponse()
            {
                Data = doc,
                IsSuccess = saveResult.IsSuccess,
            };
        }

        public async Task<DataResponse> SaveCourseFP(int courseId, string url)
        {
            var crs = await context.Courses.FirstOrDefaultAsync(q => q.Id == courseId);
            crs.AttForm = url;


            var saveResult = await context.SaveAsync();
            return new DataResponse()
            {
                Data = crs,
                IsSuccess = saveResult.IsSuccess,
            };
        }

        public async Task<DataResponse> SaveGroupsManager(int managerId, List<int> ids)
        {
            var crs = await context.JobGroups.Where(q => ids.Contains(q.Id)).ToListAsync();
            foreach (var x in crs)
            {
                x.Manager = managerId;
            }


            var saveResult = await context.SaveAsync();
            return new DataResponse()
            {
                Data = crs,
                IsSuccess = saveResult.IsSuccess,
            };
        }


        public async Task<DataResponse> GetManagerGroups(int mng_id)
        {


            var query = await context.ManagerGroups.Where(q => q.ManagerId == mng_id).Select(q => q.ProfileGroup).ToListAsync();



            return new DataResponse()
            {
                Data = query,
                IsSuccess = true,
            };

        }






    }
}