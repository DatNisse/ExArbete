using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server start...");
            string externalIP = new WebClient().DownloadString("http://icanhazip.com");
            Console.WriteLine("Serer global IP is: " + externalIP);
            UDPmanager uDPmanager = new UDPmanager();
            while (true) ;
        }
    }
}
