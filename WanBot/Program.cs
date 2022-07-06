// See https://aka.ms/new-console-template for more information
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Payload;

Console.WriteLine("Hello, World!");

var httpAdapter = new HttpAdapter("http://localhost:8080", "INITKEYaQAgQquN", 3360745829);

var about1 = await httpAdapter.SendAsync<AboutResponse, AboutRequest>(new AboutRequest());

var wsAdapter = new WebSocketAdapter("ws://localhost:8080", "INITKEYaQAgQquN", 3360745829);

var about2 = await wsAdapter.SendAsync<AboutResponse, AboutRequest>(new AboutRequest());

wsAdapter.Dispose();

wsAdapter = new WebSocketAdapter("ws://localhost:8080", "INITKEYaQAgQquN", 3360745829);
var about3 = await wsAdapter.SendAsync<AboutResponse, AboutRequest>(new AboutRequest());

return 0;