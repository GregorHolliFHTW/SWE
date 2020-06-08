using BIF.SWE1.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace myWebServer
{
    class Server
    {
        private int port = 8080;
        private TcpListener listener;
        private List<Thread> threads = new List<Thread>();

        public void startServer()
        {
            listener = new TcpListener(IPAddress.Any, port);
            SQLConnection conn = SQLConnection.Instance;
            PluginManager pm = PluginManager.Instance;

            PluginManager.Instance.Add(new TempPlugin());
            PluginManager.Instance.Add(new NaviPlugin());
            PluginManager.Instance.Add(new ToLowPlugin());
            PluginManager.Instance.Add(new FilePlugin());

            List<IPlugin> plugins = (List<IPlugin>)PluginManager.Instance.Plugins;
            Console.WriteLine("Server: {0}", $"Web Server is running on port: {port}");
            listener.Start();
            Listen();
        }

        private void handeClient(TcpClient client) //new client
        {
            Client _client = new Client(client);
            _client.Run();
            client.Close();
        }

        public void Listen()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Server: {0}", "Waiting for client...");
                    TcpClient client = listener.AcceptTcpClient();
                    if (client.Connected)
                    {
                        Console.WriteLine("Server: {0}", $"Client connected: IP {client.Client.RemoteEndPoint} ");
                        Thread t = new Thread(start: () => handeClient(client)); //new thread for each client
                        t.Start();
                        threads.Add(t);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        
    }
}
