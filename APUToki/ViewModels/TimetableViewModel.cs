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

        /// <summary>
        /// Saves the lectures in the timetable as a single string divided by a delimiter
        /// example: [SubjectNameEN]-[Curriculum]-[Semester]|[NextLecture]
        /// </summary>
        public void SaveTimetableContents()
        {
            var allLectures = Q1TimetableItems.Union(Q2TimetableItems);

            string timetableSerial = string.Empty;

            if (allLectures.Any())
            {
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
            }

            Application.Current.Properties["TimetableItems"] = timetableSerial;
            Application.Current.SavePropertiesAsync();
        }

        public async Task LoadTimetableContentsAsync()
        {
            //check if the timetable property exists first
            if (Application.Current.Properties.ContainsKey("TimetableItems"))
            {
                //call the saved string and split it into an array
                var timetableContents = Application.Current.Properties["TimetableItems"].ToString().Split(ApuBot.delimiter);

                Debug.WriteLine("[TimetableViewModel]Loading string " + Application.Current.Properties["TimetableItems"]);

                //load the database
                var timetableCellsFromDb = await App.Database.GetAllLecturesAsync();
                Debug.WriteLine("[TimetableViewModel]Finished loading database");

                for (int i = 0; i < timetableContents.Length; i++)
                {
                    //split the lecture to its attributes
                    var lectureFromSave = timetableContents[i].Split('-');

                    //use Linq to query through the lecture database
                    var lectureToLoad = timetableCellsFromDb.FirstOrDefault(x => x.SubjectNameEN == lectureFromSave[0]
                        && x.Curriculum == lectureFromSave[1]
                        && x.Semester == lectureFromSave[2]);
                    Debug.WriteLine("[TimetableViewModel]Found matching lecture " + lectureFromSave[0]);

                    //check if the lecture is not null
                    if (lectureToLoad != null)
                    {
                        //add the timetable cells to the timetable page list so it can be drawn later
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
