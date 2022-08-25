using RSS_Fider.Models;
using System.Xml;
using System.Net;
using System.ServiceModel.Syndication;
using RSS_Fider.Configuration;

namespace RSS_Fider.Rss
{
    public class RssHandler
    {
        public static List<string>? GetListOfUrls(IConfiguration configuration)
        {
            //Список-контейнер для адресов включенных пользователем Rss-лент
            List<string> RssFeed_Urls = new List<string>();

            //Привязка конфигурации к объекту FeedsConfiguration (Только секцию с настройками лент)
            //FeedsConfiguration feedsConfiguration = configuration.GetSection("Feeds").Get<FeedsConfiguration>();

            Config config = new Config();
            configuration.Bind(config);

            //Проверка состояния ленты. Если лента включена - нужно добавить ее адрес в список опроса
            for (int i = 0; i < config.Feeds.Feeds_Addresses.Url.Count; i++)
            {
                if (config.Feeds.Feeds_States.Enable[i] == "true") RssFeed_Urls.Add(config.Feeds.Feeds_Addresses.Url[i]);
            }

            return RssFeed_Urls;
        }


        public static async Task<List<RssItem>> GetRssContentHttpClient(IConfiguration configuration, List<string> RssFeed_Urls)
        {
            WebProxy proxy;
            HttpClientHandler httpClientHandler;
            HttpClient client;

            //Привязка конфигурации к объекту ProxySettings (Только секцию с настройками прокси)


            Config config = new Config();
            configuration.Bind(config);

            //Если задан адрес для прокси, то осуществляется подключение по указанному адресу с заданными учетными данными
            if (config.ProxyConfiguration.Address != "")
            {
                //Определение параметров для прокси: данные покдлючения (IP,порт), учетные данные(логин, пароль)
                proxy = new WebProxy(config.ProxyConfiguration.Address, false);
                proxy.Credentials = new NetworkCredential(config.ProxyConfiguration.Login, config.ProxyConfiguration.Password);

                //Создание обработчика HttpCLientHandler для HttpClient и передача ему данных о ранее сконфигурированном WebProxy
                httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                };

                //Создание клиента, привязка к нему обработчика и соответственно прокси
                client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
            }
            else
            {
                //Подключение без прокси
                client = new HttpClient();
            }




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
