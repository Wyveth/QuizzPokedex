using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.Forms.Platforms.Android.Core;
using MvvmCross.IoC;
using QuizzPokedex.Droid.Configuration;
using QuizzPokedex.Interfaces;
using QuizzPokedex.UI;

namespace QuizzPokedex.Droid
{
    public class Setup : MvxFormsAndroidSetup<App, FormsApp>
    {
        protected override ILoggerFactory CreateLogFactory()
        {
            return null;
        }

        protected override ILoggerProvider CreateLogProvider()
        {
            return null;
        }

        protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
        {
            Mvx.IoCProvider.RegisterType<ISqliteConnectionService, AndroidSqliteConnectionService>();
            base.InitializeFirstChance(iocProvider);
        }
    }
}