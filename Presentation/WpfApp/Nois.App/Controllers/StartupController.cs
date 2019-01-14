using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PureMVC.Interfaces;
using System.Configuration;
using Nois.WpfApp.Framework;

namespace Nois.App.Controllers
{
    [CommandName(Name = "StartupCommand")]
    public class StartupController : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            //SendNotification(SendCommand.App_Login_Show);
            //SendNotification(SendCommand.App_Tcp_Show);
            //SendNotification(SendCommand.App_Log_Show);
            //int.TryParse(ConfigurationManager.AppSettings["Receiver"], out int _receiver);
            //int.TryParse(ConfigurationManager.AppSettings["Sender"], out int _sender);
            //int.TryParse(ConfigurationManager.AppSettings["Status"], out int _status);
            //int.TryParse(ConfigurationManager.AppSettings["Update"], out int _update);
            //int.TryParse(ConfigurationManager.AppSettings["Heartbeat"], out int _heartbeat);
            //SendNotification(SendCommand.App_Client_Initialize, _receiver, "Receiver");
            //SendNotification(SendCommand.App_Client_Initialize, _sender, "Sender");
            //SendNotification(SendCommand.App_Client_Initialize, _status, "Status");
            //SendNotification(SendCommand.App_Client_Initialize, _update, "Update");
            //SendNotification(SendCommand.App_Client_Initialize, _heartbeat, "Heartbeat");
            SendNotification("ApplicationStart");
        }
    }
}
