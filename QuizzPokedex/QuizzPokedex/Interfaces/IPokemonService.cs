using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonService
    {
        void Populate(int countInsertPokemon);
        void PopulateUpdateEvolution();
        Task<List<Pokemon>> GetAllAsync();
        Task<List<Pokemon>> GetAllWithoutVariantAsync(string filter);
        Task<Pokemon> GetByIdAsync(string identifiant);
        Task<Pokemon> GetByNameAsync(string libelle);
        Task<List<Pokemon>> GetFamilyWithoutVariantAsync(string family);
        Task<List<Pokemon>> GetAllVariantAsync(string number, string typeEvolution);
        Task<int> CreateAsync(Pokemon pokemon);
        Task<int> DeleteAsync(Pokemon pokemon);
        Task<int> UpdateAsync(Pokemon pokemon);
        int GetNumberPokJsonAsync();
        Task<int> GetNumberInDbAsync();
        Task<int> GetNumberPokUpdateAsync();
        Task<byte[]> DownloadImageAsync(string UrlImg);
    }
}
