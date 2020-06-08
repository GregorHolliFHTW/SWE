using BIF.SWE1.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace myWebServer
{
    public class Request : IRequest
    {
        private StreamReader sr;
        private string[] tokens = { };
        private bool isValid = false;
        private string method, host, contentType, contentString;
        private int headerCount, contentLength;
        private Url url;
        private IDictionary<string, string> headers = new Dictionary<string, string>();
        private string userAgent;
        private Stream contentStream;
        private byte[] contentBytes = { };

        /// <summary>
        /// Request constructor.
        /// <param name="request"></param>
        ///</summary>
        public Request(Stream request)
        {
            sr = new StreamReader(request);
            getFirstLine();
            if (isValid)
            {
                extractHeaders();
                getPostContent();
            }
        }

        /// <summary>
        /// Reads the first line of the request (URL and Method).
        /// </summary>
        private void getFirstLine()
        {
            String req = sr.ReadLine();
            if (!String.IsNullOrEmpty(req))
            {
                tokens = req.Split(' ');
                if (tokens.Length != 3)
                {
                    throw new Exception("invalid http request");
                }
                method = tokens[0].ToUpper();
                url = new Url("localhost/" + tokens[1]);
            }
            //Console.WriteLine("Path: {0}",url.FileName);
            checkIfIsValid();
        }

        /// <summary>
        /// Checks if the Request is valid.
        /// </summary>
        private void checkIfIsValid()
        {
            if ((method == Strings.HTTP.GET_STRING || method == Strings.HTTP.POST_STRING))
            {
                isValid = true;
            }
        }

        /// <summary>
        /// Reads and saves the request header
        /// </summary>
        private void extractHeaders() //read and save all headers
        {          
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("Request: {0}", "Header read ready");
                    break;
                }
                string[] headerLine = line.Split(Strings.HTTP.HEADER_SEPERATOR, 2);
                headers.Add(headerLine[0].ToLower(), headerLine[1].TrimStart(' '));
            }
            headerCount = headers.Count;
            headers.TryGetValue(Strings.HTTP.HEADER_USER_AGENT.ToLower(), out userAgent);
        }

        /// <summary>
        /// If the Request Method is POST, gets the content.
        /// </summary>
        private void getPostContent() //if the request is POST, save the content
        {            
            if (method == Strings.HTTP.POST_STRING)
            {
                headers.TryGetValue(Strings.HTTP.HEADER_CONTENT_TYPE.ToLower(), out contentType);
                string length = "0";
                if (headers.TryGetValue(Strings.HTTP.HEADER_CONTENT_LENGTH.ToLower(), out length))
                {
                    contentLength = int.Parse(length);
                }
                char[] content = new char[contentLength];
                sr.Read(content, 0, contentLength);
                contentString = new String(content);
                contentBytes = Encoding.UTF8.GetBytes(contentString);
                contentStream = new MemoryStream(ContentBytes);
            }
        }

        /// <summary>
        /// Returns true if the request is valid. A request is valid, if method and url could be parsed. A header is not necessary.
        /// </summary>
        public bool IsValid
        {
            get { return isValid; }
        }

        /// <summary>
        /// Returns the request method in UPPERCASE. get -> GET.
        /// </summary>
        public string Method
        {
            get { return this.method; }
        }

        /// <summary>
        /// Returns a URL object of the request. Never returns null.
        /// </summary>
        public IUrl Url
        {
            get { return url; }
        }

        /// <summary>
        /// Returns the request header. Never returns null. All keys must be lower case.
        /// </summary>
        public IDictionary<string, string> Headers
        {
            get { return headers; }
        }

        /// <summary>
        /// Returns the user agent from the request header
        /// </summary>
        public string UserAgent
        {
            get { return userAgent; }
        }

        /// <summary>
        /// Returns the number of header or 0, if no header where found.
        /// </summary>
        public int HeaderCount
        {
            get { return headerCount; }
        }

        /// <summary>
        /// Returns the parsed content length request header.
        /// </summary>
        public int ContentLength
        {
            get { return contentLength; }
        }

        /// <summary>
        /// Returns the parsed content type request header. Never returns null.
        /// </summary>
        public string ContentType
        {
            get { return contentType; }
        }

        /// <summary>
        /// Returns the request content (body) stream or null if there is no content stream.
        /// </summary>
        public Stream ContentStream
        {
            get { return contentStream; }
        }

        /// <summary>
        /// Returns the request content (body) as string or null if there is no content.
        /// </summary>
        public string ContentString
        {
            get { return contentString; }
        }

        /// <summary>
        /// Returns the request content (body) as byte[] or null if there is no content.
        /// </summary>
        public byte[] ContentBytes
        {
            get { return contentBytes; }
        }
    }
}