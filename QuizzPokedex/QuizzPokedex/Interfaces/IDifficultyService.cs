using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IDifficultyService
    {
        Task<List<Difficulty>> GetAllAsync();
        Task<Difficulty> GetByIdAsync(string identifiant);
        Task<int> CreateAsync(Difficulty difficulty);
        Task<int> DeleteAsync(Difficulty difficulty);
        Task<int> UpdateAsync(Difficulty difficulty);
    }
}
