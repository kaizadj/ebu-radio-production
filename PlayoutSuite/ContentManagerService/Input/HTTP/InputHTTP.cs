using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using ContentManagerService.GUI;
using System.Net;

namespace ContentManagerService.Input.HTTP
{
    class InputHTTP : IInputPlugin
    {
        private TcpListener myListener;
        private int port = 80;
        private InputHTTPAction action;
        private ContentManagerCore core;

        public InputHTTP(ContentManagerCore core)
        {
            this.core = core;
            this.setup(null);
            this.start();
        }
        public void setup(Dictionary<string, string> variableList)
        {
            action = new InputHTTPAction(core);
            try
            {
                if (System.Configuration.ConfigurationSettings.AppSettings.AllKeys.Contains("httpPort"))
                    port = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["httpPort"]);
                myListener = new TcpListener(port);
            }
            catch (Exception e)
            {
                UIMain.fatalError("[InputHTTP] Unable to open port : "+port+"\n"+e.Message);
            }

        }

        public bool start()
        {
            try{
                myListener.Start();
                Thread th = new Thread(new ThreadStart(StartListen));
                th.Start();
                return true;
            }
            catch (Exception e)
            {
                UIMain.fatalError("[InputHTTP] Unable to open port : "+port+"\n"+e.Message);
                return false;
            }
        }

        public bool stop()
        {
            throw new NotImplementedException();
        }

        public string getPluginType()
        {
            return "HTTP";
        }

        public void StartListen()
        {
            while (true)
            {

                Socket mySocket = myListener.AcceptSocket();
                Console.WriteLine("Socket Type " + mySocket.SocketType);
                if (mySocket.Connected)
                {
                    InputHTTPHandle httpHandle = new InputHTTPHandle(ref mySocket, action,core.slidegen.getAvailableSlides());
                    Thread t = new Thread(new ThreadStart(httpHandle.handleRequest));
                    t.Start();
                }
                
            }
            
        }


        
    }

    
}
