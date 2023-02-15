using Android.Content.Res;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class TypePokService : ITypePokService
    {
        #region Fields
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
        #region CRUD
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
                int id = int.Parse(item);
                TypePok typePok = await GetByIdAsync(id);
                if (typePok != null)
                    result.Add(typePok);
            }

            return result;
        }
        public async Task<TypePok> GetByIdAsync(int id)
        {
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

        public async Task<int> GetNumberJsonAsync()
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

        public async Task<int> CreateAsync(TypePok typePok)
        {
            var result = await _database.InsertAsync(typePok);
            return result;
        }
        #endregion

        public async Task<TypePok> GetTypeRandom()
        {
            List<TypePok> result = await GetAllAsync();

            Random random = new Random();
            int numberRandom = random.Next(result.Count);

            return await Task.FromResult(result[numberRandom]);
        }

        public async Task<TypePok> GetTypeRandom(List<TypePok> alreadySelected)
        {
            List<TypePok> result = await GetAllAsync();

            Random random = new Random();
            int numberRandom = random.Next(result.Count);
            TypePok typePok = alreadySelected.Find(m => m.Id.Equals(result[numberRandom].Id));

            while (typePok != null)
            {
                numberRandom = random.Next(result.Count);
                typePok = alreadySelected.Find(m => m.Id.Equals(result[numberRandom].Id));
            }

            return await Task.FromResult(result[numberRandom]);
        }
        #endregion

        #region Populate Database
        public async Task<List<TypeJson>> GetListScrapJson()
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
                        await CreateAsync(type);

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
            type.PathMiniGo = await DownloadFile(typeJson.UrlMiniGo, "Image/TypePok/MiniGo", typeJson.Name.Replace(" ", "_") + Constantes.ExtensionImage);
            type.DataMiniGo = await DownloadImageAsync(typeJson.UrlMiniGo);
            type.UrlFondGo = typeJson.UrlFondGo;
            type.PathFondGo = await DownloadFile(typeJson.UrlFondGo, "Image/TypePok/FondGo", typeJson.Name.Replace(" ", "_") + Constantes.ExtensionImage);
            type.DataFondGo = await DownloadImageAsync(typeJson.UrlFondGo);
            type.UrlMiniHome = typeJson.UrlMiniHome;
            type.PathMiniHome = await DownloadFile(typeJson.UrlMiniHome, "Image/TypePok/MiniHome", typeJson.Name.Replace(" ", "_") + Constantes.ExtensionImage);
            type.DataMiniHome = await DownloadImageAsync(typeJson.UrlMiniHome);
            type.UrlIconHome = typeJson.UrlIconHome;
            type.PathIconHome = await DownloadFile(typeJson.UrlIconHome, "Image/TypePok/IconHome", typeJson.Name.Replace(" ", "_") + Constantes.ExtensionImage);
            type.DataIconHome = await DownloadImageAsync(typeJson.UrlIconHome);
            type.UrlAutoHome = typeJson.UrlAutoHome;
            type.PathAutoHome = await DownloadFile(typeJson.UrlAutoHome, "Image/TypePok/AutoHome", typeJson.Name.Replace(" ", "_") + Constantes.ExtensionImage);
            type.DataAutoHome = await DownloadImageAsync(typeJson.UrlAutoHome);
            type.ImgColor = typeJson.imgColor;
            type.InfoColor = typeJson.infoColor;
            type.TypeColor = typeJson.typeColor;
            return type;
        }

        public async Task<byte[]> DownloadImageAsync(string UrlImg)
        {
            try
            {
                using (var httpResponse = await _httpClient.GetAsync(UrlImg))
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
            catch (System.Exception ex)
            {
                return null;
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
