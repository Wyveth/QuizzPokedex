using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
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
        }

        public override async Task Initialize()
        {
            LoadPokemonTask = MvxNotifyTask.Create(LoadPokemonAsync);
            await base.Initialize();
        }

        private async Task LoadPokemonAsync()
        {
            var resultPokemon = await _pokemonService.GetAllWithoutVariantAsync(SearchText
                , FiltreActiveGen1
                , FiltreActiveGen2
                , FiltreActiveGen3
                , FiltreActiveGen4
                , FiltreActiveGen5
                , FiltreActiveGen6
                , FiltreActiveGen7
                , FiltreActiveGen8
                , FiltreActiveGenArceus);
            Pokemons = new MvxObservableCollection<Pokemon>(resultPokemon);
        }

        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand ModalFilterCommandAsync => new MvxAsyncCommand(ModalFilterAsync);
        public IMvxAsyncCommand ModalTypeFilterCommandAsync => new MvxAsyncCommand(ModalTypeFilterAsync);
        public IMvxAsyncCommand ModalGenFilterCommandAsync => new MvxAsyncCommand(ModalGenFilterAsync);
        public IMvxAsyncCommand BackModalGenFilterCommandAsync => new MvxAsyncCommand(BackModalGenFilterAsync);
        public IMvxAsyncCommand CloseModalGenFilterCommandAsync => new MvxAsyncCommand(CloseModalGenFilterAsync);
        public IMvxAsyncCommand CreatePokemonCommandAsync => new MvxAsyncCommand(CreatePokemonAsync);
        public IMvxAsyncCommand<Pokemon> UpdatePokemonCommandAsync => new MvxAsyncCommand<Pokemon>(UpdatePokemonAsync);
        public IMvxAsyncCommand<Pokemon> DeletePokemonCommandAsync => new MvxAsyncCommand<Pokemon>(DeletePokemonAsync);
        public IMvxAsyncCommand<Pokemon> DetailsPokemonCommandAsync => new MvxAsyncCommand<Pokemon>(DetailsPokemonAsync);

        #region Command Filter
        public IMvxAsyncCommand FilterByNameCommandAsync => new MvxAsyncCommand(FilterByNameAsync);
        public IMvxAsyncCommand FilterByGen1CommandAsync => new MvxAsyncCommand(FilterByGen1Async);
        public IMvxAsyncCommand FilterByGen2CommandAsync => new MvxAsyncCommand(FilterByGen2Async);
        public IMvxAsyncCommand FilterByGen3CommandAsync => new MvxAsyncCommand(FilterByGen3Async);
        public IMvxAsyncCommand FilterByGen4CommandAsync => new MvxAsyncCommand(FilterByGen4Async);
        public IMvxAsyncCommand FilterByGen5CommandAsync => new MvxAsyncCommand(FilterByGen5Async);
        public IMvxAsyncCommand FilterByGen6CommandAsync => new MvxAsyncCommand(FilterByGen6Async);
        public IMvxAsyncCommand FilterByGen7CommandAsync => new MvxAsyncCommand(FilterByGen7Async);
        public IMvxAsyncCommand FilterByGen8CommandAsync => new MvxAsyncCommand(FilterByGen8Async);
        public IMvxAsyncCommand FilterByGenArceusCommandAsync => new MvxAsyncCommand(FilterByGenArceusAsync);
        #endregion

        #region Navigation Back & Filter
        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task ModalFilterAsync()
        {
            IsVisibleModalFilter = !IsVisibleModalFilter;
        }

        private async Task ModalGenFilterAsync()
        {
            IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
            IsVisibleModalGenFilter = !IsVisibleModalGenFilter;
        }

        private async Task ModalTypeFilterAsync()
        {
            IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
            IsVisibleModalTypeFilter = !IsVisibleModalTypeFilter;
        }

        private async Task BackModalGenFilterAsync()
        {
            IsVisibleBackgroundModalFilter = false;
            IsVisibleModalGenFilter = false;
        }

        private async Task CloseModalGenFilterAsync()
        {
            IsVisibleBackgroundModalFilter = false;
            IsVisibleModalGenFilter = false;
            IsVisibleModalFilter = false;
        }
        #endregion

        #region Filter By
        private async Task FilterByNameAsync()
        {
            await LoadPokemonAsync();
        }

        private async Task FilterByGen1Async()
        {
            if (FiltreActiveGen1)
            {
                FiltreActiveGen1 = false;
                BackgroundColorGen1 = Constantes.WhiteHexa;
                TextColorGen1 = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGen1 = true;
                BackgroundColorGen1 = Constantes.BlackHexa;
                TextColorGen1 = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByGen2Async()
        {
            if (FiltreActiveGen2)
            {
                FiltreActiveGen2 = false;
                BackgroundColorGen2 = Constantes.WhiteHexa;
                TextColorGen2 = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGen2 = true;
                BackgroundColorGen2 = Constantes.BlackHexa;
                TextColorGen2 = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByGen3Async()
        {
            if (FiltreActiveGen3)
            {
                FiltreActiveGen3 = false;
                BackgroundColorGen3 = Constantes.WhiteHexa;
                TextColorGen3 = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGen3 = true;
                BackgroundColorGen3 = Constantes.BlackHexa;
                TextColorGen3 = Constantes.WhiteHexa;
            }
            
            await LoadPokemonAsync();
        }

        private async Task FilterByGen4Async()
        {
            if (FiltreActiveGen4)
            {
                FiltreActiveGen4 = false;
                BackgroundColorGen4 = Constantes.WhiteHexa;
                TextColorGen4 = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGen4 = true;
                BackgroundColorGen4 = Constantes.BlackHexa;
                TextColorGen4 = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByGen5Async()
        {
            if (FiltreActiveGen5)
            {
                FiltreActiveGen5 = false;
                BackgroundColorGen5 = Constantes.WhiteHexa;
                TextColorGen5 = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGen5 = true;
                BackgroundColorGen5 = Constantes.BlackHexa;
                TextColorGen5 = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByGen6Async()
        {
            if (FiltreActiveGen6)
            {
                FiltreActiveGen6 = false;
                BackgroundColorGen6 = Constantes.WhiteHexa;
                TextColorGen6 = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGen6 = true;
                BackgroundColorGen6 = Constantes.BlackHexa;
                TextColorGen6 = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByGen7Async()
        {
            if (FiltreActiveGen7)
            {
                FiltreActiveGen7 = false;
                BackgroundColorGen7 = Constantes.WhiteHexa;
                TextColorGen7 = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGen7 = true;
                BackgroundColorGen7 = Constantes.BlackHexa;
                TextColorGen7 = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByGen8Async()
        {
            if (FiltreActiveGen8)
            {
                FiltreActiveGen8 = false;
                BackgroundColorGen8 = Constantes.WhiteHexa;
                TextColorGen8 = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGen8 = true;
                BackgroundColorGen8 = Constantes.BlackHexa;
                TextColorGen8 = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByGenArceusAsync()
        {
            if (FiltreActiveGenArceus)
            {
                FiltreActiveGenArceus = false;
                BackgroundColorGenArceus = Constantes.WhiteHexa;
                TextColorGenArceus = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveGenArceus = true;
                BackgroundColorGenArceus = Constantes.BlackHexa;
                TextColorGenArceus = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }
        #endregion

        #region CRUD
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
        #endregion

        #region PROPERTIES
        #region Collection
        public MvxNotifyTask LoadPokemonTask { get; private set; }

        private MvxObservableCollection<Pokemon> _pokemons;

        public MvxObservableCollection<Pokemon> Pokemons
        {
            get { return _pokemons; }
            set { 
                SetProperty(ref _pokemons, value);
                RaisePropertyChanged(() => Pokemons);
            }
        }

        private MvxObservableCollection<TypePok> _typesPok;

        public MvxObservableCollection<TypePok> TypesPok
        {
            get { return _typesPok; }
            set { SetProperty(ref _typesPok, value); }
        }
        #endregion

        #region Data
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
        #endregion

        #region Search
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { 
                if (_searchText != value) { 
                    _searchText = value; 
                    RaisePropertyChanged(() => SearchText);
                    _ = FilterByNameAsync();
                } 
            }
        }
        #endregion

        #region Visibility Filter
        private bool _isVisibleModalFilter;

        public bool IsVisibleModalFilter
        {
            get { return _isVisibleModalFilter; }
            set { SetProperty(ref _isVisibleModalFilter, value); }
        }


        private bool _isVisibleBackgroundModalFilter;

        public bool IsVisibleBackgroundModalFilter
        {
            get { return _isVisibleBackgroundModalFilter; }
            set { SetProperty(ref _isVisibleBackgroundModalFilter, value); }
        }

        private bool _isVisibleModalGenFilter;

        public bool IsVisibleModalGenFilter
        {
            get { return _isVisibleModalGenFilter; }
            set { SetProperty(ref _isVisibleModalGenFilter, value); }
        }

        private bool _isVisibleModalTypeFilter;

        public bool IsVisibleModalTypeFilter
        {
            get { return _isVisibleModalTypeFilter; }
            set
            { SetProperty(ref _isVisibleModalTypeFilter, value); }
        }
        #endregion

        #region Filter Gen
        #region Generation 1
        private bool _filtreActiveGen1 = false;

        public bool FiltreActiveGen1
        {
            get { return _filtreActiveGen1; }
            set { SetProperty(ref _filtreActiveGen1, value); }
        }

        private string _backgroundColorGen1 = "#FFFFFF";

        public string BackgroundColorGen1
        {
            get { return _backgroundColorGen1; }
            set { SetProperty(ref _backgroundColorGen1, value); }
        }

        private string _textColorGen1 = "#000000";

        public string TextColorGen1
        {
            get { return _textColorGen1; }
            set { SetProperty(ref _textColorGen1, value); }
        }
        #endregion

        #region Generation 2
        private bool _filtreActiveGen2 = false;

        public bool FiltreActiveGen2
        {
            get { return _filtreActiveGen2; }
            set { SetProperty(ref _filtreActiveGen2, value); }
        }

        private string _backgroundColorGen2 = "#FFFFFF";

        public string BackgroundColorGen2
        {
            get { return _backgroundColorGen2; }
            set { SetProperty(ref _backgroundColorGen2, value); }
        }

        private string _textColorGen2 = "#000000";

        public string TextColorGen2
        {
            get { return _textColorGen2; }
            set { SetProperty(ref _textColorGen2, value); }
        }
        #endregion

        #region Generation 3
        private bool _filtreActiveGen3 = false;

        public bool FiltreActiveGen3
        {
            get { return _filtreActiveGen3; }
            set { SetProperty(ref _filtreActiveGen3, value); }
        }

        private string _backgroundColorGen3 = "#FFFFFF";

        public string BackgroundColorGen3
        {
            get { return _backgroundColorGen3; }
            set { SetProperty(ref _backgroundColorGen3, value); }
        }

        private string _textColorGen3 = "#000000";

        public string TextColorGen3
        {
            get { return _textColorGen3; }
            set { SetProperty(ref _textColorGen3, value); }
        }
        #endregion

        #region Generation 4
        private bool _filtreActiveGen4 = false;

        public bool FiltreActiveGen4
        {
            get { return _filtreActiveGen4; }
            set { SetProperty(ref _filtreActiveGen4, value); }
        }

        private string _backgroundColorGen4 = "#FFFFFF";

        public string BackgroundColorGen4
        {
            get { return _backgroundColorGen4; }
            set { SetProperty(ref _backgroundColorGen4, value); }
        }
        private string _textColorGen4 = "#000000";

        public string TextColorGen4
        {
            get { return _textColorGen4; }
            set { SetProperty(ref _textColorGen4, value); }
        }
        #endregion

        #region Generation 5
        private bool _filtreActiveGen5 = false;

        public bool FiltreActiveGen5
        {
            get { return _filtreActiveGen5; }
            set { SetProperty(ref _filtreActiveGen5, value); }
        }

        private string _backgroundColorGen5 = "#FFFFFF";

        public string BackgroundColorGen5
        {
            get { return _backgroundColorGen5; }
            set { SetProperty(ref _backgroundColorGen5, value); }
        }
        private string _textColorGen5 = "#000000";

        public string TextColorGen5
        {
            get { return _textColorGen5; }
            set { SetProperty(ref _textColorGen5, value); }
        }
        #endregion

        #region Generation 6
        private bool _filtreActiveGen6 = false;

        public bool FiltreActiveGen6
        {
            get { return _filtreActiveGen6; }
            set { SetProperty(ref _filtreActiveGen6, value); }
        }

        private string _backgroundColorGen6 = "#FFFFFF";

        public string BackgroundColorGen6
        {
            get { return _backgroundColorGen6; }
            set { SetProperty(ref _backgroundColorGen6, value); }
        }
        private string _textColorGen6 = "#000000";

        public string TextColorGen6
        {
            get { return _textColorGen6; }
            set { SetProperty(ref _textColorGen6, value); }
        }
        #endregion

        #region Generation 7
        private bool _filtreActiveGen7 = false;

        public bool FiltreActiveGen7
        {
            get { return _filtreActiveGen7; }
            set { SetProperty(ref _filtreActiveGen7, value); }
        }

        private string _backgroundColorGen7 = "#FFFFFF";

        public string BackgroundColorGen7
        {
            get { return _backgroundColorGen7; }
            set { SetProperty(ref _backgroundColorGen7, value); }
        }
        private string _textColorGen7 = "#000000";

        public string TextColorGen7
        {
            get { return _textColorGen7; }
            set { SetProperty(ref _textColorGen7, value); }
        }
        #endregion

        #region Generation 8
        private bool _filtreActiveGen8 = false;

        public bool FiltreActiveGen8
        {
            get { return _filtreActiveGen8; }
            set { SetProperty(ref _filtreActiveGen8, value); }
        }

        private string _backgroundColorGen8 = "#FFFFFF";

        public string BackgroundColorGen8
        {
            get { return _backgroundColorGen8; }
            set { SetProperty(ref _backgroundColorGen8, value); }
        }
        private string _textColorGen8 = "#000000";

        public string TextColorGen8
        {
            get { return _textColorGen8; }
            set { SetProperty(ref _textColorGen8, value); }
        }
        #endregion

        #region Generation Arceus
        private bool _filtreActiveGenArceus = false;

        public bool FiltreActiveGenArceus
        {
            get { return _filtreActiveGenArceus; }
            set { SetProperty(ref _filtreActiveGenArceus, value); }
        }

        private string _backgroundColorGenArceus = "#FFFFFF";

        public string BackgroundColorGenArceus
        {
            get { return _backgroundColorGenArceus; }
            set { SetProperty(ref _backgroundColorGenArceus, value); }
        }
        private string _textColorGenArceus = "#000000";

        public string TextColorGenArceus
        {
            get { return _textColorGenArceus; }
            set { SetProperty(ref _textColorGenArceus, value); }
        }
        #endregion
        #endregion
        #endregion
    }
}
