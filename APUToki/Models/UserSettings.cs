using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace APUToki.Models
{
    public static class UserSettings
    {
        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static string LastTimetableUpdate
        {
            get => AppSettings.GetValueOrDefault(nameof(LastTimetableUpdate), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(LastTimetableUpdate), value);
        }
        public static void ClearAllData()
        {
            AppSettings.Clear();
        }
    
    }
}
