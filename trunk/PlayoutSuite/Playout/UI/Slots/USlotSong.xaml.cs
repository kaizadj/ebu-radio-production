using System;
using System.Collections.Generic;
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
using LogListPrj;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Globalization;

namespace Playout.UI.Slots
{
	/// <summary>
	/// Interaction logic for USlot.xaml
	/// </summary>
    /// 

	public partial class USlotSong : UserControl, IDisposable, INotifyPropertyChanged
    {
        private SlotItem _slot=null;

        public DispatcherTimer t;
        private TimeSpan _currenttime =TimeSpan.Zero;
        public TimeSpan currenttime {
            get
            {
                return this._currenttime;
            }
            set
            {
                this._currenttime = value;
                OnPropertyChanged("displaytime");
            }
        }
        private double _percentageElapsed = 0.0;
        public double percentageElapsed
        {
            get
            {
                return _percentageElapsed;
            }
            set
            {
                this._percentageElapsed = value;
                OnPropertyChanged("percentageElapsed");
            }
        }
        private Style _displaytimestyle = new Style(typeof(TextBlock));
        public Style displaytimestyle
        {
            get
            {
                return this._displaytimestyle;
            }
            set {
                this._displaytimestyle = value;
                OnPropertyChanged("displaytimestyle");
            }

        }
        private Style _displaybgstyle = new Style(typeof(Rectangle));
        public Style displaybgstyle
        {
            get
            {
                return this._displaybgstyle;
            }
            set
            {
                this._displaybgstyle = value;
                OnPropertyChanged("displaybgstyle");
            }

        }
        public String displaytime
        {
            get
            {
                return DataConverter.timespantostr(this.currenttime);
            }
            private set { }
        }

		public USlotSong(SlotItem slot)
		{
            t = new DispatcherTimer();
            t.Interval = TimeSpan.FromMilliseconds(100);
            t.Tick += new EventHandler(t_Tick);
            this._slot = slot;

            _currenttime = slot.item.runtime;

            InitializeComponent();


            Binding myBinding = new Binding("item.label");
            myBinding.Source = slot;
            ULABEL.SetBinding(TextBlock.TextProperty, myBinding);



            Binding myBinding2 = new Binding("displaytime");
            myBinding2.Source = this;
            UTIME.SetBinding(TextBlock.TextProperty, myBinding2);


            Binding myStyleBinding = new Binding("displaytimestyle");
            myStyleBinding.Source = this;
            UTIME.SetBinding(TextBlock.StyleProperty, myStyleBinding);



            Binding myStyle2Binding = new Binding("displaybgstyle");
            myStyle2Binding.Source = this;
            UEMBOSS.SetBinding(Rectangle.StyleProperty, myStyle2Binding);

          /*  Binding progB = new Binding("percentageElapsed");
            progB.Source = this;
            UPROGRESS_Copy.RenderTransform.SetBinding(UserControl.RenderTransformProperty, progB);
            */

            Binding styleP = new Binding("status");
            styleP.Source = slot;
            styleP.Converter = new StatusStyleConverter();
            styleP.ConverterParameter = this;
            USYMBOL.SetBinding(UserControl.StyleProperty, styleP);

            t.Start();
           /* SBprog = (Storyboard)this.FindResource("SBPROGRESS");
            SBprog.SpeedRatio = 10.0 / slot.timemarker.duration.TotalSeconds;*/
            displaytimestyle = (Style)this.FindResource("BannerInlayText");
            displaybgstyle = (Style)this.FindResource("BannerBar");
		}

        
        void t_Tick(object sender, EventArgs e)
        {

            if (this._slot.status == DS.SlotStatus.ONAIR || this._slot.status == DS.SlotStatus.CLOSING)
            {
            if (this.currenttime > TimeSpan.Zero)
            {
                this.currenttime = this.currenttime.Subtract(TimeSpan.FromMilliseconds(100));
                this._percentageElapsed = this.currenttime.TotalMilliseconds / this._slot.timemarker.duration.TotalMilliseconds;
            }
            else
            {
                this.currenttime = TimeSpan.Zero;
                //SBprog.Stop();
            }

            if (this._slot.status == DS.SlotStatus.CLOSING)
            {
                displaytimestyle = (Style)this.FindResource("BannerInlayTextClosing");

                displaybgstyle = (Style)this.FindResource("BannerBarClosing");
            }
            else
                displaybgstyle = (Style)this.FindResource("BannerBarPlaying");

            }
        }


        public void Dispose()
        {
            t.Stop();
            t = null; 
            _slot = null;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [ValueConversion(typeof(DS.SlotStatus), typeof(Style))]
    public class StatusStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((DS.SlotStatus)value == DS.SlotStatus.ONAIR)
                return (Style)((USlotSong)parameter).FindResource("BannerInlayPlay");
            else if ((DS.SlotStatus)value == DS.SlotStatus.READY)
                return (Style)((USlotSong)parameter).FindResource("BannerInlayReady");
            else
                return (Style)((USlotSong)parameter).FindResource("BannerInlayWait");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}