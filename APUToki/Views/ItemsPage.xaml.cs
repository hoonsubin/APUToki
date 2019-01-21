using System;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using APUToki.Models;
using APUToki.ViewModels;
using APUToki.Services;

namespace APUToki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        //initialize view model
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            //set the binding context for the front-end part as the ItemsViewModel.cs
            BindingContext = viewModel = new ItemsViewModel();

            //set the item source for the list view
            ItemsListView.ItemsSource = viewModel.ItemGrouped;

        }

        //execute when an item is selected
        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //if the selected item is not an item, exit out of the block
            if (!(args.SelectedItem is Item item))
                return;

            //show a new page which is the ItemDetailViewModel
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            //Manually deselect item after the page shows
            ItemsListView.SelectedItem = null;
        }
        /*
        //execute when the AddItem button is touched
        async void AddItem_Clicked(object sender, EventArgs e)
        {
            //show a new page called NewItemPage
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
            
        }
        */

        async void ExportItem_Clicked(object sender, EventArgs e)
        {
            var answer = DisplayAlert("Export events", "Do you wish to export all the academic events to your device calendar?", "Yes", "No");
            if (await answer)
            {
                try
                {
                    //execute export events
                    await SyncEvents.ExportEvents();
                }
                catch (Exception ex)
                {
                    await SyncEvents.SendErrorEmail(ex.Message);
                }
            }

        }

        void Handle_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            //only scroll when the item is more than 1 in the list
            if (viewModel.ScrollToEvent && viewModel.Items.Count > 0)
            {
                viewModel.ScrollToEvent = false;
                //get the index of the event that is later than today
                int todaysEvent = viewModel.Items.IndexOf(viewModel.Items
                    .FirstOrDefault(i => DateTime.Compare(DateTime.ParseExact(i.StartDateTime, "yyyy/MM/dd", null), DateTime.Now) > 0));

                //if the index is found, scroll to that item in the list
                if (todaysEvent != -1)
                {
                    ItemsListView.ScrollTo(viewModel.Items[todaysEvent], ScrollToPosition.Center, true);
                    Debug.WriteLine("[ScrollToEvent]Moving to the current event " + viewModel.Items[todaysEvent].EventName + "-" + viewModel.Items[todaysEvent].StartDateTime);
                }
            }
        }

        //override the OnAppearing function, which runs when the page is shown
        protected override void OnAppearing()
        {
            base.OnAppearing();

            //run the load items command once when there is no items
            if (viewModel.ItemGrouped.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);

            }
        }
    }
}