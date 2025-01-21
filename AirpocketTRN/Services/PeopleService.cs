using AirpocketTRN.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AirpocketTRN.Services
{
    public interface IPeopleService
    {

    }
    public class PeopleService : IPeopleService
    {
        FLYEntities context = null;
        public PeopleService()
        {
            context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
        }

        public IQueryable<Person> GetPeople()
        {
            IQueryable<Person> query = context.Set<Person>().AsNoTracking();
            return query;
        }
        public IQueryable<ViewTeacher> GetTeachers()
        {
            IQueryable<ViewTeacher> query = context.Set<ViewTeacher>().AsNoTracking();
            return query;
        }
        public IQueryable<ViewEmployeeAb> GetViewEmployeesAbs()
        {
            IQueryable<ViewEmployeeAb> query = context.Set<ViewEmployeeAb>().AsNoTracking();
            return query;
        }
        public IQueryable<ViewEmployee> GetViewEmployees()
        {
            IQueryable<ViewEmployee> query = context.Set<ViewEmployee>().AsNoTracking();
            return query;
        }
        public IQueryable<ViewEmployeeAb> GetEmployeeByGroups(string grps)
        {
            if (string.IsNullOrEmpty(grps))
                return context.Set<ViewEmployeeAb>().AsNoTracking();
            var groups = grps.Split('-').ToList();
            IQueryable<ViewEmployeeAb> query = context.Set<ViewEmployeeAb>()
                //.Where(q=>groups.Contains(q.JobGroupCode))
                .Where(q=>q.JobGroup!="FSG")
                .AsNoTracking();
            return query;
        }

        public async Task<DataResponse> GetPeopleById(int id)
        {
            var person = await context.People.FirstOrDefaultAsync(q => q.Id == id);

            return new DataResponse()
            {
                Data = person,
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetEmployeeById(int id)
        {
            var person = await context.ViewEmployees.FirstOrDefaultAsync(q => q.Id == id);

            return new DataResponse()
            {
                Data = person,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetTeacherDocs(int id)
        {
            var docs = await context.ViewTeacherDocuments.Where(q => q.TeacherId == id).ToListAsync();

            return new DataResponse()
            {
                Data = docs,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> DeleteTeacher(int id)
        {

            var employee = await context.ViewEmployeeAbs.Where(q => q.PersonId == id).FirstOrDefaultAsync();
            if (employee != null)
            {
                var teacher = await context.Teachers.FirstOrDefaultAsync(q => q.PersonId == id);
                context.Teachers.Remove(teacher);

            }
            else
            {
                var person = await context.People.FirstOrDefaultAsync(q => q.Id == id);
                context.People.Remove(person);
            }

            var saveResult = await context.SaveAsync();
            if (!saveResult.IsSuccess)
                return saveResult;

            return new DataResponse()
            {
                IsSuccess = true,
                Data = id,
            };
        }



        public async Task<DataResponse> GetTeachersReport(teacherReportDto dto)
        {

            var query =from x in  context.ViewTeacherCourses select x;
            if (dto.Ids != null && dto.Ids.Count > 0)
                query = query.Where(q => dto.Ids.Contains(q.Id));
            if (dto.type != null)
                query = query.Where(q => q.CourseTypeId == dto.type);
            if (dto.from != null)
            {
                var f = ((DateTime)dto.from).Date;
                query = query.Where(q => q.DateStart >= f);
            }
            if (dto.to != null)
            {
                var t = ((DateTime)dto.to).Date.AddDays(1).Date;
                query = query.Where(q => q.DateEnd < t);
            }

            var result = await query.OrderBy(q => q.LastName).ThenBy(q => q.FirstName).ThenBy(q => q.CourseType).ThenBy(q => q.DateStart).ToListAsync();


            return new DataResponse()
            {
                IsSuccess = true,
                Data = result,
            };
        }



        public async Task<DataResponse> SaveTeacher(ViewModels.TeacherDto dto)
        {
            Person person = null;
            Teacher teacher = null;

            if (dto.EmployeeId != -1)
            {
                var employee = await context.ViewEmployees.FirstOrDefaultAsync(q => q.Id == dto.EmployeeId);
                var _teacher = await context.Teachers.FirstOrDefaultAsync(q => q.PersonId == employee.PersonId);
                if (_teacher != null)
                    context.Teachers.Remove(_teacher);
                teacher = new Teacher()
                {
                    PersonId = employee.PersonId,
                    Remark = dto.Remark,
                };
                context.Teachers.Add(teacher);

                var docs = await context.TeacherDocuments.Where(q => q.TeacherId == employee.PersonId).ToListAsync();
                var docids = dto.Documents.Where(q => q.Id>0).Select(q => q.Id).ToList();
                var deleted = docs.Where(q => !docids.Contains(q.Id)).ToList();
                context.TeacherDocuments.RemoveRange(deleted);

                var newdocs = dto.Documents.Where(q => q.Id <0).ToList();

                foreach (var x in newdocs)
                    teacher.TeacherDocuments.Add(new TeacherDocument()
                    {
                        FileUrl = x.FileUrl,
                        Remark = x.Remark,
                        TypeId = x.TypeId,
                    });

                await context.SaveChangesAsync();
                var tdocs = await context.ViewTeacherDocuments.Where(q => q.TeacherId == employee.PersonId).ToListAsync();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = new { obj = dto, docs = tdocs },
                };
                 
            }
            else
            {
                if (dto.Id == -1)
                {
                    person = new Person() { Id = -1 };
                    person.DateCreate = DateTime.Now;
                    context.People.Add(person);
                }

                else
                {
                    person = await context.People.FirstOrDefaultAsync(q => q.Id == dto.Id);

                }

                if (person == null)
                    return new DataResponse()
                    {
                        Data = dto,
                        IsSuccess = false,
                        Errors = new List<string>() { "entity not found" }
                    };

                //ViewModels.Location.Fill(entity, dto);
                person.FirstName = dto.FirstName;
                person.LastName = dto.LastName;
                person.NID = dto.NID;
                person.IDNo = dto.IDNo;
                person.Mobile = dto.Mobile;
                person.Email = dto.Email;
                person.SexId = 31;
                person.MarriageId = 16;
                person.Address = dto.Address;
                person.UserId = dto.UserId;
                person.ImageUrl = dto.ImageUrl;
                person.PostalCode = dto.PostalCode;

                var _teacher = await context.Teachers.FirstOrDefaultAsync(q => q.PersonId == person.Id);
                if (_teacher != null)
                    context.Teachers.Remove(_teacher);

                person.Teacher = new Teacher() { Remark = dto.Remark };


                var docs = await context.TeacherDocuments.Where(q => q.TeacherId == person.Id).ToListAsync();
                var docids = dto.Documents.Where(q => q.Id >0).Select(q => q.Id).ToList();
                var deleted = docs.Where(q => !docids.Contains(q.Id)).ToList();
                context.TeacherDocuments.RemoveRange(deleted);

                var newdocs = dto.Documents.Where(q => q.Id <0).ToList();

                foreach (var x in newdocs)
                    person.Teacher.TeacherDocuments.Add(new TeacherDocument()
                    {
                        FileUrl = x.FileUrl,
                        Remark = x.Remark,
                        TypeId = x.TypeId,
                    });




                await context.SaveChangesAsync();
                dto.Id = person.Id;
                var tdocs = await context.ViewTeacherDocuments.Where(q => q.TeacherId == person.Id).ToListAsync();
                return new DataResponse()
                {
                    IsSuccess = true,
                    Data =new { obj=dto, docs=tdocs },
                };
            }


        }



        public async Task<DataResponse> GetAllowedEmployeesForCourse(int courseid)
        {
            var query = from x in context.ViewCourseAllowedEmployees
                         
                        where x.CourseId == courseid && x.InActive==false
                        select x;
            var result = await query.OrderBy(q => q.JobGroupCode).ThenBy(q => q.LastName).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }




    }


    public class teacherReportDto
    {
        public List<int> Ids { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public int? type { get; set; }
    }



}