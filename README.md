# SWE Project Holli

## Client and Server

This server listens to http://localhost:8080/?

The Client is established via a web interface, it calls up the relevant index.html and home.js files, which will allow graphical interaction with the plugin functionalities via REST API. Simply put the Client code handles the request and notifies the necessary plugins.

Subsequent requests to the plugins will, if necessary, generate and deliver a new html page by the plugin.

The Server adds the plugins to the plugin manager, listens to port 8080 and establishes a connection to the database. If the client connects, a new thread is  started and the client is handled.



## Request and Response

Both are implemented using their respective interfaces.

The request constructor requires a Stream, through several methods, this Stream is then dissected. 

private void getFirstLine() - splits the request to extract the tokens (f.e. GET / HTTP / 1.1) and then constructs a new URL

private void checkIfIsValid() - confirms that one of the tokens(method) is GET or POST 

private void extractHeaders() - reads and stores the request header

private void getPostContent() - if a POST method occursm the content is extracted



The response contains among other methods the ability to add and create headers as well as the status codes and the ability to return these status codes as a string.



## Pluginmanager and IPlugin

The plugin manager simply contains a list of plugins which may be added or cleared. It also returns a singleton instance of the plugin manager ensuring only one plugin manager at a time may be initiated and used.

Every base plugin must contain two methods:

float CanHandle(IRequest req); - returns a value of 0 or 1 depending on, if the relevant parameter is contained within a requests segements.

IResponse Handle(IRequest req); - Called by the server when the plugin should handle the request.



## FilePlugin

Helps to navigate to the directory of the html file and determine file extensions. In essence it returns the requested file, if the url path is '/' then index.html is returned. An additional handleExt() method allows for certain file extensions to be recognized. 



## NaviPlugin

It is possible to search the osm data which is loaded into a database via chosen street, postcode or city. Depending on which parameters are given a relevant SQL command is constructed and executed to select the wanted data from the database. This information is then returned via a streamwriter variable, in which a new html page for the results is constructed.



## TempPlugin

Temperatures may be generated and stored in the databse at will and can be called up and even navigated via "next" and "prev", which in turn changes the Parameter in the request. If the search does not find a result, "No result found" will be the output.

Additionally it is possible to display the search result as XML by using the URL without using parameters.



## ToLowPlugin

The resulting lowercase string will be returned via a Javascript alert.

