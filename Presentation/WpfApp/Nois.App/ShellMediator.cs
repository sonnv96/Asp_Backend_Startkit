using AutoUpdaterDotNET;
using Nois.Core.Domain.Users;
using Nois.Framework.Infrastructure;
using Nois.Services.Users;
using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.Logger;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Nois.App
{
    public class ShellMediator : ViewModelMediator
    {
        private static readonly INoisLogger _log = NoisLogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));
        public ShellMediator()
            : base("ShellMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var shell = Application.Current.MainWindow as Shell;
            if (shell != null)
            {
                shell.DataContext = this;
                ViewComponent = shell;
            }

            Initialize();
        }

        private Shell Shell
        {
            get { return (Shell)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                "Login_Success",
                SendCommand.App_Shell_Hide
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case "Login_Success":
                    var notificationBody = notification.Body as NotificationBody;
                    var username = notificationBody.GetValue<string>("Username");

                    var userService = EngineContext.Instance.Resolve<IUserService>();
                    userService.GetUserByEmail("");
                    if (username.Equals("admin"))
                    {
                        UpdateVisibility = Visibility.Visible;
                        SettingsVisibility = Visibility.Visible;
                    }
                    else
                    {
                        UpdateVisibility = Visibility.Collapsed;
                        SettingsVisibility = Visibility.Collapsed;
                    }
                    OperationVisibility = Visibility.Visible;
                    ReportVisibility = Visibility.Hidden;
                    Shell.Show();
                    //SendNotification(SendCommand.App_Tcp_Show);
                    return;
                case SendCommand.App_Shell_Hide:
                    Shell.Hide();
                    return;
            }
        }

        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                RaisePropertyChanged("Version");
            }
        }

        private Visibility _operationVisibility;
        public Visibility OperationVisibility
        {
            get { return _operationVisibility; }
            set
            {
                _operationVisibility = value;
                RaisePropertyChanged("OperationVisibility");
            }
        }

        private Visibility _reportVisibility;
        public Visibility ReportVisibility
        {
            get { return _reportVisibility; }
            set
            {
                _reportVisibility = value;
                RaisePropertyChanged("ReportVisibility");
            }
        }

        private Visibility _updateVisibility;
        public Visibility UpdateVisibility
        {
            get { return _updateVisibility; }
            set
            {
                _updateVisibility = value;
                RaisePropertyChanged("UpdateVisibility");
            }
        }

        private Visibility _settingsVisibility;
        public Visibility SettingsVisibility
        {
            get { return _settingsVisibility; }
            set
            {
                _settingsVisibility = value;
                RaisePropertyChanged("SettingsVisibility");
            }
        }

        public ICommand UserClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SendNotification(SendCommand.App_UserInfo_Show);
                });
            }
        }

        public ICommand OperationClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    OperationVisibility = Visibility.Visible;
                    ReportVisibility = Visibility.Hidden;
                });
            }
        }

        public ICommand ReportClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    ReportVisibility = Visibility.Visible;
                    OperationVisibility = Visibility.Hidden;
                });
            }
        }

        public ICommand LogoutClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SendNotification(SendCommand.App_Logout);
                });
            }
        }

        public ICommand Closing
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            SendNotification(SendCommand.App_Stop);
                            Environment.Exit(-1);
                            return;
                        case MessageBoxResult.No:
                            return;
                    }
                });
            }
        }

        public ICommand Minimize
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Shell.WindowState = WindowState.Minimized;
                });
            }
        }

        public ICommand UpdateClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    AutoUpdater.Start("http://nois.newoceaninfosys.com:1253/update/version.xml");
                });
            }
        }

        public ICommand SettingsClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SendNotification(SendCommand.App_Settings_Show);
                });
            }
        }

        private void Initialize()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            Version = $"{assembly.GetName().Version}";
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            AutoUpdater.RemindLaterAt = 1;
            AutoUpdater.ReportErrors = true;
            AutoUpdater.RunUpdateAsAdmin = true;
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
