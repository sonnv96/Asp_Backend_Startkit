using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nois.App.Views
{
    public class SettingsMediator : ViewModelMediator
    {
        public SettingsMediator()
            : base("SettingsMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var settings = new Settings();
            settings.DataContext = this;
            ViewComponent = settings;

            PLCHost = ConfigurationManager.AppSettings["PLCHost"];
            Receiver = ConfigurationManager.AppSettings["Receiver"];
            Sender = ConfigurationManager.AppSettings["Sender"];
            Status = ConfigurationManager.AppSettings["Status"];
            Update = ConfigurationManager.AppSettings["Update"];
            Heartbeat = ConfigurationManager.AppSettings["Heartbeat"];
        }

        private Settings Settings
        {
            get { return (Settings)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                SendCommand.App_Settings_Show
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case SendCommand.App_Settings_Show:
                    Settings.ShowDialog();
                    return;
            }
        }

        private string _plcHost;
        public string PLCHost
        {
            get { return _plcHost; }
            set
            {
                _plcHost = value;
                RaisePropertyChanged("PLCHost");
            }
        }

        private string _sender;
        public string Sender
        {
            get { return _sender; }
            set
            {
                _sender = value;
                RaisePropertyChanged("Sender");
            }
        }

        private string _receiver;
        public string Receiver
        {
            get { return _receiver; }
            set
            {
                _receiver = value;
                RaisePropertyChanged("Receiver");
            }
        }

        private string _update;
        public string Update
        {
            get { return _update; }
            set
            {
                _update = value;
                RaisePropertyChanged("Update");
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        private string _heartbeat;
        public string Heartbeat
        {
            get { return _heartbeat; }
            set
            {
                _heartbeat = value;
                RaisePropertyChanged("Heartbeat");
            }
        }

        public ICommand OkCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var settings = configFile.AppSettings.Settings;
                    settings["PLCHost"].Value = PLCHost;
                    settings["Sender"].Value = Sender;
                    settings["Receiver"].Value = Receiver;
                    settings["Update"].Value = Update;
                    settings["Status"].Value = Status;
                    settings["Heartbeat"].Value = Heartbeat;
                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                    Settings.Hide();
                });
            }
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
