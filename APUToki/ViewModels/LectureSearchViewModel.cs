using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using APUToki.Models;
using APUToki.Services;
using System.Diagnostics;
using Xamarin.Forms;

namespace APUToki.ViewModels
{
    public class LectureSearchViewModel : ContentPage
    {
        public ObservableCollection<Lecture> SearchResults { get; set; }
        //public ObservableCollection<LectureItem> LectureDatabase { get; set; }
        public Command LoadItemsCommand { get; set; }

        public LectureSearchViewModel()
        {
            Title = "Lecture Search";
            SearchResults = new ObservableCollection<Lecture>();

            //load the items from the database
            //set the load items command to Execute load items command
            async void execute() => await ExecuteLoadItemsCommand();
            LoadItemsCommand = new Command(execute);

        }

        //invokes everytime when the searchbar is changed
        public async Task SearchLecturesAsync(string query)
        {
            //load saved lectures from the database
            var database = await App.Database.GetAllLecturesAsync();

            Debug.WriteLine($"[SearchLecturesAsync]There are {database.Count} lectures in the database");

            SearchResults.Clear();

            if (!string.IsNullOrEmpty(query))
            {
                Debug.WriteLine("[SearchLecturesAsync]User searched for " + query);
                //search with the given query
                var searchedLectures = SearchEngine.SearchLecture(query, database);
                if (searchedLectures != null)
                {
                    foreach (var i in searchedLectures)
                    {
                        SearchResults.Add(i);
                    }
                }
                int resultsCount = SearchResults.Count;

                Debug.WriteLine("[SearchLecturesAsync]Got " + resultsCount + " results");
                Debug.WriteLine("[SearchLecturesAsync]Took " + SearchEngine.LastSearchTime + " ms to search");


                if (resultsCount <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Notice", "No results found", "Dismiss");
                }
            }

        }

        //clear the current list of items and load the new items through GetItemsAsync
        //this is executed when the user refreshes the list
        async Task ExecuteLoadItemsCommand()
        {
            //the code will run only when IsBusy is false
            if (IsBusy)
                return;
            //make is busy ture so that the user can't run this twice while it is runnig
            IsBusy = true;
            Debug.WriteLine("[ExecuteLoadLecturesCommand]Start loading lectures...");

            try
            {
                //update the list only if it is connected to the internet
                if (SyncEvents.IsConnectedToInternet())
                {
                    //get new events online, and add them to the database if there is a new one
                    await SyncEvents.UpdateLectureListAsync();
                }

            }
            catch (Exception ex)
            {
                await SyncEvents.SendErrorEmail(ex.ToString());
            }
            finally
            {
                //set the IsBusy to false after the program finishes
                IsBusy = false;
                Debug.WriteLine("[ExecuteLoadLecturesCommand]Finished loading");

            }
        }


    }
}

