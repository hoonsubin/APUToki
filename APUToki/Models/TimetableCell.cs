using System;
using System.Collections.Generic;

namespace APUToki.Models
{
    public class TimetableCell
    {
        public TimetableCell()
        {
        }

        public Lecture ParentLecture { get; set; }

        public int Row { get; private set; }

        public int Column { get; private set; }

        public string DayOfWeek { get; set; }

        public string Period { get; set; }

        public string ClassStartTime { get; set; }

        public string ClassEndTime { get; private set; }

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
            //convert the DayOfWeek value to a number, missing value is 99
            var dayOfWeekToInt = new Dictionary<string, int>
            {
                {"Monday", 1},
                {"Tuesday", 2},
                {"Wednesday", 3},
                {"Thursday", 4},
                {"Friday", 5},
                {"T.B.A.", 99}
            };

            int col = dayOfWeekToInt[dayOfWeek];

            //convert the first char of the Period attribute to int, missing value is 99
            int row = (int)char.GetNumericValue(period[0]);

            //the timetable row max number is 6, anything above 6 will be considered missing
            if (row == -1)
            {
                row = 99;
            }

            var timetableCell = new TimetableCell
            {
                ParentLecture = lecture,
                Row = row,
                Column = col,
                DayOfWeek = dayOfWeek,
                Period = period,
                ClassStartTime = classStartTime,
                ClassEndTime = classStartTime.Contains("T.B.A.") ? "T.B.A." : DateTime.ParseExact(classStartTime, "HH:mm", null).AddHours(1).AddMinutes(35).ToString("HH:mm")
            };

            return timetableCell;
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
