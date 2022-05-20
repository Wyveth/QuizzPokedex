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
        private readonly ITypePokService _typePokService;

        public WelcomeViewModel(IMvxNavigationService navigation, IPokemonService pokemonService, ITypePokService typePokService)
        {
            _navigation = navigation;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
        }

        public override async Task Initialize()
        {
            ProgressBarTask = MvxNotifyTask.Create(ProgressBarAsync);
            await base.Initialize();
        }

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
