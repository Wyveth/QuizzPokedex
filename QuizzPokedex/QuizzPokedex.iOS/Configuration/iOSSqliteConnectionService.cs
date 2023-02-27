using QuizzPokedex.Interfaces;
using SQLite;
using System;
using System.IO;
using System.Reflection;

namespace QuizzPokedex.iOS.Configuration
{
    public class iOSSqliteConnectionService : ISqliteConnectionService
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

                if (!File.Exists(databaseFilePath))
                {
                    FileStream fileStreamToWrite = File.Create(databaseFilePath);
                    embeddedDatabaseStream.Seek(0, SeekOrigin.Begin);
                    embeddedDatabaseStream.CopyTo(fileStreamToWrite);
                    fileStreamToWrite.Close();
                }

                _connection = new SQLiteAsyncConnection(databaseFilePath);
            }

            return _connection;
        }
    }
}