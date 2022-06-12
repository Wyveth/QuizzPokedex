using MvvmCross.Forms.Views;
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
    public partial class QuizzView : MvxContentPage<QuizzViewModel>
    {
        public QuizzView()
        {
            InitializeComponent();

            //List<ITransformation> ts = new List<ITransformation>();
            //ITransformation ImageTransformation = new GrayscaleTransformation();
            //ts.Add(ImageTransformation);
        }

        private void QuizzUnfinishedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuizzUnfinishedList.SelectedItem != null)
                QuizzUnfinishedList.SelectedItem = null;
        }
    }
}