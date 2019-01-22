using System;
using System.Collections.Generic;
using APUToki.Models;
using APUToki.ViewModels;
using Xamarin.Forms;

namespace APUToki.Views
{
    public partial class LectureSearchPage : ContentPage
    {
        LectureSearchViewModel viewModel;

        public LectureSearchPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new LectureSearchViewModel();

        }

        //execute when an item is selected
        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //if the selected item is not an item, exit out of the block
            if (!(args.SelectedItem is LectureItem lectureItem))
                return;

            //show a new page which is the ItemDetailViewModel
            await Navigation.PushAsync(new LectureDetailPage(new LectureDetailViewModel(lectureItem)));

            //Manually deselect item after the page shows
            ItemsListView.SelectedItem = null;
        }


        async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            await viewModel.SearchLecturesAsync(searchQuery.Text);
        }

        //override the OnAppearing function, which runs when the page is shown
        protected override void OnAppearing()
        {
            base.OnAppearing();

            //run the load items command once when there is no items
            if (searchQuery.Text != "" && viewModel.SearchResults.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);

            }
        }

    }
}
