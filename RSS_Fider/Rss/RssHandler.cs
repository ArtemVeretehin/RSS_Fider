using RSS_Fider.Models;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.ServiceModel.Syndication;

namespace RSS_Fider.Rss
{
    public class RssHandler
    {
        //static readonly HttpClient? client;
        public static List<RssItem> GetRssContent(string RssFeed_URL)
        {
            List<RssItem> Rss_Items = new List<RssItem>();

            // Создаем XmlReader для чтения RSS
            XmlReader FeedReader = XmlReader.Create(RssFeed_URL);

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

                    // создаем элемент для вывода в ListView                  
                    //Rss_Items.Add(new RssItem(RSI.Title.Text, RSI.Id, RSI.Summary.Text, PublishTime_ForClient.ToString()));


                    Rss_Items.Add(new RssItem(RSI.Title.Text, RSI.Id, RSI.Summary.Text, PublishTime_ForClient.DateTime.ToString()));
                }
            }
            return Rss_Items;
        }

        public static string? GetUrl()
        {
            string? RSS_URL = "";

            //Загружаем конфиг
            XDocument FeederConfig = XDocument.Load("config.xml");
            //Получаем тело конфига
            XElement? FeederSettings = FeederConfig.Element("configuration");

            //Если конфиг не пустой, то получаем URL RSS-ленты
            if (FeederSettings is not null)
            {
                XElement? FeedUrl_URL = FeederSettings.Element("FeedUrl")?.Element("FeedUrl_1");
                RSS_URL = FeedUrl_URL?.Value;
            }

            return RSS_URL;
        }


        public static async Task<List<RssItem>> GetRssContentHttpClient(IConfiguration configuration, string RssFeed_URL)
        {
            string proxy_address;
            string proxy_login;
            string proxy_password;
            //configuration["ProxySettings:Address"] = "123";
            proxy_address = configuration["ProxySettings:Address"];
            proxy_login = configuration["ProxySettings:Login"];
            proxy_password = configuration["ProxySettings:Password"];
            //Задаем параметры для прокси: данные покдлючения (IP,порт), учетные данные(логин, пароль)
            WebProxy proxy = new WebProxy(proxy_address, false);                                    //Правильный адрес - "94.158.189.78:50207
            proxy.Credentials = new NetworkCredential(proxy_login, proxy_password);                              //Правильные логин/пароль "8fu9jxas", "bxnbWr4V"

            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
            };
            //Создаем клиент и привязываем к нему прокси
            HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true);

            //Получаем Stream с указанного адреса
            Stream stream = await client.GetStreamAsync(RssFeed_URL);
            
            
            List<RssItem> Rss_Items = new List<RssItem>();

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
            return Rss_Items;
        }

    }
}
