using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Android.Resource;

namespace QuizzPokedex.Services
{
    public class PokemonAttaqueService : IPokemonAttaqueService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public PokemonAttaqueService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<PokemonAttaque>> GetAllAsync()
        {
            var result = await _database.Table<PokemonAttaque>().ToListAsync();
            return result;
        }

        public async Task<List<PokemonAttaque>> GetPokemonAttaquesAsync(string pokemonAttaques)
        {
            string[] vs = pokemonAttaques.Split(',');
            List<PokemonAttaque> result = new List<PokemonAttaque>();
            foreach (var item in vs)
            {
                int id = int.Parse(item);
                PokemonAttaque pokemonattaque = await GetByIdAsync(id);
                if (pokemonattaque != null)
                    result.Add(pokemonattaque);
            }

            return result;
        }

        public async Task<List<PokemonAttaque>> GetAttaquesByPokemonTypeLearnAsync(int pokemonId, string typeLearn)
        {
            var result = await _database.Table<PokemonAttaque>().ToListAsync();
            return result.FindAll(m => m.PokemonId.Equals(pokemonId) && m.TypeLearn.Equals(typeLearn));
        }
        
        public async Task<PokemonAttaque> GetByIdAsync(int id)
        {
            var result = await _database.Table<PokemonAttaque>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }
        #endregion

        public async Task<int> CreateAsync(PokemonAttaque pokemonAttaque)
        {
            var result = await _database.InsertAsync(pokemonAttaque);
            return result;
        }

        
        #endregion
        #endregion
    }
}
