using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.View;
using System.Collections.Generic;

namespace Nois.App.Message.Views
{
    public class MessageWindowMediator : ViewModelMediator
    {
        public MessageWindowMediator()
            : base("MessageWindowMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var message = new MessageWindow();
            message.DataContext = this;
            ViewComponent = message;
        }

        private MessageWindow MessageWindow
        {
            get { return (MessageWindow)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                SendCommand.App_MessageWindow_ShowMessage
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case SendCommand.App_MessageWindow_ShowMessage:
                    var type = (string)notification.Type;
                    Text = (string)notification.Body;
                    switch (type)
                    {
                        case "info":
                            Caption = "Information";
                            Background = "#418bca";
                            break;
                        case "error":
                            Caption = "Error";
                            Background = "#d9544f";
                            break;
                    }
                    if (!MessageWindow.IsShow)
                    {
                        MessageWindow.Show();
                        MessageWindow.IsShow = true;
                        MessageWindow.StartCloseTimer();
                        MessageWindow.Topmost = true;
                    }
                    return;
            }
        }

        private string _background;
        public string Background
        {
            get { return _background; }
            set
            {
                _background = value;
                RaisePropertyChanged("Background");
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged("Text");
            }
        }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                RaisePropertyChanged("Caption");
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
