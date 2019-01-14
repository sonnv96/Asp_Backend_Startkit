using Nois.App.TcpClient.Models;
using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.Timer;
using Nois.WpfApp.Framework.View;
using SimpleTCP;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Input;

namespace Nois.App.TcpClient.Views
{
    public class ClientListMediator : ViewModelMediator
    {
        ClientModel receiver, sender, status, update, plcStatus;
        private TimerChecker _timerChecker;
        private string _plcHost;
        private List<ClientModel> Clients;

        public ClientListMediator()
            : base("ClientListMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var clientList = new ClientList();
            clientList.DataContext = this;
            ViewComponent = clientList;

            _plcHost = ConfigurationManager.AppSettings["PLCHost"];
            Clients = new List<ClientModel>();
        }

        private ClientList ClientList
        {
            get { return (ClientList)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                SendCommand.App_Tcp_Show,
                SendCommand.App_Client_Write,
                SendCommand.App_Client_Initialize,
                SendCommand.App_Client_Stop
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case SendCommand.App_Tcp_Show:
                    ////receiver
                    //receiver = new ClientModel(_plcHost, 10000, "Receiver", Client_DataReceived, Client_StatusChange)
                    //{
                    //    ConnectClick = ConnectClick,
                    //};
                    //this.ClientList.Dispatcher.Invoke(() => { ControlList.Add(receiver); });
                    //receiver.Connect();
                    //Thread.Sleep(1000);
                    ////sender
                    //sender = new ClientModel(_plcHost, 10001, "Sender", Client_DataReceived, Client_StatusChange)
                    //{
                    //    ConnectClick = ConnectClick,
                    //};
                    //this.ClientList.Dispatcher.Invoke(() => { ControlList.Add(sender); });
                    //sender.Connect();
                    //Thread.Sleep(1000);
                    ////status
                    //status = new ClientModel(_plcHost, 3001, "Status", Client_DataReceived, Client_StatusChange)
                    //{
                    //    ConnectClick = ConnectClick,
                    //};
                    //this.ClientList.Dispatcher.Invoke(() => { ControlList.Add(status); });
                    //status.Connect();
                    //Thread.Sleep(1000);
                    ////update
                    //update = new ClientModel(_plcHost, 10002, "Update", Client_DataReceived, Client_StatusChange)
                    //{
                    //    ConnectClick = ConnectClick,
                    //};
                    //this.ClientList.Dispatcher.Invoke(() => { ControlList.Add(update); });
                    //update.Connect();

                    //Thread.Sleep(1000);
                    ////plc heartbit
                    //plcStatus = new ClientModel(_plcHost, 3000, "PlcStatus", PlcStatus_DataReceived, PlcStatus_StatusChange)
                    //{
                    //    ConnectClick = ConnectClick,
                    //};
                    //this.ClientList.Dispatcher.Invoke(() => { ControlList.Add(plcStatus); });
                    //plcStatus.Connect();

                    //_timerChecker = new TimerChecker(5, 1000, TimerCheckerStatusChange);
                    //_timerChecker.Start();
                    return;
                case SendCommand.App_Client_Initialize:
                    var port = (int)notification.Body;
                    if (port == 0)
                    {
                        return;
                    }
                    var name = notification.Type;
                    //check client is exist
                    var client = Clients.FirstOrDefault(x => x.Name.Equals(name));
                    if (client != null)
                    {
                        client.Connect();
                    }
                    else
                    {
                        client = new ClientModel(_plcHost, port, name, Client_DataReceived, Client_StatusChange)
                        {
                            ConnectClick = ConnectClick,
                        };
                        client.Connect();
                        Clients.Add(client);
                    }
                    return;
                case SendCommand.App_Client_Stop:
                    name = (string)notification.Body;
                    client = Clients.FirstOrDefault(x => x.Name.Equals(name));
                    if (client != null)
                    {
                        client.Disconnect();
                        Clients.Remove(client);
                    }
                    return;
                case SendCommand.App_Client_Write:
                    var message = (string)notification.Body;
                    sender.Write(message);
                    return;
            }
        }

        private ICollection<ClientModel> _controlList = new ObservableCollection<ClientModel>();
        public ICollection<ClientModel> ControlList
        {
            get { return _controlList; }
            set
            {
                _controlList = value;
                RaisePropertyChanged("ControlList");
            }
        }

        public ICommand ConnectClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    //var model = (ClientModel)obj;
                    //if (model.Connect())
                    //{
                    //    _log.Info("Client " + model.Name + " connected");
                    //}
                });
            }
        }

        private void Client_DataReceived(CustomizeSimpleTcpClient sender, Message e)
        {
            SendNotification(SendCommand.App_Operation_DataReceived, e.MessageString, sender.Port.ToString());
        }
        private void Client_StatusChange(CustomizeSimpleTcpClient sender, bool s)
        {
            //To do
            SendNotification(SendCommand.App_Operation_ChangeStatusPort, sender.Port, s ? "Connected" : "Disconnected");
        }
        private void PlcStatus_DataReceived(CustomizeSimpleTcpClient sender, Message e)
        {
            _timerChecker.Reset();
        }
        private void PlcStatus_StatusChange(CustomizeSimpleTcpClient sender, bool s)
        {
            if (s)
                _timerChecker.Reset();
            //To do
            SendNotification(SendCommand.App_Operation_ChangeStatusPort, sender.Port, s ? "Connected" : "Disconnected");
        }
        private void TimerCheckerStatusChange()
        {
            receiver.ReConnect();
            sender.ReConnect();
            status.ReConnect();
            update.ReConnect();
            plcStatus.ReConnect();
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
