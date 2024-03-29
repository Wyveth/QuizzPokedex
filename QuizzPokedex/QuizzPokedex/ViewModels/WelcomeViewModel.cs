﻿using Android.Content.PM;
using Android.OS;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class WelcomeViewModel : MvxViewModel
    {
        #region Fields
        private readonly IMvxNavigationService _navigation;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;
        private readonly ITalentService _talentService;
        private readonly IAttaqueService _attaqueService;
        private readonly ITypeAttaqueService _typeAttaqueService;
        private readonly IProfileService _profileService;
        private readonly MvxSubscriptionToken _token;
        #endregion

        #region Constructor
        public WelcomeViewModel(IMvxNavigationService navigation, IMvxMessenger messenger, IPokemonService pokemonService, ITypePokService typePokService, ITalentService talentService, IAttaqueService attaqueService, ITypeAttaqueService typeAttaqueService, IProfileService profileService)
        {
            _navigation = navigation;
            _token = messenger.Subscribe<MessageRefresh>(RefreshAsync);
            _pokemonService = pokemonService;
            _typePokService = typePokService;
            _talentService = talentService;
            _attaqueService = attaqueService;
            _typeAttaqueService = typeAttaqueService;
            _profileService = profileService;
        }
        #endregion

        #region Public Methods
        public override async Task Initialize()
        {
            ButtonIsEnabled = false;
            PackageManager pm = Android.App.Application.Context.PackageManager;
            PackageInfo pInfo = pm.GetPackageInfo(Android.App.Application.Context.ApplicationInfo.PackageName, PackageInfoFlags.MetaData);
            Version = pInfo.VersionName;
            BackGroundTask = MvxNotifyTask.Create(BackGroundAsync);
            ProgressBarTask = MvxNotifyTask.Create(ProgressBarAsync);
            ProfileTask = MvxNotifyTask.Create(ProfileAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task BackGroundAsync()
        {
            ImgPokedexUp = await Utils.GetByteAssetImage(Constantes.Pokedex_Up);
            ImgPokedexDown = await Utils.GetByteAssetImage(Constantes.Pokedex_Down);
        }

        private async Task ProgressBarAsync()
        {
            ProgressBarIsVisible = false;

            int nbTypeAttaqueMax = await _typeAttaqueService.GetNumberJsonAsync();
            int nbTypeAttaqueInDb = await _typeAttaqueService.GetNumberInDbAsync();

            if (!nbTypeAttaqueInDb.Equals(nbTypeAttaqueMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbTypeAttaqueInDb, nbTypeAttaqueMax);
                TextProgressBar = "Création Type Attaque: " + nbTypeAttaqueInDb.ToString() + "/" + nbTypeAttaqueMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbTypeAttaqueInDb = await _typeAttaqueService.GetNumberInDbAsync();
                    ValueProgressBar = await getPercent(nbTypeAttaqueInDb, nbTypeAttaqueMax);
                    TextProgressBar = "Création Type Attaque: " + nbTypeAttaqueInDb.ToString() + "/" + nbTypeAttaqueMax.ToString();
                }

                ProgressBarIsVisible = false;
            }

            int nbTypePokMax = await _typePokService.GetNumberJsonAsync();
            int nbTypePokInDb = await _typePokService.GetNumberInDbAsync();

            if (!nbTypePokInDb.Equals(nbTypePokMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbTypePokInDb, nbTypePokMax);
                TextProgressBar = "Création Type Pokémon: " + nbTypePokInDb.ToString() + "/" + nbTypePokMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbTypePokInDb = await _typePokService.GetNumberInDbAsync();
                    ValueProgressBar = await getPercent(nbTypePokInDb, nbTypePokMax);
                    TextProgressBar = "Création Type Pokémon: " + nbTypePokInDb.ToString() + "/" + nbTypePokMax.ToString();
                }

                ProgressBarIsVisible = false;
            }

            int nbAttaqueMax = await _attaqueService.GetNumberJsonAsync();
            int nbAttaqueInDb = await _attaqueService.GetNumberInDbAsync();

            if (!nbAttaqueInDb.Equals(nbAttaqueMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbAttaqueInDb, nbAttaqueMax);
                TextProgressBar = "Création Attaque: " + nbAttaqueInDb.ToString() + "/" + nbAttaqueMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbAttaqueInDb = await _attaqueService.GetNumberInDbAsync();
                    ValueProgressBar = await getPercent(nbAttaqueInDb, nbAttaqueMax);
                    TextProgressBar = "Création Attaque: " + nbAttaqueInDb.ToString() + "/" + nbAttaqueMax.ToString();
                }

                ProgressBarIsVisible = false;
            }

            int nbTalentMax = await _talentService.GetNumberJsonAsync();
            int nbTalentInDb = await _talentService.GetNumberInDbAsync();

            if (!nbTalentInDb.Equals(nbTalentMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbTalentInDb, nbTalentMax);
                TextProgressBar = "Création Talent: " + nbTalentInDb.ToString() + "/" + nbTalentMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbTalentInDb = await _talentService.GetNumberInDbAsync();
                    ValueProgressBar = await getPercent(nbTalentInDb, nbTalentMax);
                    TextProgressBar = "Création Talent: " + nbTalentInDb.ToString() + "/" + nbTalentMax.ToString();
                }

                ProgressBarIsVisible = false;
            }

            int nbPokMax = await _pokemonService.GetNumberJsonAsync();
            int nbPokInDb = await _pokemonService.GetNumberInDbAsync();

            if (!nbPokInDb.Equals(nbPokMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbPokInDb, nbPokMax);
                TextProgressBar = "Création DB: " + nbPokInDb.ToString() + "/" + nbPokMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbPokInDb = await _pokemonService.GetNumberInDbAsync();
                    ValueProgressBar = await getPercent(nbPokInDb, nbPokMax);
                    TextProgressBar = await GetTextCreateProgressBar(nbPokInDb, nbPokMax);
                }

                ProgressBarIsVisible = false;
            }
            
            int nbPokNotUpdated = await _pokemonService.GetNumberPokUpdateAsync();

            if (!nbPokNotUpdated.Equals(nbPokMax))
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbPokNotUpdated, nbPokMax);
                TextProgressBar = "Check DB: " + nbPokNotUpdated.ToString() + "/" + nbPokMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbPokNotUpdated = await _pokemonService.GetNumberPokUpdateAsync();
                    ValueProgressBar = await getPercent(nbPokNotUpdated, nbPokMax);
                    TextProgressBar = await GetTextCheckProgressBar(nbPokNotUpdated, nbPokMax);
                }

                ProgressBarIsVisible = false;
            }

            int nbTypeAttaqueChecked = await _typeAttaqueService.GetNumberCheckAsync();
            ProgressBarIsVisible = true;
            
            if (!nbTypeAttaqueChecked.Equals(nbTypeAttaqueMax))
            {

                ValueProgressBar = await getPercent(nbTypeAttaqueChecked, nbTypeAttaqueMax);
                TextProgressBar = "Check TypeAttaque: " + nbTypeAttaqueChecked.ToString() + "/" + nbTypeAttaqueMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbTypeAttaqueChecked = await _typeAttaqueService.GetNumberCheckAsync();
                    ValueProgressBar = await getPercent(nbTypeAttaqueChecked, nbTypeAttaqueMax);
                    TextProgressBar = await GetTextCheckSpriteProgressBar(nbTypeAttaqueChecked, nbTypeAttaqueMax);
                }
            }

            int nbTypePokChecked = await _typePokService.GetNumberCheckAsync();
            ProgressBarIsVisible = true;

            if (!nbTypePokChecked.Equals(nbTypePokMax))
            {

                ValueProgressBar = await getPercent(nbTypePokChecked, nbTypePokMax);
                TextProgressBar = "Check TypePok: " + nbTypePokChecked.ToString() + "/" + nbTypePokMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbTypePokChecked = await _typePokService.GetNumberCheckAsync();
                    ValueProgressBar = await getPercent(nbTypePokChecked, nbTypePokMax);
                    TextProgressBar = await GetTextCheckSpriteProgressBar(nbTypePokChecked, nbTypePokMax);
                }
            }

            int nbPokChecked = await _pokemonService.GetNumberCheckAsync();
            ProgressBarIsVisible = true;
            
            if (!nbPokChecked.Equals(nbPokMax))
            {
                
                ValueProgressBar = await getPercent(nbPokChecked, nbPokMax);
                TextProgressBar = "Check Sprite: " + nbPokChecked.ToString() + "/" + nbPokMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbPokChecked = await _pokemonService.GetNumberCheckAsync();
                    ValueProgressBar = await getPercent(nbPokChecked, nbPokMax);
                    TextProgressBar = await GetTextCheckSpriteProgressBar(nbPokChecked, nbPokMax);
                }
            }

            ValueProgressBar = 1;
            TextProgressBar = "Initialisation de l'application: Terminé!";
            await Task.Delay(2000);

            ProgressBarIsVisible = false;
            ButtonIsEnabled = true;
        }

        private async Task<string> GetTextCreateProgressBar(int nbPokInDb, int nbPokMax)
        {
            string total = nbPokInDb.ToString() + "/" + nbPokMax.ToString();
            StringBuilder result = new StringBuilder();
            if (nbPokInDb <= 211)
                result.Append("Création Gen 1: " + total);
            else if (nbPokInDb <= 322)
                result.Append("Création Gen 2: " + total);
            else if (nbPokInDb <= 484)
                result.Append("Création Gen 3: " + total);
            else if (nbPokInDb <= 613)
                result.Append("Création Gen 4: " + total);
            else if (nbPokInDb <= 797)
                result.Append("Création Gen 5: " + total);
            else if (nbPokInDb <= 889)
                result.Append("Création Gen 6: " + total);
            else if (nbPokInDb <= 989)
                result.Append("Création Gen 7: " + total);
            else if (nbPokInDb <= 1104)
                result.Append("Création Gen 8: " + total);
            else if (nbPokInDb <= 1114)
                result.Append("Création Gen Arceus: " + total);
            else
                result.Append("Création Gen 9: " + total);

            return await Task.FromResult(result.ToString());
        }

        private async Task<string> GetTextCheckProgressBar(int nbPokInDb, int nbPokMax)
        {
            string total = nbPokInDb.ToString() + "/" + nbPokMax.ToString();
            StringBuilder result = new StringBuilder();
            if (nbPokInDb <= 211)
                result.Append("Check Gen 1: " + total);
            else if (nbPokInDb <= 322)
                result.Append("Check Gen 2: " + total);
            else if (nbPokInDb <= 484)
                result.Append("Check Gen 3: " + total);
            else if (nbPokInDb <= 613)
                result.Append("Check Gen 4: " + total);
            else if (nbPokInDb <= 797)
                result.Append("Check Gen 5: " + total);
            else if (nbPokInDb <= 889)
                result.Append("Check Gen 6: " + total);
            else if (nbPokInDb <= 989)
                result.Append("Check Gen 7: " + total);
            else if (nbPokInDb <= 1104)
                result.Append("Check Gen 8: " + total);
            else if (nbPokInDb <= 1114)
                result.Append("Check Gen Arceus: " + total);
            else
                result.Append("Check Gen 9: " + total);

            return await Task.FromResult(result.ToString());
        }

        private async Task<string> GetTextCheckSpriteProgressBar(int nbPokInDb, int nbPokMax)
        {
            string total = nbPokInDb.ToString() + "/" + nbPokMax.ToString();
            StringBuilder result = new StringBuilder();
            if (nbPokInDb <= 211)
                result.Append("Check Sprite Gen 1: " + total);
            else if (nbPokInDb <= 322)
                result.Append("Check Sprite Gen 2: " + total);
            else if (nbPokInDb <= 484)
                result.Append("Check Sprite Gen 3: " + total);
            else if (nbPokInDb <= 613)
                result.Append("Check Sprite Gen 4: " + total);
            else if (nbPokInDb <= 797)
                result.Append("Check Sprite Gen 5: " + total);
            else if (nbPokInDb <= 889)
                result.Append("Check Sprite Gen 6: " + total);
            else if (nbPokInDb <= 989)
                result.Append("Check Sprite Gen 7: " + total);
            else if (nbPokInDb <= 1104)
                result.Append("Check Sprite Gen 8: " + total);
            else if (nbPokInDb <= 1114)
                result.Append("Check Sprite Gen Arceus: " + total);
            else
                result.Append("Check Sprite Gen 9: " + total);

            return await Task.FromResult(result.ToString());
        }

        private async Task<double> getPercent(double nbPok, double nbPokMax)
        {
            double percent = 0;
            var result = nbPok / nbPokMax;
            percent = double.Parse(result.ToString());
            return await Task.FromResult(percent);
        }

        private async Task ProfileAsync()
        {
            List<Profile> profiles = await _profileService.GetAllAsync();
            ActivatedProfile = profiles.Find(m => m.Activated.Equals(true));

            if (profiles.Count >= 1)
            {
                FirstProfileCreated = true;
                ActivatedPokemonProfile = await _pokemonService.GetByIdAsync(ActivatedProfile.PokemonID);
            }

            if (profiles.Count >= 2)
            {
                SecondProfileCreated = false;
                IsVisibleSecondProfile = false;
                IsVisibleThirdProfile = false;
            }

            if (profiles.Count == 3)
            {
                ThirdProfileCreated = false;
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

        public IMvxAsyncCommand ShowHideOtherProfileCommandAsync => new MvxAsyncCommand(ShowHideOtherProfileAsync);

        public IMvxAsyncCommand ActivatedProfileLongCommandAsync => new MvxAsyncCommand(ActivatedProfileLongAsync);

        public IMvxAsyncCommand<Profile> OpenModalChangeProfileCommandAsync => new MvxAsyncCommand<Profile>(OpenModalChangeProfileAsync);

        public IMvxAsyncCommand CloseModalChangeProfileCommandAsync => new MvxAsyncCommand(CloseModalChangeProfileAsync);

        public IMvxAsyncCommand ChangeProfileCommandAsync => new MvxAsyncCommand(ChangeProfileAsync);

        private async Task NavigationQuizzAsync()
        {
            int countProfile = await _profileService.CountGetAllAsync();

            if (countProfile > 0)
                await _navigation.Navigate<QuizzViewModel>();
            else
                await _navigation.Navigate<ProfileViewModel, Profile>(new Profile());
        }

        private async Task NavigationProfileAsync()
        {
            await _navigation.Navigate<ProfileViewModel, Profile>(new Profile());
        }

        private async Task NavigationPokedexAsync()
        {
            int countProfile = await _profileService.CountGetAllAsync();

            if (countProfile > 0)
                await _navigation.Navigate<PokedexViewModel>();
            else
                await _navigation.Navigate<ProfileViewModel, Profile>(new Profile());
        }

        private async Task ShowHideOtherProfileAsync()
        {
            List<Profile> profiles = await _profileService.GetAllAsync();
            List<Profile> profileNotActivated = profiles.FindAll(m => m.Activated.Equals(false));

            if (profiles.Count >= 1)
            {
                ActivatedProfile = profiles.Find(m => m.Activated.Equals(true));
                ActivatedPokemonProfile = await _pokemonService.GetByIdAsync(ActivatedProfile.PokemonID);
                IsVisibleSecondProfile = !IsVisibleSecondProfile;
            }

            if (profiles.Count >= 2)
            {
                SecondProfileCreated = !SecondProfileCreated;
                NotActivatedFirstProfile = profileNotActivated[0];
                NotActivatedPokemonFirstProfile = await _pokemonService.GetByIdAsync(profileNotActivated[0].PokemonID);
                IsVisibleThirdProfile = !IsVisibleThirdProfile;
            }

            if (profiles.Count == 3)
            {
                ThirdProfileCreated = !ThirdProfileCreated;
                NotActivatedSecondProfile = profileNotActivated[1];
                NotActivatedPokemonSecondProfile = await _pokemonService.GetByIdAsync(profileNotActivated[1].PokemonID);
            }
        }

        private async Task ActivatedProfileLongAsync()
        {
            await Task.Run(() =>
            {
            });
        }

        private async Task OpenModalChangeProfileAsync(Profile profile)
        {
            SelectedProfileChange = profile;
            MsgChangeProfile = "Veux-tu activer le profil de " + profile.Name + "?";
            await ShowHideModal();
        }

        private async Task CloseModalChangeProfileAsync()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
                IsVisibleModalChangeProfile = !IsVisibleModalChangeProfile;
                SelectedProfileChange = null;
            });
        }

        private async Task ChangeProfileAsync()
        {
            await _profileService.UpdateProfileActivatedAsync(SelectedProfileChange);
            SelectedProfileChange = null;
            await ProfileAsync();
            await ShowHideModal();
        }

        private async Task ShowHideModal()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
                IsVisibleModalChangeProfile = !IsVisibleModalChangeProfile;
            });
        }
        #endregion

        #region Properties
        #region Collection
        public MvxNotifyTask BackGroundTask { get; private set; }
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

        private bool _buttonIsEnabled;
        public bool ButtonIsEnabled
        {
            get { return _buttonIsEnabled; }
            set { SetProperty(ref _buttonIsEnabled, value); }
        }

        #endregion

        #region Profile
        private string _msgChangeProfile;

        public string MsgChangeProfile
        {
            get { return _msgChangeProfile; }
            set { SetProperty(ref _msgChangeProfile, value); }
        }

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

        private Profile _selectedProfileChange;

        public Profile SelectedProfileChange
        {
            get { return _selectedProfileChange; }
            set { SetProperty(ref _selectedProfileChange, value); }
        }

        private Profile _activatedProfile;

        public Profile ActivatedProfile
        {
            get { return _activatedProfile; }
            set { SetProperty(ref _activatedProfile, value); }
        }

        private Pokemon _activatedPokemonProfile;

        public Pokemon ActivatedPokemonProfile
        {
            get { return _activatedPokemonProfile; }
            set { SetProperty(ref _activatedPokemonProfile, value); }
        }

        private Profile _notActivatedFirstProfile;

        public Profile NotActivatedFirstProfile
        {
            get { return _notActivatedFirstProfile; }
            set { SetProperty(ref _notActivatedFirstProfile, value); }
        }

        private Pokemon _notActivatedPokemonFirstProfile;

        public Pokemon NotActivatedPokemonFirstProfile
        {
            get { return _notActivatedPokemonFirstProfile; }
            set { SetProperty(ref _notActivatedPokemonFirstProfile, value); }
        }

        private Profile _notActivatedSecondProfile;

        public Profile NotActivatedSecondProfile
        {
            get { return _notActivatedSecondProfile; }
            set { SetProperty(ref _notActivatedSecondProfile, value); }
        }

        private Pokemon _notActivatedPokemonSecondProfile;

        public Pokemon NotActivatedPokemonSecondProfile
        {
            get { return _notActivatedPokemonSecondProfile; }
            set { SetProperty(ref _notActivatedPokemonSecondProfile, value); }
        }
        #endregion

        #region Modal
        private bool _isVisibleBackgroundModalFilter = false;

        public bool IsVisibleBackgroundModalFilter
        {
            get { return _isVisibleBackgroundModalFilter; }
            set { SetProperty(ref _isVisibleBackgroundModalFilter, value); }
        }

        private bool _isVisibleModalChangeProfile = false;

        public bool IsVisibleModalChangeProfile
        {
            get { return _isVisibleModalChangeProfile; }
            set { SetProperty(ref _isVisibleModalChangeProfile, value); }
        }
        #endregion

        #region Image Background
        private byte[] _imgPokedexUp;

        public byte[] ImgPokedexUp
        {
            get { return _imgPokedexUp; }
            set { SetProperty(ref _imgPokedexUp, value); }
        }

        private byte[] _imgPokedexDown;

        public byte[] ImgPokedexDown
        {
            get { return _imgPokedexDown; }
            set { SetProperty(ref _imgPokedexDown, value); }
        }
        #endregion

        #region Version
        private string _version;

        public string Version
        {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }
        #endregion
        #endregion
    }
}
