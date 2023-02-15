using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Models.ClassJson;
using QuizzPokedex.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.CustomStart
{
    public class AppStart : MvxAppStart
    {
        #region Field
        private readonly ISqliteConnectionService _connectionService;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;
        private readonly ITalentService _talentService;
        private readonly IAttaqueService _attaqueService;
        private readonly ITypeAttaqueService _typeAttaqueService;
        private readonly IDifficultyService _difficultyService;
        private readonly IQuestionTypeService _questionTypeService;
        #endregion

        #region Constructor
        public AppStart(IMvxApplication app, IMvxNavigationService mvxNavigationService, ISqliteConnectionService connectionService, IPokemonService pokemonService, ITypePokService typePokService, ITalentService talentService, IAttaqueService attaqueService, ITypeAttaqueService typeAttaqueService, IDifficultyService difficultyService, IQuestionTypeService questionTypeService)
            : base(app, mvxNavigationService)
        {
            _connectionService = connectionService;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
            _talentService = talentService;
            _attaqueService = attaqueService;
            _typeAttaqueService = typeAttaqueService;
            _difficultyService = difficultyService;
            _questionTypeService = questionTypeService;
        }
        #endregion

        #region Protected Methods
        protected override Task NavigateToFirstViewModel(object hint = null)
        {
            //initialisation des tables par défaut
            //_connectionService.GetAsyncConnection().DropTableAsync<TypePok>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Talent>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<TypeAttaque>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Attaque>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Pokemon>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<PokemonTypePok>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<PokemonWeakness>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<PokemonTalent>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<PokemonAttaque>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Profile>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Favorite>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Difficulty>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<QuestionType>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Answer>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Question>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Quizz>().Wait();

            _connectionService.GetAsyncConnection().CreateTableAsync<TypePok>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Talent>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<TypeAttaque>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Attaque>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Pokemon>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<PokemonTypePok>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<PokemonWeakness>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<PokemonTalent>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<PokemonAttaque>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Profile>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Favorite>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Difficulty>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<QuestionType>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Answer>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Question>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Quizz>().Wait();

            populateDb();

            return NavigationService.Navigate<WelcomeViewModel>();
        }

        protected async Task populateDb()
        {
            await Task.Run(async () =>
            {
                await PopulateTypeAttaque();
                await PopulateTypePok();
                await PopulateAttaque();
                await PopulateTalent();
                await PopulatePokemon();
                await PopulateDifficulty();
                await PopulateQuestionType();
            });
        }
        #endregion

        #region Private Methods
        private async Task PopulateTypeAttaque()
        {
            List<TypeAttaqueJson> typesJson = await _typeAttaqueService.GetListScrapJson();
            int nbTypeAttaqueInDb = await _typeAttaqueService.GetNumberInDbAsync();

            await _typeAttaqueService.Populate(nbTypeAttaqueInDb, typesJson);
        }

        private async Task PopulateTypePok()
        {
            List<TypeJson> typesJson = await _typePokService.GetListScrapJson();
            int nbTypePokInDb = await _typePokService.GetNumberInDbAsync();

            await _typePokService.Populate(nbTypePokInDb, typesJson);
        }

        private async Task PopulateAttaque()
        {
            List<AttaqueJson> attaquesJson = await _attaqueService.GetListScrapJson();
            int nbAttaqueInDb = await _attaqueService.GetNumberInDbAsync();

            await _attaqueService.Populate(nbAttaqueInDb, attaquesJson);
        }

        private async Task PopulateTalent()
        {
            List<TalentJson> talentsJson = await _talentService.GetListScrapJson();
            int nbTalentInDb = await _talentService.GetNumberInDbAsync();

            await _talentService.Populate(nbTalentInDb, talentsJson);
        }

        private async Task PopulatePokemon()
        {
            List<PokemonJson> PoksJson = await _pokemonService.GetListScrapJson();
            int nbPokInDb = await _pokemonService.GetNumberInDbAsync();

            await _pokemonService.Populate(nbPokInDb, PoksJson);
            await _pokemonService.PopulateUpdateEvolution(PoksJson);
            await _pokemonService.CheckIfPictureNotExistDownload(PoksJson);
            await _pokemonService.ResetNextLaunch();
        }

        private async Task PopulateDifficulty()
        {
            int countD = await _difficultyService.GetAllCountAsync();
            if (countD.Equals(0))
                await _difficultyService.Populate();
        }

        private async Task PopulateQuestionType()
        {
            int countQT = await _questionTypeService.GetAllCountAsync();
            if (countQT.Equals(0))
                await _questionTypeService.Populate();
        }
        #endregion
    }
}
