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
    public class TypePokViewModel : MvxViewModel
    {
        #region Fields
        private readonly IMvxNavigationService _navigation;
        private readonly ITypePokService _typePokService;
        #endregion

        #region Constructor
        public TypePokViewModel(IMvxNavigationService navigation, ITypePokService typePokService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _typePokService = typePokService;
        }
        #endregion

        #region Methods
        #region Public Methods
        public override void Prepare()
        {
            base.Prepare();
        }

        public override async Task Initialize()
        {
            LoadTypePokTask = MvxNotifyTask.Create(LoadTypePokAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task LoadTypePokAsync()
        {
            var result = await _typePokService.GetAllAsync();
            TypePoks = new MvxObservableCollection<TypePok>(result);
        }

        private async void RefreshAsync(MessageRefresh msg)
        {
            if (msg.Refresh)
                await LoadTypePokAsync();
        }
        #endregion
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand<string> FilterTypePoksCommand => new MvxAsyncCommand<string>(FilterTypePokAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task FilterTypePokAsync(string libelle)
        {
            Name = libelle;
            await LoadTypePokAsync();

        }
        #endregion

        #region Properties
        public MvxNotifyTask LoadTypePokTask { get; private set; }

        private MvxObservableCollection<TypePok> _typePoks;

        public MvxObservableCollection<TypePok> TypePoks
        {
            get { return _typePoks; }
            set { SetProperty(ref _typePoks, value); }
        }

        private string Name { get; set; } = "";
        #endregion
    }
}
