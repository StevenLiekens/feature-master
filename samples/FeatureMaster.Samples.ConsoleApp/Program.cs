using System.Threading.Tasks;
using FeatureMaster.Samples.ConsoleApp.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeatureMaster.Samples.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables("FIZZBUZZ_");
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;
                    config.AddJsonFile(@"appsettings.json", false, true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                    config.AddEnvironmentVariables();
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureFeatureToggles<FizzBuzzFeature>(context.Configuration.GetSection("Features:FizzBuzz").Bind)
                            .ConfigureFeatureToggleRouter<FizzBuzzFeature, FizzBuzzContext>((feature, fizzBuzzContext) =>
                            {
                                // Disable the new color scheme after 15 counts, regardless of configuration
                                // Consider it a trial version... send me money for the full version
                                if (fizzBuzzContext.Count > 15)
                                {
                                    feature.UseNewColorScheme = false;
                                }
                            });
                    services.AddHostedService<FizzBuzzService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
