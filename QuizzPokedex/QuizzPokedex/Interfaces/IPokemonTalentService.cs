﻿using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonTalentService
    {
        Task<List<PokemonTalent>> GetAllAsync();
        Task<List<PokemonTalent>> GetPokemonTalentsAsync(string pokemonTalents);
        Task<PokemonTalent> GetByIdAsync(int id);
        Task<int> CreateAsync(PokemonTalent pokemonTalent);
    }
}