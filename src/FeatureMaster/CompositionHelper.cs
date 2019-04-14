using System;
using FeatureMaster.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureMaster
{
    public static class CompositionHelper
    {
        public static IServiceCollection ConfigureFeatureToggles<TFeature>(this IServiceCollection services, Action<TFeature> configureDefaults)
            where TFeature : Feature, new()
        {
            return services.Configure(configureDefaults)
                           .AddSingleton<IFeatureToggles<TFeature>, FeatureTogglesImpl<TFeature>>();
        }

        public static IServiceCollection ConfigureFeatureToggleRouter<TFeature, TToggleContext>(this IServiceCollection services, ToggleContextHandler<TFeature, TToggleContext> toggleRouter)
            where TFeature : Feature
        {
            return services.AddTransient<IFeatureToggleRouter<TFeature, TToggleContext>>(_ => new FeatureToggleRouterImpl<TFeature, TToggleContext>(toggleRouter));
        }
    }
}
