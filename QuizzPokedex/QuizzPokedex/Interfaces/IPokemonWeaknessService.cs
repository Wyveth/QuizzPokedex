using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonWeaknessService
    {
        Task<List<PokemonWeakness>> GetAllAsync();
        Task<List<PokemonWeakness>> GetPokemonWeaknessesAsync(string pokemonWeaknesses);
        Task<PokemonWeakness> GetByIdAsync(int id);
        Task<int> CreateAsync(PokemonWeakness pokemonWeakness);
    }
}
