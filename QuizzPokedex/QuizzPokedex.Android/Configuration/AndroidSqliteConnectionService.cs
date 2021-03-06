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

                //var databaseFolderExt = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                //var databaseFilePathExt = Path.Combine(databaseFolderExt, Android.OS.Environment.DirectoryDownloads);
                //if (!System.IO.File.Exists(databaseFilePathExt + "/" + FileName))
                //{
                //    FileStream fileStreamToWrite = System.IO.File.Create(databaseFilePathExt + "/" + FileName);
                //    using (var stream = System.IO.File.OpenRead(databaseFilePath))
                //    {
                //        stream.Seek(0, SeekOrigin.Begin);
                //        stream.CopyTo(fileStreamToWrite);
                //    }

                //    fileStreamToWrite.Close();
                //}

                _connection = new SQLiteAsyncConnection(databaseFilePath);
            }

            return _connection;
        }
    }
}