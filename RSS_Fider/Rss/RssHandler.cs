using RSS_Fider.Models;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using RSS_Fider.Proxy;

namespace RSS_Fider.Rss
{
    public class RssHandler
    {
        public static List<string>? GetListOfUrls(IConfiguration configuration)
        {
            List<string> RssFeed_Urls = new List<string>();
            FeedsConfiguration feedsConfiguration = configuration.GetSection("Feeds").Get<FeedsConfiguration>();
            
            for (int i=0;i<feedsConfiguration.Feeds_Addresses.Url.Count;i++)
            {
                if (feedsConfiguration.Feeds_States.Enable[i] == "true") RssFeed_Urls.Add(feedsConfiguration.Feeds_Addresses.Url[i]);
            }


            return RssFeed_Urls;
        }



        public static async Task<List<RssItem>> GetRssContentHttpClient(IConfiguration configuration, List<string> RssFeed_Urls)
        {
            ProxySettings proxySettings = configuration.GetSection("ProxySettings").Get<ProxySettings>();

            //Задаем параметры для прокси: данные покдлючения (IP,порт), учетные данные(логин, пароль)
            WebProxy proxy = new WebProxy(proxySettings.Address, false);                                    //Правильный адрес - "94.158.189.78:50207
            proxy.Credentials = new NetworkCredential(proxySettings.Login, proxySettings.Password);                              //Правильные логин/пароль "8fu9jxas", "bxnbWr4V"

            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
            };
            //Создаем клиент и привязываем к нему прокси
            HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true);

            List<RssItem> Rss_Items = new List<RssItem>();

            foreach (string RssFeed_Url in RssFeed_Urls)
            {
                //Получаем Stream с указанного адреса
                Stream stream = await client.GetStreamAsync(RssFeed_Url);             

                XmlReader FeedReader = XmlReader.Create(stream);

                // Загружаем RSS
                SyndicationFeed Channel = SyndicationFeed.Load(FeedReader);

                // если загрузились
                if (Channel != null)
                {
                    // обрабатываем каждую новость канала
                    foreach (SyndicationItem RSI in Channel.Items)
                    {
                        DateTimeOffset PublishTime_ForClient;

                        //Получаем смещение в текущем часовом поясе
                        TimeSpan clientOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);

                        try
                        {
                            DateTimeOffset PublishTime_RSS = RSI.PublishDate;
                            PublishTime_ForClient = PublishTime_RSS.ToOffset(clientOffset);
                        }
                        catch (FormatException)
                        {
                            PublishTime_ForClient = DateTimeOffset.MinValue;

                        }

                        Rss_Items.Add(new RssItem(RSI.Title.Text, RSI.Id, RSI.Summary.Text, PublishTime_ForClient.DateTime.ToString()));
                    }
                }
            }
      
            return Rss_Items;
        }






      
    }
}
