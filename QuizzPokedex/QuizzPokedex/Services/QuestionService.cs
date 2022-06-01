using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly ISqliteConnectionService _connectionService;
        private readonly IPokemonService _pokemonService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly IAnswerService _answerService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();

        public QuestionService(ISqliteConnectionService connectionService, IPokemonService pokemonService, IQuestionTypeService questionTypeService, IAnswerService answerService)
        {
            _connectionService = connectionService;
            _pokemonService = pokemonService;
            _questionTypeService = questionTypeService;
            _answerService = answerService;
        }

        public async Task<List<Question>> GetAllAsync()
        {
            var result = await _database.Table<Question>().ToListAsync();
            return result;
        }

        public async Task<List<Question>> GetAllByQuestionsIDAsync(string QuestionsID)
        {
            List<Question> result = new List<Question>();
            foreach (string item in QuestionsID.Split(','))
            {
                int id = int.Parse(item);
                result.Add(await GetByIdAsync(id));
            }
            
            return await Task.FromResult(result);
        }

        public async Task<Question> GetByIdAsync(int id)
        {
            var result = await _database.Table<Question>().ToListAsync();
            return result.Find(m => m.Id.Equals(id));
        }

        public async Task<int> CreateAsync(Question question)
        {
            var result = await _database.InsertAsync(question);
            return result;
        }

        public async Task<int> DeleteAsync(Question question)
        {
            var result = await _database.DeleteAsync(question);
            return result;
        }

        public async Task<int> UpdateAsync(Question question)
        {
            var result = await _database.InsertOrReplaceAsync(question);
            return result;
        }

        public async Task<string> GenerateQuestions(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, bool easy, bool normal, bool hard)
        {
            int nbQuestionMax = 10;
            int nbAnswerMax = 4;

            string result = string.Empty;
            List<Question> questions = new List<Question>();
            
            for (int nbQuestion = 0; nbQuestion < nbQuestionMax; nbQuestion++)
            {
                List<Pokemon> pokemonsAnswer = new List<Pokemon>();
                for (int nbAnswer = 0; nbAnswer < nbAnswerMax; nbAnswer++)
                {
                    pokemonsAnswer.Add(await _pokemonService.getPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus));
                }
                
                Question question = new Question()
                {
                    Order = nbQuestion + 1,
                    AnswersID = await _answerService.GenerateAnswers(pokemonsAnswer),
                    QuestionTypeID = await _questionTypeService.GetQuestionTypeRandom(easy, normal, hard)
                };

                await CreateAsync(question);
                questions.Add(question);
            }

            int i = 0;
            foreach (Question question in questions)
            {
                if (i == 0)
                {
                    result = question.Id.ToString();
                    i++;
                }
                else
                {
                    result += ',' + question.Id.ToString();
                }
            }

            return await Task.FromResult(result);
        }
    }
}
