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
        private readonly IMvxMessenger _messenger;

        public QuizzViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IQuizzService quizzService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _logger = logger;
            _quizzService = quizzService;
            _messenger = messenger;
        }

        public override void Prepare(Quizz quizz)
        {
            Quizz = quizz;

            if (!Quizz.Id.Equals(0))
                //mode édition
                ModeUpdate = true;

            base.Prepare();
        }

        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand SaveCommandAsync => new MvxAsyncCommand(SaveAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task SaveAsync()
        {
            //Save le quizz si complet (cf ci dessous)
            if (Quizz.Id != 0)
            {
                if (ModeUpdate)
                    await _quizzService.UpdateQuizzAsync(Quizz);
                else
                {
                    //on va chercher le number le plus haut et on ajoute 1
                    //var number = await _quizzService.FindNumberEtapeAsync(Quizz.IdRecette);
                    //Quizz.Number = number + 1;
                    await _quizzService.CreateQuizzAsync(Quizz);
                }

                //nouvel enregistrement crée on informe par abonnement de rafraichir
                var refresh = new MessageRefresh(this, true);
                _messenger.Publish(refresh);

                await _navigation.Close(this);
            }
        }
        #endregion

        #region PROPERTIES
        private Quizz _quizz;
        public Quizz Quizz
        {
            get { return _quizz; }
            set { SetProperty(ref _quizz, value); }
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
