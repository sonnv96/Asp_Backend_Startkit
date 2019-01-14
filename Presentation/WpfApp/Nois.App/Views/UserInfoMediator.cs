using Nois.Core.Domain.Users;
using Nois.Framework.Infrastructure;
using Nois.Services.Users;
using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.View;
using System.Collections.Generic;
using System.Windows.Input;

namespace Nois.App.Views
{
    public class UserInfoMediator : ViewModelMediator
    {
        private IUserService _userService;

        public UserInfoMediator()
            : base("UserInfoMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var userInfo = new UserInfo();
            userInfo.DataContext = this;
            ViewComponent = userInfo;

            _userService = EngineContext.Instance.Resolve<IUserService>();
        }

        private UserInfo UserInfo
        {
            get { return (UserInfo)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                SendCommand.App_UserInfo_Show,
                SendCommand.App_Login_ChangeUsername
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case SendCommand.App_UserInfo_Show:
                    UserInfo.ShowDialog();
                    return;
                case SendCommand.App_Login_ChangeUsername:
                    var username = (string)notification.Body;
                    User = _userService.GetUserByUsername(username);
                    return;
            }
        }

        private User _user;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged("User");
            }
        }

        public ICommand HideCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    UserInfo.Hide();
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
