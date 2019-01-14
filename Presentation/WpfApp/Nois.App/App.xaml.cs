using log4net.Config;
using Nois.Framework.Infrastructure;
using Nois.WpfApp.Framework.Infrastructure;
using Nois.WpfApp.Framework.Logger;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Nois.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly INoisLogger _log = NoisLogManager.GetLogger(typeof(App));
        private void ApplicationStartUp(object sender, StartupEventArgs e)
        {
            //Process[] processes = Process.GetProcessesByName("Nois.App");
            //if (processes.Count() > 1)
            //    processes.OrderBy(p => p.StartTime).FirstOrDefault().Kill();

            //var Serial = ConfigurationManager.AppSettings["Serial"];
            //if (Result("Lensmode", GetID(), Serial) != 1)
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show("Invalid license", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    Environment.Exit(-1);
            //}

            try
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Dispatcher.UnhandledException += DispatcherOnUnhandledException;

                DOMConfigurator.Configure();
                _log.Info("", "Application start");
                _log.Debug("", "Debug");

                EngineContext.Initialize(new WpfEngine());

                var facade = (AppFacade)AppFacade.Instance;
                Bootstrapper bootstrapper = new Bootstrapper();
                bootstrapper.Run();
                facade.Startup("StartupCommand");
            }
            catch (Exception ex)
            {
                _log.Fatal("", ex.Message);
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Application startup fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Error("", "CurrentDomain-UnhandledException: " + e.ExceptionObject);
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(e.Exception.InnerException.Message);
            e.Handled = true;
        }

        public string GetID()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)
                {
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }

        public int Result(string product, string id, string key)
        {
            try
            {
                if (KeyGenerate(product, id) == key)
                {
                    return 1;
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public string KeyGenerate(string product, string id)
        {
            var program_name_string = product;
            UInt64 program_name = 1;
            foreach (var c in program_name_string)
            {
                program_name *= Convert.ToUInt64(c);
            }

            var user_id_string = id;
            UInt64 user_id = 1;
            foreach (var c in user_id_string)
            {
                user_id += Convert.ToUInt64(c);
            }

            var product_key = EncryptKey(program_name, user_id * program_name);

            return product_key.ToString();
        }

        public string EncryptKey(UInt64 seed, UInt64 value)
        {
            Random rand = new Random((int)seed);
            var key = value ^ (UInt64)(UInt64.MaxValue * rand.NextDouble());
            return key.ToString();
        }
    }
}
