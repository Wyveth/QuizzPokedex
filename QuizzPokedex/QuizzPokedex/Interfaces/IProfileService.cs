using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IProfileService
    {
        Task<List<Profile>> GetProfilesAsync();
        Task<int> CreateProfileAsync(Profile profile);
        Task<int> DeleteProfileAsync(Profile profile);
        Task<int> UpdateProfileAsync(Profile profile);
    }
}
