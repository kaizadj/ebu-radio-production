using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Playout.UI.Slots
{
    /// <summary>
    /// Interaction logic for USlotError.xaml
    /// </summary>
    public partial class USlotError : UserControl, IDisposable
    {
        DispatcherTimer timer;
        EventHandler evtTick;
        private long logid;
        public USlotError(long logid, String msg = "error")
        {
            this.logid = logid;
            timer =  new DispatcherTimer();
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(10);

            evtTick = new EventHandler(timer_Tick);
            timer.Tick += evtTick;
            
            errormsg.Text = msg;

        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (timer.Interval == TimeSpan.FromSeconds(10))
            {
                timer.Interval = TimeSpan.FromMilliseconds(100);
            }
            else
            {
                if (Opacity >= 0.0)
                    this.Opacity = this.Opacity - 0.1;
                else
                {
                    this.timer.Stop();
                    this.Dispose();
                    ((PlayOutFrame)((Grid)((ScrollViewer)((StackPanel)this.Parent).Parent).Parent).Parent).removeUSlot(logid);
               }
            }
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Tick -= evtTick;
            timer = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
        }

        public void Hide()
        {
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            if(!this.timer.IsEnabled)
                this.timer.Start();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
                timer.Start();
        }
    }
}
