﻿using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly IPokemonTypePokService _pokemonTypePokService;
        private readonly IPokemonWeaknessService _pokemonWeaknessService;
        private readonly IPokemonTalentService _pokemonTalentService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly ITalentService _talentService;
        private readonly IAnswerService _answerService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public QuestionService(ISqliteConnectionService connectionService, ITypePokService typePokService, IPokemonService pokemonService, IPokemonTypePokService pokemonTypePokService, IPokemonWeaknessService pokemonWeaknessService, IPokemonTalentService pokemonTalentService, IQuestionTypeService questionTypeService, IAnswerService answerService, ITalentService talentService)
        {
            _connectionService = connectionService;
            _typePokService = typePokService;
            _pokemonService = pokemonService;
            _pokemonTypePokService = pokemonTypePokService;
            _pokemonWeaknessService = pokemonWeaknessService;
            _pokemonTalentService = pokemonTalentService;
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

        public async Task<List<Question>> GetAllByQuestionsIDAsync(string questionsID)
        {
            List<Question> result = new List<Question>();
            foreach (string item in questionsID.Split(','))
            {
                int id = int.Parse(item);
                result.Add(await GetByIdAsync(id));
            }

            return await Task.FromResult(result);
        }

        public async Task<int> GetAllByQuestionsIDResumeAsync(string[] questionsID)
        {
            List<Question> result = new List<Question>();
            foreach (string item in questionsID)
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
        public async Task<string> GenerateQuestions(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, bool easy, bool normal, bool hard)
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
                        AnswersID = await GetAnswersID_QTypPok(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExistQTypPok);
                        int answerID = int.Parse(AnswersID);
                        Answer answer = await _answerService.GetByIdAsync(answerID);
                        alreadyExistQTypPok.Add(await _pokemonService.GetByIdAsync(answer.IsCorrectID));
                        DataObjectID = answer.IsCorrectID;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypPokFamilyVarious))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExistQTypTypPok);
                        AnswersID = await GetAnswersID_QTypPokFamily(questionType, pokemon);
                        alreadyExistQTypTypPok.Add(pokemon);
                        DataObjectID = pokemon.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypPokTypVarious))
                    {
                        TypePok typePok = await _typePokService.GetTypeRandom();
                        AnswersID = await GetAnswersID_QTypPok(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, typePok, alreadyExistQTypTyp);
                        alreadyExistQTypTyp.Add(typePok);
                        DataObjectID = typePok.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTypPok))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExistQTypTypPok);
                        AnswersID = await GetAnswersID_QTypTypPok(questionType, pokemon, false);
                        alreadyExistQTypTypPok.Add(pokemon);
                        DataObjectID = pokemon.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypTypPokVarious))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExistQTypTypPok);
                        AnswersID = await GetAnswersID_QTypTypPok(questionType, pokemon, true);
                        alreadyExistQTypTypPok.Add(pokemon);
                        DataObjectID = pokemon.Id;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypWeakPokVarious))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExistQTypTypPok);
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
                        AnswersID = await GetAnswersID_QTypPokDesc(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExistQTypPok);
                        int answerID = int.Parse(AnswersID);
                        Answer answer = await _answerService.GetByIdAsync(answerID);
                        alreadyExistQTypPok.Add(await _pokemonService.GetByIdAsync(answer.IsCorrectID));
                        DataObjectID = answer.IsCorrectID;
                    }
                    else if (questionType.Code.Equals(Constantes.QTypPokTalentVarious))
                    {
                        Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExistQTypTypPok);
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
                        AnswersID = await GetAnswersID_QTypPokStat(questionType, gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExistQTypPokStat);
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
            StringBuilder result = new StringBuilder();
            int i = 0;
            foreach (int id in questions.Select(m => m.Id))
            {
                if (i == 0)
                {
                    result.Append(id.ToString());
                    i++;
                }
                else
                {
                    result.Append(',' + id.ToString());
                }
            }

            return await Task.FromResult(result.ToString());
        }

        private async Task<string> GetAnswersID_QTypPok(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, List<Pokemon> alreadyExist)
        {
            string AnswersID = string.Empty;

            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                await Task.Run(async () =>
                {
                    Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExist);
                    Pokemon pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    while (pokemonExist != null)
                    {
                        pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExist);
                        pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    }
                    pokemonsAnswer.Add(pokemon);
                });
            }

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, pokemonsAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypPokFamily(QuestionType questionType, Pokemon pokemon)
        {
            string AnswersID = string.Empty;
            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            if (pokemon.Evolutions != null)
            {
                foreach (var item in pokemon.Evolutions.Split(','))
                {
                    if (!item.Equals(pokemon.Id.ToString()))
                        pokemonsAnswer.Add(await _pokemonService.GetByIdAsync(int.Parse(item)));
                }
            }

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, pokemonsAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypPok(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, TypePok typePok, List<TypePok> alreadyExistQTypTyp)
        {
            string AnswersID = string.Empty;
            Random random = new Random();
            int nbAnswerCorrectRandom = random.Next(questionType.NbAnswers);
            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            for (int i = 0; i < nbAnswerCorrectRandom; i++)
            {
                Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, typePok, pokemonsAnswer);
                if (pokemonsAnswer.Find(m => m.Name.Equals(pokemon.Name)) == null)
                    pokemonsAnswer.Add(pokemon);
                else
                    break;
            }

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, pokemonsAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypTypPok(QuestionType questionType, Pokemon pokemon, bool various)
        {
            string AnswersID = string.Empty;

            List<TypePok> typesAnswer = new List<TypePok>();
            List<PokemonTypePok> pokemonTypePoks = await _pokemonTypePokService.GetTypesPokByPokemon(pokemon.Id);

            if (!various)
                typesAnswer.Add(await _typePokService.GetByIdAsync(pokemonTypePoks[0].TypePokId));
            else
                foreach (PokemonTypePok item in pokemonTypePoks)
                    typesAnswer.Add(await _typePokService.GetByIdAsync(item.TypePokId));

            AnswersID = await _answerService.GenerateCorrectAnswers(questionType, typesAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypWeakPok(QuestionType questionType, Pokemon pokemon, bool various)
        {
            string AnswersID = string.Empty;

            List<TypePok> weaknessAnswer = new List<TypePok>();
            List<PokemonWeakness> pokemonWeaknesses = await _pokemonWeaknessService.GetWeaknessesByPokemon(pokemon.Id);
            
            if (!various)
                weaknessAnswer.Add(await _typePokService.GetByIdAsync(pokemonWeaknesses[0].TypePokId));
            else
                foreach (PokemonWeakness item in pokemonWeaknesses)
                    weaknessAnswer.Add(await _typePokService.GetByIdAsync(item.TypePokId));

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

        private async Task<string> GetAnswersID_QTypPokDesc(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, List<Pokemon> alreadyExist)
        {
            string AnswersID = string.Empty;

            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                await Task.Run(async () =>
                {
                    Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExist);
                    Pokemon pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    while (pokemonExist != null)
                    {
                        pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExist);
                        pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    }
                    pokemonsAnswer.Add(pokemon);
                });
            }

            AnswersID = await _answerService.GenerateCorrectAnswersDesc(questionType, pokemonsAnswer);

            return await Task.FromResult(AnswersID);
        }

        private async Task<string> GetAnswersID_QTypTalentPok(QuestionType questionType, Pokemon pokemon)
        {
            string AnswersID = string.Empty;

            List<Talent> talentsAnswer = new List<Talent>();
            List<PokemonTalent> pokemonTalentServices = await _pokemonTalentService.GetTalentsByPokemon(pokemon.Id);

            if (pokemonTalentServices != null)
                foreach (PokemonTalent item in pokemonTalentServices)
                    talentsAnswer.Add(await _talentService.GetByIdAsync(item.TalentId));

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

        private async Task<string> GetAnswersID_QTypPokStat(QuestionType questionType, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, List<Pokemon> alreadyExist)
        {
            string AnswersID = string.Empty;

            List<Pokemon> pokemonsAnswer = new List<Pokemon>();

            for (int nbAnswer = 0; nbAnswer < questionType.NbAnswersPossible; nbAnswer++)
            {
                await Task.Run(async () =>
                {
                    Pokemon pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExist);
                    Pokemon pokemonExist = alreadyExist.Find(m => m.Id.Equals(pokemon.Id));
                    while (pokemonExist != null)
                    {
                        pokemon = await _pokemonService.GetPokemonRandom(gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, genArceus, alreadyExist);
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
