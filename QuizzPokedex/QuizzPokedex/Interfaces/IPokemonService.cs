using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonService
    {
        void PopulatePokemon();
        Task<List<Pokemon>> GetPokemonsAsync();
        Task<Pokemon> GetPokemonByNameAsync(string libelle);
        Task<int> CreatePokemonAsync(Pokemon pokemon);
        Task<int> DeletePokemonAsync(Pokemon pokemon);
        Task<int> UpdatePokemonAsync(Pokemon pokemon);
        Task<int> GetNumberPokemonAsync();
        Task<byte[]> DownloadImageAsync(string UrlImg);
    }
}
