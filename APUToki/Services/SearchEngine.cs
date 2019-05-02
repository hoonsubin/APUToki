using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using APUToki.Models;


namespace APUToki.Services
{
    public static class SearchEngine
    {
        //store how long the search took
        public static string LastSearchTime { get; private set; }

        public static int ResultCount { get; private set; }

        /// <summary>
        /// Searches the given database of lectures with the given query
        /// </summary>
        /// <returns>list of results</returns>
        /// <param name="query">Query.</param>
        /// <param name="database">Database.</param>
        public static List<Lecture> SearchLecture(string _query, List<Lecture> database)
        {
            var results = new List<Lecture>();

            //check if the query is not empty
            if (!string.IsNullOrEmpty(_query))
            {
                //make the search query lowercase and trim it
                string query = _query.ToLower().Trim();

                //start counting
                var searchTime = System.Diagnostics.Stopwatch.StartNew();

                var res = database.FindAll(x => x.SearchTags.FirstOrDefault(tag => tag.Contains(query)) != null);

                foreach(var i in res)
                {
                    results.Add(i);
                }

                /*
                //iterate through the lecture database
                foreach (var lecture in database)
                {
                    lecture.SearchTags.Where(x => x.Contains(query));

                    //iterate through the list of tags in the lecture
                    foreach (var searchTag in lecture.SearchTags)
                    {
                        //add the resulting lecture of the tg matches the given query
                        if (searchTag.Contains(query))
                        {
                            results.Add(lecture);
                        }
                    }
                }
                */
                //stop the search timer
                searchTime.Stop();

                //assign the elapsed time to the attribute
                LastSearchTime = searchTime.ElapsedMilliseconds.ToString();

                //get how many items the result has
                ResultCount = results.Count;

                return results;
            }

            //return null if the
            return null;

        }




    }
}
