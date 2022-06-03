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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class ProfileViewModel : MvxViewModel<Profile>
    {
        #region Fields
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
            IsVisibleStarter = true;
            List<Pokemon> pokemonsAvailable = await _pokemonService.GetAllStartGen1Async();
            StarterGrass = pokemonsAvailable[0];
            StarterFire = pokemonsAvailable[1];
            StarterWater = pokemonsAvailable[2];
            StarterElectrik = pokemonsAvailable[3];
            
        }
        #endregion

        #region Command
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
            bool exist = await _profileService.CheckIfProfilPokemonExist(StarterGrass);
            if (!exist)
            {
                pokemonSelected = StarterGrass;
                IsVisibleStarter = true;
            }
            else
            {
                pokemonSelected = null;
                IsVisibleStarter = false;
            }
        }

        private async Task SelectedStarterFireAsync()
        {
            bool exist = await _profileService.CheckIfProfilPokemonExist(StarterFire);
            if (!exist)
            {
                pokemonSelected = StarterFire;
                IsVisibleStarter = true;
            }
            else
            {
                pokemonSelected = null;
                IsVisibleStarter = false;
            }
        }

        private async Task SelectedStarterWaterAsync()
        {
            bool exist = await _profileService.CheckIfProfilPokemonExist(StarterWater);
            if (!exist)
            {
                pokemonSelected = StarterWater;
                IsVisibleStarter = true;
            }
            else
            {
                pokemonSelected = null;
                IsVisibleStarter = false;
            }
        }

        private async Task SelectedStarterElectrikAsync()
        {
            bool exist = await _profileService.CheckIfProfilPokemonExist(StarterElectrik);
            if (!exist)
            {
                pokemonSelected = StarterElectrik;
                IsVisibleStarter = true;
            }
            else
            {
                pokemonSelected = null;
                IsVisibleStarter = false;
            }
        }

        private async Task SaveAsync()
        {
            //Save le Profile s'il est complet  (Pseudo, Date de naissance, Pokémon)
            if (!string.IsNullOrEmpty(Profile.Name) &&
                pokemonSelected != null)
            {
                if (Profile.BirthDate == string.Empty)
                    Profile.BirthDate = DateTime.Now.ToShortDateString();

                Profile.PokemonID = pokemonSelected.Id;
                Profile.Activated = true;
                await _profileService.CreateAsync(Profile);

                //nouvel enregistrement crée on informe par abonnement de rafraichir
                var refresh = new MessageRefresh(this, true);
                _messenger.Publish(refresh);

                await _navigation.Close(this);
            }
        }
        #endregion

        #region Properties
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

        private bool _isVisibleStarter = false;

        public bool IsVisibleStarter
        {
            get { return _isVisibleStarter; }
            set { SetProperty(ref _isVisibleStarter, value); }
        }

        #region Starter Grass
        private Pokemon _starterGrass;

        public Pokemon StarterGrass
        {
            get { return _starterGrass; }
            set { SetProperty(ref _starterGrass, value); }
        }
        #endregion

        #region Starter Fire
        private Pokemon _starterFire;

        public Pokemon StarterFire
        {
            get { return _starterFire; }
            set { SetProperty(ref _starterFire, value); }
        }
        #endregion

        #region Starter Water
        private Pokemon _starterWater;

        public Pokemon StarterWater
        {
            get { return _starterWater; }
            set { SetProperty(ref _starterWater, value); }
        }
        #endregion

        #region Sarter Electrik
        private Pokemon _starterElectrik;

        public Pokemon StarterElectrik
        {
            get { return _starterElectrik; }
            set { SetProperty(ref _starterElectrik, value); }
        }
        #endregion

        private bool _isEnabled = false;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        private bool _modeUpdate;

        public bool ModeUpdate
        {
            get { return _modeUpdate; }
            set { SetProperty(ref _modeUpdate, value); }
        }
        #endregion
    }
}
