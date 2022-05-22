using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuizzPokedex.ViewModels
{
    public class ProfileViewModel : MvxViewModel<Profile>
    {
        #region Field
        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IProfileService _profileService;
        private readonly IMvxMessenger _messenger;
        #endregion

        #region Constructor
        public ProfileViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IProfileService ProfileService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _logger = logger;
            _profileService = ProfileService;
            _messenger = messenger;
        }
        #endregion

        #region Public Methods
        public override void Prepare(Profile profile)
        {
            Profile = profile;

            if (Profile.Id.Equals(0))
                //mode création
                TakePhotoCommandVisible = true;
            else
                //mode édition
                ModeUpdate = true;

            base.Prepare();
        }
        #endregion

        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand SaveCommandAsync => new MvxAsyncCommand(SaveAsync);
        public IMvxAsyncCommand TakePhotoCommandAsync => new MvxAsyncCommand(TakePhotoAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
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

        private async Task TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                //Affichage de la photo a partir du cache

                Photo = photo;
                TakePhotoCommandVisible = false;
                await RaisePropertyChanged(() => Photo);
                await RaisePropertyChanged(() => TakePhotoCommandVisible);

            }
            catch (Exception ex)
            {
                //_logger.ErrorException("Exception sur Photo", ex);
            }
        }
        #endregion

        #region PROPERTIES
        private Profile _Profile;
        public Profile Profile
        {
            get { return _Profile; }
            set { SetProperty(ref _Profile, value); }
        }

        private FileResult _photo;

        public FileResult Photo
        {
            get { return _photo; }
            set { SetProperty(ref _photo, value); }
        }

        private bool _takePhotoCommandVisible;

        public bool TakePhotoCommandVisible
        {
            get { return _takePhotoCommandVisible; }
            set { SetProperty(ref _takePhotoCommandVisible, value); }
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
