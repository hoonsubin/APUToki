using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using APUToki.ViewModels;
using APUToki.Models;
using APUToki.Services;
using System.Windows.Input;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

namespace APUToki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimetablePage : ContentPage
    {
        //initialize view model
        TimetableViewModel viewModel;

        List<View> CurrentCells;

        //public ICommand SwitchQuarter { get; }

        public TimetablePage()
        {
            InitializeComponent();
            //set the binding context for the front-end part
            BindingContext = viewModel = new TimetableViewModel();

            //create a new list that holds the xaml elements
            CurrentCells = new List<View>();

            //add the existing timetable items to the grid
            DrawCellsToGrid(viewModel.Q1TimetableItems);


            MessagingCenter.Subscribe<LectureDetailViewModel, List<TimetableCell>>(this, "AddTimetableCell", (sender, arg) => {
                AddCellsToTimetable(arg);
                Debug.WriteLine($"Got {arg} from LectureDetailViewModel");
            });
        }

        /// <summary>
        /// Gets the list of cells, and draws them to the timetable grid
        /// </summary>
        /// <param name="cellsToDisplay">Cells to display.</param>
        void DrawCellsToGrid(List<TimetableCell> cellsToDisplay)
        {
            //loop through the list timetable cells from the view model
            foreach (var i in cellsToDisplay)
            {
                //create a new cell for the timetable to display
                var cell = new Button
                {
                    Text = i.ParentLecture.SubjectNameEN,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 8,
                    BackgroundColor = Color.White
                };
                
                //add the label to the grid layout with a dynamic row and column
                gridLayout.Children.Add(cell, i.Column, i.Row);
                Debug.WriteLine("Adding " + i.ParentLecture.SubjectNameEN + " " + i.Period + " to timetable");
                CurrentCells.Add(cell);
            }
        }

        /// <summary>
        /// Switchs the quarter to be displayed when the button is pressed
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Even argument</param>
        void SwitchQuarter_Clicked(object sender, EventArgs e)
        {
            if (btnTermChange.Text.Contains("1st"))
            {
                //switch 1st quarter to 2nd quarter
                btnTermChange.Text = "2nd Q";
                ClearAllGridChildrens();
                DrawCellsToGrid(viewModel.Q2TimetableItems);

            }
            else
            {
                //switch 2nd quarter to 1st quarter
                btnTermChange.Text = "1st Q";
                ClearAllGridChildrens();
                DrawCellsToGrid(viewModel.Q1TimetableItems);
            }
        }

        /// <summary>
        /// Deletes all the cells on the grid and clears the
        /// </summary>
        void ClearAllGridChildrens()
        {
            foreach (var i in CurrentCells)
            {
                gridLayout.Children.Remove(i);
            }
            CurrentCells.Clear();
            Debug.WriteLine("Clearing all grids");
        }

        public void AddCellsToTimetable(List<TimetableCell> cellsToAdd)
        {
            //combine Q1 list and Q2 list
            var allCurrentLectures = viewModel.Q1TimetableItems.Union(viewModel.Q2TimetableItems);

            //check if there are any intersecting items in two lists, and make them into a list of cells with the common period
            var intersectingLecture = allCurrentLectures.Intersect(cellsToAdd);

            if (intersectingLecture.Any())
            {
                //check if the lecture is the same
                if (allCurrentLectures.FirstOrDefault(cell => cell.ParentLecture.SubjectNameEN == cellsToAdd[0].ParentLecture.SubjectNameEN) != null)
                {
                    Application.Current.MainPage.DisplayAlert("Alert", "This lecture is already in the timetable", "Dismiss");
                }
                else
                {
                    //todo: get the lecture with the same period in the existing list, and add options to switch with current lecture
                    Application.Current.MainPage.DisplayAlert("Alert", "Lecture is conflicting with ", "Dismiss");
                }
            }
            else
            {
                //add the cells to the timetable
                foreach (var cell in cellsToAdd)
                {

                    //check if the lecture is either 1st or 2nd period
                    //semester lectures are added to both lists
                    if (cell.ParentLecture.Term.Contains("1"))
                    {
                        Debug.WriteLine(cell.ParentLecture.SubjectNameEN + " is 1st period");
                        viewModel.Q1TimetableItems.Add(cell);
                    }
                    else if (cell.ParentLecture.Term.Contains("2"))
                    {
                        Debug.WriteLine(cell.ParentLecture.SubjectNameEN + " is 2nd period");
                        viewModel.Q2TimetableItems.Add(cell);
                    }
                    else
                    {
                        //add the semester lecture to both lists
                        viewModel.Q1TimetableItems.Add(cell);
                        viewModel.Q2TimetableItems.Add(cell);
                    }
                }

                //draw the newly added timetable cells to the grid
                DrawCellsToGrid(cellsToAdd);

                //todo: soft message to notify that the lecture has been added
            }
        }

        async void Search_ClickedAsync(object sender, EventArgs e)
        {
            //open the search page
            await Navigation.PushAsync(new NavigationPage(new LectureSearchPage()));

        }


    }
}
