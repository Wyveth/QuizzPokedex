using MvvmCross.Forms.Views;
using QuizzPokedex.Models;
using QuizzPokedex.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizzPokedex.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileView : MvxContentPage<ProfileViewModel>
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        private async void StarterGrass_Clicked(object sender, EventArgs e)
        {
            await StarterGrass.RotateTo(40,250,Easing.BounceOut);
            await StarterGrass.RotateTo(-40, 250, Easing.BounceOut);
        }

        private async void StarterFire_Clicked(object sender, EventArgs e)
        {
            await StarterFire.RotateTo(40, 250, Easing.BounceOut);
            await StarterFire.RotateTo(-40, 250, Easing.BounceOut);
        }

        private async void StarterWater_Clicked(object sender, EventArgs e)
        {
            await StarterWater.RotateTo(40, 250, Easing.BounceOut);
            await StarterWater.RotateTo(-40, 250, Easing.BounceOut);
        }

        private async void StarterElectrik_Clicked(object sender, EventArgs e)
        {
            await StarterElectrik.RotateTo(40, 250, Easing.BounceOut);
            await StarterElectrik.RotateTo(-40, 250, Easing.BounceOut);
        }
    }
}