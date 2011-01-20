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

namespace Playout.UI
{
    /// <summary>
    /// Interaction logic for AutoCloseWindow.xaml
    /// </summary>
    public partial class AutoCloseWindow : Window
    {
        

        public AutoCloseWindow(ImageSource imageSource)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.display.Source = imageSource;
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
