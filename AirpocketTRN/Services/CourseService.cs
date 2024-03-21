﻿using AirpocketTRN.Models;
using AirpocketTRN.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var _exg = new List<string>() { "TRE", "TRI", "P1", "P2", "ISCCM", "SCCM", "CCM" };
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


        public async Task<DataResponse> GetTrainingExpiredCertificateTypes(int year, int month, int certificate_type_id,int mng_id)
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


        public async Task<DataResponse> GetTrainingSchedule(int year, int month,int mng_id)
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
                                          where mng_grps.Contains(x.JobGroupRoot) && x.Year==year
                                          select x;
                 
                filter_course_ids =await course_people_query.Select(q => q.CourseId).Distinct().ToListAsync();
                filter_employee_ids = await context.ViewProfiles.Where(q => mng_grps.Contains(q.JobGroupRoot)).Select(q => q.PersonId).ToListAsync();
            }

            var session_query = from x in context.ViewCourseSessions
                                where x.Year == year && x.Month == month
                                select x;
            if (mng_id != -1)
                session_query = session_query.Where(q => filter_course_ids.Contains(q.CourseId));

            var session_query_list = await (session_query).ToListAsync();
            var sessions = (from x in session_query_list
                            group x by new { ((DateTime)x.DateStart).Date } into grp
                            select new
                            {
                                Date = grp.Key.Date,
                                Items = grp.Select(q => new
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
                                }).OrderBy(w => w.DateStart).ThenBy(w => w.Title).ToList()
                            }).OrderBy(q => q.Date).ToList();

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


        public async Task<DataResponse> GetTrainingSchedule(int year,int mng_id)
        {
            List<ViewCoursePeople> course_people = new List<ViewCoursePeople>();
            Manager mng = null;
            List<string> mng_grps = null;
            List<int> filter_course_ids = null;
            List<int> filter_employee_ids = null;
            if (mng_id != -1)
            {
                mng = await context.Managers.FirstOrDefaultAsync(q => q.EmployeeId == mng_id);
                mng_grps = await context.ManagerGroups.Where(q => q.ManagerId == mng_id).Select(q=>q.ProfileGroup).ToListAsync();

                var course_people_query = from x in context.ViewCoursePeoples
                                          where mng_grps.Contains(x.JobGroupRoot) && x.Year==year
                                          select x;
                course_people = await course_people_query.ToListAsync();
                filter_course_ids = course_people.Select(q => q.CourseId).Distinct().ToList();
                filter_employee_ids = await context.ViewProfiles.Where(q=>mng_grps.Contains(q.JobGroupRoot)).Select(q => q.PersonId).ToListAsync();
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

            var courses =await (from x in context.ViewCourseNews

                           where x.DateStart >= df && x.DateStart < dt  
                           group x by new { x.CourseType } into grp
                           select new
                           {

                               Type = grp.Key.CourseType,
                               Scheduled = grp.Where(q => q.StatusId == 1).Count(),
                               InProgress = grp.Where(q => q.StatusId == 2).Count(),
                               Done = grp.Where(q => q.StatusId == 3).Count(),
                               Canceled = grp.Where(q => q.StatusId == 4).Count(),
                               Duration=grp.Where(q=>q.StatusId!=4 ).Sum(q=>q.Duration),


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
                               where x.JobGroupCode2.StartsWith(jg) && x.StatusId !=4 && x.DateStart >= df && x.DateStart < dt
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
                                   Status=grp.Key.CoursePeopleStatus,
                                   Count = grp.Count(),
                               };
            var passed =await query_passed.OrderBy(q => q.Count).ToListAsync();

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
                              group x by new { Group = x.JobGroupCode2.Substring(0,7) } into grp
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

        public async Task<DataResponse> GetCoursePeople(int cid)
        {
            var result = await context.ViewCoursePeoples.OrderBy(q => q.CourseId == cid).ToListAsync();

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

        public async Task<DataResponse> GetCourseViewObject(int cid)
        {
            var course = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();
            var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var syllabi = await context.CourseSyllabus.Where(q => q.CourseId == cid).ToListAsync();

            return new DataResponse()
            {
                Data = new
                {
                    course,
                    sessions,
                    syllabi

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
        public async Task<DataResponse> GetTrainingCard(int pid)
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
                    ImageUrl = result.First().ImageUrl,
                    
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

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
                    ImageUrl = result.First().ImageUrl,
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
                    DateExpire = person.LineExpireDate == null ? null : (Nullable<DateTime>)((DateTime)person.LineExpireDate) ,
                    Interval = 12,
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
                    PersonId = employee.PersonId,
                });

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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    ImageUrl = result.First().ImageUrl,
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
                    DateExpire = person.RecurrentIssueDate == null ? null : (Nullable<DateTime>)((DateTime)person.RecurrentIssueDate).AddYears(1),
                    Interval = 12,
                    ImageUrl = result.First().ImageUrl,
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
                    DateExpire = person.LineIssueDate == null ? null : (Nullable<DateTime>)((DateTime)person.LineIssueDate).AddYears(1),

                    Interval = 12,
                    ImageUrl = result.First().ImageUrl,
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
                });
            }

            await context.SaveChangesAsync();
            dto.Id = entity.Id;

            await SaveCourseTypeNotApplicable(new course_type_notapplicable_viewmodel() { 
              groups=dto.not_applicables,
               type_id=dto.Id
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

        public async Task<DataResponse> SaveCourseTypeNotApplicable( course_type_notapplicable_viewmodel  dto)
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
                    CourseTypeId =type_id,
                    TrainingGroup = item  ,
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
            var last_history = context.ViewCertificateHistoryRankeds.Where(q => q.RankOrder == 1 && profile_ids.Contains(q.PersonId) && q.CertificateTypeId == certificate_type.Id).Select(q=>q.Id).ToList();
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
        public async Task<DataResponse> SaveCourse(ViewModels.CourseViewModel dto)
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
                    foreach (var x in dto.Syllabi)
                    {
                        entity.CourseSyllabus.Add(new CourseSyllabu()
                        {
                            Duration = x.Duration,
                            Title = x.Title,
                        });
                    }
                }
            }
            else
            {
                var _syllabi = await context.CourseSyllabus.Where(q => q.CourseId == dto.Id).ToListAsync();
                var _syllabiIds = _syllabi.Select(q => q.Id).ToList();
                var _dtoIds = dto.Syllabi.Select(q => q.Id).ToList();
                var _deletedSyl = _syllabi.Where(q => !_dtoIds.Contains(q.Id)).ToList();
                context.CourseSyllabus.RemoveRange(_deletedSyl);

                var newSyllabi = dto.Syllabi.Where(q => q.Id < 0).ToList();
                foreach (var x in newSyllabi)
                {
                    entity.CourseSyllabus.Add(new CourseSyllabu() { Duration = x.Duration, Title = x.Title });
                }


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
            dto.Id = entity.Id;
            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }

        public async Task<DataResponse> SaveCertificate(ViewModels.CertificateViewModel dto)
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

        public async Task<DataResponse> SaveCoursePeople(dynamic dto)
        {
            int courseId = Convert.ToInt32(dto.Id);
            string pid = Convert.ToString(dto.pid);
            // string eid = Convert.ToString(dto.eid);

            var personIds = pid.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            // var employeeIds = eid.Split('-').Select(q => Convert.ToInt32(q)).ToList();

            var exists = await context.CoursePeoples.Where(q => q.CourseId == courseId).Select(q => q.PersonId).ToListAsync();
            var newids = personIds.Where(q => !exists.Contains(q)).ToList();

            foreach (var id in newids)
            {
                context.CoursePeoples.Add(new CoursePeople()
                {
                    CourseId = courseId,
                    PersonId = id,
                    StatusId = -1,
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
            var syllabus = await context.CourseSyllabus.Where(q => q.Id == Id).FirstOrDefaultAsync();
            syllabus.Remark = Remark;
            syllabus.Status = Done;
            syllabus.InstructorId = Instructor;
            syllabus.SessionKey = Session;



            await context.SaveChangesAsync();
            var syll = await context.ViewSyllabus.Where(q => q.Id == Id).FirstOrDefaultAsync();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = syll,
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
                    DateExpire = not_applicable ==null? cp.DateExpire:null,
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
                if (cp.DateExpire == null  && !string.IsNullOrEmpty(certificate_type.IssueField))
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
            return query;
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

        public async Task<DataResponse> GetCoursePeopleAndSessions(int cid)
        {
            
            var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var people = await context.ViewCoursePeoples.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            //var press = await context.CourseSessionPresences.Where(q => q.CourseId == cid).ToListAsync();
            var press = await context.ViewCourseSessionPresences.Where(q => q.CourseId == cid).ToListAsync();
            var syllabi = await context.ViewSyllabus.Where(q => q.CourseId == cid).ToListAsync();


            return new DataResponse()
            {
                Data = new
                {
                    sessions,
                    people,
                    press,
                    syllabi
                },
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetCoursePeopleAndSessionsByDate(DateTime dt,int pid)
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
                            Sessions = grp.OrderBy(q=>q.DateStart).Select(q => new { q.DateStart, q.DateEnd }).ToList()
                        }).ToList();
            return new DataResponse()
            {
                Data = grps,
                IsSuccess = true,
            };

        }

        public async Task<DataResponse> NotifyCoursePeople(int cid, string recs)
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

        public async Task<DataResponse> SyncSessionsToRoster(int cid)
        {
            var sessions = await context.ViewCourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var cps = await context.CoursePeoples.Where(q => q.CourseId == cid).ToListAsync();
            var personIds = cps.Select(q => q.PersonId).ToList();
            var fltcrew = new List<string>() { "P1", "P2", "ISCCM", "SCCM", "CCM", "TRE", "TRI", "LTC" };
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

                        var ofdp = (from x in context.ViewFDPIdeas.AsNoTracking()
                                    where x.CrewId == emp.Id && x.DutyType == 1165
                                    && (
                                      (session.DateStartUtc >= x.DutyStart && session.DateStartUtc <= x.RestUntil) || (session.DateEndUtc >= x.DutyStart && session.DateEndUtc <= x.RestUntil)
                                      || (x.DutyStart >= session.DateStartUtc && x.DutyStart <= session.DateEndUtc) || (x.RestUntil >= session.DateStartUtc && x.RestUntil <= session.DateEndUtc)
                                      )
                                    select x).FirstOrDefault();
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


        public async Task<DataResponse> SyncSessionsToRosterTeachers(int cid)
        {
            var sessions = await context.ViewCourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var crs = await context.Courses.Where(q => q.Id == cid).FirstOrDefaultAsync();
            List<int> emps = new List<int>();
            if (crs.CurrencyId != null)
                emps.Add((int)crs.CurrencyId);
            if (crs.Instructor2 != null)
                emps.Add((int)crs.Instructor2);
            // var cps = await context.CoursePeoples.Where(q => q.CourseId == cid).ToListAsync();
            //var personIds = cps.Select(q => q.PersonId).ToList();
            var fltcrew = new List<string>() { "P1", "P2", "ISCCM", "SCCM", "CCM", "TRE", "TRI", "LTC" };
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

                        var ofdp = (from x in context.ViewFDPIdeas.AsNoTracking()
                                    where x.CrewId == emp.Id && x.DutyType == 1165
                                    && (
                                      (session.DateStartUtc >= x.DutyStart && session.DateStartUtc <= x.RestUntil) || (session.DateEndUtc >= x.DutyStart && session.DateEndUtc <= x.RestUntil)
                                      || (x.DutyStart >= session.DateStartUtc && x.DutyStart <= session.DateEndUtc) || (x.RestUntil >= session.DateStartUtc && x.RestUntil <= session.DateEndUtc)
                                      )
                                    select x).FirstOrDefault();
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
            if (tid ==  -1000 )
            {
                var cp_l2=await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.CertificateTypeId == 197 && q.RankLast == 1).FirstOrDefaultAsync();
                var cp_l3 = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.CertificateTypeId == 198 && q.RankLast == 1).FirstOrDefaultAsync();
                var emp = await context.ViewEmployees.Where(q => q.PersonId == pid).FirstOrDefaultAsync();
                ViewCoursePeoplePassedRanked cp = cp_l2;
                if (cp_l2 == null)
                    cp = cp_l3;
                else
                {
                    if (cp_l3 != null && cp_l3.DateExpire>=cp_l2.DateExpire)
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

                var cp_l1_exp = cp_l1 == null || cp_l1.DateExpire==null ? new DateTime(1900, 1, 1) : cp_l1.DateExpire;
                
                var cp_l2_exp= cp_l2 == null || cp_l2.DateExpire == null ? new DateTime(1900, 1, 1) : cp_l2.DateExpire;
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


        public async Task<DataResponse> GetTrnStatCoursePeople(DateTime df, DateTime dt, int? ct, int? status, int? cstatus, string cls, int? pid, int? inst1, int? inst2, int? rank, int? active, string grp,string dep)
        {
            var _df = df.Date;
            var _dt = dt.Date.AddDays(1);
            var query = from x in context.ViewCoursePeopleRankedByStarts
                        where x.DateStart >= _df && x.DateStart <= _dt
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


            var query =await context.ManagerGroups.Where(q => q.ManagerId == mng_id).Select(q=>q.ProfileGroup).ToListAsync();

             

            return new DataResponse()
            {
                Data = query,
                IsSuccess = true,
            };

        }






    }
}