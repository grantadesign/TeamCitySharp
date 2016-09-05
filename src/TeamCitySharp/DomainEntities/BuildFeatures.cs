using System.Collections.Generic;

namespace TeamCitySharp.DomainEntities
{
    public class BuildFeatures
    {
        public override string ToString()
        {
            return "features";
        }

        public List<BuildFeature> Feature { get; set; }
    }
}