using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();

        public ProfileService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<List<Profile>> GetAllAsync()
        {
            return await _database.Table<Profile>().ToListAsync();
        }

        public async Task<Profile> GetProfileActivatedAsync()
        {
            return await _database.Table<Profile>().Where(m => m.Activated.Equals(true)).FirstAsync();
        }

        public async Task<List<Profile>> GetProfileNotActivatedAsync()
        {
            return await _database.Table<Profile>().Where(m => m.Activated.Equals(false)).ToListAsync();
        }

        public async Task<bool> CheckIfProfilPokemonExist(Pokemon pokemon)
        {
            List<Profile> profiles = await GetAllAsync();
            Profile profile = profiles.Find(m => m.PokemonID.Equals(pokemon.Id));

            if (profile == null)
                return false;
            else
                return true;
        }

        public async Task<int> CreateAsync(Profile profile)
        {
            var result = await _database.InsertAsync(profile);
            return result;
        }

        public async Task<int> DeleteAsync(Profile profile)
        {
            var result = await _database.DeleteAsync(profile);
            return result;
        }

        public async Task<int> UpdateAsync(Profile profile)
        {
            var result = await _database.InsertOrReplaceAsync(profile);
            return result;
        }
    }
}
