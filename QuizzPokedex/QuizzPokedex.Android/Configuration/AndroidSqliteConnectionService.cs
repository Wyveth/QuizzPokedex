using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuizzPokedex.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                var databaseFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var databaseFilePath = Path.Combine(databaseFolder, FileName);

                _connection = new SQLiteAsyncConnection(databaseFilePath);
            }

            return _connection;
        }
    }
}