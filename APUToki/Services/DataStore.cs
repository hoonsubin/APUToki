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
            database.CreateTableAsync<Item>().Wait();

            //create the lecture database
            database.CreateTableAsync<LectureItem>().Wait();
        }

        //get all the items in the database, it will return the result as a list
        public Task<List<Item>> GetItemsAsync()
        {
            Debug.WriteLine("[DataStore]Getting items from database");
            return database.Table<Item>().ToListAsync();
        }

        public Task<List<Item>> SortListByDate()
        {
            Debug.WriteLine("[DataStore]Getting sorted items list");
            return database.QueryAsync<Item>("SELECT * FROM [Item] ORDER BY [StartDateTime] ASC");
        }

        //return the item from the database with the given id
        public Task<Item> GetItemAsync(int id)
        {
            Debug.WriteLine("[DataStore]Getting item with the ID " + id);
            return database.Table<Item>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        //save the given item to the database
        public Task<int> SaveItemAsync(Item item)
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
        public Task<int> DeleteItemAsync(Item item)
        {
            Debug.WriteLine("[DataStore]Deleting item " + item.EventName + " ID: " + item.Id);
            return database.DeleteAsync(item);
        }

        //################################Lecture Items#################################

        //get all the lectures in the database, it will return the result as a list
        public Task<List<LectureItem>> GetLecturesAsync()
        {
            Debug.WriteLine("[DataStore]Getting lectures from database");
            return database.Table<LectureItem>().ToListAsync();
        }

        public Task<List<LectureItem>> SortByLectureName()
        {
            Debug.WriteLine("[DataStore]Getting sorted items list");
            return database.QueryAsync<LectureItem>("SELECT * FROM [LectureItem] ORDER BY [SubjectNameEN] ASC");
        }

        //return the item from the database with the given id
        public Task<LectureItem> GetLectureByIdAsync(int id)
        {
            Debug.WriteLine("[DataStore]Getting item with the ID " + id);
            return database.Table<LectureItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        //save the given item to the database
        public Task<int> SaveLectureAsync(LectureItem lecture)
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
        public Task<int> DeleteLectureAsync(LectureItem lecture)
        {
            Debug.WriteLine("[DataStore]Deleting item " + lecture.SubjectNameEN + " ID: " + lecture.Id);
            return database.DeleteAsync(lecture);
        }

    }
}
