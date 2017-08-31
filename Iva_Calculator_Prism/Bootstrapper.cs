using Microsoft.Practices.Unity;
using Prism.Unity;
using Iva_Calculator_Prism.Views;
using System.Windows;

namespace Iva_Calculator_Prism
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterTypeForNavigation<AccountsResultsView>("AccountsResultsView");
            Container.RegisterTypeForNavigation<SettingsView>("SettingsView");
        }
    }
}
