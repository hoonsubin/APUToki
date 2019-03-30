using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using APUToki.ViewModels;
using APUToki.Models;
using APUToki.Services;
using System.Diagnostics;
using Xamarin.Forms;
using System.Linq;

namespace APUToki.Views
{
    public partial class TimetablePage : ContentPage
    {
        //initialize view model
        TimetableViewModel viewModel;

        public TimetablePage()
        {
            InitializeComponent();
            //set the binding context for the front-end part
            BindingContext = viewModel = new TimetableViewModel();

            //add the existing timetable items to the grid
            AddTimetableCells(viewModel.TimetableItems);

            MessagingCenter.Subscribe<LectureDetailViewModel, List<TimetableCell>>(this, "AddTimetableCell", (sender, arg) => {
                AddCellToTimetable(arg);
                Debug.WriteLine("Got message from LectureDetailViewModel" + arg);
            });

        }

        void AddTimetableCells(List<TimetableCell> cellsToDisplay)
        {

            //loop through the list timetable cells from the view model
            foreach (var i in cellsToDisplay)
            {
                //create all new label
                var cell = new BoxView
                {
                    //Text = "X",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.Gray
                };

                //add the label to the grid layout with a dynamic row and column
                gridLayout.Children.Add(cell, i.Column, i.Row);
            }
        }

        public void AddCellToTimetable(List<TimetableCell> cellsToAdd)
        {
            var intersectingList = viewModel.TimetableItems.Intersect(cellsToAdd);

            if (intersectingList.Any())
            {
                Application.Current.MainPage.DisplayAlert("Alert", "Lecture is conflicting", "Dismiss");
            }
            else
            {
                foreach (var i in cellsToAdd)
                {
                    //add the item to the timetable list
                    viewModel.TimetableItems.Add(i);
                }
            }


            AddTimetableCells(cellsToAdd);
        }

        async void Search_ClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new LectureSearchPage()));

        }


    }
}
