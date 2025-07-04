using ApiProfile.Models;
using ApiProfile.ViewModels;
using Newtonsoft.Json.Linq;
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

        [Route("api/profile/resend/auth")]

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostAuth()
        {
            var context = new Models.dbEntities();
            var profiles = await context.ViewProfiles.Where(q => q.InActive == false && q.UserId == null).ToListAsync();
            var ids = profiles.Select(q => q.PersonId).ToList();
            var people = await context.People.Where(q => ids.Contains(q.Id)).ToListAsync();
            foreach (var person in people)
            {
                AspNetUser user = new AspNetUser()
                {
                    UserName = person.NID,
                    Email = (person.FirstName[0] + person.LastName[0] + person.NID).ToUpper() + "@AEROTECH.APP",
                    NormalizedEmail = (person.FirstName[0] + person.LastName[0] + person.NID).ToUpper() + "@AEROTECH.APP",
                    NormalizedUserName = person.NID,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    PasswordHash = "ABUOcscaBcucAd+3adgqDoYDGbwJ+vsH31J7wOZWMJi8jlAjxnzNYHrkrPLEUtyNvA==",
                    SecurityStamp = "dde4f3c0-52cf-4f0a-9655-65e6e2c53a07",
                    Id = Guid.NewGuid().ToString(),

                };
                context.AspNetUsers.Add(user);
                person.UserId = user.Id;
                var text = "Dear " + person.FirstName.ToUpper() + " " + person.LastName.ToUpper() + ", " + "\n"
                         + "Your account information:" + "\n"
                         + "Username: " + person.NID + "\n"
                         + "Password: " + "123456" + "\n"
                         + "Please visit the link below to access your panel" + "\n"
                         + ConfigurationManager.AppSettings["pulsepocket_url"];
                Magfa m1 = new Magfa();
                var res = m1.enqueue(1, person.Mobile, text)[0];
                person.FaxTelNumber = res.ToString();

            }
            await context.SaveAsync();
            return Ok(profiles.Select(q => q.NID).ToList());

        }

        DateTime? to_date(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var prts = str.Split('-').Select(q=>Convert.ToInt32(q)).ToList();
            return new DateTime(prts[0],prts[1],prts[2]);

        }
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
            var do_user = false;
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
                do_user = true;


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

            foreach(var cer in dto.Certificates)
            {
                var cer_obj = new CertificateHistory()
                {
                     Person= person,
                      CourseTypeId=cer.course_type_id,
                       DateIssue=to_date(cer.date_issue_str),
                       DateExpire=to_date(cer.date_expire_str),
                       DateCreate=DateTime.Now,
                };
                context.CertificateHistories.Add(cer_obj);
            }

            if (do_user)
            {
                //password: 123456
                AspNetUser user = new AspNetUser()
                {
                    UserName = person.NID,
                    Email = (person.FirstName[0] + person.LastName[0] + person.NID).ToUpper() + "@AEROTECH.APP",
                    NormalizedEmail = (person.FirstName[0] + person.LastName[0] + person.NID).ToUpper() + "@AEROTECH.APP",
                    NormalizedUserName = person.NID,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    PasswordHash = "ABUOcscaBcucAd+3adgqDoYDGbwJ+vsH31J7wOZWMJi8jlAjxnzNYHrkrPLEUtyNvA==",
                    SecurityStamp = "dde4f3c0-52cf-4f0a-9655-65e6e2c53a07",
                    Id = Guid.NewGuid().ToString(),

                };
                context.AspNetUsers.Add(user);
                person.UserId = user.Id;


            }

            var saveResult = await context.SaveAsync();
            if (saveResult.Code != HttpStatusCode.OK)
                return saveResult;

            if (do_user)
            {
                var text = "Dear " + person.FirstName.ToUpper() + " " + person.LastName.ToUpper() + ", " + "\n"
                         + "Your account information:" + "\n"
                         + "Username: " + person.NID + "\n"
                         + "Password: " + "123456" + "\n"
                         + "Please visit the link below to access your panel" + "\n"
                         + ConfigurationManager.AppSettings["pulsepocket_url"];
                Magfa m1 = new Magfa();
                var res = m1.enqueue(1, person.Mobile, text)[0];
                person.FaxTelNumber = res.ToString();
                await context.SaveAsync();
            }

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
                    employee.Certificates = await context.view_trn_certificate_history_last.Where(q => q.person_id == entity.Id).ToListAsync();
                    


                }

               


            }

            //soosk
            ///var employee = await unitOfWork.PersonRepository.GetEmployeeDtoByNID(nid, cid);
            return Ok(employee);
        }


        [Route("api/profile/abs/{id}")]
        public async Task<IHttpActionResult> GetProfileAbs(int id)
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
                var result = new
                {
                    Person = employee,
                };
                return Ok(result);
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

                var sms = await context.ViewCrewPickupSMS.Where(q => q.PersonId == id && q.IsVisited == 0).OrderByDescending(q => q.DateSent).Take(20).ToListAsync();
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
                    query = query.Where(q => q.JobGroupRoot == grp || q.PostRoot == grp);
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

        //war2
        [Route("api/profiles/main/new/{cid}/{active}/{grp}")]

        public async Task<IHttpActionResult> GetProfilesByCustomerIdNew(int cid, int active, string grp)
        {
            try
            {
                var context = new Models.dbEntities();
                var query = context.view_profile.Where(q => q.CustomerId == cid);
                if (active == 1)
                    query = query.Where(q => q.InActive == false);
                grp = grp.Replace('x', '/');
                if (grp != "-1")
                    query = query.Where(q => q.JobGroupRoot == grp || q.PostRoot == grp);
                var profiles = await query.ToListAsync();
                var course_types = await context.view_trn_course_type_group.Where(q => q.group_root == grp).ToListAsync();
                var certificates=await context.view_trn_certificate_history_last.Where(q=>q.group_root==grp).ToListAsync();

                var result = new List<JObject>();
                foreach (var p in profiles)
                {
                    JObject jsonObject = new JObject();
                    jsonObject["Id"] = p.Id;
                    jsonObject["PID"] = p.PID;
                    jsonObject["IsActive"] = p.IsActive;
                    jsonObject["IsDeleted"] = p.IsDeleted;
                    jsonObject["CustomerId"] = p.CustomerId;
                    jsonObject["GroupId"] = p.GroupId;
                    jsonObject["C1GroupId"] = p.C1GroupId;
                    jsonObject["JobGroupC1"] = p.JobGroupC1;
                    jsonObject["JobGroupCodeC1"] = p.JobGroupCodeC1;
                    jsonObject["C2GroupId"] = p.C2GroupId;
                    jsonObject["JobGroupC2"] = p.JobGroupC2;
                    jsonObject["JobGroupCodeC2"] = p.JobGroupCodeC2;
                    jsonObject["C3GroupId"] = p.C3GroupId;
                    jsonObject["JobGroup"] = p.JobGroup;
                    jsonObject["JobGroupCode"] = p.JobGroupCode;
                    jsonObject["JobGroupRoot"] = p.JobGroupRoot;
                    jsonObject["PostRoot"] = p.PostRoot;
                    jsonObject["NID"] = p.NID;
                    jsonObject["FirstName"] = p.FirstName;
                    jsonObject["LastName"] = p.LastName;
                    jsonObject["Name"] = p.Name;
                    jsonObject["Mobile"] = p.Mobile;
                    jsonObject["PassportNumber"] = p.PassportNumber;
                    jsonObject["IDNo"] = p.IDNo;
                    jsonObject["PersonId"] = p.PersonId;
                    jsonObject["InActive"] = p.InActive;
                    jsonObject["JobGroupMain"] = p.JobGroupMain;
                    jsonObject["JobGroupMainCode"] = p.JobGroupMainCode;
                    jsonObject["JobGroupCode2"] = p.JobGroupCode2;
                    jsonObject["JobGroup2RootCode"] = p.JobGroup2RootCode;
                    jsonObject["RemainNDT"] = p.RemainNDT;
                    jsonObject["RemainCAO"] = p.RemainCAO;
                    jsonObject["RemainPassport"] = p.RemainPassport;
                    jsonObject["date_passport"] = p.date_passport;
                    jsonObject["RemainCMC"] = p.RemainCMC;
                    jsonObject["date_cmc"] = p.date_cmc;
                    jsonObject["RemainLicence"] = p.RemainLicence;
                    jsonObject["date_licence"] = p.date_licence;
                    jsonObject["RemainLicenceIR"] = p.RemainLicenceIR;
                    jsonObject["date_licence_ir"] = p.date_licence_ir;
                    jsonObject["RemainProficiency"] = p.RemainProficiency;
                    jsonObject["date_proficiency"] = p.date_proficiency;
                    jsonObject["RemainProficiencyOPC"] = p.RemainProficiencyOPC;
                    jsonObject["date_proficiency_opc"] = p.date_proficiency_opc;
                    jsonObject["RemainLPR"] = p.RemainLPR;
                    jsonObject["date_lpr"] = p.date_lpr;
                    jsonObject["RemainMedical"] = p.RemainMedical;
                    jsonObject["date_medical"] = p.date_medical;
                    jsonObject["RemainTRI"] = p.RemainTRI;
                    jsonObject["date_tri"] = p.date_tri;
                    jsonObject["RemainTRE"] = p.RemainTRE;
                    jsonObject["date_tre"] = p.date_tre;
                    jsonObject["RemainLine"] = p.RemainLine;
                    jsonObject["date_line"] = p.date_line;
                    jsonObject["RemainEGPWS"] = p.RemainEGPWS;
                    jsonObject["date_egpws"] = p.date_egpws;
                    jsonObject["RemainTypeMD"] = p.RemainTypeMD;
                    jsonObject["date_type_md"] = p.date_type_md;
                    jsonObject["RemainType737"] = p.RemainType737;
                    jsonObject["date_type_737"] = p.date_type_737;
                    jsonObject["RemainTypeAirbus"] = p.RemainTypeAirbus;
                    jsonObject["date_type_airbus"] = p.date_type_airbus;
                    jsonObject["BaseAirline"] = p.BaseAirline;
                    jsonObject["HomeBase"] = p.HomeBase;
                    jsonObject["IsType737"] = p.IsType737;
                    jsonObject["IsTypeMD"] = p.IsTypeMD;
                    jsonObject["IsTypeAirbus"] = p.IsTypeAirbus;
                    jsonObject["IsTypeFokker"] = p.IsTypeFokker;
                    jsonObject["UserId"] = p.UserId;
                    jsonObject["RemainTypeFoker50"] = p.RemainTypeFoker50;
                    jsonObject["date_type_foker50"] = p.date_type_foker50;
                    jsonObject["RemainTypeFoker100"] = p.RemainTypeFoker100;
                    jsonObject["date_type_foker100"] = p.date_type_foker100;
                    jsonObject["Reserved1"] = p.Reserved1;
                    jsonObject["Reserved2"] = p.Reserved2;
                    jsonObject["Reserved3"] = p.Reserved3;
                    jsonObject["Reserved4"] = p.Reserved4;
                    jsonObject["Reserved5"] = p.Reserved5;
                    jsonObject["PostCode"] = p.PostCode;
                    jsonObject["PostTitle"] = p.PostTitle;
                    jsonObject["ImageUrl"] = p.ImageUrl;
                    foreach (var ct in course_types)
                    {
                        var certificate=certificates.Where(q=>q.person_id==p.PersonId && q.course_type_id==ct.course_type_id).FirstOrDefault();
                        jsonObject["remain_" + ct.title.Replace(" ", "_")] = certificate== null? -100000:certificate.remaining;
                        jsonObject["date_" + ct.title.Replace(" ", "_")] = certificate == null?(Nullable<DateTime>) null:certificate.date_expire;
                    }

                    result.Add(jsonObject);
                }



                // return Ok(result);
                return Ok(new
                {
                     result,
                     course_types
                });
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

                });





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
                    DateIssue = x.DateIssue,
                    DateExpire = x.DateExpire,



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
