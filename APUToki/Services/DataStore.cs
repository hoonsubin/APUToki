using System.Threading.Tasks;
using System.Collections.Generic;
using SQLite;
using APUToki.Models;

using System.Linq;
using System.Diagnostics;

namespace APUToki.Services
{
    public class DataStore
    {
        readonly SQLiteAsyncConnection database;

        public DataStore(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            //keep the database open so we don't have to re-open or close everytime we do something to it
            database.CreateTableAsync<AcademicEvent>().Wait();

            //create the lecture database
            database.CreateTableAsync<Lecture>().Wait();
        }

        #region Academic Calendar controls
        //get all the items in the database, it will return the result as a list
        public Task<List<AcademicEvent>> GetItemsAsync()
        {
            Debug.WriteLine("[DataStore]Getting items from database");
            return database.Table<AcademicEvent>().ToListAsync();
        }

        public Task<List<AcademicEvent>> SortListByDate()
        {
            Debug.WriteLine("[DataStore]Getting sorted items list");
            return database.QueryAsync<AcademicEvent>("SELECT * FROM [AcademicEvent] ORDER BY [StartDateTime] ASC");
        }

        //return the item from the database with the given id
        public Task<AcademicEvent> GetItemAsync(int id)
        {
            Debug.WriteLine("[DataStore]Getting item with the ID " + id);
            return database.Table<AcademicEvent>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        //save the given item to the database
        public Task<int> SaveItemAsync(AcademicEvent item)
        {
            if (item.Id != 0)
            {
                //if the id is not 0, update the item with the same id
                Debug.WriteLine("[DataStore]Updating item " + item.EventName + " - " + item.StartDateTime + " ID: " + item.Id);
                return database.UpdateAsync(item);
            }

            Debug.WriteLine("[DataStore]Inserting new item " + item.EventName + " - " + item.StartDateTime);
            return database.InsertAsync(item);
        }

        //delete the given item from the database
        public Task<int> DeleteItemAsync(AcademicEvent item)
        {
            Debug.WriteLine("[DataStore]Deleting item " + item.EventName + " ID: " + item.Id);
            return database.DeleteAsync(item);
        }
        #endregion

        #region Lecture controls
        //get all the lectures in the database, it will return the result as a list
        public Task<List<Lecture>> GetLecturesAsync()
        {
            Debug.WriteLine("[DataStore]Getting lectures from database");
            return database.Table<Lecture>().ToListAsync();
        }

        //sort the database list by it English lecture name
        public Task<List<Lecture>> SortByLectureName()
        {
            Debug.WriteLine("[DataStore]Getting sorted items list");
            return database.QueryAsync<Lecture>("SELECT * FROM [Lecture] ORDER BY [SubjectNameEN] ASC");
        }

        //return the item from the database with the given id
        public Task<Lecture> GetLectureByIdAsync(int id)
        {
            Debug.WriteLine("[DataStore]Getting item with the ID " + id);
            return database.Table<Lecture>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        //save the given item to the database
        public Task<int> SaveLectureAsync(Lecture lecture)
        {
            if (lecture.Id != 0)
            {
                //if the id is not 0, update the item with the same id
                Debug.WriteLine("[DataStore]Updating item " + lecture.SubjectNameEN + " - " + lecture.InstructorEN + " ID: " + lecture.Id);
                return database.UpdateAsync(lecture);
            }

            Debug.WriteLine("[DataStore]Inserting new item " + lecture.SubjectNameEN + " - " + lecture.SubjectNameEN);
            return database.InsertAsync(lecture);
        }

        //delete the given item from the database
        public Task<int> DeleteLectureAsync(Lecture lecture)
        {
            Debug.WriteLine("[DataStore]Deleting item " + lecture.SubjectNameEN + " ID: " + lecture.Id);
            return database.DeleteAsync(lecture);
        }

        #endregion

    }
}
