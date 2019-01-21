using System;

using APUToki.Models;

namespace APUToki.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            //set the title to the name of the event
            Title = item?.EventName;
            //load the item contents
            Item = item;
        }
    }
}
