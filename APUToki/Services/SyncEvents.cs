using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using Plugin.Calendars;
using Plugin.Permissions;
using Plugin.Calendars.Abstractions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using APUToki.Models;
using Acr.UserDialogs;
using Xamarin.Essentials;
using System.Net;
using Plugin.Connectivity;

namespace APUToki.Services
{
    public static class SyncEvents
    {
        #region Academic Calendar
        /// <summary>
        /// Gets the required permissions for getting and writting the system calendar information
        /// </summary>
        /// <returns>Required permission</returns>
        public static async Task UpdateAcaEventsAsync()
        {
            var dbItems = await App.Database.GetItemsAsync();

            //get the list that is in the academic events calendar, but not in the db
            //the Item model interface will only compare the EventName and StartDateTime, and add to the list only if they are the same
            var newOnlineItems = ApuBot.AcademicEventList().Except(dbItems).ToList();

            //sync if there are new items
            if (newOnlineItems.Count > 0)
            {
                var answer = await Application.Current.MainPage.DisplayAlert("Notice", "Found " + newOnlineItems.Count + " new events, wish to update list?", "Yes", "No");
                Debug.WriteLine("[SyncEvents]Found new events");

                if (answer)
                {
                    foreach (var i in newOnlineItems)
                    {
                        //get the event that has the same date as the new one, but with a different event name
                        var changedEvent = dbItems.FirstOrDefault( n => n.StartDateTime == i.StartDateTime && n.EventName != i.EventName);

                        if (changedEvent != null)
                        {
                            //delete the old one
                            await App.Database.DeleteItemAsync(changedEvent);
                        }

                        //add the new item to the database
                        await App.Database.SaveItemAsync(i);
                    }
                    Debug.WriteLine("[SyncEvents]The event database has been updated");
                }
            }
        }

        //Export the datas (academic events) inside the database to the calendar
        public static async Task ExportEvents()
        {
            //assign the current calendar permission status
            var calendarPer = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Calendar);
            Debug.WriteLine("[ExportEvents]Calendar permission is " + calendarPer.ToString());

