using log4net;
using Nois.Helper;
using SimpleTCP;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Nois.App.TcpClient
{
    public class CustomizeSimpleTcpClient : SimpleTcpClient
    {
        private static readonly ILog _log = LogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));
        private Thread _checkStatus;
        public bool StatusFromOut { get; set; }
        public CustomizeSimpleTcpClient(int id, string name, string ipAddress, int port)
        {
            Id = id;
            Name = name;
            IpAddress = ipAddress;
            Port = port;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        private bool _isStart;
        public void Start()
        {
            _isStart = true;
            Connect();
            _checkStatus = new Thread(() => {
                while (_isStart)
                {
                    if(!StatusFromOut)
                        Status = CheckStatus();
                    if (!Status)
                        Connect();
                    Thread.Sleep(5000);
                }
            });
            _checkStatus.IsBackground = true;
            _checkStatus.Start();
        }
        public void Stop()
        {
            _isStart = false;
            if (TcpClient != null && TcpClient.Connected)
                Disconnect();
            Status = false;
        }
        public event EventHandler<bool> StatusChanged;
        private bool _status;
        public bool Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                    StatusChanged?.Invoke(this, value);
                _status = value;
            }
        }
        private void Connect()
        {
            try
            {
                if (TcpClient != null && TcpClient.Connected)
                    Disconnect();
                Connect(IpAddress, Port);
                Status = true;
                _log.Info($"{ObjectInfo()} Connected");
            }
            catch (Exception ex)
            {
                _log.Info($"{ObjectInfo()} Connect method exeption: {ex.FullMessage()}");
                Status = false;
            }
        }
        private bool CheckStatus()
        {
            try
            {
                if (TcpClient != null && TcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    var buff = new byte[1];
                    if (TcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                        return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"{ObjectInfo()} CheckStatus method exeption: {ex.FullMessage()}");
                return false;
            }
            return TcpClient != null && TcpClient.Connected;
        }
        private string ObjectInfo()
        {
            return $"{IpAddress}:{Port}";
        }
    }
}
