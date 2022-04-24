using Android.Content.Res;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class TypePokService : ITypePokService
    {
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();

        public TypePokService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<List<TypePok>> GetTypePoksAsync()
        {
            var result = await _database.Table<TypePok>().ToListAsync();
            return result;
        }

        public async Task<TypePok> GetTypePokByNameAsync(string libelle)
        {
            var result = await _database.Table<TypePok>().ToListAsync();
            return result.Find(m => m.Name.Equals(libelle));
        }

        public async Task<int> CreateTypePokAsync(TypePok typePok)
        {
            var result = await _database.InsertAsync(typePok);
            return result;
        }

        public async Task<int> DeleteTypePokAsync(TypePok typePok)
        {
            var result = await _database.DeleteAsync(typePok);
            return result;
        }

        public async Task<int> UpdateTypePokAsync(TypePok typePok)
        {
            var result = await _database.InsertOrReplaceAsync(typePok);
            return result;
        }

        public async Task<int> GetNumberTypePokAsync()
        {
            var result = await _database.Table<TypePok>().CountAsync();
            return result;
        }

        public void PopulateTypePok()
        {
            List<TypePok> typesPok = new List<TypePok>();
            ConcurrentDictionary<string, TypePok> typesInsertDic = new ConcurrentDictionary<string, TypePok>();

            AssetManager assets = Android.App.Application.Context.Assets;
            string json;
            using (StreamReader sr = new StreamReader(assets.Open("PokeScrap.json")))
            {
                json = sr.ReadToEnd();
            }

            List<PokemonJson> pokemonLst = JsonConvert.DeserializeObject<List<PokemonJson>>(json);
            foreach (PokemonJson pokemonJson in pokemonLst)
            {
                ConvertTypePokJsonInTypePok(pokemonJson.Types, typesInsertDic);
            }

            foreach (TypePok item in typesInsertDic.Values)
            {
                _ = CreateTypePokAsync(item);
            }
        }

        public List<TypePok> ConvertTypePokJsonInTypePok(string typePok, ConcurrentDictionary<string, TypePok> typesInsertDic)
        {
            List<TypePok> types = new List<TypePok>();
            string[] typesTab = typePok.Split(',');

            foreach (string item in typesTab)
            {
                    TypePok type = new TypePok();
                    type.Name = item;
                    type.UrlImg = item + ".png";
                    types.Add(type);

                typesInsertDic.TryAdd(type.Name, type);
            }

            return types;
        }
    }
}
