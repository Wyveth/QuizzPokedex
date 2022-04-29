using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class PokedexViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigation;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;

        //creation de l'abonnement ici (pour rafraichir via un abonné)
        private readonly MvxSubscriptionToken _token;

        public PokedexViewModel(IMvxNavigationService navigation, IPokemonService pokemonService, ITypePokService typePokService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
            _token = messenger.Subscribe<MessageRefresh>(RefreshAsync);
        }

        public override async Task Initialize()
        {
            LoadPokemonTask = MvxNotifyTask.Create(LoadPokemonAsync);
            await base.Initialize();
        }

        private async Task LoadPokemonAsync()
        {
            var resultPokemon = await _pokemonService.GetAllNormalEvolutionAsync(NameFilter);
            Pokemons = new MvxObservableCollection<Pokemon>(resultPokemon);
        }

        private async void RefreshAsync(MessageRefresh msg)
        {
            if (msg.Refresh)
                await LoadPokemonAsync();
        }

        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand CreatePokemonCommandAsync => new MvxAsyncCommand(CreatePokemonAsync);
        public IMvxAsyncCommand<Pokemon> UpdatePokemonCommandAsync => new MvxAsyncCommand<Pokemon>(UpdatePokemonAsync);
        public IMvxAsyncCommand<Pokemon> DeletePokemonCommandAsync => new MvxAsyncCommand<Pokemon>(DeletePokemonAsync);
        public IMvxAsyncCommand<Pokemon> DetailsPokemonCommandAsync => new MvxAsyncCommand<Pokemon>(DetailsPokemonAsync);
        public IMvxAsyncCommand<string> FilterByNameCommandAsync => new MvxAsyncCommand<string>(FilterByNameAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task FilterByNameAsync(string name)
        {
            NameFilter = "Dracaufeu";
            await LoadPokemonAsync();

        }
        private async Task CreatePokemonAsync()
        {
            await _navigation.Navigate<PokemonViewModel, Pokemon>(new Pokemon());
        }

        private async Task UpdatePokemonAsync(Pokemon Pokemon)
        {
            await _navigation.Navigate<PokemonViewModel, Pokemon>(Pokemon);
        }

        private async Task DeletePokemonAsync(Pokemon Pokemon)
        {
            //Peut etre mettre une boite de dialogue de confirmation avant delete (leçon sur les dialogBox)

            await _pokemonService.DeleteAsync(Pokemon).ContinueWith(
                async (result) =>
                    await LoadPokemonAsync()
                    );
        }

        private async Task DetailsPokemonAsync(Pokemon Pokemon)
        {
            await _navigation.Navigate<PokemonViewModel, Pokemon>(Pokemon);
        }
        #endregion

        #region PROPERTIES
        public MvxNotifyTask LoadPokemonTask { get; private set; }

        private MvxObservableCollection<Pokemon> _pokemons;

        public MvxObservableCollection<Pokemon> Pokemons
        {
            get { return _pokemons; }
            set { SetProperty(ref _pokemons, value); }
        }

        private MvxObservableCollection<TypePok> _typesPok;

        public MvxObservableCollection<TypePok> TypesPok
        {
            get { return _typesPok; }
            set { SetProperty(ref _typesPok, value); }
        }

        private Pokemon _selectedPokemon;
        public Pokemon SelectedPokemon
        {
            get { return _selectedPokemon; }
            set
            {
                _selectedPokemon = value;
                _ = DetailsPokemonAsync(_selectedPokemon);
            }
        }

        private string _nameFilter = "";

        public string NameFilter
        {
            get { return _nameFilter; }
            set
            {
                _nameFilter = value;
            }
        }
        #endregion
    }
}
