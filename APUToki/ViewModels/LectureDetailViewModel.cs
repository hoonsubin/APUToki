﻿using System;
using System.Windows.Input;
using System.Collections.Generic;

using APUToki.Models;
using Xamarin.Forms;

namespace APUToki.ViewModels
{
    public class LectureDetailViewModel : BaseViewModel
    {
        public LectureItem LectureItem { get; set; }
        public List<string> LecturePeriods { get; set; }

        //open the uri to the devices default browser
        public ICommand OpenWebCommand { get; }

        public LectureDetailViewModel(LectureItem lectureItem = null)
        {
            //set the title to the name of the event
            Title = lectureItem?.SubjectNameEN;
            //load the item contents
            LectureItem = lectureItem;

            //define what the button is going to do, in this case open the syllabus of the given lecture id
            OpenWebCommand = new Command(() => Device.OpenUri
            (new Uri("https://portal2.apu.ac.jp/campusp/slbssbdr.do?value%28risyunen%29=2018&value%28semekikn%29=2&value%28kougicd%29="
            + lectureItem.SubjectId)));

        }

    }
}
