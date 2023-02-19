using Android.Content.Res;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class TypeAttaqueService : ITypeAttaqueService
    {
        #region Fields
        private const int _downloadImageTimeoutInSeconds = 15;
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        private readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };
        #endregion

        #region Constructor
        public TypeAttaqueService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<TypeAttaque>> GetAllAsync()
        {
            var result = await _database.Table<TypeAttaque>().ToListAsync();
            return result;
        }

        public async Task<List<TypeAttaque>> GetTypesAttaqueAsync(string typesAttaque)
        {
            string[] vs = typesAttaque.Split(',');
            List<TypeAttaque> result = new List<TypeAttaque>();
            foreach (var item in vs)
            {
                int id = int.Parse(item);
                TypeAttaque typeAttaque = await GetByIdAsync(id);
                if (typeAttaque != null)
                    result.Add(typeAttaque);
            }

            return result;
        }
        public async Task<TypeAttaque> GetByIdAsync(int id)
        {
            var result = await _database.Table<TypeAttaque>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<TypeAttaque> GetByNameAsync(string libelle)
        {
            var result = await _database.Table<TypeAttaque>().ToListAsync();
            return result.Find(m => m.Name.Equals(libelle));
        }

        public async Task<int> GetNumberInDbAsync()
        {
            var result = await _database.Table<TypeAttaque>().CountAsync();
            return result;
        }

        public async Task<int> GetNumberJsonAsync()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("TypeAttaqueScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<TypeAttaqueJson>>(json).Count);
        }

        public async Task<int> GetNumberCheckAsync()
        {
            var result = await _database.Table<TypeAttaque>().Where(m => m.Check.Equals(true)).CountAsync();
            return result;
        }
        #endregion

        public async Task<int> CreateAsync(TypeAttaque typeAttaque)
        {
            var result = await _database.InsertAsync(typeAttaque);
            return result;
        }

        public async Task<int> UpdateAsync(TypeAttaque typeAttaque)
        {
            var result = await _database.InsertOrReplaceAsync(typeAttaque);
            return result;
        }

        public async Task ResetNextLaunch()
        {
            List<TypeAttaque> typeAttaques = await GetAllAsync();

            foreach (TypeAttaque typeAttaque in typeAttaques)
            {
                try
                {
                    typeAttaque.Check = false;
                    TypeAttaque typeAttaqueUpdated = typeAttaque;
                    await UpdateAsync(typeAttaqueUpdated);
                }
                catch
                {
                    Debug.Write("Download Error: " + typeAttaque.Name);
                }
            }
        }
        #endregion
        #endregion

        #region Populate Database

        public async Task<List<TypeAttaqueJson>> GetListScrapJson()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("TypeAttaqueScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<TypeAttaqueJson>>(json));
        }

        public async Task Populate(int nbTypeAttaqueInDb, List<TypeAttaqueJson> typesAttaqueJson)
        {
            if (!nbTypeAttaqueInDb.Equals(typesAttaqueJson.Count))
            {
                int countTypeJson = 0;
                foreach (TypeAttaqueJson typeAttaqueJson in typesAttaqueJson)
                {
                    countTypeJson++;

                    if (countTypeJson > nbTypeAttaqueInDb)
                    {
                        TypeAttaque typeAttaque = await ConvertTypeAttaqueJsonInTypeAttaque(typeAttaqueJson);
                        await CreateAsync(typeAttaque);

                        Console.WriteLine("Creation Type Attaque: " + typeAttaque.Name);
                    }
                }
            }
        }

        public async Task CheckIfPictureNotExistDownload(List<TypeAttaqueJson> typesAttaqueJson)
        {
            List<TypeAttaque> typesAttaque = await GetAllAsync();

            foreach (TypeAttaque typeAttaque in typesAttaque)
            {
                TypeAttaqueJson typeAttaqueJson = typesAttaqueJson.Find(m => m.Name.Equals(typeAttaque.Name));

                try
                {
                    Debug.Write("Info: " + typeAttaque.Name);
                    if (!File.Exists(typeAttaque.PathImg))
                    {
                        await DownloadFile(typeAttaque.UrlImg, "Image/TypeAttaque", typeAttaqueJson.Name.Replace(" ", "_") + Constantes.ExtensionImage);
                        Debug.Write("Download Image OK");
                    }

                    typeAttaque.Check = true;

                    TypeAttaque typeAttaqueUpdated = typeAttaque;
                    if (typeAttaqueUpdated != null)
                        await UpdateAsync(typeAttaqueUpdated);
                }
                catch
                {
                    Debug.Write("Download Error: " + typeAttaque.Name);
                }
            }
        }

        public async Task<TypeAttaque> ConvertTypeAttaqueJsonInTypeAttaque(TypeAttaqueJson typeAttaqueJson)
        {
            TypeAttaque typeAttaque = new TypeAttaque();
            typeAttaque.Name = typeAttaqueJson.Name;
            typeAttaque.Description = typeAttaqueJson.Description;
            typeAttaque.UrlImg = typeAttaqueJson.UrlImg;
            typeAttaque.PathImg = await DownloadFile(typeAttaqueJson.UrlImg, "Image/TypeAttaque", typeAttaqueJson.Name.Replace(" ", "_") + Constantes.ExtensionImage);
            return typeAttaque;
        }


        public async Task<string> DownloadFile(string url, string folder, string filename)
        {
            string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), folder);
            Directory.CreateDirectory(pathToNewFolder);

            WebClient webClient = new WebClient();
            string pathToNewFile = Path.Combine(pathToNewFolder, filename);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new
                RemoteCertificateValidationCallback
                (
                   delegate { return true; }
                );
                webClient.DownloadFile(new Uri(url), pathToNewFile);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                webClient.Dispose();
            }

            return await Task.FromResult(pathToNewFile);
        }
        #endregion
    }
}
