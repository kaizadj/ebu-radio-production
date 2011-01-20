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
using System.Windows.Shapes;

namespace DBMgrRibbon
{
    /// <summary>
    /// Interaction logic for UIAudio.xaml
    /// </summary>
    public partial class UIAudio : Window
    {
        public UIAudio()
        {
            InitializeComponent();
        }

        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            this.audioplayer.stopAudio();

            
            
        }


    }
}
