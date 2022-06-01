using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IDifficultyService
    {
        Task Populate();
        Task<List<Difficulty>> GetAllAsync();
        Task<Difficulty> GetByIdAsync(int id);
        Task<Difficulty> GetByLibelleAsync(string libelle);
        Task<int> CreateAsync(Difficulty difficulty);
        Task<int> DeleteAsync(Difficulty difficulty);
        Task<int> UpdateAsync(Difficulty difficulty);
    }
}
