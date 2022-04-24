using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface ITypePokService
    {
        void PopulateTypePok();
        Task<List<TypePok>> GetTypePoksAsync();
        Task<TypePok> GetTypePokByNameAsync(string libelle);
        Task<int> CreateTypePokAsync(TypePok typePok);
        Task<int> DeleteTypePokAsync(TypePok typePok);
        Task<int> UpdateTypePokAsync(TypePok typePok);
        Task<int> GetNumberTypePokAsync();
    }
}
