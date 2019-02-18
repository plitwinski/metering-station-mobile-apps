using MeteringStation.Mobile.DataAccess.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MeteringStation.Mobile.DataAccess.Databases
{
    internal class MetersDatabase : IMetersDatabase
    {
        private SQLiteAsyncConnection _database;

        protected MetersDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        protected Task InitAsync()
            => _database.CreateTableAsync<RegisteredMeter>();

        public async Task<IEnumerable<RegisteredMeter>> GetMetersAsync()
            => await _database.Table<RegisteredMeter>().ToListAsync();

        public async Task UpsertAsync(RegisteredMeter registeredMeter)
            => await _database.InsertOrReplaceAsync(registeredMeter);

        public async Task<int> RemoveMeterAsync(RegisteredMeter registeredMeter)
            => await _database.DeleteAsync(registeredMeter);


        public static MetersDatabase Create()
        {
            try
            {
                var database = new MetersDatabase(
             Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
             "meters-database.db3"));

                database.InitAsync().Wait();
                return database;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
