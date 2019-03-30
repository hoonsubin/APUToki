using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using APUToki.Models;
using APUToki.Views;
using Xamarin.Forms;
using APUToki.Services;

namespace APUToki.ViewModels
{
    public class TimetableViewModel : BaseViewModel
    {
        public ObservableCollection<TimetableCell> TimetableItems { get; set; }

        public TimetableViewModel()
        {
            Title = "Timetable";
            if (TimetableItems == null)
            {
                Debug.WriteLine("No Timetable items found, making a new list");
                TimetableItems = new ObservableCollection<TimetableCell>();
            }


        }




    }
}
