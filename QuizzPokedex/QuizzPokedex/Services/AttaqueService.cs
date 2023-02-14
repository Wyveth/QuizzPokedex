using Android.Content.Res;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class AttaqueService : IAttaqueService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private readonly ITypePokService _typePokService;
        private readonly ITypeAttaqueService _typeAttaqueService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public AttaqueService(ISqliteConnectionService connectionService, ITypePokService typePokService, ITypeAttaqueService typeAttaqueService)
        {
            _connectionService = connectionService;
            _typePokService = typePokService;
            _typeAttaqueService = typeAttaqueService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<Attaque>> GetAllAsync()
        {
            var result = await _database.Table<Attaque>().ToListAsync();
            return result;
        }

        public async Task<List<Attaque>> GetAttaquesAsync(string attaques)
        {
            string[] vs = attaques.Split(',');
            List<Attaque> result = new List<Attaque>();
            foreach (var item in vs)
            {
                int id = int.Parse(item);
                Attaque attaque = await GetByIdAsync(id);
                if (attaque != null)
                    result.Add(attaque);
            }

            return result;
        }
        public async Task<Attaque> GetByIdAsync(int id)
        {
            var result = await _database.Table<Attaque>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<Attaque> GetByNameAsync(string libelle)
        {
            var result = await _database.Table<Attaque>().ToListAsync();
            return result.Find(m => m.Name.Equals(libelle));
        }

        public async Task<int> GetNumberInDbAsync()
        {
            var result = await _database.Table<Attaque>().CountAsync();
            return result;
        }
        #endregion

        public async Task<int> CreateAsync(Attaque attaque)
        {
            var result = await _database.InsertAsync(attaque);
            return result;
        }
        #endregion
        #endregion

        #region Populate Database
        public async Task<List<AttaqueJson>> GetListAttaqueScrapJson()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("AttaqueScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<AttaqueJson>>(json));
        }

        public async Task Populate(int nbAttaqueInDb, List<AttaqueJson> attaquesJson)
        {
            if (!nbAttaqueInDb.Equals(attaquesJson.Count))
            {
                int countAttaqueJson = 0;
                foreach (AttaqueJson attaqueJson in attaquesJson)
                {
                    countAttaqueJson++;

                    if (countAttaqueJson > nbAttaqueInDb)
                    {
                        Attaque attaque = await ConvertAttaqueJsonInAttaque(attaqueJson);
                        await CreateAsync(attaque);

                        Console.WriteLine("Creation Attaque: " + attaque.Name);
                    }
                }
            }
        }

        public async Task<Attaque> ConvertAttaqueJsonInAttaque(AttaqueJson attaqueJson)
        {
            Attaque attaque = new Attaque();
            attaque.Name = attaqueJson.Name;
            attaque.Description = attaqueJson.Description;
            attaque.Power = attaqueJson.Power;
            attaque.Precision = attaqueJson.Precision;
            attaque.PP = attaqueJson.PP;
            TypeAttaque typeAttaque = await _typeAttaqueService.GetByNameAsync(attaqueJson.TypeAttaque);
            attaque.TypeAttaqueId = typeAttaque.Id;
            TypePok typePok = await _typePokService.GetByNameAsync(attaqueJson.TypePok);
            attaque.TypePokId = typePok.Id;
            return attaque;
        }
        #endregion
    }
}
