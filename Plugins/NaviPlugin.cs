using BIF.SWE1.Interfaces;
using System;
using System.Web;
using System.IO;
using MySql.Data.MySqlClient;
using System.Text;
using System.Collections.Generic;

namespace myWebServer
{
    public class NaviPlugin : IPlugin
    {
        /// <summary>
        /// Returns a score between 0 and 1 to indicate that the plugin is willing to handle the request. The plugin with the highest score will execute the request.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A score between 0 and 1</returns>
        public float CanHandle(IRequest req)
        {
            if (req.Method == Strings.HTTP.GET_STRING)
            {
                foreach (string seg in req.Url.Segments)
                {
                    if (seg == "streetSearch")
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Called by the server when the plugin should handle the request.
        /// Searches in DB for given street, city or postcode and can return XML or HTML response..
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A new response object.</returns>
        public IResponse Handle(IRequest req)
        {
            Response response = new Response();
            string city = "";
            string postcode = "";
            string street = "";
            int navIndex = 0;
            for (int i = 0; i < req.Url.Segments.Length; i++)
            {
                if (req.Url.Segments[i] == "streetSearch")
                {
                    navIndex = i;
                }
            }
            {
                if (!req.Url.Parameter.TryGetValue("city", out city))
                {
                    city = "";
                }
                if (!req.Url.Parameter.TryGetValue("postcode", out postcode))
                {
                    postcode = "";
                }
                if (!req.Url.Parameter.TryGetValue("street", out street))
                {
                    street = "";
                }
                //DB
                using (var streamReader = new MemoryStream())
                {
                    int nr = 0;
                    using (var sw = new StreamWriter(streamReader))
                    {
                        using (MySqlConnection myConnection = SQLConnection.Instance.Connection())
                        {
                            try
                            {
                                myConnection.Open();
                                Console.WriteLine("NaviPlugin: {0}", $"Request search for City: {city}  Postcode: {postcode} Street: {street}");
                                MySqlCommand cmd = new MySqlCommand("select * from navi where street like concat('%',@street,'%') and city like concat('%',@city,'%') and postcode like concat('%',@postcode,'%')", myConnection);
                                cmd.Parameters.AddWithValue("@street", street);
                                cmd.Parameters.AddWithValue("@city", city);
                                cmd.Parameters.AddWithValue("@postcode", postcode);
                                cmd.Prepare();
                                MySqlDataReader rdr = cmd.ExecuteReader();

                                sw.Write("<html>");
                                sw.Write("<head>");
                                sw.Write("<title>Navi</title>");
                                sw.Write("</head>");
                                sw.Write("<body>");
                                sw.Write("<form action=\"/\"> <button class=\"repair__button\" type=\"submit\">Home</button></form>");
                                if (rdr.HasRows)
                                {
                                    sw.Write("<table border=1 frame=hsides rules=rows>");
                                    sw.Write("<tr>");
                                    sw.Write("<th> Nr. </th>");
                                    sw.Write("<th> City</th>");
                                    sw.Write("<th> Postcode</th>");
                                    sw.Write("<th> Street</th>");
                                    sw.Write("</tr>");

                                    while (rdr.Read())
                                    {
                                        nr++;
                                        sw.Write("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", nr, rdr[1], rdr[2], rdr[3]);
                                    }

                                    sw.Write("</table>");
                                }
                                else
                                {
                                    sw.Write("<div>No results found!</div>");
                                }
                                sw.Write("</body>");
                                sw.Write("</html>");
                                sw.Flush();
                                response.SetContent(streamReader.ToArray());
                                rdr.Close();
                            }
                            catch (MySqlException e)
                            {
                                sw.Write("<html>");
                                sw.Write("<head>");
                                sw.Write("<title>Navi</title>");
                                sw.Write("</head>");
                                sw.Write("<body>");
                                sw.Write("<div>DB ERROR</div>");
                                sw.Write("<form action=\"/\"> <button class=\"repair__button\" type=\"submit\">Home</button></form>");
                                sw.Write("</body>");
                                sw.Write("</html>");
                                sw.Flush();
                                response.SetContent(streamReader.ToArray());
                                Console.WriteLine(e);
                            }
                            response.AddHeader(Strings.HTTP.HEADER_CONTENT_TYPE, "text/html");
                            myConnection.Close();
                        }
                    }
                }
                response.StatusCode = 200;
                return response;
            }
        }
    }
}