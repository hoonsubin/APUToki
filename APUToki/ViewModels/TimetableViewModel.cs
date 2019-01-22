using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using APUToki.Models;
using APUToki.Views;
using Xamarin.Forms;

namespace APUToki.ViewModels
{
    public class TimetableViewModel : BaseViewModel
    {
        public ObservableCollection<LectureItem> LectureItems { get; set; }

        public TimetableViewModel()
        {
            Title = "Timetable";
            LectureItems = new ObservableCollection<LectureItem>();
        }
    }
}
