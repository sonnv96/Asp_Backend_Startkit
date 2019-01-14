using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nois.App
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return new Shell();
        }
        protected override void InitializeShell()
        {
            base.InitializeShell();
            var shellMediator = (ShellMediator)Facade.Instance.RetrieveMediator("ShellMediator");
            var shell = (Window)this.Shell; ;
            shell.DataContext = shellMediator;

            Application.Current.MainWindow = shell;
            shellMediator.ViewComponent = shell;
        }
        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

            var assemblies = new List<Assembly>();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string dll in Directory.GetFiles(path, "*.dll"))
                assemblies.Add(Assembly.LoadFile(dll));

            //var typeFinder = (ITypeFinder)Facade.Instance.RetrieveProxy(AppTypeFinder.Name);
            //assemblies.AddRange(typeFinder.GetAssemblies().Where(p => !p.IsDynamic));

            var moduleTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                moduleTypes.AddRange(assembly.GetExportedTypes()
                    .Where(ct => ct.GetInterfaces().FirstOrDefault() == typeof(IModule)));
            }
            foreach (var type in moduleTypes.Distinct())
                moduleCatalog.AddModule(type);
        }
    }
}
