namespace RSS_Fider.Models
{
    public class FeedsConfiguration
    {
        public Feeds_Addresses Feeds_Addresses { get; set; }
        public Feeds_States Feeds_States { get; set; }
    }

    public class Feeds_Addresses
    {
        public List<string> Url { get; set; }

    }

    public class Feeds_States
    {
        public List<string> Enable { get; set; }
    }
}
