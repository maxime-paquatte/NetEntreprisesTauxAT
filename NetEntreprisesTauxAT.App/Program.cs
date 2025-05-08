using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetEntreprisesTauxAT.App.Services;

namespace NetEntreprisesTauxAT.App;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(params string[] args)
    {
        ApplicationConfiguration.Initialize();
        
        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddCommandLine(args);
            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        });
        builder.ConfigureLogging((context, loggingBuilder) =>
        {
            loggingBuilder.AddConfiguration(context.Configuration);
            loggingBuilder.AddConsole();
        });
        builder.ConfigureServices((context, services) =>
        {
            services.AddTransient<MainForm>();
            services.AddTransient<TauxAtService>();
            services.Configure<TauxAtService.Options>(o => context.Configuration.GetSection("NetEntreprisesTauxAT:Service").Bind(o));
        });
        
        var host = builder.Build();

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }

}