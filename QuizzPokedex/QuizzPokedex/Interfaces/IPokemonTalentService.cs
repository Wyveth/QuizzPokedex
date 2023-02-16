using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonTalentService
    {
        Task<List<PokemonTalent>> GetAllAsync();
        Task<List<PokemonTalent>> GetPokemonTalentsAsync(string pokemonTalents);
        Task<PokemonTalent> GetByIdAsync(int id);
        Task<List<PokemonTalent>> GetTalentsByPokemon(int pokemonId);
        Task<List<PokemonTalent>> GetPokemonsByTalent(int talentId);
        Task<int> CreateAsync(PokemonTalent pokemonTalent);
    }
}
