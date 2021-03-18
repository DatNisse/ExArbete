using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RnDServer
{
    class HolePunch
    {
        private List<IPEndPoint> clients;
        UdpClient recivingUdpClient;
        public HolePunch()
        {
            Console.WriteLine("Begin UDP listner");
            clients = new List<IPEndPoint>();
            recivingUdpClient = new UdpClient(11000);
            Thread listenThread = new Thread(ListenMessage);
            listenThread.Start();
            Console.WriteLine("UDP listner running");
        }

        private void ListenMessage()
        {
            Console.WriteLine("Begin listen");
            while (true)
            {
                try
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] recivingBytes = recivingUdpClient.Receive(ref RemoteIpEndPoint);
                    Console.WriteLine("got something!");
                    bool found = false;
                    foreach (IPEndPoint client in clients)
                    {
                        if (!client.Port.Equals(RemoteIpEndPoint.Port) && !client.Address.Equals(RemoteIpEndPoint.Address))
                        {
                            SendMessage(client, recivingBytes);
                        }
                        else
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {                        
                        clients.Add(RemoteIpEndPoint);
                        SendMessage(RemoteIpEndPoint, System.Text.Encoding.UTF8.GetBytes("Greetings!"));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        void SendMessage(IPEndPoint targetEp, byte[] b_msg)
        {
            recivingUdpClient.Connect(targetEp);
            try
            {
                recivingUdpClient.Send(b_msg, b_msg.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            recivingUdpClient.Dispose();
            recivingUdpClient = new UdpClient(11000);
        }

    }
}
