﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirpocketTRN
{
    public class dto_exam_student_answer
    {

        public int person_id { get; set; }
        public int question_id { get; set; }
        public int answer_id { get; set; }
        public string remark { get; set; }


    }
    public class person_score
    {
        public int person_id { get; set; }
        public int score { get; set; }
        public string status_id { get; set; }
    }

    public class dto_exam_result
    {
        public int course_id { get; set; }
        public List<person_score> scores { get; set; }
        public string exam_date { get; set; }



    }
    public class dto_exam_sign
    {
        public int persin_id { get; set; }

        public int course_id { get; set; }

        public int student_id { get; set; }



    }
    public class DataResponse
    {
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; }
    }

    public class DateObject
    {
        public DateTime? Date { get; set; }
        public DateTime? DateUtc { get; set; }

        public static DateObject ConvertToDate(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new DateObject()
                {
                    Date = null,
                    DateUtc = null,
                };
            }
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var dt = (new DateTime(prts[0], prts[1], prts[2])).Date;
            return new DateObject()
            {
                Date = dt,
            };
        }
        public static DateObject ConvertToDateTime(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new DateObject()
                {
                    Date = null,
                    DateUtc = null,
                };
            }
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var dt = new DateTime(prts[0], prts[1], prts[2], prts[3], prts[4], 0);
            return new DateObject()
            {
                Date = dt,
                DateUtc = dt.ToUniversalTime()
            };
        }

        public static List<DateObject> ConvertToDateTimeSession(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<DateObject>() {
                    new DateObject()
                    {
                        Date = null,
                        DateUtc = null,
                    },
                    new DateObject()
                    {
                        Date = null,
                        DateUtc = null,
                    }
                };


            }
            //2021-07-01-08-00-10-00
            var prts = str.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var dt1 = new DateTime(prts[0], prts[1], prts[2], prts[3], prts[4], 0);
            var dt2 = new DateTime(prts[0], prts[1], prts[2], prts[5], prts[6], 0);
            var result = new List<DateObject>();
            result.Add(new DateObject()
            {
                Date = dt1,
                DateUtc = dt1.ToUniversalTime()
            });
            result.Add(new DateObject()
            {
                Date = dt2,
                DateUtc = dt2.ToUniversalTime()
            });
            return result;
        }

    }
}