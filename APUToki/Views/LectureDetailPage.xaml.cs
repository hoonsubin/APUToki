using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using APUToki.Models;
using APUToki.ViewModels;

namespace APUToki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LectureDetailPage : ContentPage
    {
        LectureDetailViewModel viewModel;

        //this runs and calls the information in the item object
        public LectureDetailPage(LectureDetailViewModel viewModel)
        {
            InitializeComponent();

            //set the binding context for the front-end part
            BindingContext = this.viewModel = viewModel;
        }

        //this runs if the item has no information in it
        public LectureDetailPage()
        {
            InitializeComponent();

            //declare the temp item information that'll be shown
            var lectureItem = new LectureItem
            {
                SubjectNameEN = "Empty Lecture",
                InstructorEN = "Professor",
                Period = "5th Period",
                Classroom = "classroom",
                DayOfWeek = "Monday",
                Curriculum = "curriculum",
                BuildingFloor = "A building 3rd floor"
            };

            //set a new view model page
            viewModel = new LectureDetailViewModel(lectureItem);
            BindingContext = viewModel;
        }
    }
}