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
        private readonly ISqliteConnectionService _connectionService;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;

        public AppStart(IMvxApplication app, IMvxNavigationService mvxNavigationService, ISqliteConnectionService connectionService, IPokemonService pokemonService, ITypePokService typePokService)
            : base(app, mvxNavigationService)
        {
            _connectionService = connectionService;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
        }

        protected override Task NavigateToFirstViewModel(object hint = null)
        {
            //initialisation des tables par défaut
            //_connectionService.GetAsyncConnection().DropTableAsync<TypePok>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Pokemon>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Profile>().Wait();

            _connectionService.GetAsyncConnection().CreateTableAsync<TypePok>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Pokemon>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Profile>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Difficulty>().Wait();

            populateDb();

            return NavigationService.Navigate<WelcomeViewModel>();
        }

        protected async void populateDb()
        {
            await Task.Run(async () =>
            {
                await PopulateTypePok();
                await PopulatePokemon();
                await PopulateDifficulty();
            });
        }

        private async Task PopulateTypePok()
        {
            List<TypeJson> typesJson = await _typePokService.GetListTypeScrapJson();
            int nbTypePokInDb = await _typePokService.GetNumberInDbAsync();

            await _typePokService.Populate(nbTypePokInDb, typesJson);
        }

        private async Task PopulatePokemon()
        {
            List<PokemonJson> PoksJson = await _pokemonService.GetListPokeScrapJson();
            int nbPokInDb = await _pokemonService.GetNumberInDbAsync();

            await _pokemonService.Populate(nbPokInDb, PoksJson);
            await _pokemonService.PopulateUpdateEvolution(PoksJson);
        }

        private async Task PopulateDifficulty()
        {
            await Task.Run(() =>
            {

            });
        }
    }
}
