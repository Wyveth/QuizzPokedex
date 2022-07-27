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
        private readonly ITalentService _talentService;
        private SQLite.SQLiteAsyncConnection _database => _connectionService.GetAsyncConnection();
        #endregion

        #region Constructor
        public AnswerService(ISqliteConnectionService connectionService, IPokemonService pokemonService, ITypePokService typePokService, ITalentService talentService)
        {
            _connectionService = connectionService;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
            _talentService = talentService;
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
            else if (questionType.Code.Equals(Constantes.QTypTypPok) || questionType.Code.Equals(Constantes.QTypTyp) || questionType.Code.Equals(Constantes.QTypTypPokVarious))
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
            else if (questionType.Code.Equals(Constantes.QTypTalent) || questionType.Code.Equals(Constantes.QTypTalentReverse))
            {
                List<Talent> talents = new List<Talent>();
                foreach (Answer item in answers)
                {
                    talents.Add(await _talentService.GetByIdAsync(item.IsCorrectID));
                }

                int qMissing = questionType.NbAnswers - answers.Count;

                for (int i = 0; i < qMissing; i++)
                {
                    talents.Add(await _talentService.GetTalentRandom(talents));
                }

                if (questionType.Code.Equals(Constantes.QTypTalent))
                    answers = await GenerateAnswers(questionType, talents, answers);
                else if (questionType.Code.Equals(Constantes.QTypTalentReverse))
                    answers = await GenerateAnswers(questionType, talents, answers, true);
            }
            else if (questionType.Code.Equals(Constantes.QTypPokStat))
            {
                Random random = new Random();
                List<Pokemon> pokemons = new List<Pokemon>();
                List<string> statAnswers = new List<string>();
                int stat = 0;
                string typeStat = "";

                foreach (Answer item in answers)
                {
                    typeStat = item.Type;
                    stat = int.Parse(item.Libelle);
                    statAnswers.Add(item.Libelle);
                    pokemons.Add(await _pokemonService.GetByIdAsync(item.IsCorrectID));
                }

                int minValue = 0;
                int maxValue = 255;

                if (stat - 50 > minValue)
                    minValue = stat - 50;

                if (stat + 50 < maxValue)
                    maxValue = stat + 50;

                int qMissing = questionType.NbAnswers - answers.Count;

                for (int i = 0; i < qMissing; i++)
                {
                    int x = 0;
                    while (x.Equals(0))
                    {
                        x = random.Next(minValue, maxValue);

                        if (statAnswers.Contains(x.ToString()))
                            x = 0;
                        else
                            statAnswers.Add(x.ToString());
                    }
                }

                answers = await GenerateAnswersStat(questionType, pokemons, answers, typeStat, statAnswers);
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

        public async Task<string> ConvertDescription(Pokemon pokemon)
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

                foreach (Pokemon item in pokemons)
                {
                    description = description.Replace(item.Name, "[...]");
                }

                return await Task.FromResult(description);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Quizz Talent
        public async Task<string> GenerateCorrectAnswers(QuestionType questionType, List<Talent> talentsAnswer, bool Reverse)
        {
            List<Answer> answers = new List<Answer>();
            Random random = new Random();

            string result = string.Empty;
            Dictionary<int, Talent> dic = new Dictionary<int, Talent>();
            foreach (var talent in talentsAnswer)
            {
                while (!dic.ContainsValue(talent))
                {
                    int numberRandom = random.Next(questionType.NbAnswers);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, talent);
                }
            }

            foreach (KeyValuePair<int, Talent> pair in dic)
            {
                Answer answer = new Answer()
                {
                    IsSelected = false,
                    IsCorrect = true,
                    IsCorrectID = pair.Value.Id,
                    Libelle = Reverse ? pair.Value.Name : pair.Value.Description,
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

        private async Task<List<Answer>> GenerateAnswers(QuestionType questionType, List<Talent> talents, List<Answer> talentsAnswer, bool Reverse = false)
        {
            Random random = new Random();

            string result = string.Empty;
            Dictionary<int, Talent> dic = new Dictionary<int, Talent>();

            foreach (var talent in talents)
            {
                while (!dic.ContainsValue(talent))
                {
                    int numberRandom = random.Next(questionType.NbAnswers);

                    if (!dic.ContainsKey(numberRandom))
                        dic.Add(numberRandom, talent);
                }
            }

            foreach (KeyValuePair<int, Talent> pair in dic)
            {
                Answer answerExist = talentsAnswer.Find(m => m.IsCorrectID.Equals(pair.Value.Id));
                if (answerExist == null)
                {
                    Answer answer = new Answer()
                    {
                        IsSelected = false,
                        IsCorrect = false,
                        IsCorrectID = -1,
                        Libelle = Reverse ? pair.Value.Name : pair.Value.Description,
                        Order = pair.Key + 1
                    };

                    talentsAnswer.Add(answer);
                }
                else
                {
                    answerExist.Order = pair.Key + 1;
                    await UpdateAsync(answerExist);
                }
            }

            return await Task.FromResult(talentsAnswer);
        }
        #endregion

        #region Quizz Stat
        public async Task<string> GenerateCorrectAnswersStat(QuestionType questionType, List<Pokemon> pokemonsAnswer, string typeStat)
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
                    Libelle = Utils.GetValueStat(pair.Value, typeStat),
                    Type = typeStat,
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

        private async Task<List<Answer>> GenerateAnswersStat(QuestionType questionType, List<Pokemon> pokemons, List<Answer> pokemonsAnswer, string typeStat, List<string> statAnswers)
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
                answerExist.Order = pair.Key + 1;
                await UpdateAsync(answerExist);

                statAnswers.Remove(Utils.GetValueStat(pair.Value, typeStat));
            }

            Dictionary<int, string> dicStat = new Dictionary<int, string>();
            foreach (var stat in statAnswers)
            {
                int numberRandom = random.Next(questionType.NbAnswers);
                while (!dicStat.ContainsValue(stat))
                {
                    numberRandom = random.Next(questionType.NbAnswers);
                    if (!dicStat.ContainsKey(numberRandom) && !dic.ContainsKey(numberRandom))
                        dicStat.Add(numberRandom, stat);
                }
            }

            foreach (KeyValuePair<int, string> pair in dicStat)
            {
                Answer answer = new Answer()
                {
                    IsSelected = false,
                    IsCorrect = false,
                    IsCorrectID = -1,
                    Libelle = pair.Value,
                    Type = typeStat,
                    Order = pair.Key + 1
                };

                pokemonsAnswer.Add(answer);
            }

            return await Task.FromResult(pokemonsAnswer);
        }
        #endregion
        #endregion
        #endregion
    }
}
