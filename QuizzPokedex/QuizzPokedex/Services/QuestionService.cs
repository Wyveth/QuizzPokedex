using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Services
{
    public class QuestionService : IQuestionService
    {
        #region Fields
        private readonly ISqliteConnectionService _connectionService;
        private readonly ITypePokService _typePokService;
        private readonly IPokemonService _pokemonService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly IAnswerService _answerService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public QuestionService(ISqliteConnectionService connectionService, ITypePokService typePokService, IPokemonService pokemonService, IQuestionTypeService questionTypeService, IAnswerService answerService)
        {
            _connectionService = connectionService;
            _typePokService = typePokService;
            _pokemonService = pokemonService;
            _questionTypeService = questionTypeService;
            _answerService = answerService;
        }
        #endregion

        #region Public Methods
        #region CRUD
        #region Get Data
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
        #endregion

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
        #endregion

        #region Generate Quizz
        public async Task<string> GenerateQuestions(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, bool easy, bool normal, bool hard)
        {
            int nbQuestionMax = 10;

            string result = string.Empty;
            List<Question> questions = new List<Question>();

            for (int nbQuestion = 0; nbQuestion < nbQuestionMax; nbQuestion++)
            {
                QuestionType questionType = await _questionTypeService.GetQuestionTypeRandom(easy, normal, hard);

                string AnswersID = await GetAnswersID(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus);

                Question question = new Question()
                {
                    Order = nbQuestion + 1,
                    AnswersID = AnswersID,
                    QuestionTypeID = questionType.Id
                };

                await CreateAsync(question);
                questions.Add(question);
            }

            return await Task.FromResult(await GetQuestionsID(questions));
        }
        #endregion
        #endregion

        #region Private Methods
        private async Task<string> GetQuestionsID(List<Question> questions)
        {
            string result = string.Empty;
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
        private async Task<string> GetAnswersID(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus)
        {
            string AnswersID = string.Empty;
            if (questionType.Code.Equals(Constantes.QTypTyp))
            {
                List<TypePok> typesAnswer = new List<TypePok>();
                for (int nbAnswer = 0; nbAnswer < questionType.NbAnswers; nbAnswer++)
                {
                    typesAnswer.Add(await _typePokService.GetTypeRandom(typesAnswer));
                }
                AnswersID = await _answerService.GenerateAnswers(typesAnswer);
            }
            else
            {
                List<Pokemon> pokemonsAnswer = new List<Pokemon>();
                for (int nbAnswer = 0; nbAnswer < questionType.NbAnswers; nbAnswer++)
                {
                    pokemonsAnswer.Add(await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, pokemonsAnswer));
                }
                AnswersID = await _answerService.GenerateAnswers(pokemonsAnswer);
            }

            return await Task.FromResult(AnswersID);
        }
        #endregion
    }
}
