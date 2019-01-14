using Nois.Core.Domain.Users;
using Nois.Framework.Infrastructure;
using Nois.Services.Users;
using Nois.WpfApp.Framework;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace Nois.App.Login.Controllers
{
    [CommandName(Name = "LoginCommand")]
    public class LoginController : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            var notificationBody = notification.Body as NotificationBody;
            var username = notificationBody.GetValue<string>("Username");
            var password = notificationBody.GetValue<string>("Password");

            var userRegistrationService = EngineContext.Instance.Resolve<IUserRegistrationService>();

            var message = "";
            var loginResult = userRegistrationService.ValidateUser(username, password);

            if (loginResult.UserLoginResult != UserLoginResult.Successful)
            {
                switch (loginResult.UserLoginResult)
                {
                    case UserLoginResult.UserNotExist:
                        message = "User is not exist";
                        break;
                    case UserLoginResult.Deleted:
                        message = "User was deleted";
                        break;
                    case UserLoginResult.NotActive:
                        message = "User is not actived";
                        break;
                    case UserLoginResult.WrongPIN:
                        message = "Wrong PIN";
                        break;
                    case UserLoginResult.WrongPassword:
                    default:
                        message = "Wrong Credentials";
                        break;
                }
                SendNotification("Login_Fails", message);
            }
            else
            {
                //Login
                SendNotification("Login_Success", NotificationBody.Instance.AddItem("UserLoginResult", loginResult)
                                                                            .AddItem("Username", username));
            }
        }
    }
}
