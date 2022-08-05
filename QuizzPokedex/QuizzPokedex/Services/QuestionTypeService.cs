using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
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

        public async Task<int> GetAllCountAsync()
        {
            var result = await _database.Table<QuestionType>().ToListAsync();
            return result.Count;
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
            #region Easy
            Difficulty difficultyEasy = await _difficultyService.GetByLibelleAsync(Constantes.EasyTQ);

            #region QTypPok
            QuestionType questionType = new QuestionType()
            {
                Code = Constantes.QTypPok,
                Libelle = "Quel est ce pokémon?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokDesc
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokDesc,
                Libelle = "Trouver la description du pokémon",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokDescReverse
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokDescReverse,
                Libelle = "Trouver le bon pokémon",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTypPok
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPok,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 6,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTyp
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTyp,
                Libelle = "Quel est-ce type?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 6,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTalent
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTalent,
                Libelle = "Trouver la description du talent",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTalentReverse
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTalentReverse,
                Libelle = "Trouver le bon talent",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokStat
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokStat,
                Libelle = "Quel est la stat {0} de {1}?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTypPokVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPokVarious,
                Libelle = "Quel sont le ou les types de ce pokémon?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 6,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypWeakPokVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypWeakPokVarious,
                Libelle = "Quel sont le ou les faiblesses de ce pokémon?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 6,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokTalentVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokTalentVarious,
                Libelle = "Quel sont le ou les talents de ce pokémon?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokFamilyVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokFamilyVarious,
                Libelle = "Quel sont le ou les évolutions de ce pokémon?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokTypVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokTypVarious,
                Libelle = "Quel sont le ou les pokémons de ce type?",
                DifficultyID = difficultyEasy.Id,
                NbAnswers = 4,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion
            #endregion

            #region Normal
            Difficulty difficultyNormal = await _difficultyService.GetByLibelleAsync(Constantes.NormalTQ);

            #region QTypPok
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPok,
                Libelle = "Quel est ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 8,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokBlurred,
                Libelle = "Quel est ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                IsBlurred = true,
                NbAnswers = 8,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokBlack,
                Libelle = "Quel est ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                IsHide = true,
                NbAnswers = 8,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokDesc
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokDesc,
                Libelle = "Trouver la description du pokémon",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 6,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokDescReverse
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokDescReverse,
                Libelle = "Trouver le bon pokémon",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 6,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTypPok
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPok,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 12,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPok,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                IsBlurred = true,
                NbAnswers = 12,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPok,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                IsHide = true,
                NbAnswers = 12,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTyp
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTyp,
                Libelle = "Quel est-ce type?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 12,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTalent
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTalent,
                Libelle = "Trouver la description du talent",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 6,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTalentReverse
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTalentReverse,
                Libelle = "Trouver le bon talent",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 6,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokStat
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokStat,
                Libelle = "Quel est la stat {0} de {1}?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 8,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTypPokVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPokVarious,
                Libelle = "Quel sont le ou les types de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPokVarious,
                Libelle = "Quel sont le ou les types de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                IsBlurred = true,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPokVarious,
                Libelle = "Quel sont le ou les types de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                IsHide = true,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypWeakPokVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypWeakPokVarious,
                Libelle = "Quel sont le ou les faiblesses de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypWeakPokVarious,
                Libelle = "Quel sont le ou les faiblesses de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                IsBlurred = true,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypWeakPokVarious,
                Libelle = "Quel sont le ou les faiblesses de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                IsHide = true,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokTalentVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokTalentVarious,
                Libelle = "Quel sont le ou les talents de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 8,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokFamilyVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokFamilyVarious,
                Libelle = "Quel sont le ou les évolutions de ce pokémon?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 8,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokTypVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokTypVarious,
                Libelle = "Quel sont le ou les pokémons de ce type?",
                DifficultyID = difficultyNormal.Id,
                NbAnswers = 8,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion
            #endregion

            #region Hard
            Difficulty difficultyHard = await _difficultyService.GetByLibelleAsync(Constantes.HardTQ);

            #region QTypPok
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPok,
                Libelle = "Quel est ce pokémon?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 12,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokBlurred,
                Libelle = "Quel est ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsBlurred = true,
                IsGrayscale = true,
                NbAnswers = 12,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokBlack,
                Libelle = "Quel est ce pokémon?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 12,
                IsHide = true,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokBlack,
                Libelle = "Quel est ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsBlurred = true,
                IsHide = true,
                NbAnswers = 12,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokDesc
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokDesc,
                Libelle = "Trouver la description du pokémon",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 8,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokDescReverse
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokDescReverse,
                Libelle = "Trouver le bon pokémon",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 8,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTypPok
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPok,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 18,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPok,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsBlurred = true,
                IsGrayscale = true,
                NbAnswers = 18,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPok,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsHide = true,
                NbAnswers = 18,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPok,
                Libelle = "Quel est le type principal de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsBlurred = true,
                IsHide = true,
                NbAnswers = 18,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTyp
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTyp,
                Libelle = "Quel est-ce type?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 18,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTalent
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTalent,
                Libelle = "Trouver la description du talent",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 8,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTalentReverse
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTalentReverse,
                Libelle = "Trouver le bon talent",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 8,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokStat
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokStat,
                Libelle = "Quel est la stat {0} de {1}?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 12,
                NbAnswersPossible = 1
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypTypPokVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPokVarious,
                Libelle = "Quel sont le ou les types de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 18,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPokVarious,
                Libelle = "Quel sont le ou les types de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsBlurred = true,
                IsGrayscale = true,
                NbAnswers = 18,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPokVarious,
                Libelle = "Quel sont le ou les types de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsHide = true,
                NbAnswers = 18,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypTypPokVarious,
                Libelle = "Quel sont le ou les types de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsBlurred = true,
                IsHide = true,
                NbAnswers = 18,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypWeakPokVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypWeakPokVarious,
                Libelle = "Quel sont le ou les faiblesses de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 18,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypWeakPokVarious,
                Libelle = "Quel sont le ou les faiblesses de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsBlurred = true,
                IsGrayscale = true,
                NbAnswers = 18,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypWeakPokVarious,
                Libelle = "Quel sont le ou les faiblesses de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsHide = true,
                NbAnswers = 18,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);

            questionType = new QuestionType()
            {
                Code = Constantes.QTypWeakPokVarious,
                Libelle = "Quel sont le ou les faiblesses de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                IsBlurred = true,
                IsHide = true,
                NbAnswers = 18,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokTalentVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokTalentVarious,
                Libelle = "Quel sont le ou les talents de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokFamilyVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokFamilyVarious,
                Libelle = "Quel sont le ou les évolutions de ce pokémon?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion

            #region QTypPokTypVarious
            questionType = new QuestionType()
            {
                Code = Constantes.QTypPokTypVarious,
                Libelle = "Quel sont le ou les pokémons de ce type?",
                DifficultyID = difficultyHard.Id,
                NbAnswers = 12,
                IsMultipleAnswers = true
            };
            await CreateAsync(questionType);
            #endregion
            #endregion
        }
        #endregion

        #region Generate Quizz
        public async Task<QuestionType> GetQuestionTypeRandom(bool easy, bool normal, bool hard)
        {
            List<QuestionType> result = await GetAllAsync();
            List<QuestionType> resultFilterDifficulty = await GetQuestionTypesWithFilterDifficulty(result, easy, normal, hard);

            return await Task.FromResult(await GetQuestionTypeRandomBySelectedDifficulty(resultFilterDifficulty, easy, normal, hard));
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

        private async Task<QuestionType> GetQuestionTypeRandomBySelectedDifficulty(List<QuestionType> resultFilterDifficulty, bool easy, bool normal, bool hard)
        {
            List<QuestionType> questionTypes = new List<QuestionType>();
            QuestionType questionTypeSelected = new QuestionType();
            List<string> DifficultSelected = new List<string>();

            Random random = new Random();

            if (easy)
                DifficultSelected.Add(Constantes.EasyTQ);
            if (normal)
                DifficultSelected.Add(Constantes.NormalTQ);
            if (hard)
                DifficultSelected.Add(Constantes.HardTQ);

            if (DifficultSelected.Count.Equals(1))
            {
                if(easy)
                    questionTypeSelected = await GetQuestionTypeRandomByEasyDifficulty(resultFilterDifficulty);
                else if (normal)
                    questionTypeSelected = await GetQuestionTypeRandomByNormalDifficulty(resultFilterDifficulty);
                else if (hard)
                    questionTypeSelected = await GetQuestionTypeRandomByHardDifficulty(resultFilterDifficulty);
            }
            else
            {
                int difficultyRandom = random.Next(DifficultSelected.Count);
                if(DifficultSelected[difficultyRandom].Equals(Constantes.EasyTQ))
                    questionTypeSelected = await GetQuestionTypeRandomByEasyDifficulty(resultFilterDifficulty);
                else if (DifficultSelected[difficultyRandom].Equals(Constantes.NormalTQ))
                    questionTypeSelected = await GetQuestionTypeRandomByNormalDifficulty(resultFilterDifficulty);
                else if (DifficultSelected[difficultyRandom].Equals(Constantes.HardTQ))
                    questionTypeSelected = await GetQuestionTypeRandomByHardDifficulty(resultFilterDifficulty);
            }


            //Random random = new Random();
            //int numberRandom = random.Next(100);

            ////If Question Type => Type // 10%
            //if (numberRandom <= 3)
            //    questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokStat));
            //if (numberRandom <= 6)
            //    questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTyp));
            //else if (numberRandom <= 9)
            //    questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTalent) || m.Code.Equals(Constantes.QTypTalentReverse) || m.Code.Equals(Constantes.QTypPokTalentVarious));
            //else if (numberRandom <= 12)
            //    questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokDesc) || m.Code.Equals(Constantes.QTypPokDescReverse));
            //else if (numberRandom <= 15)
            //    questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokFamilyVarious) || m.Code.Equals(Constantes.QTypPokTypVarious));
            //else if (numberRandom <= 20)
            //    questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTypPok) || m.Code.Equals(Constantes.QTypTypPokVarious) || m.Code.Equals(Constantes.QTypWeakPokVarious));
            //else
            //    questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPok) || m.Code.Equals(Constantes.QTypPokBlurred) || m.Code.Equals(Constantes.QTypPokBlack));

            return await Task.FromResult(questionTypeSelected);
        }

        private async Task<QuestionType> GetQuestionTypeRandomByEasyDifficulty(List<QuestionType> resultFilterDifficulty)
        {
            List<QuestionType> questionTypes = new List<QuestionType>();
            Random random = new Random();
            int numberRandom = random.Next(100);

            if (numberRandom <= 3)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTyp));
            else if (numberRandom <= 6)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTalent) || m.Code.Equals(Constantes.QTypTalentReverse));
            else if (numberRandom <= 9)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokDesc) || m.Code.Equals(Constantes.QTypPokDescReverse));
            else if (numberRandom <= 12)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokFamilyVarious));
            else if (numberRandom <= 15)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTypPok));
            else
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPok));

            int numberTypeQuestion = random.Next(questionTypes.Count);
            return await Task.FromResult(questionTypes[numberTypeQuestion]);
        }

        private async Task<QuestionType> GetQuestionTypeRandomByNormalDifficulty(List<QuestionType> resultFilterDifficulty)
        {
            List<QuestionType> questionTypes = new List<QuestionType>();
            Random random = new Random();
            int numberRandom = random.Next(100);

            if (numberRandom <= 3)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTyp));
            else if (numberRandom <= 6)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTalent) || m.Code.Equals(Constantes.QTypTalentReverse) || m.Code.Equals(Constantes.QTypPokTalentVarious));
            else if (numberRandom <= 9)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokDesc) || m.Code.Equals(Constantes.QTypPokDescReverse));
            else if (numberRandom <= 12)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokFamilyVarious) || m.Code.Equals(Constantes.QTypPokTypVarious));
            else if (numberRandom <= 15)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTypPok) || m.Code.Equals(Constantes.QTypTypPokVarious));
            else
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPok) || m.Code.Equals(Constantes.QTypPokBlurred));

            int numberTypeQuestion = random.Next(questionTypes.Count);
            return await Task.FromResult(questionTypes[numberTypeQuestion]);
        }

        private async Task<QuestionType> GetQuestionTypeRandomByHardDifficulty(List<QuestionType> resultFilterDifficulty)
        {
            List<QuestionType> questionTypes = new List<QuestionType>();
            Random random = new Random();
            int numberRandom = random.Next(100);

            if (numberRandom <= 3)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokStat));
            if (numberRandom <= 6)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTyp));
            else if (numberRandom <= 9)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTalent) || m.Code.Equals(Constantes.QTypTalentReverse) || m.Code.Equals(Constantes.QTypPokTalentVarious));
            else if (numberRandom <= 12)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokDesc) || m.Code.Equals(Constantes.QTypPokDescReverse));
            else if (numberRandom <= 15)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPokFamilyVarious) || m.Code.Equals(Constantes.QTypPokTypVarious));
            else if (numberRandom <= 20)
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypTypPok) || m.Code.Equals(Constantes.QTypTypPokVarious) || m.Code.Equals(Constantes.QTypWeakPokVarious));
            else
                questionTypes = resultFilterDifficulty.FindAll(m => m.Code.Equals(Constantes.QTypPok) || m.Code.Equals(Constantes.QTypPokBlurred) || m.Code.Equals(Constantes.QTypPokBlack));

            int numberTypeQuestion = random.Next(questionTypes.Count);
            return await Task.FromResult(questionTypes[numberTypeQuestion]);
        }
        #endregion
    }
}
