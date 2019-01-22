using System;
using System.Collections.Generic;

namespace APUToki.Models
{
    //this lecture item class holds all the lecture information
    public class LectureItem : IEquatable<LectureItem>
    {
        public LectureItem()
        {
        }

        public int Id { get; set; }

        //the duration and start of lecture (semester course, Q1, Q2)
        public string Term { get; set; }

        public string DayOfWeek { get; set; }

        public string Period { get; set; }

        public string Classroom { get; set; }

        public string BuildingFloor { get; set; }

        public string SubjectId { get; set; }

        public string SubjectNameJP { get; set; }

        public string SubjectNameEN { get; set; }

        public string InstructorJP { get; set; }

        public string InstructorEN { get; set; }

        public string Language { get; set; }

        public string Grade { get; set; }

        public string Field { get; set; }
        //which field in APS
        public string APS { get; set; }
        //which field in APM
        public string APM { get; set; }

        //add public SyllabusItem Syllabus { get; set; }
        //what semester the lecture is (ex: 2018 Fall)
        public string Semester { get; set; }

        public string Curriculum { get; set; }

        public string StartTime { get; set; }

        public string EndTime
        {
            get
            {
                //return the StartTime value, but added 1 hour and 35 minutes to it
                return StartTime.Contains("T.B.A.") ? "T.B.A." : DateTime.ParseExact(StartTime, "HH:mm", null).AddHours(1).AddMinutes(35).ToString("HH:mm");
            }
        }

        //the tags are used for searching, and will be dynamically generated
        public List<string> SearchTags
        {
            get
            {
                var outputList = new List<string>
                {
                    Term.Contains("Q") ? "quarter" : "semester" + " class",
                    DayOfWeek.ToLower(),
                    Period.ToLower(),
                    Classroom.Replace("FII", "f2"),
                    Classroom.ToLower(),
                    BuildingFloor.Replace("FII", "f2"),
                    BuildingFloor.ToLower(),
                    SubjectId.ToLower(),
                    SubjectNameJP,
                    SubjectNameEN.ToLower(),
                    InstructorJP,
                    InstructorEN.ToLower(),
                    Language.ToLower(),
                    Grade.ToLower(),
                    Grade.Replace("Year", "grade"),
                    Field.ToLower(),
                    APS.ToLower(),
                    APM.ToLower(),
                    Semester.ToLower(),
                    Curriculum.ToLower(),
                    Language.Contains("J") ? "japanese" : "english"
                };

                return outputList;
            }
        }

        //check if the subject name, lecture semester and curriculum is the same
        public bool Equals(LectureItem other)
        {
            if (other is null)
                return false;
            return SubjectNameEN == other.SubjectNameEN && Semester == other.Semester && Curriculum == other.Curriculum;
        }

        public override bool Equals(object obj) => Equals(obj as LectureItem);

        public override int GetHashCode() => (SubjectNameEN, Semester, Curriculum).GetHashCode();
    }
}
