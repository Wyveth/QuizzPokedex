using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface ITypePokService
    {
        Task Populate(int nbTypePokInDb, List<TypeJson> typesJson);
        Task<List<TypePok>> GetAllAsync();
        Task<List<TypePok>> GetTypesAsync(string types);
        Task<TypePok> GetByIdAsync(string identifiant);
        Task<TypePok> GetByNameAsync(string libelle);
        Task<string> GetBackgroundColorType(string libelle);
        Task<int> CreateAsync(TypePok typePok);
        Task<List<TypeJson>> GetListTypeScrapJson();
        Task<int> GetNumberTypeJsonAsync();
        Task<int> GetNumberInDbAsync();
        Task<byte[]> DownloadImageAsync(string UrlImg);
    }
}
