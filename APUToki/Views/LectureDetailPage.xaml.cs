using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;

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
            //TimetablesListView.ItemsSource = viewModel.TimetableCells;

            UpdateTimetableGrid();
        }

        //this runs if the item has no information in it
        public LectureDetailPage()
        {
            InitializeComponent();

            //declare the temp item information that'll be shown
            var lectureItem = new Lecture
            {
                SubjectNameEN = "[Lecture Name]",
                InstructorEN = "[Professor Name]",
                Classroom = "[Classroom]",
                Curriculum = "[Curriculum]",
                BuildingFloor = "[Lecture building and floor]",
                Grade = "[Student Year]",
                Field = "[Major]",
                TimetableCells = new List<TimetableCell>
                {
                    new TimetableCell{DayOfWeek = "Friday", Period = "1st Period"},
                    new TimetableCell{DayOfWeek = "Friday", Period = "2nd Period"}
                }
            };

            //set a new view model page
            viewModel = new LectureDetailViewModel(lectureItem);

            BindingContext = viewModel;
        }

        void UpdateTimetableGrid()
        {
            //loop through the list timetable cells from the view model
            foreach (var i in viewModel.TimetableCells)
            {
                //create all new label
                var cell = new BoxView
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.Gray
                };

                //add the label to the grid layout with a dynamic row and column
                gridLayout.Children.Add(cell, i.Column, i.Row);
            }
        }
    }
}