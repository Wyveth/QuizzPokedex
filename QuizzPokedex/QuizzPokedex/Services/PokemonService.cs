using Android.Content.Res;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class PokemonService : IPokemonService
    {
        #region Properties
        private const int _downloadImageTimeoutInSeconds = 15;

        private readonly ISqliteConnectionService _connectionService;
        private readonly ITypePokService _typePokService;
        private readonly IProfileService _profileService;
        private readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };

        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public PokemonService(ISqliteConnectionService connectionService, ITypePokService typePokService, IProfileService profileService)
        {
            _connectionService = connectionService;
            _typePokService = typePokService;
            _profileService = profileService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<Pokemon>> GetAllAsync()
        {
            var result = await _database.Table<Pokemon>().OrderBy(m => m.Number).ToListAsync();
            return result;
        }

        public async Task<List<Pokemon>> GetAllStartGen1Async()
        {
            List<Pokemon> pokemons = new List<Pokemon>();

            var result = await GetAllAsync();

            pokemons.Add(result.Find(m => m.Number.Equals("001")));
            pokemons.Add(result.Find(m => m.Number.Equals("004")));
            pokemons.Add(result.Find(m => m.Number.Equals("007")));
            pokemons.Add(result.Find(m => m.Number.Equals("025")));

            return await Task.FromResult(pokemons);
        }

        public async Task<List<Pokemon>> GetAllWithoutVariantAsync(string filter, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, bool steel, bool fighting, bool dragon, bool water, bool electric, bool fairy, bool fire, bool ice, bool bug, bool normal, bool grass, bool poison, bool psychic, bool rock, bool ground, bool ghost, bool dark, bool flying, bool descending)
        {
            List<Pokemon> result = await _database.Table<Pokemon>().Where(m => m.TypeEvolution.Equals(Constantes.NormalEvolution)).OrderBy(m => m.Number).ToListAsync();

            Profile profile = await _profileService.GetProfileActivatedAsync();
            IEnumerable<Favorite> favorites = await _database.Table<Favorite>().Where(m => m.ProfileID.Equals(profile.Id)).ToListAsync();

            byte[] ImgFavorite = null;
            if (favorites.Count() > 0)
                ImgFavorite = await Utils.GetByteAssetImage(Constantes.LoveFull);

            foreach (Favorite favorite in favorites)
            {
                Pokemon pokemon = result.Find(m => m.Id.Equals(favorite.PokemonID));
                if (pokemon != null)
                {
                    pokemon.Favorite = true;
                    pokemon.ImgFavorite = ImgFavorite;
                }
            }

            List<Pokemon> resultFilter = new List<Pokemon>();
            List<Pokemon> resultFilterGen = new List<Pokemon>();
            List<Pokemon> resultFilterType = new List<Pokemon>();

            resultFilterGen = await GetPokemonsWithFilterGen(result, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus);

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

        public async Task<List<Pokemon>> GetFamilyWithoutVariantAsync(string family)
        {
            string[] vs = family.Split(',');
            List<Pokemon> result = new List<Pokemon>();

            foreach (var item in vs)
            {
                int id = int.Parse(item);
                Pokemon pokemon = await GetByIdAsync(id);
                if (pokemon != null)
                    result.Add(pokemon);
            }

            return result;
        }

        public async Task<List<Pokemon>> GetAllVariantAsync(string number, string typeEvolution)
        {
            var result = await _database.Table<Pokemon>().Where(m => m.Number.Equals(number) && m.TypeEvolution.Equals(typeEvolution)).OrderBy(m => m.Number).ToListAsync();
            return result;
        }

        public async Task<List<Pokemon>> GetPokemonsNotUpdatedAsync()
        {
            var result = await _database.Table<Pokemon>().Where(m => m.Updated.Equals(false)).OrderBy(m => m.Number).ToListAsync();
            return result;
        }

        public async Task<Pokemon> GetByIdAsync(int id)
        {
            return await _database.Table<Pokemon>().Where(m => m.Id.Equals(id)).FirstAsync();
        }

        public async Task<Pokemon> GetByNameAsync(string libelle)
        {
            List<Pokemon> pokemons = await _database.Table<Pokemon>().ToListAsync();

            string filter = libelle;
            if (libelle.Equals(Constantes.Type_0))
                filter = Constantes.Type0;

            Pokemon result = pokemons.Find(m => m.Name.Equals(filter));

            if (result != null)
            {
                return result;
            }
            else
            {
                return result = pokemons.Find(m => m.Name.Contains(libelle.Split(' ')[0]) && m.TypeEvolution.Equals(Constantes.NormalEvolution));
            }
        }
        #endregion

        public async Task<int> CreateAsync(Pokemon Pokemon)
        {
            var result = await _database.InsertAsync(Pokemon);
            return result;
        }

        public async Task<int> UpdateAsync(Pokemon Pokemon)
        {
            var result = await _database.InsertOrReplaceAsync(Pokemon);
            return result;
        }

        public async Task<int> GetNumberInDbAsync()
        {
            var result = await _database.Table<Pokemon>().CountAsync();
            return result;
        }

        public async Task<int> GetNumberPokJsonAsync()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("PokeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<PokemonJson>>(json).Count);
        }

        public async Task<int> GetNumberPokUpdateAsync()
        {
            var result = await _database.Table<Pokemon>().Where(m => m.Updated.Equals(true)).CountAsync();
            return result;
        }
        #endregion

        #region Populate Database
        public async Task<List<PokemonJson>> GetListPokeScrapJson()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("PokeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<PokemonJson>>(json));
        }

        public async Task Populate(int nbPokInDb, List<PokemonJson> pokemonsJson)
        {
            if (!nbPokInDb.Equals(pokemonsJson.Count))
            {
                int countPokemonJson = 0;
                foreach (PokemonJson pokemonJson in pokemonsJson)
                {
                    countPokemonJson++;

                    if (countPokemonJson > nbPokInDb)
                    {
                        Pokemon pokemon = await ConvertPokemonJsonInPokemon(pokemonJson);
                        await CreateAsync(pokemon);

                        Debug.Write("Creation:" + pokemon.Number + " - " + pokemon.Name);

                        if (Constantes.IsTestDB)
                            if (pokemon.Number.Equals("721"))
                                break;
                    }
                }
            }
        }

        public async Task<Pokemon> ConvertPokemonJsonInPokemon(PokemonJson pokemonJson)
        {
            Pokemon pokemon = new Pokemon();

            pokemon.Number = pokemonJson.Number;
            pokemon.Name = pokemonJson.Name;
            pokemon.DisplayName = pokemonJson.DisplayName;
            pokemon.DescriptionVx = pokemonJson.DescriptionVx;
            pokemon.DescriptionVy = pokemonJson.DescriptionVy;
            pokemon.UrlImg = pokemonJson.UrlImg;
            pokemon.DataImg = await DownloadImageAsync(pokemonJson.UrlImg);

            pokemon.UrlSprite = pokemonJson.UrlSprite;
            pokemon.DataSprite = await DownloadImageAsync(pokemonJson.UrlSprite);

            pokemon.Size = pokemonJson.Size;
            pokemon.Category = pokemonJson.Category;
            pokemon.Weight = pokemonJson.Weight;
            pokemon.Talent = pokemonJson.Talent;
            pokemon.DescriptionTalent = pokemonJson.DescriptionTalent;

            string[] typesTab = pokemonJson.Types.Split(',');
            int i = 0;
            foreach (string item in typesTab)
            {
                TypePok type = await _typePokService.GetByNameAsync(item);

                if (i == 0)
                {
                    pokemon.Types = type.Name.ToString();
                    pokemon.TypesID = type.Id.ToString();
                    i++;
                }
                else
                {
                    pokemon.Types += ',' + type.Name.ToString();
                    pokemon.TypesID += ',' + type.Id.ToString();
                }
            }

            string[] weaknessTab = pokemonJson.Weakness.Split(',');
            i = 0;
            foreach (string item in weaknessTab)
            {
                TypePok type = await _typePokService.GetByNameAsync(item);
                if (i == 0)
                {
                    pokemon.Weakness = type.Name.ToString();
                    pokemon.WeaknessID = type.Id.ToString();
                    i++;
                }
                else
                {
                    pokemon.Weakness += ',' + type.Name.ToString();
                    pokemon.WeaknessID += ',' + type.Id.ToString();
                }
            }

            pokemon.Evolutions = pokemonJson.Evolutions;
            pokemon.TypeEvolution = pokemonJson.TypeEvolution;
            pokemon.WhenEvolution = pokemonJson.WhenEvolution;
            pokemon.StatPv = pokemonJson.statPv;
            pokemon.StatAttaque = pokemonJson.statAttaque;
            pokemon.StatDefense = pokemonJson.statDefense;
            pokemon.StatAttaqueSpe = pokemonJson.statAttaqueSpe;
            pokemon.StatDefenseSpe = pokemonJson.statDefenseSpe;
            pokemon.StatVitesse = pokemonJson.statVitesse;
            pokemon.StatTotal = pokemonJson.statTotal;
            pokemon.Generation = pokemonJson.Generation;
            pokemon.NextUrl = pokemonJson.NextUrl;
            pokemon.Updated = false;

            return pokemon;
        }

        public async Task PopulateUpdateEvolution(List<PokemonJson> pokemonsJson)
        {
            List<Pokemon> pokemonsNoUpdated = await GetPokemonsNotUpdatedAsync();

            foreach (Pokemon pokemonNoUpdated in pokemonsNoUpdated)
            {
                PokemonJson pokemonJson = pokemonsJson.Find(m => m.Name.Equals(pokemonNoUpdated.Name));

                int i = int.Parse(pokemonJson.Number);
                try
                {
                    Pokemon pokemonUpdated = await UpdateEvolutionWithJson(pokemonJson, pokemonNoUpdated);
                    if (pokemonUpdated != null)
                        await UpdateAsync(pokemonUpdated);

                    Debug.Write("Update: " + pokemonJson.Number + " - " + pokemonJson.Name);

                    if(Constantes.IsTestDB)
                        if (pokemonJson.Number.Equals("721"))
                        break;
                }
                catch
                {
                    Debug.Write("Update Error: " + pokemonJson.Number + " - " + pokemonJson.Name);
                }
            }
        }

        public async Task<Pokemon> UpdateEvolutionWithJson(Pokemon pokemonNoUpdated)
        {
            List<PokemonJson> pokemonsJson = await GetListPokeScrapJson();

            PokemonJson pokemonJson = pokemonsJson.Find(m => m.Name.Equals(pokemonNoUpdated.Name));

            int i = int.Parse(pokemonJson.Number);
            try
            {
                Pokemon pokemonUpdated = await UpdateEvolutionWithJson(pokemonJson, pokemonNoUpdated);
                if (pokemonUpdated != null)
                    await UpdateAsync(pokemonUpdated);

                Debug.Write("Update: " + pokemonJson.Number + " - " + pokemonJson.Name);
                return pokemonUpdated;
            }
            catch
            {
                Debug.Write("Update Error: " + pokemonJson.Number + " - " + pokemonJson.Name);
                return null;
            }
        }

        public async Task<Pokemon> UpdateEvolutionWithJson(PokemonJson pokemonJson, Pokemon pokemonUpdate)
        {
            if (!string.IsNullOrEmpty(pokemonJson.Evolutions))
            {
                string[] evolutionsTab = pokemonJson.Evolutions.Split(',');

                int i = 0;
                foreach (string item in evolutionsTab)
                {
                    Pokemon pokemon = await GetByNameAsync(item);
                    if (i == 0)
                    {
                        pokemonUpdate.Evolutions = pokemon.Id.ToString();
                        i++;
                    }
                    else
                    {
                        pokemonUpdate.Evolutions += ',' + pokemon.Id.ToString();
                    }
                }

                pokemonUpdate.Updated = true;

                return pokemonUpdate;
            }
            else
                return null;
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

        #region Generate Quizz
        public async Task<Pokemon> GetPokemonRandom(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus)
        {
            List<Pokemon> result = await GetAllAsync();
            List<Pokemon> resultFilterGen = await GetPokemonsWithFilterGen(result, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus);

            Random random = new Random();
            int numberRandom = random.Next(resultFilterGen.Count);

            return await Task.FromResult(resultFilterGen[numberRandom]);
        }
        public async Task<Pokemon> GetPokemonRandom(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, List<Pokemon> alreadySelected)
        {
            List<Pokemon> result = await GetAllAsync();
            List<Pokemon> resultFilterGen = await GetPokemonsWithFilterGen(result, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus);

            Random random = new Random();
            int numberRandom = random.Next(resultFilterGen.Count);
            Pokemon pokemon = alreadySelected.Find(m => m.Id.Equals(resultFilterGen[numberRandom].Id));

            while (pokemon != null)
            {
                numberRandom = random.Next(resultFilterGen.Count);
                pokemon = alreadySelected.Find(m => m.Id.Equals(resultFilterGen[numberRandom].Id));
            }

            return await Task.FromResult(resultFilterGen[numberRandom]);
        }
        #endregion
        #endregion

        #region Private Methods
        private async Task<List<Pokemon>> GetPokemonsWithFilterGen(List<Pokemon> result, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus)
        {
            List<Pokemon> resultFilterGen = new List<Pokemon>();

            if (gen1)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(1) && m.TypeEvolution.Equals(Constantes.NormalEvolution)));
            if (gen2)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(2) && m.TypeEvolution.Equals(Constantes.NormalEvolution)));
            if (gen3)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(3) && m.TypeEvolution.Equals(Constantes.NormalEvolution)));
            if (gen4)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(4) && m.TypeEvolution.Equals(Constantes.NormalEvolution)));
            if (gen5)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(5) && m.TypeEvolution.Equals(Constantes.NormalEvolution)));
            if (gen6)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(6) || m.TypeEvolution.Equals(Constantes.MegaEvolution)).Distinct());
            if (gen7)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(7) || m.TypeEvolution.Equals(Constantes.Alola)).Distinct());
            if (gen8)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(8) || m.TypeEvolution.Equals(Constantes.Galar) || m.TypeEvolution.Equals(Constantes.GigaEvolution)).Distinct());
            if (genArceus)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(0) || m.TypeEvolution.Equals(Constantes.Hisui)).Distinct());

            if (resultFilterGen.Count.Equals(0))
                resultFilterGen = result;

            return await Task.FromResult(resultFilterGen);
        }

        private async Task<List<Pokemon>> GetPokemonsWithFilterType(List<Pokemon> resultFilterGen, bool steel, bool fighting, bool dragon, bool water, bool electric, bool fairy, bool fire, bool ice, bool bug, bool normal, bool grass, bool poison, bool psychic, bool rock, bool ground, bool ghost, bool dark, bool flying)
        {
            List<Pokemon> resultFilterType = new List<Pokemon>();
            if (steel)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Steel)));
            if (fighting)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Fighting)));
            if (dragon)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Dragon)));
            if (water)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Water)));
            if (electric)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Electric)));
            if (fairy)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Fairy)));
            if (fire)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Fire)));
            if (ice)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Ice)));
            if (bug)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Bug)));
            if (normal)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Normal)));
            if (grass)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Grass)));
            if (poison)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Poison)));
            if (psychic)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Psychic)));
            if (rock)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Rock)));
            if (ground)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Ground)));
            if (ghost)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Ghost)));
            if (dark)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Dark)));
            if (flying)
                resultFilterType.AddRange(resultFilterGen.FindAll(m => m.Types.Contains(Constantes.Flying)));

            if (resultFilterType.Count.Equals(0))
                resultFilterType = resultFilterGen;

            return await Task.FromResult(resultFilterType);
        }
        #endregion
    }
}
