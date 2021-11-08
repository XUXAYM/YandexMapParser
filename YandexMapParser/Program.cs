using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Configuration;

using YandexMapParser.Domain;
using YandexMapParser.Infrastructure;
using NLog;

namespace YandexMapParser
{
    class Program
    {
        private const string WEB_DRIVER_PATH = "web_driver_path";

        public readonly static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main()
        {
            IPointReader pointReader = new MockPointRepo();
            IAddressWriter addressWriter = new MockAddressRepo(); 
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--log-level=3");
            IWebDriver webDriver = new ChromeDriver(ConfigurationManager.AppSettings[WEB_DRIVER_PATH], options);
            IGeocodingHandler geocodingHandler = new YandexMapHandler(webDriver);
            
            logger.Info("Start programm");
            
            PointHandler handler = new PointHandler(pointReader, addressWriter, geocodingHandler);
            handler.Run();

            logger.Info("End programm");
            Console.ReadKey();
        }
    }
}