            if (calendarPer != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                //prompt the permission grant notice if required
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Calendar))
                {
                    await Application.Current.MainPage.DisplayAlert("Permission", "Exporting events requires permission to access calendar", "OK");
                }
                //set if the permission is granted or not
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Calendar);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Plugin.Permissions.Abstractions.Permission.Calendar))
                    calendarPer = results[Plugin.Permissions.Abstractions.Permission.Calendar];
            }

            //get the calendar data if the permission is granted
            if (calendarPer == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                //obtain all the calendars (accounts) in the system
                //todo: this part may also show errors regarding getting the system calendar
                var calendars = await CrossCalendars.Current.GetCalendarsAsync();

                //get the events in the database
                var dbEvents = await App.Database.SortListByDate();

                //check all the calendars, and add the contents in the database to the APU Calendar
                await AddEventsToDevice(calendars, dbEvents);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Access Denied", "Requires permission to export to device", "Dismiss");
            }
        }

        /// <summary>
        /// Gets the academic events online, and add the ones that are not already in the database
        /// </summary>
        /// <returns>The events to device.</returns>
        /// <param name="calendars">List of system calendars</param>
        /// <param name="eventSource">Event source</param>
        public static async Task AddEventsToDevice(IList<Calendar> calendars, List<AcademicEvent> eventSource)
        {
            using (IProgressDialog progress = UserDialogs.Instance.Progress("Progress", null, null, true, MaskType.Black))
            {
                progress.PercentComplete = 1;

                //get the apu calendar
                //todo: this part is showing errors on some devices; cannot find calendar to wirte
                var apuCalendar = calendars.FirstOrDefault(c => c.Name == "APU Academic Calendar");
                //if there was none, make a new one
                if (apuCalendar == null)
                {
                    apuCalendar = new Calendar
                    {
                        Name = "APU Academic Calendar",
                        Color = "#AB003E"
                    };
                    await CrossCalendars.Current.AddOrUpdateCalendarAsync(apuCalendar);
                    Debug.WriteLine("[ExportEvents]No APU calendar, made a new one");
                }

                progress.PercentComplete = 30;
                //get the list of all the events in the current calendar within the range of the database events
                var rawEventsOfThisCal = await CrossCalendars.Current
                        .GetEventsAsync(apuCalendar, DateTime.ParseExact(eventSource[0].StartDateTime, "yyyy/MM/dd", null)
                        , DateTime.ParseExact(eventSource[eventSource.Count - 1].EndDateTime, "yyyy/MM/dd", null).AddHours(23));

                Debug.WriteLine("[ExportEvents]There are " + rawEventsOfThisCal.Count + " events in this calendar");

                progress.PercentComplete = 50;
                //create a new list for converting Calendar Events to databse items
                var convertedEventsFromCal = new List<AcademicEvent>();

                foreach (var i in rawEventsOfThisCal)
                {
                    convertedEventsFromCal.Add(new AcademicEvent
                    {
                        EventName = i.Name,
                        StartDateTime = i.Start.ToString("yyyy/MM/dd")
                    });
                }
                progress.PercentComplete = 80;

                //compare the device calendar and the database calendar, and add ones that is only in the db
                var eventsToAdd = eventSource.Except(convertedEventsFromCal).ToList();

                //only run if there are not synced events
                if (eventsToAdd.Count > 0)
                {
                    //loop through all the events in the db, and add them to the system
                    foreach (var calItem in eventsToAdd)
                    {
                        //get events in the calendar that has the same time with the new one, but as a different name (old)
                        var oldEvent = rawEventsOfThisCal.FirstOrDefault(cur => cur.Start == DateTime.ParseExact(calItem.StartDateTime, "yyyy/MM/dd", null) && cur.Name != calItem.EventName);
                        if (oldEvent != null)
                        {
                            //delete the old event in the calendar
                            await CrossCalendars.Current.DeleteEventAsync(apuCalendar, oldEvent);
                        }
                        //create a new event for the system calendar
                        var acaEvent = new CalendarEvent
                        {
                            Name = calItem.EventName,
                            Start = DateTime.ParseExact(calItem.StartDateTime, "yyyy/MM/dd", null),
                            End = DateTime.ParseExact(calItem.StartDateTime, "yyyy/MM/dd", null).AddHours(23),
                            Reminders = new List<CalendarEventReminder> { new CalendarEventReminder() }
                        };
                        //add the created event
                        await CrossCalendars.Current.AddOrUpdateEventAsync(apuCalendar, acaEvent);

                        Debug.WriteLine("[ExportEvents]Added " + acaEvent.Name + "-" + acaEvent.Start + " to " + apuCalendar.Name);
                    }
                    Debug.WriteLine("[ExportEvents]Finished exporting " + eventsToAdd.Count + " events");
                    await Application.Current.MainPage.DisplayAlert("Finished", "Finished exporting " + eventsToAdd.Count + " events", "Dismiss");
                    progress.PercentComplete = 99;
                }
                else
                {
                    progress.PercentComplete = 99;
                    Debug.WriteLine("[ExportEvents]Calendar is already up to date");
                    await Application.Current.MainPage.DisplayAlert("Message", "Device calendar is already up to date", "Dismiss");
                }
            }
        }

        #endregion

        public static async Task SendErrorEmail(string message)
        {
            var reportBug = await Application.Current.MainPage.DisplayAlert("Error", "There was an error, do you wish the send the error report to the developer?\n" + message, "Yes", "No");
            Debug.WriteLine("[Error]"+ message);
            if (reportBug)
            {
                try
                {
                    //get device information
                    // Device Model (SMG-950U, iPhone10,6)
                    var device = DeviceInfo.Model;
                    // Manufacturer (Samsung)
                    var manufacturer = DeviceInfo.Manufacturer;
                    // Operating System Version Number (7.0)
                    var version = DeviceInfo.VersionString;
                    // Platform (Android)
                    var platform = DeviceInfo.Platform;
                    // Device Type (Physical)
                    var deviceType = DeviceInfo.DeviceType;

                    string emailBody = $"Device model: {device}\nManufacturer: {manufacturer}\nOS version: {platform} - {version}\nDevice type: {deviceType}\nError Message: {message}";

                    List<string> sendTo = new List<string>
                    {
                        "hoonki16@apu.ac.jp"
                    };

                    var emailContent = new EmailMessage
                    {
                        Subject = "Error message",
                        Body = emailBody,
                        To = sendTo
                    };
                    await Email.ComposeAsync(emailContent);
                    
                }
                catch (FeatureNotSupportedException)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", "Seems like email is not supported on this device", "Dismiss");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "This is crazy!\nAn error had occurred during the error report! I wish I can send this one too but I can't\nError message: " + ex.Message, "Abort!");
                }

            }

        }

        #region Lectures Database
        /// <summary>
        /// Updates the course database to the ones online
        /// </summary>
        /// <returns>The course list async</returns>
        public static async Task UpdateLectureListAsync()
        {
            //get the current lecture database version
            string currentVer = UserSettings.LastTimetableUpdate;

            //get the online lecture database version
            string currentOnlineVer = ApuBot.GetOnlineTimetableLastDate();
            Debug.WriteLine("[SyncEvents]Lasted update: " + currentOnlineVer);

            //check if they match, if not, run the update
            if (currentVer != currentOnlineVer)
            {
                //ask user if they want to update
                var answer = await Application.Current.MainPage.DisplayAlert("Notice", "There is a timetable online. Wish to update?\nOnline version: " + currentOnlineVer, "Yes", "No");

                if (answer)
                {
                    //todo: add progress bar feature
                    //get all the lectures online
                    var newLectures = ApuBot.LecturesList();

                    var oldDatabase = await App.Database.GetAllLecturesAsync();

                    //empty the current local database to not make duplicates
                    await App.Database.DeleteAllLecturesAsync(oldDatabase);
                    /*
                    foreach (var i in newLectures)
                    {
                        foreach (var n in i.TimetableCells)
                        {
                            Debug.WriteLine("got timetable cell " + n.ParentLecture.SubjectNameEN + " " + n.Period);
                        }
                    }
                    */
                    foreach (var i in newLectures)
                    {
                        await App.Database.SaveLectureAsync(i);
                    }
                    //add all the new lectures to the database
                    //await App.Database.SaveAllLecturesAsync(newLectures);

                    Debug.WriteLine("[SyncEvents]The lectures database has been updated");
                    await Application.Current.MainPage.DisplayAlert("Notice", "Updated the database", "Dismiss");
                    //update the last updated version date to a persistant setting
                    UserSettings.LastTimetableUpdate = currentOnlineVer;
                }
            }
            else
            {
                Debug.WriteLine("[SyncEvents]No updates found");
            }
        }

        #endregion

        public static bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://www.apu.ac.jp"))
                {
                    //return true if the site loads
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }


    }
}
