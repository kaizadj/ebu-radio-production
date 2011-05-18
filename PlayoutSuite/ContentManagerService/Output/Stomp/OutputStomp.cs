using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apache.NMS;
using Apache.NMS.Util;
using ContentManagerService.GUI;
using SlideGeneratorLib;

namespace ContentManagerService.Output.Stomp
{
    public class OutputStomp
    {

        public delegate void OutputEvent(String filename);
        public event OutputEvent onUploadEnd;
        private SlideGeneratorLib.SlideGenerator slidegen;

        public OutputStomp(SlideGeneratorLib.SlideGenerator slidegen)
        {
            this.slidegen = slidegen;
        }


        internal void sendToStompProcess(String filename, String link){
            
            String radiovistxt = "";
            String radiovislink = link;
            try
            {
                radiovistxt = slidegen.cstlist["RADIOVISTXT"];
            }
            catch { }

            if (radiovislink == "")
            {
                radiovislink=System.Configuration.ConfigurationSettings.AppSettings["radiovislink"];
            }
            if(radiovistxt =="")
                radiovistxt = System.Configuration.ConfigurationSettings.AppSettings["radiovistxt"];
            String server = System.Configuration.ConfigurationSettings.AppSettings["stompServer"];
            Int32 port = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["stompPort"]);
            String url = System.Configuration.ConfigurationSettings.AppSettings["stompHttpUrl"];

            
            
            
            Int32 stompn = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["stompTopicCount"]);
            for (int i = 0; i < stompn; i++)
            {

                String topic = System.Configuration.ConfigurationSettings.AppSettings["stompTopic" + (i + 1)];
                Console.WriteLine("SEND STOMP : " + topic);

                Thread newThread2 = new Thread(new ParameterizedThreadStart(this.sendToStompThread));
                newThread2.Start(new StompParam(server, port, topic + "image", radiovislink, "SHOW " + url + "" + filename));

                Thread newThread3 = new Thread(new ParameterizedThreadStart(this.sendToStompThread));
                newThread3.Start(new StompParam(server, port, topic + "text", radiovislink, "TEXT " + radiovistxt + ""));
            }
        }

        private void sendToStompThread(object s)
        {
            StompParam stomp = (StompParam)s;
            sendToStomp(stomp.address, stomp.port, stomp.topic, stomp.link, stomp.showparam);
        }

        private void sendToStomp(string address, int port, string topic, String link, String showparam)
        {
            try
            {

                Uri connecturi = new Uri("stomp:tcp://" + address + ":" + port);

                // UIMain.errorAdd("[STOMP]  About to connect to " + connecturi);

                // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.

                IConnectionFactory factory = new NMSConnectionFactory(connecturi);

                using (IConnection connection = factory.CreateConnection(System.Configuration.ConfigurationSettings.AppSettings["stompLogin"], System.Configuration.ConfigurationSettings.AppSettings["stompPasscode"]))
                using (ISession session = connection.CreateSession())
                {

                    IDestination destination = SessionUtil.GetDestination(session, topic.Replace("/topic/", ""), DestinationType.Topic);

                    // UIMain.errorAdd("[STOMP] Using destination: " + destination);
                    Console.WriteLine("[STOMP] " + destination + " " + showparam);
                    // Create a consumer and producer 
                    using (IMessageConsumer consumer = session.CreateConsumer(destination))
                    using (IMessageProducer producer = session.CreateProducer(destination))
                    {
                        // Start the connection so that messages will be processed.
                        connection.Start();

                        producer.RequestTimeout = receiveTimeout;
                        consumer.Listener += new MessageListener(OnMessage);

                        // Send a message
                        ITextMessage request = session.CreateTextMessage(showparam);

                        //request.NMSCorrelationID = topic;
                        //request.Properties["NMSXGroupID"] = "EBURADIO";
                        //request.Properties["myHeader"] = "Cheddar";
                        if(link!="")
                        request.Properties["link"] = link;
                        request.Properties["trigger-time"] = "now";

                        producer.Send(request);

                        // Wait for the message
                        semaphore.WaitOne((int)receiveTimeout.TotalMilliseconds, true);
                        if (message == null)
                        {
                              UIMain.errorAdd("[STOMP] No message received!");
                        }
                        else
                        {
                              UIMain.errorAdd("[STOMP] Received message with ID:   " + message.NMSMessageId);
                              UIMain.errorAdd("[STOMP] Received message with text: " + message.Text);
                        }
                        producer.Close();
                        consumer.Close();
                    }
                    session.Close();
                    connection.Close();
                }
                

            }
            catch (Exception e)
            {
                //UIMain.errorAdd("[STOMP] catch"); 
                Console.WriteLine("[STOMP] catch : " + e.Message);
            }
        }

        private static AutoResetEvent semaphore = new AutoResetEvent(false);
        private static ITextMessage message = null;
        private static TimeSpan receiveTimeout = TimeSpan.FromSeconds(10);
        protected static void OnMessage(IMessage receivedMsg)
        {
            message = receivedMsg as ITextMessage;
            semaphore.Set();

        }
    }
}
