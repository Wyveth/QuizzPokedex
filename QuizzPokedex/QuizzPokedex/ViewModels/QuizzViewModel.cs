using MvvmCross.Commands;
using MvvmCross.IoC;
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
    public class QuizzViewModel : MvxViewModel<Quizz>
    {
        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IQuizzService _quizzService;
        private readonly IPokemonService _pokemonService;
        private readonly IMvxMessenger _messenger;

        public QuizzViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IQuizzService quizzService, IMvxMessenger messenger, IPokemonService pokemonService)
        {
            _navigation = navigation;
            _logger = logger;
            _quizzService = quizzService;
            _pokemonService = pokemonService;
            _messenger = messenger;
        }

        public override void Prepare(Quizz quizz)
        {
            Quizz = quizz;

            base.Prepare();
        }

        public async override Task Initialize()
        {
            Pokemon = await _pokemonService.GetByNameAsync("Charpenti");
            await base.Initialize();
        }

        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }
        #endregion

        #region PROPERTIES
        private Quizz _quizz;
        public Quizz Quizz
        {
            get { return _quizz; }
            set { SetProperty(ref _quizz, value); }
        }

        private Pokemon _pokemon;

        public Pokemon Pokemon
        {
            get { return _pokemon; }
            set { SetProperty(ref _pokemon, value); }
        }
        #endregion
    }
}
