
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.Forms.Platforms.Ios.Core;
using MvvmCross.IoC;
using QuizzPokedex.Interfaces;
using QuizzPokedex.iOS.Configuration;

namespace QuizzPokedex.iOS
{
    public class Setup : MvxFormsIosSetup<App, UI.FormsApp>
    {
        protected override ILoggerFactory CreateLogFactory()
        {
            throw new System.NotImplementedException();
        }

        protected override ILoggerProvider CreateLogProvider()
        {
            throw new System.NotImplementedException();
        }

        protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
        {
            Mvx.IoCProvider.RegisterType<ISqliteConnectionService, iOSSqliteConnectionService>();
            base.InitializeFirstChance(iocProvider);
        }
    }
}