using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace QuizzPokedex.ViewModels
{
    public class ProfilesViewModel : MvxViewModel
    {
        #region Fields
        private readonly IMvxNavigationService _navigation;
        private readonly IProfileService _profileService;
        //creation de l'abonnement ici (pour rafraichir via un abonné)
        private readonly MvxSubscriptionToken _token;
        #endregion

        #region Contructor
        public ProfilesViewModel(IMvxNavigationService navigation, IProfileService profileService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _profileService = profileService;
            _token = messenger.Subscribe<MessageRefresh>(RefreshAsync);
        }
        #endregion

        #region Public Methods
        public override void Prepare()
        {
            base.Prepare();
        }

        public override async Task Initialize()
        {
            LoadProfileTask = MvxNotifyTask.Create(LoadProfileAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task LoadProfileAsync()
        {
            var result = await _profileService.GetAllAsync();
            Profiles = new MvxObservableCollection<Profile>(result);
        }

        private async void RefreshAsync(MessageRefresh msg)
        {
            if (msg.Refresh)
                await LoadProfileAsync();
        }
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand CreateProfileCommandAsync => new MvxAsyncCommand(CreateProfileAsync);
        public IMvxAsyncCommand<Profile> UpdateProfileCommandAsync => new MvxAsyncCommand<Profile>(UpdateProfileAsync);
        public IMvxAsyncCommand<Profile> DeleteProfileCommandAsync => new MvxAsyncCommand<Profile>(DeleteProfileAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task CreateProfileAsync()
        {
            await _navigation.Navigate<ProfileViewModel, Profile>(new Profile());
        }

        private async Task UpdateProfileAsync(Profile profile)
        {
            await _navigation.Navigate<ProfileViewModel, Profile>(profile);
        }

        private async Task DeleteProfileAsync(Profile profile)
        {
            //Peut etre mettre une boite de dialogue de confirmation avant delete (leçon sur les dialogBox)

            await _profileService.DeleteAsync(profile).ContinueWith(
                async (result) =>
                    await LoadProfileAsync()
                    );
        }

        #endregion

        #region Properties
        public MvxNotifyTask LoadProfileTask { get; private set; }

        private MvxObservableCollection<Profile> _profiles;

        public MvxObservableCollection<Profile> Profiles
        {
            get { return _profiles; }
            set { SetProperty(ref _profiles, value); }
        }

        private int Categorie { get; set; } = 0;
        #endregion
    }
}
