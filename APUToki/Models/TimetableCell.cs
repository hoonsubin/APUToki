using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace APUToki.Models
{
    [Table("TimetableCells")]
    public class TimetableCell
    {
        public TimetableCell()
        {
        }

        #region Properties

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Lecture))]
        public int LectureId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Lecture ParentLecture { get; set; }

        public int Row
        {
            get
            {
                //convert the first char of the Period attribute to int, missing value is 99
                int row = (int)char.GetNumericValue(Period[0]);

                //the timetable row max number is 6, anything above 6 will be considered missing
                if (ParentLecture.Term.Contains("Session"))
                {
                    row = 99;
                }

                return row;
            }
        }

        public int Column
        {
            get
            {
                //convert the DayOfWeek value to a number, missing value is 99
                var dayOfWeekToInt = new Dictionary<string, int>
                {
                    {"Monday", 1},
                    {"Tuesday", 2},
                    {"Wednesday", 3},
                    {"Thursday", 4},
                    {"Friday", 5},
                    {"T.B.A.", 99},
                    {"Session", 99}
                };

                int col = dayOfWeekToInt[DayOfWeek];

                return col;
            }
        }

        public string DayOfWeek { get; set; }

        public string Period { get; set; }

        public string ClassStartTime { get; set; }

        public string ClassEndTime { get; private set; }

        #endregion

        /// <summary>
        /// Parse and return a timetable cell
        /// </summary>
        /// <returns>new timetable cell</returns>
        /// <param name="lecture">Lecture.</param>
        /// <param name="dayOfWeek">Day of week.</param>
        /// <param name="period">Period.</param>
        /// <param name="classStartTime">Class start time.</param>
        public static TimetableCell Parse(Lecture lecture, string dayOfWeek, string period, string classStartTime)
        {
            if (lecture.Term.Contains("Session"))
            {
                //sessions have a different start and end time
                //so manually add them
                return new TimetableCell
                {
                    ParentLecture = lecture,
                    ClassStartTime = "T.B.A.",
                    DayOfWeek = dayOfWeek,
                    Period = period,
                    ClassEndTime = "T.B.A"
                };
            }

            return new TimetableCell
            {
                ParentLecture = lecture,
                DayOfWeek = dayOfWeek,
                Period = period,
                ClassStartTime = classStartTime,
                ClassEndTime = classStartTime.Contains("T.B.A.") ? "T.B.A." : DateTime.ParseExact(classStartTime, "HH:mm", null).AddHours(1).AddMinutes(35).ToString("HH:mm")
            };
        }

        /// <summary>
        /// Check if the timetable is conflicting
        /// </summary>
        /// <returns><c>true</c>, if the row and the column is the same, <c>false</c> otherwise.</returns>
        /// <param name="other">Other.</param>
        public bool HasConflict(TimetableCell other)
        {
            if (other is null)
                return false;

            return Row == other.Row && Column == other.Column;
        }

    }
}
