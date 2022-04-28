using MvvmCross.Forms.Views;
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
            var transY = Convert.ToInt32(menuTest.TranslationY);
            if (transY == 0 &&
                e.VerticalDelta > 15)
            {
                var trans = menuTest.Height + menuTest.Margin.Top;
                var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();

                // Start both animations concurrently
                Task.WhenAll(
                    menuTest.TranslateTo(0, -(trans + safeInsets.Top), 200, Easing.CubicIn),
                    menuTest.FadeTo(0.25, 200));
            }
            else if (transY != 0 &&
                     e.VerticalDelta < 0 &&
                     Math.Abs(e.VerticalDelta) > 10)
            {
                Task.WhenAll(
                    menuTest.TranslateTo(0, 0, 200, Easing.CubicOut),
                    menuTest.FadeTo(1, 200));
            }
        }
    }
}