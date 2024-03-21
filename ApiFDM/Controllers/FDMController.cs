﻿using System;
using System.Collections.Generic;
using System.Linq;
using ApiFDM.Objects;
using ApiFDM.Models;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data;
using System.Data.Entity;

using System.Data.SqlClient;



namespace ApiFDM.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FDMController : ApiController
    {
        dbEntities context = new dbEntities();

        ////Main Dashboard////
       
        [HttpGet]
        [Route("api/fdm/dashboard/eventname/{ymf}/{ymt}/{ACType}")]
        public async Task<DataResponse> GetAllFDMEventNameMD(int ymf, int ymt, string ACType)
        {
            var query = from x in context.FDMEventMonthlies
                            //where x.Day >= df && x.Day <= dt
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && x.AircraftType.Contains(ACType)
                        group x by new { x.EventName } into grp
                        select new
                        {
                            grp.Key.EventName,
                            IncidentCount = grp.Sum(q => q.EventCount),
                            HighLevelCount = grp.Sum(q => q.HighCount),
                            MediumLevelCount = grp.Sum(q => q.MediumCount),
                            LowLevelCount = grp.Sum(q => q.LowCount),
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),

                        };


            var ds = query.OrderByDescending(q => q.Scores);
            var result = new
            {
                data = ds,
                TotalHighLevels = ds.Sum(q => q.HighLevelCount),
                TotalMediumLevels = ds.Sum(q => q.MediumLevelCount),
                TotalLowLevels = ds.Sum(q => q.LowLevelCount),

            };


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };

        }

        [HttpGet]
        [Route("api/fdm/register/byMonth/{ymf}/{ymt}/{ACType}")]
        public async Task<DataResponse> GetEventByRegMonthly(int ymf, int ymt, string ACType)
        {
            try
            {
                var query = from x in context.FDMRegMonthlies
                            where x.YearMonth >= ymf && x.YearMonth <= ymt && x.aircrafttype.Contains(ACType)
                            group x by new { x.Register, x.RegisterId } into grp
                            select new
                            {
                                grp.Key.Register,
                                EventCount = grp.Sum(q => q.EventCount),
                                FlightCount = grp.Sum(q => q.FlightCount),
                                HighCount = grp.Sum(q => q.HighCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                HighScore = grp.Sum(q => q.HighCount) * 4,
                                LowScore = grp.Sum(q => q.LowCount),
                                MediumScore = grp.Sum(q => q.MediumCount) * 2,
                                Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),

                                ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventPerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,


                            };

                var ds = query.ToList();
                var result = new
                {
                    data = ds,
                    TotalFilght = ds.Sum(q => q.FlightCount),
                    TotalEvent = ds.Sum(q => q.EventCount),
                    TotalHighLevel = ds.Sum(q => q.HighCount),
                    TotalMediumLevel = ds.Sum(q => q.MediumCount),
                    TotalLowLevel = ds.Sum(q => q.LowCount),
                    TotalScores = ds.Sum(q => q.Scores),
                    AverageEvents = ds.Average(q => q.EventCount),
                    AverageScores = ds.Average(q => q.Scores),

                };

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = ex.InnerException
                };
            }
        }

        [HttpGet]
        [Route("api/get/fdm/top/cpt/{ymf}/{ymt}/{ACType}")]
        public async Task<DataResponse> GetTopCptMD(int ymf, int ymt, string ACType)
        {

            try
            {
                var kosquery = await (from x in context.FDMCptMonthlyTBLs
                                      where x.YearMonth >= ymf && x.YearMonth <= ymt
                                      select x).ToListAsync();


                var query = from x in kosquery
                            where x.YearMonth >= ymf && x.YearMonth <= ymt && x.AircraftType.Contains(ACType)
                            group x by new { x.CptName, x.CptId, x.CptCode } into grp


                            select new
                            {
                                grp.Key.CptCode,
                                grp.Key.CptId,
                                grp.Key.CptName,
                                HighCount = grp.Sum(q => q.HighCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                FlightCount = grp.Sum(q => q.FlightCount),
                                Score = grp.Sum(q => q.Score),
                                HighScore = grp.Sum(q => q.HighScore),
                                LowScore = grp.Sum(q => q.LowScore),
                                MediumScore = grp.Sum(q => q.MediumScore),

                                ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                                EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,
                            };

                var result = query.OrderByDescending(q => q.ScorePerFlight).Take(10).ToList();


                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    Data = ex.InnerException,
                    IsSuccess = false
                };
            }
        }

        [HttpGet]
        [Route("api/fdm/compare/top/cpt/{ymf}/{ymt}/{ACType}")]
        public async Task<DataResponse> CompareTopCpt(int ymf, int ymt, string ACType)
        {

            int year = Convert.ToInt32(ymt.ToString().Substring(0, 4));
            int month = Convert.ToInt32(ymt.ToString().Substring(4));
            var d1 = new DateTime(year, month, 1);
            var secondYMF = Convert.ToInt32(d1.AddMonths(-12).ToString("yyyyMM"));


            var min_from = Math.Min(ymf, secondYMF);
            var max_to = ymt;

            try
            {
                var kosquery = await (from x in context.FDMCptMonthlyTBLs
                                      where x.YearMonth >= min_from && x.YearMonth <= max_to && x.AircraftType.Contains(ACType)
                                      select x).ToListAsync();

                var qry_max = (from x in kosquery//context.FDMCptMonthlies
                               where x.YearMonth >= ymf && x.YearMonth <= ymt && x.AircraftType.Contains(ACType)
                               group x by new { x.CptId } into grp
                               select new
                               {

                                   grp.Key.CptId,
                                   Idx = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0

                               }).OrderByDescending(q => q.Idx).Take(10);



                var cptIds = qry_max.Select(q => q.CptId).ToList();

                var query = from x in kosquery//context.FDMCptMonthlies
                            where x.YearMonth >= secondYMF && x.YearMonth <= ymt && cptIds.Contains(x.CptId)
                            group x by new { x.CptId, x.CptName } into grp
                            select new
                            {

                                grp.Key.CptName,
                                grp.Key.CptId,
                                EventPerFlight = grp.Sum(q => q.EventPerFlight),
                                ScoreperFlight = grp.Sum(q => q.ScorePerFlight),
                                Items = grp.OrderByDescending(q => q.YearMonth).ToList()
                            };

                var result = query.ToList();




                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    Data = ex.InnerException,
                    IsSuccess = false
                };
            }
        }

        [HttpGet]
        [Route("api/fdm/compare/top/register/{ymf}/{ymt}/{ACType}")]
        public async Task<DataResponse> CompareRegister(int ymf, int ymt, string ACType)
        {

            int year = Convert.ToInt32(ymt.ToString().Substring(0, 4));
            int month = Convert.ToInt32(ymt.ToString().Substring(4));
            var d1 = new DateTime(year, month, 1);
            var secondYMF = Convert.ToInt32(d1.AddMonths(-12).ToString("yyyyMM"));

            try
            {
                var qry_max = (from x in context.FDMRegMonthlies
                               where x.YearMonth >= ymf && x.YearMonth <= ymt
                               group x by new { x.Register, x.RegisterId } into grp
                               select new
                               {
                                   grp.Key.RegisterId,
                                   grp.Key.Register,
                                   TotlaScore = grp.Sum(q => q.ScorePerEvent),
                               }).OrderByDescending(q => q.TotlaScore).Take(10).ToList();

                var cptIds = qry_max.Select(q => q.RegisterId).ToList();

                var query = from x in context.FDMRegMonthlies
                            where x.YearMonth >= secondYMF && x.YearMonth <= ymt && cptIds.Contains(x.RegisterId) && x.aircrafttype.Contains(ACType)
                            group x by new { x.Register, x.RegisterId } into grp
                            select new
                            {

                                grp.Key.Register,
                                grp.Key.RegisterId,
                                ScorePerFlight = grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.FlightCount),

                                ScorePerevent = grp.Sum(q => q.ScorePerEvent),

                                Items = grp.OrderByDescending(q => q.YearMonth).ToList()
                            };

                var result = query.ToList();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    Data = ex.InnerException,
                    IsSuccess = false
                };
            }
        }

        [HttpGet]
        [Route("api/fdm/dashboard/all/events/{ymf}/{ymt}/{ACType}")]
        public async Task<DataResponse> GetEventsDaily(int ymf, int ymt, string ACType)
        {
            var query = from x in context.FDMMonthlies
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && x.AircraftType.Contains(ACType)
                        group x by new { x.YearMonth } into grp
                        select new
                        {
                            YearMonth = grp.Key.YearMonth.HasValue ? grp.Key.YearMonth.ToString() : string.Empty,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighLevelCount = grp.Sum(q => q.HighCount),
                            MediumLevelCount = grp.Sum(q => q.MediumCount),
                            LowLevelCount = grp.Sum(q => q.LowCount),
                            EventsCount = grp.Sum(q => q.EventCount),
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                            HighPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.HighCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            MediumPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.MediumCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            LowPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.LowCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            HighScore = grp.Sum(q => q.HighCount) * 4,
                            LowScore = grp.Sum(q => q.LowCount),
                            MediumScore = grp.Sum(q => q.MediumCount) * 2,

                            ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100.0,

                        };
            var ds = query.ToList();
            var result = new
            {
                data = ds,
                TotalFlightCount = ds.Sum(q => q.FlightCount),
                TotalEventsCount = ds.Sum(q => q.EventsCount),
                TotalScores = ds.Sum(q => q.Scores),
                EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventsCount) * 1.0 / ds.Sum(q => q.FlightCount) : 0,
                ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Scores) * 1.0/ ds.Sum(q => q.FlightCount) * 1.0,
                TotalHighCount = ds.Sum(q => q.HighLevelCount),
                TotalMediumCount = ds.Sum(q => q.MediumLevelCount),
                TotalLowCount = ds.Sum(q => q.LowLevelCount),
                TotalHighScore = ds.Sum(q => q.HighScore),
                TotalMediumScore = ds.Sum(q => q.MediumScore),
                TotalLowScore = ds.Sum(q => q.LowScore),
            };


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };

        }


        [HttpGet]
        [Route("api/fdm/dashboard/monthly/{year}/{month}/{ACType}")]
        public async Task<DataResponse> fdmdashboardMonthly(int year, int month, string ACType)
        {
            var d1 = new DateTime(year, month, 1);
            var dates = new List<int?>();
            dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            for (var i = 0; i <= 11; i++)
            {
                d1 = d1.AddMonths(-1);
                dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            }

            try
            {
                var query = from x in context.FDMMonthlies
                            where dates.Contains(x.YearMonth) && x.AircraftType.Contains(ACType)
                            group x by new { x.YearMonth } into grp
                            select new
                            {
                                YearMonth = grp.Key.YearMonth.HasValue ? grp.Key.YearMonth.ToString() : string.Empty,
                                HighCount = grp.Sum(q => q.HighCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                FlightCount = grp.Sum(q => q.FlightCount),
                                Score = grp.Sum(q => q.Score),
                                HighScore = grp.Sum(q => q.HighCount) * 4,
                                LowScore = grp.Sum(q => q.LowCount),
                                MediumScore = grp.Sum(q => q.MediumCount) * 2,

                                ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                                EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,

                                //ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                //EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                                //EventPerFlight = grp.Sum(q => q.EventCount) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                //ScorePerEvent = grp.Sum(q => q.ScorePerEvent)

                            };
                var result = query.ToList();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse() { Data = ex.InnerException, IsSuccess = false };
            }
        }

        [HttpGet]
        [Route("api/fdm/route/{ymf}/{ymt}/{ACType}")]
        public async Task<DataResponse> GetFDMRoute(int ymf, int ymt, string ACType)
        {
            try
            {
                var query = from x in context.FDMAirportMonthlies
                            where x.YearMonth >= ymf && x.YearMonth <= ymt && x.AircraftType.Contains(ACType)
                            group x by new { x.Route } into grp
                            select new
                            {
                                grp.Key.Route,
                                HighCount = grp.Sum(q => q.HighCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                FlightCount = grp.Sum(q => q.FlightCount),
                                EventCount = grp.Sum(q => q.EventsCount),
                                HighScore = grp.Sum(q => q.HighScore),
                                MediumScore = grp.Sum(q => q.MediumScore),
                                LowScore = grp.Sum(q => q.LowScore),
                                Score = grp.Sum(q => q.Score),

                                ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventPerFlight = grp.Sum(q => q.EventsCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                ScorePerEvent = grp.Sum(q => q.EventsCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventsCount) * 1.0,

                                //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                                //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),

                            };

                var result = query.ToList();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    Data = ex.InnerException,
                    IsSuccess = false
                };
            }
        }


        


        ////Captain Dashboard////
        [HttpGet]
        [Route("api/fdm/cpt/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetCptMonthly2(int year, int month, int cptId)
        {
            var d1 = new DateTime(year, month, 1);
            var dates = new List<int?>();
            dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            for (var i = 0; i <= 11; i++)
            {
                d1 = d1.AddMonths(-1);
                dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            }
            try
            {




                var query = from x in context.FDMCptMonthlies
                            where dates.Contains(x.YearMonth) && x.CptId == cptId
                            group x by new { x.YearMonth } into grp
                            select new
                            {
                                YearMonth = grp.Key.YearMonth.ToString(),// ? grp.Key.YearMonth.ToString() : string.Empty,
                                FlightCount = grp.Sum(q => q.FlightCount),
                                HighCount = grp.Sum(q => q.HighCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                EventCount = grp.Sum(q => q.EventCount),
                                Score = grp.Sum(q => q.Score),
                                HighScore = grp.Sum(q => q.HighScore),
                                LowScore = grp.Sum(q => q.LowScore),
                                MediumScore = grp.Sum(q => q.MediumScore),

                                ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,
                                ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0

                                //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                                //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),
                                //EventPerFlight = grp.Sum(q => q.EventPerFlight)

                            };
                var ds = query.ToList();
                var result = new
                {
                    data = ds,
                    TotalFlightCount = ds.Sum(q => q.FlightCount),
                    TotalEventsCount = ds.Sum(q => q.EventCount),
                    TotalScores = ds.Sum(q => q.Score),

                    EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 1.0 / ds.Sum(q => q.FlightCount) : 0,
                    ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Score) * 1.0 / ds.Sum(q => q.FlightCount) * 1.0,

                    //EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 100.0 / ds.Sum(q => q.FlightCount) : 0,
                    //ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Score) * 100.0 / ds.Sum(q => q.FlightCount) * 1.0,

                    TotalHighCount = ds.Sum(q => q.HighCount),
                    TotalMediumCount = ds.Sum(q => q.MediumCount),
                    TotalLowCount = ds.Sum(q => q.LowCount),
                    TotalHighScore = ds.Sum(q => q.HighScore),
                    TotalMediumScore = ds.Sum(q => q.MediumScore),
                    TotalLowScore = ds.Sum(q => q.LowScore),
                };


                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex.InnerException
                };
            }
        }

        [HttpGet]
        [Route("api/fdm/get/total/cpt/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetCptTotal(int ymf, int ymt, int cptId)
        {

            try
            {
                var query = from x in context.FDMCptMonthlies
                            where x.YearMonth >= ymf && x.YearMonth <= ymt && x.CptId == cptId
                            group x by new { x.YearMonth } into grp
                            select new
                            {
                                YearMonth = grp.Key.YearMonth.ToString(), //? grp.Key.YearMonth.ToString() : string.Empty,
                                FlightCount = grp.Sum(q => q.FlightCount),
                                HighCount = grp.Sum(q => q.HighCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                EventCount = grp.Sum(q => q.EventCount),
                                Score = grp.Sum(q => q.Score),
                                HighScore = grp.Sum(q => q.HighScore),
                                LowScore = grp.Sum(q => q.LowScore),
                                MediumScore = grp.Sum(q => q.MediumScore),

                                ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                                EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount * 1.0),

                                //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                                //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),
                                //EventPerFlight = grp.Sum(q => q.EventPerFlight)

                            };
                var ds = query.ToList();
                var result = new
                {
                    data = ds,
                    TotalFlightCount = ds.Sum(q => q.FlightCount),
                    TotalEventsCount = ds.Sum(q => q.EventCount),
                    TotalScores = ds.Sum(q => q.Score),

                    EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 1.0 / ds.Sum(q => q.FlightCount) : 0,
                    ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Score) * 1.0 / ds.Sum(q => q.FlightCount) * 1.0,

                    //EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 100.0 / ds.Sum(q => q.FlightCount) : 0,
                    //ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Score) * 100.0 / ds.Sum(q => q.FlightCount) * 1.0,

                    TotalHighCount = ds.Sum(q => q.HighCount),
                    TotalMediumCount = ds.Sum(q => q.MediumCount),
                    TotalLowCount = ds.Sum(q => q.LowCount),
                    TotalHighScore = ds.Sum(q => q.HighScore),
                    TotalMediumScore = ds.Sum(q => q.MediumScore),
                    TotalLowScore = ds.Sum(q => q.LowScore),
                };



                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex.InnerException
                };
            }
        }


        [HttpGet]
        [Route("api/fdm/get/total/Fo/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetFoTotal(int ymf, int ymt, int cptId)
        {

            try
            {
                var query = from x in context.FDMFoMonthlies
                            where x.YearMonth >= ymf && x.YearMonth <= ymt && x.P2Id == cptId
                            group x by new { x.YearMonth } into grp
                            select new
                            {
                                YearMonth = grp.Key.YearMonth.HasValue ? grp.Key.YearMonth.ToString() : string.Empty,
                                FlightCount = grp.Sum(q => q.FlightCount),
                                HighCount = grp.Sum(q => q.HighCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                EventCount = grp.Sum(q => q.EventCount),
                                Score = grp.Sum(q => q.Score),
                                HighScore = grp.Sum(q => q.HighScore),
                                LowScore = grp.Sum(q => q.LowScore),
                                MediumScore = grp.Sum(q => q.MediumScore),

                                ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                                EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,


                                //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                                //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),
                                //EventPerFlight = grp.Sum(q => q.EventPerFlight)

                            };
                var ds = query.ToList();
                var result = new
                {
                    data = ds,
                    TotalFlightCount = ds.Sum(q => q.FlightCount),
                    TotalEventsCount = ds.Sum(q => q.EventCount),
                    TotalScores = ds.Sum(q => q.Score),

                    EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 1.0 / ds.Sum(q => q.FlightCount) : 0,
                    ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Score) * 1.0 / ds.Sum(q => q.FlightCount) * 1.0,

                    //EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 100.0 / ds.Sum(q => q.FlightCount) : 0,
                    //ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Score) * 100.0 / ds.Sum(q => q.FlightCount) * 1.0,

                    TotalHighCount = ds.Sum(q => q.HighCount),
                    TotalMediumCount = ds.Sum(q => q.MediumCount),
                    TotalLowCount = ds.Sum(q => q.LowCount),
                    TotalHighScore = ds.Sum(q => q.HighScore),
                    TotalMediumScore = ds.Sum(q => q.MediumScore),
                    TotalLowScore = ds.Sum(q => q.LowScore),
                };



                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex.InnerException
                };
            }
        }


       

        [HttpGet]
        [Route("api/fdm/cpt/route/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetFDMRoute(int ymf, int ymt, int cptId)
        {


            try
            {

                var query = from x in context.FDMCptAirportMonthlies
                            where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.CptId == cptId || x.CptId == cptId || x.CptId == cptId)
                            group x by new { x.Route } into grp
                            select new
                            {
                                grp.Key.Route,
                                HighCount = grp.Sum(q => q.HighCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                FlightCount = grp.Sum(q => q.FlightCount),
                                EventCount = grp.Sum(q => q.EventsCount),
                                HighScore = grp.Sum(q => q.HighScore),
                                MediumScore = grp.Sum(q => q.MediumScore),
                                LowScore = grp.Sum(q => q.LowScore),
                                Score = grp.Sum(q => q.Score),
                                ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventPerFlight = grp.Sum(q => q.EventsCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                ScorePerEvent = grp.Sum(q => q.EventsCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventsCount) * 1.0,
                            };

                var result = query.ToList();
                return new DataResponse()
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    Data = ex.InnerException,
                    IsSuccess = false
                };
            }


        }


        //[HttpGet]
        //[Route("api/fdm/get/airport/cpt/{yf}/{yt}/{mf}/{mt}/{cptId}")]
        //public async Task<DataResponse> GetCptAirport(int yf, int yt, int mf, int mt, int cptId)
        //{

        //    try
        //    {
        //        var query = from x in context.FDMCptAirportEventMonthlies
        //                    where x.Year >= yf && x.Year <= yf && x.Month >= mf && x.Month <= mt && ((x.P2Id == cptId && (x.PF == "f" || x.PF == "F")) || x.P1Id == cptId || x.IPId == cptId)
        //                    group x by new { x.IATA } into grp
        //                    select new
        //                    {
        //                        grp.Key.IATA,
        //                        Departure = grp.Sum(q => q.DepartureAirportEventCount),
        //                        Arrival = grp.Sum(q => q.ArrivalAirportEventCount)
        //                    };
        //        var result = query.ToList();



        //        return new DataResponse()
        //        {
        //            IsSuccess = true,
        //            Data = result
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse()
        //        {
        //            IsSuccess = false,
        //            Data = ex.InnerException
        //        };
        //    }
        //}
        [HttpGet]
        [Route("api/fdm/dashboard/fo/events/monthly2/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetFoEventsNameMonthly2(int ymf, int ymt, int cptId)
        {
            var query = from x in context.FDMFoEventMonthlies
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && x.P2Id == cptId
                        group x by new { x.EventName } into grp
                        select new
                        {
                            EventCount = grp.Sum(q => q.EventCount),
                            grp.Key.EventName,
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                        };

            var result = query.ToList();


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }


        [HttpGet]
        [Route("api/fdm/fo/monthly2/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetFDMFoMonthly2(int year, int month, int cptId)
        {
            var d1 = new DateTime(year, month, 1);
            var dates = new List<int?>();
            dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            for (var i = 0; i <= 11; i++)
            {
                d1 = d1.AddMonths(-1);
                dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            }
            var query = from x in context.FDMFoMonthlies
                        where dates.Contains(x.YearMonth) && x.P2Id == cptId
                        group x by new { x.YearMonth } into grp
                        select new
                        {
                            YearMonth = grp.Key.YearMonth.HasValue ? grp.Key.YearMonth.ToString() : string.Empty,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            EventCount = grp.Sum(q => q.EventCount),
                            Score = grp.Sum(q => q.Score),
                            HighScore = grp.Sum(q => q.HighScore),
                            LowScore = grp.Sum(q => q.LowScore),
                            MediumScore = grp.Sum(q => q.MediumScore),

                            ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                            //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),
                            //EventPerFlight = grp.Sum(q => q.EventPerFlight)

                        };
            var ds = query.ToList();
            var result = new
            {
                data = ds,
                TotalFlightCount = ds.Sum(q => q.FlightCount),
                TotalEventsCount = ds.Sum(q => q.EventCount),
                TotalScores = ds.Sum(q => q.Score),

                EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 1.0 / ds.Sum(q => q.FlightCount) : 0,
                ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Score) * 1.0 / ds.Sum(q => q.FlightCount) * 1.0,

                //EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 100.0 / ds.Sum(q => q.FlightCount) : 0,
                //ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Score) * 100.0 / ds.Sum(q => q.FlightCount) * 1.0,

                TotalHighCount = ds.Sum(q => q.HighCount),
                TotalMediumCount = ds.Sum(q => q.MediumCount),
                TotalLowCount = ds.Sum(q => q.LowCount),
                TotalHighScore = ds.Sum(q => q.HighScore),
                TotalMediumScore = ds.Sum(q => q.MediumScore),
                TotalLowScore = ds.Sum(q => q.LowScore),
            };



            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/fo/register/monthly2/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetRegFoMonthly2(int ymf, int ymt, int cptId)
        {
            var query = from x in context.FDMRegFoMonthlies
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && x.P2Id == cptId
                        group x by new { x.Register, x.RegisterId } into grp
                        select new
                        {
                            grp.Key.Register,
                            EventCount = grp.Sum(q => q.EventCount),
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            HighScore = grp.Sum(q => q.HighCount) * 4,
                            LowScore = grp.Sum(q => q.LowCount),
                            MediumScore = grp.Sum(q => q.MediumCount) * 2,
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),

                            ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            //EventPerFlight = grp.Sum(q => q.EventCount) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,



                        };

            var result = query.ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/dashboard/cpt/events/monthly2/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetCptEventsNameMonthly2(int ymf, int ymt, int cptId)
        {
            var query = from x in context.FDMCptEventMonthlies
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && x.CptId == cptId
                        group x by new { x.EventName } into grp
                        select new
                        {
                            grp.Key.EventName,
                            EventCount = grp.Sum(q => q.EventCount),
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                        };

            var result = query.ToList();


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }



        /////////////////////////////////


        [HttpGet]
        [Route("api/get/fdm/{year}/{month}")]
        public DataResponse GetFDM(int year, int month)
        {
            var query = from x in context.ViewFDMs
                        where x.Date.Value.Year == year && x.Date.Value.Month == month
                        select x;


            var Boeing = query.Where(q => q.AircraftType.Contains("B")).ToList();
            var MD = query.Where(q => q.AircraftType.Contains("MD")).ToList();
            var newRecord = query.Where(q => q.Removed == false && q.Approved == false).ToList();
            var confirmed = query.Where(q => q.Approved == true).ToList();
            var removed = query.Where(q => q.Removed == true).ToList();


            if (query != null)
            {
                return new DataResponse
                {
                    IsSuccess = true,
                    Data = new
                    {
                        newRecord,
                        confirmed,
                        removed,
                        Boeing,
                        MD
                    }
                };
            }
            else
            {
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = null
                };
            }
        }

        [HttpPost]
        [Route("api/status/fdm/{rowId}/{status}/{usr}")]
        public async Task<DataResponse> FDMStatus(int rowId, int status, string usr)
        {
            try
            {
                var record = context.FDMs.Single(q => q.Id == rowId);
                record.Validity = status;
                record.ValidationPerson = usr;
                var result = context.ViewFDMs.Single(q => q.Id == rowId);
                await context.SaveChangesAsync();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result,

                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " INNER: " + ex.InnerException.Message;

                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg
                };
            }

        }

        [HttpPost]
        [Route("api/confirm/fdm/{rowID}")]
        public async Task<DataResponse> confirmFDM(int rowId)
        {
            try
            {
                var record = context.FDMs.Single(q => q.Id == rowId);
                record.Validity = 1;
                var result = context.ViewFDMs.Single(q => q.Id == rowId);
                context.SaveChanges();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };

            }
            catch
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = null
                };

            }

        }

        [HttpGet]
        [Route("api/fdm/lock/md/{year}/{month}/{usr}")]
        public async Task<DataResponse> LockMDFDM(int year, int month, string usr)
        {


            try
            {
                var records = context.FDMs.Where(q => q.Date.Value.Year == year && q.Date.Value.Month == month && (q.Validity == 0 || q.Validity == null) && q.AircraftType.Contains("MD")).ToList();
                if (records.Count() == 0)
                {
                    context.Database.ExecuteSqlCommand("Update FDM set Confirmation = 1, ConfirmationPerson = ' " + usr + " ' Where AircraftType LIKE 'MD%' and YEAR(fdm.[DATE]) = " + year + " and MONTH(fdm.[DATE]) = " + month);
                    return new DataResponse()
                    {
                        IsSuccess = true,
                    };
                }
                else
                {

                    return new DataResponse()
                    {
                        IsSuccess = false,
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex
                };
            }
        }

        [HttpGet]
        [Route("api/fdm/lock/boeing/{year}/{month}/{usr}")]
        public async Task<DataResponse> LockBoeingFDM(int year, int month, string usr)
        {


            try
            {
                var records = context.FDMs.Where(q => q.Date.Value.Year == year && q.Date.Value.Month == month && (q.Validity == 0 || q.Validity == null) && q.AircraftType.Contains("B")).ToList();
                if (records.Count() == 0)
                {
                    context.Database.ExecuteSqlCommand("Update FDM set Confirmation = 1, ConfirmationPerson = ' " + usr + " ' Where AircraftType LIKE 'B%' and YEAR(fdm.[DATE]) = " + year + " and MONTH(fdm.[DATE]) = " + month);
                    return new DataResponse()
                    {
                        IsSuccess = true,
                    };
                }
                else
                {

                    return new DataResponse()
                    {
                        IsSuccess = false,
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex
                };
            }
        }

        //[HttpGet]
        //[Route("api/fdm/avg")]
        //public async Task<DataResponse> GetFDMAVG()
        //{


        //    try
        //    {
        //        var result = context.FDMAVGs.Single();
        //        return new DataResponse()
        //        {
        //            IsSuccess = true,
        //            Data = result.ScorePerEvent
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse()
        //        {
        //            IsSuccess = false,
        //            Data = ex
        //        };
        //    }
        //}



        [HttpPost]
        [Route("api/delete/fdm/{rowId}")]
        public async Task<DataResponse> DeleteFDM(int rowId)
        {
            var viewRecord = context.ViewFDMs.Single(q => q.Id == rowId);
            if (viewRecord.Validity != 1 || viewRecord.Validity != 2 || viewRecord.Confirmation != false || viewRecord.Confirmation != null)
            {
                var record = context.FDMs.Single(q => q.Id == viewRecord.Id);
                context.FDMs.Remove(record);
                context.SaveChangesAsync();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = viewRecord
                };
            }
            else
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = viewRecord.Status
                };
            }
        }



        



        //[HttpGet]
        //[Route("api/fdm/dashboard/register/{df}/{dt}")]
        //public async Task<DataResponse> GetFDMRegDaily(DateTime df, DateTime dt)
        //{
        //    var query = from x in context.FDMRegDailies
        //                where x.FlightDate <= dt.Date && x.FlightDate >= df.Date
        //                group x by new { x.RegisterID, x.Register, } into grp
        //                select new
        //                {
        //                    grp.Key.Register,
        //                    grp.Key.RegisterID,
        //                    FlightCount = grp.Sum(q => q.FlightCount),
        //                    IncidentCount = grp.Sum(q => q.EventCount),
        //                    HighLevelCount = grp.Sum(q => q.HighCount),
        //                    MediumLevelCount = grp.Sum(q => q.MediumCount),
        //                    LowLevelCount = grp.Sum(q => q.LowCount),
        //                    Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
        //                    ScorePercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
        //                };

        //    var ds = query.ToList();
        //    var result = new
        //    {
        //        data = ds,
        //        TotalFlights = ds.Sum(q => q.FlightCount),
        //        TotalHighLevel = ds.Sum(q => q.HighLevelCount),
        //        TotalMediumLevel = ds.Sum(q => q.MediumLevelCount),
        //        TotalLowLevel = ds.Sum(q => q.LowLevelCount),
        //    };

        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };


        //}


        [HttpGet]
        [Route("api/fdm/dashboard/register/monthly/{df}/{dt}")]
        public async Task<DataResponse> GetAllFDMRegMonthly(DateTime df, DateTime dt)
        {
            var query = from x in context.FDMRegCptMonthlies
                        where x.month >= df.Month && x.year >= df.Year && x.month <= dt.Month && x.year <= dt.Year
                        group x by new { x.Register, x.RegisterID, x.month, x.year } into grp
                        select new
                        {
                            grp.Key.RegisterID,
                            grp.Key.Register,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            IncidentCount = grp.Sum(q => q.EventCount),
                            HighLevelCount = grp.Sum(q => q.HighCount),
                            LowLevelCount = grp.Sum(q => q.LowCount),
                            MediumLevelCount = grp.Sum(q => q.MediumCount),
                            grp.Key.month,
                            grp.Key.year,
                        };


            var ds = query.ToList();
            var result = new
            {
                data = ds,
                TotalFlights = ds.Sum(q => q.FlightCount),
                TotalHighLevel = ds.Sum(q => q.HighLevelCount),
                TotalMediumLevel = ds.Sum(q => q.MediumLevelCount),
                TotalLowLevel = ds.Sum(q => q.LowLevelCount),
            };


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }



        [HttpGet]
        [Route("api/fdm/dashboard/cpt/tops/{count}/{df}/{dt}")]
        public async Task<DataResponse> GetTopCpt(int count, DateTime df, DateTime dt)
        {
            var query = from x in context.FDMCptAlls
                        where x.Month >= df.Month && x.Month <= dt.Month && x.Year >= df.Year && x.Year <= dt.Year
                        group x by new { x.CptName, x.CptId, x.CptCode } into grp
                        select new
                        {
                            grp.Key.CptName,
                            grp.Key.CptId,
                            grp.Key.CptCode,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            IncidentCount = grp.Sum(q => q.EventCount),
                            HighLevelCount = grp.Sum(q => q.HighCount),
                            LowLevelCount = grp.Sum(q => q.LowCount),
                            MediumLevelCount = grp.Sum(q => q.MediumCount),
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                            ScorePercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100,

                        };


            var result = query.OrderByDescending(q => q.Scores).Take(10).ToList();


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }



      






        //[HttpGet]
        //[Route("api/fdm/dashboard/cpt/events/{df}/{dt}/{p1Id}")]
        //public async Task<DataResponse> GetCptEventsDaily(DateTime df, DateTime dt, int p1Id)
        //{
        //    var query = from x in context.FDMCptEventDailies
        //                where x.Day >= df && x.Day <= dt && x.CptId == p1Id
        //                group x by new { x.EventName } into grp
        //                select new
        //                {
        //                    grp.Key.EventName,
        //                    EventCount = grp.Sum(q => q.EventCount),
        //                    Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
        //                };

        //    var result = query.ToList();


        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}


        [HttpGet]
        [Route("api/fdm/cpt/events/monthly/{year}/{month}/{crewId}")]
        public async Task<DataResponse> GetRegCptEvents(int year, int month, int crewId)
        {
            var query = from x in context.FDMCptMonthlies
                            //where x.Month >= mf && x.Month <= mt && x.Year >= yf && x.Year <= yt && x.CptId == cptId
                            //where x.YearMonth >= ymf && x.YearMonth <= ymt && x.CptId == cptId
                        where x.Year == year && x.Month == month && x.CptId == crewId
                        group x by new { x.YearMonth } into grp
                        select new
                        {
                            grp.Key.YearMonth,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            EventCount = grp.Sum(q => q.EventCount),
                            Score = grp.Sum(q => q.Score),
                            //HighPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.HighCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            //MediumPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.MediumCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            //LowPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.LowCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            HighScore = grp.Sum(q => q.HighScore),
                            LowScore = grp.Sum(q => q.LowScore),
                            MediumScore = grp.Sum(q => q.MediumScore),

                            ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                            //ScorePerEvent = grp.Sum(q => q.ScorePerEvent)

                        };
            var ds = query.ToList();
            var result = new
            {
                data = ds,
                TotalFlightCount = ds.Sum(q => q.FlightCount),
                TotalEventsCount = ds.Sum(q => q.EventCount),
                TotalScore = ds.Sum(q => q.Score),
                EventPerFlight = ds.Sum(q => q.FlightCount),
                ScorePerFlight = ds.Sum(q => q.ScorePerFlight),
                TotalHighCount = ds.Sum(q => q.HighCount),
                TotalMediumCount = ds.Sum(q => q.MediumCount),
                TotalLowCount = ds.Sum(q => q.LowCount),
                TotalHighScore = ds.Sum(q => q.HighScore),
                TotalMediumScore = ds.Sum(q => q.MediumScore),
                TotalLowScore = ds.Sum(q => q.LowScore),
            };


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/dashboard/cpt/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetCptMonthly(int year, int month, int cptId)
        {
            var d1 = new DateTime(year, month, 1);
            var dates = new List<int?>();
            dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            for (var i = 0; i <= 11; i++)
            {
                d1 = d1.AddMonths(-1);
                dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            }
            var query = from x in context.FDMCptMonthlies
                            //where x.Month >= mf && x.Month <= mt && x.Year >= yf && x.Year <= yt && x.CptId == cptId
                            //where x.YearMonth >= ymf && x.YearMonth <= ymt && x.CptId == cptId
                        where dates.Contains(x.YearMonth) && x.CptId == cptId
                        group x by new { x.YearMonth } into grp
                        select new
                        {
                            grp.Key.YearMonth,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            EventCount = grp.Sum(q => q.EventCount),
                            Score = grp.Sum(q => q.Score),
                            //HighPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.HighCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            //MediumPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.MediumCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            //LowPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.LowCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            HighScore = grp.Sum(q => q.HighScore),
                            LowScore = grp.Sum(q => q.LowScore),
                            MediumScore = grp.Sum(q => q.MediumScore),

                            ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                            //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),
                            //EventPerFlight = grp.Sum(q => q.EventPerFlight)

                        };
            var ds = query.ToList();
            var result = new
            {
                data = ds,
                TotalFlightCount = ds.Sum(q => q.FlightCount),
                TotalEventsCount = ds.Sum(q => q.EventCount),
                TotalScore = ds.Sum(q => q.Score),
                EventPerFlight = ds.Sum(q => q.FlightCount),
                ScorePerFlight = ds.Sum(q => q.ScorePerFlight),
                TotalHighCount = ds.Sum(q => q.HighCount),
                TotalMediumCount = ds.Sum(q => q.MediumCount),
                TotalLowCount = ds.Sum(q => q.LowCount),
                TotalHighScore = ds.Sum(q => q.HighScore),
                TotalMediumScore = ds.Sum(q => q.MediumScore),
                TotalLowScore = ds.Sum(q => q.LowScore),
            };


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }





        [HttpGet]
        [Route("api/fdm/dashboard/fo/events/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetFoEventsNameMonthly(int year, int month, int cptId)
        {
            var query = from x in context.FDMFoEventMonthlies
                            //where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt && x.P2Id == cptId
                            //where x.YearMonth >= ymf && x.YearMonth <= ymt && x.P2Id == cptId
                        where x.year == year && x.month == month && x.P2Id == cptId
                        group x by new { x.EventName } into grp
                        select new
                        {
                            EventCount = grp.Sum(q => q.EventCount),
                            grp.Key.EventName,
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                        };

            var result = query.ToList();


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/dashboard/cpt/events/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetCptEventsNameMonthly(int year, int month, int cptId)
        {
            var query = from x in context.FDMCptEventMonthlies
                            //where x.Month >= mf && x.Month <= mt && x.Year >= yf && x.Year <= yt && x.CptId == cptId
                        where x.year == year && x.month == month && x.CptId == cptId
                        group x by new { x.EventName } into grp
                        select new
                        {
                            grp.Key.EventName,
                            EventCount = grp.Sum(q => q.EventCount),
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                        };

            var result = query.ToList();


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        //[HttpGet]
        //[Route("api/fdm/dashboard/cpt/events/monthly2/{ymf}/{ymt}/{cptId}")]
        //public async Task<DataResponse> GetCptEventsNameMonthly(int ymf, int ymt, int cptId)
        //{
        //    var query = from x in context.FDMCptEventMonthlies
        //                    //where x.Month >= mf && x.Month <= mt && x.Year >= yf && x.Year <= yt && x.CptId == cptId
        //                where x.YearMonth >= ymf && x.YearMonth <= ymt && x.CptId == cptId
        //                group x by new { x.EventName } into grp
        //                select new
        //                {
        //                    grp.Key.EventName,
        //                    EventCount = grp.Sum(q => q.EventCount),
        //                    Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
        //                };

        //    var result = query.ToList();


        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}

        [HttpGet]
        [Route("api/fdm/cpt/fo/events/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetCptFoMonthly(int year, int month, int cptId)
        {
            var query = from x in context.FDMCptFoMonthlies
                            //where x.Year >= yf && x.Year <= yt && x.Month >= mf && x.Month <= mt && (x.P1Id == cptId || x.IPId == cptId)
                            //where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.P1Id == cptId || x.P2Id == cptId || x.IPId == cptId)
                        where x.year == year && x.month == month && (x.P1Id == cptId || x.P2Id == cptId || x.IPId == cptId)
                        group x by new { x.P2Id, x.P2Code, x.P2Name } into grp
                        select new
                        {
                            grp.Key.P2Id,
                            grp.Key.P2Code,
                            grp.Key.P2Name,
                            EventsCount = grp.Sum(q => q.EventCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                        };


            var result = query.Where(q => q.EventsCount > 0).ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/cpt/fo/events/monthly2/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetCptFoMonthly2(int ymf, int ymt, int cptId)
        {
            var query = from x in context.FDMCptFoMonthlies
                            //where x.Year >= yf && x.Year <= yt && x.Month >= mf && x.Month <= mt && (x.P1Id == cptId || x.IPId == cptId)
                            //where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.P1Id == cptId || x.P2Id == cptId || x.IPId == cptId)
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.P1Id == cptId || x.P2Id == cptId || x.IPId == cptId)
                        group x by new { x.P2Id, x.P2Code, x.P2Name } into grp
                        select new
                        {
                            grp.Key.P2Id,
                            grp.Key.P2Code,
                            grp.Key.P2Name,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            EventCount = grp.Sum(q => q.EventCount),
                            Score = grp.Sum(q => q.Score),
                            HighScore = grp.Sum(q => q.HighScore),
                            LowScore = grp.Sum(q => q.LowScore),
                            MediumScore = grp.Sum(q => q.MediumScore),

                            ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                            //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),
                            //EventPerFlight = grp.Sum(q => q.EventPerFlight)

                        };


            var result = query.Where(q => q.EventCount > 0).ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        //[HttpGet]
        //[Route("api/fdm/cpt/fo/events/daily/{df}/{dt}/{P1Id}")]
        //public async Task<DataResponse> GetCptFoDaily(DateTime df, DateTime dt, int P1Id)
        //{
        //    var query = from x in context.FDMCptFoDailies
        //                where x.Day >= df && x.Day <= dt && (x.P1Id == P1Id || x.IPId == P1Id)
        //                group x by new { x.P2Id, x.P2Code, x.P2Name } into grp
        //                select new
        //                {
        //                    grp.Key.P2Id,
        //                    grp.Key.P2Code,
        //                    grp.Key.P2Name,
        //                    EventsCount = grp.Sum(q => q.EventCount),
        //                    HighCount = grp.Sum(q => q.HighCount),
        //                    MediumCount = grp.Sum(q => q.MediumCount),
        //                    LowCount = grp.Sum(q => q.LowCount),
        //                };


        //    var result = query.Where(q => q.EventsCount > 0).ToList();
        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}

        [HttpGet]
        [Route("api/fdm/cpt/ip/events/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetCptIpMonthly(int year, int month, int cptId)
        {
            var query = from x in context.FDMCptIpMonthlies
                            //where x.Year >= yf && x.Year <= yt && x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.p1Id == cptId
                            // where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.p1Id == cptId || x.IpId == cptId)
                        where x.year == year && x.month == month && (x.p1Id == cptId || x.IpId == cptId)
                        group x by new { x.IpId, x.IpCode, x.IpName } into grp
                        select new
                        {
                            grp.Key.IpId,
                            grp.Key.IpCode,
                            grp.Key.IpName,
                            EventsCount = grp.Sum(q => q.EventCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                        };


            var result = query.Where(q => q.EventsCount > 0).ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }


        [HttpGet]
        [Route("api/fdm/cpt/ip/events/monthly2/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetCptIpMonthly2(int ymf, int ymt, int cptId)
        {
            var query = from x in context.FDMCptIpMonthlies
                            //where x.Year >= yf && x.Year <= yt && x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.p1Id == cptId
                            // where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.p1Id == cptId || x.IpId == cptId)
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.p1Id == cptId || x.IpId == cptId)
                        group x by new { x.IpId, x.IpCode, x.IpName } into grp
                        select new
                        {
                            grp.Key.IpId,
                            grp.Key.IpCode,
                            grp.Key.IpName,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            EventCount = grp.Sum(q => q.EventCount),
                            Score = grp.Sum(q => q.Score),
                            HighScore = grp.Sum(q => q.HighScore),
                            LowScore = grp.Sum(q => q.LowScore),
                            MediumScore = grp.Sum(q => q.MediumScore),

                            ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,


                            //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                            //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),
                            //EventPerFlight = grp.Sum(q => q.EventPerFlight)

                        };


            var result = query.Where(q => q.EventCount > 0).ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/ip/cpt/events/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetIpCptMonthly(int year, int month, int cptId)
        {
            var query = from x in context.FDMCptFoMonthlies
                            //where x.Year >= yf && x.Year <= yt && x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.IPId == cptId
                            //where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.P1Id == cptId || x.P2Id == cptId || x.IPId == cptId)
                        where x.year == year && x.month == month && (x.P1Id == cptId || x.P2Id == cptId || x.IPId == cptId)
                        group x by new { x.P1Id, x.P1Code, x.P1Name } into grp
                        select new
                        {
                            grp.Key.P1Id,
                            grp.Key.P1Code,
                            grp.Key.P1Name,
                            EventsCount = grp.Sum(q => q.EventCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                        };


            var result = query.Where(q => q.EventsCount > 0).ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/ip/cpt/events/monthly2/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetIpCptMonthly2(int ymf, int ymt, int cptId)
        {
            var query = from x in context.FDMCptFoMonthlies
                            //where x.Year >= yf && x.Year <= yt && x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.IPId == cptId
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && (x.P1Id == cptId || x.P2Id == cptId || x.IPId == cptId)
                        //where x.Year == year && x.Month == month && (x.P1Id == cptId || x.P2Id == cptId || x.IPId == cptId)
                        group x by new { x.P1Id, x.P1Code, x.P1Name } into grp
                        select new
                        {
                            grp.Key.P1Id,
                            grp.Key.P1Code,
                            grp.Key.P1Name,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            EventCount = grp.Sum(q => q.EventCount),
                            Score = grp.Sum(q => q.Score),
                            HighScore = grp.Sum(q => q.HighScore),
                            LowScore = grp.Sum(q => q.LowScore),
                            MediumScore = grp.Sum(q => q.MediumScore),

                            ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,
                        };


            //    ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
            //    ScorePerEvent = grp.Sum(q => q.ScorePerEvent),
            //    EventPerFlight = grp.Sum(q => q.EventPerFlight)
            //};



            var result = query.Where(q => q.EventCount > 0).ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }


        //[HttpGet]
        //[Route("api/fdm/cpt/ip/events/daily/{df}/{dt}/{P1Id}")]
        //public async Task<DataResponse> GetCptIpDaily(DateTime df, DateTime dt, int P1Id)
        //{
        //    var query = from x in context.FDMCptIpDailies
        //                where x.Day >= df && x.Day <= dt && x.P1Id == P1Id
        //                group x by new { x.IpId, x.IpCode, x.IpName } into grp
        //                select new
        //                {
        //                    grp.Key.IpId,
        //                    grp.Key.IpCode,
        //                    grp.Key.IpName,
        //                    EventsCount = grp.Sum(q => q.EventCount),
        //                    HighCount = grp.Sum(q => q.HighCount),
        //                    MediumCount = grp.Sum(q => q.MediumCount),
        //                    LowCount = grp.Sum(q => q.LowCount),
        //                };


        //    var result = query.Where(q => q.EventsCount > 0).ToList();
        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}

        //[HttpGet]
        //[Route("api/fdm/ip/cpt/events/daily/{df}/{dt}/{P1Id}")]
        //public async Task<DataResponse> GetIpCptDaily(DateTime df, DateTime dt, int P1Id)
        //{
        //    var query = from x in context.FDMCptFoDailies
        //                where x.Day >= df && x.Day <= dt && (x.IPId == P1Id)
        //                group x by new { x.P1Id, x.P1Code, x.P1Name } into grp
        //                select new
        //                {
        //                    grp.Key.P1Id,
        //                    grp.Key.P1Code,
        //                    grp.Key.P1Name,
        //                    EventsCount = grp.Sum(q => q.EventCount),
        //                    HighCount = grp.Sum(q => q.HighCount),
        //                    MediumCount = grp.Sum(q => q.MediumCount),
        //                    LowCount = grp.Sum(q => q.LowCount),
        //                };


        //    var result = query.Where(q => q.EventsCount > 0).ToList();
        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}

        [HttpGet]
        [Route("api/fdm/cpt/events/register/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetRegCptEventsMonthly(int year, int month, int cptId)
        {
            var query = from x in context.FDMRegCptMonthlies
                            //where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt && x.CptId == cptId
                        where x.year == year && x.month == month && x.CptId == cptId
                        group x by new { x.Register, x.RegisterID } into grp
                        select new
                        {
                            grp.Key.Register,
                            EventCount = grp.Sum(q => q.EventCount),
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            HighScore = grp.Sum(q => q.HighCount) * 4,
                            LowScore = grp.Sum(q => q.LowCount),
                            MediumScore = grp.Sum(q => q.MediumCount) * 2,
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),

                            ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            //EventPerFlight = grp.Sum(q => q.EventCount) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,

                        };

            var result = query.ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/cpt/events/register/monthly2/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetRegCptEventsMonthly2(int ymf, int ymt, int cptId)
        {
            var query = from x in context.FDMRegCptMonthlies
                            //where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt && x.CptId == cptId
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && x.CptId == cptId
                        group x by new { x.Register, x.RegisterID } into grp
                        select new
                        {
                            grp.Key.Register,
                            EventCount = grp.Sum(q => q.EventCount),
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            HighScore = grp.Sum(q => q.HighCount) * 4,
                            LowScore = grp.Sum(q => q.LowCount),
                            MediumScore = grp.Sum(q => q.MediumCount) * 2,
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                            ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventPerFlight = grp.Sum(q => q.EventCount) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,

                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,


                        };

            var result = query.ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/events/{p1id}/{regId}/{typeId}/{df}/{dt}")]
        public async Task<DataResponse> GetEventsFDM(int p1id, int regId, int typeId, DateTime df, DateTime dt)
        {
            var query = from x in context.ViewFDMs
                        where x.Date >= df && x.Date <= dt
                        select x;

            if (p1id != -1)
                query = query.Where(q => q.P1Id == p1id);

            if (regId != -1)
                query = query.Where(q => q.RegisterID == regId);

            if (typeId != -1)
                query = query.Where(q => q.TypeID == typeId);

            var result = query.ToList();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = result,
            };
        }

        [HttpGet]
        [Route("api/get/cpt/fdm/info/{df}/{dt}/{crewId}")]
        public async Task<DataResponse> GetCptFDMInfo(DateTime? df, DateTime? dt, int crewId)
        {
            var query = from x in context.ViewFDMs
                        where x.Date >= (df) && x.Date <= (dt) && (x.P1Id == crewId || x.IPId == crewId || (x.P2Id == crewId && (x.PF == "f" || x.PF == "F"))) && x.Validity == 1 && x.Confirmation == true
                        select x;

            var result = query.ToList();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/get/cpt/fdm/info/monthly/{year}/{month}/{crewId}")]
        public async Task<DataResponse> GetCptFDMInfo(int year, int month, int crewId)
        {
            var query = from x in context.ViewFDMs
                        where x.Date.Value.Year == year && x.Date.Value.Month == month && (x.P1Id == crewId || x.IPId == crewId || x.P2Id == crewId)
                        select x;

            var result = query.ToList();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/get/cpt/fdm/sb/monthly/{year}/{month}/{crewId}")]
        public async Task<DataResponse> GetCptFDMSBInfo(int year, int month, int crewId)
        {
            var query = from x in context.ViewFDMs
                        where x.Date.Value.Year == year && x.Date.Value.Month == month && (x.P1Id == crewId || x.IPId == crewId || x.P2Id == crewId) && x.Validity == 1 && x.Confirmation == true
                        select x;

            var result = query.ToList();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        //[HttpGet]
        //[Route("api/fdm/fo/daily/{p2id}/{df}/{dt}")]
        //public async Task<DataResponse> GetFDMFoDaily(int p2id, DateTime df, DateTime dt)
        //{
        //    var query = from x in context.FDMFoDailies
        //                where x.Day >= df && x.Day <= dt && x.P2Id == p2id
        //                group x by new { x.Day } into grp
        //                select new
        //                {
        //                    grp.Key.Day,
        //                    HighCount = grp.Sum(q => q.HighCount),
        //                    MediumCount = grp.Sum(q => q.MediumCount),
        //                    LowCount = grp.Sum(q => q.LowCount),
        //                    Score = grp.Sum(q => q.Score),
        //                    FlightCount = grp.Sum(q => q.FlightCount),
        //                    EventCount = grp.Sum(q => q.EventCount),
        //                    ScorePercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
        //                };
        //    var ds = query.ToList();
        //    var result = new
        //    {
        //        data = ds,
        //        TotalFilght = ds.Sum(q => q.FlightCount),
        //        TotalEvent = ds.Sum(q => q.EventCount),
        //        TotalHighLevel = ds.Sum(q => q.HighCount),
        //        TotalMediumLevel = ds.Sum(q => q.MediumCount),
        //        TotalLowLevel = ds.Sum(q => q.LowCount),
        //        TotalScores = ds.Sum(q => q.Score),
        //        AverageEvents = ds.Average(q => q.EventCount),
        //        AverageScores = ds.Average(q => q.Score),
        //    };

        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}

        [HttpGet]
        [Route("api/fdm/fo/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetFDMFoMonthly(int year, int month, int cptId)
        {
            var query = from x in context.FDMFoMonthlies
                            // where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt && x.P2Id == cptId
                        where x.year == year && x.P2Id == cptId
                        //where x.YearMonth >= ymf && x.YearMonth <= ymt && x.P2Id == cptId
                        group x by new { x.month } into grp
                        select new
                        {
                            grp.Key.month,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighLevelCount = grp.Sum(q => q.HighCount),
                            MediumLevelCount = grp.Sum(q => q.MediumCount),
                            LowLevelCount = grp.Sum(q => q.LowCount),
                            EventsCount = grp.Sum(q => q.EventCount),
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                            HighPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.HighCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            MediumPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.MediumCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            LowPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.LowCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            HighScore = grp.Sum(q => q.HighCount) * 4,
                            LowScore = grp.Sum(q => q.LowCount),
                            MediumScore = grp.Sum(q => q.MediumCount) * 2,

                            ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100.0,

                        };
            var ds = query.ToList();
            var result = new
            {
                data = ds,
                TotalFlightCount = ds.Sum(q => q.FlightCount),
                TotalEventsCount = ds.Sum(q => q.EventsCount),
                TotalScores = ds.Sum(q => q.Scores),

                EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventsCount) * 1.0 / ds.Sum(q => q.FlightCount) : 0,
                ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Scores) * 1.0 / ds.Sum(q => q.FlightCount) * 1.0,

                //EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventsCount) * 100.0 / ds.Sum(q => q.FlightCount) : 0,
                //ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Scores) * 100.0 / ds.Sum(q => q.FlightCount) * 1.0,

                TotalHighCount = ds.Sum(q => q.HighLevelCount),
                TotalMediumCount = ds.Sum(q => q.MediumLevelCount),
                TotalLowCount = ds.Sum(q => q.LowLevelCount),
                TotalHighScore = ds.Sum(q => q.HighScore),
                TotalMediumScore = ds.Sum(q => q.MediumScore),
                TotalLowScore = ds.Sum(q => q.LowScore),
            };



            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/fo/event/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetFDMFoEventMonthly(int year, int month, int cptId)
        {
            var d1 = new DateTime(year, month, 1);
            var dates = new List<int?>();
            dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            for (var i = 0; i <= 11; i++)
            {
                d1 = d1.AddMonths(-1);
                dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            }
            var query = from x in context.FDMFoMonthlies
                            // where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt && x.P2Id == cptId
                        where dates.Contains(x.YearMonth) && x.P2Id == cptId
                        //where x.YearMonth >= ymf && x.YearMonth <= ymt && x.P2Id == cptId
                        group x by new { x.month } into grp
                        select new
                        {
                            grp.Key.month,
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighLevelCount = grp.Sum(q => q.HighCount),
                            MediumLevelCount = grp.Sum(q => q.MediumCount),
                            LowLevelCount = grp.Sum(q => q.LowCount),
                            EventsCount = grp.Sum(q => q.EventCount),
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                            HighPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.HighCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            MediumPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.MediumCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            LowPercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : grp.Sum(q => q.LowCount) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                            HighScore = grp.Sum(q => q.HighCount) * 4,
                            LowScore = grp.Sum(q => q.LowCount),
                            MediumScore = grp.Sum(q => q.MediumCount) * 2,

                            ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100.0,

                        };
            var ds = query.ToList();
            var result = new
            {
                data = ds,
                TotalFlightCount = ds.Sum(q => q.FlightCount),
                TotalEventsCount = ds.Sum(q => q.EventsCount),
                TotalScores = ds.Sum(q => q.Scores),

                EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventsCount) * 1.0 / ds.Sum(q => q.FlightCount) : 0,
                ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Scores) * 1.0 / ds.Sum(q => q.FlightCount) * 1.0,

                //EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventsCount) * 100.0 / ds.Sum(q => q.FlightCount) : 0,
                //ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Scores) * 100.0 / ds.Sum(q => q.FlightCount) * 1.0,

                TotalHighCount = ds.Sum(q => q.HighLevelCount),
                TotalMediumCount = ds.Sum(q => q.MediumLevelCount),
                TotalLowCount = ds.Sum(q => q.LowLevelCount),
                TotalHighScore = ds.Sum(q => q.HighScore),
                TotalMediumScore = ds.Sum(q => q.MediumScore),
                TotalLowScore = ds.Sum(q => q.LowScore),
            };



            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }


        //[HttpGet]
        //[Route("api/fdm/fo/register/{p2id}/{mf}/{mt}")]
        //public async Task<DataResponse> GetRegFoDaily(int p2id, DateTime mf, DateTime mt)
        //{
        //    var query = from x in context.FDMRegFoDaily
        //                where x.Day >= df && x.Day <= dt && x.P2Id == p2id
        //                group x by new { x.Register, x.RegisterID } into grp
        //                select new
        //                {
        //                    grp.Key.Register,
        //                    grp.Key.RegisterID,
        //                    FlightCount = grp.Sum(q => q.FlightCount),
        //                    HighLevelCount = grp.Sum(q => q.HighCount),
        //                    MediumLevelCount = grp.Sum(q => q.MediumCount),
        //                    LowLevelCount = grp.Sum(q => q.LowCount),

        //                };

        //    var result = query.ToList();
        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}


        [HttpGet]
        [Route("api/fdm/fo/register/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetRegFoMonthly(int year, int month, int cptId)
        {
            var query = from x in context.FDMRegFoMonthlies
                            //where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt && x.P2Id == cptId
                            // where x.YearMonth >= ymf && x.YearMonth <= ymt && x.P2Id == cptId
                        where x.year == year && x.month == month && x.P2Id == cptId
                        group x by new { x.Register, x.RegisterId } into grp
                        select new
                        {
                            grp.Key.Register,
                            EventCount = grp.Sum(q => q.EventCount),
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            HighScore = grp.Sum(q => q.HighCount) * 4,
                            LowScore = grp.Sum(q => q.LowCount),
                            MediumScore = grp.Sum(q => q.MediumCount) * 2,
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),

                            ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            //EventPerFlight = grp.Sum(q => q.EventCount) * 100.0 / grp.Sum(q => q.FlightCount) * 1.0,



                        };

            var result = query.ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        //[HttpGet]
        //[Route("api/fdm/register/daily/{df}/{dt}/{reg}")]
        //public async Task<DataResponse> GetRegdaily(DateTime df, DateTime dt, string reg)
        //{
        //    var query = from x in context.FDMRegDailies
        //                where x.FlightDate >= df && x.FlightDate <= dt && x.Register == reg
        //                group x by new { x.FlightDate } into grp
        //                select new
        //                {
        //                    grp.Key.FlightDate,
        //                    EventCount = grp.Sum(q => q.EventCount),
        //                    FlightCount = grp.Sum(q => q.FlightCount),
        //                    HighCount = grp.Sum(q => q.HighCount),
        //                    MediumCount = grp.Sum(q => q.MediumCount),
        //                    LowCount = grp.Sum(q => q.LowCount),
        //                    Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
        //                    ScorePercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100,

        //                };

        //    var ds = query.ToList();
        //    var result = new
        //    {
        //        data = ds,
        //        TotalFilght = ds.Sum(q => q.FlightCount),
        //        TotalEvent = ds.Sum(q => q.EventCount),
        //        TotalHighLevel = ds.Sum(q => q.HighCount),
        //        TotalMediumLevel = ds.Sum(q => q.MediumCount),
        //        TotalLowLevel = ds.Sum(q => q.LowCount),
        //        TotalScores = ds.Sum(q => q.Scores),
        //        AverageEvents = ds.Average(q => q.EventCount),
        //        AverageScores = ds.Average(q => q.Scores),

        //    };
        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}

        [HttpGet]
        [Route("api/fdm/register/monthly/{ymf}/{ymt}/{reg}")]
        public async Task<DataResponse> GetRegMonthly(int ymf, int ymt, string reg)
        {


            try
            {

                var query = from x in context.FDMRegMonthlies
                            where x.YearMonth >= ymf && x.YearMonth <= ymt && x.Register == reg
                            group x by new { x.YearMonth } into grp
                            select new
                            {
                                grp.Key.YearMonth,
                                EventCount = grp.Sum(q => q.EventCount),
                                FlightCount = grp.Sum(q => q.FlightCount),
                                HighCount = grp.Sum(q => q.HighCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                                ScorePerFlight = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                                EventPerFlight = grp.Sum(q => q.FlightCount) != 0 ? grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) : 0,
                                ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,
                            };

                var ds = query.ToList();
                var result = new
                {
                    data = ds,
                    TotalFilght = ds.Sum(q => q.FlightCount),
                    TotalEvent = ds.Sum(q => q.EventCount),
                    TotalHighLevel = ds.Sum(q => q.HighCount),
                    TotalMediumLevel = ds.Sum(q => q.MediumCount),
                    TotalLowLevel = ds.Sum(q => q.LowCount),
                    TotalScores = ds.Sum(q => q.Scores),
                    AverageEvents = ds.Average(q => q.EventCount),
                    AverageScores = ds.Average(q => q.Scores),
                    EventPerFlight = ds.Sum(q => q.FlightCount) != 0 ? ds.Sum(q => q.EventCount) * 1.0 / ds.Sum(q => q.FlightCount) : 0,
                    ScorePerEvent = ds.Sum(q => q.EventCount) == 0 ? 0 : ds.Sum(q => q.Scores) * 1.0 / ds.Sum(q => q.EventCount) * 1.0,
                    ScorePerFlight = ds.Sum(q => q.FlightCount) == 0 ? 0 : ds.Sum(q => q.Scores) * 1.0 / ds.Sum(q => q.FlightCount) * 1.0,

                };

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    Data = ex.InnerException,
                    IsSuccess = false
                };
            }
        }



      

        //[HttpGet]
        //[Route("api/fdm/register/cpt/daily/{df}/{dt}/{reg}")]
        //public async Task<DataResponse> GetRegCptDaily(DateTime df, DateTime dt, string reg)
        //{
        //    var query = from x in context.FDMRegCptDaily_
        //                where x.Day >= df && x.Day <= dt && x.Register == reg
        //                group x by new { x.CptCode } into grp
        //                select new
        //                {
        //                    grp.Key.CptCode,
        //                    HighCount = grp.Sum(q => q.HighCount),
        //                    MediumCount = grp.Sum(q => q.MediumCount),
        //                    LowCount = grp.Sum(q => q.LowCount),
        //                    EventCount = grp.Sum(q => q.EventCount),
        //                    FlightCount = grp.Sum(q => q.FlightCount),
        //                    Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
        //                    ScorePercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
        //                };

        //    var result = query.ToList();
        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };
        //}

        [HttpGet]
        [Route("api/fdm/register/cpt/monthly/{ymf}/{ymt}/{reg}")]
        public async Task<DataResponse> GetRegCptMonthly(int ymf, int ymt, string reg)
        {
            var query = from x in context.FDMRegCptMonthlies
                            //where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Register == reg && x.Year >= yf && x.Year <= yt
                        where x.YearMonth >= ymf && x.YearMonth <= ymt
                        group x by new { x.CptCode } into grp
                        select new
                        {
                            grp.Key.CptCode,
                            EventCount = grp.Sum(q => q.EventCount),
                            FlightCount = grp.Sum(q => q.FlightCount),
                            HighCount = grp.Sum(q => q.HighCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                            ScorePercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100,
                        };

            var result = query.ToList();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        //[HttpGet]
        //[Route("api/fdm/register/events/daily/{df}/{dt}/{reg}")]
        //public async Task<DataResponse> GetRegEventsNameDaily(DateTime df, DateTime dt, string reg)
        //{
        //    var query = from x in context.FDMRegEventDailies
        //                where x.Day >= df && x.Day <= dt && x.register == reg
        //                group x by new { x.EventName } into grp
        //                select new
        //                {
        //                    EventCount = grp.Sum(q => q.EventCount),
        //                    grp.Key.EventName,
        //                    Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
        //                };
        //    var result = query.ToList();


        //    return new DataResponse()
        //    {
        //        IsSuccess = true,
        //        Data = result
        //    };

        //}

        [HttpGet]
        [Route("api/fdm/register/events/monthly/{ymf}/{ymt}/{reg}")]
        public async Task<DataResponse> GetRegEventsNameMonthly(int ymf, int ymt, string reg)
        {

            //var query = from x in context.FDMRegEventMonthly1
            //                // where x.Month >= mf && x.Month <= mt && x.Year == yf && x.Year <= yt && x.register == reg
            //            where x.YearMonth >= ymf && x.YearMonth <= ymt && x.register == reg

            var query = from x in context.FDMRegEventMonthlies
                            // where x.Month >= mf && x.Month <= mt && x.Year == yf && x.Year <= yt && x.register == reg
                        where x.YearMonth >= ymf && x.YearMonth <= ymt && x.register == reg
                        group x by new { x.EventName } into grp
                        select new
                        {
                            EventCount = grp.Sum(q => q.EventCount),
                            grp.Key.EventName,
                            Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                        };

            var result = query.ToList();


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };

        }




        [HttpGet]
        [Route("api/fdm/get/cpt/monthly/{yf}/{yt}/{mf}/{mt}")]
        public async Task<DataResponse> GetCptByMonth(int yf, int yt, int mf, int mt)
        {
            try
            {
                var query = (from x in context.FDMCptAlls
                             where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt
                             select x
                            ).ToList();
                var result = (from x in query

                              group x by new { x.CptId, x.CptName, x.JobGroup } into grp
                              select new
                              {
                                  CptName = grp.Key.CptName,
                                  CptId = grp.Key.CptId,
                                  Items = grp.OrderBy(q => q.Month).ToList(),
                                  EventsCount = grp.Sum(q => q.EventCount),
                                  Flights = grp.Sum(q => q.FlightCount),
                                  Scores = grp.Sum(q => q.Score),
                                  ScorePerFlight = grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                  JobGroup = grp.Key.JobGroup
                              }).ToList();

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                int jjj = 0;
                var test = ex.InnerException;
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = 1
                };
            }

        }

        [HttpGet]
        [Route("api/fdm/get/cpt/phase/monthly/{year}/{month}/{cptId}")]
        public async Task<DataResponse> GetCptPhaseMonthly(int year, int month, int cptId)
        {
            var query = (from x in context.FDMPhaseMonthlies
                             //where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yt && x.Year <= yf && x.CrewId == cptId
                             //where x.YearMonth >= ymf && x.YearMonth <= ymt && x.CrewId == cptId
                         where x.year == year && x.month == month && x.CrewId == cptId
                         select x).ToList();
            var result = (from x in query
                          group x by new { x.Phase, x.Name } into grp
                          select new
                          {
                              Phase = grp.Key.Phase,
                              CptName = grp.Key.Name,
                              Score = grp.Sum(q => q.Score)
                          }).ToList();

            //var result = query.ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }

        [HttpGet]
        [Route("api/fdm/get/cpt/phase/monthly2/{ymf}/{ymt}/{cptId}")]
        public async Task<DataResponse> GetCptPhaseMonthly2(int ymf, int ymt, int cptId)
        {
            var query = (from x in context.FDMPhaseMonthlies
                             //where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yt && x.Year <= yf && x.CrewId == cptId
                         where x.YearMonth >= ymf && x.YearMonth <= ymt && x.CrewId == cptId
                         //where x.Year == year && x.Month == month && x.CrewId == cptId
                         select x).ToList();
            var result = (from x in query
                          group x by new { x.Phase, x.Name } into grp
                          select new
                          {
                              Phase = grp.Key.Phase,
                              CptName = grp.Key.Name,
                              Score = grp.Sum(q => q.Score),
                              EventCount = grp.Sum(q => q.EventCount)
                          }).ToList();

            //var result = query.ToList();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result
            };
        }




      

       



       




        [HttpGet]
        [Route("api/get/fdm/top/cpt2/{yf}/{yt}/{mf}/{mt}")]
        public async Task<DataResponse> GetTopCpt2(int yf, int yt, int mf, int mt)
        {

            //var query = (from x in context.FDMCptAll
            //             where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt
            //             select x).ToList();
            //var result = (from x in query
            //              group x by new { x.CptName, x.CptCode, x.CptId } into grp
            //              select new
            //              {
            //                  CptName = grp.Key.CptName,
            //                  CptCode = grp.Key.CptCode,
            //                  CptId = grp.Key.CptId,
            //                  Score = grp.Sum(q => q.Score)
            //              });

            //return new DataResponse()
            //{
            //    IsSuccess = true,
            //    Data = result.OrderByDescending(q => q.Score).Take(10)
            //};


            var query = from x in context.FDMCptMonthlies
                        where x.Month >= mf && x.Month <= mt && x.Year >= yf && x.Year <= yt
                        group x by new { x.CptName, x.CptId, x.CptCode } into grp
                        select new
                        {
                            grp.Key.CptCode,
                            grp.Key.CptId,
                            grp.Key.CptName,
                            HighCount = grp.Sum(q => q.HighCount),
                            LowCount = grp.Sum(q => q.LowCount),
                            MediumCount = grp.Sum(q => q.MediumCount),
                            FlightCount = grp.Sum(q => q.FlightCount),
                            Score = grp.Sum(q => q.Score),
                            HighScore = grp.Sum(q => q.HighScore),
                            LowScore = grp.Sum(q => q.LowScore),
                            MediumScore = grp.Sum(q => q.MediumScore),

                            ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                            EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                            ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,

                            //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                            //EventPerFlight = grp.Sum(q => q.EventPerFlight),
                            //EventsCount = grp.Sum(q => q.EventCount),
                            //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),

                        };


            var result = query.OrderByDescending(q => q.ScorePerFlight).Take(10).ToList();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = result

            };
        }

        [HttpGet]
        [Route("api/get/fdm/top/cpt/month/{year}/{month}")]
        public async Task<DataResponse> GetTopCptByMonth(int year, int month)
        {

            var d1 = new DateTime(year, month, 1);
            var dates = new List<int?>();
            dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            for (var i = 0; i <= 11; i++)
            {
                d1 = d1.AddMonths(-1);
                dates.Add(Convert.ToInt32(d1.ToString("yyyyMM")));
            }
            try
            {
                var query = from x in context.FDMCptMonthlies
                                //where x.Year == year && x.Month == month
                            where dates.Contains(x.YearMonth)
                            group x by new { x.CptName, x.CptId, x.CptCode } into grp
                            select new
                            {
                                grp.Key.CptCode,
                                grp.Key.CptId,
                                grp.Key.CptName,
                                Month = grp.Sum(q => q.Month),
                                HighCount = grp.Sum(q => q.HighCount),
                                LowCount = grp.Sum(q => q.LowCount),
                                MediumCount = grp.Sum(q => q.MediumCount),
                                FlightCount = grp.Sum(q => q.FlightCount),
                                Score = grp.Sum(q => q.Score),
                                //HighScore = grp.Sum(q => q.HighScore),
                                //LowScore = grp.Sum(q => q.LowScore),
                                //MediumScore = grp.Sum(q => q.MediumScore),

                                ScorePerFlight = (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,
                                EventsCount = grp.Sum(q => q.HighCount) + grp.Sum(q => q.MediumCount) + grp.Sum(q => q.LowCount),
                                EventPerFlight = grp.Sum(q => q.EventCount) * 1.0 / grp.Sum(q => q.FlightCount) * 1.0,

                                //EventPerFlight = grp.Sum(q => q.EventPerFlight),
                                ScorePerEvent = grp.Sum(q => q.EventCount) == 0 ? 0 : grp.Sum(q => q.Score) * 1.0 / grp.Sum(q => q.EventCount) * 1.0,

                                //ScorePerFlight = grp.Sum(q => q.ScorePerFlight),
                                ////EventPerFlight = grp.Sum(q => q.EventPerFlight),
                                //EventsCount = grp.Sum(q => q.EventCount),
                                //ScorePerEvent = grp.Sum(q => q.ScorePerEvent),

                            };

                var result = query.OrderByDescending(q => q.ScorePerFlight).Take(10).ToList();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex

                };
            }
        }

        [HttpGet]
        [Route("api/get/fdm/all/cpt/{yf}/{yt}/{mf}/{mt}")]
        public async Task<DataResponse> GetAllCpt(int yf, int yt, int mf, int mt)
        {
            var query = (from x in context.FDMCptAlls
                         where x.Month >= (mf + 1) && x.Month <= (mt + 1) && x.Year >= yf && x.Year <= yt
                         select x).ToList();
            var result = (from x in query
                          group x by new { x.HighCount, x.MediumCount, x.LowCount } into grp
                          select new
                          {
                              HighCount = grp.Sum(q => q.HighCount),
                              MediumCount = grp.Sum(q => q.MediumCount),
                              LowCount = grp.Sum(q => q.LowCount),
                              FlightCount = grp.Sum(q => q.FlightCount),
                              IncidentCount = grp.Sum(q => q.EventCount),
                              Scores = grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount),
                              ScorePercentage = grp.Sum(q => q.FlightCount) == 0 ? 0 : (grp.Sum(q => q.HighCount) * 4 + grp.Sum(q => q.MediumCount) * 2 + grp.Sum(q => q.LowCount)) * 1.0 / grp.Sum(q => q.FlightCount) * 100,

                          });

            var result2 = new
            {
                data = result,
                TotalFlights = result.Sum(q => q.FlightCount),
                TotalHighLevel = result.Sum(q => q.HighCount),
                TotalMediumLevel = result.Sum(q => q.MediumCount),
                TotalLowLevel = result.Sum(q => q.LowCount),
                TotalEvents = result.Sum(q => q.IncidentCount),
                TotalScore = result.Sum(q => q.Scores),

            };



            return new DataResponse()
            {
                IsSuccess = true,
                Data = result2
            };
        }

      



        [HttpGet]
        [Route("api/fdm/delete/event/{id}")]
        public async Task<DataResponse> DeleteEvent(int id)
        {
            try
            {
                var item = context.FDMs.Single(q => q.Id == id);
                context.FDMs.Remove(item);
                context.SaveChanges();
                return new DataResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = ex
                };
            }
        }

        [HttpGet]
        [Route("api/fdm/visible/{year}/{month}")]
        public async Task<DataResponse> MakeVisible(int year, int month)
        {
            try
            {
                var record = context.FDMs.Where(q => q.Date.Value.Year == year && q.Date.Value.Month == month).ToList();
                for (int i = 0; i < record.Count; i++)
                {
                    record[i].IsVisible = true;
                    record[i].IsVisibleDate = DateTime.Now;
                }
                await context.SaveChangesAsync();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = record
                };
            }
            catch (Exception ex)
            {
                var innerExeption = ex.InnerException;
                var message = ex.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = "InnerExeption: " + innerExeption + " " + "Message: " + message
                };
            }
        }


        //////[HttpGet]
        //////[Route("api/fdm/cpt/route/{yf}/{yt}/{mf}/{mt}/{crewId}")]
        //////public async Task<DataResponse> GetCptRoute(int crewId, int yf, int yt, int mf, int mt)
        //////{
        //////    try
        //////    {
        //////        var query = from x in context.FDMCptAirportMonthlies
        //////                    where x.CrewId == crewId && x.Year >= yf && x.Year <= yt && x.Month >= mf && x.Month <= mt
        //////                    group x by new { x.CrewId, x.Name, x.JobGroup, x.FromAirportIATA, x.ToAirportIATA } into grp
        //////                    select new
        //////                    {
        //////                        HighCount = grp.Sum(q => q.HighCount),
        //////                        MediumCount = grp.Sum(q => q.MediumCount),
        //////                        LowCount = grp.Sum(q => q.LowCount),
        //////                        FlightCount = grp.Sum(q => q.FlightCount),
        //////                        Route = grp.Key.FromAirportIATA + "-" + grp.Key.ToAirportIATA
        //////                    };

        //////        var result = query.ToList();
        //////        return new DataResponse()
        //////        {
        //////            IsSuccess = true,
        //////            Data = result,
        //////        };
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        return new DataResponse()
        //////        {
        //////            IsSuccess = false,
        //////            Data = "Message: " + ex.Message + " Inner: " + ex.InnerException
        //////        };
        //////    }
        //////}


    }
}