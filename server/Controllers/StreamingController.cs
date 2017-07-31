using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace server.Controllers
{
    [Route("api/streaming")]
    public class StreamingController : Controller
    {
        [HttpGet]
        public string Hello() => "hello world! Streaming controller here!";

        [HttpPost]
        public async Task Post()
        {
            Console.WriteLine("connected");
            using (var sr = new StreamReader(Request.Body))
            using (var sw = new StreamWriter(Response.Body))
            {
                Console.WriteLine(HttpContext.RequestAborted.IsCancellationRequested ? "END OF STREAM" : "ok");
                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    var line = await sr.ReadLineAsync();
                    Console.WriteLine($"received: '{line}'");

                    await sw.WriteLineAsync((int.Parse(line) * 2).ToString());
                    await sw.FlushAsync();
                }

                for (int i = 100; i < 110; i++)
                {
                    Console.WriteLine("sending: " + i);
                    await sw.WriteLineAsync(i.ToString());
                    await sw.FlushAsync();
                    await Task.Delay(1000);
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
