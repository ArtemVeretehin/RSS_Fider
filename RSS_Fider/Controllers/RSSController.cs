using Microsoft.AspNetCore.Mvc;
using RSS_Fider.Rss;
using System.Net;
using RSS_Fider.Models;

namespace RSS_Fider.Controllers
{
    public class RSSController : Controller
    {
        //Метод для обновления содержимого страницы, недоступен для пользователя
        [HttpPost]
        public async Task<JsonResult> RssIndex([FromServices] IConfiguration configuration)
        {
            List<string>? RssFeed_Urls = RssHandler.GetListOfUrls(configuration);

            List<RssItem> list = new List<RssItem>();

            if (RssFeed_Urls?.Count > 0)
            {
                list = await RssHandler.GetRssContentHttpClient(configuration, RssFeed_Urls);
            }
            list.Sort((b1, b2) => string.Compare(b2.PublishDate, b1.PublishDate));
            JsonResult obj = Json(list);
            return Json(list);
        }


        //Получение представления
        public IActionResult RssRefresh([FromServices] IConfiguration configuration)
        {
            return PartialView();
        }

        public IActionResult FeedsConfig([FromServices] IConfiguration configuration)
        {
            FeedsConfiguration feedsConfiguration = configuration.GetSection("Feeds").Get<FeedsConfiguration>();
            return View(feedsConfiguration);
        }

        [HttpPost]
        public void RssFormat([FromServices] IConfiguration configuration)
        {
            if (configuration["DescriptionFormating:Enable"] == "false") configuration["DescriptionFormating:Enable"] = "true";
            if (configuration["DescriptionFormating:Enable"] == "true") configuration["DescriptionFormating:Enable"] = "false";
        }

        [HttpPost]
        public void RssChangeUpdateTime([FromServices] IConfiguration configuration, string param)
        {

            configuration["UpdateTime:Value"] = param;
        }

        [HttpPost]
        public void RssChangeFeedState([FromServices] IConfiguration configuration, string FeedId)
        {
            if (configuration[$"Feeds:Feeds_States:Enable:{FeedId}"] == "false") configuration[$"Feeds:Feeds_States:Enable:{FeedId}"] = "true";
            if (configuration[$"Feeds:Feeds_States:Enable:{FeedId}"] == "true") configuration[$"Feeds:Feeds_States:Enable:{FeedId}"] = "false";

        }

        [HttpPost]
        public void RssChangeFeedUrl([FromServices] IConfiguration configuration, string FeedId, string newUrl)
        {
            configuration[$"Feeds:Feeds_Addresses:Url:{FeedId}"] = newUrl;
        }


    }
}