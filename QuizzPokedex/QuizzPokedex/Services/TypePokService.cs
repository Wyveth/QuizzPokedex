using Android.Content.Res;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class TypePokService : ITypePokService
    {
        #region Properties
        private const int _downloadImageTimeoutInSeconds = 15;

        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        private readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };
        #endregion

        #region Constructor
        public TypePokService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region Get Data
        public async Task<List<TypePok>> GetAllAsync()
        {
            var result = await _database.Table<TypePok>().ToListAsync();
            return result;
        }

        public async Task<List<TypePok>> GetTypesAsync(string types)
        {
            string[] vs = types.Split(',');
            List<TypePok> result = new List<TypePok>();
            foreach (var item in vs)
            {
                TypePok typePok = await GetByIdAsync(item);
                if(typePok != null)
                    result.Add(typePok);
            }

            return result;
        }
        public async Task<TypePok> GetByIdAsync(string identifiant)
        {
            int id = int.Parse(identifiant);
            var result = await _database.Table<TypePok>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<TypePok> GetByNameAsync(string libelle)
        {
            var result = await _database.Table<TypePok>().ToListAsync();
            return result.Find(m => m.Name.Equals(libelle));
        }

        public async Task<string> GetBackgroundColorType(string libelle)
        {
            var result = await _database.Table<TypePok>().ToListAsync();
            return result.Find(m => m.Name.Equals(libelle)).TypeColor;
        }

        public async Task<int> GetNumberTypeJsonAsync()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("TypeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<TypeJson>>(json).Count);
        }

        public async Task<int> GetNumberInDbAsync()
        {
            var result = await _database.Table<TypePok>().CountAsync();
            return result;
        }
        #endregion

        #region CRUD
        public async Task<int> CreateAsync(TypePok typePok)
        {
            var result = await _database.InsertAsync(typePok);
            return result;
        }
        #endregion
        #endregion

        #region Populate Database
        public async Task<List<TypeJson>> GetListTypeScrapJson()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("TypeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<TypeJson>>(json));
        }

        public async Task Populate(int nbTypePokInDb, List<TypeJson> typesJson)
        {
            if (!nbTypePokInDb.Equals(typesJson.Count))
            {
                int countTypeJson = 0;
                foreach (TypeJson typeJson in typesJson)
                {
                    countTypeJson++;

                    if (countTypeJson > nbTypePokInDb)
                    {
                        TypePok type = await ConvertTypePokJsonInTypePok(typeJson);
                        _ = CreateAsync(type);

                        Console.WriteLine("Creation Type: " + type.Name);
                    }
                }
            }
        }

        public async Task<TypePok> ConvertTypePokJsonInTypePok(TypeJson typeJson)
        {
            TypePok type = new TypePok();
            type.Name = typeJson.Name;
            type.UrlMiniGo = typeJson.UrlMiniGo;
            type.DataMiniGo = await DownloadImageAsync(typeJson.UrlMiniGo);
            type.UrlFondGo = typeJson.UrlFondGo;
            type.DataFondGo = await DownloadImageAsync(typeJson.UrlFondGo);
            type.UrlMiniHome = typeJson.UrlMiniHome;
            type.DataMiniHome = await DownloadImageAsync(typeJson.UrlMiniHome);
            type.UrlIconHome = typeJson.UrlIconHome;
            type.DataIconHome = await DownloadImageAsync(typeJson.UrlIconHome);
            type.UrlAutoHome = typeJson.UrlAutoHome;
            type.DataAutoHome = await DownloadImageAsync(typeJson.UrlAutoHome);
            type.ImgColor = typeJson.imgColor;
            type.InfoColor = typeJson.infoColor;
            type.TypeColor = typeJson.typeColor;
            return type;
        }

        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                using (var httpResponse = await _httpClient.GetAsync(imageUrl))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return await httpResponse.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        //Url is Invalid
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                //Handle Exception
                throw new Exception(e.Message);
            }
        }
        #endregion
    }
}
