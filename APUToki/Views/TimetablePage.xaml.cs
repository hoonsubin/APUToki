using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using APUToki.ViewModels;
using APUToki.Models;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System.Linq;

namespace APUToki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimetablePage : ContentPage
    {
        //initialize view model
        TimetableViewModel viewModel;

        List<View> CurrentCells;

        public TimetablePage()
        {
            InitializeComponent();
            //set the binding context for the front-end part
            BindingContext = viewModel = new TimetableViewModel();

            //create a new list that holds the xaml elements
            CurrentCells = new List<View>();

            //show the last timetable view
            if (Application.Current.Properties.ContainsKey("LastOpenedQ"))
            {
                btnTermChange.Text = Application.Current.Properties["LastOpenedQ"].ToString();
            }

            viewModel.LoadTimetableContents();

            //add the existing timetable items to the grid
            DrawQuarter();

            MessagingCenter.Subscribe<LectureDetailViewModel, List<TimetableCell>>(this, "AddTimetableCell", (sender, arg) => 
            {
                AddCellsToTimetable(arg);
            });

            MessagingCenter.Subscribe<LectureDetailViewModel, List<TimetableCell>>(this, "RemoveTimetableCell", (sender, arg) =>
            {
                RemoveCellsFromTimetableAsync(arg);
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
                var backgroundColor = new BoxView
                {
                    BackgroundColor = Color.LightGray
                };

                //create a new cell for the timetable to display
                var cell = new Button
                {
                    Text = i.ParentLecture.SubjectNameEN,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Micro,typeof(Button)),
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.Black,
                    Margin = 0
                };

                //add button click function which will call the method inside the view model
                cell.Clicked += async (sender, args) => await Navigation.PushAsync(new LectureDetailPage(new LectureDetailViewModel(i.ParentLecture), true));

                gridLayout.Children.Add(backgroundColor, i.Column, i.Row);

                //add the button to the grid layout with a dynamic row and column
                gridLayout.Children.Add(cell, i.Column, i.Row);

                //add the button to the current cells list which is used to track which element to delete
                //this will generate the Current Cells list so that we don't have to save it
                CurrentCells.Add(backgroundColor);
                CurrentCells.Add(cell);
            }
        }

        void DrawQuarter()
        {
            var is1stQuarter = btnTermChange.Text.Contains("1st");

            if (is1stQuarter)
            {
                ClearAllGridChildrens();
                DrawCellsToGrid(viewModel.Q1TimetableItems);
            }
            else
            {
                ClearAllGridChildrens();
                DrawCellsToGrid(viewModel.Q2TimetableItems);
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
                //switch to 2nd quarter
                btnTermChange.Text = "2nd Q";
                DrawQuarter();

            }
            else
            {
                //switch to 1st quarter
                btnTermChange.Text = "1st Q";
                DrawQuarter();
            }

            Application.Current.Properties["LastOpenedQ"] = btnTermChange.Text;
            Application.Current.SavePropertiesAsync();
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
        }


        /// <summary>
        /// Removes the given list of cells from timetable and the list that holds.
        /// </summary>
        /// <param name="cellsToRemove">List of cells to remove.</param>
        public async void RemoveCellsFromTimetableAsync(List<TimetableCell> cellsToRemove)
        {
            //loop through the timetable cell list to remove
            foreach (var i in cellsToRemove)
            {
                viewModel.Q1TimetableItems.Remove(i);
                viewModel.Q2TimetableItems.Remove(i);
            }

            //clear and re-draw the buttons from the timetalble
            if (btnTermChange.Text.Contains("1st"))
            {
                ClearAllGridChildrens();
                DrawCellsToGrid(viewModel.Q1TimetableItems);

            }
            else
            {
                ClearAllGridChildrens();
                DrawCellsToGrid(viewModel.Q2TimetableItems);
            }
            //save (update) the current list of lectures
            viewModel.SaveTimetableContents();
            //remove the current page and go back to the timetable view page
            await Navigation.PopAsync();
        }


        /// <summary>
        /// Checks the existing list of timetable cells and compare them with the new ones
        /// If there is no conflicts or same lectures, this will add the element to the list and draw them
        /// to the right quarter
        /// </summary>
        /// <param name="cellsToAdd">Cells to add.</param>
        public void AddCellsToTimetable(List<TimetableCell> cellsToAdd)
        {
            //combine Q1 list and Q2 list
            var allCurrentLectures = viewModel.Q1TimetableItems.Union(viewModel.Q2TimetableItems);

            //check if there are any intersecting items in two lists, and make them into a list of cells
            var intersectingLecture = allCurrentLectures.Intersect(cellsToAdd);

            if (intersectingLecture.Any())
            {
                //check if the lecture is the same. We use the first index of the list because they all have the same parent lecture
                if (allCurrentLectures.FirstOrDefault(cell => cell.ParentLecture.SubjectNameEN == cellsToAdd[0].ParentLecture.SubjectNameEN) != null)
                {
                    Application.Current.MainPage.DisplayAlert("Alert", "This lecture is already in the timetable", "Dismiss");
                }
                else
                {

                    //todo: get the lecture with the same period in the existing list, and add options to switch with current lecture
                    Application.Current.MainPage.DisplayAlert("Message","Lecture is conflicting with existing lecture", "Dismiss");
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
                        viewModel.Q1TimetableItems.Add(cell);
                    }
                    else if (cell.ParentLecture.Term.Contains("2"))
                    {
                        viewModel.Q2TimetableItems.Add(cell);
                    }
                    else
                    {
                        //add the semester lecture to both lists
                        viewModel.Q1TimetableItems.Add(cell);
                        viewModel.Q2TimetableItems.Add(cell);
                    }
                }

                //variables to make the code more easy to read
                string currentPage = btnTermChange.Text;
                string termOfLecture = cellsToAdd[0].ParentLecture.Term;

                //check if the quarter matches, or it is a semester course
                if (termOfLecture.Contains(currentPage) || termOfLecture.Contains("Semester"))
                {
                    //draw the newly added timetable cells to the grid
                    DrawCellsToGrid(cellsToAdd);
                }

                viewModel.SaveTimetableContents();

                Application.Current.MainPage.DisplayAlert("Message", $"{cellsToAdd[0].ParentLecture.SubjectNameEN} has been added to the timetable", "Ok");
            }
        }

        async void Search_ClickedAsync(object sender, EventArgs e)
        {
            //open the search page
            //await Navigation.PushAsync(new NavigationPage(new LectureSearchPage()));
            await Navigation.PushAsync(new LectureSearchPage());
        }
    }
}
