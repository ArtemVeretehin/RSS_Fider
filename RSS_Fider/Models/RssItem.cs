namespace RSS_Fider.Models
{
    public class RssItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PublishDate { get; set; }

        public RssItem(string Title,string Link, string Description, string PublishDate)
        {
            this.Title = Title;
            this.Link = Link;
            this.Description = Description;
            this.PublishDate = PublishDate;
        }

    }
}
