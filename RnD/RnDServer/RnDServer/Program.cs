using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
//using Mono.Nat;

namespace RnDServer
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("start");
            //OPENNAT oPENNAT = new OPENNAT();
            //await oPENNAT.func1Async();
            //TCPer();
            //NatDiscover();
            //NETLIBTEST nETLIBTEST = new NETLIBTEST();
            //nETLIBTEST.ServerSide();
            //Console.ReadLine();
            

            string externalIP = new WebClient().DownloadString("http://icanhazip.com");
            Console.WriteLine("Serer global IP is: " + externalIP);
            HolePunch holePunch = new HolePunch();
            while (true) ;
        }
        
        /*
        static void Main(string[] args)
        {
            Console.WriteLine("Hello world!");
            var t = Task.Run(async () =>
            {
                Console.WriteLine("Task start");                
                var discoverer = new NatDiscoverer();                
                var cts = new CancellationTokenSource(10000);
                Console.WriteLine("DiscoverDeviceAsync");
                
                try
                {
                    var devices = await discoverer.DiscoverDevicesAsync(PortMapper.Upnp, cts);
                    try
                    {
                        var ip = await devices.First().GetExternalIPAsync();
                        Console.WriteLine("The external IP Address is: {0} ", ip);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Console.WriteLine("CreatePortMapAsync");
                    try
                    {
                        await devices.First().CreatePortMapAsync(new Mapping(Protocol.Tcp, 1600, 1700, "The mapping name"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                Console.WriteLine("GetExternalIPAsync");
                
                
                
                
                
            });
            try
            {
                Console.WriteLine("Wait");
                t.Wait();
                Console.WriteLine("WaitDone");
            }
            catch (AggregateException e)
            {
                if (e.InnerException is NatDeviceNotFoundException)
                {
                    Console.WriteLine("Not found");
                    Console.WriteLine("Press any key to exit...");
                }
            }
            Console.WriteLine("ReadKey");
            Console.ReadKey();
        }
        */

        //public const int upnp_port = 3075;

        //private static UPnPNATClass pnp = new UPnPNATClass();
        //private static IStaticPortMappingCollection mapc = pnp.StaticPortMappingCollection;

        //public static IPAddress local_ip()
        //{
        //    foreach (IPAddress addr in Dns.GetHostEntry(string.Empty).AddressList)
        //        if (addr.AddressFamily == AddressFamily.InterNetwork)
        //            return addr;
        //    return null;
        //}

        //public static void upnp_open()
        //{
        //    mapc.Add(upnp_port, "UDP", upnp_port, local_ip().ToString(), true, "P2P Service Name");
        //}

        //public static void upnp_close()
        //{
        //    mapc.Remove(upnp_port, "UDP");
        //}

        static void TCPer()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[3];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            //Socket listener = new Socket(ipAddress.AddressFamily,
            //    SocketType.Stream, ProtocolType.Tcp);
            TcpListener listner = TcpListener.Create(11000);
            listner.AllowNatTraversal(true);
            //listner.Server.
            listner.Start();
            Console.WriteLine("started");

        }

        static void Webber()
        {
            WebSocket connection = new ClientWebSocket();


        }

        async static void NatDiscover()
        {


            Console.WriteLine("start discover");

            var discoverer = new NatDiscoverer();

            Console.WriteLine("await discorverer");
            // using SSDP protocol, it discovers NAT device.
            var device = await discoverer.DiscoverDeviceAsync();

            // display the NAT's IP address
            Console.WriteLine("The external IP Address is: {0} ", await device.GetExternalIPAsync());

            // create a new mapping in the router [external_ip:1702 -> host_machine:1602]
            await device.CreatePortMapAsync(new Mapping(Protocol.Tcp, 1602, 1702, "For testing"));

            // configure a TCP socket listening on port 1602
            var endPoint = new IPEndPoint(IPAddress.Any, 1602);
            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
            socket.Bind(endPoint);
            socket.Listen(4);
        }
    }
}
