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
        private readonly ITalentService _talentService;
        private readonly IAnswerService _answerService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public QuestionService(ISqliteConnectionService connectionService, ITypePokService typePokService, IPokemonService pokemonService, IQuestionTypeService questionTypeService, IAnswerService answerService, ITalentService talentService)
        {
            _connectionService = connectionService;
            _typePokService = typePokService;
            _pokemonService = pokemonService;
            _talentService = talentService;
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
            List<Pokemon> alreadyExistQTypPok = new List<Pokemon>();
            List<Pokemon> alreadyExistQTypTypPok = new List<Pokemon>();
            List<TypePok> alreadyExistQTypTyp = new List<TypePok>();
            List<Talent> alreadyExistQTypTalent = new List<Talent>();
            List<Pokemon> alreadyExistQTypPokStat = new List<Pokemon>();

            for (int nbQuestion = 0; nbQuestion < nbQuestionMax; nbQuestion++)
            {
                await Task.Run(async () =>
                {
                    QuestionType questionType = await _questionTypeService.GetQuestionTypeRandom(easy, normal, hard);

                    if (questionType.Code.Equals(Constantes.QTypPok)
                    || questionType.Code.Equals(Constantes.QTypPokBlurred)
                    || questionType.Code.Equals(Constantes.QTypPokBlack)
                    || questionType.Code.Equals(Constantes.QTypPokDescReverse))
                    {
                        AnswersID = await GetAnswersID_QTypPok(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExistQTypPok);
                        int answerID = int.Parse(AnswersID);
                        Answer answer = await _answerService.GetByIdAsync(answerID);
                        alreadyExistQTypPok.Add(await _pokemonService.GetByIdAsync(answer.IsCorrectID));
                        DataObjectID = answer.IsCorrectID;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTypPok))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExistQTypTypPok);
                        AnswersID = await GetAnswersID_QTypTypPok(questionType, pokemon, false);
                        alreadyExistQTypTypPok.Add(pokemon);
                        DataObjectID = pokemon.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTypPokVarious))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExistQTypTypPok);
                        AnswersID = await GetAnswersID_QTypTypPok(questionType, pokemon, true);
                        alreadyExistQTypTypPok.Add(pokemon);
                        DataObjectID = pokemon.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypWeakPokVarious))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExistQTypTypPok);
                        AnswersID = await GetAnswersID_QTypWeakPok(questionType, pokemon, true);
                        alreadyExistQTypTypPok.Add(pokemon);
                        DataObjectID = pokemon.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTyp))
                    {
                        AnswersID = await GetAnswersID_QTypTyp(questionType, alreadyExistQTypTyp);
                        int answerID = int.Parse(AnswersID);
                        Answer answer = await _answerService.GetByIdAsync(answerID);
                        alreadyExistQTypTyp.Add(await _typePokService.GetByIdAsync(answer.IsCorrectID));
                        DataObjectID = answer.IsCorrectID;

                        if (alreadyExistQTypTyp.Count.Equals(18))
                            alreadyExistQTypTyp = new List<TypePok>();
                    }
                    else if (questionType.Code.Equals(Constantes.QTypPokDesc))
                    {
                        AnswersID = await GetAnswersID_QTypPokDesc(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExistQTypPok);
                        int answerID = int.Parse(AnswersID);
                        Answer answer = await _answerService.GetByIdAsync(answerID);
                        alreadyExistQTypPok.Add(await _pokemonService.GetByIdAsync(answer.IsCorrectID));
                        DataObjectID = answer.IsCorrectID;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTalentPokVarious))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExistQTypTypPok);
                        AnswersID = await GetAnswersID_QTypTalentPok(questionType, pokemon);
                        alreadyExistQTypTypPok.Add(pokemon);
                        DataObjectID = pokemon.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTalent) || questionType.Code.Equals(Constantes.QTypTalentReverse))
                    {
                        AnswersID = await GetAnswersID_QTypTalent(questionType, alreadyExistQTypTalent);
                        int answerID = int.Parse(AnswersID);
                        Answer answer = await _answerService.GetByIdAsync(answerID);
                        alreadyExistQTypTalent.Add(await _talentService.GetByIdAsync(answer.IsCorrectID));
                        DataObjectID = answer.IsCorrectID;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypPokStat))
                    {
                        AnswersID = await GetAnswersID_QTypPokStat(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExistQTypPokStat);
                        int answerID = int.Parse(AnswersID);
                        Answer answer = await _answerService.GetByIdAsync(answerID);
                        alreadyExistQTypPokStat.Add(await _pokemonService.GetByIdAsync(answer.IsCorrectID));
                        DataObjectID = answer.IsCorrectID;
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

        private async Task<string> GetAnswersID_QTypPok(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, List<Pokemon> alreadyExist)
        {
            string AnswersID = string.Empty;

            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                await Task.Run(async () =>
                {
                    Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExist);
                    Pokemon pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    while (pokemonExist != null)
                    {
                        pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExist);
                        pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    }
                    pokemonsAnswer.Add(pokemon);
                });
            }

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, pokemonsAnswer); ;

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypTypPok(QuestionType questionType, Pokemon pokemon, bool various)
        {
            string AnswersID = string.Empty;

            List<Task> tasks = new List<Task>();
            List<TypePok> typesAnswer = new List<TypePok>();

            if (!various)
                typesAnswer.Add(await _typePokService.GetByIdAsync(int.Parse(pokemon.TypesID.Split(',')[0])));
            else
                foreach (string item in pokemon.TypesID.Split(','))
                    typesAnswer.Add(await _typePokService.GetByIdAsync(int.Parse(item)));

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, typesAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypWeakPok(QuestionType questionType, Pokemon pokemon, bool various)
        {
            string AnswersID = string.Empty;

            List<Task> tasks = new List<Task>();
            List<TypePok> weaknessAnswer = new List<TypePok>();

            if (!various)
                weaknessAnswer.Add(await _typePokService.GetByIdAsync(int.Parse(pokemon.WeaknessID.Split(',')[0])));
            else
                foreach (string item in pokemon.WeaknessID.Split(','))
                    weaknessAnswer.Add(await _typePokService.GetByIdAsync(int.Parse(item)));

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, weaknessAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypTyp(QuestionType questionType, List<TypePok> alreadySelected)
        {
            string AnswersID = string.Empty;

            List<Task> tasks = new List<Task>();
            List<TypePok> typesAnswer = new List<TypePok>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                tasks.Add(
                    Task.Run(async () =>
                    {
                        typesAnswer.Add(await _typePokService.GetTypeRandom(alreadySelected));
                    })
                );
            }

            Task.WaitAll(tasks.ToArray());
            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, typesAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypPokDesc(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, List<Pokemon> alreadyExist)
        {
            string AnswersID = string.Empty;

            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                await Task.Run(async () =>
                {
                    Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExist);
                    Pokemon pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    while (pokemonExist != null)
                    {
                        pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExist);
                        pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    }
                    pokemonsAnswer.Add(pokemon);
                });
            }

            AnswersID = await _answerService.GenerateCorrectAnswersDesc(questionType, pokemonsAnswer); ;

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypTalentPok(QuestionType questionType, Pokemon pokemon)
        {
            string AnswersID = string.Empty;

            List<Task> tasks = new List<Task>();
            List<Talent> talentsAnswer = new List<Talent>();

            if(pokemon.TalentsID != null)
                foreach (string item in pokemon.TalentsID.Split(','))
                    talentsAnswer.Add(await _talentService.GetByIdAsync(int.Parse(item)));

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, talentsAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypTalent(QuestionType questionType, List<Talent> alreadySelected)
        {
            string AnswersID = string.Empty;

            List<Task> tasks = new List<Task>();
            List<Talent> talentsAnswer = new List<Talent>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                tasks.Add(
                    Task.Run(async () =>
                    {
                        talentsAnswer.Add(await _talentService.GetTalentRandom(alreadySelected));
                    })
                );
            }

            Task.WaitAll(tasks.ToArray());
            if(questionType.Code.Equals(Constantes.QTypTalent))
                AnswersID = await _answerService.GenerateCorrectAnswers(questionType, talentsAnswer, false);
            else if (questionType.Code.Equals(Constantes.QTypTalentReverse))
                AnswersID = await _answerService.GenerateCorrectAnswers(questionType, talentsAnswer, true);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypPokStat(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, List<Pokemon> alreadyExist)
        {
            string AnswersID = string.Empty;

            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                await Task.Run(async () =>
                {
                    Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExist);
                    Pokemon pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    while (pokemonExist != null)
                    {
                        pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, genArceus, alreadyExist);
                        pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    }
                    pokemonsAnswer.Add(pokemon);
                });
            }

            string typeStat = GetRandomTypeStat();
            AnswersID = await _answerService.GenerateCorrectAnswersStat(questionType, pokemonsAnswer, typeStat);

            return await Task.FromResult(AnswersID);
        }

        private string GetRandomTypeStat()
        {
            Random random = new Random();
            int numberRandom = random.Next(6);

            string typeStat = "";

            switch (numberRandom)
            {
                case 0: typeStat = Constantes.Pv; break;
                case 1: typeStat = Constantes.Attaque; break;
                case 2: typeStat = Constantes.Defense; break;
                case 3: typeStat = Constantes.AttaqueSpe; break;
                case 4: typeStat = Constantes.DefenseSpe; break;
                case 5: typeStat = Constantes.Vitesse; break;
            }

            return typeStat;
        }
        #endregion
    }
}
