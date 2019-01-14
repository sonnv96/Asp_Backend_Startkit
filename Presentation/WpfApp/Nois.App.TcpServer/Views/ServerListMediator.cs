using log4net;
using Nois.App.TcpServer.Models;
using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;

namespace Nois.App.TcpServer.Views
{
    public class ServerListMediator : ViewModelMediator
    {
        private string _plcHost;
        private static readonly ILog _log = LogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));

        public ServerListMediator()
            : base("ServerListMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var serverList = new ServerList();
            serverList.DataContext = this;
            ViewComponent = serverList;

            _plcHost = ConfigurationManager.AppSettings["PLCHost"];
        }

        private ServerList ServerList
        {
            get { return (ServerList)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                SendCommand.App_Server_Show,
                SendCommand.App_Tcp_Show,
                SendCommand.App_Client_Initialize
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case SendCommand.App_Tcp_Show:
                    ServerList.Show();
                    return;
                case SendCommand.App_Server_Show:
                    var newServer = new ServerModel()
                    {
                        StartClick = StartClick,
                        StopClick = StopClick,
                        SendClick = SendClick,
                        AddClick = AddClick,
                        RdGroup = ControlList.Count.ToString(),
                        Name = ControlList.Count.ToString()
                    };
                    this.ServerList.Dispatcher.Invoke(() => { ControlList.Add(newServer); });
                    return;
                case SendCommand.App_Client_Initialize:
                    var port = (int)notification.Body;
                    var server = new ServerModel()
                    {
                        IP = _plcHost,
                        Port = port,
                        StartClick = StartClick,
                        StopClick = StopClick,
                        SendClick = SendClick,
                        AddClick = AddClick,
                        RdGroup = ControlList.Count.ToString(),
                        Name = ControlList.Count.ToString()
                    };
                    this.ServerList.Dispatcher.Invoke(() => { ControlList.Add(server); });
                    return;
            }
        }

        private ICollection<ServerModel> _controlList = new ObservableCollection<ServerModel>();
        public ICollection<ServerModel> ControlList
        {
            get { return _controlList; }
            set
            {
                _controlList = value;
                RaisePropertyChanged("ControlList");
            }
        }

        public ICommand StartClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var model = (ServerModel)obj;
                    if (model.StartServer())
                    {
                        SendNotification(SendCommand.App_Client_Show, model.IP, model.Port.ToString());
                        _log.Info("Server " + model.Name + " started");
                    }
                });
            }
        }

        public ICommand StopClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var model = (ServerModel)obj;
                    model.StopServer();
                    _log.Info("Server " + model.Name + " stopped");
                    _log.Info("Client " + model.Name + " disconnected");
                });
            }
        }

        public ICommand SendClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var model = (ServerModel)obj;
                    model.SendData();
                    _log.Info("Server " + model.Name + " send data: " + model.Message);
                });
            }
        }

        public ICommand AddClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SendNotification(SendCommand.App_Server_Show);
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
