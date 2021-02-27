using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await StreamWithHttpClient();
        }

        static async Task StreamWithHttpClient()
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://localhost:5001/api/streaming"));
            request.Version = new Version(2, 0);
            request.Headers.TransferEncodingChunked = true;
            request.Content = new PostStreamContent(GenerateNumbersNoManualChunking);
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            // Send the request
            var sr = new StreamReader(stream);

            // Consume the headers and response body
            await Receive(sr);
        }

        private static async Task Receive(StreamReader sr)
        {
            while (true)
            {
                var line = await sr.ReadLineAsync();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                Console.WriteLine("received: " + line);
            }
        }

        static async Task GenerateNumbersNoManualChunking(Stream stream)
        {
            await stream.FlushAsync();

            var sw = new StreamWriter(stream);

            for (int i = 0; ; i++)
            {
                Console.WriteLine($"sending '{i}'");
                await sw.WriteLineAsync(i.ToString());
                await sw.FlushAsync();
                await stream.FlushAsync();
                await Task.Delay(500);
            }
        }
    }
}