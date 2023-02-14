using MvvmCross.Base;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class PokemonTalentService : IPokemonTalentService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public PokemonTalentService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<PokemonTalent>> GetAllAsync()
        {
            var result = await _database.Table<PokemonTalent>().ToListAsync();
            return result;
        }

        public async Task<List<PokemonTalent>> GetPokemonTalentsAsync(string pokemonTalents)
        {
            string[] vs = pokemonTalents.Split(',');
            List<PokemonTalent> result = new List<PokemonTalent>();
            foreach (var item in vs)
            {
                int id = int.Parse(item);
                PokemonTalent pokemontalent = await GetByIdAsync(id);
                if (pokemontalent != null)
                    result.Add(pokemontalent);
            }

            return result;
        }
        public async Task<PokemonTalent> GetByIdAsync(int id)
        {
            var result = await _database.Table<PokemonTalent>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }
        #endregion

        public async Task<int> CreateAsync(PokemonTalent pokemonTalent)
        {
            var result = await _database.InsertAsync(pokemonTalent);
            return result;
        }
        #endregion
        #endregion
    }
}
