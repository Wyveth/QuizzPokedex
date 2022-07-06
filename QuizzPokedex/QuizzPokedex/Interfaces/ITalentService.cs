using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface ITalentService
    {
        Task<List<Talent>> GetAllAsync();
        Task<List<Talent>> GetTalentsAsync(string talents);
        Task<Talent> GetByIdAsync(int id);
        Task<Talent> GetByNameAsync(string libelle);
        Task<int> CreateAsync(Talent talent);
        Task<Talent> GetTalentRandom();
        Task<Talent> GetTalentRandom(List<Talent> alreadySelected);
    }
}
