using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Dash
{
    public class Program
    {
        public static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        
        public static async Task Main(string[] args)
        {
            await CreateWebHostBuilder(args)
                .Build()
                .RunAsync(Cts.Token);
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