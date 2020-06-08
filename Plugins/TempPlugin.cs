using BIF.SWE1.Interfaces;
using System;
using System.Xml;
using System.IO;
using MySql.Data.MySqlClient;

namespace myWebServer
{
    public class TempPlugin : IPlugin
    {
        private string year, month, day;
        private bool mod = false;

        /// <summary>
        /// Returns a score between 0 and 1 to indicate that the plugin is willing to handle the request. The plugin with the highest score will execute the request.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A score between 0 and 1</returns>
        public float CanHandle(IRequest req)
        {
            foreach (string seg in req.Url.Segments)
            {
                if (seg == "GetTemperature")
                {
                    return 1;
                }
            }
            return 0;
        }


        /// <summary>
        /// Called by the server when the plugin should handle the request.
        /// Reads temperature data from DB or can generate new temperature data.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A new response object.</returns>
        public IResponse Handle(IRequest req)
        {
            Response response = new Response();
            int tempIndex = 0;
            foreach (string seg in req.Url.Segments)
            {
                if (seg == "generateData")
                {
                    generateData();
                    response.SetContent("Data Generated");
                    response.StatusCode = 200;
                    return response;
                }
            }
            if (req.Url.Parameter.Count > 0)
            {
                mod = false;
                if (!req.Url.Parameter.TryGetValue("year", out year))
                {
                    year = "";
                }
                if (!req.Url.Parameter.TryGetValue("month", out month))
                {
                    month = "";
                }
                if (!req.Url.Parameter.TryGetValue("day", out day))
                {
                    day = "";
                }
            }
            else
            {
                mod = true;
                for (int i = 0; i < req.Url.Segments.Length; i++)
                {
                    if (req.Url.Segments[i] == "GetTemperature")
                    {
                        tempIndex = i;
                    }
                    if (i == tempIndex + 1)
                    {
                        year = req.Url.Segments[i];
                    }
                    else if (i == tempIndex + 2)
                    {
                        month = req.Url.Segments[i];
                    }
                    else if (i == tempIndex + 3)
                    {
                        day = req.Url.Segments[i];
                    }
                }
            }
            //search in DB
            using (var streamReader = new MemoryStream())
            {
                using (MySqlConnection myConnection = SQLConnection.Instance.Connection())
                {
                    try
                    {
                        myConnection.Open();
                        Console.WriteLine("TempPlugin: {0}", $"Request temps for : {year}/{month}/{day}");
                        MySqlCommand cmd = new MySqlCommand("select * from temperature where YEAR(RecordDate) = @year" +
                        " and MONTH(RecordDate) = @month" +
                        $" and DAY(RecordDate) = @day", myConnection);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@day", day);
                        cmd.Prepare();
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        DateTime date = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                        DateTime dateNext = date.AddDays(1);
                        DateTime datePrev = date.AddDays(-1);

                        if (mod == true)
                        {

                            using (XmlWriter writer = XmlWriter.Create(streamReader))
                            {

                                writer.WriteStartElement("temps");
                                writer.WriteAttributeString("Date", $"{year}/{month}/{day}");
                                while (rdr.Read())
                                {
                                    writer.WriteElementString("temp", rdr[1].ToString());
                                }
                                writer.WriteEndElement();
                                writer.Flush();
                                response.SetContent(streamReader.ToArray());
                                response.AddHeader(Strings.HTTP.HEADER_CONTENT_TYPE, "text/xml");
                            }
                        }
                        else
                        {
                            using (var sw = new StreamWriter(streamReader))
                            {
                                sw.Write("<html>");
                                sw.Write("<head>");
                                sw.Write("<title>Temps</title>");
                                sw.Write("</head>");
                                sw.Write("<body>");
                                sw.Write("<form action=\"/\"> <button class=\"repair__button\" type=\"submit\">Home</button></form>");
                                if (rdr.HasRows)
                                {
                                    sw.Write("<table border=1 frame=hsides rules=rows>");
                                    sw.Write("<tr>");
                                    sw.Write("<th> {0}/{1}/{2}</th>", year, month, day);
                                    sw.Write("</tr>");
                                    while (rdr.Read())
                                    {
                                        sw.Write("<tr><td>{0}<td></tr>", rdr[1]);
                                    }
                                    sw.Write("</table>");
                                    sw.Write($"<button onclick=\"window.location.href='/GetTemperature?year={datePrev.Year}&month={datePrev.Month}&day={datePrev.Day}';\">Prev</button>");
                                    sw.Write($"<button onclick=\"window.location.href='/GetTemperature?year={dateNext.Year}&month={dateNext.Month}&day={dateNext.Day}';\">Next</button>");
                                }
                                else
                                {
                                    sw.Write("<div>No results found!</div>");
                                }
                                sw.Write("</body>");
                                sw.Write("</html>");
                                sw.Flush();
                                response.SetContent(streamReader.ToArray());
                            }
                            response.AddHeader(Strings.HTTP.HEADER_CONTENT_TYPE, "text/html");
                        }
                        rdr.Close();
                        myConnection.Close();
                    }
                    catch (MySqlException e)
                    {
                        using (var sw = new StreamWriter(streamReader))
                        {
                            sw.Write("<html>");
                            sw.Write("<head>");
                            sw.Write("<title>Navi</title>");
                            sw.Write("</head>");
                            sw.Write("<body>");
                            sw.Write("<div>DATABASE ERROR</div>");
                            sw.Write("<form action=\"/\"> <button class=\"repair__button\" type=\"submit\">Home</button></form>");
                            sw.Write("</body>");
                            sw.Write("</html>");
                            sw.Flush();
                            response.SetContent(streamReader.ToArray());
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            response.StatusCode = 200;
            return response;
        }

        /// <summary>
        /// Generates new temperture data.
        /// </summary>
        private void generateData()
        {
            var sqlInsert = "INSERT INTO temperature (Temperature,recordDate) VALUES(@temp,@oldDate) ";
            var sqlDelete = "TRUNCATE TABLE temperature";
            DateTime date = DateTime.Now;
            DateTime oldDate = date.AddYears(-10);

            Random r = new Random();
            using (MySqlConnection myConnection = SQLConnection.Instance.Connection())
            {
                try
                {
                    myConnection.Open();
                    var cmd = new MySqlCommand(sqlDelete, myConnection);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    while (!DateTime.Equals(date, oldDate))
                    {
                        for (int i = 0; i <= 10; i++)
                        {
                            int temp = r.Next(-10, 60);
                            Console.WriteLine(oldDate.ToString() + " " + temp);
                            cmd = new MySqlCommand(sqlInsert, myConnection);
                            string formatForMySql = oldDate.ToString("yyyy-MM-dd");
                            cmd.Parameters.AddWithValue("@oldDate", formatForMySql);
                            cmd.Parameters.AddWithValue("@temp", temp);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        oldDate = oldDate.AddDays(1);
                    }
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("TempPlugin: {0}", e);
                }
            }
        }
    }
}
