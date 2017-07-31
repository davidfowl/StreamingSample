using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream().Wait();
        }

        static async Task Stream()
        {
            var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, 5000);

            using (var stream = client.GetStream())
            {
                // Send the request
                var sw = new StreamWriter(stream);
                var sr = new StreamReader(stream);

                await sw.WriteAsync($"POST /api/streaming HTTP/1.1\r\n");
                await sw.WriteAsync("Transfer-Encoding: chunked\r\n");
                await sw.WriteAsync("Host: localhost: 5000\r\n");
                await sw.WriteAsync("\r\n");
                await sw.FlushAsync();

                var sendTask = GenerateNumbers(stream);

                // Consume the headers and response body
                await ConsumeResponseLineAndHeadersAsync(sr);

                var receiveTask = Receive(stream);

                await Task.WhenAll(sendTask, receiveTask);
            }
        }

        private static async Task ConsumeResponseLineAndHeadersAsync(StreamReader sr)
        {
            var responseLine = await sr.ReadLineAsync();
            while (true)
            {
                var header = await sr.ReadLineAsync();
                if (string.IsNullOrEmpty(header))
                {
                    break;
                }
            }
        }

        static async Task Receive(Stream stream)
        {
            var sr = new StreamReader(stream);

            while (!sr.EndOfStream)
            {
                Console.WriteLine("received: " + await sr.ReadLineAsync());
            }
        }

        static async Task GenerateNumbers(Stream stream)
        {
            var sw = new StreamWriter(stream);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"sending '{i}'");
                var hexLength = (sw.Encoding.GetByteCount(i.ToString()) + 2).ToString("x");
                await sw.WriteAsync(hexLength);
                await sw.WriteAsync("\r\n");
                await sw.WriteAsync(i.ToString());
                await sw.WriteAsync("\r\n");
                await sw.WriteAsync("\r\n");
                await sw.FlushAsync();
                await Task.Delay(500);
            }

            await sw.WriteAsync("0");
            await sw.WriteAsync("\r\n");
            await sw.WriteAsync("\r\n");
            await sw.FlushAsync();
        }
    }
}