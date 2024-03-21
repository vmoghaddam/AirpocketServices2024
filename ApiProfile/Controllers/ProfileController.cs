﻿using ApiProfile.Models;
using ApiProfile.ViewModels;
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

namespace ApiProfile.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProfileController : ApiController
    {
        [Route("api/profile/employee/save")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostEmployee(ViewModels.Employee dto)
        {
            var context = new Models.dbEntities();

            var nidCheck = await context.People.Where(q => q.Id != dto.PersonId && q.NID == dto.Person.NID).FirstOrDefaultAsync();
            if (nidCheck != null)
            {
                return Exceptions.getDuplicateException("Person-01", "NID");
            }

            Models.Person person = null;
            if (dto.PersonId != -1)
                person = await context.People.Where(q => q.Id == dto.PersonId).FirstOrDefaultAsync();
            else
                person = await context.People.Where(q => q.NID == dto.Person.NID).FirstOrDefaultAsync();
            if (person == null)
            {
                person = new Models.Person();
                person.DateCreate = DateTime.Now;
                context.People.Add(person);
            }
            ViewModels.Person.Fill(person, dto.Person);
            var cid = (int)dto.CustomerId;
            Models.PersonCustomer personCustomer = await context.PersonCustomers.Where(q => q.CustomerId == cid && q.PersonId == dto.Person.PersonId).FirstOrDefaultAsync();
            //await unitOfWork.PersonRepository.GetPersonCustomer((int)dto.CustomerId, dto.Person.PersonId);
            if (personCustomer == null)
            {
                personCustomer = new Models.PersonCustomer();

                person.PersonCustomers.Add(personCustomer);
            }
            ViewModels.PersonCustomer.Fill(personCustomer, dto);
            Models.Employee employee = await context.Employees.Where(q => q.Id == personCustomer.Id).FirstOrDefaultAsync();
            if (employee == null)
                employee = new Models.Employee();
            personCustomer.Employee = employee;
            ViewModels.Employee.Fill(employee, dto);

            FillEmployeeLocations(context, employee, dto);

            FillAircraftTypes(context, person, dto);
            FillDocuments(context, person, dto);

            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;


            dto.Id = employee.Id;
            return Ok(dto);
        }

        [Route("api/profile/employee/nid/{cid}/{nid}")]
        public async Task<IHttpActionResult> GetEmployee(string nid, int cid)
        {
            var context = new Models.dbEntities();
            ViewModels.Employee employee = null;
            var entity = await context.People.SingleOrDefaultAsync(q => q.NID == nid && !q.IsDeleted);
            if (entity == null)
                return Ok();
            employee = new ViewModels.Employee();
            employee.Person = new ViewModels.Person();
            ViewModels.Person.FillDto(entity, employee.Person);
            var actypes = await context.ViewPersonAircraftTypes.Where(q => q.PersonId == entity.Id).ToListAsync();
            employee.Person.AircraftTypes = ViewModels.PersonAircraftType.GetDtos(actypes);
            employee.Person.AircraftTypes2 = ViewModels.PersonAircraftType2.GetDtos(actypes);

            var doc = await context.ViewPersonDocuments.Where(q => q.PersonId == entity.Id).ToListAsync();
            var docIds = doc.Select(q => q.Id).ToList();
            var files = await context.ViewPersonDocumentFiles.Where(q => q.PersonId == entity.Id).ToListAsync();
            employee.Person.Documents = ViewModels.PersonDocument.GetDtos(doc, files);

            var pc = context.PersonCustomers.SingleOrDefault(q => q.CustomerId == cid && q.PersonId == entity.Id && !q.IsDeleted);

            if (pc != null)
            {
                var emp = await context.Employees.FirstOrDefaultAsync(q => q.Id == pc.Id);
                if (emp != null)
                {
                    employee.CustomerId = cid;
                    employee.DateActiveEnd = pc.DateActiveEnd;
                    employee.DateActiveStart = pc.DateActiveStart;
                    employee.DateJoinCompany = pc.DateJoinCompany;
                    employee.DateJoinCompanyP = pc.DateJoinCompanyP;
                    employee.DateConfirmedP = pc.DateConfirmedP;
                    employee.DateConfirmed = pc.DateConfirmed;
                    employee.DateLastLogin = pc.DateLastLogin;
                    employee.DateLastLoginP = pc.DateLastLoginP;
                    employee.DateRegister = pc.DateRegister;
                    employee.DateRegisterP = pc.DateRegisterP;
                    employee.Id = pc.Id;
                    employee.IsActive = pc.IsActive;
                    employee.Password = pc.Password;
                    employee.PersonId = entity.Id;
                    employee.GroupId = pc.GroupId;
                    employee.C1GroupId = pc.C1GroupId;
                    employee.C2GroupId = pc.C2GroupId;
                    employee.C3GroupId = pc.C3GroupId;
                    employee.PID = emp.PID;
                    employee.Phone = emp.Phone;
                    employee.BaseAirportId = emp.BaseAirportId;
                    employee.DateInactiveBegin = emp.DateInactiveBegin;
                    employee.DateInactiveEnd = emp.DateInactiveEnd;
                    employee.InActive = emp.InActive;
                    var locs = await context.ViewEmployeeLocations.Where(q => q.EmployeeId == pc.Id).ToListAsync();
                    employee.Locations = ViewModels.EmployeeLocation.GetDtos(locs);


                }


            }

            //soosk
            ///var employee = await unitOfWork.PersonRepository.GetEmployeeDtoByNID(nid, cid);
            return Ok(employee);
        }


        [Route("api/profile/abs/{id}")]
        public async Task<IHttpActionResult> GetProfileAbs(  int  id)
        {
            var context = new Models.dbEntities();
            var profile = await context.ViewProfiles.FirstOrDefaultAsync(q => q.Id == id);
            var abs = new
            {
                profile.Name,
                profile.Id,
                profile.PID,
                profile.PersonId,
                profile.JobGroup,
                profile.NID,
                profile.Mobile

            };
            return Ok(abs);
        }



        [Route("api/profile/opc/nid/{nid}")]
        public async Task<IHttpActionResult> GetOPC(string nid)
        {
            try
            {


                var context = new Models.dbEntities();
                
                var employee = await context.ViewOPCs.Where(q => q.NID == nid).FirstOrDefaultAsync();
                //soosk
                ///var employee = await unitOfWork.PersonRepository.GetEmployeeDtoByNID(nid, cid);
                var result = new { 
                  Person=employee,
                };
                return Ok(result );
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER:  " + ex.InnerException.Message;
                return Ok(msg);
            }
        }


        [Route("api/profile/sms/{id}")]
        public async Task<IHttpActionResult> GetSMS(int id)
        {
            try
            {


                var context = new Models.dbEntities();

                var sms = await context.ViewCrewPickupSMS.Where(q => q.PersonId==id && q.IsVisited==0).OrderByDescending(q=>q.DateSent).Take(20).ToListAsync();
                //soosk
                ///var employee = await unitOfWork.PersonRepository.GetEmployeeDtoByNID(nid, cid);
                 
                return Ok(sms);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   INNER:  " + ex.InnerException.Message;
                return Ok(msg);
            }
        }



        public class UpdTrnDto
        {
            public List<int> ids { get; set; }
            public DateTime issue { get; set; }
            public DateTime expire { get; set; }
            public string type { get; set; }
        }
        [Route("api/upd/trn")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostUpdTrn(UpdTrnDto dto)
        {
            var context = new Models.dbEntities();
            var people = context.People.Where(q => dto.ids.Contains(q.Id)).ToList();

            foreach (var person in people)
            {
                switch (dto.type)
                {
                    //dg
                    case "DG":
                    case "DIS-DG":

                        person.DangerousGoodsExpireDate = dto.expire;
                        person.DangerousGoodsIssueDate = dto.issue;

                        break;
                    //1	SEPT-P
                    case "SEPTP":

                        person.SEPTPExpireDate = dto.expire;
                        person.SEPTPIssueDate = dto.issue;

                        break;
                    //2   SEPT - T
                    case "SEPTT":

                        person.SEPTExpireDate = dto.expire;
                        person.SEPTIssueDate = dto.issue;

                        break;
                    //4	CRM
                    case "CRM":

                        person.UpsetRecoveryTrainingExpireDate = dto.expire;
                        person.UpsetRecoveryTrainingIssueDate = dto.issue;

                        break;
                    //5	CCRM
                    case "CCRM":

                        person.CCRMExpireDate = dto.expire;
                        person.CCRMIssueDate = dto.issue;

                        break;
                    //6	SMS
                    case "SMS":
                    case "DIS-SMS":

                        person.SMSExpireDate = dto.expire;
                        person.SMSIssueDate = dto.issue;

                        break;
                    //7	AV-SEC
                    case "AVSEC":
                    case "DIS-AVSEC":

                        person.AviationSecurityExpireDate = dto.expire;
                        person.AviationSecurityIssueDate = dto.issue;

                        break;
                    //8	COLD-WX
                    case "COLDWX":

                        person.ColdWeatherOperationExpireDate = dto.expire;
                        person.ColdWeatherOperationIssueDate = dto.issue;

                        break;
                    //9	HOT-WX
                    case "HOTWX":

                        person.HotWeatherOperationExpireDate = dto.expire;
                        person.HotWeatherOperationIssueDate = dto.issue;

                        break;
                    //10	FIRSTAID
                    case "FIRSTAID":

                        person.FirstAidExpireDate = dto.expire;
                        person.FirstAidIssueDate = dto.issue;

                        break;
                    //lpc
                    case "LINE":

                        person.LineExpireDate = dto.expire;
                        person.LineIssueDate = dto.issue;

                        break;

                    //lpr
                    case "TYPEMD":

                        person.TypeMDExpireDate = dto.expire;
                        person.TypeMDIssueDate = dto.issue;
                        // person.ProficiencyCheckDateOPC = cp.DateIssue;

                        break;
                    case "TYPE737":

                        person.Type737ExpireDate = dto.expire;
                        person.Type737IssueDate = dto.issue;
                        break;
                    case "TYPEAIRBUS":

                        person.TypeAirbusExpireDate = dto.expire;
                        person.TypeAirbusIssueDate = dto.issue;

                        break;
                    //grt

                    //recurrent
                    case "RECURRENT":

                        person.RecurrentExpireDate = dto.expire;
                        person.RecurrentIssueDate = dto.issue;

                        break;
                    //fmt
                    case "FMT":

                        person.EGPWSExpireDate = dto.expire;
                        person.EGPWSIssueDate = dto.issue;

                        break;
                    //                      { type: 'DIS-AT', title: 'AIR TRAFFIC', REC: 3 },
                    case "DIS-AT":

                        person.ExpireDate5 = dto.expire;
                        person.IssueDate5 = dto.issue;

                        break;
                    //{ type: 'DIS-AI', title: 'AV. INTRODUCTION', REC: 3 },
                    case "DIS-AI":

                        person.ExpireDate7 = dto.expire;
                        person.IssueDate7 = dto.issue;

                        break;
                    //{ type: 'DIS-RC', title: 'COMMUNICATION', REC: 3 },
                    case "DIS-RC":

                        person.LRCExpireDate = dto.expire;
                        person.LRCIssueDate = dto.issue;

                        break;

                    //{ type: 'DIS-FM', title: 'FLIGHT MONITORING', REC: 3 },
                    case "DIS-FM":

                        person.ExpireDate4 = dto.expire;
                        person.IssueDate4 = dto.issue;

                        break;
                    //{ type: 'DIS-ERP', title: 'ERP', REC: 3 },
                    case "DIS-ERP":

                        person.ERPExpireDate = dto.expire;
                        person.ERPIssueDate = dto.issue;

                        break;
                    //{ type: 'DIS-MEL', title: 'MEL', REC: 3 },
                    case "DIS-MEL":

                        person.MELExpireDate = dto.expire;
                        person.MELIssueDate = dto.issue;

                        break;
                    //{ type: 'DIS-NAV', title: 'NAVIGATION', REC: 3 },
                    case "DIS-NAV":

                        person.ExpireDate6 = dto.expire;
                        person.IssueDate6 = dto.issue;

                        break;
                    //{ type: 'DIS-MET', title: 'METEOROLOGY', REC: 3 },
                    case "DIS-MET":

                        person.METExpireDate = dto.expire;
                        person.METIssueDate = dto.issue;

                        break;
                    //{ type: 'DIS-MB', title: 'M & B', REC: 3 },
                    case "DIS-MB":

                        person.MBExpireDate = dto.expire;
                        person.MBIssueDate = dto.issue;

                        break;
                    //{ type: 'DIS-AIRLAW', title: 'AIR LAW', REC: 3 },
                    case "DIS-AIRLAW":

                        person.ExpireDate8 = dto.expire;
                        person.IssueDate8 = dto.issue;

                        break;
                    //{ type: 'DIS-FP', title: 'FLIGHT PLAN', REC: 3 },
                    case "DIS-FP":

                        person.RSPExpireDate = dto.expire;
                        person.RSPIssueDate = dto.issue;

                        break;
                    //{ type: 'DIS-DRM', title: 'DRM', REC: 3 },
                    case "DIS-DRM":

                        person.DRMExpireDate = dto.expire;
                        person.DRMIssueDate = dto.issue;

                        break;
                    //{ type: 'DIS-PER', title: 'PERFORMANCE', REC: 3 },
                    case "DIS-PER":

                        person.PERExpireDate = dto.expire;
                        person.PERIssueDate = dto.issue;

                        break;

                    case "DIS-PH1":

                        person.Phase1ExpireDate = dto.expire;
                        person.Phase1IssueDate = dto.issue;

                        break;

                    case "DIS-PH2":

                        person.Phase2ExpireDate = dto.expire;
                        person.Phase2IssueDate = dto.issue;

                        break;

                    case "DIS-PH3":

                        person.Phase3ExpireDate = dto.expire;
                        person.Phase3IssueDate = dto.issue;

                        break;
                    default:
                        break;
                }
            }
            var result = context.SaveChanges();
            return Ok(result);
        }


        [Route("api/profiles/main/{cid}/{active}/{grp}")]

        public async Task<IHttpActionResult> GetProfilesByCustomerId(int cid, int active, string grp)
        {
            try
            {
                var context = new Models.dbEntities();
                var query = context.ViewProfiles.Where(q => q.CustomerId == cid);
                if (active == 1)
                    query = query.Where(q => q.InActive == false);
                grp = grp.Replace('x', '/');
                if (grp != "-1")
                    query = query.Where(q => q.JobGroupRoot == grp || q.PostRoot==grp);
                var profiles = await query.ToListAsync();

                return Ok(profiles);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " " + ex.InnerException.Message;
                return Ok(msg);
                //throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

        }


        [Route("api/login/info")]

        public async Task<IHttpActionResult> GetLoginInfo(DateTime df, DateTime dt, string user, string city)
        {
            try
            {
                city = city.ToLower();
                var context = new Models.dbEntities();
                dt = dt.AddDays(1).Date;
                var _qry = from x in context.LoginInfoes
                           where x.DateCreate >= df && x.DateCreate < dt
                           select x;
                if (user != "-1")
                    _qry = _qry.Where(q => q.User == user);
                if (city != "-1")
                    _qry = _qry.Where(q => q.LocationCity.ToLower() == city);
                var query = await _qry.Select(q => new { q.Id, q.IP, q.LocationCity, q.User, q.DateCreate, q.Info }).ToListAsync();


                return Ok(query);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        public void FillEmployeeLocations(dbEntities context, Models.Employee employee, ViewModels.Employee dto)
        {
            var exists = context.EmployeeLocations.Where(q => q.EmployeeId == employee.Id).ToList();
            var dtoLocation = dto.Locations.First();
            if (exists == null || exists.Count == 0)
            {
                employee.EmployeeLocations.Add(new Models.EmployeeLocation()
                {
                    DateActiveEnd = dtoLocation.DateActiveEnd,
                    DateActiveEndP = dtoLocation.DateActiveEnd != null ? (Nullable<decimal>)Convert.ToDecimal(Utils.DateTimeUtil.GetPersianDateTimeDigital((DateTime)dtoLocation.DateActiveEnd)) : null,
                    DateActiveStart = dtoLocation.DateActiveStart,
                    DateActiveStartP = dtoLocation.DateActiveStart != null ? (Nullable<decimal>)Convert.ToDecimal(Utils.DateTimeUtil.GetPersianDateTimeDigital((DateTime)dtoLocation.DateActiveStart)) : null,
                    IsMainLocation = dtoLocation.IsMainLocation,
                    LocationId = dtoLocation.LocationId,
                    OrgRoleId = dtoLocation.OrgRoleId,
                    Phone = dtoLocation.Phone,
                    Remark = dtoLocation.Remark

                });
            }
            else
            {
                exists[0].DateActiveEnd = dtoLocation.DateActiveEnd;
                exists[0].DateActiveEndP = dtoLocation.DateActiveEnd != null ? (Nullable<decimal>)Convert.ToDecimal(Utils.DateTimeUtil.GetPersianDateTimeDigital((DateTime)dtoLocation.DateActiveEnd)) : null;
                exists[0].DateActiveStart = dtoLocation.DateActiveStart;
                exists[0].DateActiveStartP = dtoLocation.DateActiveStart != null ? (Nullable<decimal>)Convert.ToDecimal(Utils.DateTimeUtil.GetPersianDateTimeDigital((DateTime)dtoLocation.DateActiveStart)) : null;
                exists[0].IsMainLocation = dtoLocation.IsMainLocation;
                exists[0].LocationId = dtoLocation.LocationId;
                exists[0].OrgRoleId = dtoLocation.OrgRoleId;
                exists[0].Phone = dtoLocation.Phone;
                exists[0].Remark = dtoLocation.Remark;
            }

        }
        public void FillAircraftTypes(dbEntities context, Models.Person person, ViewModels.Employee dto)
        {



            var existing = context.PersonAircraftTypes.Where(q => q.PersonId == person.Id).ToList();
            context.PersonAircraftTypes.RemoveRange(existing);

            /*
            var deleted = (from x in existing
                           where dto.Person.AircraftTypes.FirstOrDefault(q => q.Id == x.Id) == null
                           select x).ToList();
            var added = (from x in dto.Person.AircraftTypes
                         where existing.FirstOrDefault(q => q.Id == x.Id) == null
                         select x).ToList();
            var edited = (from x in existing
                          where dto.Person.AircraftTypes.FirstOrDefault(q => q.Id == x.Id) != null
                          select x).ToList();
            foreach (var x in deleted)
                context.PersonAircraftTypes.Remove(x);*/
            List<PersonAircraftType2> added = new List<PersonAircraftType2>();
            if (dto.Person.AircraftTypes.Count > 0)
                added = dto.Person.AircraftTypes.Select(q => new PersonAircraftType2() { AircraftType = q.AircraftType, AircraftTypeId = q.AircraftTypeId }).ToList();
            else
                added = dto.Person.AircraftTypes2.Select(q => new PersonAircraftType2() { AircraftType = q.AircraftType, AircraftTypeId = q.AircraftTypeId }).ToList();

            foreach (var x in added)
                context.PersonAircraftTypes.Add(new Models.PersonAircraftType()
                {
                    Person = person,
                    AircraftTypeId = x.AircraftTypeId,
                    IsActive = true,
                    Remark = "",
                   // DateLimitBegin = DateTime.Now.AddYears(-5),
                   // DateLimitEnd = x.DateLimitEnd

                }) ;
            




        }


        


        public void FillDocuments(dbEntities context, Models.Person person, ViewModels.Employee dto)
        {
            var existing = context.PersonDocuments.Include("Documents").Where(q => q.PersonId == person.Id).ToList();
            var deleted = (from x in existing
                           where dto.Person.Documents.FirstOrDefault(q => q.Id == x.Id) == null
                           select x).ToList();
            var added = (from x in dto.Person.Documents
                         where existing.FirstOrDefault(q => q.Id == x.Id) == null
                         select x).ToList();
            var edited = (from x in existing
                          where dto.Person.Documents.FirstOrDefault(q => q.Id == x.Id) != null
                          select x).ToList();
            foreach (var x in deleted)
                context.PersonDocuments.Remove(x);
            foreach (var x in added)
            {
                var pd = new Models.PersonDocument()
                {
                    Person = person,

                    Remark = x.Remark,
                    DocumentTypeId = x.DocumentTypeId,
                    Title = x.Title,


                };
                foreach (var file in x.Documents)
                {
                    pd.Documents.Add(new Document()
                    {
                        FileType = file.FileType,
                        FileUrl = file.FileUrl,
                        SysUrl = file.SysUrl,
                        Title = file.Title,

                    });
                }
                context.PersonDocuments.Add(pd);
            }
            foreach (var x in edited)
            {
                var item = dto.Person.Documents.FirstOrDefault(q => q.Id == x.Id);
                if (item != null)
                {
                    x.DocumentTypeId = item.DocumentTypeId;
                    x.Title = item.Title;
                    x.Remark = item.Remark;

                    while (x.Documents.Count > 0)
                    {
                        var f = x.Documents.First();
                        context.Documents.Remove(f);
                    }
                    foreach (var f in item.Documents)
                        x.Documents.Add(new Document()
                        {
                            FileType = f.FileType,
                            FileUrl = f.FileUrl,
                            SysUrl = f.SysUrl,
                            Title = f.Title,

                        });
                }
            }
        }




    }
}
