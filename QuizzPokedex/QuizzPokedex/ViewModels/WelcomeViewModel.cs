using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class WelcomeViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigation;
        private readonly IPokemonService _pokemonService;

        public WelcomeViewModel(IMvxNavigationService navigation, IPokemonService pokemonService)
        {
            _navigation = navigation;
            _pokemonService = pokemonService;
        }

        public override async Task Initialize()
        {
            ProgressBarTask = MvxNotifyTask.Create(ProgressBarAsync);
            await base.Initialize();
        }

        private async Task ProgressBarAsync()
        {
            int nbPokMax = await _pokemonService.GetNumberPokJsonAsync();
            int nbPok = await _pokemonService.GetNumberInDbAsync();
            int nbPokNotUpdated = await _pokemonService.GetNumberPokUpdateAsync();
            double x = 0;
            if (nbPok.Equals(nbPokMax))
            {
                ProgressBarIsVisible = false;
            }
            else
            {
                ProgressBarIsVisible = true;
                ValueProgressBar = await getPercent(nbPok, nbPokMax);
                TextProgressBar = "Création: " + nbPok.ToString() + "/" + nbPokMax.ToString();

                while (ValueProgressBar != 1)
                {
                    nbPok = await _pokemonService.GetNumberInDbAsync();
                    ValueProgressBar = await getPercent(nbPok, nbPokMax);
                    TextProgressBar = "Création: " + nbPok.ToString() + "/" + nbPokMax.ToString();
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

        public IMvxAsyncCommand NavigationQuizzCommandAsync => new MvxAsyncCommand(NavigationQuizzAsync);
        public IMvxAsyncCommand NavigationPokedexCommandAsync => new MvxAsyncCommand(NavigationPokedexAsync);

        private async Task NavigationQuizzAsync()
        {
            await _navigation.Navigate<QuizzViewModel>();
        }

        private async Task NavigationPokedexAsync()
        {
            await _navigation.Navigate<PokedexViewModel>();
        }

        #region Properties
        #region Collection
        public MvxNotifyTask ProgressBarTask { get; private set; }
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
        #endregion
    }
}
