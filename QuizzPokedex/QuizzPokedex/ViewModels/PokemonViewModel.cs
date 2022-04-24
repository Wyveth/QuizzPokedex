using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace QuizzPokedex.ViewModels
{
    public class PokemonViewModel : MvxViewModel<Pokemon>
    {

        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IPokemonService _pokemonService;
        private readonly IMvxMessenger _messenger;

        public PokemonViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IPokemonService pokemonService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _logger = logger;
            _pokemonService = pokemonService;
            _messenger = messenger;
        }

        public override void Prepare(Pokemon pokemon)
        {
            Pokemon = pokemon;

            base.Prepare();
        }


        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }
        #endregion

        #region PROPERTIES
        private Pokemon _pokemon;
        public Pokemon Pokemon
        {
            get { return _pokemon; }
            set { SetProperty(ref _pokemon, value); }
        }
        #endregion
    }
}
