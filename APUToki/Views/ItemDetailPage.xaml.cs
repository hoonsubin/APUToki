using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using APUToki.Models;
using APUToki.ViewModels;

namespace APUToki.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        //this runs and calls the information in the item object
        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            //set the binding context for the front-end part
            BindingContext = this.viewModel = viewModel;
        }

        //this runs if the item has no information in it
        public ItemDetailPage()
        {
            InitializeComponent();

            //declare the temp item information that'll be shown
            var item = new Item
            {
                EventName = "Event 1",
                StartDateTime = DateTime.Now.ToString("yyyy/MM/dd"),
                //DayOfWeek = DateTime.Now.DayOfWeek.ToString()
            };

            //set a new view model page
            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }
    }
}