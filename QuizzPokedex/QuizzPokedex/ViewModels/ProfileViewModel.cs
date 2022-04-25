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
    public class ProfileViewModel : MvxViewModel<Profile>
    {
        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IProfileService _ProfileService;
        private readonly IMvxMessenger _messenger;

        public ProfileViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IProfileService ProfileService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _logger = logger;
            _ProfileService = ProfileService;
            _messenger = messenger;
        }

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
                Profile.BirthDate != string.Empty )
            {
                if (ModeUpdate)
                    await _ProfileService.UpdateAsync(Profile);
                else
                    await _ProfileService.CreateAsync(Profile);

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

        //private async Task LoadPhotoFromCacheAsync(FileResult photo)
        //{
        //    if (photo!=null)
        //    {
        //        //recupération de la photo du cache suivant le device
        //        var filePhoto = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

        //        using (var stream = await photo.OpenReadAsync())
        //        {
        //            using (var newStream = File.OpenWrite(filePhoto))
        //            {
        //                await stream.CopyToAsync(newStream);
        //            }

        //            //On attribue a notre image la photo du cache pour prévisualiser le résultat
        //            var imageSource = ImageSource.FromFile(filePhoto);
        //            Photo = imageSource;
        //            TakePhotoCommandVisible = false;
        //            await RaisePropertyChanged(() => Photo);
        //            await RaisePropertyChanged(() => TakePhotoCommandVisible);
        //        }
        //    }
        //}
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
