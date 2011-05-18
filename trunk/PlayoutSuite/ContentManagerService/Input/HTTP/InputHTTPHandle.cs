using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using ContentManagerService.GUI;
using System.Windows;

namespace ContentManagerService.Input.HTTP
{
    public class InputHTTPHandle
    {
        InputHTTPAction action;

        Socket mySocket;
        String sHttpVersion = "";
        private List<string> availableSlides;

        public InputHTTPHandle(ref Socket mySocket, InputHTTPAction action, List<String> availableSlides)
        {
            this.mySocket = mySocket;
            this.action = action;
            this.availableSlides = availableSlides;
        }

        public void handleRequest()
        {
            Console.WriteLine("Client IP {0}\n", mySocket.RemoteEndPoint);

            Byte[] bReceive = new Byte[1024];
            int i = mySocket.Receive(bReceive, bReceive.Length, 0);

            string sBuffer = Encoding.ASCII.GetString(bReceive);

            if (sBuffer.Substring(0, 3) != "GET")
            {
                Console.WriteLine("Only Get Method is supported..");
                mySocket.Close();
                return;
            }


            InputHTTPResult res = parseRequest(sBuffer);
            if (res.command == InputHTTPResult.INPUTCOMMAND.NOTIMPLEMENTED)
            {
                String msg = "Not implemented";
                this.SendHeader(this.sHttpVersion, "text/plain", msg.Length, " 404 Not Found", ref mySocket);
                this.SendToBrowser(msg, ref mySocket);
            }
            else if (res.command == InputHTTPResult.INPUTCOMMAND.ERROR)
            {
                String msg = "Exception: Bad Request Synthax";
                this.SendHeader(this.sHttpVersion, "text/plain", msg.Length, " 404 Not Found", ref mySocket);
                this.SendToBrowser(msg, ref mySocket);
            }
            else
            {
                String msg = "OK";
                switch (res.command)
                {
                    case InputHTTPResult.INPUTCOMMAND.UPDATE:
                        action.update(res.parameters);
                        break;
                    case InputHTTPResult.INPUTCOMMAND.UPDATEANDBROADCAST:
                        if (res.parameters.ContainsKey("BSLIDE") && res.parameters["BSLIDE"] != "")
                        {
                            if (availableSlides.Contains(res.parameters["BSLIDE"]))
                                action.updateAndBroadcast(res.parameters, res.parameters["BSLIDE"]);
                            else
                                msg = "SLIDE NOT FOUND (" + res.parameters["BSLIDE"]+")";
                        }
                        else
                        {
                            action.update(res.parameters);
                        }
                        break;
                    case InputHTTPResult.INPUTCOMMAND.BROADCASTSLIDE:
                        if (res.parameters.ContainsKey("BSLIDE") && res.parameters["BSLIDE"] != "")
                        {
                            if (availableSlides.Contains(res.parameters["BSLIDE"]))
                                action.broadcast(res.parameters["BSLIDE"]);
                            else
                                msg = "SLIDE NOT FOUND (" + res.parameters["BSLIDE"]+")";
                        }
                        else
                        {
                            msg = "Broadcast BAD FORMAT: BSLIDE argument is missing.";
                        }
                        break;

                }
                
                this.SendHeader(this.sHttpVersion, "text/plain", msg.Length, " 200 Ok", ref mySocket);
                this.SendToBrowser(msg, ref mySocket);
            }

            mySocket.Close();
        }
        public void dispatchAction(){

        }

        public void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, ref Socket mySocket)
        {
            String sBuffer = "";
            if (sMIMEHeader.Length == 0)
            {
                sMIMEHeader = "text/plain"; // Default Mime Type text/plain
            }
            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: EBU-RadioProductionPlatform\r\n";
            sBuffer = sBuffer + "Content-Type: " + sMIMEHeader + "\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";
            Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);
            SendToBrowser(bSendData, ref mySocket);
        }

        public InputHTTPResult parseRequest(String sBuffer)
        {
            try
            {
                int iStartPos = sBuffer.IndexOf("HTTP", 1);
                this.sHttpVersion = sBuffer.Substring(iStartPos, sBuffer.IndexOf("\n", iStartPos));
                string sRequest = sBuffer.Substring(0, iStartPos - 1);

                sRequest = sRequest.Substring(sRequest.Substring(0, sRequest.IndexOf('?')).LastIndexOf("/")+1);

                String[] s = sRequest.Split(("?").ToArray(), 2, StringSplitOptions.None);
                
                InputHTTPResult.INPUTCOMMAND cmd = InputHTTPResult.INPUTCOMMAND.NOTIMPLEMENTED;
                switch (s[0].ToLower())
                {
                    case "update":
                        cmd = InputHTTPResult.INPUTCOMMAND.UPDATE;
                        break;
                    case "updateandbroadcast":
                        cmd = InputHTTPResult.INPUTCOMMAND.UPDATEANDBROADCAST;
                        break;
                    case "broadcast":
                        cmd = InputHTTPResult.INPUTCOMMAND.BROADCASTSLIDE;
                        break;
                    case "loadslidecart":
                        cmd = InputHTTPResult.INPUTCOMMAND.LOADSLIDECART;
                        break;
                    default:
                        cmd = InputHTTPResult.INPUTCOMMAND.NOTIMPLEMENTED;
                        break;
                }


                char separator = '|';
                String separatorParameter = System.Configuration.ConfigurationSettings.AppSettings["httpUrlSeparator"];
                if(separatorParameter != null){
                    if (separatorParameter.Length != 1)
                    {
                        MessageBox.Show("Configuration key httpUrlSeparator : Bad format");
                        Environment.Exit(1);
                    }
                    else
                        separator = separatorParameter.ToCharArray().First();
                }
                String[] s2 = s[1].Split(separator);


                Dictionary<String, String> parameters = new Dictionary<String, String>();
                for (int i = 0; i < s2.Count(); i++)
                {
                    String[] p = s2[i].Split(("=").ToArray(), 2);
                    if(p.Length == 2)
                        parameters.Add(p[0], System.Web.HttpUtility.UrlDecode(p[1]));
                    
                }
                return new InputHTTPResult(cmd, parameters);
            }
            catch
            {
                Console.WriteLine("Error when parsing");
                return new InputHTTPResult(InputHTTPResult.INPUTCOMMAND.ERROR, null);
            }

        }

        public void SendToBrowser(String sData, ref Socket mySocket)
        {
            SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }

        public void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            int numBytes = 0; try
            {
                if (mySocket.Connected) { if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1) Console.WriteLine("Socket Error cannot Send Packet"); else { } }
                else Console.WriteLine("Connection Dropped....");
            }
            catch (Exception e) { Console.WriteLine("Error Occurred : {0} ", e); }
        }

    }

    public class InputHTTPResult
    {
        public enum INPUTCOMMAND { UPDATE, UPDATEANDBROADCAST, BROADCASTSLIDE, LOADSLIDECART, ERROR, NOTIMPLEMENTED }
        public Dictionary<String, String> parameters = new Dictionary<String, String>();
        public INPUTCOMMAND command;

        public InputHTTPResult(INPUTCOMMAND command, Dictionary<String,String> parameters){

            this.command = command;
            this.parameters = parameters;
            
        }
    }

}
