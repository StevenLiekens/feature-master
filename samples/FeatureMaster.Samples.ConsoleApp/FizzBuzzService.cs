using System;
using System.Threading;
using System.Threading.Tasks;
using FeatureMaster.Samples.ConsoleApp.Features;
using Microsoft.Extensions.Hosting;

namespace FeatureMaster.Samples.ConsoleApp
{
    public class FizzBuzzService : IHostedService, IDisposable
    {
        private readonly IFeatureToggles<FizzBuzzFeature> _featureToggles;

        private Timer _timer;

        public FizzBuzzService(IFeatureToggles<FizzBuzzFeature> featureToggles)
        {
            _featureToggles = featureToggles;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var context = new FizzBuzzContext
            {
                Count = 1
            };
            _timer = new Timer(state => PrintNext((FizzBuzzContext)state), context, 1000, 1000);
            return Task.CompletedTask;
        }

        private void PrintNext(FizzBuzzContext context)
        {
            var feature = _featureToggles.GetFeatureToggles(context);
            if (feature.UseNewColorScheme)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            } else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            bool fizz, buzz;
            if (fizz = context.Count % 3 == 0)
            {                
                Console.Write("Fizz");
            }
            if (buzz = context.Count % 5 == 0)
            {
                Console.Write("Buzz");
            }
            if (!fizz && !buzz)
            {
                Console.Write(context.Count);
            }
            Console.WriteLine();
            context.Count++;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
