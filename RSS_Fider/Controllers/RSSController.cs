using Microsoft.AspNetCore.Mvc;
using RSS_Fider.Rss;
using System.Net;
using RSS_Fider.Models;

namespace RSS_Fider.Controllers
{
    public class RSSController : Controller
    {
        //Обновление данных Rss-ленты с преобразованием в JSON, недоступен для пользователя
        [HttpPost]
        public async Task<JsonResult> RssIndex([FromServices] IConfiguration configuration)
        {
            //Список-контейнер для элементов Rss-ленты
            List<RssItem> list = new List<RssItem>();
            //Список-контейнер для адресов включенных пользователем Rss-лент
            List<string>? RssFeed_Urls = RssHandler.GetListOfUrls(configuration);
            
            //Получение элементов Rss-ленты
            if (RssFeed_Urls?.Count > 0)
            {
                list = await RssHandler.GetRssContentHttpClient(configuration, RssFeed_Urls);
            }
            //Сортировка элементов Rss-ленты по дате добавления
            list.Sort((b1, b2) => string.Compare(b2.PublishDate, b1.PublishDate));

            //Формирование JSON на основе полученного списка
            return Json(list);
        }


        //Получение представления - основное окно, просмотр ленты
        public IActionResult RssRefresh([FromServices] IConfiguration configuration)
        {
            return PartialView();
        }


        //Получение представления - окно настроек ленты
        public IActionResult FeedsConfig([FromServices] IConfiguration configuration)
        {
            FeedsConfiguration feedsConfiguration = configuration.GetSection("Feeds").Get<FeedsConfiguration>();
            return View(feedsConfiguration);
        }


        //Изменение настроек вывода описания (с форматированием/без форматирования по тегам)
        [HttpPost]
        public void RssFormat([FromServices] IConfiguration configuration)
        {
            string FormatState = configuration["DescriptionFormating:Enable"];

            switch (FormatState)
            {
                case "false":
                    configuration["DescriptionFormating:Enable"] = "true";
                    break;
                case "true":
                    configuration["DescriptionFormating:Enable"] = "false";
                    break;
            }
        }


        //Изменение состояния выбранной ленты (включена/выключена)
        [HttpPost]
        public void RssChangeFeedState([FromServices] IConfiguration configuration, string FeedId)
        {
            string FeedState = configuration[$"Feeds:Feeds_States:Enable:{FeedId}"];

            switch (FeedState)
            {
                case "false":
                    configuration[$"Feeds:Feeds_States:Enable:{FeedId}"] = "true";
                    break;
                case "true":
                    configuration[$"Feeds:Feeds_States:Enable:{FeedId}"] = "false";
                    break;
            }
        }


        //Изменение периода обновления ленты
        [HttpPost]
        public void RssChangeUpdateTime([FromServices] IConfiguration configuration, string param)
        {
            configuration["UpdateTime:Value"] = param;
        }

        
        //Изменение адреса выбранной ленты
        [HttpPost]
        public void RssChangeFeedUrl([FromServices] IConfiguration configuration, string FeedId, string newUrl)
        {
            configuration[$"Feeds:Feeds_Addresses:Url:{FeedId}"] = newUrl;
        }
    }
}