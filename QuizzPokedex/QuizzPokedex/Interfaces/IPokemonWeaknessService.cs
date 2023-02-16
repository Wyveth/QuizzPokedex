using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonWeaknessService
    {
        Task<List<PokemonWeakness>> GetAllAsync();
        Task<PokemonWeakness> GetByIdAsync(int id);
        Task<List<PokemonWeakness>> GetWeaknessesByPokemon(int pokemonId);
        Task<List<PokemonWeakness>> GetPokemonsByWeakness(int typePokId);
        Task<int> CreateAsync(PokemonWeakness pokemonWeakness);
    }
}
