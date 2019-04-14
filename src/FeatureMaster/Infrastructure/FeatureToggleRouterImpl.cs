namespace FeatureMaster.Infrastructure
{
    internal sealed class FeatureToggleRouterImpl<TFeature, TToggleContext> : IFeatureToggleRouter<TFeature, TToggleContext>
    {
        private readonly ToggleContextHandler<TFeature, TToggleContext> _handler;

        internal FeatureToggleRouterImpl(ToggleContextHandler<TFeature, TToggleContext> handler)
        {
            _handler = handler;
        }

        public void Toggle(TFeature feature, TToggleContext context)
        {
            _handler(feature, context);
        }
    }
}
