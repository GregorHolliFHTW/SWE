using Xunit;
using myWebServer;
using System.IO;
using System.Text;



public class ResponseTests
{
    public Response ResponseCreate()
    {
        var ms = new MemoryStream();
        Response res = new Response();
        res.StatusCode = 200;
        res.SetContent("{success}");
        res.StatusCode = 200;
        res.AddHeader(Strings.HTTP.HEADER_SERVER, "C# WebServer");
        res.AddHeader(Strings.HTTP.HEADER_CONTENT_LENGTH, res.ContentLength.ToString());
        res.AddHeader(Strings.HTTP.HEADER_CONTENT_TYPE, "text/html;charset=utf-8");
        res.AddHeader(Strings.HTTP.HEADER_CONTENT_CODING, "gzip");
        res.AddHeader(Strings.HTTP.HEADER_CONNECTION, "close");
        res.Send(ms);
        return res;
    }

    [Fact]
    public void response_status_code()
    {
        Response res = ResponseCreate();
        Assert.Equal("200 OK", res.Status);
    }
    [Fact]
    public void response_header_not_null()
    {
        Response res = ResponseCreate();
        Assert.NotNull(res.Headers);
    }

    [Fact]
    public void response_header_content_length()
    {
        Response res = ResponseCreate();
        string content = "{success}";
        Assert.Equal(System.Text.Encoding.UTF8.GetByteCount(content), res.ContentLength);
    }

    [Fact]
    public void response_header_content_type()
    {
        Response res = ResponseCreate();
        string contentType = "text/html;charset=utf-8";
        Assert.Equal(contentType, res.ContentType);
    }

    [Fact]
    public void response_header_server()
    {
        Response res = ResponseCreate();
        string serverHeader = "C# WebServer";
        Assert.Equal(serverHeader, res.ServerHeader);
    }

    [Fact]
    public void response_header_set_content_string()
    {
        Response res = new Response();
        string content = "<!DOCTYPE html><html><body><h1>SWE</h1></body></html>";
        res.SetContent(content);
        Assert.Equal(content, res.ContentString);
    }

    [Fact]
    public void response_header_set_content_byte()
    {
        Response res = new Response();
        string content = "<!DOCTYPE html><html><body><h1>SWE</h1></body></html>";
        byte[] contentByte = System.Text.Encoding.UTF8.GetBytes(content);
        res.SetContent(contentByte);
        Assert.Equal(content, res.ContentString);
    }

    [Fact]
    public void response_header_set_content_type()
    {
        Response res = new Response();
        string contentType = "text/html";
        res.ContentType = contentType;
        Assert.Equal(contentType, res.ContentType);
    }

    [Fact]
    public void response_add_header()
    {
        Response res = new Response();
        res.AddHeader(Strings.HTTP.HEADER_SERVER, "C# WebServer");
        Assert.True(res.Headers.ContainsKey(Strings.HTTP.HEADER_SERVER));
        Assert.Equal("C# WebServer", res.Headers[Strings.HTTP.HEADER_SERVER]);
    }
}