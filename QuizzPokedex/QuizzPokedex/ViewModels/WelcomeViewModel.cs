using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class WelcomeViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigation;

        public WelcomeViewModel(IMvxNavigationService navigation)
        {
            _navigation = navigation;
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
    }
}
