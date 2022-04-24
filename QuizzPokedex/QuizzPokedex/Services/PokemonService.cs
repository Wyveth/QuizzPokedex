using Android.Content.Res;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
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
        private const int _downloadImageTimeoutInSeconds = 15;

        private readonly ISqliteConnectionService _connectionService;
        private readonly ITypePokService _typePokService;
        private readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };

        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();

        public PokemonService(ISqliteConnectionService connectionService, ITypePokService typePokService)
        {
            _connectionService = connectionService;
            _typePokService = typePokService;
        }

        public async Task<List<Pokemon>> GetPokemonsAsync()
        {
            var result = await _database.Table<Pokemon>().OrderBy(m => m.Number).ToListAsync();

            return result;
        }

        public async Task<Pokemon> GetPokemonByNameAsync(string libelle)
        {
            var result = await _database.Table<Pokemon>().ToListAsync();
            return result.Find(m => m.Name.Equals(libelle));
        }

        public async Task<int> CreatePokemonAsync(Pokemon Pokemon)
        {
            var result = await _database.InsertAsync(Pokemon);
            return result;
        }

        public async Task<int> DeletePokemonAsync(Pokemon Pokemon)
        {
            var result = await _database.DeleteAsync(Pokemon);
            return result;
        }

        public async Task<int> UpdatePokemonAsync(Pokemon Pokemon)
        {
            var result = await _database.InsertOrReplaceAsync(Pokemon);
            return result;
        }

        public async Task<int> GetNumberPokemonAsync()
        {
            var result = await _database.Table<Pokemon>().CountAsync();
            return result;
        }

        public async void PopulatePokemon()
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
                Pokemon pokemon = await ConvertPokemonJsonInPokemon(pokemonJson);
                _ = CreatePokemonAsync(pokemon);

                Debug.Write(pokemon.Number + "" + pokemon.Name);
            }
        }

        public async Task<Pokemon> ConvertPokemonJsonInPokemon(PokemonJson pokemonJson)
        {
            Pokemon pokemon = new Pokemon();

            pokemon.Number = pokemonJson.Number;
            pokemon.Name = pokemonJson.Name;
            pokemon.UrlImg = pokemonJson.UrlImg;
            pokemon.DataImg = await DownloadImageAsync(pokemonJson.UrlImg);
            
            pokemon.UrlSprite = pokemonJson.UrlSprite;
            pokemon.DataSprite = await DownloadImageAsync(pokemonJson.UrlSprite);

            pokemon.Size = pokemonJson.Size;
            pokemon.Category = pokemonJson.Category;
            pokemon.Weight = pokemonJson.Weight;
            pokemon.Talent = pokemonJson.Talent;

            string[] typesTab = pokemonJson.Types.Split(',');
            int i = 0;
            foreach (string item in typesTab)
            {
                TypePok type = await _typePokService.GetTypePokByNameAsync(item);
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
                TypePok type = await _typePokService.GetTypePokByNameAsync(item);
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

            pokemon.Generation = pokemonJson.Generation;
            pokemon.NextUrl = pokemonJson.NextUrl;

            return pokemon;
        }

        public async Task<Pokemon> UpdateEvolutionWithJson(PokemonJson pokemonJson)
        {
            Pokemon pokemonUpdate = await GetPokemonByNameAsync(pokemonJson.Name);
            string[] evolutionsTab = pokemonJson.Evolutions.Split(',');

            int i = 0;
            foreach (string item in evolutionsTab)
            {
                Pokemon pokemon = await GetPokemonByNameAsync(item);
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
    }
}
