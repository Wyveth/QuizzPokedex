using Android.Content.Res;
using FFImageLoading;
using Java.Lang;
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
using System.Net.Security;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class PokemonService : IPokemonService
    {
        #region Properties
        private const int _downloadImageTimeoutInSeconds = 15;

        private readonly ISqliteConnectionService _connectionService;
        private readonly ITypePokService _typePokService;
        private readonly ITalentService _talentService;
        private readonly IAttaqueService _attaqueService;
        private readonly IProfileService _profileService;
        private readonly IPokemonTypePokService _pokemonTypePokService;
        private readonly IPokemonWeaknessService _pokemonWeaknessService;
        private readonly IPokemonTalentService _pokemonTalentService;
        private readonly IPokemonAttaqueService _pokemonAttaqueService;
        private readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };

        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public PokemonService(ISqliteConnectionService connectionService, ITypePokService typePokService, ITalentService talentService, IAttaqueService attaqueService, IProfileService profileService, IPokemonTypePokService pokemonTypePokService, IPokemonWeaknessService pokemonWeaknessService, IPokemonTalentService pokemonTalentService, IPokemonAttaqueService pokemonAttaqueService)
        {
            _connectionService = connectionService;
            _typePokService = typePokService;
            _talentService = talentService;
            _attaqueService = attaqueService;
            _profileService = profileService;
            _pokemonTypePokService = pokemonTypePokService;
            _pokemonWeaknessService = pokemonWeaknessService;
            _pokemonTalentService = pokemonTalentService;
            _pokemonAttaqueService = pokemonAttaqueService;
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

        public async Task<List<Pokemon>> GetAllWithoutVariantAsync(string filter, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, bool steel, bool fighting, bool dragon, bool water, bool electric, bool fairy, bool fire, bool ice, bool bug, bool normal, bool grass, bool poison, bool psychic, bool rock, bool ground, bool ghost, bool dark, bool flying, bool descending)
        {
            List<Pokemon> result = await _database.Table<Pokemon>().Where(m => m.TypeEvolution.Equals(Constantes.NormalEvolution)).OrderBy(m => m.Number).ToListAsync();

            Profile profile = await _profileService.GetProfileActivatedAsync();
            IEnumerable<Favorite> favorites = await _database.Table<Favorite>().Where(m => m.ProfileID.Equals(profile.Id)).ToListAsync();

            byte[] ImgFavorite = null;
            if (favorites.Any())
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
            List<Pokemon> resultFilterGen;
            List<Pokemon> resultFilterType;

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
                return pokemons.Find(m => m.Name.Contains(libelle.Split(' ')[0]) && m.TypeEvolution.Equals(Constantes.NormalEvolution));
            }
        }
        #endregion

        public async Task<int> CreateAsync(Pokemon pokemon)
        {
            var result = await _database.InsertAsync(pokemon);
            return result;
        }

        public async Task<int> UpdateAsync(Pokemon pokemon)
        {
            var result = await _database.InsertOrReplaceAsync(pokemon);
            return result;
        }

        public async Task<int> GetNumberInDbAsync()
        {
            var result = await _database.Table<Pokemon>().CountAsync();
            return result;
        }

        public async Task<int> GetNumberJsonAsync()
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

        public async Task<int> GetNumberPokCheckSpriteAsync()
        {
            var result = await _database.Table<Pokemon>().Where(m => m.Check.Equals(true)).CountAsync();
            return result;
        }
        #endregion

        #region Populate Database
        public async Task<List<PokemonJson>> GetListScrapJson()
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

                        if (Constantes.IsGenerateDB && pokemon.Number.Equals("721"))
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
            pokemon.PathImg = await DownloadFile(pokemonJson.UrlImg, "Image/G" + pokemonJson.Generation, pokemonJson.NameEN + Constantes.ExtensionImage);

            pokemon.UrlSprite = pokemonJson.UrlSprite;
            pokemon.PathSprite = await DownloadFile(pokemonJson.UrlSprite, "Sprite/G" + pokemonJson.Generation, pokemonJson.NameEN + Constantes.ExtensionImage);

            pokemon.UrlSound = pokemonJson.UrlSound;
            pokemon.PathSound = "Sound/G" + pokemonJson.Generation + "/" + pokemonJson.NameEN + Constantes.ExtensionSound;

            pokemon.Size = pokemonJson.Size;
            pokemon.Category = pokemonJson.Category;
            pokemon.Weight = pokemonJson.Weight;

            int i = 0;
            //StringBuilder typesPokID = new StringBuilder();
            //foreach (TypePokJson typePokJson in pokemonJson.Types)
            //{
            //    TypePok typePok = await _typePokService.GetByNameAsync(typePokJson.Name);
            //    if (typePok == null)
            //    {
            //        typePok = new PokemonTypePok() { PokemonId = pokemon.id, TypePokId = typePok.id };
            //        //await _talentService.CreateAsync(typePok);
            //    }

            //    if (i == 0)
            //    {
            //        talentsID.Append(talent.Id.ToString());
            //        i++;
            //    }
            //    else
            //    {
            //        talentsID.Append(',' + talent.Id.ToString());
            //    }
            //}
            //pokemon.TypesID = typesPokID.ToString();

            //foreach (string type in pokemonJson.Weakness)
            //{

            //}

            //foreach (string type in pokemonJson.Talents)
            //{

            //}

            //foreach (string type in pokemonJson.Attaques)
            //{

            //}
            //if (!string.IsNullOrEmpty(pokemonJson.Talent))
            //{
            //    string[] talentsTab = pokemonJson.Talent.Split(',');
            //    string[] descriptionTalentsTab = pokemonJson.DescriptionTalent.Split(';');
            //    StringBuilder talentsID = new StringBuilder();
            //    foreach (string item in talentsTab)
            //    {
            //        Talent talent = await _talentService.GetByNameAsync(item);
            //        if (talent == null)
            //        {
            //            talent = new Talent() { Name = item, Description = descriptionTalentsTab[i] };
            //            await _talentService.CreateAsync(talent);
            //        }

            //        if (i == 0)
            //        {
            //            talentsID.Append(talent.Id.ToString());
            //            i++;
            //        }
            //        else
            //        {
            //            talentsID.Append(',' + talent.Id.ToString());
            //        }
            //    }

            //    pokemon.TalentsID = talentsID.ToString();
            //}

            //string[] typesTab = pokemonJson.Types.Split(',');
            //StringBuilder types = new StringBuilder();
            //StringBuilder typesID = new StringBuilder();

            //i = 0;
            //foreach (string item in typesTab)
            //{
            //    TypePok type = await _typePokService.GetByNameAsync(item);

            //    if (i == 0)
            //    {
            //        types.Append(type.Name);
            //        typesID.Append(type.Id.ToString());
            //        i++;
            //    }
            //    else
            //    {
            //        types.Append(',' + type.Name);
            //        typesID.Append(',' + type.Id.ToString());
            //    }
            //}

            //pokemon.Types = types.ToString();
            //pokemon.TypesID = typesID.ToString();

            //string[] weaknessTab = pokemonJson.Weakness.Split(',');
            //StringBuilder weakness = new StringBuilder();
            //StringBuilder weaknessID = new StringBuilder();

            //i = 0;
            //foreach (string item in weaknessTab)
            //{
            //    TypePok type = await _typePokService.GetByNameAsync(item);
            //    if (i == 0)
            //    {
            //        weakness.Append(type.Name);
            //        weaknessID.Append(type.Id.ToString());
            //        i++;
            //    }
            //    else
            //    {
            //        weakness.Append(',' + type.Name);
            //        weaknessID.Append(',' + type.Id.ToString());
            //    }
            //}

            //pokemon.Weakness = weakness.ToString();
            //pokemon.WeaknessID = weaknessID.ToString();

            pokemon.Evolutions = pokemonJson.Evolutions;
            pokemon.TypeEvolution = pokemonJson.TypeEvolution;
            pokemon.WhenEvolution = pokemonJson.WhenEvolution;
            pokemon.StatPv = pokemonJson.StatPv;
            pokemon.StatAttaque = pokemonJson.StatAttaque;
            pokemon.StatDefense = pokemonJson.StatDefense;
            pokemon.StatAttaqueSpe = pokemonJson.StatAttaqueSpe;
            pokemon.StatDefenseSpe = pokemonJson.StatDefenseSpe;
            pokemon.StatVitesse = pokemonJson.StatVitesse;
            pokemon.StatTotal = pokemonJson.StatTotal;
            pokemon.Generation = pokemonJson.Generation;
            pokemon.Game = pokemonJson.Game;
            pokemon.NextUrl = pokemonJson.NextUrl;
            pokemon.Updated = false;
            pokemon.Check = false;

            return pokemon;
        }

        public async Task PopulateUpdateEvolution(List<PokemonJson> pokemonsJson)
        {
            List<Pokemon> pokemonsNoUpdated = await GetPokemonsNotUpdatedAsync();

            foreach (Pokemon pokemonNoUpdated in pokemonsNoUpdated)
            {
                PokemonJson pokemonJson = pokemonsJson.Find(m => m.Name.Equals(pokemonNoUpdated.Name));

                try
                {
                    Pokemon pokemonUpdated = await UpdateEvolutionWithJson(pokemonJson, pokemonNoUpdated);
                    if (pokemonUpdated != null)
                        await UpdateAsync(pokemonUpdated);

                    Debug.Write("Update: " + pokemonJson.Number + " - " + pokemonJson.Name);

                    if (Constantes.IsGenerateDB && pokemonJson.Number.Equals("721"))
                        break;
                }
                catch
                {
                    Debug.Write("Update Error: " + pokemonJson.Number + " - " + pokemonJson.Name);
                }
            }
        }

        public async Task CheckIfPictureNotExistDownload(List<PokemonJson> pokemonsJson)
        {
            List<Pokemon> pokemons = await GetAllAsync();

            foreach (Pokemon pokemon in pokemons)
            {
                PokemonJson pokemonJson = pokemonsJson.Find(m => m.Name.Equals(pokemon.Name));

                try
                {
                    Debug.Write("Info: " + pokemon.Number + " - " + pokemon.Name);
                    if (!File.Exists(pokemon.PathImg))
                    {
                        await DownloadFile(pokemon.UrlImg, "Image/G" + pokemon.Generation, pokemonJson.NameEN + Constantes.ExtensionImage);
                        Debug.Write("Download Image OK");
                    }

                    if (!File.Exists(pokemon.PathSprite))
                    {
                        await DownloadFile(pokemon.UrlSprite, "Sprite/G" + pokemon.Generation, pokemonJson.NameEN + Constantes.ExtensionImage);
                        Debug.Write("Download Sprite OK");
                    }

                    pokemon.Check = true;

                    Pokemon pokemonUpdated = pokemon;
                    if (pokemonUpdated != null)
                        await UpdateAsync(pokemonUpdated);
                }
                catch
                {
                    Debug.Write("Download Error: " + pokemon.Number + " - " + pokemon.Name);
                }
            }
        }

        public async Task ResetNextLaunch()
        {
            List<Pokemon> pokemons = await GetAllAsync();

            foreach (Pokemon pokemon in pokemons)
            {
                try
                {
                    pokemon.Check = false;
                    Pokemon pokemonUpdated = pokemon;
                    await UpdateAsync(pokemonUpdated);
                }
                catch
                {
                    Debug.Write("Download Error: " + pokemon.Number + " - " + pokemon.Name);
                }
            }
        }

        public async Task<Pokemon> UpdateEvolutionWithJson(Pokemon pokemonNoUpdated)
        {
            List<PokemonJson> pokemonsJson = await GetListScrapJson();

            PokemonJson pokemonJson = pokemonsJson.Find(m => m.Name.Equals(pokemonNoUpdated.Name));

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
            #region TypePok
            int i = 0;
            StringBuilder typesPokID = new StringBuilder();
            foreach (TypePokJson typePokJson in pokemonJson.Types)
            {
                PokemonTypePok pokemonTypePok = new PokemonTypePok();
                TypePok typePok = await _typePokService.GetByNameAsync(typePokJson.Name);
                if (typePok != null)
                {
                    pokemonTypePok = new PokemonTypePok() { PokemonId = pokemonUpdate.Id, TypePokId = typePok.Id };
                    await _pokemonTypePokService.CreateAsync(pokemonTypePok);

                    if (i == 0)
                    {
                        typesPokID.Append(pokemonTypePok.Id.ToString());
                        i++;
                    }
                    else
                    {
                        typesPokID.Append(',' + pokemonTypePok.Id.ToString());
                    }
                }
            }
            pokemonUpdate.TypesID = typesPokID.ToString();
            #endregion

            #region Weakness
            i = 0;
            StringBuilder weaknessesID = new StringBuilder();
            foreach (TypePokJson typePokJson in pokemonJson.Weakness)
            {
                PokemonWeakness pokemonWeakness = new PokemonWeakness();
                TypePok typePok = await _typePokService.GetByNameAsync(typePokJson.Name);
                if (typePok != null)
                {
                    pokemonWeakness = new PokemonWeakness() { PokemonId = pokemonUpdate.Id, TypePokId = typePok.Id };
                    await _pokemonWeaknessService.CreateAsync(pokemonWeakness);

                    if (i == 0)
                    {
                        weaknessesID.Append(pokemonWeakness.Id.ToString());
                        i++;
                    }
                    else
                    {
                        weaknessesID.Append(',' + pokemonWeakness.Id.ToString());
                    }
                }
            }
            pokemonUpdate.WeaknessID = weaknessesID.ToString();
            #endregion

            #region Talent
            i = 0;
            StringBuilder talentsID = new StringBuilder();
            foreach (SkillJson talentJson in pokemonJson.Talents)
            {
                PokemonTalent pokemonTalent = new PokemonTalent();
                Talent talent = await _talentService.GetByNameAsync(talentJson.Name);
                if (talent != null)
                {
                    pokemonTalent = new PokemonTalent() { PokemonId = pokemonUpdate.Id, TalentId = talent.Id, isHidden = talentJson.isHidden };
                    await _pokemonTalentService.CreateAsync(pokemonTalent);

                    if (i == 0)
                    {
                        talentsID.Append(pokemonTalent.Id.ToString());
                        i++;
                    }
                    else
                    {
                        talentsID.Append(',' + pokemonTalent.Id.ToString());
                    }
                }
            }
            pokemonUpdate.TalentsID = talentsID.ToString();
            #endregion

            #region Attaque
            i = 0;
            StringBuilder attaquesID = new StringBuilder();
            foreach (AttackJson attaqueJson in pokemonJson.Attaques)
            {
                PokemonAttaque pokemonAttaque = new PokemonAttaque();
                Attaque attaque = await _attaqueService.GetByNameAsync(attaqueJson.Name);
                if (attaque != null)
                {
                    pokemonAttaque = new PokemonAttaque() { PokemonId = pokemonUpdate.Id, AttaqueId = attaque.Id, TypeLearn = attaqueJson.TypeLearn, CTCS = attaqueJson.CTCS, Level = attaqueJson.Level };
                    await _pokemonAttaqueService.CreateAsync(pokemonAttaque);

                    if (i == 0)
                    {
                        attaquesID.Append(pokemonAttaque.Id.ToString());
                        i++;
                    }
                    else
                    {
                        attaquesID.Append(',' + pokemonAttaque.Id.ToString());
                    }
                }
                else
                {
                    Debug.Write("Attaque not found: " + attaqueJson.Name);
                }
            }
            pokemonUpdate.AttacksID = attaquesID.ToString();
            #endregion

            if (!string.IsNullOrEmpty(pokemonJson.Evolutions))
            {
                string[] evolutionsTab = pokemonJson.Evolutions.Split(',');
                StringBuilder evolution = new StringBuilder();

                i = 0;
                foreach (string item in evolutionsTab)
                {
                    Pokemon pokemon = await GetByNameAsync(item);
                    if (i == 0)
                    {
                        evolution.Append(pokemon.Id.ToString());
                        i++;
                    }
                    else
                    {
                        evolution.Append(',' + pokemon.Id.ToString());
                    }
                }

                pokemonUpdate.Evolutions = evolution.ToString();

                pokemonUpdate.Updated = true;

                return pokemonUpdate;
            }
            else
                return null;
        }

        public async Task<byte[]> DownloadImageAsync(string UrlImg)
        {
            try
            {
                using (var httpResponse = await _httpClient.GetAsync(UrlImg))
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
            catch (Java.Lang.Exception e)
            {
                //Handle Exception
                throw new Java.Lang.Exception(e.Message);
            }
        }
        #endregion

        #region Generate Quizz
        public async Task<Pokemon> GetPokemonRandom(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus)
        {
            List<Pokemon> result = await GetAllAsync();
            List<Pokemon> resultFilterGen = await GetPokemonsWithFilterGen(result, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus);

            Random random = new Random();
            int numberRandom = random.Next(resultFilterGen.Count);

            return await Task.FromResult(resultFilterGen[numberRandom]);
        }

        public async Task<Pokemon> GetPokemonRandom(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, List<Pokemon> alreadySelected)
        {
            List<Pokemon> result = await GetAllAsync();
            List<Pokemon> resultFilterGen = await GetPokemonsWithFilterGen(result, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus);

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

        public async Task<Pokemon> GetPokemonRandom(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, TypePok typePok, List<Pokemon> alreadySelected)
        {
            List<Pokemon> result = await GetAllAsync();
            List<Pokemon> resultFilterGen = await GetPokemonsWithFilterGen(result, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus);
            resultFilterGen = resultFilterGen.FindAll(m => m.Types.Contains(typePok.Name));

            Random random = new Random();
            int numberRandom = random.Next(resultFilterGen.Count);
            Pokemon pokemon = alreadySelected.Find(m => m.Id.Equals(resultFilterGen[numberRandom].Id));

            while (pokemon != null)
            {
                numberRandom = random.Next(resultFilterGen.Count);
                pokemon = alreadySelected.Find(m => m.Id.Equals(resultFilterGen[numberRandom].Id));

                if (alreadySelected.Count.Equals(resultFilterGen.Count))
                    break;
            }

            return await Task.FromResult(resultFilterGen[numberRandom]);
        }
        #endregion
        #endregion

        #region Private Methods
        private async Task<List<Pokemon>> GetPokemonsWithFilterGen(List<Pokemon> result, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus)
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
            if (gen9)
                resultFilterGen.AddRange(result.FindAll(m => m.Generation.Equals(9) || m.TypeEvolution.Equals(Constantes.Paldea)));
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
                if(pokemon != null)
                    pokemons.Add(pokemon);
            }
            return pokemons;
        }

        private async Task<string> DownloadFile(string url, string folder, string filename)
        {
            string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), folder);
            Directory.CreateDirectory(pathToNewFolder);

            WebClient webClient = new WebClient();
            string pathToNewFile = Path.Combine(pathToNewFolder, filename);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new
                RemoteCertificateValidationCallback
                (
                   delegate { return true; }
                );
                webClient.DownloadFile(new Uri(url), pathToNewFile);
            }
            catch (System.Exception ex)
            {
                return null;
            }
            finally
            {
                webClient.Dispose();
            }

            return await Task.FromResult(pathToNewFile);
        }
        #endregion
    }
}
