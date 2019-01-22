using System;

using APUToki.Models;

namespace APUToki.ViewModels
{
    public class LectureDetailViewModel : BaseViewModel
    {
        public LectureItem LectureItem { get; set; }
        public LectureDetailViewModel(LectureItem lectureItem = null)
        {
            //set the title to the name of the event
            Title = lectureItem?.SubjectNameEN;
            //load the item contents
            LectureItem = lectureItem;
        }
    }
}
