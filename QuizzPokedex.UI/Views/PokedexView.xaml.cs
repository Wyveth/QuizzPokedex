﻿using MvvmCross;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;
using QuizzPokedex.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace QuizzPokedex.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PokedexView : MvxContentPage<PokedexViewModel>
    {
        public PokedexView()
        {
            InitializeComponent();
        }

        private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var transY = Convert.ToInt32(searchFilter.TranslationY);
            if (transY == 0 &&
                e.VerticalDelta > 15)
            {
                var trans = searchFilter.Height + searchFilter.Margin.Top;
                var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();

                // Start both animations concurrently
                Task.WhenAll(
                    searchFilter.TranslateTo(0, -(trans + safeInsets.Top), 200, Easing.CubicIn),
                    searchFilter.FadeTo(0, 200),
                    backButton.TranslateTo(0, -(trans + safeInsets.Top), 200, Easing.CubicIn),
                    backButton.FadeTo(0, 200));
            }
            else if (transY != 0 &&
                     e.VerticalDelta < 0 &&
                     Math.Abs(e.VerticalDelta) > 10)
            {
                Task.WhenAll(
                    searchFilter.TranslateTo(0, 0, 200, Easing.CubicOut),
                    searchFilter.FadeTo(1, 200),
                    backButton.TranslateTo(0, 0, 200, Easing.CubicOut),
                    backButton.FadeTo(1, 200));
            }

            if (transY == 0 &&
                e.VerticalDelta < -15)
            {
                var trans = menuFilter.Height + menuFilter.Margin.Bottom;
                var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();

                // Start both animations concurrently
                Task.WhenAll(
                    menuFilter.TranslateTo(0, (trans + safeInsets.Bottom), 200, Easing.CubicIn),
                    menuFilter.FadeTo(0, 200),
                    filterModal.TranslateTo(0, (trans + safeInsets.Bottom), 200, Easing.CubicIn),
                    filterModal.FadeTo(0, 200));
            }
            else if (transY != 0 &&
                     e.VerticalDelta > 0 &&
                     Math.Abs(e.VerticalDelta) > 10)
            {
                Task.WhenAll(
                    menuFilter.TranslateTo(0, 0, 200, Easing.CubicOut),
                    menuFilter.FadeTo(1, 200),
                    filterModal.TranslateTo(0, 0, 200, Easing.CubicOut),
                    filterModal.FadeTo(1, 200));
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PokemonList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PokemonList.SelectedItem != null)
                PokemonList.SelectedItem = null;
        }
    }
}