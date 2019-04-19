using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using APUToki.Models;
using APUToki.Views;
using Xamarin.Forms;
using APUToki.Services;
using System.Windows.Input;
using System.Linq;

namespace APUToki.ViewModels
{
    public class TimetableViewModel : BaseViewModel
    {

        public ICommand TouchedLectureFromTimetable { get; }

        public List<TimetableCell> Q2TimetableItems { get; set; }

        public List<TimetableCell> Q1TimetableItems { get; set; }

        public TimetableViewModel()
        {
            Title = "Timetable";

            //todo: make a function to save and load these two lists from the database
            Q1TimetableItems = new List<TimetableCell>();
            Q2TimetableItems = new List<TimetableCell>();

        }

        public void SaveTimetableContents()
        {
            var allLectures = Q1TimetableItems.Union(Q2TimetableItems);

            string timetableSerial = string.Empty;

            foreach (var i in allLectures)
            {
                string serial = i.ParentLecture.SubjectNameEN + "-" + i.ParentLecture.Curriculum + "-" + i.ParentLecture.Semester;

                if (!timetableSerial.Contains(serial))
                {
                    timetableSerial += serial + ApuBot.delimiter;
                    Debug.WriteLine(i.GetHashCode());
                }


            }

            //remove the last delimiter
            timetableSerial = timetableSerial.Remove(timetableSerial.Length - 1);

            Debug.WriteLine("[TimetableViewModel]Saving " + timetableSerial);

            Application.Current.Properties["TimetableItems"] = timetableSerial;
        }

        public async Task LoadTimetableContentsAsync()
        {
            //check if the timetable property exists first
            if (Application.Current.Properties.ContainsKey("TimetableItems"))
            {
                var timetableContents = Application.Current.Properties["TimetableItems"].ToString().Split(ApuBot.delimiter);
                var timetableCellsFromDb = await App.Database.GetAllLecturesAsync();

                for (int i = 0; i < timetableContents.Length; i++)
                {
                    var lectureFromSave = timetableContents[i].Split('-');

                    var lectureToLoad = timetableCellsFromDb.FirstOrDefault(x => x.SubjectNameEN == lectureFromSave[0]
                        && x.Curriculum == lectureFromSave[1]
                        && x.Semester == lectureFromSave[2]);

                    if (lectureToLoad != null)
                    {
                        foreach (var cell in lectureToLoad.TimetableCells)
                        {
                            if (lectureToLoad.Term.Contains("1"))
                            {
                                Q1TimetableItems.Add(cell);
                            }
                            else if (lectureToLoad.Term.Contains("2"))
                            {
                                Q2TimetableItems.Add(cell);
                            }
                            else
                            {
                                //add the semester lecture to both lists
                                Q1TimetableItems.Add(cell);
                                Q2TimetableItems.Add(cell);
                            }
                        }

                        Debug.WriteLine("[TimetableViewModel]Loading lecture " + lectureToLoad.SubjectNameEN + " from the database");

                    }
                    else
                    {
                        Debug.WriteLine("[Error]Could not find " + i + " from the database");
                    }
                }
            }
        }

    }
}
