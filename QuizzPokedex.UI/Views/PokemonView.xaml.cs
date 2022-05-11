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
    }
}