namespace TeamCitySharp.DomainEntities
{
    public class PropertyType
    {
        public PropertyType()
        {
        }

        public PropertyType(string rawValue)
        {
            RawValue = rawValue;
        }

        public override string ToString()
        {
            return RawValue;
        }

        public string RawValue { get; set; }
    }
}