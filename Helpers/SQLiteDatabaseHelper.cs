using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAUIAppTempoAgora.Models;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MAUIAppTempoAgora.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _connection;

        //construtor: executa "create table if not exists"
        public SQLiteDatabaseHelper(string path)
        {
            //conexão com o banco de dados (localhost, port, user...)
            _connection = new SQLiteAsyncConnection(path);

            _connection.CreateTableAsync<Tempo>().Wait();
        }

        public Task<int> Insert(Tempo tempo)
        {
            return _connection.InsertAsync(tempo);
        }


        public Task<List<Tempo>> GetAll()
        {
            return _connection.Table<Tempo>().ToListAsync();
        }

    }
}