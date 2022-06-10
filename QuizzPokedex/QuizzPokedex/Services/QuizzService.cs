﻿using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class QuizzService : IQuizzService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private readonly IQuestionService _questionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public QuizzService(ISqliteConnectionService connectionService, IQuestionService questionService)
        {
            _connectionService = connectionService;
            _questionService = questionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<Quizz>> GetAllAsync()
        {
            var result = await _database.Table<Quizz>().ToListAsync();
            return result;
        }

        public async Task<List<Quizz>> GetAllByProfileAsync(string profileId)
        {
            int id = int.Parse(profileId);
            var result = await _database.Table<Quizz>().ToListAsync();
            return result.FindAll(m => m.ProfileID.Equals(id));
        }

        public async Task<Quizz> GetByIdAsync(int id)
        {
            var result = await _database.Table<Quizz>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }
        #endregion

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
        #endregion

        #region Generate Quizz
        public async Task<Quizz> GenerateQuizz(int profileId, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, bool easy, bool normal, bool hard)
        {
            Quizz quizz = new Quizz()
            {
                QuestionsID = await _questionService.GenerateQuestions(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, easy, normal, hard),
                Done = false,
                ProfileID = profileId
            };

            await CreateAsync(quizz);

            return await Task.FromResult(quizz);
        }
        #endregion
        #endregion
    }
}
