﻿using MvvmCross.Forms.Views;
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
	public partial class QTypTypPokVariousQuizzView : MvxContentPage<QTypTypPokVariousQuizzViewModel>
	{
		public QTypTypPokVariousQuizzView()
		{
			InitializeComponent ();
		}
    }
}