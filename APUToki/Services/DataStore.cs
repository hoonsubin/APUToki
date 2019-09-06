using System.Threading.Tasks;
using System.Collections.Generic;
using SQLite;
using APUToki.Models;
using SQLiteNetExtensionsAsync.Extensions;
using System.Diagnostics;
using System.IO;
using System;
using Newtonsoft.Json;

namespace APUToki.Services
{
    public class DataStore
    {
        readonly SQLiteAsyncConnection _database;

        public DataStore()
        {
            _database = CreateAsyncConnection();

            //create database tables
            _database.CreateTableAsync<AcademicEvent>().Wait();
            _database.CreateTableAsync<Lecture>().Wait();
            _database.CreateTableAsync<TimetableCell>().Wait();

        }

        /// <summary>
        /// returns where the SQLite database is saved in string directory
        /// </summary>
        public static string DatabaseFilePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db3");
            }
        }

        public static SQLiteAsyncConnection CreateAsyncConnection()
        {
            return new SQLiteAsyncConnection(DatabaseFilePath);
        }

        #region Academic Calendar controls
        /// <summary>
        /// Gets all the academic events async.
        /// </summary>
        /// <returns>List of academic events</returns>
        public Task<List<AcademicEvent>> GetItemsAsync()
        {
            Debug.WriteLine("[DataStore]Getting items from database");
            return _database.Table<AcademicEvent>().ToListAsync();
        }

        /// <summary>
        /// Sorts the list of academic events by date async.
        /// </summary>
        /// <returns>The sorted list by date</returns>
        public Task<List<AcademicEvent>> SortListByDate()
        {
            Debug.WriteLine("[DataStore]Getting sorted items list");
            return _database.QueryAsync<AcademicEvent>("SELECT * FROM [AcademicEvents] ORDER BY [StartDateTime] ASC");
        }

        /// <summary>
        /// Saves the given academic event to the database async
        /// </summary>
        /// <returns>Id of the academic event</returns>
        /// <param name="item">academic event to save</param>
        public Task<int> SaveItemAsync(AcademicEvent item)
        {
            if (item.Id != 0)
            {
                //if the id is not 0, update the item with the same id
                Debug.WriteLine("[DataStore]Updating item " + item.EventName + " - " + item.StartDateTime + " ID: " + item.Id);
                return _database.UpdateAsync(item);
            }

            Debug.WriteLine("[DataStore]Inserting new item " + item.EventName + " - " + item.StartDateTime);
            return _database.InsertAsync(item);
        }

        /// <summary>
        /// Deletes the given academic event from the database async.
        /// </summary>
        /// <returns>The id of the deleted event</returns>
        /// <param name="item">academic event to delete</param>
        public Task<int> DeleteItemAsync(AcademicEvent item)
        {
            Debug.WriteLine("[DataStore]Deleting item " + item.EventName + " ID: " + item.Id);
            return _database.DeleteAsync(item);
        }
        #endregion

        #region Lecture controls
        /// <summary>
        /// Gets all lectures from the database async.
        /// </summary>
        /// <returns>List of lectures</returns>
        public Task<List<Lecture>> GetAllLecturesAsync()
        {
            Debug.WriteLine("[DataStore]Getting lectures from database");
            //return _database.Table<Lecture>().ToListAsync();
            return _database.GetAllWithChildrenAsync<Lecture>();
        }

        /// <summary>
        /// Gets all timetable cells from the database async.
        /// </summary>
        /// <returns>List of timetable cells</returns>
        public Task<List<TimetableCell>> GetAllTimetableCellsAsync()
        {
            Debug.WriteLine("[Datastore]Getting all timetable cells from the database");
            return _database.GetAllWithChildrenAsync<TimetableCell>();
        }

        /// <summary>
        /// Saves all the contents in a list of lectures with the children with ORM async.
        /// </summary>
        /// <returns>All lectures</returns>
        /// <param name="lectures">List of lectures</param>
        public Task SaveAllLecturesAsync(List<Lecture> lectures)
        {
            //todo: this part is showing a bug where the children of the individual lectures are not saving
            Debug.WriteLine("[DataStore]Saving all the lectures from the list");
            return _database.InsertOrReplaceAllWithChildrenAsync(lectures, true);
        }

        /// <summary>
        /// Saves the given lecture and its childrens to the database async.
        /// </summary>
        /// <returns>The lecture async.</returns>
        /// <param name="lecture">Lecture.</param>
        public Task SaveLectureAsync(Lecture lecture)
        {
            Debug.WriteLine("[DataStore]Inserting new item " + lecture.SubjectNameEN + " by " + lecture.InstructorEN);
            return _database.InsertOrReplaceWithChildrenAsync(lecture, recursive: true);
        }

        /// <summary>
        /// Deletes the given lecture from the database async.
        /// </summary>
        /// <returns>deleted lecture</returns>
        /// <param name="lecture">Lecture to delete</param>
        public Task DeleteLectureAsync(Lecture lecture)
        {
            Debug.WriteLine("[DataStore]Deleting item " + lecture.SubjectNameEN + " ID: " + lecture.Id);
            return _database.DeleteAsync(lecture, true);
        }

        /// <summary>
        /// Deletes all the lectures from the database async.
        /// </summary>
        /// <returns>List of deleted lectures</returns>
        /// <param name="lectures">list of Lectures to delete</param>
        public Task DeleteAllLecturesAsync(List<Lecture> lectures)
        {
            Debug.WriteLine("[DataStore]Deleting all lectures");
            return _database.DeleteAllAsync(lectures, true);
        }
        #endregion

        #region JSON Serialization

        /// <summary>
        /// Serializes the input Lecture object to JSON in string.
        /// </summary>
        /// <returns>Serialized json string.</returns>
        /// <param name="lecture">Lecture.</param>
        public string SerializeToJson(object lecture)
        {
            return JsonConvert.SerializeObject(lecture, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        /// <summary>
        /// Deserializes from json to Lecture object.
        /// </summary>
        /// <returns>Lecture object.</returns>
        /// <param name="json">Json.</param>
        public List<TimetableCell> DeserializeTimetableListFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<TimetableCell>>(json);
        }

        /// <summary>
        /// Deserializes from json to Lecture object.
        /// </summary>
        /// <returns>Lecture object.</returns>
        /// <param name="json">Json.</param>
        public List<Lecture> DeserializeLectureListFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<Lecture>>(json);
        }
        #endregion
    }
}
