using Nois.WpfApp.Framework;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.App.Login.Controllers
{
    public class LogoutController : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            //Login
            SendNotification(SendCommand.App_Login_Show);
            SendNotification(SendCommand.App_Shell_Hide);
        }
    }
}
