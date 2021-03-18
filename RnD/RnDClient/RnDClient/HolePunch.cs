using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace RnDClient
{
    class HolePunch
    {
        private static IPAddress serverGlobalIp;
        private static int serverPort;
        private UdpClient udpClient;

        public HolePunch(string _serverGlobalIp, int _serverPort)
        {
            serverGlobalIp = IPAddress.Parse(_serverGlobalIp);
            serverPort = _serverPort;
            udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            udpClient.Connect(serverGlobalIp, serverPort);

            Thread ListenThread = new Thread(ListenMessage);
            ListenThread.Start();
        }

        private void ListenMessage()
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] recivieBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    string msg = System.Text.Encoding.UTF8.GetString(recivieBytes, 0, recivieBytes.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        public void SendMessage(string msg)
        {
            byte[] b_msg = System.Text.Encoding.UTF8.GetBytes(msg);
            try
            {
                udpClient.Send(b_msg, b_msg.Length);                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
