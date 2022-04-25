using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Configuration;

using YandexMapParser.Domain;
using YandexMapParser.Infrastructure;
using NLog;
using System.Collections.Generic;
using YandexMapParser.ParallelStuf;

namespace YandexMapParser
{
    class Program
    {
        private const string WEB_DRIVER_PATH = "web_driver_path"; 
        private const string WORK_PARALLEL = "work_parrallel";
        private const string PARALLEL_THREADS_COUNT = "parallel_threads_count";
        private const string DefaultConnection = "DefaultConnection";

        public readonly static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main()
        {
            SqlServerPointRepo pointReader = new SqlServerPointRepo(ConfigurationManager.ConnectionStrings[DefaultConnection].ConnectionString);
            pointReader.Connect();
            pointReader.ExecuteQuery();

            SqlServerAddressRepo addressWriter = new SqlServerAddressRepo(ConfigurationManager.ConnectionStrings[DefaultConnection].ConnectionString);
            addressWriter.Connect();

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--log-level=3");


            bool.TryParse(ConfigurationManager.AppSettings[WORK_PARALLEL], out bool isParallel);
            if (isParallel)
            {
                int threadsCount = 4;
                int.TryParse(ConfigurationManager.AppSettings[PARALLEL_THREADS_COUNT], out threadsCount);

                logger.Info($"Start program in parallel. Threads - {threadsCount}");

                var handler = new ParallelHandler(pointReader, threadsCount);
                handler.RunParallel();
            }
            else
            {
                logger.Info($"Start program in one thread");

                IWebDriver webDriver = new ChromeDriver(ConfigurationManager.AppSettings[WEB_DRIVER_PATH], options);
                IGeocodingHandler geocodingHandler = new YandexMapHandler(webDriver);
                PointHandler handler = new PointHandler(pointReader, addressWriter, geocodingHandler);
                handler.Run();
            }

            logger.Info("End program");
        }
    }
}
