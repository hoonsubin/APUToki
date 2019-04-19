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


    }
}
