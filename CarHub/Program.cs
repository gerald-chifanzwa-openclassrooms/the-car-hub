using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace CarHub
{
    public class Program
    {
        private const string LogTemplate = "[{Timestamp:HH:mm:ss, MMM dd yyyy} {Level:u3}]-{SourceContext}{NewLine}{Message:lj} {Exception}{NewLine}";
        public static void Main(string [] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string [] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog((context, config) =>
                    {
                        config
                           .ReadFrom
                           .Configuration(context.Configuration, "Logging")
                           .Enrich.FromLogContext()
                           .WriteTo
                           .Console(theme: AnsiConsoleTheme.Code, outputTemplate: LogTemplate);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }

}
