using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
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
    }
}
