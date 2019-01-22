using System;

using APUToki.Models;

namespace APUToki.ViewModels
{
    public class LectureDetailViewModel : BaseViewModel
    {
        public LectureItem Item { get; set; }
        public LectureDetailViewModel(LectureItem item = null)
        {
            //set the title to the name of the event
            Title = item?.SubjectNameEN;
            //load the item contents
            Item = item;
        }
    }
}
