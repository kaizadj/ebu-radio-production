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
using SlideGeneratorLib;
using System.Threading;
using ContentServiceLibrary;
using System.ServiceModel;
using System.Windows.Threading;

namespace ContentManagerService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SlideGenerator slidegen;
        public static ServiceHost host;
        public static String url = "net.tcp://localhost:8095/ContentService";
        public static ContentAutoBroadcast autobroadcast;
        private static MainWindow Instance = null;
        public MainWindow()
        {
            
            Console.WriteLine("Opening ContentEngineService...");
            Type serviceType = typeof(IContentService);

            //ServiceHost duplex = new ServiceHost(typeof(AudioService), new Uri[] { new Uri("net.tcp://localhost:8090/"), new Uri("http://localhost:8080/") });
            //duplex = new ServiceHost(typeof(AudioService));
            host = new ServiceHost(typeof(ContentService));
            host.Opened += new EventHandler(host_Opened);
            host.Faulted += new EventHandler(host_Faulted);

            host.AddServiceEndpoint(typeof(IContentService), new NetTcpBinding(), url);

            host.Open();

            slidegen = new SlideGenerator(System.Configuration.ConfigurationSettings.AppSettings["DataFolder"] + "\\img\\conf\\conf.xml");


            autobroadcast = new ContentAutoBroadcast(slidegen, host);
            
            List<String> slides = slidegen.getAvailableSlides();
            InitializeComponent();
            Instance = this;

            autobroadcast.onBroadcast += new EventHandler(autobroadcast_onBroadcast);
        }

        void autobroadcast_onBroadcast(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
               {
                   errorlist.Items.Add("onair : " + autobroadcast.slideOnair);
                    Canvas s = slidegen.loadXMLSlide(autobroadcast.slideOnair);
                    if (s != null)
                        panel2.Children.Add(s);

                    if (panel2.Children.Count > 1)
                        panel2.Children.RemoveAt(0);
               }));
        }
        public static void errorAdd(String msg, String module = "undefined")
        {
            if (Instance != null)
            {
                errorListAdd(msg, module);
            }
        }
        private static void errorListAdd(String msg, String module = "undefined")
        {
            if (Instance != null)
            {
                Instance.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    Instance.errorlist.Items.Add("[" + module + "] " + DateTime.Now.ToLongTimeString() + ": " + msg);
                }));
            }
        }
        void host_Faulted(object sender, EventArgs e)
        {
            Console.WriteLine("Connection error");
        }

        void host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("connection ok!");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            Canvas s = slidegen.loadXMLSlide(textBox1.Text);
            if (s != null)
                panel.Children.Add(s);

            if (panel.Children.Count > 1)
                panel.Children.RemoveAt(0);
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String str = "";
            str += "Variables :\n";
            for (int i = 0; i < slidegen.cstlist.Count; i++)
            {
                str += slidegen.cstlist.ElementAt(i).Key + "=" +slidegen.cstlist.ElementAt(i).Value;
                str += "\n";
            }
            MessageBox.Show(str);
        }

    }
}
