using myWebServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace SWE_Holli
{

    class Program
    {

        private static void Listen()
        {
            Server server = new Server();
            server.startServer();
        }

        static void Main(string[] args)
        {
            Listen();
        }

    }
}
