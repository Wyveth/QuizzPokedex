using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class ProfileService : IProfileService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public ProfileService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
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
        #endregion

        public async Task<int> CreateAsync(Profile profile)
        {
            var result = await _database.InsertAsync(profile);

            //Update Other Profile
            await UpdateProfileActivatedAsync(profile);

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

        public async Task<bool> UpdateProfileActivatedAsync(Profile profile)
        {
            List<Profile> profiles = await GetAllAsync();
            bool result = false;
            foreach (Profile item in profiles)
            {
                if(item.Id == profile.Id)
                    item.Activated = true;
                else
                    item.Activated = false;
                await UpdateAsync(item);
            }
            return result;
        }
        #endregion
        public async Task<bool> CheckIfProfilPokemonExist(Pokemon pokemon)
        {
            List<Profile> profiles = await GetAllAsync();
            Profile profile = profiles.Find(m => m.PokemonID.Equals(pokemon.Id));

            if (profile == null)
                return false;
            else
                return true;
        }

        public async Task<int> CountGetAllAsync()
        {
            var profiles = await _database.Table<Profile>().ToListAsync();
            return profiles.Count;
        }
        #endregion
    }
}
