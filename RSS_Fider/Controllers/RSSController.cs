using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using RSS_Fider.Rss;
using RSS_Fider.Models;
using RSS_Fider.Configuration;

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
                try
                { 
                    list = await RssHandler.GetRssContentHttpClient(configuration, RssFeed_Urls);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Возникло исключение: {ex.Message}");
                }
            }

            //Сортировка элементов Rss-ленты по дате добавления
            list.Sort((b1, b2) => string.Compare(b2.PublishDate, b1.PublishDate));

            //Формирование JSON на основе полученного списка
            return Json(list);
        }


        //Получение представления - основное окно, просмотр ленты
        public IActionResult RssRefresh([FromServices] IConfiguration configuration)
        {
            //Создание класса конфигурации и проекция параметров на него
            Config config = new Config();
            configuration.Bind(config);

            //Создание класса валидатора
            ConfigValidator validator = new ConfigValidator();

            //Получение результатов валидации
            ValidationResult results = validator.Validate(config);
            
            if (! results.IsValid)
            {
                Console.WriteLine("Параметры заданные в конфигурации не валидны.");
                Console.WriteLine("Проверьте настройки в файле конфигурации. Минимальное и текущее время обновления (MinUpdateTime,UpdateTime) должны быть указаны в формате целого числа, больше 0. " +
                    "Настройка параметра форматирования(DescriptionFormating) должна быть указана в формате true / false.");
                return View("Error");
            }
            return View();
        }


        //Получение представления - окно настроек ленты
        public IActionResult FeedsConfig([FromServices] IConfiguration configuration)
        {
            //Создание класса конфигурации и проекция параметров на него
            Config config = new Config();
            configuration.Bind(config);

            //Создание класса валидатора
            ConfigValidator validator = new ConfigValidator();

            //Получение результатов валидации
            ValidationResult results = validator.Validate(config);

            if (!results.IsValid)
            {
                Console.WriteLine("Параметры заданные в конфигурации не валидны.");
                Console.WriteLine("Проверьте настройки в файле конфигурации. Минимальное и текущее время обновления (MinUpdateTime,UpdateTime) должны быть указаны в формате целого числа, больше 0. " +
                    "Настройка параметра форматирования(DescriptionFormating) должна быть указана в формате true / false.");
                return View("Error");
            }

            return View(config);
        }


        //Изменение настроек вывода описания (с форматированием/без форматирования по тегам)
        [HttpPost]
        public void RssFormat([FromServices] IConfiguration configuration)
        {
            string FormatState = configuration["DescriptionFormating"];

            switch (FormatState)
            {
                case "false":
                    configuration["DescriptionFormating"] = "true";
                    break;
                case "true":
                    configuration["DescriptionFormating"] = "false";
                    break;
                default:
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
                default:
                    break;
            }
        }


        //Изменение периода обновления ленты
        [HttpPost]
        public void RssChangeUpdateTime([FromServices] IConfiguration configuration, string param)
        {
            configuration["UpdateTime"] = param;
        }

        
        //Изменение адреса выбранной ленты
        [HttpPost]
        public void RssChangeFeedUrl([FromServices] IConfiguration configuration, string FeedId, string newUrl)
        {
            configuration[$"Feeds:Feeds_Addresses:Url:{FeedId}"] = newUrl;
        }
    }
}