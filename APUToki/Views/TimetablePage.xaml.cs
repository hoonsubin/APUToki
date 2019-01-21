using System;
using System.Collections.Generic;
using APUToki.ViewModels;

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
        }
    }
}
