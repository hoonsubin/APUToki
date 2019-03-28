using System.Collections.Generic;

namespace APUToki.Models
{
    public interface ILecture
    {
        //int Id { get; set; }
        string Term { get; set; }
        string Classroom { get; set; }
        string BuildingFloor { get; set; }
        string SubjectId { get; set; }
        string SubjectNameJP { get; set; }
        string SubjectNameEN { get; set; }
        string InstructorJP { get; set; }
        string InstructorEN { get; set; }
        string GradeEval { get; set; }
        string Language { get; set; }
        string Grade { get; set; }
        string Field { get; set; }
        string APS { get; set; }
        string APM { get; set; }
        string Semester { get; set; }
        string Curriculum { get; set; }
        //List<TimetableCell> TimetableCells;

        bool Equals(Lecture other);
        bool Equals(object obj);
        int GetHashCode();
    }
}