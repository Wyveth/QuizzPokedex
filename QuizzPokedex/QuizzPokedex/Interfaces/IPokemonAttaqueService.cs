using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonAttaqueService
    {
        Task<List<PokemonAttaque>> GetAllAsync();
        Task<List<PokemonAttaque>> GetPokemonAttaquesAsync(string pokemonAttaques);
        Task<PokemonAttaque> GetByIdAsync(int id);
        Task<int> CreateAsync(PokemonAttaque pokemonAttaque);
    }
}
