using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;

namespace Server
{
    class Game
    {
        public bool isGameover;
        readonly IPEndPoint player1;
        readonly IPEndPoint player2;
        readonly string player1_id;
        readonly string player2_id;

        Game(IPEndPoint p1, string p1_id, IPEndPoint p2, string p2_id)
        {
            player1 = p1;
            player2 = p2;
            player1_id = p1_id;
            player2_id = p2_id;
            isGameover = true;
        }

        public IPEndPoint GetP1EndPoint()
        {
            return player1;
        }
        public IPEndPoint GetP2EndPoint()
        {
            return player2;
        }
        public string GetP1ID()
        {
            return player1_id;
        }
        public string GetP2ID()
        {
            return player2_id;
        }
    }
    class UDPmanager
    {
        private List<IPEndPoint> clients;
        private List<Game> games;
        UdpClient recivingUdpClient;
        Protomanager protomanager;

        public UDPmanager()
        {
            Console.WriteLine("Begin UDP listner");
            clients = new List<IPEndPoint>();
            games = new List<Game>();
            protomanager = new Protomanager();
            recivingUdpClient = new UdpClient(11000);
            Thread listenThread = new Thread(ListenMessage);
            listenThread.Start();
            Console.WriteLine("UDP listner running");
        }

        private void ListenMessage()
        {
            while (true)
            {
                try
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] recivingBytes = recivingUdpClient.Receive(ref RemoteIpEndPoint);
                    Console.WriteLine("got something!");

                    GameMessage message = protomanager.ParseMessage(recivingBytes);

                    
                    if (message.IsGreet)
                    {
                        bool found = false;
                        foreach (IPEndPoint client in clients)
                        {
                            if (client.Port.Equals(RemoteIpEndPoint.Port) && client.Address.Equals(RemoteIpEndPoint.Address))
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            clients.Add(RemoteIpEndPoint);                            
                            SendMessage(RemoteIpEndPoint, protomanager.GreetMessage(message.PlayerId));
                        }
                    }
                    else if(message.IsActive)
                    {

                        //check which game
                        foreach (Game game in games)
                        {
                            
                            if (game.isGameover)
                            {
                                //send data to other player
                                if ((game.GetP1EndPoint().Port.Equals(RemoteIpEndPoint.Port) && game.GetP1EndPoint().Equals(RemoteIpEndPoint.Address)))
                                {
                                    SendMessage(game.GetP2EndPoint(), recivingBytes);
                                    game.isGameover = message.IsGameover; //if gameover mark game as done
                                }
                                else if (game.GetP2EndPoint().Port.Equals(RemoteIpEndPoint.Port) && game.GetP2EndPoint().Equals(RemoteIpEndPoint.Address))
                                {
                                    SendMessage(game.GetP1EndPoint(), recivingBytes);
                                    game.isGameover = message.IsGameover; //if gameover mark game as done
                                } 
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void SendMessage(IPEndPoint targetEp, byte[] b_msg)
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
