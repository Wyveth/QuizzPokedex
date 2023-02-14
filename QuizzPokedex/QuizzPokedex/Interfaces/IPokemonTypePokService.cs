using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonTypePokService
    {
        Task<List<PokemonTypePok>> GetAllAsync();
        Task<List<PokemonTypePok>> GetPokemonTypePoksAsync(string pokemonTypePoks);
        Task<PokemonTypePok> GetByIdAsync(int id);
        Task<int> CreateAsync(PokemonTypePok pokemonTypePok);
    }
}
