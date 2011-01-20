using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace WpfCap
{
    public class CapPlayer : Image,IDisposable
    {
        public CapPlayer()
        {
            try
            {
                initBitmap();
                Application.Current.Exit += new ExitEventHandler(Current_Exit);
            }
            catch
            {
                MessageBox.Show("No available webcam");
                _device = null;
            }            
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            this.Dispose();
        }
        void initBitmap()
        {
            
            if (_device == null)
            {
                _device = new CapDevice();
                _device.OnNewBitmapReady += new EventHandler(_device_OnNewBitmapReady);
            }
            else
            {
                _device.Start();
            }
        }

        void _device_OnNewBitmapReady(object sender, EventArgs e)
        {
            Binding b = new Binding();
            b.Source = _device;
            b.Path = new PropertyPath(CapDevice.FramerateProperty);
            this.SetBinding(CapPlayer.FramerateProperty, b);

            this.Source = _device.BitmapSource;
        }

        
        CapDevice _device;


        public float Framerate
        {
            get { return (float)GetValue(FramerateProperty); }
            set { SetValue(FramerateProperty, value); }
        }
        public static readonly DependencyProperty FramerateProperty =
            DependencyProperty.Register("Framerate", typeof(float), typeof(CapPlayer), new UIPropertyMetadata(default(float)));



        /// <summary>
        /// Gets the current bitmap
        /// </summary>
        public BitmapSource CurrentBitmap
        {
            get
            {
                // Return right value
                return (_device != null && _device.BitmapSource != null) ? new TransformedBitmap(_device.BitmapSource.Clone(), new RotateTransform(0)):null;// /*new RotateTransform(Rotation)*/) : null;
            }
        }




        #region IDisposable Members

        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();
        }

        #endregion
    }
}
