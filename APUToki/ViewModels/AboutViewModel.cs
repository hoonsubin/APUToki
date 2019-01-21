using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace APUToki.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            //define what the button is going to do
            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("http://en.apu.ac.jp/home/")));
        }
        //open the uri to the devices default browser
        public ICommand OpenWebCommand { get; }
    }
}