using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlideGeneratorLib;
using System.ServiceModel;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Net;
using System.IO;
using System.Net.Sockets;
using ContentServiceLibrary;
using System.Threading;

namespace ContentManagerService
{
    public class ContentAutoBroadcast
    {
        SlideGenerator slidegen;
        private   ServiceHost host;
        

        private Boolean enabled { get; set; }
        private DispatcherTimer timer { get; set; }
        public List<String> slides { get; set; }
        private String _slideOnair = "";
        public String slideOnair { get; set; }
        public object parent { get; set; }
        public event EventHandler onBroadcast;

        public ContentAutoBroadcast(SlideGenerator slidegen, ServiceHost host)
        {
            this.slides = new List<String>();
            this.slidegen = slidegen;
            this.host = host;
            timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(10);
            this.timer.Tick += new EventHandler(timer_Tick);
            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            
            this.timer.Interval = TimeSpan.FromSeconds(10);
            int i = this.slides.IndexOf(this.slideOnair);
            if (i + 1 == this.slides.Count) i = 0;            
            else i++;
            if(this.slides.Count>0)
                broadcast(this.slides.ElementAt(i));

        }

        public void setAutoBroadcast(bool isEnabled)
        {
            if (isEnabled)
            {
                this.timer.Start();
                broadcast(this.slides.First());
            }
            else
            {
                this.timer.Stop();
            }
        }

        public bool getAutoBroadcast()
        {

            return this.timer.IsEnabled;
        }

        public void setAutoBroadcastParameters(TimeSpan Interval, List<string> slidekeys)
        {

            MainWindow.errorAdd("NEW BROADCASTING CART: "+slidekeys.Count+" items");
            timer.Interval = Interval;
            this.slides = slidekeys;
        }

        public List<String> getAutoBroadCastSlides()
        {
            return this.slides;
        }

        public TimeSpan getAutoBroadcastInterval()
        {
            return timer.Interval;
        }

        public void broadcast(string slidekey)
        {
            timer.Stop();
            timer.Start();

            MainWindow.errorAdd(slidekey + " - broadcasting...");

            this.slideOnair = slidekey;



            Canvas slide = slidegen.loadXMLSlide(slidekey);
            String filename = DateTime.Now.ToFileTimeUtc()+"-d-" + slidekey + ".jpg";
            Console.WriteLine("FTP SEND " + filename);



            slidegen.saveToJpg(slide, slidegen.tmpfolder + filename);

            Thread newThread = new Thread(new ParameterizedThreadStart(this.sendToFtpThread));
            LinkedList<FtpParam> ftplist = new LinkedList<FtpParam>();

            Int32 ftpn = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["ftpCount"]);
            for (int i = 0; i < ftpn; i++)
            {
                String ftpserver = System.Configuration.ConfigurationSettings.AppSettings["ftpServer"+(i+1)];
                String ftpuser = System.Configuration.ConfigurationSettings.AppSettings["ftpUser" + (i + 1)];
                String ftppwd = System.Configuration.ConfigurationSettings.AppSettings["ftpPwd" + (i + 1)];
                

                ftplist.AddLast(new FtpParam(ftpserver+""+ filename, ftpuser, ftppwd, slidegen.tmpfolder + filename));
            }
            newThread.Start(ftplist);
            
            
            String server = System.Configuration.ConfigurationSettings.AppSettings["stompServer"];
            Int32 port = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["stompPort"]);
            String url = System.Configuration.ConfigurationSettings.AppSettings["stompHttpUrl"];


