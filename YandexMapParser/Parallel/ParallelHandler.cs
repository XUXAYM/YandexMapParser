using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMapParser.Domain;
using YandexMapParser.Domain.Entitites;
using YandexMapParser.Infrastructure;

namespace YandexMapParser.ParallelStuf
{
    class ParallelHandler
    {
        private const string WEB_DRIVER_PATH = "web_driver_path";
        private const string POINTS_PER_THREAD = "points_per_thread";
        private const string DefaultConnection = "DefaultConnection";

        readonly private IPointReader pointReader;
        private List<ParallelGeocodingExcecutor> excecutors;

        readonly private int _threadsCount;
        readonly private int _pointsCount = 250;

        public ParallelHandler(IPointReader pointReader, int threadsCount)
        {
            if(int.TryParse(ConfigurationManager.AppSettings[POINTS_PER_THREAD], out int pointsCount))
            {
                _pointsCount = pointsCount;
            }

            this.pointReader = pointReader;
            _threadsCount = threadsCount;
        }

        public void RunParallel()
        {
            while (pointReader.CanRead())
            {
                try
                {
                    InizializeParallelExcecutors();

                    var points = GetPoints();
                    AddressPoint[][] addressPointsParallelMass = new AddressPoint[excecutors.Count][];

                    for (int i = 0; i < excecutors.Count; i++)
                    {
                        addressPointsParallelMass[i] = points.Skip(i * _pointsCount).Take(_pointsCount).ToArray();
                    }

                    Parallel.For(0, excecutors.Count, (i) => excecutors[i].Execute(addressPointsParallelMass[i]));
                }
                catch (Exception e)
                {
                    Program.logger.Error(e.Message);
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    FlushExcecutors();
                    ClearTempFolder();
                }
            }
        }

        private List<AddressPoint> GetPoints()
        {
            int count = excecutors.Count * _pointsCount;
            int currentCount = 0;
            var points = new List<AddressPoint>();

            while (pointReader.CanRead() && currentCount < count)
            {
                try
                {
                    points.Add(pointReader.ReadNext());
                    currentCount++;
                }
                catch (Exception e)
                {
                    Program.logger.Error(e.Message);
                    Console.WriteLine(e.Message);
                }
            }

            return points;
        }

        private void InizializeParallelExcecutors()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--log-level=3");

            excecutors = new List<ParallelGeocodingExcecutor>();
            for (int i = 0; i < _threadsCount; i++)
            {
                IWebDriver webDriver = new ChromeDriver(ConfigurationManager.AppSettings[WEB_DRIVER_PATH], options);
                IGeocodingHandler geocodingHandler = new YandexMapHandler(webDriver);
                SqlServerAddressRepo addressWriter = new SqlServerAddressRepo(ConfigurationManager.ConnectionStrings[DefaultConnection].ConnectionString);
                addressWriter.Connect();
                excecutors.Add(new ParallelGeocodingExcecutor(addressWriter, geocodingHandler));
            }
        }

        private void FlushExcecutors()
        {
            foreach (var excecutor in excecutors)
            {
                excecutor.Dispose();
            }
            Program.logger.Info("Excecutors were flushed");
        }

        private void ClearTempFolder()
        {
            var tmp = Path.GetTempPath();

            foreach (var file in Directory.GetFiles(tmp))
            {
                try
                {
                    File.Delete(file);
                    Program.logger.Info("Deleted temp file: " + file);
                }
                catch (Exception e)
                {
                    Program.logger.Warn($"Didn't delete temp file: {file}. More info: {e.Message}");
                }
            }

            foreach (var directory in Directory.GetDirectories(tmp))
            {
                try
                {
                    Directory.Delete(directory, true);
                    Program.logger.Info("Deleted temp directory: " + directory);
                }
                catch (Exception e)
                {
                    Program.logger.Warn($"Didn't delete temp directory: {directory}. More info: {e.Message}");
                }
            }
        }
    }
}
