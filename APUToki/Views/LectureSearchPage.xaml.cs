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

        void Handle_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {

        }
        //execute when an item is selected
        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //if the selected item is not an item, exit out of the block
            if (!(args.SelectedItem is LectureItem item))
                return;

            //show a new page which is the ItemDetailViewModel
            await Navigation.PushAsync(new LectureDetailPage(new LectureDetailViewModel(item)));

            //Manually deselect item after the page shows
            ItemsListView.SelectedItem = null;
        }
    }
}
