using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface ITypeAttaqueService
    {
        Task Populate(int nbTypeAttaqueInDb, List<TypeAttaqueJson> typesAttaqueJson);
        Task CheckIfPictureNotExistDownload(List<TypeAttaqueJson> typesAttaqueJson);
        Task<List<TypeAttaque>> GetAllAsync();
        Task<List<TypeAttaque>> GetTypesAttaqueAsync(string typesAttaque);
        Task<TypeAttaque> GetByIdAsync(int id);
        Task<TypeAttaque> GetByNameAsync(string libelle);
        Task<List<TypeAttaqueJson>> GetListScrapJson();
        Task<int> CreateAsync(TypeAttaque typeAttaque);
        Task<int> GetNumberJsonAsync();
        Task<int> GetNumberInDbAsync();
        Task<int> GetNumberCheckAsync();
        Task<int> UpdateAsync(TypeAttaque typeAttaque);
        Task ResetNextLaunch();
    }
}
