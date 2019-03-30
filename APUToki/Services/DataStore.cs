using System.Threading.Tasks;
using System.Collections.Generic;
using SQLite;
using APUToki.Models;
using SQLiteNetExtensionsAsync.Extensions;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System;

namespace APUToki.Services
{
    public class DataStore
    {
        readonly SQLiteAsyncConnection _database;

        public DataStore()
        {
            _database = CreateAsyncConnection();

            //create database table
            _database.CreateTableAsync<AcademicEvent>().Wait();
            _database.CreateTableAsync<Lecture>().Wait();
            _database.CreateTableAsync<TimetableCell>().Wait();

        }

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
        //get all the items in the database, it will return the result as a list
        public Task<List<AcademicEvent>> GetItemsAsync()
        {
            Debug.WriteLine("[DataStore]Getting items from database");
            return _database.Table<AcademicEvent>().ToListAsync();
        }

        public Task<List<AcademicEvent>> SortListByDate()
        {
            Debug.WriteLine("[DataStore]Getting sorted items list");
            return _database.QueryAsync<AcademicEvent>("SELECT * FROM [AcademicEvents] ORDER BY [StartDateTime] ASC");
        }

        //return the item from the database with the given id
        public Task<AcademicEvent> GetItemAsync(int id)
        {
            Debug.WriteLine("[DataStore]Getting item with the ID " + id);
            return _database.Table<AcademicEvent>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        //save the given item to the database
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

        //delete the given item from the database
        public Task<int> DeleteItemAsync(AcademicEvent item)
        {
            Debug.WriteLine("[DataStore]Deleting item " + item.EventName + " ID: " + item.Id);
            return _database.DeleteAsync(item);
        }
        #endregion

        #region Lecture controls
        //get all the lectures in the database, it will return the result as a list
        public Task<List<Lecture>> GetAllLecturesAsync()
        {
            Debug.WriteLine("[DataStore]Getting lectures from database");
            //return _database.Table<Lecture>().ToListAsync();
            return _database.GetAllWithChildrenAsync<Lecture>();
        }

        public Task<List<TimetableCell>> GetAllTimetableCellsAsync()
        {
            Debug.WriteLine("[Datastore]Getting all timetable cells from the database");
            return _database.GetAllWithChildrenAsync<TimetableCell>();
        }

        public Task SaveAllLecturesAsync(List<Lecture> lectures)
        {
            //Console.WriteLine("[DataStore]Saving all the lectures from the list");
            return _database.InsertOrReplaceAllWithChildrenAsync(lectures, true);
        }

        //save the given item to the database
        public Task SaveLectureAsync(Lecture lecture)
        {
            Debug.WriteLine("[DataStore]Inserting new item " + lecture.SubjectNameEN + " by " + lecture.InstructorEN);
            return _database.InsertOrReplaceWithChildrenAsync(lecture, recursive: true);
        }

        //save the given timetable to the database
        public Task SaveTimetableCellAsync(TimetableCell cell)
        {
            Debug.WriteLine("[DataStore]Inserting new item " + cell.Period + " - " + cell.DayOfWeek);
            return _database.InsertOrReplaceWithChildrenAsync(cell, recursive: true);
        }

        //delete the given item from the database
        public Task DeleteLectureAsync(Lecture lecture)
        {
            if (lecture.TimetableCells.Count > 0)
            {
                Debug.WriteLine("[DataStore]Deleting timecell...");
                _database.DeleteAllAsync(lecture.TimetableCells, true);
            }
            Debug.WriteLine("[DataStore]Deleting item " + lecture.SubjectNameEN + " ID: " + lecture.Id);
            return _database.DeleteAsync(lecture, true);
        }

        public Task DeleteAllLecturesAsync(List<Lecture> lectures)
        {
            Debug.WriteLine("[DataStore]Deleting all lectures ");
            return _database.DeleteAllAsync(lectures, true);
        }

        public Task DeleteTimetableCellAsync(TimetableCell cell)
        {

            return _database.DeleteAsync(cell, true);
        }
        #endregion

    }
}
