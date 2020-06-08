using BIF.SWE1.Interfaces;
using Xunit;
using myWebServer;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class PluginTest
{
    private string host = "localhost:8080";
    byte[] bodyBytes = { };
    /*string content = "";
    Stream stream;*/
    public Request RequestCreate(string method, string url, string content = "")
    {
        Request req;
        using (var ms = new MemoryStream())
        {
            using (var sw = new StreamWriter(ms, Encoding.ASCII))
            {
                bodyBytes = Encoding.UTF8.GetBytes(content);
                sw.WriteLine("{0} {1} HTTP/1.1", method, url);
                sw.WriteLine("Host: {0}", host);
                sw.WriteLine("Connection: keep-alive");
                sw.WriteLine("Accept: text/html,application/xhtml+xml");
                sw.WriteLine("User-Agent: Unit-Test-Agent/1.0 (The OS)");
                sw.WriteLine("Accept-Encoding: gzip,deflate,sdch");
                sw.WriteLine("Accept-Language: de-AT,de;q=0.8,en-US;q=0.6,en;q=0.4");
                sw.WriteLine("Content-Type: text/plain");
                sw.WriteLine($"Content-Length: {bodyBytes.Length}");
                sw.WriteLine();
                sw.Flush();
                ms.Write(bodyBytes, 0, bodyBytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                req = new Request(ms);
            }
        }
        return req;
    }

    [Fact]
    public void FilePlugin_Can_Handle_PNG()
    {
        Request request = RequestCreate("GET", "/wwwroot/csharp.png");
        Assert.Equal(1, new FilePlugin().CanHandle(request));
    }

    [Fact]
    public void FilePlugin_Can_Handle_JS()
    {
        Request request = RequestCreate("GET", "/wwwroot/home.js");
        Assert.Equal(1, new FilePlugin().CanHandle(request));
    }

     [Fact]
    public void FilePlugin_Can_Handle_HTML()
    {
        Request request = RequestCreate("GET", "/wwwroot/index.html");
        Assert.Equal(1, new FilePlugin().CanHandle(request));
    }

    [Fact]
    public void FilePlugin_CanT_Handle()
    {
        Request request = RequestCreate("GET", "/tolow");
        Assert.Equal(0, new FilePlugin().CanHandle(request));
    }
    [Fact]
    public void FilePlugin_returns_404()
    {
        Request request = RequestCreate("GET", "/nosuchfile.html");
        Response response = (Response)new FilePlugin().Handle(request);
        Assert.Equal(404, response.StatusCode);
    }

    [Fact]
    public void FilePlugin_returns_200()
    {
        Request request = RequestCreate("GET", "/wwwroot/csharp.png");
        Response response = (Response)new FilePlugin().Handle(request);
        Assert.Equal(200, response.StatusCode);
    }

    [Fact]
    public void NaviPlugin_Can_Handle()
    {
        Request request = RequestCreate("GET", "/streetSearch");
        Assert.Equal(1, new NaviPlugin().CanHandle(request));
    }

    [Fact]
    public void NaviPlugin_CanT_Handle()
    {
        Request request = RequestCreate("GET", "/searchStreet");
        Assert.Equal(0, new NaviPlugin().CanHandle(request));
    }

    [Fact]
    public void NaviPlugin_returns_200()
    {
        Request request = RequestCreate("GET", "/streetSearch");
        Response response = (Response)new NaviPlugin().Handle(request);
        Assert.Equal(200, response.StatusCode);
    }

    [Fact]
    public void NaviPlugin_returns_html()
    {
        Request request = RequestCreate("GET", "/streetSearch?city=&postcode=&street=");
        Response response = (Response)new NaviPlugin().Handle(request);
        Assert.Equal(true, response.ContentString.Contains("<html>"));
        KeyValuePair<string, string> content = new KeyValuePair<string, string>(Strings.HTTP.HEADER_CONTENT_TYPE, "text/html");
        Assert.True(response.Headers.Contains(content));
    }

    [Fact]
    public void NaviPlugin_no_data_found()
    {
        Request request = RequestCreate("GET", "/streetSearch?city=NotSuchCity&postcode=&street=");
        Response response = (Response)new NaviPlugin().Handle(request);
        Assert.True(response.ContentString.Contains("No results found!"));
    }

    [Fact]
    public void TempPlugin_Can_Handle_REST()
    {
        Request request = RequestCreate("GET", "/GetTemperature/2019/1/1");
        Assert.Equal(1, new TempPlugin().CanHandle(request));
    }
    [Fact]
    public void TempPlugin_Can_Handle()
    {
        Request request = RequestCreate("GET", "/GetTemperature?year=2019&month=1&day=1");
        Assert.Equal(1, new TempPlugin().CanHandle(request));
    }

    [Fact]
    public void TempPlugin_CanT_Handle()
    {
        Request request = RequestCreate("GET", "/GetTemperatures");
        Assert.Equal(0, new TempPlugin().CanHandle(request));
    }

    [Fact]
    public void TempPlugin_returns_200()
    {
        Request request = RequestCreate("GET", "/GetTemperature/2019/09/20");
        Response response = (Response)new TempPlugin().Handle(request);
        Assert.Equal(200, response.StatusCode);
    }

    [Fact]
    public void TempPlugin_GeneratesData_200()
    {
        Request request = RequestCreate("GET", "/GetTemperature/generateData");
        Response response = (Response)new TempPlugin().Handle(request);
        Assert.Equal("Data Generated", response.ContentString);
    }

    [Fact]
    public void TempPlugin_no_data_found()
    {
        Request request = RequestCreate("GET", "/GetTemperature?year=2000&month=1&day=1");
        Response response = (Response)new TempPlugin().Handle(request);
        Assert.True(response.ContentString.Contains("No results found!"));
    }

    [Fact]
    public void TempPlugin_returns_xml()
    {
        Request request = RequestCreate("GET", "/GetTemperature/2019/1/1");
        Response response = (Response)new TempPlugin().Handle(request);
        Assert.Equal(true, response.ContentString.Contains("<?xml version=\"1.0\" encoding=\"utf-8\"?>"));
        KeyValuePair<string, string> content = new KeyValuePair<string, string>(Strings.HTTP.HEADER_CONTENT_TYPE, "text/xml");
        Assert.True(response.Headers.Contains(content));
    }

    [Fact]
    public void TempPlugin_returns_html()
    {
        Request request = RequestCreate("GET", "/GetTemperature?year=2019&month=1&day=1");
        Response response = (Response)new TempPlugin().Handle(request);
        Assert.Equal(true, response.ContentString.Contains("<html>"));
        KeyValuePair<string, string> content = new KeyValuePair<string, string>(Strings.HTTP.HEADER_CONTENT_TYPE, "text/html");
        Assert.True(response.Headers.Contains(content));
    }

    [Fact]
    public void ToLowPlugin_Can_Handle()
    {
        Request request = RequestCreate("GET", "/tolow?text=TeXt");
        Assert.Equal(1, new ToLowPlugin().CanHandle(request));
    }

    [Fact]
    public void ToLowPlugin_CanT_Handle()
    {
        Request request = RequestCreate("GET", "/tolo");
        Assert.Equal(0, new ToLowPlugin().CanHandle(request));
    }
    
}