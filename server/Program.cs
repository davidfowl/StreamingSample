using System.IO;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var onWindowsServer =
               RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
               RuntimeInformation.OSDescription.Contains("Server");

            var host = new WebHostBuilder()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseIISIntegration()
               .UseStartup<Startup>();

            if (onWindowsServer)
            {
                host.UseWebListener();
            }
            else
            {
                host.UseKestrel();
            }

            host.Build().Run();
        }
    }
}
