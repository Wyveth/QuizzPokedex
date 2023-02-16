using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonTypePokService
    {
        Task<List<PokemonTypePok>> GetAllAsync();
        Task<List<PokemonTypePok>> GetPokemonTypePoksAsync(string pokemonTypePoks);
        Task<PokemonTypePok> GetByIdAsync(int id);
        Task<List<PokemonTypePok>> GetTypesPokByPokemon(int pokemonId);
        Task<List<PokemonTypePok>> GetPokemonsByTypePok(int typePokId);
        Task<int> CreateAsync(PokemonTypePok pokemonTypePok);
    }
}
