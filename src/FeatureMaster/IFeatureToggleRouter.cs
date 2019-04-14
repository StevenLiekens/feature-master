namespace FeatureMaster
{
    public interface IFeatureToggleRouter<TFeature, TToggleContext>
    {
        void Toggle(TFeature feature, TToggleContext context);
    }
}
