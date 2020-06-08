using Xunit;
using myWebServer;
using System.IO;
using System.Text;


public class RequestTests
{
    private string method = "POST";
    private string url = "/test";
    private string host = "localhost:8080";
    byte[] bodyBytes = { };
    string content = "";
    Stream stream;

    public Request RequestCreate()
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms, Encoding.ASCII);

        sw.WriteLine("{0} {1} HTTP/1.1", method, url);
        sw.WriteLine("Host: {0}", host);
        sw.WriteLine("Connection: keep-alive");
        sw.WriteLine("Accept: text/html,application/xhtml+xml");
        sw.WriteLine("User-Agent: Unit-Test-Agent/1.0 (The OS)");
        sw.WriteLine("Accept-Encoding: gzip,deflate,sdch");
        sw.WriteLine("Accept-Language: de-AT,de;q=0.8,en-US;q=0.6,en;q=0.4");
        sw.WriteLine("Content-Type: application/x-www-form-urlencoded");


        content = "<!DOCTYPE html><html><body><h1>SWE</h1></body></html>";
        bodyBytes = Encoding.UTF8.GetBytes(content);
        sw.WriteLine($"Content-Length: {bodyBytes.Length}");
        sw.WriteLine();
        sw.Flush();

        ms.Write(bodyBytes, 0, bodyBytes.Length);
        sw.Flush();
        ms.Seek(0, SeekOrigin.Begin);
        stream = new MemoryStream(bodyBytes);
        return new Request(ms);
    }

    [Fact]
    public void request_check_method()
    {
        Request request = RequestCreate();
        Assert.Equal(method, request.Method);
    }
    [Fact]
    public void request_header_not_null()
    {
        Request request = RequestCreate();
        Assert.NotNull(request.Headers);
    }

    [Fact]
    public void request_header_count_not_zero()
    {
        Request request = RequestCreate();
        Assert.NotEqual(0, request.HeaderCount);
    }

    [Fact]
    public void request_header_has_connection()
    {
        Request request = RequestCreate();
        Assert.NotNull(request.Headers);
        Assert.True(request.Headers.ContainsKey(Strings.HTTP.HEADER_CONNECTION.ToLower()));
        Assert.Equal("keep-alive", request.Headers[Strings.HTTP.HEADER_CONNECTION.ToLower()]);
    }

    [Fact]
    public void request_header_has_user_agent()
    {
        Request request = RequestCreate();
        Assert.NotNull(request.Headers);
        Assert.True(request.Headers.ContainsKey(Strings.HTTP.HEADER_USER_AGENT.ToLower()));
        Assert.Equal("Unit-Test-Agent/1.0 (The OS)", request.Headers[Strings.HTTP.HEADER_USER_AGENT.ToLower()]);
    }

    [Fact]
    public void request_header_has_accept()
    {
        Request request = RequestCreate();
        Assert.NotNull(request.Headers);
        Assert.True(request.Headers.ContainsKey(Strings.HTTP.HEADER_ACCEPT.ToLower()));
        Assert.Equal("text/html,application/xhtml+xml", request.Headers[Strings.HTTP.HEADER_ACCEPT.ToLower()]);
    }

    [Fact]
    public void request_header_has_accept_encoding()
    {
        Request request = RequestCreate();
        Assert.NotNull(request.Headers);
        Assert.True(request.Headers.ContainsKey(Strings.HTTP.HEADER_ACCEPT_ENCODING.ToLower()));
        Assert.Equal("gzip,deflate,sdch", request.Headers[Strings.HTTP.HEADER_ACCEPT_ENCODING.ToLower()]);
    }

    [Fact]
    public void request_header_has_accept_language()
    {
        Request request = RequestCreate();
        Assert.NotNull(request.Headers);
        Assert.True(request.Headers.ContainsKey(Strings.HTTP.HEADER_ACCEPT_LANGUAGE.ToLower()));
        Assert.Equal("de-AT,de;q=0.8,en-US;q=0.6,en;q=0.4", request.Headers[Strings.HTTP.HEADER_ACCEPT_LANGUAGE.ToLower()]);
    }

    [Fact]
    public void request_header_content_type()
    {
        Request request = RequestCreate();
        Assert.True(request.Headers.ContainsKey(Strings.HTTP.HEADER_CONTENT_TYPE.ToLower()));
        Assert.Equal("application/x-www-form-urlencoded", request.Headers[Strings.HTTP.HEADER_CONTENT_TYPE.ToLower()]);
    }

    [Fact]
    public void request_header_content_length()
    {
        Request request = RequestCreate();
        Assert.True(request.Headers.ContainsKey(Strings.HTTP.HEADER_CONTENT_LENGTH.ToLower()));
        Assert.Equal(bodyBytes.Length.ToString(), request.Headers[Strings.HTTP.HEADER_CONTENT_LENGTH.ToLower()]);
    }
    [Fact]
    public void request_check_content_string()
    {
        Request request = RequestCreate();
        Assert.Equal(content, request.ContentString);
    }

    [Fact]
    public void request_check_content_byte()
    {
        Request request = RequestCreate();
        Assert.Equal(bodyBytes, request.ContentBytes);
    }

    [Fact]
    public void request_chech_content_stream()
    {
        Request request = RequestCreate();
        // Assert.Equal(stream, request.ContentStream);
    }
}