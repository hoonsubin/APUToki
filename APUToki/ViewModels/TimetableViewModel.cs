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

            Q1TimetableItems = new List<TimetableCell>();
            Q2TimetableItems = new List<TimetableCell>();

        }

        /// <summary>
        /// Saves the lectures in the timetable as a single string divided by a delimiter
        /// example: [SubjectNameEN]-[Curriculum]-[Semester]|[NextLecture]
        /// </summary>
        public void SaveTimetableContents()
        {

            //todo: change this method so that it serializes the list of timetable cell as xml

            var allTimeCells = Q1TimetableItems.Union(Q2TimetableItems);

            var allLectures = new List<Lecture>();

            foreach(var i in allTimeCells)
            {
                if (!allLectures.Contains(i.ParentLecture))
                {
                    allLectures.Add(i.ParentLecture);
                }
            }

            if (allTimeCells.Any())
            {
                var serializedObject = App.Database.SerializeToJson(allLectures);

                Debug.WriteLine("Saving " + serializedObject);

                Application.Current.Properties["TimetableItems"] = serializedObject;
                //save the serialized dictionary to the disk
                Application.Current.SavePropertiesAsync();
            }
            else
            {
                Debug.WriteLine("No timetable in list to save");
            }

        }

        public void LoadTimetableContents()
        {
            if (Application.Current.Properties.ContainsKey("TimetableItems"))
            {
                //todo: problem with loading the parent lecture object


                List<Lecture> alllectures = App.Database.DeserializeLectureListFromJson(Application.Current.Properties["TimetableItems"].ToString());

                foreach (var i in alllectures)
                {

                    Debug.WriteLine("Loading " + i.SubjectNameEN);

                    foreach (var x in i.TimetableCells)
                    {
                        x.ParentLecture = i;

                        if (i.Term.Contains("1st"))
                        {

                            Q1TimetableItems.Add(x);
                        }
                        else if (i.Term.Contains("2nd"))
                        {
                            Q2TimetableItems.Add(x);
                        }
                        else
                        {
                            Q1TimetableItems.Add(x);
                            Q2TimetableItems.Add(x);
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("Found no save for timetables");
            }
        }

    }
}
