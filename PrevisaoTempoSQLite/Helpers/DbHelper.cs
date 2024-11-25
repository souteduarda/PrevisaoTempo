using PrevisaoTempoSQLite.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrevisaoTempoSQLite.Helpers
{
    public class DbHelper
    {
        readonly SQLiteAsyncConnection conn;

        public DbHelper(string path)
        {
            conn = new SQLiteAsyncConnection(path);
            conn.CreateTableAsync<Weather>().Wait();

        }

        public Task<int> Insert(Weather weather) {
            return conn.InsertAsync(weather);
        }

        public Task<int> Delete(int id) {
            return conn.Table<Weather>().DeleteAsync(i => i.Id == id);
        }

        public Task<int> DeleteAll() {
            return conn.DeleteAllAsync<Weather>();
        }

        public Task<List<Weather>> GetAll()
        {
            return conn.Table<Weather>().ToListAsync();
        }

        public Task<List<Weather>> Search(string query) {
            string sql = "SELECT * FROM Weather WHERE city LIKE '%"+ query +"%'";

            return conn.QueryAsync<Weather>(sql);
        }
    }
}
