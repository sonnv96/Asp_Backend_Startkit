using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.View;
using System.Collections.Generic;
using System.Windows.Input;

namespace Nois.App.Login.Views
{
    public class LoginBarMediator : ViewModelMediator
    {
        public string ShutdownIcon { get; set; }

        public LoginBarMediator()
            :base("LoginBarMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var loginBar = new LoginBar();
            loginBar.DataContext = this;
            ViewComponent = loginBar;
        }

        private LoginBar LoginBar
        {
            get { return (LoginBar)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
                           {
                                SendCommand.App_Login_ChangeUsername,
                                SendCommand.App_Logout
                           };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {

            switch (notification.Name)
            {
                case SendCommand.App_Login_ChangeUsername:
                    CurrentUsername = (string)notification.Body;
                    return;
                case SendCommand.App_Logout:
                    SendNotification(CommandName.LogoutController);
                    SendNotification(SendCommand.App_Stop);
                    return;
            }
        }

        private string _currentUsername;
        public string CurrentUsername
        {
            get { return _currentUsername; }
            set
            {
                _currentUsername = value;
                RaisePropertyChanged("CurrentUsername");
            }
        }

        public ICommand LogoutClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SendNotification(CommandName.LogoutController);
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
