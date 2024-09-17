using AirpocketTRN.Models;
using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management.Instrumentation;
using System.Net.Http.Formatting;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using static AirpocketTRN.Services.TrainingService;

namespace AirpocketTRN.Services
{
    public interface ITrainingService
    {

    }

    public class TrainingService : ITrainingService
    {
        FLYEntities context = null;

        public TrainingService()
        {
            context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
        }


        public class crm_assessment_form
        {
            public int id { get; set; }
            public string instructor_name { get; set; }
            public string assessor_name { get; set; }
            public string department { get; set; }
            public int? class_format { get; set; }
            public DateTime? instructor_sign_date { get; set; }
            public DateTime? assessor_sign_data { get; set; }
            public DateTime? training_manager_sign_date { get; set; }
            public int? flight_id { get; set; }
            public int? instructor_id { get; set; }
            public int? assessor_id { get; set; }
            public int? training_manager_id { get; set; }
            public int? fdp_id { get; set; }
            public DateTime? assessment_date { get; set; }
            public int? training_manager_user_id { get; set; }
            public DateTime? ops_training_manager_sign_date { get; set; }
            public int? ops_training_manager_user_id { get; set; }
            public int? instructor_user_id { get; set; }
            public DateTime? student_sign_date { get; set; }
            public int? student_user_id { get; set; }
            public int? responsible_id1 { get; set; }
            public DateTime? responsible_sign_date1 { get; set; }
            public int? responsible_id2 { get; set; }
            public DateTime? responsible_sign_date2 { get; set; }

            public List<view_trn_crm_assessment> questions { get; set; }
        }

