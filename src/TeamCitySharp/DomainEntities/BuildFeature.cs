namespace TeamCitySharp.DomainEntities
{
    public class BuildFeature
    {
        public BuildFeature()
        {
            Properties = new Properties();
        }

        public override string ToString()
        {
            return Type;
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public Properties Properties { get; set; }
    }
}