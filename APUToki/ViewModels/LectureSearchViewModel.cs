using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using APUToki.Models;

using Xamarin.Forms;

namespace APUToki.ViewModels
{
    public class LectureSearchViewModel : ContentPage
    {
        public ObservableCollection<LectureItem> SearchResults { get; set; }

        public LectureSearchViewModel()
        {
            Title = "Lecture Search";
        }
    }
}

