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

namespace DBMgrRibbon.UIControls
{
    /// <summary>
    /// Interaction logic for UIClockInput.xaml
    /// </summary>
    public partial class UIClockInput : UserControl
    {
        public Boolean relativ { get; set; }
        public String Value
        {
            get
            {
                return formatTime((int)this.sbH.Value, (int)this.sbM.Value, (int)this.sbS.Value);
            }

            set
            {
                String[] v = value.Split(':');
                this.sbH.Value = Int32.Parse(v[0]);
                this.sbM.Value = Int32.Parse(v[1]);
                this.sbS.Value = Int32.Parse(v[2]);
            }

        }
        public String formatTime(int hours, int minutes, int seconds)
        {
            String h = (hours < 10) ? "0" + hours : hours.ToString();
            String m = (minutes < 10) ? "0" + minutes : minutes.ToString();
            String s = (seconds < 10) ? "0" + seconds : seconds.ToString();
            return h + ":" + m + ":" + s;
        }

        public UIClockInput()
        {
            InitializeComponent();
        }

        private void inSeconds_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
