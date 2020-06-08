using BIF.SWE1.Interfaces;
using System;
using System.IO;
namespace myWebServer
{
    public class ToLowPlugin : IPlugin
    {

        /// <summary>
        /// Returns a score between 0 and 1 to indicate that the plugin is willing to handle the request. The plugin with the highest score will execute the request.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A score between 0 and 1</returns>
        public float CanHandle(IRequest req)
        {
            foreach (string seg in req.Url.Segments)
            {
                if (seg == "tolow")
                {
                    return 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Called by the server when the plugin should handle the request.
        /// Returns the entered string converted to lowercase.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A new response object.</returns>
        public IResponse Handle(IRequest req)
        {
            Response response = new Response();
            string text = string.Empty;
            text = req.ContentString;
            Console.WriteLine(req.ContentString);
            text = text.ToLower();
            response.SetContent(text);
            response.AddHeader(Strings.HTTP.HEADER_CONTENT_TYPE, "text/plain");
            response.StatusCode = 200;
            return response;
        }
    }
}