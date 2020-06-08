using BIF.SWE1.Interfaces;
using System;
using System.Collections.Generic;
using myWebServer;
using System.IO;

namespace myWebServer
{
    public class FilePlugin : IPlugin
    {
        /// <summary>
        /// Returns a score between 0 and 1 to indicate that the plugin is willing to handle the request. The plugin with the highest score will execute the request.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A score between 0 and 1</returns>
        public float CanHandle(IRequest req)
        {
            if (req.Url.FileName != string.Empty || req.Url.Path == "/")
            {
                return 1;
            }
            return 0;

        }

        /// <summary>
        /// Called by the server when the plugin should handle the request.
        /// Returns the requested file.
        /// If Url Path is '/' return index.hmtl (home page).
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A new response object.</returns>
        public IResponse Handle(IRequest req)
        {
            string dPath = Directory.GetCurrentDirectory();
            Response response = new Response();
            if (req.Url.Path.Contains(".."))
            {
                response.StatusCode = 404; //Not Found
            }
            else
            {
                // response.StatusCode = 200;
                FileInfo fileInfo;
                try
                {
                    Console.WriteLine("File:" + req.Url.Path + "  " + req.Url.FileName);

                    if (req.Url.Path == "/")
                    {
                        fileInfo = new FileInfo(Path.Combine(dPath + "/wwwroot", "index.html"));
                        response.AddHeader(Strings.HTTP.HEADER_CONTENT_TYPE, handleExt(req.Url.Extension));
                    }
                    else
                    {
                        fileInfo = new FileInfo(Path.Combine(dPath + req.Url.Path, req.Url.FileName));
                        response.AddHeader(Strings.HTTP.HEADER_CONTENT_TYPE, handleExt(req.Url.Extension));
                    }
                    using (FileStream fs = fileInfo.OpenRead())
                    {
                        response.AddHeader(Strings.HTTP.HEADER_CONTENT_LENGTH, (fs.Length).ToString());
                        byte[] data = new Byte[fs.Length];

                        using (BinaryReader reader = new BinaryReader(fs))
                        {
                            reader.Read(data, 0, data.Length);
                            response.SetContent(data);
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("FilePlugin: {0}", e);
                    response.StatusCode = 404;
                    return response;
                }
                response.StatusCode = 200;
            }
            return response;
        }

        /// <summary>
        /// Returns content type depending on the file extension.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A content type string.</returns>
        private string handleExt(string fileExt)
        {
            string contentType = "";
            switch (fileExt)
            {
                case "png":
                    {
                        contentType = "image/apng";
                        break;
                    }
                case "jpg":
                    {
                        contentType = "image/jpg";
                        break;
                    }
                case "ico":
                    {
                        contentType = "image/jpg";
                        break;
                    }
                case "":
                case "html":
                    {
                        contentType = "text/html";
                        break;
                    }
            }
            return contentType;
        }
    }
}