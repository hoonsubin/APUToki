using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Diagnostics;
using APUToki.Models;
using Xamarin.Forms;

namespace APUToki.ViewModels
{
    public class LectureDetailViewModel : BaseViewModel
    {
        public Lecture LectureItem { get; set; }

        public List<TimetableCell> TimetableCells { get; set; }

        //open the uri to the devices default browser
        public ICommand OpenWebCommand { get; }

        public ICommand AddToTimetableCommand { get; }

        public LectureDetailViewModel(Lecture lectureItem = null)
        {
            //set the title to the name of the event
            Title = lectureItem?.SubjectNameEN;
            //load the item contents
            LectureItem = lectureItem;

            TimetableCells = lectureItem.TimetableCells;

            //define what the button is going to do, in this case open the syllabus of the given lecture id
            OpenWebCommand = new Command(() => Device.OpenUri
            (new Uri("https://portal2.apu.ac.jp/campusp/slbssbdr.do?value%28risyunen%29=2019&value%28semekikn%29=1&value%28kougicd%29="
            + lectureItem.SubjectId)));

            AddToTimetableCommand = new Command(() => AddLecture(lectureItem));

        }

        private void AddLecture(Lecture lecture)
        {
            MessagingCenter.Send(this, "AddTimetableCell", lecture.TimetableCells);
            Debug.WriteLine("Sent message to TimetablePage.xaml.cs");
        }

    }
}
