using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class DifficultyService : IDifficultyService
    {
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();

        public DifficultyService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<List<Difficulty>> GetAllAsync()
        {
            var result = await _database.Table<Difficulty>().ToListAsync();
            return result;
        }

        public async Task<Difficulty> GetByIdAsync(string identifiant)
        {
            int id = int.Parse(identifiant);
            var result = await _database.Table<Difficulty>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

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
    }
}
