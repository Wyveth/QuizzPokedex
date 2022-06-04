﻿using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class QuestionTypeService : IQuestionTypeService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private readonly IDifficultyService _difficultyService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constuctor
        public QuestionTypeService(ISqliteConnectionService connectionService, IDifficultyService difficultyService)
        {
            _connectionService = connectionService;
            _difficultyService = difficultyService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<QuestionType>> GetAllAsync()
        {
            var result = await _database.Table<QuestionType>().ToListAsync();
            return result;
        }

        public async Task<QuestionType> GetByIdAsync(int id)
        {
            var result = await _database.Table<QuestionType>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }
        #endregion

        public async Task<int> CreateAsync(QuestionType questionType)
        {
            var result = await _database.InsertAsync(questionType);
            return result;
        }

        public async Task<int> DeleteAsync(QuestionType questionType)
        {
            var result = await _database.DeleteAsync(questionType);
            return result;
        }

        public async Task<int> UpdateAsync(QuestionType questionType)
        {
            var result = await _database.InsertOrReplaceAsync(questionType);
            return result;
        }
        #endregion

        #region Populate Database
        public async Task Populate()
        {
            Difficulty difficultyEasy = await _difficultyService.GetByLibelleAsync(Constantes.EasyTQ);
            QuestionType questionType = new QuestionType()
            {
                Code = Constantes.QTypPok,
                Libelle = "Qui est ce pokémon?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTyp,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTyp,
                Libelle = "Quel est-ce type?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4
            };
            await CreateAsync(questionType);
        }
        #endregion

        #region Generate Quizz
        public async Task<QuestionType> GetQuestionTypeRandom(bool easy, bool normal, bool hard)
        {
            List<QuestionType> result = await GetAllAsync();
            List<QuestionType> resultFilterDifficulty = await GetQuestionTypesWithFilterDifficulty(result, easy, normal, hard);

            //Random random = new Random();
            //int numberRandom = random.Next(resultFilterDifficulty.Count);
            //int resultRandom = resultFilterDifficulty[numberRandom].Id;
            return await Task.FromResult(await GetNumberRandomWithPercentage(resultFilterDifficulty));
        }
        #endregion
        #endregion

        #region Private Methods
        private async Task<List<QuestionType>> GetQuestionTypesWithFilterDifficulty(List<QuestionType> result, bool easy, bool normal, bool hard)
        {
            List<QuestionType> resultFilterDifficulty = new List<QuestionType>();
            Difficulty difficulty = new Difficulty();

            if (easy)
            {
                difficulty = await _difficultyService.GetByLibelleAsync(Constantes.EasyTQ);
                resultFilterDifficulty.AddRange(result.FindAll(m => m.DifficultyID.Equals(difficulty.Id)));
            }

            if (normal)
            {
                difficulty = await _difficultyService.GetByLibelleAsync(Constantes.NormalTQ);
                resultFilterDifficulty.AddRange(result.FindAll(m => m.DifficultyID.Equals(difficulty.Id)));
            }
            if (hard)
            {
                difficulty = await _difficultyService.GetByLibelleAsync(Constantes.HardTQ);
                resultFilterDifficulty.AddRange(result.FindAll(m => m.DifficultyID.Equals(difficulty.Id)));
            }

            if (resultFilterDifficulty.Count.Equals(0))
                resultFilterDifficulty = result;

            return await Task.FromResult(resultFilterDifficulty);
        }

        private async Task<QuestionType> GetNumberRandomWithPercentage(List<QuestionType> resultFilterDifficulty)
        {
            List<QuestionType> questionTypes = new List<QuestionType>();

            Random random = new Random();
            int numberRandom = random.Next(100);

            //If Question Type => Type // 10%
            if (numberRandom <= 20)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTyp));
            else
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPok));

            int numberTypeQuestion = random.Next(questionTypes.Count);
            return await Task.FromResult(questionTypes[numberTypeQuestion]);
        }
        #endregion
    }
}