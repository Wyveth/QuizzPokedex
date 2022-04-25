using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IPokemonService
    {
        void Populate();
        Task<List<Pokemon>> GetAllAsync();
        Task<Pokemon> GetByNameAsync(string libelle);
        Task<int> CreateAsync(Pokemon pokemon);
        Task<int> DeleteAsync(Pokemon pokemon);
        Task<int> UpdateAsync(Pokemon pokemon);
        Task<int> GetNumberAsync();
        Task<byte[]> DownloadImageAsync(string UrlImg);
    }
}
