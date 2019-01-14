using log4net;
using Nois.WpfApp.Framework.View;
using SimpleTCP;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Nois.App.TcpServer.Models
{
    public class ServerModel : ViewModel
    {
        private static readonly ILog _log = LogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));

        SimpleTcpServer server;
        Thread AutoSend;

        public ServerModel()
        {
            try
            {
                server = new SimpleTcpServer();
                server.Delimiter = 0x13;
                server.StringEncoder = Encoding.UTF8;
                StartButton = true;
                AutoSend = new Thread(AutoSendData);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }

        public string Name { get; set; }

        private string _ip;
        public string IP
        {
            get { return _ip; }
            set
            {
                _ip = value;
                RaisePropertyChanged("IP");
            }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                RaisePropertyChanged("Port");
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

        private bool _startButton;
        public bool StartButton
        {
            get { return _startButton; }
            set
            {
                _startButton = value;
                RaisePropertyChanged("StartButton");
            }
        }

        private bool _isAuto;
        public bool IsAuto
        {
            get { return _isAuto; }
            set
            {
                _isAuto = value;
                RaisePropertyChanged("IsAuto");
            }
        }

        private string _timeAuto;
        public string TimeAuto
        {
            get { return _timeAuto; }
            set
            {
                _timeAuto = value;
                RaisePropertyChanged("TimeAuto");
            }
        }

        private string _rdGroup;
        public string RdGroup
        {
            get { return _rdGroup; }
            set
            {
                _rdGroup = value;
                RaisePropertyChanged("RdGroup");
            }
        }

        public ICommand StartClick { get; set; }

        public ICommand StopClick { get; set; }

        public ICommand SendClick { get; set; }

        public ICommand AddClick { get; set; }

        public void AutoSendData()
        {
            while (true)
            {
                if (IsAuto)
                {
                    try
                    {
                        server.Broadcast(Message);
                        _log.Info("Server " + Name + " send data");

                        Thread.Sleep(int.Parse(TimeAuto));
                    }
                    catch (Exception e)
                    {
                        Message = e.Message;
                        AutoSend.Abort();
                    }
                }
                else
                {
                    AutoSend.Abort();
                }
            }
        }

        public bool StartServer()
        {
            try
            {
                server.Start(IPAddress.Parse(IP), Port);
                StartButton = false;
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public void StopServer()
        {
            if (AutoSend.IsAlive)
            {
                AutoSend.Abort();
            }
            try
            {
                server.Stop();
                StartButton = true;
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }

        public void SendData()
        {
            if (!AutoSend.IsAlive && IsAuto)
            {
                AutoSend = new Thread(AutoSendData);
                AutoSend.Start();
                return;
            }
            var generatedString = string.Empty;
            server.Broadcast(Message);
        }
    }
}
