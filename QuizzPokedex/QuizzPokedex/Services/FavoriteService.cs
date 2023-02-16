using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class FavoriteService : IFavoriteService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;
        private readonly IPokemonTypePokService _pokemonTypePokService;
        private readonly IProfileService _profileService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public FavoriteService(ISqliteConnectionService connectionService, IPokemonService pokemonService, ITypePokService typePokService, IPokemonTypePokService pokemonTypePokService, IProfileService profileService)
        {
            _connectionService = connectionService;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
            _pokemonTypePokService = pokemonTypePokService;
            _profileService = profileService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<Favorite> GetFavorite(Pokemon pokemon)
        {
            Profile profile = await _profileService.GetProfileActivatedAsync();
            return await _database.Table<Favorite>().Where(m => m.ProfileID.Equals(profile.Id) && m.PokemonID.Equals(pokemon.Id)).FirstOrDefaultAsync();
        }

        public async Task<List<Pokemon>> GetAllByProfileAsync(string filter, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, bool steel, bool fighting, bool dragon, bool water, bool electric, bool fairy, bool fire, bool ice, bool bug, bool normal, bool grass, bool poison, bool psychic, bool rock, bool ground, bool ghost, bool dark, bool flying, bool descending)
        {
            Profile profile = await _profileService.GetProfileActivatedAsync();
            IEnumerable<Favorite> favorites = await _database.Table<Favorite>().Where(m => m.ProfileID.Equals(profile.Id)).ToListAsync();
            List<Pokemon> result = new List<Pokemon>();

            foreach (Favorite favorite in favorites)
                result.Add(await _pokemonService.GetByIdAsync(favorite.PokemonID));

            List<Pokemon> resultFilter = new List<Pokemon>();
            List<Pokemon> resultFilterGen = new List<Pokemon>();
            List<Pokemon> resultFilterType = new List<Pokemon>();

            resultFilterGen = await GetPokemonsWithFilterGen(result, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus);

            resultFilterType = await GetPokemonsWithFilterType(resultFilterGen, steel, fighting, dragon, water, electric, fairy, fire, ice, bug, normal, grass, poison, psychic, rock, ground, ghost, dark, flying);

            if (!string.IsNullOrEmpty(filter) && resultFilterType.Count != 0)
                resultFilter = resultFilterType.FindAll(m => m.Name.ToLowerInvariant().Contains(filter) || m.Number.ToLowerInvariant().Contains(filter));
            else if (!string.IsNullOrEmpty(filter) && resultFilterType.Count == 0)
                resultFilter = result.FindAll(m => m.Name.ToLowerInvariant().Contains(filter) || m.Number.ToLowerInvariant().Contains(filter));
            else if (string.IsNullOrEmpty(filter) && resultFilterType.Count != 0)
                resultFilter = resultFilterType;
            else if (string.IsNullOrEmpty(filter) && resultFilterType.Count == 0)
                resultFilter = result;

            if (!descending)
                return resultFilter.Distinct()
            .OrderBy(x => int.Parse(x.Number))
            .ThenBy(x => x.Number)
            .ToList();
            else
                return resultFilter.Distinct()
            .OrderByDescending(x => int.Parse(x.Number))
            .ThenBy(x => x.Number)
            .ToList();
        }
        #endregion

        public async Task<int> CreateAsync(Favorite favorite)
        {
            Profile profile = await _profileService.GetProfileActivatedAsync();
            favorite.ProfileID = profile.Id;
            return await _database.InsertAsync(favorite);
        }

        public async Task<int> DeleteAsync(Favorite favorite)
        {
            var result = await _database.DeleteAsync(favorite);
            return result;
        }

        public async Task<int> UpdateAsync(Favorite favorite)
        {
            var result = await _database.InsertOrReplaceAsync(favorite);
            return result;
        }
        #endregion

        #region Check
        public async Task<bool> CheckIfFavoriteExist(Pokemon pokemon)
        {
            Favorite favorite = await GetFavorite(pokemon);

            if (favorite == null)
                return await Task.FromResult(false);
            else
                return await Task.FromResult(true);
        }
        #endregion
        #endregion

        #region Private Methods
        private async Task<List<Pokemon>> GetPokemonsWithFilterGen(List<Pokemon> result, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus)
        {
            List<Pokemon> resultFilterGen = new List<Pokemon>();

            if (gen1)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(1)));
            if (gen2)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(2)));
            if (gen3)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(3)));
            if (gen4)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(4)));
            if (gen5)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(5)));
            if (gen6)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(6)));
            if (gen7)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(7)));
            if (gen8)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(8)));
            if (gen9)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(9)));
            if (genArceus)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(0)));

            if (resultFilterGen.Count.Equals(0))
                resultFilterGen = result;

            return await Task.FromResult(resultFilterGen);
        }

        private async Task<List<Pokemon>> GetPokemonsWithFilterType(List<Pokemon> resultFilterGen, bool steel, bool fighting, bool dragon, bool water, bool electric, bool fairy, bool fire, bool ice, bool bug, bool normal, bool grass, bool poison, bool psychic, bool rock, bool ground, bool ghost, bool dark, bool flying)
        {
            List<Pokemon> resultFilterType = new List<Pokemon>();
            if (steel)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Steel));

            if (fighting)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Fighting));

            if (dragon)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Dragon));

            if (water)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Water));

            if (electric)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Electric));

            if (fairy)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Fairy));

            if (fire)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Fire));

            if (ice)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Ice));

            if (bug)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Bug));

            if (normal)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Normal));

            if (grass)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Grass));

            if (poison)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Poison));

            if (psychic)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Psychic));

            if (rock)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Rock));

            if (ground)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Ground));

            if (ghost)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Ghost));

            if (dark)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Dark));

            if (flying)
                resultFilterType.AddRange(await GetPokemonByFilterType(resultFilterGen, Constantes.Flying));

            if (resultFilterType.Count.Equals(0))
                resultFilterType = resultFilterGen;

            return await Task.FromResult(resultFilterType);
        }

        private async Task<List<Pokemon>> GetPokemonByFilterType(List<Pokemon> resultFilterGen, string typeName)
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            TypePok typePok = await _typePokService.GetByNameAsync(typeName);
            List<PokemonTypePok> pokemonTypePoks = await _pokemonTypePokService.GetPokemonsByTypePok(typePok.Id);
            foreach (PokemonTypePok pokemonTypePok in pokemonTypePoks)
            {
                Pokemon pokemon = resultFilterGen.Find(m => m.Id.Equals(pokemonTypePok.PokemonId));
                if (pokemon != null)
                    pokemons.Add(pokemon);
            }
            return pokemons;
        }
        #endregion
    }
}
