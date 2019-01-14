using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.Logger;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Nois.App.LogByObject.Views
{
    public class MainMediator : ViewModelMediator
    {
        private static readonly INoisLogger _log = NoisLogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));
        public MainMediator()
            : base("MainMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var log = new Main();
            log.DataContext = this;
            ViewComponent = log;

            Initialize();
        }

        private Main Main
        {
            get { return (Main)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                SendCommand.App_Log_Show,
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case SendCommand.App_Log_Show:
                    Main.Show();
                    return;
            }
        }

        private ICollection<LogModel> _logs;
        public ICollection<LogModel> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                RaisePropertyChanged("Logs");
            }
        }

        public ICommand OkCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        var file = new FileInfo(@"./logsetting.txt");
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        using (StreamWriter writetext = file.CreateText())
                        {
                            foreach (var log in Logs)
                            {
                                writetext.WriteLine(log.Id + "," + log.Name + "," + log.IsChecked);
                            }
                        }
                        Main.Hide();
                        _log.LoadFile();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
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

        private void Initialize()
        {
            try
            {
                Logs = new ObservableCollection<LogModel>();
                var file = new FileInfo(@"./logsetting.txt");
                if (file.Exists)
                {
                    //read data from file
                    var lines = File.ReadAllLines(@"./logsetting.txt");

                    foreach (string line in lines)
                    {
                        var data = line.Split(',');
                        var log = new LogModel
                        {
                            Id = int.Parse(data[0]),
                            Name = data[1],
                            IsChecked = bool.Parse(data[2])
                        };
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            Logs.Add(log);
                        });
                    }
                }
                else
                {
                    Logs = new ObservableCollection<LogModel>()
                    {
                        new LogModel{Id = 1, Name = "Operation", IsChecked = true},
                        new LogModel{Id = 2, Name = "Sender", IsChecked = true},
                        new LogModel{Id = 3, Name = "Receiver", IsChecked = true},
                        new LogModel{Id = 4, Name = "Update", IsChecked = true},
                        new LogModel{Id = 5, Name = "Status", IsChecked = true},
                        new LogModel{Id = 6, Name = "Heartbeat", IsChecked = true},
                    };

                    using (StreamWriter writetext = file.CreateText())
                    {
                        foreach (var log in Logs)
                        {
                            writetext.WriteLine(log.Id + "," + log.Name + "," + log.IsChecked);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
