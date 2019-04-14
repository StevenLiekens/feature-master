using System;

namespace FeatureMaster
{
    public abstract class Feature : ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
