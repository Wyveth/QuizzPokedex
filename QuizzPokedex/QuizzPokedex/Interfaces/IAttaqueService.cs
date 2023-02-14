using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IAttaqueService
    {
        Task Populate(int nbAttaqueInDb, List<AttaqueJson> attaquesJson);
        Task<List<Attaque>> GetAllAsync();
        Task<List<Attaque>> GetAttaquesAsync(string attaques);
        Task<Attaque> GetByIdAsync(int id);
        Task<Attaque> GetByNameAsync(string libelle);
        Task<List<AttaqueJson>> GetListAttaqueScrapJson();
        Task<int> CreateAsync(Attaque attaque);
        Task<int> GetNumberInDbAsync();
    }
}
