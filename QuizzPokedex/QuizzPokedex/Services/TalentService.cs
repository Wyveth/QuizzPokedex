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
    public class TalentService : ITalentService
    {
        #region Fields
        private const int _downloadImageTimeoutInSeconds = 15;

        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        private readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };
        #endregion

        #region Constructor
        public TalentService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<Talent>> GetAllAsync()
        {
            var result = await _database.Table<Talent>().ToListAsync();
            return result;
        }

        public async Task<List<Talent>> GetTalentsAsync(string talents)
        {
            string[] vs = talents.Split(',');
            List<Talent> result = new List<Talent>();
            foreach (var item in vs)
            {
                int id = int.Parse(item);
                Talent talent = await GetByIdAsync(id);
                if (talent != null)
                    result.Add(talent);
            }

            return result;
        }
        public async Task<Talent> GetByIdAsync(int id)
        {
            var result = await _database.Table<Talent>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<Talent> GetByNameAsync(string libelle)
        {
            var result = await _database.Table<Talent>().ToListAsync();
            return result.Find(m => m.Name.Equals(libelle));
        }
        #endregion

        public async Task<int> CreateAsync(Talent talent)
        {
            var result = await _database.InsertAsync(talent);
            return result;
        }
        #endregion

        public async Task<Talent> GetTalentRandom()
        {
            List<Talent> result = await GetAllAsync();

            Random random = new Random();
            int numberRandom = random.Next(result.Count);

            return await Task.FromResult(result[numberRandom]);
        }

        public async Task<Talent> GetTalentRandom(List<Talent> alreadySelected)
        {
            List<Talent> result = await GetAllAsync();

            Random random = new Random();
            int numberRandom = random.Next(result.Count);
            Talent talent = alreadySelected.Find(m => m.Id.Equals(result[numberRandom].Id));

            while (talent != null)
            {
                numberRandom = random.Next(result.Count);
                talent = alreadySelected.Find(m => m.Id.Equals(result[numberRandom].Id));
            }

            return await Task.FromResult(result[numberRandom]);
        }
        #endregion
    }
}
