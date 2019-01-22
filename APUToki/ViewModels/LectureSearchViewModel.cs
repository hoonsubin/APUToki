using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using APUToki.Models;
using APUToki.Services;
using System.Diagnostics;
using Xamarin.Forms;
using Plugin.Connectivity;

namespace APUToki.ViewModels
{
    public class LectureSearchViewModel : ContentPage
    {
        public ObservableCollection<LectureItem> SearchResults { get; set; }
        public ObservableCollection<LectureItem> LectureDatabase { get; set; }
        public Command LoadItemsCommand { get; set; }

        public LectureSearchViewModel()
        {
            Title = "Lecture Search";
            SearchResults = new ObservableCollection<LectureItem>();

            //load the items from the database
            //set the load items command to Execute load items command
            async void execute() => await ExecuteLoadItemsCommand();
            LoadItemsCommand = new Command(execute);

        }

        //invokes everytime when the searchbar is changed
        public async Task SearchLecturesAsync(string query)
        {
            //load saved lectures from the database
            var database = await App.Database.SortByLectureName();

            SearchResults.Clear();

            if (query != "")
            {
                //the search algorithm starts here, currently it's just a simple linear filtering
                //loop through the database
                foreach (var i in database)
                {
                    //loop through the tags of the item
                    foreach (var tag in i.SearchTags)
                    {
                        //add the item to the list if it contains the word
                        //this will be the main search algorithm
                        if (!SearchResults.Contains(i) && tag.Contains(query.ToLower()))
                        {
                            SearchResults.Add(i);
                        }

                    }
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
            Debug.WriteLine("[ExecuteLoadItemsCommand]Start loading items...");

            try
            {
                //update the list only if it is connected to the internet
                if (CrossConnectivity.Current.IsConnected)
                {
                    //get new events online, and add them to the database if there is a new one
                    await SyncEvents.UpdateLectureListAsync();
                }
                else { Debug.WriteLine("No internet connection"); }

            }
            catch (Exception ex)
            {
                await SyncEvents.SendErrorEmail(ex.Message);
            }
            finally
            {
                //set the IsBusy to false after the program finishes
                IsBusy = false;
                //Debug.WriteLine("[ExecuteLoadItemsCommand]There are " + lec.Count + " items in the database");
            }
        }


    }
}

