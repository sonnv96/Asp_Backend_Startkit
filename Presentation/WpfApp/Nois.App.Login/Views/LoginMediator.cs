using log4net;
using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Nois.App.Login.Views
{
    public class LoginMediator : ViewModelMediator
    {
        public LoginMediator()
            : base("LoginMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var loginWindow = new LoginWindow();
            loginWindow.DataContext = this;
            ViewComponent = loginWindow;
            var dic = new Dictionary<string, string>();
            var value = dic[""];
        }

        private LoginWindow LoginWindow
        {
            get { return (LoginWindow)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
                           {
                               "ApplicationStart",
                               "Login_Success",
                               "Login_Fails"
                           };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {

            switch (notification.Name)
            {
                case "ApplicationStart":
                    Loading = false;
                    Password = "";
                    LoginWindow.Show();
                    ElementFocus = String.IsNullOrEmpty(Username) ? "username" : "password";
                    return;
                case "Login_Success":
                    LoginWindow.Hide();
                    return;
                case "Login_Fails":
                    Loading = false;
                    Message = (string)notification.Body;
                    ShowHideMessage = Visibility.Visible;
                    return;
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
            }
        }

        private string _username = "admin";
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                RaisePropertyChanged("Username");
            }
        }

        private string _elementFocus;
        public string ElementFocus
        {
            get { return _elementFocus; }
            set
            {
                _elementFocus = value;
                RaisePropertyChanged("ElementFocus");
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged("Loading");
            }
        }

        private Visibility _showHideMessage;
        public Visibility ShowHideMessage
        {
            get { return _showHideMessage; }
            set
            {
                _showHideMessage = value;
                RaisePropertyChanged("ShowHideMessage");
            }
        }

        public ICommand LoginClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    ElementFocus = "";
                    if (String.IsNullOrEmpty(Username))
                    {
                        Message = "Username can not be empty";
                        ShowHideMessage = Visibility.Visible;
                        ElementFocus = "username";
                    }
                    else if (String.IsNullOrEmpty(Password))
                    {
                        Message = "Password can not be empty";
                        ShowHideMessage = Visibility.Visible;
                        ElementFocus = "password";
                    }
                    else
                    {
                        Loading = true;
                        ShowHideMessage = Visibility.Collapsed;
                        ElementFocus = "";
                        SendNotification("LoginCommand", NotificationBody.Instance
                            .AddItem("Username", Username)
                            .AddItem("Password", Password));
                    }
                });
            }
        }

        public ICommand Closing
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Environment.Exit(-1);
                });
            }
        }

        public ICommand Minimize
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    LoginWindow.WindowState = WindowState.Minimized;
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
