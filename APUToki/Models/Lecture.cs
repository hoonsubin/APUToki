using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace APUToki.Models
{
    [Table("Lectures")]
    public class Lecture : IEquatable<Lecture>, ILecture
    {
        public Lecture()
        {
            //initilize the lists when this object is constructed
            //TimetableCells = new List<TimetableCell>();
            //SearchTags = new List<string>();

        }

        #region Properties

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Term { get; set; }

        public string Classroom { get; set; }

        public string BuildingFloor { get; set; }

        public string SubjectId { get; set; }

        public string SubjectNameJP { get; set; }

        public string SubjectNameEN { get; set; }

        public string InstructorJP { get; set; }

        public string InstructorEN { get; set; }

        public string GradeEval { get; set; }

        public string Language { get; set; }

        public string Grade { get; set; }

        public string Field { get; set; }

        public string APS { get; set; }

        public string APM { get; set; }

        public string Semester { get; set; }

        public string Curriculum { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TimetableCell> TimetableCells { get; set; }

        #endregion

        public void AddCell(TimetableCell cell)
        {
            //create a new List of there is none
            if (TimetableCells == null)
            {
                TimetableCells = new List<TimetableCell>();
            }
            TimetableCells.Add(cell);
        }

        public List<string> SearchTags
        {
            get
            {

                var outputList = new List<string>
                {
                    Term.ToLower(),
                    Classroom.Replace("FII", "f2").ToLower(),
                    BuildingFloor.Replace("FII", "f2").ToLower(),
                    SubjectId.ToLower(),
                    SubjectNameJP,
                    SubjectNameEN.ToLower(),
                    InstructorJP,
                    InstructorEN.ToLower(),
                    Language.ToLower(),
                    Grade.Replace("Year", "grade").ToLower(),
                    Field.ToLower(),
                    APS.ToLower(),
                    APM.ToLower(),
                    Semester.ToLower(),
                    Curriculum.ToLower(),
                    Language.Contains("J") ? "japanese" : "english" + " lecture"
                };
                if (TimetableCells != null)
                {
                    foreach (var i in TimetableCells)
                    {
                        if (!outputList.Contains(i.DayOfWeek.ToLower()))
                        {
                            outputList.Add(i.DayOfWeek.ToLower());
                        }
                        if (!outputList.Contains(i.Period.ToLower()))
                        {
                            outputList.Add(i.Period.ToLower());
                        }
                    }
                }
                return outputList;
            }
        }

        #region IEquatable
        //check if the subject name, lecture semester and curriculum is the same
        public bool Equals(Lecture other)
        {
            if (other is null)
                return false;
            return SubjectNameEN == other.SubjectNameEN && Semester == other.Semester && Curriculum == other.Curriculum && InstructorEN == other.InstructorEN;
        }

        //override the object comparison logic with the one above
        public override bool Equals(object obj) => Equals(obj as Lecture);

        public override int GetHashCode() => (SubjectNameEN, Semester, Curriculum).GetHashCode();

        #endregion

        /// <summary>
        /// Check if the two lectures' times are conflicting
        /// </summary>
        /// <returns><c>true</c>, if there is a conflicting item, <c>false</c> otherwise.</returns>
        /// <param name="other">Other.</param>
        public bool HasConflict(Lecture other)
        {
            bool hasConflict = false;

            //return false if the other item is null
            if (other is null)
                return false;

            //loop through the list of cells for this instance
            foreach (var cell in TimetableCells)
            {
                //loop through the list of cells for the other instance
                foreach (var otherCell in other.TimetableCells)
                {
                    //check if there are any conflicting items
                    hasConflict = cell.HasConflict(otherCell);
                }
            }
            //this will return true even if there's only one conflicting item
            return hasConflict;
        }
    }
}
