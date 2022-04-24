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

        public async Task<List<Quizz>> GetQuizzsAsync()
        {
            var result = await _database.Table<Quizz>().ToListAsync();
            return result;
        }

        public async Task<int> CreateQuizzAsync(Quizz Quizz)
        {
            var result = await _database.InsertAsync(Quizz);
            return result;
        }

        public async Task<int> DeleteQuizzAsync(Quizz Quizz)
        {
            var result = await _database.DeleteAsync(Quizz);
            return result;
        }

        public async Task<int> UpdateQuizzAsync(Quizz Quizz)
        {
            var result = await _database.InsertOrReplaceAsync(Quizz);
            return result;
        }
    }
}
