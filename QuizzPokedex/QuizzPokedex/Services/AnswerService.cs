using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
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
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public AnswerService(ISqliteConnectionService connectionService, IPokemonService pokemonService, ITypePokService typePokService)
        {
            _connectionService = connectionService;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
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
        #region Quizz
        public async Task<List<Answer>> GenerateAnswers(Quizz quizz, QuestionType questionType, List<Answer> answers)
        {
            if (questionType.Code.Equals(Constantes.QTypPok) 
                || questionType.Code.Equals(Constantes.QTypPokBlurred)
                || questionType.Code.Equals(Constantes.QTypPokBlack)
                || questionType.Code.Equals(Constantes.QTypPokDescReverse))
            {
                List<Pokemon> pokemons = new List<Pokemon>();
                foreach (Answer item in answers)
                {
                    pokemons.Add(await _pokemonService.GetByIdAsync(item.IsCorrectID));
                }

                int qMissing = questionType.NbAnswers - answers.Count;

                for (int i = 0; i < qMissing; i++)
                {
                    pokemons.Add(await _pokemonService.GetPokemonRandom(quizz.Gen1, quizz.Gen2, quizz.Gen3, quizz.Gen4, quizz.Gen5, quizz.Gen6, quizz.Gen7, quizz.Gen8, quizz.GenArceus, pokemons));
                }

                answers = await GenerateAnswers(questionType, pokemons, answers);
            }
            else if (questionType.Code.Equals(Constantes.QTypTypPok) || questionType.Code.Equals(Constantes.QTypTyp))
            {
                List<TypePok> typePoks = new List<TypePok>();
                foreach (Answer item in answers)
                {
                    typePoks.Add(await _typePokService.GetByIdAsync(item.IsCorrectID));
                }

                int qMissing = questionType.NbAnswers - answers.Count;

                for (int i = 0; i < qMissing; i++)
                {
                    typePoks.Add(await _typePokService.GetTypeRandom(typePoks));
                }

                answers = await GenerateAnswers(questionType, typePoks, answers);
            }
            else if (questionType.Code.Equals(Constantes.QTypPokDesc))
            {
                List<Pokemon> pokemons = new List<Pokemon>();
                foreach (Answer item in answers)
                {
                    pokemons.Add(await _pokemonService.GetByIdAsync(item.IsCorrectID));
                }

                int qMissing = questionType.NbAnswers - answers.Count;

                for (int i = 0; i < qMissing; i++)
                {
                    pokemons.Add(await _pokemonService.GetPokemonRandom(quizz.Gen1, quizz.Gen2, quizz.Gen3, quizz.Gen4, quizz.Gen5, quizz.Gen6, quizz.Gen7, quizz.Gen8, quizz.GenArceus, pokemons));
                }

                answers = await GenerateAnswersDesc(questionType, pokemons, answers);
            }

            return await Task.FromResult(answers);
        }
        #endregion

        #region Quizz Pokemon
        public async Task<string> GenerateCorrectAnswers(QuestionType questionType, List<Pokemon> pokemonsAnswer)
        {
            List<Answer> answers = new List<Answer>();
            Random random = new Random();

            string result = string.Empty;
            Dictionary<int, Pokemon> dic = new Dictionary<int, Pokemon>();
            foreach (var pokemon in pokemonsAnswer)
            {
                while (!dic.ContainsValue(pokemon))
                {
                    int numberRandom = random.Next(questionType.NbAnswers);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, pokemon);
                }
            }

            foreach (KeyValuePair<int, Pokemon> pair in dic)
            {
                Answer answer = new Answer()
                {
                    IsSelected = false,
                    IsCorrect = true,
                    IsCorrectID = pair.Value.Id,
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

        private async Task<List<Answer>> GenerateAnswers(QuestionType questionType, List<Pokemon> pokemons, List<Answer> pokemonsAnswer)
        {
            Random random = new Random();

            string result = string.Empty;
            Dictionary<int, Pokemon> dic = new Dictionary<int, Pokemon>();

            foreach (var pokemon in pokemons)
            {
                while (!dic.ContainsValue(pokemon))
                {
                    int numberRandom = random.Next(questionType.NbAnswers);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, pokemon);
                }
            }

            foreach (KeyValuePair<int, Pokemon> pair in dic)
            {
                Answer answerExist = pokemonsAnswer.Find(m => m.IsCorrectID.Equals(pair.Value.Id));
                if (answerExist == null)
                {
                    Answer answer = new Answer()
                    {
                        IsSelected = false,
                        IsCorrect = false,
                        IsCorrectID = -1,
                        Libelle = pair.Value.Name,
                        Order = pair.Key + 1
                    };

                    pokemonsAnswer.Add(answer);
                }
                else
                {
                    answerExist.Order = pair.Key + 1;
                    await UpdateAsync(answerExist);
                }
            }

            return await Task.FromResult(pokemonsAnswer);
        }
        #endregion

        #region Quizz Type
        public async Task<string> GenerateCorrectAnswers(QuestionType questionType, List<TypePok> typesAnswer)
        {
            List<Answer> answers = new List<Answer>();
            Random random = new Random();

            string result = string.Empty;
            Dictionary<int, TypePok> dic = new Dictionary<int, TypePok>();
            foreach (var type in typesAnswer)
            {
                while (!dic.ContainsValue(type))
                {
                    int numberRandom = random.Next(questionType.NbAnswers);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, type);
                }
            }

            foreach (KeyValuePair<int, TypePok> pair in dic)
            {
                Answer answer = new Answer()
                {
                    IsSelected = false,
                    IsCorrect = true,
                    IsCorrectID = pair.Value.Id,
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

        private async Task<List<Answer>> GenerateAnswers(QuestionType questionType, List<TypePok> typePoks, List<Answer> typesAnswer)
        {
            Random random = new Random();

            string result = string.Empty;
            Dictionary<int, TypePok> dic = new Dictionary<int, TypePok>();

            foreach (var type in typePoks)
            {
                while (!dic.ContainsValue(type))
                {
                    int numberRandom = random.Next(questionType.NbAnswers);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, type);
                }
            }

            foreach (KeyValuePair<int, TypePok> pair in dic)
            {
                Answer answerExist = typesAnswer.Find(m => m.IsCorrectID.Equals(pair.Value.Id));
                if (answerExist == null)
                {
                    Answer answer = new Answer()
                    {
                        IsSelected = false,
                        IsCorrect = false,
                        IsCorrectID = -1,
                        Libelle = pair.Value.Name,
                        Order = pair.Key + 1
                    };

                    typesAnswer.Add(answer);
                }
                else
                {
                    answerExist.Order = pair.Key + 1;
                    await UpdateAsync(answerExist);
                }
            }

            return await Task.FromResult(typesAnswer);
        }
        #endregion

        #region Quizz Description
        public async Task<string> GenerateCorrectAnswersDesc(QuestionType questionType, List<Pokemon> pokemonsAnswer)
        {
            List<Answer> answers = new List<Answer>();
            Random random = new Random();

            string result = string.Empty;
            Dictionary<int, Pokemon> dic = new Dictionary<int, Pokemon>();
            foreach (var pokemon in pokemonsAnswer)
            {
                while (!dic.ContainsValue(pokemon))
                {
                    int numberRandom = random.Next(questionType.NbAnswers);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, pokemon);
                }
            }

            foreach (KeyValuePair<int, Pokemon> pair in dic)
            {
                string description = await ConvertDescription(pair.Value);
                Answer answer = new Answer()
                {
                    IsSelected = false,
                    IsCorrect = true,
                    IsCorrectID = pair.Value.Id,
                    Libelle = description,
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

        private async Task<List<Answer>> GenerateAnswersDesc(QuestionType questionType, List<Pokemon> pokemons, List<Answer> pokemonsAnswer)
        {
            Random random = new Random();

            string result = string.Empty;
            Dictionary<int, Pokemon> dic = new Dictionary<int, Pokemon>();

            foreach (var pokemon in pokemons)
            {
                while (!dic.ContainsValue(pokemon))
                {
                    int numberRandom = random.Next(questionType.NbAnswers);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, pokemon);
                }
            }

            foreach (KeyValuePair<int, Pokemon> pair in dic)
            {
                Answer answerExist = pokemonsAnswer.Find(m => m.IsCorrectID.Equals(pair.Value.Id));
                if (answerExist == null)
                {
                    string description = await ConvertDescription(pair.Value);
                    Answer answer = new Answer()
                    {
                        IsSelected = false,
                        IsCorrect = false,
                        IsCorrectID = -1,
                        Libelle = description,
                        Order = pair.Key + 1
                    };

                    pokemonsAnswer.Add(answer);
                }
                else
                {
                    answerExist.Order = pair.Key + 1;
                    await UpdateAsync(answerExist);
                }
            }

            return await Task.FromResult(pokemonsAnswer);
        }

        private async Task<string> ConvertDescription(Pokemon pokemon)
        {
            string description = "";
            if (pokemon.DescriptionVx.Contains(pokemon.DisplayName))
            {
                description = await CheckAndConvert(pokemon.DescriptionVx, pokemon.Evolutions);
            }
            else if (pokemon.DescriptionVx.Contains(pokemon.DisplayName))
            {
                description = await CheckAndConvert(pokemon.DescriptionVy, pokemon.Evolutions);
            }
            else
            {
                description = pokemon.DescriptionVx;
            }

            return await Task.FromResult(description);
        }

        private async Task<string> CheckAndConvert(string description, string family)
        {
            try
            {
                List<Pokemon> pokemons = new List<Pokemon>();
                foreach (string idPok in family.Split(','))
                {
                    int id = int.Parse(idPok);
                    pokemons.Add(await _pokemonService.GetByIdAsync(id));
                }

                //string[] descSplit = description.Split(' ');

                //int index = -1;
                //for (int i = 0; i < descSplit.Length; i++)
                //{
                //    if (descSplit[i].Contains(pokemonName))
                //    {
                //        index = i;
                //        break;
                //    }
                //}

                //if (!index.Equals(-1))
                foreach (Pokemon item in pokemons)
                {
                    description = description.Replace(item.Name, "[...]");
                }

                return await Task.FromResult(description);
            } 
            catch(Exception ex) 
            {
                throw ex;
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
