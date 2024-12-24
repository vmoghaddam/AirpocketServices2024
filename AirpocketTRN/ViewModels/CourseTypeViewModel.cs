using AirpocketTRN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirpocketTRN.ViewModels
{
    public class RelatedJobGroupSimple
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullCode { get; set; }
    }
    public class CourseTypeViewModel
    {
        public int Id { get; set; }
        public int? CalenderTypeId { get; set; }
        public int? CourseCategoryId { get; set; }
        public int? LicenseResultBasicId { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public int? Interval { get; set; }
        public bool? IsGeneral { get; set; }
        public bool? Status { get; set; }
        public int? Duration { get; set; }
        public int? CertificateTypeId { get; set; }
        public int? Mandatory { get; set; }
        public List<string> not_applicables
        {
            get; set;
        }

        public string Category { get; set; }
        public List<RelatedJobGroupSimple> JobGroups { get; set; }
        public static void Fill(Models.CourseType entity, ViewModels.CourseTypeViewModel coursetype)
        {
            entity.Id = coursetype.Id;
            entity.CalenderTypeId = coursetype.CalenderTypeId;
            entity.CourseCategoryId = coursetype.CourseCategoryId;
            entity.LicenseResultBasicId = coursetype.LicenseResultBasicId;
            entity.Title = coursetype.Title;
            entity.Remark = coursetype.Remark;
            entity.Interval = coursetype.Interval;
            entity.IsGeneral = coursetype.IsGeneral;
            entity.Status = coursetype.Status;
            entity.Duration = coursetype.Duration;
            entity.CertificateTypeId = coursetype.CertificateTypeId;
            entity.Mandatory = coursetype.Mandatory;

        }
        public static void FillDto(Models.CourseType entity, ViewModels.CourseTypeViewModel coursetype)
        {
            coursetype.Id = entity.Id;
            coursetype.CalenderTypeId = entity.CalenderTypeId;
            coursetype.CourseCategoryId = entity.CourseCategoryId;
            coursetype.LicenseResultBasicId = entity.LicenseResultBasicId;
            coursetype.Title = entity.Title;
            coursetype.Remark = entity.Remark;
            coursetype.Interval = entity.Interval;
            coursetype.IsGeneral = entity.IsGeneral;
            coursetype.Status = entity.Status;
            coursetype.Duration = entity.Duration;
            coursetype.CertificateTypeId = entity.CertificateTypeId;
            coursetype.Mandatory = entity.Mandatory;
        }



    }

    public class ExamViewModel
    {
        public int id { get; set; }
        public int course_id { get; set; }
        public Nullable<System.DateTime> exam_date { get; set; }
        public string exam_date_persian { get; set; }
        public string location_title { get; set; }
        public string location_address { get; set; }
        public string location_phone { get; set; }
        public string remark { get; set; }
        public Nullable<int> status_id { get; set; }
        public Nullable<int> created_by { get; set; }
        public Nullable<int> confirmed_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> confirmed_date { get; set; }
        public Nullable<int> exam_type_id { get; set; }
        public Nullable<System.DateTime> signed_by_ins1_date { get; set; }
        public Nullable<System.DateTime> signed_by_ins2_date { get; set; }
        public Nullable<System.DateTime> signed_by_director_date { get; set; }
        public Nullable<System.DateTime> signed_by_staff_date { get; set; }
        public Nullable<int> duration { get; set; }
        public Nullable<System.DateTime> date_start { get; set; }
        public Nullable<System.DateTime> date_end_scheduled { get; set; }
        public Nullable<System.DateTime> date_end_actual { get; set; }
        public Nullable<System.DateTime> date_start_scheduled { get; set; }

        public List<int?> groups { get; set; }
        public List<int> people { get; set; }
        public List<view_trn_exam_question_template> template { get; set; }
        public List<view_trn_exam_question> questions { get; set; }
        public List<view_trn_exam_summary_details> summary { get; set; }
    }
    public class exam_template
    {
        public int template_id { get; set; }
        public int category_id { get; set; }
        public string category { get; set; }
        public int? count { get; set; }
    }
    public class CourseViewModel
    {
        public int Id { get; set; }
        public int CourseTypeId { get; set; }
        public string DateStart { get; set; }

        public string DateEnd { get; set; }

        public string Instructor { get; set; }
        public string Location { get; set; }

        public int? OrganizationId { get; set; }
        public int? Duration { get; set; }
        public int? DurationUnitId { get; set; }


        public string Remark { get; set; }

        public string TrainingDirector { get; set; }
        public string Title { get; set; }

        public bool? Recurrent { get; set; }
        public int? Interval { get; set; }
        public int? CalanderTypeId { get; set; }


        public bool? IsGeneral { get; set; }
        public int? CustomerId { get; set; }
        public string No { get; set; }
        public bool? IsNotificationEnabled { get; set; }

        public List<string> Sessions { get; set; }

        public int? StatusId { get; set; }
        public int? CurrencyId { get; set; }

        public int? Instructor2Id { get; set; }
        public string HoldingType { get; set; }

        public int? Cost { get; set; }
        public int? session_changed { get; set; }

        //pasco
        public bool? InForm { get; set; }
        public bool? SendLetter { get; set; }

        public bool? Financial { get; set; }

        public bool? Certificate { get; set; }
        public List<CourseDoc> Documents { get; set; }
        public List<SyllabusDto> Syllabi { get; set; }

        public List<ExamViewModel> exams { get; set; }

    }
    public class SyllabusDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public int TypeId { get; set; }
        public int InstructorId { get; set; }

    }
    public class CourseDoc
    {
        public int Id { get; set; }
        public int? TypeId { get; set; }
        public string Remark { get; set; }
        public string FileUrl { get; set; }
        public string Type { get; set; }

    }
    public class CertificateViewModel
    {
        public int Id { get; set; }
        public int CourseTypeId { get; set; }
        public string DateStart { get; set; }

        public string DateEnd { get; set; }

        public string Instructor { get; set; }
        public string Location { get; set; }

        public int? OrganizationId { get; set; }
        public int? Duration { get; set; }
        public int? DurationUnitId { get; set; }


        public string Remark { get; set; }

        public string TrainingDirector { get; set; }
        public string Title { get; set; }

        public bool? Recurrent { get; set; }
        public int? Interval { get; set; }
        public int? CalanderTypeId { get; set; }


        public bool? IsGeneral { get; set; }
        public int? CustomerId { get; set; }
        public string No { get; set; }
        public bool? IsNotificationEnabled { get; set; }

        public int? PersonId { get; set; }
        public string CertificateNo { get; set; }
        public string DateIssue { get; set; }
        public string DateExpire { get; set; }


        public List<string> Sessions { get; set; }

        public int? StatusId { get; set; }
    }

    public class CoursePeopleStatusViewModel
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? PersonId { get; set; }
        public int? StatusId { get; set; }
        public string Remark { get; set; }
        public string Issue { get; set; }

        public string Expire { get; set; }
        public string No { get; set; }
        public string Group { get; set; }
    }

    public class TeacherDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NID { get; set; }
        public string IDNo { get; set; }
        public string Mobile { get; set; }
        public int SexId { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public int EmployeeId { get; set; }
        public string Remark { get; set; }
        public string PostalCode { get; set; }
        public List<TeacherDoc> Documents { get; set; }

    }

    public class TeacherDoc
    {
        public int Id { get; set; }
        public int? TypeId { get; set; }
        public string Remark { get; set; }
        public string FileUrl { get; set; }
        public string Type { get; set; }

    }

    public class Attendance
    {
        public int CourseId { get; set; }
        public int PersonId { get; set; }
        public string Remark { get; set; }
        public string Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Key { get; set; }
    }


}