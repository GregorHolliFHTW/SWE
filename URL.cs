using BIF.SWE1.Interfaces;
using System;
using System.Web;
using System.Collections.Generic;

namespace myWebServer
{
    public class Url : IUrl
    {
        String rawUrl = "";
        String[] urlArray = { };
        private String url = "";
        private String path = "";

        private String fileName = String.Empty;
        private String fileExtension = "";
        private String fragment = "";
        private int paramCount = 0;
        private IDictionary<string, string> paramters = new Dictionary<string, string>();
        private string[] segments = { };

        /// <summary>
        /// URL constructor.
        /// </summary>
        public Url(string _rawUrl)
        {
            rawUrl = _rawUrl;
            url = _rawUrl;

            extractProtocol(); 
            extractFragment(); // save the fragment and remove it
            separateParameters(); 
            extractSegments(); // Save the segments as array 
            extractParameters(); // If any parameters, save the as Dictionary
        }

        /// <summary>
        /// Extracts the protcol http or https.
        /// </summary>
        private void extractProtocol()
        {
            if (rawUrl.Contains("http://") || rawUrl.Contains("https://"))
            {
                rawUrl = rawUrl.Split("://")[1];
            }
        }

        /// <summary>
        /// Extracts fragment from the url.
        /// </summary>
        private void extractFragment()
        {
            if (rawUrl.Contains(Strings.URL.fragmentSeparator))
            {
                string[] splited = rawUrl.Split(Strings.URL.fragmentSeparator);
                fragment = splited[1];
                rawUrl = rawUrl.Remove(rawUrl.IndexOf(Strings.URL.fragmentSeparator));
            }
        }

        /// <summary>
        /// Separates the path from the parameters.
        /// Saves the path.
        /// </summary>
        private void separateParameters()
        {
            if (rawUrl.Contains(Strings.URL.pathParameterSeparator))
            {
                urlArray = rawUrl.Split(Strings.URL.pathParameterSeparator);
                if (rawUrl.Contains(Strings.URL.separator))
                {

                    path = urlArray[0].Split(Strings.URL.separator, 2)[1];
                }
            }
            // if no parameters, extract the path
            else
            {
                if (rawUrl.Contains(Strings.URL.separator))
                {
                    path = rawUrl.Split(Strings.URL.separator, 2)[1];
                    //hostName = rawUrl.Split('/', 2)[0];
                }
            }
        }

        /// <summary>
        /// Extracts the url segments.
        /// </summary>
        private void extractSegments()
        {
            String[] segmentsArray;
            segmentsArray = path.Split(Strings.URL.separator);
            segments = new string[segmentsArray.Length];
            for (int i = 0; i < segmentsArray.Length; i++)
            {
                if (segmentsArray[i].Contains(Strings.URL.fileDot))
                {
                    fileName = segmentsArray[i];
                    fileExtension = Strings.URL.fileDot + segmentsArray[i].Split(Strings.URL.fileDot)[1];
                    path = path.Remove((path.Length - fileName.Length) - 1);
                    //continue;
                }
                segments[i] = segmentsArray[i];
            }
        }

        /// <summary>
        /// Extracts the url parameters.
        /// </summary>
        private void extractParameters()
        {

            String[] urlParamsRaw;
            if (urlArray.Length > 0)
            {
                urlParamsRaw = urlArray[1].Split(Strings.URL.parameterSeparator);
                paramCount = urlParamsRaw.Length;
                foreach (String s in urlParamsRaw)
                {
                    if (s.Contains('='))
                    {
                        paramters.Add(s.Split('=')[0].ToLower(), System.Web.HttpUtility.UrlDecode(s.Split('=')[1]));
                    }
                }
            }
        }

        /// <summary>
        /// Returns the raw url.
        /// </summary>
        public string RawUrl
        {
            get { return url; }
        }

        /// <summary>
        /// Returns the path of the url, without parameter.
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Returns a dictionary with the parameter of the url. Never returns null.
        /// </summary>
        public IDictionary<string, string> Parameter
        {
            get { return paramters; }
        }

        /// <summary>
        /// Returns the number of parameter of the url. Returns 0 if there are no parameter.
        /// </summary>
        public int ParameterCount
        {
            get { return paramCount; }
        }

        /// <summary>
        /// Returns the segments of the url path. A segment is divided by '/' chars. Never returns null.
        /// </summary>
        public string[] Segments
        {
            get { return segments; }
        }

        /// <summary>
        /// Returns the filename (with extension) of the url path. If the url contains no filename, a empty string is returned. Never returns null. A filename is present in the url, if the last segment contains a name with at least one dot.
        /// </summary>
        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// Returns the extension of the url filename, including the leading dot. If the url contains no filename, a empty string is returned. Never returns null.
        /// </summary>
        public string Extension
        {
            get { return fileExtension; }
        }

        /// <summary>
        /// Returns the url fragment. A fragment is the part after a '#' char at the end of the url. If the url contains no fragment, a empty string is returned. Never returns null.
        /// </summary>
        public string Fragment
        {
            get { return fragment; }
        }
    }
}


