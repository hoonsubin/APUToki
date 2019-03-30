using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using APUToki.ViewModels;
using APUToki.Models;
using APUToki.Services;
using System.Diagnostics;
using Xamarin.Forms;

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

            UpdateTimetableCells();

            MessagingCenter.Subscribe<LectureDetailViewModel, TimetableCell>(this, "AddTimetableCell", (sender, arg) => {

                Debug.WriteLine("Getting a message " + arg.Period);
                AddCellToTimetable(arg);
            });

        }

        void UpdateTimetableCells()
        {

            //loop through the list timetable cells from the view model
            foreach (var i in viewModel.TimetableItems)
            {
                //create all new label
                var cell = new Label
                {
                    Text = "X",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                };

                //add the label to the grid layout with a dynamic row and column
                gridLayout.Children.Add(cell, i.Column, i.Row);
            }
        }

        public void AddCellToTimetable(TimetableCell cellToAdd)
        {
            viewModel.TimetableItems.Add(cellToAdd);
            UpdateTimetableCells();
        }

        async void Search_ClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new LectureSearchPage()));

        }


    }
}
