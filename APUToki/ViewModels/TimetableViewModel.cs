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
            //combine Quarter 1 and Querter 2
            var allTimeCells = Q1TimetableItems.Union(Q2TimetableItems);

            //make sure the list is not empty
            if (allTimeCells.Any())
            {
                var allLectures = new List<Lecture>();

                //loop through all the timetable cells and add the lectures that are not in the list
                foreach (var i in allTimeCells)
                {
                    if (!allLectures.Contains(i.ParentLecture))
                    {
                        allLectures.Add(i.ParentLecture);
                    }
                }

                //serialize the list of lectures as JSON string
                var serializedObject = App.Database.SerializeToJson(allLectures);

                Debug.WriteLine("Saving " + serializedObject);

                //assign the JOSN list to the properties dictionary to make it persistant
                Application.Current.Properties["TimetableItems"] = serializedObject;
                //save the serialized dictionary to the disk
                Application.Current.SavePropertiesAsync();
            }
            else
            {
                Debug.WriteLine("No timetable in list to save");
            }

        }

        /// <summary>
        /// Loads the JSON string that is saved in the device property keys
        /// And converts then to list of timetable lectures (both 1st and 2nd quarter)
        /// </summary>
        public void LoadTimetableContents()
        {
            //todo: this method consumes a lot of resource, must refactor
            if (Application.Current.Properties.ContainsKey("TimetableItems"))
            {
                //deserialized the saved JSON string as list of lectures
                List<Lecture> alllectures = App.Database.DeserializeLectureListFromJson(Application.Current.Properties["TimetableItems"].ToString());

                foreach (var i in alllectures)
                {
                    Debug.WriteLine("Loading " + i.SubjectNameEN);

                    //loop through the timetable cells to add the items to the quarter lists
                    foreach (var x in i.TimetableCells)
                    {
                        //assign the parent lecture as this is not saved in the JSON
                        x.ParentLecture = i;

                        if (i.Term.Contains("1st"))
                        {
                            Q1TimetableItems.Add(x);
                        }
                        else if (i.Term.Contains("2nd"))
                        {
                            Q2TimetableItems.Add(x);
                        }
                        else //semester lectures
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
