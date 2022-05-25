using FFImageLoading.Transformations;
using FFImageLoading.Work;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class ProfileViewModel : MvxViewModel<Profile>
    {
        #region Field
        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IProfileService _profileService;
        private readonly IMvxMessenger _messenger;
        private readonly ITypePokService _typePokService;
        private readonly IPokemonService _pokemonService;
        #endregion

        #region Constructor
        public ProfileViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IProfileService ProfileService, ITypePokService typePokService, IPokemonService pokemonService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _logger = logger;
            _profileService = ProfileService;
            _typePokService = typePokService;
            _pokemonService = pokemonService;
            _messenger = messenger;
        }
        #endregion

        #region Public Methods
        public override void Prepare(Profile profile)
        {
            Profile = profile;

            base.Prepare();
        }

        public override async Task Initialize()
        {
            LoadPokemonTask = MvxNotifyTask.Create(LoadPokemonAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task LoadPokemonAsync()
        {
            List<Pokemon> pokemonsAvailable = await _pokemonService.GetAllStartGen1Async();

            StarterGrass = pokemonsAvailable[0];
            StarterFire = pokemonsAvailable[1];
            StarterWater = pokemonsAvailable[2];
            StarterElectrik = pokemonsAvailable[3];
        }
        #endregion

            #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand SelectedStarterGrassCommandAsync => new MvxAsyncCommand(SelectedStarterGrassAsync);
        public IMvxAsyncCommand SelectedStarterFireCommandAsync => new MvxAsyncCommand(SelectedStarterFireAsync);
        public IMvxAsyncCommand SelectedStarterWaterCommandAsync => new MvxAsyncCommand(SelectedStarterWaterAsync);
        public IMvxAsyncCommand SelectedStarterElectrikCommandAsync => new MvxAsyncCommand(SelectedStarterElectrikAsync);
        public IMvxAsyncCommand SaveCommandAsync => new MvxAsyncCommand(SaveAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task SelectedStarterGrassAsync()
        {
            pokemonSelected = StarterGrass;
        }

        private async Task SelectedStarterFireAsync()
        {
            pokemonSelected = StarterFire;
        }

        private async Task SelectedStarterWaterAsync()
        {
            pokemonSelected = StarterWater;
        }

        private async Task SelectedStarterElectrikAsync()
        {
            pokemonSelected = StarterElectrik;
        }

        private async Task SaveAsync()
        {
            //Save l'Profile s'il est complet  (nom, calorie, photo et categorie)
            if (Profile.Name != string.Empty &&
                Profile.BirthDate != string.Empty)
            {
                if (ModeUpdate)
                    await _profileService.UpdateAsync(Profile);
                else
                    await _profileService.CreateAsync(Profile);

                //nouvel enregistrement crée on informe par abonnement de rafraichir
                var refresh = new MessageRefresh(this, true);
                _messenger.Publish(refresh);

                await _navigation.Close(this);
            }
        }
        #endregion

        #region PROPERTIES
        public MvxNotifyTask LoadPokemonTask { get; private set; }

        private Profile _profile;
        public Profile Profile
        {
            get { return _profile; }
            set { SetProperty(ref _profile, value); }
        }

        private Pokemon _pokemonSelected;

        public Pokemon pokemonSelected
        {
            get { return _pokemonSelected; }
            set { SetProperty(ref _pokemonSelected, value); }
        }


        #region Starter Grass
        private Pokemon _starterGrass;

        public Pokemon StarterGrass
        {
            get { return _starterGrass; }
            set { SetProperty(ref _starterGrass, value); }
        }

        private bool _isVisibleStarterGrass;

        public bool IsVisibleStarterGrass
        {
            get { return _isVisibleStarterGrass; }
            set { SetProperty(ref _isVisibleStarterGrass, value); }
        }
        #endregion

        #region Starter Fire
        private Pokemon _starterFire;

        public Pokemon StarterFire
        {
            get { return _starterFire; }
            set { SetProperty(ref _starterFire, value); }
        }

        private bool _isVisibleStarterFire;

        public bool IsVisibleStarterFire
        {
            get { return _isVisibleStarterFire; }
            set { SetProperty(ref _isVisibleStarterFire, value); }
        }
        #endregion

        #region Starter Water
        private Pokemon _starterWater;

        public Pokemon StarterWater
        {
            get { return _starterWater; }
            set { SetProperty(ref _starterWater, value); }
        }

        private bool _isVisibleStarterWater;

        public bool IsVisibleStarterWater
        {
            get { return _isVisibleStarterWater; }
            set { SetProperty(ref _isVisibleStarterWater, value); }
        }
        #endregion

        #region Sarter Electrik
        private Pokemon _starterElectrik;

        public Pokemon StarterElectrik
        {
            get { return _starterElectrik; }
            set { SetProperty(ref _starterElectrik, value); }
        }

        private bool _isVisibleStarterElectrik;

        public bool IsVisibleStarterElectrik
        {
            get { return _isVisibleStarterElectrik; }
            set { SetProperty(ref _isVisibleStarterElectrik, value); }
        }
        #endregion

        private bool _modeUpdate;

        public bool ModeUpdate
        {
            get { return _modeUpdate; }
            set { SetProperty(ref _modeUpdate, value); }
        }
        #endregion
    }
}
