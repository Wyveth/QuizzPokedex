using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonService
    {
        Task Populate(int nbPokInDb, List<PokemonJson> pokemonsJson);
        Task PopulateUpdateEvolution(List<PokemonJson> pokemonsJson);
        Task<List<Pokemon>> GetAllAsync();
        Task<List<Pokemon>> GetAllStartGen1Async();
        Task<List<Pokemon>> GetAllWithoutVariantAsync(string filter, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, bool steel, bool fighting, bool dragon, bool water, bool electric, bool fairy, bool fire, bool ice, bool bug, bool normal, bool grass, bool poison, bool psychic, bool rock, bool ground, bool ghost, bool dark, bool flying, bool descending);
        Task<Pokemon> GetByIdAsync(int identifiant);
        Task<Pokemon> GetByNameAsync(string libelle);
        Task<List<Pokemon>> GetFamilyWithoutVariantAsync(string family);
        Task<List<Pokemon>> GetAllVariantAsync(string number, string typeEvolution);
        Task<List<Pokemon>> GetPokemonsNotUpdatedAsync();
        Task<Pokemon> UpdateEvolutionWithJson(Pokemon pokemonUpdate);
        Task<Pokemon> UpdateEvolutionWithJson(PokemonJson pokemonJson, Pokemon pokemonUpdate);
        Task<int> CreateAsync(Pokemon pokemon);
        Task<int> UpdateAsync(Pokemon pokemon);
        Task<List<PokemonJson>> GetListPokeScrapJson(); 
        Task<int> GetNumberPokJsonAsync();
        Task<int> GetNumberInDbAsync();
        Task<int> GetNumberPokUpdateAsync();
        Task<Pokemon> GetPokemonRandom(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus);
        Task<Pokemon> GetPokemonRandom(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, List<Pokemon> alreadySelected);
        Task<byte[]> DownloadImageAsync(string UrlImg);
    }
}
