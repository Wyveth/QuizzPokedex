using Java.IO;
using QuizzPokedex.Interfaces;
using SQLite;
using System;
using System.IO;
using System.Reflection;

namespace QuizzPokedex.Droid.Configuration
{
    public class AndroidSqliteConnectionService : ISqliteConnectionService
    {
        private const string FileName = "SQlite.QuizzPokedex.db";
        private SQLiteAsyncConnection _connection;

        public SQLiteAsyncConnection GetAsyncConnection()
        {
            //création de la base si existe pas sinon renvoie la bdd courante
            if (_connection == null)
            {
                var databaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var databaseFilePath = Path.Combine(databaseFolder, FileName);

                Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
                Stream embeddedDatabaseStream = assembly.GetManifestResourceStream("QuizzPokedex.SQlite.QuizzPokedex.db");

                if (!System.IO.File.Exists(databaseFilePath))
                {
                    FileStream fileStreamToWrite = System.IO.File.Create(databaseFilePath);
                    embeddedDatabaseStream.Seek(0, SeekOrigin.Begin);
                    embeddedDatabaseStream.CopyTo(fileStreamToWrite);
                    fileStreamToWrite.Close();
                }
                //https://1drv.ms/u/s!AuC6m7Y-ssAPh3v0Km9kRzrmnzNA?e=IX6hu4

                _connection = new SQLiteAsyncConnection(databaseFilePath);
            }

            return _connection;
        }
    }
}