using Android.Content.Res;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuizzPokedex.Services
{
    public class PokemonService : IPokemonService
    {
        #region Properties
        private const int _downloadImageTimeoutInSeconds = 15;

        private readonly ISqliteConnectionService _connectionService;
        private readonly ITypePokService _typePokService;
        private readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };

        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public PokemonService(ISqliteConnectionService connectionService, ITypePokService typePokService)
        {
            _connectionService = connectionService;
            _typePokService = typePokService;
        }
        #endregion

        #region Public Methods
        #region Get Data
        public async Task<List<Pokemon>> GetAllAsync()
        {
            var result = await _database.Table<Pokemon>().OrderBy(m => m.Number).ToListAsync();

            return result;
        }

        public async Task<List<Pokemon>> GetAllWithoutVariantAsync(string filter)
        {
            var result = await _database.Table<Pokemon>().Where(m => m.TypeEvolution.Equals(Constantes.NormalEvolution)).OrderBy(m => m.Number).ToListAsync();

            if (!string.IsNullOrEmpty(filter))
                result = result.FindAll(m => m.Name.ToLowerInvariant().Contains(filter) || m.Number.ToLowerInvariant().Contains(filter));

            return result;
        }

        public async Task<List<Pokemon>> GetFamilyWithoutVariantAsync(string family)
        {
            string[] vs = family.Split(',');
            List<Pokemon> result = new List<Pokemon>();

            foreach (var item in vs)
            {
                Pokemon pokemon = await GetByIdAsync(item);
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

        public async Task<Pokemon> GetByIdAsync(string identifiant)
        {
            int id = int.Parse(identifiant);
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

        #region CRUD
        public async Task<int> CreateAsync(Pokemon Pokemon)
        {
            var result = await _database.InsertAsync(Pokemon);
            return result;
        }

        public async Task<int> DeleteAsync(Pokemon Pokemon)
        {
            var result = await _database.DeleteAsync(Pokemon);
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

        public int GetNumberPokJsonAsync()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("PokeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<List<PokemonJson>>(json).Count;
        }

        public async Task<int> GetNumberPokUpdateAsync()
        {
            var result = await _database.Table<Pokemon>().Where(m => m.Updated.Equals(false)).CountAsync();
            return result;
        }
        #endregion
        #endregion

        #region Populate Database
        private List<PokemonJson> GetListPokeScrapJson()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("PokeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<List<PokemonJson>>(json);
        }

        public async Task<Pokemon> UpdateEvolutionWithJson(Pokemon pokemonNoUpdated)
        {
            List<PokemonJson> pokemonsJson = GetListPokeScrapJson();

            PokemonJson pokemonJson = pokemonsJson.Find(m => m.Name.Equals(pokemonNoUpdated.Name));

            int i = int.Parse(pokemonJson.Number);
            try
            {
                Pokemon pokemonUpdated = await UpdateEvolutionWithJson(pokemonJson, pokemonNoUpdated);
                if (pokemonUpdated != null)
                    _ = UpdateAsync(pokemonUpdated);

                Debug.Write("Update: " + pokemonJson.Number + " - " + pokemonJson.Name);
                return pokemonUpdated;
            }
            catch
            {
                Debug.Write("Update Error: " + pokemonJson.Number + " - " + pokemonJson.Name);
                return null;
            }
        }

        public async void Populate(int countInsertPokemon)
        {
            List<PokemonJson> pokemonsJson = GetListPokeScrapJson();

            Task.Delay(4000).Wait();
            int countPokemonJson = 0;

            foreach (PokemonJson pokemonJson in pokemonsJson)
            {
                countPokemonJson++;

                if (countPokemonJson > countInsertPokemon)
                {
                    Pokemon pokemon = await ConvertPokemonJsonInPokemon(pokemonJson);
                    _ = CreateAsync(pokemon);

                    Debug.Write("Creation:" + pokemon.Number + " - " + pokemon.Name);
                }
            }
        }
        
        public async void PopulateUpdateEvolution()
        {
            List<PokemonJson> pokemonsJson = GetListPokeScrapJson();
            List<Pokemon> pokemonsNoUpdated = await GetPokemonsNotUpdatedAsync();
            Task.Delay(7000).Wait();

            foreach(Pokemon pokemonNoUpdated in pokemonsNoUpdated)
            {
                PokemonJson pokemonJson = pokemonsJson.Find(m => m.Name.Equals(pokemonNoUpdated.Name));

                int i = int.Parse(pokemonJson.Number);
                try
                {
                    Pokemon pokemonUpdated = await UpdateEvolutionWithJson(pokemonJson, pokemonNoUpdated);
                    if (pokemonUpdated != null)
                        _ = UpdateAsync(pokemonUpdated);

                    Debug.Write("Update: " + pokemonJson.Number + " - " + pokemonJson.Name);
                }
                catch
                {
                    Debug.Write("Update Error: " + pokemonJson.Number + " - " + pokemonJson.Name);
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
                    pokemon.Types = type.Id.ToString();
                    i++;
                }
                else
                {
                    pokemon.Types += ',' + type.Id.ToString();
                }
            }

            string[] weaknessTab = pokemonJson.Weakness.Split(',');
            i = 0;
            foreach (string item in weaknessTab)
            {
                TypePok type = await _typePokService.GetByNameAsync(item);
                if (i == 0)
                {
                    pokemon.Weakness = type.Id.ToString();
                    i++;
                }
                else
                {
                    pokemon.Weakness += ',' + type.Id.ToString();
                }
            }

            pokemon.Evolutions = pokemonJson.Evolutions;
            pokemon.TypeEvolution = pokemonJson.TypeEvolution;
            pokemon.whenEvolution = pokemonJson.whenEvolution;
            pokemon.statPv = pokemonJson.statPv;
            pokemon.statAttaque = pokemonJson.statAttaque;
            pokemon.statDefense = pokemonJson.statDefense;
            pokemon.statAttaqueSpe = pokemonJson.statAttaqueSpe;
            pokemon.statDefenseSpe = pokemonJson.statDefenseSpe;
            pokemon.statVitesse = pokemonJson.statVitesse;
            pokemon.statTotal = pokemonJson.statTotal;
            pokemon.Generation = pokemonJson.Generation;
            pokemon.NextUrl = pokemonJson.NextUrl;
            pokemon.Updated = false;

            return pokemon;
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
                return null;
            }
        }
        #endregion
    }
}
