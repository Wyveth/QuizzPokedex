using Java.IO;
using QuizzPokedex.Interfaces;
using SQLite;
using System;
using System.IO;

namespace QuizzPokedex.Droid.Configuration
{
    public class AndroidSqliteConnectionService : ISqliteConnectionService
    {
        private const string FileName = "SQlite.QuizzPokedex";
        private SQLiteAsyncConnection _connection;

        public SQLiteAsyncConnection GetAsyncConnection()
        {
            //création de la base si existe pas sinon renvoie la bdd courante
            if (_connection == null)
            {
                var databaseFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                var databaseFilePath = Path.Combine(databaseFolder, FileName);

                _connection = new SQLiteAsyncConnection(databaseFilePath);
            }

            return _connection;
        }
    }
}