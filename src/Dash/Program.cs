using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Dash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                //.ConfigureServices(x => x.AddTransient())
                .UseStartup<Startup>();
        }
    }
}