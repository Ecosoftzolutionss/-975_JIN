using Microsoft.Practices.Unity;
using Prism.Unity;
using System;
using System.Windows;

namespace DSM
{
    public class Bootstrapper : UnityBootstrapper
    {
        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(runWithDefaultConfiguration);
        }

        /// <summary>
        /// Create and Show the Shell
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        /// <summary>
        /// Set MainWindow for prism
        /// </summary>
        protected override void InitializeShell()
        {
            App.Current.MainWindow = (Window)Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            Type dmsModuleType = typeof(DMSModule);
            ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo { ModuleName = dmsModuleType.Name, ModuleType = dmsModuleType.AssemblyQualifiedName });
        }
    }
}
