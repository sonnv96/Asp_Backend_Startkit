using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Nois.App.Message.Views
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        public bool IsShow;
        public MessageWindow()
        {
            InitializeComponent();
        }
        public void StartCloseTimer()
        {
            //timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
            timer.Tick -= TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            Hide();
            IsShow = false;
        }
    }
}
