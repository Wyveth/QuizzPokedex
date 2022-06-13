using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<int> GetAllByQuestionsIDResumeAsync(string[] QuestionsID)
        {
            List<Question> result = new List<Question>();
            foreach (string item in QuestionsID)
            {
                int id = int.Parse(item);
                result.Add(await GetByIdAsync(id));
            }

            return await Task.FromResult(result.FindAll(m => m.Done.Equals(true)).Count);
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

        public async Task<int> GetCountAsync()
        {
            var result = await _database.Table<Question>().CountAsync();
            return result;
        }
        #endregion

        #region Generate Quizz
        public async Task<string> GenerateQuestions(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, bool easy, bool normal, bool hard)
        {
            int nbQuestionMax = await GetNbQuestionByDifficulty(easy, normal, hard);

            string AnswersID = string.Empty;
            int DataObjectID = 0;
            List<Question> questions = new List<Question>();

            for (int nbQuestion = 0; nbQuestion < nbQuestionMax; nbQuestion++)
            {
                await Task.Run(async () =>
                {
                    QuestionType questionType = await _questionTypeService.GetQuestionTypeRandom(easy, normal, hard);

                    if (questionType.Code.Equals(Constantes.QTypPok))
                    {
                        AnswersID = await GetAnswersID_QTypPok(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus);
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTypPok))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus);
                        AnswersID = await GetAnswersID_QTypTypePok(questionType, pokemon);
                        DataObjectID = pokemon.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTyp))
                    {
                        AnswersID = await GetAnswersID_QTypTyp(questionType);
                    }

                    Question question = new Question()
                    {
                        Order = nbQuestion + 1,
                        AnswersID = AnswersID,
                        DataObjectID = DataObjectID,
                        QuestionTypeID = questionType.Id,
                        Done = false
                    };

                    await CreateAsync(question);
                    questions.Add(question);
                    Debug.Write("Creation Question:" + questions.Count + "/" + nbQuestionMax);
                });
            }
            return await Task.FromResult(await GetQuestionsID(questions));
        }

        public async Task<int> GetNbQuestionByDifficulty(bool easy, bool normal, bool hard)
        {
            int nbQuestionMax = 0;
            if (easy)
                nbQuestionMax = 10;
            else if (normal)
                nbQuestionMax = 15;
            else if (hard)
                nbQuestionMax = 20;

            return await Task.FromResult(nbQuestionMax);
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

        private async Task<string> GetAnswersID_QTypPok(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus)
        {
            string AnswersID = string.Empty;

            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                await Task.Run(async () =>
                {
                    pokemonsAnswer.Add(await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, pokemonsAnswer));
                });
            }

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, pokemonsAnswer); ;

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypTypePok(QuestionType questionType, Pokemon pokemon)
        {
            string AnswersID = string.Empty;

            List<Task> tasks = new List<Task>();
            List<TypePok> typesAnswer = new List<TypePok>();

            typesAnswer.Add(await _typePokService.GetByIdAsync(int.Parse(pokemon.TypesID.Split(',')[0])));

            for (int nbAnswer = 1; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                tasks.Add(
                    Task.Run(async () =>
                    {
                        typesAnswer.Add(await _typePokService.GetTypeRandom(typesAnswer));
                    })
                );
            }

            Task.WaitAll(tasks.ToArray());
            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, typesAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypTyp(QuestionType questionType)
        {
            string AnswersID = string.Empty;

            List<Task> tasks = new List<Task>();
            List<TypePok> typesAnswer = new List<TypePok>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                tasks.Add(
                    Task.Run(async () =>
                    {
                        typesAnswer.Add(await _typePokService.GetTypeRandom(typesAnswer));
                    })
                );
            }

            Task.WaitAll(tasks.ToArray());
            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, typesAnswer);

            return await Task.FromResult(AnswersID);
        }
        #endregion
    }
}
