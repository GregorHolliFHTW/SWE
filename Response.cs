using BIF.SWE1.Interfaces;
using System;
using System.Web;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace myWebServer
{
    public class Response : IResponse
    {

        private int contentLength = 0;
        private string contentString = "";
        private string contentType = "";
        private string status = "";
        private string serverHeader = "";
        public string responseHeader = "";

        private IDictionary<string, string> headers = new Dictionary<string, string>();
        private byte[] contentBytes = { };
        private Stream contentStream;
        private int statusCode = 0;

        /// <summary>
        /// Response constructor.
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Returns a writable dictionary of the response headers. Never returns null.
        /// </summary>
        public IDictionary<string, string> Headers
        {
            get { return headers; }
        }

        /// <summary>
        /// Returns the content length or 0 if no content is set yet.
        /// </summary>
        public int ContentLength
        {
            get { return contentLength; }
        }

        /// <summary>
        /// Gets or sets the content type of the response.
        /// </summary>
        /// <exception cref="InvalidOperationException">A specialized implementation may throw a InvalidOperationException when the content type is set by the implementation.</exception>

        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        /// <summary>
        /// Gets or sets the current status code. An Exceptions is thrown, if no status code was set.
        /// </summary>
        public int StatusCode
        {
            get { return statusCode; }
            set
            {
                statusCode = value;
                switch (statusCode)
                {
                    case 100:
                        {
                            status = statusCode + " Continue";
                            break;
                        }
                    case 200:
                        {
                            status = statusCode + " OK";
                            break;
                        }
                    case 301:
                        {
                            status = statusCode + "  See Other";

                            break;
                        }
                    case 400:
                        {
                            status = statusCode + " Bad Request";
                            break;
                        }
                    case 404:
                        {
                            status = statusCode + " Not Found";
                            break;
                        }
                    case 500:
                        {
                            status = statusCode + " Internal Server Error";
                            break;
                        }
                }

            }
        }

        /// <summary>
        /// Returns the status code as string. (200 OK)
        /// </summary>
        public string Status
        {
            get { return status; }
        }

        /// <summary>
        /// Adds or replaces a response header in the headers dictionary.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="value"></param>
        public void AddHeader(string header, string value)
        {
            if (headers.ContainsKey(header))
            {
                headers[header] = value;
            }
            else
            {
                headers.Add(header, value);
            }
            if (header == Strings.HTTP.HEADER_CONTENT_TYPE)
            {
                contentType = value;
            }
            if (header == Strings.HTTP.HEADER_SERVER)
            {
                serverHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets the Server response header. Defaults to "BIF-SWE1-Server".
        /// </summary>
        public string ServerHeader
        {
            get { return serverHeader; }
            set { serverHeader = value; }
        }

        /// <summary>
        /// Sets a string content. The content will be encoded in UTF-8.
        /// </summary>
        /// <param name="content"></param>
        public void SetContent(string content)
        {
            contentString = content;
            contentBytes = Encoding.UTF8.GetBytes(content);
            contentLength = contentBytes.Length;
        }

        /// <summary>
        /// Sets a byte[] as content.
        /// </summary>
        /// <param name="content"></param>
        public void SetContent(byte[] content)
        {
            contentBytes = content;
            contentLength = content.Length;
            contentString = System.Text.Encoding.Default.GetString(content);
        }

        /// <summary>
        /// Sets the stream as content.
        /// </summary>
        /// <param name="stream"></param>
        public void SetContent(Stream stream)
        {
            contentStream = stream;
            using (var streamReader = new MemoryStream())
            {

                stream.CopyTo(streamReader);
                SetContent(streamReader.ToArray());
            }
        }

        /// <summary>
        /// Sends the response to the network stream.
        /// </summary>
        /// <param name="network"></param>
        public void Send(Stream network)
        {
            using (StreamWriter writer = new StreamWriter(network))
            {
                byte[] header = Encoding.UTF8.GetBytes(createHeader());
                writer.Write(createHeader());
                writer.WriteLine();
                writer.Flush();
                network.Write(contentBytes, 0, contentLength);
            }
        }

        /// <summary>
        /// Creates the response header.
        /// </summary>
        /// <returns>A new headers as a string.</returns>
        private string createHeader()
        {
            responseHeader = $"HTTP/1.1 {status}\r\n";
            foreach (KeyValuePair<string, string> h in headers)
            {
                responseHeader += h.Key + ": " + h.Value + "\r\n";
            }
            return responseHeader;

        }

        /// <summary>
        /// Gets the response header.
        /// </summary>
        public string ResponseHeader
        {
            get { return responseHeader; }
        }

        /// <summary>
        /// Gets the content as string.
        /// </summary>
        public string ContentString
        {
            get { return contentString; }
        }
    }
}