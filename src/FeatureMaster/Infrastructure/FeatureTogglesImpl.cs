using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureMaster.Infrastructure
{
    public sealed class FeatureTogglesImpl<TFeature> : IFeatureToggles<TFeature>
        where TFeature : Feature, new()
    {
        private readonly IOptionsMonitor<TFeature> _options;

        private readonly IServiceProvider _sp;

        public FeatureTogglesImpl(IOptionsMonitor<TFeature> options, IServiceProvider sp)
        {
            _options = options;
            _sp = sp;
        }

        public TFeature GetFeatureToggles()
        {
            return _options.CurrentValue;
        }

        public TFeature GetFeatureToggles<TToggleContext>(TToggleContext context = default)
        {
            var toggles = (TFeature)GetFeatureToggles().Clone();
            var toggleRouter = _sp.GetService<IFeatureToggleRouter<TFeature, TToggleContext>>();
            if (toggleRouter is object)
            {
                toggleRouter.Toggle(toggles, context);
            }

            return toggles;
        }
    }
}
