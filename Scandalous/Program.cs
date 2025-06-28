using Microsoft.Extensions.DependencyInjection;
using Scandalous.Core.Services;

namespace ScanUtility
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            // Set up dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            
            using var serviceProvider = services.BuildServiceProvider();
            var formScan = serviceProvider.GetRequiredService<FormScan>();
            
            Application.Run(formScan);
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            // Register services
            services.AddSingleton<IDocumentScanner, DocumentScanner>();
            services.AddSingleton<IConfigurationManager, ConfigurationManager>();
            
            // Register the main form
            services.AddTransient<FormScan>();
        }
    }
}