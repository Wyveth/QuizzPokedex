using MvvmCross.IoC;
using MvvmCross.ViewModels;
using QuizzPokedex.CustomStart;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            //register service
            CreatableTypes().EndingWith("Service").AsInterfaces().RegisterAsLazySingleton();

            //Demarrer vers la première page FirstViewModel            
            RegisterCustomAppStart<AppStart>();
        }
    }
}
