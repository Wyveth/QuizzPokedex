using Microcharts;
using MvvmCross.Forms.Views;
using QuizzPokedex.ViewModels;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizzPokedex.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PokemonView : MvxContentPage<PokemonViewModel>
    {
        public PokemonView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ScrollViewPok.ScrollToAsync(0, 0, true);
            base.OnAppearing();
        }

        private void EvolutionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EvolutionList.SelectedItem != null)
                EvolutionList.SelectedItem = null;
        }

        private void MegaEvolutionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MegaEvolutionList.SelectedItem != null)
                MegaEvolutionList.SelectedItem = null;
        }

        private void GigaEvolutionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GigaEvolutionList.SelectedItem != null)
                GigaEvolutionList.SelectedItem = null;
        }

        private void AlolaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlolaList.SelectedItem != null)
                AlolaList.SelectedItem = null;
        }

        private void GalarList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GalarList.SelectedItem != null)
                GalarList.SelectedItem = null;
        }
        private void HisuiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HisuiList.SelectedItem != null)
                HisuiList.SelectedItem = null;
        }

        private void VariantList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VariantList.SelectedItem != null)
                VariantList.SelectedItem = null;
        }

        private void VarianteSexeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VarianteSexeList.SelectedItem != null)
                VarianteSexeList.SelectedItem = null;
        }
    }
}