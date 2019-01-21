using System;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using APUToki.Models;

//don't forget to change the namespace and the using libraries when implementing this to the app
namespace APUToki.Services
{
    /// <summary>
    /// A class for accessing the APU Academic Office homepage and get information
    /// </summary>
    public static class ApuBot
    {

        //the delimiter for dividing the cells
        const char delimiter = '|';

        //link for english academic calendar
        const string enAcademicOfficeUri = "http://en.apu.ac.jp/academic/top/curriculum_17.html/?c=17";

        /// <summary>
        /// Get all the links found in the Academic Office menu as a string
        /// </summary>
        /// <returns>The links from page.</returns>
        /// <param name="menu">Menu.</param>
        public static List<string> GetLinksFromPage(string menu = "01")
        {

            //Link of the Academic Office homepage, this will be the starting location=
            string uri = enAcademicOfficeUri;

            //XPath syntax for searching the Academic Office page menus
            string xpath = $"//div[contains(@class, 'menu_title curriculum{menu}')]";

            //the url that will be outputted
            string outLink;

            //declare a list that will contain all the uris found in the menj
            var uriList = new List<string>();

            try
            {
                //it loads the html document from the given link
                HtmlWeb web = new HtmlWeb();
                var document = web.Load(uri);

                //this defines the tables in the html document, the ancestor of the element using the defined XPath
                var pageBody = document.DocumentNode.SelectSingleNode(xpath);

                //follow the sibliing of the current node div with the given class
                foreach (var ul in pageBody.SelectNodes("following-sibling::ul"))
                {
                    //go throught the list node and get the href and the inner text
                    foreach (var li in ul.SelectNodes("./li/a"))
                    {
                        //get the value of href in the current node
                        outLink = "http://en.apu.ac.jp" + li.Attributes["href"].Value;

                        //add the uri to the list that is going to get returned
                        uriList.Add(outLink);
                    }
                }
                return uriList;
            }
            catch (Exception)
            {

                //if there is no link, or some other problem comes up, output an
                return null;
            }
        }

        /// <summary>
        /// Scrape all the text from the calendar table. This will only scrape the table for content.
        /// </summary>
        /// <returns>The value from table.</returns>
        /// <param name="uri">URI.</param>
        public static List<string> GetValueFromTable(string uri)
        {
            try
            {
                //the xpath expression for defining the elements
                string xpath = "//table[contains(@class, 'fcktable')]/tbody";

                //load the entire html document from the given uri
                var web = new HtmlWeb();
                var document = web.Load(uri);

                //define the tables in the html document, the ancestor of the element
                var tableBody = document.DocumentNode.SelectSingleNode(xpath);

                var calendarItem = new List<string>();

                //check for 404 error in the website and return error
                if (document.DocumentNode.SelectSingleNode("//title").InnerText == "404ページ")
                    throw new HtmlWebException("The page is showing a 404 error");

                string year = string.Empty;

                //loop through the rows of the table
                foreach (HtmlNode tr in tableBody.SelectNodes("./tr")) //this block is showing errors
                {
                    //declare the row content
                    string rowContent = string.Empty;

                    //declare the column variable
                    int col = 0;

                    //loop every columns within a single table
                    foreach (HtmlNode td in tr.SelectNodes("./td"))
                    {
                        //make the current cell the inner text of the element, and get rid of some parts
                        string currentCell = td.InnerText.Trim()
                            .Replace("  ", "")
                            .Replace("&nbsp;", "Empty")
                            .Replace("&darr;", "New Year's Day")
                            .Replace("&rsquo;", "\'")
                            .Replace("\n(Back-up Examination)", "");

                        //check for the first and second column
                        //because the year column is only available in the first and second row
                        if (col == 0 || col == 1)
                        {
                            //change the date column to Date|Month (only applies for the first row
                            if (currentCell == "Date") { currentCell = "Date" + delimiter + "Month"; }
                            //replace the - to a delimiter
                            currentCell = currentCell.Replace('-', delimiter);
                        }

                        //check if it is the first column
                        if (col == 0)
                        {
                            //if the year is already present, change the year var accordingly
                            if (currentCell.Length == 4) { year = currentCell; }
                            //the year is going to be the content of the first column; date-month
                            else { rowContent = year + delimiter; }
                        }

                        //stack the content with the delimiter till the next row
                        rowContent += currentCell + delimiter;

                        //incrment the column
                        col++;
                    }

                    //cut the last delimiter
                    rowContent = rowContent.Remove(rowContent.Length - 1);

                    calendarItem.Add(rowContent);
                }
                return calendarItem;
            }
            catch (Exception e)
            {
                //failed to get the node from the site
                Debug.WriteLine("Error while getting the list " + e);
                return null;
            }
        }

        /// <summary>
        /// Convert the input event string into a more cleaner format for prcessing
        /// </summary>
        /// <returns>The to date time.</returns>
        /// <param name="inputEvent">Input.</param>
        public static string ChangeDateFormat(string inputEvent)
        {
            Dictionary<string, string> monthToNumber = new Dictionary<string, string>
            {
                {"Jan", "01"},
                {"Feb", "02"},
                {"Mar", "03"},
                {"Apr", "04"},
                {"May", "05"},
                {"Jun", "06"},
                {"Jul", "07"},
                {"Aug", "08"},
                {"Sep", "09"},
                {"Oct", "10"},
                {"Nov", "11"},
                {"Dec", "12"}
            };

            //the input string is formatted like this;
            //[year] | [date] | [month] | [day] | [event] | [Description]
            //this will change the format to [year/month/date]|[day of month]|[holiday]

            //split the input string by the delimiter
            string[] acaEvent = inputEvent.Split(delimiter);

            //convert the string to integer, and back to string for formatting
            int intDay = int.Parse(acaEvent[1]);

            //combine the array into a single array for the date
            string joinedDate = acaEvent[0] + "/" + monthToNumber[acaEvent[2]] + "/" + intDay.ToString("00");

            //check if the Event column is empty, and replace it with the next column
            if (acaEvent[4] == "Empty")
            {
                acaEvent[4] = acaEvent[5];
            }
            //mark national holidays into classes as usual
            else if (acaEvent[5].Contains("Classes as usual"))
            {
                acaEvent[4] = acaEvent[4] + "(" + acaEvent[5] + ")";
            }


            //make the final date time string with adding 
            string dateTime = joinedDate + delimiter + acaEvent[3] + delimiter + acaEvent[4];

            return dateTime;

        }

        /// <summary>
        /// Return the list of all academic events from the academic calendar
        /// </summary>
        /// <returns>The event list</returns>
        public static ObservableCollection<Item> AcademicEventList()
        {
            //this method will output the list of items
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            //menu number 1 represents the Academic Calendar
            //iterate through all the links in the menu
            foreach (string i in GetLinksFromPage("01"))
            {
                //get the multiple lists
                foreach (string row in GetValueFromTable(i))
                {
                    //skip to the next iteration if it contains the two characters
                    if (row.Contains("Date") || row.Contains("New Year's Day") && row.Contains("Empty")) { continue; }

                    //feed the finalRow to the ChangeDateFormat method
                    string rowFixed = ChangeDateFormat(row);

                    //convert the string into an array
                    string[] calendarItems = rowFixed.Split(delimiter);

                    //add the item with the given parameters
                    items.Add(new Item
                    {
                        //Id = Guid.NewGuid().ToString(),
                        EventName = calendarItems[2],
                        StartDateTime = calendarItems[0]
                    });
                }
            }
            return items;
        }


    }
}
