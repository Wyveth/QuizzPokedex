using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class WelcomeViewModel : MvxViewModel
    {
        #region Field
        private readonly IMvxNavigationService _navigation;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;
        private readonly IProfileService _profileService;

        //Creation de l'abonnement ici (pour rafraichir via un abonné)
        private readonly MvxSubscriptionToken _token;
        #endregion

        #region Constructor
        public WelcomeViewModel(IMvxNavigationService navigation, IMvxMessenger messenger, IPokemonService pokemonService, ITypePokService typePokService, IProfileService profileService)
        {
            _navigation = navigation;
            _token = messenger.Subscribe<MessageRefresh>(RefreshAsync);
            _pokemonService = pokemonService;
            _typePokService = typePokService;
            _profileService = profileService;
        }
        #endregion

        #region Public Methods
        public override async Task Initialize()
        {
            ProgressBarTask = MvxNotifyTask.Create(ProgressBarAsync);
            ProfileTask = MvxNotifyTask.Create(ProfileAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task ProgressBarAsync()
        {
            int nbTypeMax = await _typePokService.GetNumberTypeJsonAsync();
            int nbTypeInDb = await _typePokService.GetNumberInDbAsync();

            ProgressBarIsVisible = false;

            if (!nbTypeInDb.Equals(nbTypeMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbTypeInDb, nbTypeMax);
                TextProgressBar = "Création Type: " + nbTypeInDb.ToString() + "/" + nbTypeMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbTypeInDb = await _typePokService.GetNumberInDbAsync();
                    ValueProgressBar = await getPercent(nbTypeInDb, nbTypeMax);
                    TextProgressBar = "Création Type: " + nbTypeInDb.ToString() + "/" + nbTypeMax.ToString();
                }

                ProgressBarIsVisible = false;
            }

            int nbPokMax = await _pokemonService.GetNumberPokJsonAsync();
            int nbPokInDb = await _pokemonService.GetNumberInDbAsync();

            if (!nbPokInDb.Equals(nbPokMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbPokInDb, nbPokMax);
                TextProgressBar = "Création Pokémon: " + nbPokInDb.ToString() + "/" + nbPokMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbPokInDb = await _pokemonService.GetNumberInDbAsync();
                    ValueProgressBar = await getPercent(nbPokInDb, nbPokMax);
                    TextProgressBar = "Création Pokémon: " + nbPokInDb.ToString() + "/" + nbPokMax.ToString();
                }

                ProgressBarIsVisible = false;
            }

            int nbPokNotUpdated = await _pokemonService.GetNumberPokUpdateAsync();

            if (!nbPokNotUpdated.Equals(nbPokMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbPokNotUpdated, nbPokMax);
                TextProgressBar = "Update Pokémon: " + nbPokNotUpdated.ToString() + "/" + nbPokMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbPokNotUpdated = await _pokemonService.GetNumberPokUpdateAsync();
                    ValueProgressBar = await getPercent(nbPokNotUpdated, nbPokMax);
                    TextProgressBar = "Update Pokémon: " + nbPokNotUpdated.ToString() + "/" + nbPokMax.ToString();
                }

                ProgressBarIsVisible = false;
            }
        }

        private async Task<double> getPercent(double nbPok, double nbPokMax)
        {
            double percent = 0;
            var result = nbPok/nbPokMax;
            percent = double.Parse(result.ToString());
            return await Task.FromResult(percent);
        }

        private async Task ProfileAsync()
        {
            List<Profile> profiles = await _profileService.GetAllAsync();
            if (profiles.Count == 1)
            {
                FirstProfileCreated = true;
                Profile profile = profiles.Find(m => m.Activated.Equals(true));
                ActivatedProfile = await _pokemonService.GetByIdAsync(profile.PokemonID.ToString());
                IsVisibleSecondProfile = true;
            }
            else if (profiles.Count == 2)
            {
                FirstProfileCreated = true;
                SecondProfileCreated = true;
                IsVisibleSecondProfile = true;
                IsVisibleThirdProfile = true;
            }
            else if (profiles.Count == 3)
            {
                FirstProfileCreated = true;
                SecondProfileCreated = true;
                IsVisibleSecondProfile = true;
                ThirdProfileCreated = true;
                IsVisibleThirdProfile = true;
            }
        }

        private async void RefreshAsync(MessageRefresh msg)
        {
            if (msg.Refresh)
                await ProfileAsync();
        }
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationQuizzCommandAsync => new MvxAsyncCommand(NavigationQuizzAsync);
        public IMvxAsyncCommand NavigationProfileCommandAsync => new MvxAsyncCommand(NavigationProfileAsync);
        public IMvxAsyncCommand NavigationPokedexCommandAsync => new MvxAsyncCommand(NavigationPokedexAsync);

        private async Task NavigationQuizzAsync()
        {
            await _navigation.Navigate<QuizzViewModel>();
        }

        private async Task NavigationProfileAsync()
        {
            await _navigation.Navigate<ProfileViewModel, Profile>(new Profile());
        }

        private async Task NavigationPokedexAsync()
        {
            await _navigation.Navigate<PokedexViewModel>();
        }
        #endregion

        #region Properties
        #region Collection
        public MvxNotifyTask ProgressBarTask { get; private set; }
        public MvxNotifyTask ProfileTask { get; private set; }
        #endregion

        #region ProgressBar
        private bool _progressBarIsVisible = false;

        public bool ProgressBarIsVisible
        {
            get { return _progressBarIsVisible; }
            set { SetProperty(ref _progressBarIsVisible, value); }
        }

        private double _valueProgressBar;

        public double ValueProgressBar
        {
            get { return _valueProgressBar; }
            set { SetProperty(ref _valueProgressBar, value); }
        }

        private string _textProgressBar;

        public string TextProgressBar
        {
            get { return _textProgressBar; }
            set { SetProperty(ref _textProgressBar, value); }
        }

        #endregion

        #region Profile
        private bool _firstProfileCreated = false;

        public bool FirstProfileCreated
        {
            get { return _firstProfileCreated; }
            set { SetProperty(ref _firstProfileCreated, value); }
        }

        private bool _secondProfileCreated = false;

        public bool SecondProfileCreated
        {
            get { return _secondProfileCreated; }
            set { SetProperty(ref _secondProfileCreated, value); }
        }

        private bool _isVisibleSecondProfile = false;

        public bool IsVisibleSecondProfile
        {
            get { return _isVisibleSecondProfile; }
            set { SetProperty(ref _isVisibleSecondProfile, value); }
        }

        private bool _thirdProfileCreated = false;

        public bool ThirdProfileCreated
        {
            get { return _thirdProfileCreated; }
            set { SetProperty(ref _thirdProfileCreated, value); }
        }

        private bool _isVisibleThirdProfile = false;

        public bool IsVisibleThirdProfile
        {
            get { return _isVisibleThirdProfile; }
            set { SetProperty(ref _isVisibleThirdProfile, value); }
        }

        private Pokemon _activatedProfile;

        public Pokemon ActivatedProfile
        {
            get { return _activatedProfile; }
            set { SetProperty(ref _activatedProfile, value); }
        }

        private Pokemon _notActivatedFirstProfile;

        public Pokemon NotActivatedFirstProfile
        {
            get { return _notActivatedFirstProfile; }
            set { SetProperty(ref _notActivatedFirstProfile, value); }
        }

        private Pokemon _notActivatedSecondProfile;

        public Pokemon NotActivatedSecondProfile
        {
            get { return _notActivatedSecondProfile; }
            set { SetProperty(ref _notActivatedSecondProfile, value); }
        }
        #endregion
        #endregion
    }
}
