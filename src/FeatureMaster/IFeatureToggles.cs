namespace FeatureMaster
{
    public interface IFeatureToggles<TFeature>
        where TFeature : Feature
    {
        TFeature GetFeatureToggles();

        TFeature GetFeatureToggles<TToggleContext>(TToggleContext context = default);
    }
}
