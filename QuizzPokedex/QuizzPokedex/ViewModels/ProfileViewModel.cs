using Android.Graphics;
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
        private readonly ITypePokService _typePokService;
        private readonly object BitmapExtension;
        #endregion

        #region Constructor
        public ProfileViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IProfileService ProfileService, ITypePokService typePokService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _logger = logger;
            _profileService = ProfileService;
            _typePokService = typePokService;
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
            TypePok typePok = await _typePokService.GetByNameAsync(Constantes.Fire);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(typePok.DataFondGo, 0, typePok.DataFondGo.Length);
        }
        #endregion

        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand SaveCommandAsync => new MvxAsyncCommand(SaveAsync);

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
        #endregion

        #region PROPERTIES
        private Profile _Profile;
        public Profile Profile
        {
            get { return _Profile; }
            set { SetProperty(ref _Profile, value); }
        }

        private byte[] _backgroundType;

        public byte[] MyProperty
        {
            get { return _backgroundType; }
            set { _backgroundType = value; }
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
