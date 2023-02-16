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

        public async Task<PokemonWeakness> GetByIdAsync(int id)
        {
            var result = await _database.Table<PokemonWeakness>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<List<PokemonWeakness>> GetWeaknessesByPokemon(int pokemonId)
        {
            var result = await _database.Table<PokemonWeakness>().ToListAsync();
            return result.FindAll(m => m.PokemonId.Equals(pokemonId));
        }

        public async Task<List<PokemonWeakness>> GetPokemonsByWeakness(int typePokId)
        {
            var result = await _database.Table<PokemonWeakness>().ToListAsync();
            return result.FindAll(m => m.TypePokId.Equals(typePokId));
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