        public async Task<DataResponse> get_trn_crm_assessment(int flightId)
        {
            var result = new crm_assessment_form();

            try
            {
                var form = await context.trn_crm_assessment.FirstOrDefaultAsync(q => q.flight_id == flightId);
                result.questions = context.view_trn_crm_assessment.Where(q => q.form_id == form.id).ToList();

                result.instructor_name = form.instructor_name;
                result.assessor_name = form.assessor_name;
                result.department = form.department;
                result.class_format = form.class_format;
                result.instructor_sign_date = form.instructor_sign_date;
                result.assessor_sign_data = form.assessor_sign_data;
                result.training_manager_sign_date = form.training_manager_sign_date;
                result.flight_id = form.flight_id;
                result.instructor_id = form.instructor_id;
                result.assessor_id = form.assessor_id;
                result.training_manager_id = form.training_manager_id;
                result.assessment_date = form.assessment_date;
                result.fdp_id = form.fdp_id;

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }



        public async Task<DataResponse> save_trn_crm_assessment(dynamic dto)
        {
            try
            {
                int flightId = dto.flight_id;


                var form = await context.trn_crm_assessment
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_crm_assessment
                    {
                        flight_id = flightId
                    };
                    context.trn_crm_assessment.Add(form);
                }


                form.assessor_id = dto.assessor_id;
                form.assessor_name = dto.assessor_name;
                form.instructor_name = dto.instructor_name;
                form.instructor_id = dto.instructor_id;
                form.class_format = dto.class_format;
                form.department = dto.department;
                form.assessment_date = dto.assessment_date;


                foreach (var question in dto.questions)
                {
                    int questionId = question.question_id;

                    var existingQuestionValue = await context.trn_crm_assessment_questions_value
                        .FirstOrDefaultAsync(q => q.crm_assessment_question_id == questionId && q.crm_assessment_id == form.id);

                    if (existingQuestionValue != null)
                    {

                        existingQuestionValue.answer = question.answer;
                        existingQuestionValue.comments = question.comments;

                        context.Entry(existingQuestionValue).State = EntityState.Modified;
                    }
                    else
                    {
                        var newQuestionValue = new trn_crm_assessment_questions_value
                        {
                            crm_assessment_question_id = question.question_id,
                            answer = question.answer,
                            comments = question.comments,
                            crm_assessment_id = form.id
                        };

                        context.trn_crm_assessment_questions_value.Add(newQuestionValue);
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving CRM assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }

        public class efb_assessment_form
        {
            public int? flight_id { get; set; }
            public int? fdp_id { get; set; }
            public DateTime? date { get; set; }
            public string crew_name { get; set; }
            public string crew_grad { get; set; }
            public string check_type { get; set; }
            public string final_assessment { get; set; }
            public string special_comments { get; set; }
            public string crew_sign { get; set; }
            public string instructor_sign { get; set; }
            public List<efb_section> sections { get; set; }
        }

        public class efb_action
        {
            public int action_id { get; set; }
            public string description { get; set; }
            public int? assessment { get; set; }
        }
        public class efb_section
        {
            public int section_id { get; set; }
            public string section_name { get; set; }
            public List<efb_action> actions { get; set; }
        }

        public async Task<DataResponse> get_trn_efb_assessment(int flightId)
        {
            var result = new efb_assessment_form();

            try
            {
                var form = await context.trn_efb_assessment.FirstOrDefaultAsync(q => q.flight_id == flightId);
                result.sections = context.view_trn_efb_assessment
                    .Where(q => q.form_id == form.id)
                    .GroupBy(v => new { v.section_id, v.section_name })
                    .Select(g => new efb_section
                    {
                        section_id = g.Key.section_id,
                        section_name = g.Key.section_name,
                        actions = g.Select(a => new efb_action
                        {
                            action_id = a.action_id,
                            description = a.description,
                            assessment = a.assessment
                        }).ToList()
                    }).ToList();

                result.date = form.date;
                result.crew_name = form.crew_name;
                result.crew_grad = form.crew_grad;
                result.final_assessment = form.final_assessment;
                result.special_comments = form.special_comments;
                result.check_type = form.check_type;
                result.flight_id = form.flight_id;
                result.fdp_id = form.fdp_id;

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }


        public async Task<DataResponse> save_trn_efb_assessment(dynamic dto)
        {
            try
            {
                int flightId = dto.flight_id;

                var form = await context.trn_efb_assessment
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_efb_assessment
                    {
                        flight_id = flightId
                    };
                    context.trn_efb_assessment.Add(form);
                }

                form.date = dto.date;
                form.final_assessment = dto.final_assessment;
                form.special_comments = dto.special_comments;
                form.flight_id = dto.flight_id;
                form.check_type = dto.check_type;
                form.crew_grad = dto.crew_grad;
                form.crew_name = dto.crew_name;

                foreach (var section in dto.sections)
                {
                    foreach (var action in section.actions)
                    {
                        int actionId = action.action_id;

                        var existingActionValue = await context.trn_efb_action_values
                            .FirstOrDefaultAsync(q => q.action_id == actionId && q.efb_form_id == form.id);

                        if (existingActionValue != null)
                        {
                            existingActionValue.assessment = action.assessment;
                            context.Entry(existingActionValue).State = EntityState.Modified;
                        }
                        else
                        {
                            var newActionValue = new trn_efb_action_values
                            {
                                action_id = actionId,
                                assessment = action.assessment,
                                efb_form_id = form.id
                            };

                            context.trn_efb_action_values.Add(newActionValue);
                        }
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving EFB assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }





        public class fstd_crm_form
        {
            public DateTime? date { get; set; }
            public string crew_name { get; set; }
            public string check_type { get; set; }
            public string crew_grad { get; set; }
            public int? final_assessment { get; set; }
            public string special_comments { get; set; }
            public string crew_sign { get; set; }
            public string instructor_sign { get; set; }
            public int? flight_id { get; set; }
            public int? fdp_id { get; set; }
            public DateTime? training_manager_sign_date { get; set; }
            public int? training_manager_user_id { get; set; }
            public DateTime? ops_training_manager_sign_date { get; set; }
            public int? ops_training_manager_user_id { get; set; }
            public DateTime? instructor_sign_date { get; set; }
            public int? instructor_user_id { get; set; }
            public DateTime? student_sign_date { get; set; }
            public int? student_user_id { get; set; }
            public int? responsible_id1 { get; set; }
            public DateTime? responsible_sign_date1 { get; set; }
            public int? responsible_id2 { get; set; }
            public DateTime? responsible_sign_date2 { get; set; }
            public List<fstd_section> sections { get; set; }
        }

        public class fstd_action
        {
            public int action_id { get; set; }
            public string description { get; set; }
            public int? assessment { get; set; }
        }
        public class fstd_section
        {
            public int section_id { get; set; }
            public string section_name { get; set; }
            public List<fstd_action> actions { get; set; }
        }

        public async Task<DataResponse> get_trn_fstd_crm(int flightId)
        {
            var result = new fstd_crm_form();

            try
            {
                var form = await context.trn_fstd_crm.FirstOrDefaultAsync(q => q.flight_id == flightId);
                result.sections = context.view_trn_fstd_crm
                    .Where(q => q.form_id == form.id)
    .GroupBy(v => new { v.section_id, v.section_name })
    .Select(g => new fstd_section
    {
        section_id = g.Key.section_id,
        section_name = g.Key.section_name,
        actions = g.Select(a => new fstd_action
        {
            action_id = a.action_id,
            description = a.description,
            assessment = a.assessment
        }).ToList()
    })
    .ToList();
                ;

                result.date = form.date;
                result.crew_name = form.crew_name;
                result.check_type = form.check_type;
                result.crew_grad = form.crew_grad;
                result.final_assessment = form.final_assessment;
                result.special_comments = form.special_comments;
                result.flight_id = form.flight_id;
                result.fdp_id = form.fdp_id;

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }

        public async Task<DataResponse> save_trn_fstd_crm(dynamic dto)
        {

            try
            {
                int flightId = dto.flight_id;

                var form = await context.trn_fstd_crm
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_fstd_crm
                    {
                        flight_id = flightId
                    };
                    context.trn_fstd_crm.Add(form);
                }

                form.date = dto.date;
                form.final_assessment = dto.final_assessment;
                form.special_comments = dto.special_comments;
                form.flight_id = dto.flight_id;
                form.check_type = dto.check_type;
                form.crew_grad = dto.crew_grad;
                form.crew_name = dto.crew_name;

                foreach (var section in dto.sections)
                {
                    foreach (var action in section.actions)
                    {
                        int actionId = action.action_id;

                        var existingActionValue = await context.trn_fstd_crm_action_value
                            .FirstOrDefaultAsync(q => q.action_id == actionId && q.form_id == form.id);

                        if (existingActionValue != null)
                        {
                            existingActionValue.assessment = action.assessment;
                            context.Entry(existingActionValue).State = EntityState.Modified;
                        }
                        else
                        {
                            var newActionValue = new trn_fstd_crm_action_value
                            {
                                action_id = actionId,
                                assessment = action.assessment,
                                form_id = form.id
                            };

                            context.trn_fstd_crm_action_value.Add(newActionValue);
                        }
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving EFB assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }

        public class grt_form
        {
            public int id { get; set; }
            public string instructor_name { get; set; }
            public DateTime? assessment_date { get; set; }
            public string applicant_name { get; set; }
            public string department { get; set; }
            public string course_title { get; set; }
            public int? class_format { get; set; }
            public string remark { get; set; }
            public string instructor_sign { get; set; }
            public string manager_sign { get; set; }
            public int? flight_id { get; set; }
            public int? fdp_id { get; set; }
            public List<view_trn_grt> items { get; set; }
        }

        public async Task<DataResponse> get_trn_grt(int flightId)
        {
            var result = new grt_form();

            try
            {
                var form = await context.trn_grt.FirstOrDefaultAsync(q => q.flight_id == flightId);
                result.items = context.view_trn_grt.Where(q => q.form_id == form.id).ToList();

                result.applicant_name = form.applicant_name;
                result.department = form.department;
                result.assessment_date = form.assessment_date;
                result.course_title = form.course_title;
                result.class_format = form.class_format;
                result.remark = form.remark;
                result.flight_id = flightId;
                result.instructor_name = form.instructor_name;


                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }

        public async Task<DataResponse> save_trn_grt(dynamic dto)
        {
            try
            {
                int flightId = dto.flight_id;


                var form = await context.trn_grt
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_grt
                    {
                        flight_id = flightId
                    };
                    context.trn_grt.Add(form);
                }


                form.instructor_name = dto.instructor_name;
                form.class_format = dto.class_format;
                form.department = dto.department;
                form.assessment_date = dto.assessment_date;
                form.course_title = dto.course_title;
                form.applicant_name = dto.applicant_name;
                form.remark = dto.remark;


                foreach (var item in dto.items)
                {
                    int itemId = item.item_id;

                    var existingQuestionValue = await context.trn_grt_item_values
                        .FirstOrDefaultAsync(q => q.item_id == itemId && q.form_id == form.id);

                    if (existingQuestionValue != null)
                    {
                        existingQuestionValue.grade = item.item_grade;   
                        context.Entry(existingQuestionValue).State = EntityState.Modified;
                    }
                    else
                    {
                        var newQuestionValue = new trn_grt_item_values
                        {
                            item_id = item.item_id,
                            grade = item.item_grade,
                            form_id = item.form_id,
                        };

                        context.trn_grt_item_values.Add(newQuestionValue);
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving CRM assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }


        public class instructor_form
        {
            public int? flight_id { get; set; }
            public int? fdp_id { get; set; }
            public DateTime? date { get; set; }
            public string instructor_name { get; set; }
            public string assessor_name { get; set; }
            public string department { get; set; }
            public string course_title { get; set; }
            public int? class_format { get; set; }
            public string final_assessment { get; set; }
            public string comments { get; set; }
            public string assessor_sign { get; set; }
            public string head_sign { get; set; }
            public DateTime? training_manager_sign_date { get; set; }
            public int? training_manager_user_id { get; set; }
            public DateTime? ops_training_manager_sign_date { get; set; }
            public int? ops_training_manager_user_id { get; set; }
            public DateTime? instructor_sign_date { get; set; }
            public int? instructor_user_id { get; set; }
            public DateTime? student_sign_date { get; set; }
            public int? student_user_id { get; set; }
            public int? responsible_id1 { get; set; }
            public DateTime? responsible_sign_date1 { get; set; }
            public int? responsible_id2 { get; set; }
            public DateTime? responsible_sign_date2 { get; set; }
            public List<instructor_section> sections { get; set; }
        }

        public class instructor_action
        {
            public int action_id { get; set; }
            public string description { get; set; }
            public int? rating { get; set; }
        }
        public class instructor_section
        {
            public int section_id { get; set; }
            public string section_name { get; set; }
            public List<instructor_action> actions { get; set; }
        }

        public async Task<DataResponse> get_trn_instructor(int flightId)
        {
            var result = new instructor_form();

            try
            {
                var form = await context.trn_instructor.FirstOrDefaultAsync(q => q.flight_id == flightId);
                result.sections = context.view_trn_instructor
                    .Where(q => q.form_id == form.id)
    .GroupBy(v => new { v.section_id, v.section_name })
    .Select(g => new instructor_section
    {
        section_id = g.Key.section_id,
        section_name = g.Key.section_name,
        actions = g.Select(a => new instructor_action
        {
            action_id = a.action_id,
            description = a.description,
            rating = a.rating
        }).ToList()
    })
    .ToList();


                result.date = form.date;
                result.final_assessment = form.final_assessment;
                result.comments = form.comments;
                result.instructor_name = form.instructor_name;
                result.assessor_name = form.assessor_name;
                result.department = form.department;
                result.course_title = form.course_title;
                result.class_format = Int32.Parse(form.class_format);
                result.flight_id = form.flight_id;


                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }

        public async Task<DataResponse> save_trn_instructor(dynamic dto)
        {

            try
            {
                int flightId = dto.flight_id;

                var form = await context.trn_instructor
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_instructor
                    {
                        flight_id = flightId
                    };
                    context.trn_instructor.Add(form);
                }

                form.date = dto.date;
                form.final_assessment = dto.final_assessment;
                form.comments = dto.comments;
                form.instructor_name = dto.instructor_name;
                form.assessor_name = dto.assessor_name;
                form.department = dto.department;
                form.course_title = dto.course_title;
                form.class_format = dto.class_format;
                form.flight_id = dto.flight_id;
                
                foreach (var section in dto.sections)
                {
                    foreach (var action in section.actions)
                    {
                        int actionId = action.action_id;

                        var existingActionValue = await context.trn_instructor_actions_values
                            .FirstOrDefaultAsync(q => q.action_id == actionId && q.form_id == form.id);

                        if (existingActionValue != null)
                        {
                            existingActionValue.rating = action.rating;
                            context.Entry(existingActionValue).State = EntityState.Modified;
                        }
                        else
                        {
                            var newActionValue = new trn_instructor_actions_values
                            {
                                action_id = actionId,
                                rating = action.rating,
                                form_id = form.id
                            };

                            context.trn_instructor_actions_values.Add(newActionValue);
                        }
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving EFB assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }


        public class line_chck_form
        {
            public DateTime? date { get; set; }
            public string name { get; set; }
            public string code_no { get; set; }
            public string location { get; set; }
            public string ac_type { get; set; }
            public string ac_registration { get; set; }
            public string comments { get; set; }
            public int? flight_id { get; set; }
            public int? fdp_id { get; set; }
            public DateTime? training_manager_sign_date { get; set; }
            public int? training_manager_user_id { get; set; }
            public DateTime? ops_training_manager_sign_date { get; set; }
            public int? ops_training_manager_user_id { get; set; }
            public DateTime? instructor_sign_date { get; set; }
            public int? instructor_user_id { get; set; }
            public DateTime? student_sign_date { get; set; }
            public int? student_user_id { get; set; }
            public int? responsible_id1 { get; set; }
            public DateTime? responsible_sign_date1 { get; set; }
            public int? responsible_id2 { get; set; }
            public DateTime? responsible_sign_date2 { get; set; }
            public List<line_check_section> sections { get; set; }
        }

        public class line_check_action
        {
            public int action_id { get; set; }
            public string description { get; set; }
            public int? rating { get; set; }
        }
        public class line_check_section
        {
            public int section_id { get; set; }
            public string section_name { get; set; }
            public int? section_type { get; set; }
            public List<line_check_action> actions { get; set; }
        }

        public async Task<DataResponse> get_trn_line_check(int flightId)
        {
            var result = new line_chck_form();

            try
            {
                var form = await context.trn_line_check.FirstOrDefaultAsync(q => q.flight_id == flightId);

                result.sections = context.view_trn_line_check.Where(q => q.form_id == form.id)
                    .GroupBy(v => new { v.section_id, v.section_name, v.section_type })
                    .Select(g => new line_check_section
                    {
                        section_id = g.Key.section_id,
                        section_name = g.Key.section_name,
                        section_type = g.Key.section_type,
                        actions = g.Select(a => new line_check_action
                        {
                            action_id = a.action_id,
                            description = a.description,
                            rating = a.rating
                        }).ToList()
                    }).ToList();



                result.date = form.date;
                result.comments = form.comments;
                result.name = form.name;
                result.code_no = form.code_no;
                result.location = form.location;
                result.ac_type = form.ac_type;
                result.ac_registration = form.ac_registration;
                result.flight_id = form.flight_id;

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }

        public async Task<DataResponse> save_trn_line_check(dynamic dto)
        {

            try
            {
                int flightId = dto.flight_id;

                var form = await context.trn_line_check
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_line_check
                    {
                        flight_id = flightId
                    };
                    context.trn_line_check.Add(form);
                }

                form.date = dto.date;
                form.comments = dto.comments;
                form.name = dto.name;
                form.code_no = dto.code_no;
                form.location = dto.location;
                form.ac_type = dto.ac_type;
                form.ac_registration = dto.ac_registration;
                form.flight_id = dto.flight_id;

                foreach (var section in dto.sections)
                {
                    foreach (var action in section.actions)
                    {
                        int actionId = action.action_id;

                        var existingActionValue = await context.trn_line_check_action_values
                            .FirstOrDefaultAsync(q => q.action_id == actionId && q.form_id == form.id);

                        if (existingActionValue != null)
                        {
                            existingActionValue.rating = action.rating;
                            context.Entry(existingActionValue).State = EntityState.Modified;
                        }
                        else
                        {
                            var newActionValue = new trn_line_check_action_values
                            {
                                action_id = actionId,
                                rating = action.rating,
                                form_id = form.id
                            };

                            context.trn_line_check_action_values.Add(newActionValue);
                        }
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving EFB assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }

            //try
            //{
            //    int flight_id = dto.flight_id;
            //    var form = context.trn_line_check.FirstOrDefault(q => q.flight_id == flight_id);
            //    if (form == null)
            //    {
            //        form = new trn_line_check();
            //        context.trn_line_check.Add(form);
            //    }

            //    form.flight_id = flight_id;
            //    form.date = dto.date;
            //    form.flight_id = dto.flight_id;
            //    foreach (var section in dto.sections)
            //    {
            //        foreach (var action in section.actions)
            //        {
            //            var action_value = new trn_line_check_action_values
            //            {
            //                rating = action.rating,
            //                action_id = action.action_id,
            //            };

            //            form.trn_line_check_action_values.Add(action_value);
            //        }
            //    }

            //    context.SaveChanges();
            //    return new DataResponse()
            //    {
            //        IsSuccess = true,
            //        Data = form.id
            //    };
            //}
            //catch (Exception ex)
            //{
            //    var msg = ex.Message;
            //    if (ex.InnerException != null)
            //        msg += "   " + ex.InnerException.Message;
            //    return new DataResponse()
            //    {
            //        IsSuccess = false,
            //        Data = msg,
            //    };

            //}
        }


        public class line_crm_form
        {
            public int id { get; set; }
            public DateTime? date { get; set; }
            public string crew_members_name { get; set; }
            public string type_of_check { get; set; }
            public string final_assessment { get; set; }
            public string comments { get; set; }
            public string crew_member_sign { get; set; }
            public string instructor_sign { get; set; }
            public int? flight_id { get; set; }
            public int? fdp_id { get; set; }
            public List<line_crm_section> sections { get; set; }
        }

        public class line_crm_action
        {
            public int action_id { get; set; }
            public string description { get; set; }
            public int? assessment { get; set; }
        }
        public class line_crm_section
        {
            public int section_id { get; set; }
            public string section_name { get; set; }
            public List<line_crm_action> actions { get; set; }
        }

        public async Task<DataResponse> get_trn_line_crm(int flightId)
        {
            var result = new line_crm_form();

            try
            {
                var form = await context.trn_line_crm.FirstOrDefaultAsync(q => q.flight_id == flightId);
                result.sections = context.view_trn_line_crm
                    .Where(q => q.form_id == form.id)
    .GroupBy(v => new { v.section_id, v.section_name })
    .Select(g => new line_crm_section
    {
        section_id = g.Key.section_id,
        section_name = g.Key.section_name,
        actions = g.Select(a => new line_crm_action
        {
            action_id = a.action_id,
            description = a.description,
            assessment = a.assessment
        }).ToList()
    })
    .ToList();
                ;

                result.date = form.date;
                result.final_assessment = form.final_assessment;
                result.crew_members_name = form.crew_members_name;
                result.type_of_check = form.type_of_check;
                result.comments = form.comments;
                result.flight_id = form.flight_id;

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }

        public async Task<DataResponse> save_trn_line_crm(dynamic dto)
        {

            try
            {
                int flightId = dto.flight_id;

                var form = await context.trn_line_crm
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_line_crm
                    {
                        flight_id = flightId
                    };
                    context.trn_line_crm.Add(form);
                }

                form.date = dto.date;
                form.final_assessment = dto.final_assessment;
                form.crew_members_name = dto.crew_members_name;
                form.type_of_check = dto.type_of_check;
                form.comments = dto.comments;
                form.flight_id = dto.flight_id;

                foreach (var section in dto.sections)
                {
                    foreach (var action in section.actions)
                    {
                        int actionId = action.action_id;

                        var existingActionValue = await context.trn_line_crm_actions_values
                            .FirstOrDefaultAsync(q => q.action_id == actionId && q.form_id == form.id);

                        if (existingActionValue != null)
                        {
                            existingActionValue.assessment = action.assessment;
                            context.Entry(existingActionValue).State = EntityState.Modified;
                        }
                        else
                        {
                            var newActionValue = new trn_line_crm_actions_values
                            {
                                action_id = actionId,
                                assessment = action.assessment,
                                form_id = form.id
                            };

                            context.trn_line_crm_actions_values.Add(newActionValue);
                        }
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving EFB assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }


        public class zftt_form
        {
            public int id { get; set; }
            public DateTime? date { get; set; }
            public string crew_members_name { get; set; }
            public string type_of_check { get; set; }
            public string final_assessment { get; set; }
            public string comments { get; set; }
            public string crew_member_sign { get; set; }
            public string instructor_sign { get; set; }
            public int? flight_id { get; set; }
            public int? fdp_id { get; set; }

            public List<zftt_section> sections { get; set; }
        }

        public class zftt_action
        {
            public int action_id { get; set; }
            public string description { get; set; }
            public int? assessment { get; set; }
        }
        public class zftt_section
        {
            public int section_id { get; set; }
            public string section_name { get; set; }
            public List<zftt_action> actions { get; set; }
        }

        public async Task<DataResponse> get_trn_zftt(int flightId)
        {
            var result = new zftt_form();

            try
            {
                var form = await context.trn_zftt.FirstOrDefaultAsync(q => q.flight_id == flightId);
                result.sections = context.view_trn_zftt
                    .Where(q => q.form_id == form.id)
    .GroupBy(v => new { v.section_id, v.section_name })
    .Select(g => new zftt_section
    {
        section_id = g.Key.section_id,
        section_name = g.Key.section_name,
        actions = g.Select(a => new zftt_action
        {
            action_id = a.action_id,
            description = a.description,
            assessment = a.assessment
        }).ToList()
    })
    .ToList();
                ;

                result.date = form.date;
                result.final_assessment = form.final_assessment;
                result.crew_members_name = form.crew_members_name;
                result.type_of_check = form.type_of_check;
                result.comments = form.comments;
                result.flight_id = form.flight_id;

                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }

        public async Task<DataResponse> save_trn_zftt(dynamic dto)
        {

            try
            {
                int flightId = dto.flight_id;

                var form = await context.trn_zftt
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_zftt
                    {
                        flight_id = flightId
                    };
                    context.trn_zftt.Add(form);
                }

                form.date = dto.date;
                form.final_assessment = dto.final_assessment;
                form.crew_members_name = dto.crew_members_name;
                form.type_of_check = dto.type_of_check;
                form.comments = dto.comments;
                form.flight_id = dto.flight_id;

                foreach (var section in dto.sections)
                {
                    foreach (var action in section.actions)
                    {
                        int actionId = action.action_id;

                        var existingActionValue = await context.trn_zftt_action_values
                            .FirstOrDefaultAsync(q => q.action_id == actionId && q.form_id == form.id);

                        if (existingActionValue != null)
                        {
                            existingActionValue.assessment = action.assessment;
                            context.Entry(existingActionValue).State = EntityState.Modified;
                        }
                        else
                        {
                            var newActionValue = new trn_zftt_action_values
                            {
                                action_id = actionId,
                                assessment = action.assessment,
                                form_id = form.id
                            };

                            context.trn_zftt_action_values.Add(newActionValue);
                        }
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving EFB assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }

        }


        public class fstd_form
        {
            public string fstd_approval { get; set; }
            public string location { get; set; }
            public string fstd_operator { get; set; }
            public string fstd_type { get; set; }
            public string aircraft_type { get; set; }
            public string approval_level { get; set; }
            public DateTime? qualification_valid_until { get; set; }
            public string restrictions { get; set; }
            public string other_remarks { get; set; }
            public string low_visibility_training_remarks { get; set; }
            public string recurrent_training_checking_remarks { get; set; }
            public string operation_from_either_pilots_seat_remarks { get; set; }
            public string difference_training_remarks { get; set; }
            public string recency_of_experience_remarks { get; set; }
            public string zero_flighttime_training_remarks { get; set; }
            public string other_training_remarks { get; set; }
            public string sign { get; set; }
            public int? flight_id { get; set; }
            public int? fdp_id { get; set; }
            public List<difference_dto> differences { get; set; }
        }


        public class difference_dto
        {
            public int form_id { get; set; }
            public int system_id { get; set; }
            public string name { get; set; }
            public string differences { get; set; }
            public string syllabus_ref { get; set; }
            public int? compliance_level { get; set; }
            public bool? fchar { get; set; }
            public bool? proc { get; set; }
        }

        public async Task<DataResponse> get_trn_fstd(int flightId)
        {
            var result = new fstd_form();

            try


            {
                var form = await context.trn_fstd.FirstOrDefaultAsync(q => q.flight_id == flightId);
                if (form == null)
                {
                    return new DataResponse()
                    {
                        IsSuccess = false,
                        Data = "Form not found"
                    };
                }

                // Fetch differences ensuring distinct objects are created
                result.differences = context.view_trn_fstd_monitoring
                    .Where(q => q.form_id == form.id)
                    .Select(q => new difference_dto // Assuming you have a DTO or a similar class to hold the difference data
                    {
                        name = q.name,
                        form_id = q.form_id,
                        system_id = q.system_id,
                        differences = q.differences,
                        syllabus_ref = q.syllabus_ref,
                        compliance_level = q.compliance_level,
                        fchar = q.fchar,
                        proc = q.proc
                    })
                    .ToList();

                result.flight_id = form.flight_id;
                result.fdp_id = form.fdp_id;
                result.fstd_approval = form.fstd_approval;
                result.location = form.location;
                result.fstd_operator = form.fstd_operator;
                result.fstd_type = form.fstd_type;
                result.approval_level = form.approval_level;
                result.aircraft_type = form.aircraft_type;
                result.qualification_valid_until = form.qualification_valid_until;
                result.restrictions = form.restrictions;
                result.low_visibility_training_remarks = form.low_visibility_training_remarks;
                result.recurrent_training_checking_remarks = form.recurrent_training_checking_remarks;
                result.operation_from_either_pilots_seat_remarks = form.operation_from_either_pilots_seat_remarks;
                result.difference_training_remarks = form.difference_training_remarks;
                result.recency_of_experience_remarks = form.recency_of_experience_remarks;
                result.zero_flighttime_training_remarks = form.zero_flighttime_training_remarks;
                result.other_remarks = form.other_remarks;


                return new DataResponse()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "   " + ex.InnerException.Message;
                return new DataResponse()
                {
                    IsSuccess = false,
                    Data = msg,
                };

            }
        }



        public async Task<DataResponse> save_trn_fstd(dynamic dto)
        {

            try
            {
                int flightId = dto.flight_id;

                var form = await context.trn_fstd
                    .FirstOrDefaultAsync(q => q.flight_id == flightId);

                if (form == null)
                {
                    form = new trn_fstd
                    {
                        flight_id = flightId
                    };
                    context.trn_fstd.Add(form);
                }

                form.flight_id = dto.flight_id;
                form.fstd_approval = dto.fstd_approval;
                form.location = dto.location;
                form.fstd_operator = dto.fstd_operator;
                form.fstd_type = dto.fstd_type;
                form.approval_level = dto.approval_level;
                form.aircraft_type = dto.aircraft_type;
                form.qualification_valid_until = dto.qualification_valid_until;
                form.restrictions = dto.restrictions;
                form.low_visibility_training_remarks = dto.low_visibility_training_remarks;
                form.recurrent_training_checking_remarks = dto.recurrent_training_checking_remarks;
                form.operation_from_either_pilots_seat_remarks = dto.operation_from_either_pilots_seat_remarks;
                form.difference_training_remarks = dto.difference_training_remarks;
                form.recency_of_experience_remarks = dto.recency_of_experience_remarks;
                form.zero_flighttime_training_remarks = dto.zero_flighttime_training_remarks;
                form.other_remarks = dto.other_remarks;



                    foreach (var difference in dto.differences)
                {
                    int systemId = difference.system_id;

                    var existingActionValue = await context.trn_fstd_system_difference_value
                        .FirstOrDefaultAsync(q => q.system_id == systemId && q.form_id == form.id);

                    if (existingActionValue != null)
                    {
                        existingActionValue.compliance_level = difference.compliance_level;
                        existingActionValue.differences = difference.differences;
                        existingActionValue.syllabus_ref = difference.syllabus_ref;
                        existingActionValue.fchar = difference.fchar;
                        existingActionValue.proc = difference.proc;

                        context.Entry(existingActionValue).State = EntityState.Modified;
                    }
                    else
                    {
                        var newActionValue = new trn_fstd_system_difference_value
                        {
                            differences = difference.differences,
                            syllabus_ref = difference.syllabus_ref,
                            compliance_level = difference.compliance_level,
                            fchar = difference.fchar,
                            proc = difference.proc,
                            form_id = difference.form_id,
                            system_id = difference.system_id,
                        };

                        context.trn_fstd_system_difference_value.Add(newActionValue);
                    }
                }



                await context.SaveChangesAsync();

                return new DataResponse
                {
                    IsSuccess = true,
                    Data = form.id
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error saving EFB assessment: {ex.Message}";
                if (ex.InnerException != null)
                {
                    msg += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return new DataResponse
                {
                    IsSuccess = false,
                    Data = msg,
                };
            }
        }
    }
}