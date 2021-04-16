using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;

namespace Server
{
    class Client
    {
        public bool isActive;
        public readonly IPEndPoint ClientEndPoint;
        public readonly string ClientId;
        public Client(IPEndPoint Ep, string id)
        {
            ClientEndPoint = Ep;
            ClientId = id;
        }
    }
    class Game
    {
        public bool isGameover;
        Client player1;
        Client player2;
        public Game(bool _isGameover)
        {
            isGameover = _isGameover;
        }
        public Game(IPEndPoint p1, string p1_id, IPEndPoint p2, string p2_id)
        {
            isGameover = false;
            player1 = new Client(p1, p1_id);
            player2 = new Client(p2, p2_id);
        }

        public Client GetP1()
        {
            return player1;
        }
        public Client GetP2()
        {
            return player2;
        }
    }
    class UDPmanager
    {
        private List<Client> clients;
        private List<Game> games;
        UdpClient recivingUdpClient;
        Protomanager protomanager;

        public UDPmanager()
        {
            Console.WriteLine("Begin UDP listner");
            clients = new List<Client>();
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

                    Console.WriteLine("message was: id: {0}, isGreet: {1}", message.PlayerId, message.IsGreet);

                    //On startup client sends message with greet true
                    if (message.IsGreet)
                    {
                        bool found = false;
                        foreach (Client client in clients)
                        {
                            if (client.ClientEndPoint.Port.Equals(RemoteIpEndPoint.Port) && client.ClientEndPoint.Address.Equals(RemoteIpEndPoint.Address))
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            clients.Add(new Client(RemoteIpEndPoint, message.PlayerId));
                            SendMessage(RemoteIpEndPoint, protomanager.GreetMessage(message.PlayerId));
                        }
                        foreach (Client client in clients)
                        {
                            if (!client.ClientEndPoint.Port.Equals(RemoteIpEndPoint.Port) && !client.ClientEndPoint.Address.Equals(RemoteIpEndPoint.Address))
                            {
                                if (!client.isActive)
                                {
                                    //new opponent found!
                                    games.Add(new Game(RemoteIpEndPoint, message.PlayerId, client.ClientEndPoint, client.ClientId));
                                    SendMessage(RemoteIpEndPoint, protomanager.NewGameP1(client.ClientId));
                                    SendMessage(client.ClientEndPoint, protomanager.NewGameP2(message.PlayerId));
                                }
                            }
                        }
                    }
                    //player is known and active in a game
                    else if (message.IsActive)
                    {
                        Console.WriteLine("Active player messaged");

                        Console.WriteLine("Message was: playerID= {0}, move_number= {1}, move= {2}, isGreet= {3}, isActive= {4}, isGameover= {5}", message.PlayerId, message.MoveNumber, message.Move, message.IsGreet, message.IsActive, message.IsGameover);

                        int ig = 0;
                        int g = 0;
                        //check which game
                        foreach (Game game in games)
                        {
                            if (!game.isGameover)
                            {
                                int i = 0;
                                int p1 = 0;
                                int p2 = 0;
                                //send data to other player
                                if ((game.GetP1().ClientEndPoint.Port.Equals(RemoteIpEndPoint.Port) && game.GetP1().ClientEndPoint.Address.Equals(RemoteIpEndPoint.Address)))
                                {
                                    SendMessage(game.GetP2().ClientEndPoint, recivingBytes);
                                    if (message.IsGameover)
                                    {
                                        foreach (Client client in clients)
                                        {
                                            if (game.GetP2().ClientEndPoint.Port.Equals(client.ClientEndPoint.Port) && game.GetP2().ClientEndPoint.Address.Equals(client.ClientEndPoint.Address))
                                            {
                                                p2 = i;
                                                client.isActive = false; //marks player in client list as not in game                                                
                                            }
                                            if (game.GetP1().ClientEndPoint.Port.Equals(client.ClientEndPoint.Port) && game.GetP1().ClientEndPoint.Address.Equals(client.ClientEndPoint.Address))
                                            {
                                                p1 = i;
                                                client.isActive = false; //marks player in client list as not in game                                                
                                            }
                                            i++;
                                        }
                                        game.isGameover = message.IsGameover; //if gameover mark game as done                                        
                                    }
                                }
                                else if (game.GetP2().ClientEndPoint.Port.Equals(RemoteIpEndPoint.Port) && game.GetP2().ClientEndPoint.Address.Equals(RemoteIpEndPoint.Address))
                                {
                                    SendMessage(game.GetP1().ClientEndPoint, recivingBytes);
                                    if (message.IsGameover)
                                    {

                                        foreach (Client client in clients)
                                        {
                                            if (game.GetP2().ClientEndPoint.Port.Equals(client.ClientEndPoint.Port) && game.GetP2().ClientEndPoint.Address.Equals(client.ClientEndPoint.Address))
                                            {
                                                p2 = i;
                                                client.isActive = false; //marks player in client list as not in game
                                            }
                                            if (game.GetP1().ClientEndPoint.Port.Equals(client.ClientEndPoint.Port) && game.GetP1().ClientEndPoint.Address.Equals(client.ClientEndPoint.Address))
                                            {
                                                p1 = i;
                                                client.isActive = false; //marks player in client list as not in game
                                            }
                                            i++;
                                        }
                                        game.isGameover = message.IsGameover; //if gameover mark game as done
                                    }
                                }
                                if (game.isGameover)
                                {
                                    clients.RemoveAt(p1);
                                    clients.RemoveAt(p2);
                                    foreach (Client client in clients)
                                    {
                                        Console.WriteLine(client);
                                    }
                                }
                            }
                            ig++;
                        }
                        if (games[g].isGameover)
                        {
                            games.RemoveAt(g);
                        }
                        
                    }
                    //player is not active and looking for game
                    else
                    {
                        Console.WriteLine("Non Active player messaged");
                        foreach (Client client in clients)
                        {
                            if (!client.ClientEndPoint.Port.Equals(RemoteIpEndPoint.Port) && !client.ClientEndPoint.Address.Equals(RemoteIpEndPoint.Address))
                            {
                                if (!client.isActive)
                                {
                                    //new opponent found!
                                    games.Add(new Game(RemoteIpEndPoint, message.PlayerId, client.ClientEndPoint, client.ClientId));
                                    SendMessage(RemoteIpEndPoint, protomanager.NewGameP1(client.ClientId));
                                    SendMessage(client.ClientEndPoint, protomanager.NewGameP1(message.PlayerId));
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
