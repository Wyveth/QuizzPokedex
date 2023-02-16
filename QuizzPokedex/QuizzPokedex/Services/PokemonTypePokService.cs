using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class PokemonTypePokService : IPokemonTypePokService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public PokemonTypePokService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<PokemonTypePok>> GetAllAsync()
        {
            var result = await _database.Table<PokemonTypePok>().ToListAsync();
            return result;
        }

        public async Task<List<PokemonTypePok>> GetPokemonTypePoksAsync(string pokemonTypePoks)
        {
            string[] vs = pokemonTypePoks.Split(',');
            List<PokemonTypePok> result = new List<PokemonTypePok>();
            foreach (var item in vs)
            {
                int id = int.Parse(item);
                PokemonTypePok pokemontypePok = await GetByIdAsync(id);
                if (pokemontypePok != null)
                    result.Add(pokemontypePok);
            }

            return result;
        }
        public async Task<PokemonTypePok> GetByIdAsync(int id)
        {
            var result = await _database.Table<PokemonTypePok>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<List<PokemonTypePok>> GetTypesPokByPokemon(int pokemonId)
        {
            var result = await _database.Table<PokemonTypePok>().ToListAsync();
            return result.FindAll(m => m.PokemonId.Equals(pokemonId));
        }

        public async Task<List<PokemonTypePok>> GetPokemonsByTypePok(int typePokId)
        {
            var result = await _database.Table<PokemonTypePok>().ToListAsync();
            return result.FindAll(m => m.TypePokId.Equals(typePokId));
        }
        #endregion

        public async Task<int> CreateAsync(PokemonTypePok pokemonTypePok)
        {
            var result = await _database.InsertAsync(pokemonTypePok);
            return result;
        }
        #endregion
        #endregion
    }
}
