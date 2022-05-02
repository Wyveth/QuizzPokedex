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

        public async Task<Pokemon> GetByIdAsync(string identifiant)
        {
            int id = int.Parse(identifiant);
            return await _database.Table<Pokemon>().Where(m => m.Id.Equals(id)).FirstAsync();
        }

        public async Task<Pokemon> GetByNameAsync(string libelle)
        {
            List<Pokemon> pokemons = await _database.Table<Pokemon>().ToListAsync();

            Pokemon result = pokemons.Find(m => m.Name.Equals(libelle));

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

        public async Task<int> GetNumberAsync()
        {
            var result = await _database.Table<Pokemon>().CountAsync();
            return result;
        }
        #endregion
        #endregion

        #region Populate Database
        public async void Populate()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("PokeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            Task.Delay(4000).Wait();

            List<PokemonJson> pokemonsJson = JsonConvert.DeserializeObject<List<PokemonJson>>(json);
            foreach (PokemonJson pokemonJson in pokemonsJson)
            {
                Pokemon pokemon = await ConvertPokemonJsonInPokemon(pokemonJson);
                _ = CreateAsync(pokemon);

                Debug.Write("Creation:" + pokemon.Number + " - " + pokemon.Name);
            }
        }

        public async void PopulateUpdateEvolution()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("PokeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            List<PokemonJson> pokemonsJson = JsonConvert.DeserializeObject<List<PokemonJson>>(json);
            foreach (PokemonJson pokemonJson in pokemonsJson)
            {
                int i = int.Parse(pokemonJson.Number);
                try
                {
                    Pokemon pokemon = await UpdateEvolutionWithJson(pokemonJson);

                    if (pokemon != null)
                        _ = UpdateAsync(pokemon);

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

            pokemon.TypeEvolution = pokemonJson.TypeEvolution;
            pokemon.Generation = pokemonJson.Generation;
            pokemon.NextUrl = pokemonJson.NextUrl;

            return pokemon;
        }

        public async Task<Pokemon> UpdateEvolutionWithJson(PokemonJson pokemonJson)
        {
            Pokemon pokemonUpdate = await GetByNameAsync(pokemonJson.Name);

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
