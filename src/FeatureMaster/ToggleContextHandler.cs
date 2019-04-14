namespace FeatureMaster
{
    /// <summary>
    /// A function that makes toggling decisions based on a feature object and a context object.
    /// </summary>
    public delegate void ToggleContextHandler<TFeature, TContext>(TFeature feature, TContext context = default);
}