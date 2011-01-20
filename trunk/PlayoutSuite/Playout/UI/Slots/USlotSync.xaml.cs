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
    /// Interaction logic for USlotSync.xaml
    /// </summary>
    public partial class USlotSync : UserControl, IDisposable
    {
        private DS.SyncType syncType;
        private DateTime syncTime;

        DispatcherTimer timer;
        EventHandler evtTick;
        private long logid;
        /*public USlotSync():base(DS.SyncType.HARD, DateTime.Now)
        {
        }
        */
        public USlotSync(DS.SyncType syncType, DateTime dateTime, long logid)
        {
            // TODO: Complete member initialization
            this.syncType = syncType;
            this.syncTime = dateTime;

            timer = new DispatcherTimer();
            this.logid = logid;

            InitializeComponent();
            this.UTIME.Text = syncTime.ToLongDateString() + " " + syncTime.ToLongTimeString();
            this.USYNCTEXT.Text = syncType.ToString();


            
            
            timer.Interval = TimeSpan.FromSeconds(10);

            evtTick = new EventHandler(timer_Tick);
            timer.Tick += evtTick;
        }

        private void ThisSync_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void StartHideSequence()
        {

           /* this.timer.Interval = TimeSpan.FromMilliseconds(100);
            if (!this.timer.IsEnabled)
                this.timer.Start();*/
            this.Dispose();
            ((PlayOutFrame)((Grid)((ScrollViewer)((StackPanel)((USlot)this.Parent).Parent).Parent).Parent).Parent).removeUSlot(logid);
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Tick -= evtTick;
            timer = null;
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
                    ((PlayOutFrame)((Grid)((ScrollViewer)((StackPanel)((USlot)this.Parent).Parent).Parent).Parent).Parent).removeUSlot(logid);
                }
            }
        }
    }
}
