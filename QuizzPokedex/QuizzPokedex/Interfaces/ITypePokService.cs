using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface ITypePokService
    {
        void Populate();
        Task<List<TypePok>> GetAllAsync();
        Task<List<TypePok>> GetTypesAsync(string types);
        Task<TypePok> GetByIdAsync(string identifiant);
        Task<TypePok> GetByNameAsync(string libelle);
        Task<string> GetBackgroundColorType(string libelle);
        Task<int> CreateAsync(TypePok typePok);
        Task<int> DeleteAsync(TypePok typePok);
        Task<int> UpdateAsync(TypePok typePok);
        Task<int> GetNumberAsync();
        Task<byte[]> DownloadImageAsync(string UrlImg);
    }
}
