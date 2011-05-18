using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ContentManagerService.GUI;
using System.Net;
using System.IO;
using System.Windows;

namespace ContentManagerService.Output.Ftp
{
    class OutputFtp
    {
        public delegate void OutputEvent(String filename, String link);
        public event OutputEvent onUploadEnd;

        Dictionary<int, DateTime> lastSend = new Dictionary<int, DateTime>();

        public void send(String localFilename, String tmpPath, string link)
        {


            Thread newThread = new Thread(new ParameterizedThreadStart(this.ThreadProcess));
            LinkedList<FtpParam> ftplist = new LinkedList<FtpParam>();

            Int32 ftpn = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["ftpCount"]);
            for (int i = 0; i < ftpn; i++)
            {
                String ftpserver = System.Configuration.ConfigurationSettings.AppSettings["ftpServer" + (i + 1)];
                String ftpuser = System.Configuration.ConfigurationSettings.AppSettings["ftpUser" + (i + 1)];
                String ftppwd = System.Configuration.ConfigurationSettings.AppSettings["ftpPwd" + (i + 1)];
                String ftpalone = System.Configuration.ConfigurationSettings.AppSettings["ftpAlone" + (i + 1)];
                String ftpminperiod = System.Configuration.ConfigurationSettings.AppSettings["ftpMinPeriod" + (i + 1)];


                ftplist.AddLast(new FtpParam(ftpserver + "" + localFilename, ftpuser, ftppwd, tmpPath + localFilename, link, ftpalone, ftpminperiod, i));
                Console.WriteLine("NEW FTP");
            }
            newThread.Start(ftplist);
        }

        private void sendToFtp(String address, String user, String pwd, String filename, String alone, String link)
        {
            try
            {
                if(alone=="yes")
                {
                    
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.EnableRaisingEvents=false;
                    proc.StartInfo.FileName="C:\\EBU\\exiftool.exe";
                    proc.StartInfo.Arguments="-all= -comment=\""+link+"\" "+filename;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    proc.WaitForExit();
                }


                Uri uri = new Uri(address.Substring(0, address.Length - 2));

                UIMain.errorAdd("URI: " + uri.AbsoluteUri);
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
                    UIMain.errorAdd("Error when sending picture on the ftp server: " + e.Message);
                }
                FtpWebResponse response = null;
                try
                {
                    response = (FtpWebResponse)request.GetResponse();
                    request = (FtpWebRequest)WebRequest.Create(uri);
                    request.Credentials = new NetworkCredential(user, pwd);
                    request.Method = WebRequestMethods.Ftp.Rename;

                    // MainWindow.errorAdd("filename:"+filename.Substring(filename.LastIndexOf(@"\")+1));
                    request.RenameTo = filename.Substring(filename.LastIndexOf(@"\") + 1);
                    if (alone == "yes") request.RenameTo = "ONAIR.jpg";
                    response = (FtpWebResponse)request.GetResponse();


                }
                catch (WebException e)
                {
                    Console.WriteLine("error " + e.Message);
                    UIMain.errorAdd("Error when sending picture on the ftp server: " + e.Message);
                }

                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

                response.Close();
                

                if (alone != "yes")
                    this.onUploadEnd(filename.Substring(filename.LastIndexOf(@"\") + 1), link);
                

            }
            catch (Exception e)
            {
                UIMain.errorAdd("Error when sending picture on the ftp server" + e.Message);
            }
        }

        private void ThreadProcess(object s)
        {
            LinkedList<FtpParam> ftplist = (LinkedList<FtpParam>)s;
            for (int i = 0; i < ftplist.Count; i++)
            {
                FtpParam ftp = ftplist.ElementAt(i);
                if (!this.lastSend.ContainsKey(ftp.id))
                {
                    this.lastSend.Add(ftp.id, DateTime.Now);
                    sendToFtp(ftp.address, ftp.user, ftp.password, ftp.filename, ftp.alone, ftp.link);
                }
                else
                {
                    if (this.lastSend[ftp.id].AddSeconds(ftp.minperiod) < DateTime.Now)
                    {
                        sendToFtp(ftp.address, ftp.user, ftp.password, ftp.filename, ftp.alone, ftp.link);
                        this.lastSend[ftp.id] = DateTime.Now;
                    }
                }
                Console.WriteLine("NEW FTP");
                
            }
        }


    }
}
