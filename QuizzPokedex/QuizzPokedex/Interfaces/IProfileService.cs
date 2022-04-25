using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IProfileService
    {
        Task<List<Profile>> GetAllAsync();
        Task<int> CreateAsync(Profile profile);
        Task<int> DeleteAsync(Profile profile);
        Task<int> UpdateAsync(Profile profile);
    }
}
