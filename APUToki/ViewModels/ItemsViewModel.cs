using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Plugin.Connectivity;
using APUToki.Models;
using APUToki.Views;
using APUToki.Services;

using Xamarin.Forms;

namespace APUToki.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        //represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<ItemGroup> ItemGrouped { get; set; }
        public Command LoadItemsCommand { get; set; }

        //used for invoking the scroll to event in the ItemsPage
        public bool ScrollToEvent { get; set; } = true;

        public ItemsViewModel()
        {
            //the title of the page
            Title = "Academic Calendar";
            //make and assign the List that'll contain the items
            Items = new ObservableCollection<Item>();

            ItemGrouped = new ObservableCollection<ItemGroup>();

            //load the items from the database
            //set the load items command to Execute load items command
            async void execute() => await ExecuteLoadItemsCommand();
            LoadItemsCommand = new Command(execute);

            /*
            //messaging center is used to communicate with different components with in the project
            //the subscribe operator will make this page listen to the sender which is NewItemPage
            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) => {
                //the message will contain the following values and pass it to NewItemPage
                var newItem = item as Item;
                //and add the item to the database
                await App.Database.SaveItemAsync(newItem);
                await ExecuteLoadItemsCommand();
            });
            */
        }

        //setup and group the given list into a grouped list
        void SetupGroup(ObservableCollection<Item> items)
        {
            //setup the list of groups
            var allListItemGroups = new ObservableCollection<ItemGroup>();

            //loop through all the items (ungroupped)
            foreach (var item in items)
            {
                //find any existing groups where the group title marches the Date Month of the event
                //this is a single item of the allListItemGroups List
                var listItemGroup = allListItemGroups.FirstOrDefault(g => g.Heading == item.GroupDate);

                //if the group does not exist, create one
                if (listItemGroup == null)
                {
                    //create a new small list (group)
                    listItemGroup = new ItemGroup(item.GroupDate)
                    {
                        item
                    };
                    //add the list of items to the group
                    allListItemGroups.Add(listItemGroup);
                }
                else
                {
                    //add the item to the already existing group (but not the list of groups)
                    listItemGroup.Add(item);
                }
            }
            Debug.WriteLine("[SetupGroup]Finished grouping items");

            //make sure that there are no duplicate items for the list
            var finalGroupedList = allListItemGroups.Except(ItemGrouped);

            foreach (var i in finalGroupedList)
            {
                ItemGrouped.Add(i);
            }

            Debug.WriteLine("[SetupGroup]Finished adding all the groups to the list");

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
                //clear all the items in the list
                Items.Clear();
                ItemGrouped.Clear();
                Debug.WriteLine("[ExecuteLoadItemsCommand]Cleared items list");

                //update the list only if it is connected to the internet
                if (CrossConnectivity.Current.IsConnected)
                {
                    //get new events online, and add them to the database if there is a new one
                    await SyncEvents.UpdateAcaEventsAsync();
                }
                else { Debug.WriteLine("No internet connection"); }

                //load the items in the database
                var itemsInDb = await App.Database.SortListByDate();

                //add all the items to the list
                foreach (var item in itemsInDb)
                {
                    //add items to the list
                    Items.Add(item);
                }

                //refresh the grouped item list
                SetupGroup(Items);
            }
            catch (Exception ex)
            {
                await SyncEvents.SendErrorEmail(ex.Message);
            }
            finally
            {
                //set the IsBusy to false after the program finishes
                IsBusy = false;
                ScrollToEvent = true;
                Debug.WriteLine("[ExecuteLoadItemsCommand]There are " + Items.Count + " items in the database");
            }
        }
    }
}