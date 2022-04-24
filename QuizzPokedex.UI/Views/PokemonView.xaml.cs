using MvvmCross.Forms.Views;
using QuizzPokedex.ViewModels;
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