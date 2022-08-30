using FluentValidation;

namespace RSS_Fider.Configuration
{
    //Ниже представлен полный класс конфигурации, на который производится проекция параметров из файла конфигурации,набор классов-элементов конфигурации, класс-валидатор
    public class Config
    {
        public string MinUpdateTime { get; set; }
        public string UpdateTime { get; set; }
        public string DescriptionFormating { get; set; }
        public Feeds Feeds { get; set; }
        public ProxyConfiguration ProxyConfiguration { get; set; }
    }

    public class Feeds
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

    public class ProxyConfiguration
    {
        public string Address { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class ConfigValidator : AbstractValidator<Config>
    {
        public ConfigValidator()
        {
            RuleFor(config => config.UpdateTime).NotEmpty().Matches(@"^[1-9]\d*$");
            RuleFor(config => config.MinUpdateTime).NotEmpty().Matches(@"^[1-9]\d*$");
            RuleFor(config => config.DescriptionFormating).NotEmpty().Matches(@"^(?:true|false|0|1)$");
        }
    }
}
