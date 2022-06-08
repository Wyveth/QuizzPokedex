using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class AnswerService : IAnswerService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public AnswerService(ISqliteConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
        public async Task<List<Answer>> GetAllAsync()
        {
            var result = await _database.Table<Answer>().ToListAsync();
            return result;
        }

        public async Task<List<Answer>> GetAllByAnswersIDAsync(string answersID)
        {
            List<Answer> result = new List<Answer>();
            foreach (string item in answersID.Split(','))
            {
                int id = int.Parse(item);
                result.Add(await GetByIdAsync(id));
            }

            return await Task.FromResult(result);
        }

        public async Task<Answer> GetByIdAsync(int id)
        {
            var result = await _database.Table<Answer>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }
        #endregion

        public async Task<int> CreateAsync(Answer answer)
        {
            var result = await _database.InsertAsync(answer);
            return result;
        }

        public async Task<int> DeleteAsync(Answer answer)
        {
            var result = await _database.DeleteAsync(answer);
            return result;
        }

        public async Task<int> UpdateAsync(Answer answer)
        {
            var result = await _database.InsertOrReplaceAsync(answer);
            return result;
        }
        #endregion

        #region Generate Quizz
        public async Task<string> GenerateAnswers(List<Pokemon> pokemonsAnswer)
        {
            List<Answer> answers = new List<Answer>();
            Random random = new Random();

            string result = string.Empty;
            int selectedPokemonAnswer = 0;
            Dictionary<int, Pokemon> dic = new Dictionary<int, Pokemon>();
            foreach (var pokemon in pokemonsAnswer)
            {
                while (!dic.ContainsValue(pokemon))
                {
                    int numberRandom = random.Next(pokemonsAnswer.Count);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, pokemon);
                }
            }

            selectedPokemonAnswer = random.Next(pokemonsAnswer.Count);

            foreach (KeyValuePair<int, Pokemon> pair in dic)
            {
                int IsCorrectId = -1;
                bool IsCorrectAnswer = false;

                if (dic[selectedPokemonAnswer].Id.Equals(pair.Value.Id))
                {
                    IsCorrectId = pair.Value.Id;
                    IsCorrectAnswer = true;
                }

                Answer answer = new Answer()
                {
                    IsSelected = false,
                    IsCorrect = IsCorrectAnswer,
                    IsCorrectID = IsCorrectId,
                    Libelle = pair.Value.Name,
                    Order = pair.Key + 1
                };

                await CreateAsync(answer);
                answers.Add(answer);
            }

            int i = 0;
            foreach (Answer answer in answers)
            {
                if (i == 0)
                {
                    result = answer.Id.ToString();
                    i++;
                }
                else
                {
                    result += ',' + answer.Id.ToString();
                }
            }

            return await Task.FromResult(result);
        }

        public async Task<string> GenerateAnswers(List<TypePok> typesAnswer)
        {
            List<Answer> answers = new List<Answer>();
            Random random = new Random();

            string result = string.Empty;
            int selectedTypeAnswer = 0;
            Dictionary<int, TypePok> dic = new Dictionary<int, TypePok>();
            foreach (var type in typesAnswer)
            {
                while (!dic.ContainsValue(type))
                {
                    int numberRandom = random.Next(typesAnswer.Count);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, type);
                }
            }

            selectedTypeAnswer = random.Next(typesAnswer.Count);

            foreach (KeyValuePair<int, TypePok> pair in dic)
            {
                bool IsCorrectAnswer = false;

                if (dic[selectedTypeAnswer].Id.Equals(pair.Value.Id))
                {
                    IsCorrectAnswer = true;
                }

                Answer answer = new Answer()
                {
                    IsSelected = false,
                    IsCorrect = IsCorrectAnswer,
                    Libelle = pair.Value.Name,
                    Order = pair.Key + 1
                };

                await CreateAsync(answer);
                answers.Add(answer);
            }

            int i = 0;
            foreach (Answer answer in answers)
            {
                if (i == 0)
                {
                    result = answer.Id.ToString();
                    i++;
                }
                else
                {
                    result += ',' + answer.Id.ToString();
                }
            }

            return await Task.FromResult(result);
        }
        #endregion
        #endregion
    }
}
