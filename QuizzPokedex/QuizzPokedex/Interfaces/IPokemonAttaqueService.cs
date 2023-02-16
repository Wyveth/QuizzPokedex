using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonAttaqueService
    {
        Task<List<PokemonAttaque>> GetAllAsync();
        Task<List<PokemonAttaque>> GetPokemonAttaquesAsync(string pokemonAttaques);
        Task<List<PokemonAttaque>> GetAttaquesByPokemonTypeLearnAsync(int pokemonId, string typeLearn);
        Task<PokemonAttaque> GetByIdAsync(int id);
        Task<int> CreateAsync(PokemonAttaque pokemonAttaque);
    }
}
