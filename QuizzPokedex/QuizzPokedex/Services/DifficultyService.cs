using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class DifficultyService : IDifficultyService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public DifficultyService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<Difficulty>> GetAllAsync()
        {
            var result = await _database.Table<Difficulty>().ToListAsync();
            return result;
        }

        public async Task<int> GetAllCountAsync()
        {
            var result = await _database.Table<Difficulty>().ToListAsync();
            return result.Count;
        }

        public async Task<Difficulty> GetByIdAsync(int id)
        {
            var result = await _database.Table<Difficulty>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<Difficulty> GetByLibelleAsync(string libelle)
        {
            var result = await _database.Table<Difficulty>().ToListAsync();
            return result.Find(m => m.Libelle.Equals(libelle));
        }
        #endregion

        public async Task<int> CreateAsync(Difficulty difficulty)
        {
            var result = await _database.InsertAsync(difficulty);
            return result;
        }
        public async Task<int> DeleteAsync(Difficulty difficulty)
        {
            var result = await _database.DeleteAsync(difficulty);
            return result;
        }

        public async Task<int> UpdateAsync(Difficulty difficulty)
        {
            var result = await _database.InsertOrReplaceAsync(difficulty);
            return result;
        }
        #endregion

        #region Populate Database
        public async Task Populate()
        {
            Difficulty difficulty = new Difficulty()
            {
                Libelle = Constantes.EasyTQ
            };
            await CreateAsync(difficulty);

            difficulty = new Difficulty()
            {
                Libelle = Constantes.NormalTQ
            };
            await CreateAsync(difficulty);

            difficulty = new Difficulty()
            {
                Libelle = Constantes.HardTQ
            };
            await CreateAsync(difficulty);
        }
        #endregion
        #endregion
    }
}