            Int32 stompn = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["stompTopicCount"]);
            for (int i = 0; i < stompn; i++)
            {
                
                String topic = System.Configuration.ConfigurationSettings.AppSettings["stompTopic" + (i+1)];
                Console.WriteLine("SEND STOMP : " + topic);

                Thread newThread2 = new Thread(new ParameterizedThreadStart(this.sendToStompThread));
                newThread2.Start(new StompParam(server, port, topic + "image", "SHOW " + url + "" + filename));

                Thread newThread3 = new Thread(new ParameterizedThreadStart(this.sendToStompThread));
                newThread3.Start(new StompParam(server, port, topic + "text", "SHOW " + url + "" + filename));
            }
            
            this.onBroadcast(this, new EventBroadcastArgs(slidekey, DateTime.Now.ToLongTimeString()));
        }
        class StompParam
        {
            public String address;
            public int port;
            public String topic;
            public String showparam;

            public StompParam(String address, int port, string topic, string showparam)
            {
                // TODO: Complete member initialization
                this.address = address;
                this.port = port;
                this.topic = topic;
                this.showparam = showparam;
            }
        }
        class FtpParam
        {
            public string p;
            public string p_2;
            public string p_3;
            public string p_4;

            public FtpParam(string p, string p_2, string p_3, string p_4)
            {
                // TODO: Complete member initialization
                this.p = p;
                this.p_2 = p_2;
                this.p_3 = p_3;
                this.p_4 = p_4;
            }

        }
        private void sendToStompThread(object s)
        {
            StompParam stomp = (StompParam)s;
            sendToStomp(stomp.address, stomp.port, stomp.topic, stomp.showparam);
        }
        private void sendToFtpThread(object s)
        {
            LinkedList<FtpParam> ftplist = (LinkedList<FtpParam>)s;
            for (int i = 0; i < ftplist.Count; i++)
            {
                FtpParam ftp = ftplist.ElementAt(i);
                sendToFtp(ftp.p, ftp.p_2, ftp.p_3, ftp.p_4);
            }
        }
        private void sendToStomp(string address, int port, string topic, String showparam)
        {
            NetworkStream output;
            StreamWriter writer;
            StreamReader reader;
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(address, port);

                output = client.GetStream();
                writer = new StreamWriter(output, System.Text.Encoding.UTF8);
                reader = new StreamReader(output);
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes("CONNECT\n\n" + (char)0);

                output.Write(buffer, 0, buffer.Length);
                output.Flush();
                Console.WriteLine("[STOMP] sent");

                Console.WriteLine("[STOMP] ANSWER: ");// + reader.ReadString());

                while (reader.Peek() != -1 && reader.Peek() != 0)
                {
                    Console.WriteLine(reader.ReadLine());
                }

                byte[] buffer2 = encoder.GetBytes("SEND\ndestination:" + topic + "\n\n" + showparam + "\n\n" + (char)0);

                output.Write(buffer2, 0, buffer2.Length);
                output.Flush();
                Console.WriteLine("[STOMP] sent");
                Console.WriteLine("[STOMP] ANSWER: ");// + reader.ReadString());

                while (reader.Peek() != -1 && reader.Peek() != 0)
                {
                    Console.WriteLine(reader.ReadLine());
                }

                writer.Close();
                reader.Close();
                output.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("[CONTENT SERVICE] stomp socket error");
            }
        }

        private void sendToFtp(String address, String user, String pwd, String filename){
            try
            {
                Uri uri = new Uri(address.Substring(0, address.Length-2));

                MainWindow.errorAdd("URI: " + uri.AbsoluteUri);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(user, pwd);

                // Copy the contents of the file to the request stream.
                FileStream stream = new FileStream(filename, FileMode.Open);
                BinaryReader reader = new BinaryReader(stream);
                //StreamReader sourceStream = new StreamReader(filename);
                byte[] fileContents = reader.ReadBytes((int)stream.Length);

                stream.Close();
                reader.Close();
                request.ContentLength = fileContents.Length;
                try
                {
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                }
                catch (WebException e)
                {
                    Console.WriteLine("error " + e.Message);
                    MainWindow.errorAdd("Error when sending picture on the ftp server: " + e.Message);
                }
                FtpWebResponse response = null;
                try
                {
                    response = (FtpWebResponse)request.GetResponse();
                    request = (FtpWebRequest)WebRequest.Create(uri);
                    request.Credentials = new NetworkCredential(user, pwd);
                    request.Method = WebRequestMethods.Ftp.Rename;

                   // MainWindow.errorAdd("filename:"+filename.Substring(filename.LastIndexOf(@"\")+1));
                    request.RenameTo = filename.Substring(filename.LastIndexOf(@"\")+1);
                    response = (FtpWebResponse)request.GetResponse();

                    
                }
                catch (WebException e)
                {
                    Console.WriteLine("error " + e.Message);
                    MainWindow.errorAdd("Error when sending picture on the ftp server: "+e.Message);
                }

                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

                response.Close();
            }
            catch(Exception e)
            {
                MainWindow.errorAdd("Error when sending picture on the ftp server"+e.Message);
            }
        }
    }
}
