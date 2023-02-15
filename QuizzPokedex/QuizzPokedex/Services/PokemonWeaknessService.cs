using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class PokemonWeaknessService : IPokemonWeaknessService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public PokemonWeaknessService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<PokemonWeakness>> GetAllAsync()
        {
            var result = await _database.Table<PokemonWeakness>().ToListAsync();
            return result;
        }

        public async Task<List<PokemonWeakness>> GetPokemonWeaknessesAsync(string pokemonWeaknesses)
        {
            string[] vs = pokemonWeaknesses.Split(',');
            List<PokemonWeakness> result = new List<PokemonWeakness>();
            foreach (var item in vs)
            {
                int id = int.Parse(item);
                PokemonWeakness pokemonWeakness = await GetByIdAsync(id);
                if (pokemonWeakness != null)
                    result.Add(pokemonWeakness);
            }

            return result;
        }
        public async Task<PokemonWeakness> GetByIdAsync(int id)
        {
            var result = await _database.Table<PokemonWeakness>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }
        #endregion

        public async Task<int> CreateAsync(PokemonWeakness pokemonWeakness)
        {
            var result = await _database.InsertAsync(pokemonWeakness);
            return result;
        }
        #endregion
        #endregion
    }
}
