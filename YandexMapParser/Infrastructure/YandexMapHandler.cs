using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YandexMapParser.Domain;
using YandexMapParser.Domain.Entitites;

namespace YandexMapParser.Infrastructure
{
    public class YandexMapHandler : IGeocodingHandler
    {
        private const string YANDEX_MAP_URL_STRING = "https://yandex.ru/maps";

        private bool _disposed = false;
        private IWebDriver webDriver;
        private bool isBannerHiden = false;

        public YandexMapHandler(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            ConfigureWebDriver();
            OpenPage();
        }

        public virtual void ConfigureWebDriver()
        {
        }

        public virtual void OpenPage()
        {
            webDriver.Navigate().GoToUrl(YANDEX_MAP_URL_STRING);
            isBannerHiden = false;
            Thread.Sleep(2000);
        }

        public Address FindAddressByPointWithRetry(AddressPoint point, int delayPerTry = 0)
        {
            Address address;
            try
            {
                address = FindAddressByPoint(point, 0);
            }
            catch (NoSuchElementException e)
            {
                Program.logger.Error(e.Message);
                address = FindAddressByPointWithRetry(point, delayPerTry + 100);
            }
            catch (Exception e)
            {
                Program.logger.Error(e.Message);
                throw e;
            }
            return address;
        }

        public Address FindAddressByPoint(AddressPoint point) => FindAddressByPointWithRetry(point);

        public Address FindAddressByPoint(AddressPoint point, int toDefaultDelay)
        {
            if (toDefaultDelay < 0) throw new ArgumentOutOfRangeException("The argument must be positive number");

            if (!isBannerHiden)
            {
                try
                {
                    FindBanner().Click();
                    isBannerHiden = true;
                }
                catch
                {
                    Program.logger.Error("Banner not founded");
                }
            }
            //200 
            Thread.Sleep(toDefaultDelay);
            try
            {
                FindClearButton().Click();
            }
            catch
            {
                Program.logger.Error("Clear button not founded");
            }
            //250 + 
            //220 + 
            Thread.Sleep(50 + toDefaultDelay);
            var inputField = FindInputField();
            inputField.SendKeys(point.Point.ToStringDividedByComma());
            Thread.Sleep(20 + toDefaultDelay);
            var button = FindSearchButton();
            button.Click();
            //750 + 
            Thread.Sleep(600 + toDefaultDelay);

            var primaryAddressStr = FindPrimaryAddress().Text;
            var secondaryAddressStr = FindSecondaryAddress().Text;

            return new Address(point.Id, point.CadastralNumber, primaryAddressStr, FormatAddressService.ReverseAddressStr(secondaryAddressStr));
        }

        private IWebElement FindBanner()
        {
            string xPath = "/html/body/div[3]/div[1]/a";
            return webDriver.FindElement(By.XPath(xPath));
        }

        private IWebElement FindInputField()
        {
            try
            {
                string className = "input__context";
                return webDriver.FindElements(By.ClassName(className)).First().FindElement(By.XPath("./input"));
            }
            catch
            {
                string xPath = "/html/body/div[1]/div[2]/div[2]/header/div/div/div/form/div[2]/div/span/span/input";
                return webDriver.FindElement(By.XPath(xPath));
            }
        }

        private IWebElement FindSearchButton()
        {
            try
            {
                string xPath = "//button[@aria-label='Найти']";
                return webDriver.FindElement(By.XPath(xPath));
            }
            catch
            {
                string xPath = "/html/body/div[1]/div[2]/div[2]/header/div/div/div/form/div[3]/button";
                return webDriver.FindElement(By.XPath(xPath));
            }
        }

        private IWebElement FindClearButton()
        {
            try
            {
                string xPath = "//button[@aria-label='Закрыть']";
                return webDriver.FindElement(By.XPath(xPath));
            }
            catch
            {
                string xPath = "/html/body/div[1]/div[2]/div[2]/header/div/div/div/form/div[5]/button";
                return webDriver.FindElement(By.XPath(xPath));
            }
        }

        private IWebElement FindPrimaryAddress()
        {
            string className = "card-title-view__title";
            return webDriver.FindElements(By.ClassName(className)).First();
        }

        private IWebElement FindSecondaryAddress()
        {
            try
            {
                string className = "toponym-card-title-view__description";
                return webDriver.FindElements(By.ClassName(className)).First();
            }
            catch
            {
                string xPath = "/html/body/div[1]/div[2]/div[10]/div/div[1]/div[1]/div[1]/div/div[1]/div/div/div[3]/div[1]";
                return webDriver.FindElement(By.XPath(xPath));
            }
        }

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        webDriver.Close();
                        webDriver.Quit();
                    }
                    catch (Exception e) 
                    {
                        Program.logger.Error($"WebDriver quiting error. More info: {e.Message}");
                    }
                }
                _disposed = true;
            }
        }

        ~YandexMapHandler()
        {
            Dispose(false);
        }

        #endregion
    }
}
