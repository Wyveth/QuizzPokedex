using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IProfileService
    {
        Task<Profile> GetProfileActivatedAsync();
        Task<List<Profile>> GetProfileNotActivatedAsync();
        Task<List<Profile>> GetAllAsync();
        Task<int> CountGetAllAsync();
        Task<bool> CheckIfProfilPokemonExist(Pokemon pokemon);
        Task<int> CreateAsync(Profile profile);
        Task<int> DeleteAsync(Profile profile);
        Task<int> UpdateAsync(Profile profile);
        Task<bool> UpdateProfileActivatedAsync(Profile profile);
    }
}
