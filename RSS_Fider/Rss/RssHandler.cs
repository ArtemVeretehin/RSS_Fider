using RSS_Fider.Models;
using System.Xml;
using System.Net;
using System.ServiceModel.Syndication;
using RSS_Fider.Proxy;

namespace RSS_Fider.Rss
{
    public class RssHandler
    {
        public static List<string>? GetListOfUrls(IConfiguration configuration)
        {
            //Список-контейнер для адресов включенных пользователем Rss-лент
            List<string> RssFeed_Urls = new List<string>();

            //Привязка конфигурации к объекту FeedsConfiguration (Только секцию с настройками лент)
            FeedsConfiguration feedsConfiguration = configuration.GetSection("Feeds").Get<FeedsConfiguration>();

            //Проверка состояния ленты. Если лента включена - нужно добавить ее адрес в список опроса
            for (int i = 0; i < feedsConfiguration.Feeds_Addresses.Url.Count; i++)
            {
                if (feedsConfiguration.Feeds_States.Enable[i] == "true") RssFeed_Urls.Add(feedsConfiguration.Feeds_Addresses.Url[i]);
            }

            return RssFeed_Urls;
        }


        public static async Task<List<RssItem>> GetRssContentHttpClient(IConfiguration configuration, List<string> RssFeed_Urls)
        {
            //Привязка конфигурации к объекту ProxySettings (Только секцию с настройками прокси)
            ProxyConfiguration proxyConfiguration = configuration.GetSection("ProxyConfiguration").Get<ProxyConfiguration>();

            //Определение параметров для прокси: данные покдлючения (IP,порт), учетные данные(логин, пароль)
            WebProxy proxy = new WebProxy(proxyConfiguration.Address, false);                                    
            proxy.Credentials = new NetworkCredential(proxyConfiguration.Login, proxyConfiguration.Password);    

            //Создание обработчика HttpCLientHandler для HttpClient и передача ему данных о ранее сконфигурированном WebProxy
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
            };

            //Создание клиента, привязка к нему обработчика и соответственно прокси
            HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true);

            //Список-контейнер для элементов Rss-ленты
            List<RssItem> Rss_Items = new List<RssItem>();

            //Для каждой включенной ленты
            foreach (string RssFeed_Url in RssFeed_Urls)
            {

                //Получение Stream с указанного адреса
                Stream stream = await client.GetStreamAsync(RssFeed_Url);
     


                //Создание XmlReader на основе полученного Stream
                XmlReader FeedReader = XmlReader.Create(stream);

                // Загрузка RSS
                SyndicationFeed Channel = SyndicationFeed.Load(FeedReader);

                // Если загрузка прошла успешно
                if (Channel != null)
                {
                    // Обработка всех новостей канала
                    foreach (SyndicationItem RSI in Channel.Items)
                    {

                        DateTimeOffset PublishTime_ForClient;
                        //Получение смещения в текущем часовом поясе (для текущей клиентской машины)
                        TimeSpan clientOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);

                        //Приведение времени ко времени используемому на клиентской машине
                        try
                        {
                            DateTimeOffset PublishTime_RSS = RSI.PublishDate;
                            PublishTime_ForClient = PublishTime_RSS.ToOffset(clientOffset);
                        }
                        catch (FormatException)
                        {
                            PublishTime_ForClient = DateTimeOffset.MinValue;
                        }

                        //Добавление элемента Rss в список
                        Rss_Items.Add(new RssItem(RSI.Title.Text, RSI.Links[0].Uri.ToString(), RSI.Summary.Text, PublishTime_ForClient.DateTime.ToString()));
                    }
                }
            }


            //Возврат полученного списка Rss-элементов
            return Rss_Items;
        }

    }
}
