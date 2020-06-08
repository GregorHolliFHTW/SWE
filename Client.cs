using BIF.SWE1.Interfaces;
using System.Net.Sockets;

namespace myWebServer
{
    public class Client
    {
        private NetworkStream stream;
        private Request request;

        public Client(TcpClient client)
        {
            stream = client.GetStream();
        }

        public void Run()
        { handeRequest(); }

        private void handeRequest() //handles request and notifies necessary plugin
        {
            request = new Request(stream);
            if (request.IsValid)
            {
                Response response = new Response();
                foreach (IPlugin pl in PluginManager.Instance.Plugins)
                {
                    if (pl.CanHandle(request) == 1)
                    {
                        response = (Response)pl.Handle(request); //send plugin the task
                    }
                }
                response.AddHeader(Strings.HTTP.HEADER_SERVER, "C# WebServer"); //rem: adds/replaced response header in dictionary
                response.AddHeader(Strings.HTTP.HEADER_CONTENT_CODING, "gzip");
                response.AddHeader(Strings.HTTP.HEADER_CONNECTION, "keep-alive");
                response.Send(stream);
                stream.Flush();
                stream.Close();
            }
        }
    }
}