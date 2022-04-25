using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class MenuViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigation;
        private readonly IPokemonService _PokemonService;

        public MenuViewModel(IMvxNavigationService navigation, IPokemonService PokemonService)
        {
            _navigation = navigation;
            _PokemonService = PokemonService;
        }

        private async Task LoadPokemonAsync()
        {
            var result = await _PokemonService.GetAllAsync();
            Pokemons = new MvxObservableCollection<Pokemon>(result);
        }

        public override Task Initialize()
        {
            LoadPokemonTask = MvxNotifyTask.Create(LoadPokemonAsync);
            return base.Initialize();
        }

        #region COMMANDS
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand NavigationQuizzCommandAsync => new MvxAsyncCommand(NavigationQuizzAsync);
        public IMvxAsyncCommand NavigationPokemonsCommandAsync => new MvxAsyncCommand(NavigationPokedexAsync);


        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task NavigationQuizzAsync()
        {
            //await _navigation.Navigate<QuizzViewModel>();
        }

        private async Task NavigationPokedexAsync()
        {
            await _navigation.Navigate<PokedexViewModel>();
        }
        #endregion

        public MvxNotifyTask LoadPokemonTask { get; private set; }

        private MvxObservableCollection<Pokemon> _Pokemons;
        public MvxObservableCollection<Pokemon> Pokemons
        {
            get { return _Pokemons; }
            set { SetProperty(ref _Pokemons, value); }
        }
    }
}
