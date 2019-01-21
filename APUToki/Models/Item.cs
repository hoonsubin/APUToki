using SQLite;
using System.Collections.ObjectModel;
using System;
using Xamarin.Forms;

namespace APUToki.Models
{

    public class Item : IEquatable<Item>
    {
        //id for the item
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        //name of the event, which should be descriptive
        public string EventName { get; set; }
        //the day of the week in full (ex: Friday, instead of Fri)
        public string DayOfWeek { get { return DateTime.ParseExact(StartDateTime, "yyyy/MM/dd", null).DayOfWeek.ToString(); } }
        //the start day of the event formatted as yyyy/MM/dd (ex: 1992/02/21)
        public string StartDateTime { get; set; }
        //return silver for events that has passed
        public Color TextColor
        {
            get
            {
                string today = DateTime.Now.ToString("yyyy/MM/dd");

                //return the color gold if the event day is the same as today
                if (today == EndDateTime)
                    return Color.Gold;

                //if not, return silver when the event has past, or black if it has not
                return Done ? Color.Silver : Color.Black;

            }
        }

        //GroupDate is formatted as yyyy/MM, this is only used for list grouping reasons
        public string GroupDate
        {
            get
            {
                return StartDateTime.Remove(7);
            }
        }
        //by default the event will happen all day long so it'll be same as the starting date
        public string EndDateTime { get { return StartDateTime; } }

        //a bool for checking if the event has passed the current date or not
        public bool Done
        {
            get
            {
                //this part is used to check if the event has past current date
                DateTime date = DateTime.ParseExact(EndDateTime, "yyyy/MM/dd", null);

                //if the event is earlier than today, market it as done
                return DateTime.Compare(date, DateTime.Now) < 0;
            }
        }

        //check if the name of the event name and the starting date of the event is the same
        public bool Equals(Item other)
        {
            if (other is null)
                return false;
            return EventName == other.EventName && StartDateTime == other.StartDateTime;
        }

        public override bool Equals(object obj) => Equals(obj as Item);
        public override int GetHashCode() => (EventName, StartDateTime).GetHashCode();
    }

    //this model is used for grouping the events by their year and month
    public class ItemGroup : ObservableCollection<Item>
    {
        //long display name for the group, this will be yyyy/MM
        public string Heading { get; private set; }

        public string JumpList
        {
            get
            {
                return Heading.Remove(0, 5);
            }
        }

        //set the heading of the group, this is used to make new groups
        public ItemGroup(string heading)
        {
            Heading = heading;
        }

    }
}