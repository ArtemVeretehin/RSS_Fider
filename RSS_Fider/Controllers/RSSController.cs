using Microsoft.AspNetCore.Mvc;
using RSS_Fider.Rss;
using System.Net;

namespace RSS_Fider.Controllers
{
    public class RSSController : Controller
    {
        //Получение представления
        public IActionResult RssStart()
        {
            string? RssFeed_URL = RssHandler.GetUrl();

            if (RssFeed_URL is not null)
            {
                ViewBag.ListItems = RssHandler.GetRssContent(RssFeed_URL);
            }
            return View();
        }

        //Метод для обновления содержимого страницы, недоступен для пользователя
        [HttpPost]
        public async Task<JsonResult> RssIndex([FromServices] IConfiguration configuration)
        {
            string? RssFeed_URL = RssHandler.GetUrl();
            List<Models.RssItem> list = new List<Models.RssItem>();

            if (RssFeed_URL is not null)
            {
                list = await RssHandler.GetRssContentHttpClient(configuration,RssFeed_URL);
                //list = RssHandler.GetRssContent(RssFeed_URL);
            }
            JsonResult obj = Json(list);
            return Json(list);
        }

        //Получение представления
        public IActionResult RssRefresh([FromServices] IConfiguration configuration)
        {
            return PartialView();
        }

    }
}