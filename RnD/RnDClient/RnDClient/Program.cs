using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace RnDClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //ClientSide();

            HolePunch holePunch = new HolePunch("Enter IP here: 0.0.0.0", 11000);

            while (true)
            {
                string msg = Console.ReadLine();
                holePunch.SendMessage(msg);
            }

        }
        public static void ClientSide()
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager client = new NetManager(listener);
            client.Start();
            //client.Connect("192.168.1.40" /* host ip or name */, 9050 /* port */, "potatis" /* text key or NetDataWriter */);
            client.Connect("85.229.42.50" /* host ip or name */, 42069 /* port */, "potatis" /* text key or NetDataWriter */);
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                Console.WriteLine("We got: {0}", dataReader.GetString(100 /* max length of string */));
                dataReader.Recycle();
            };

            while (!Console.KeyAvailable)
            {
                client.PollEvents();
                Thread.Sleep(15);
            }

            client.Stop();
        }
    }
}
