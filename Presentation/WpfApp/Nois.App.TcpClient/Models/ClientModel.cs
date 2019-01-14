using log4net;
using Nois.WpfApp.Framework.View;
using SimpleTCP;
using System;
using System.Text;
using System.Windows.Input;

namespace Nois.App.TcpClient.Models
{
    public class ClientModel : ViewModel
    {
        private static readonly ILog _log = LogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));
        //Thread PingServer;
        CustomizeSimpleTcpClient client;
        Action<CustomizeSimpleTcpClient, bool> _client_StatusChange;
        Action<CustomizeSimpleTcpClient, Message> _client_DataReceived;

        public ClientModel(string ipAddress, int port, string name, Action<CustomizeSimpleTcpClient, Message> client_DataReceived, Action<CustomizeSimpleTcpClient, bool> statusChange)
        {
            client = new CustomizeSimpleTcpClient(0, name, ipAddress, port);
            client.StringEncoder = Encoding.UTF8;
            client.StatusFromOut = true;
            client.DataReceived += TcpClient_DataReceived;
            client.StatusChanged += TcpClient_StatusChanged;
            _client_DataReceived = client_DataReceived;
            _client_StatusChange = statusChange;
        }
        public string Name { get { return client != null ? client.Name : ""; } }
        public string IP { get { return client != null ? client.IpAddress : ""; } }
        public int Port { get { return client != null ? client.Port : 0; } }
        private string _currentStatus { get; set; }

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

        public ICommand ConnectClick { get; set; }

        public void Connect()
        {
            client.Start();
        }
        public void ReConnect()
        {
            client.Status = false;
        }
        public void Disconnect()
        {
            client.Stop();
        }
        public void Write(string message)
        {
            try
            {
                client.Write(message);
                _log.Info("Send to " + IP + ":" + Port + ": " + message);
            }
            catch (Exception ex)
            {
                _log.Info("Can't send to " + IP + ":" + Port + ". Error: " + ex.Message);
            }
        }

        private void TcpClient_DataReceived(object sender, Message e)
        {
            var customizeTcpClient = (CustomizeSimpleTcpClient)sender;
            _client_DataReceived(customizeTcpClient, e);
        }
        private void TcpClient_StatusChanged(object sender, bool s)
        {
            var customizeTcpClient = (CustomizeSimpleTcpClient)sender;
            _client_StatusChange(customizeTcpClient, s);
        }
    }
}
