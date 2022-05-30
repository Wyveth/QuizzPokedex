using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class QuizzService : IQuizzService
    {
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();

        public QuizzService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<List<Quizz>> GetAllAsync()
        {
            var result = await _database.Table<Quizz>().ToListAsync();
            return result;
        }

        public async Task<List<Quizz>> GetAllByProfileAsync(string profileId)
        {
            int id = int.Parse(profileId);
            var result = await _database.Table<Quizz>().ToListAsync();
            return result.FindAll(m => m.ProfileId.Equals(id));
        }

        public async Task<Quizz> GetByIdAsync(string identifiant)
        {
            int id = int.Parse(identifiant);
            var result = await _database.Table<Quizz>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<int> CreateAsync(Quizz quizz)
        {
            var result = await _database.InsertAsync(quizz);
            return result;
        }

        public async Task<int> DeleteAsync(Quizz quizz)
        {
            var result = await _database.DeleteAsync(quizz);
            return result;
        }

        public async Task<int> UpdateAsync(Quizz quizz)
        {
            var result = await _database.InsertOrReplaceAsync(quizz);
            return result;
        }
    }
}
