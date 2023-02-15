using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface ITalentService
    {
        Task Populate(int nbTalentInDb, List<TalentJson> talentsJson);
        Task<List<Talent>> GetAllAsync();
        Task<List<Talent>> GetTalentsAsync(string talents);
        Task<Talent> GetByIdAsync(int id);
        Task<Talent> GetByNameAsync(string libelle);
        Task<int> CreateAsync(Talent talent);
        Task<List<TalentJson>> GetListScrapJson();
        Task<Talent> GetTalentRandom();
        Task<Talent> GetTalentRandom(List<Talent> alreadySelected);
        Task<int> GetNumberJsonAsync();
        Task<int> GetNumberInDbAsync();
    }
}
