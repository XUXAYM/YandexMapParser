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
            catch(Exception e)
            {
                Program.logger.Error(e.Message);
                address = FindAddressByPointWithRetry(point, delayPerTry + 100);
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

            return new Address(point.Id, primaryAddressStr, secondaryAddressStr);
        }

        private IWebElement FindBanner()
        {
            string xPath = "/html/body/div[3]/div[1]/a";
            return webDriver.FindElement(By.XPath(xPath));
        }

        private IWebElement FindInputField()
        {
            string xPath = "/html/body/div[1]/div[2]/div[3]/div/div/div/div/form/div[2]/div/span/span/input";
            return webDriver.FindElement(By.XPath(xPath));
        }
        
        private IWebElement FindSearchButton()
        {
            string xPath = "/html/body/div[1]/div[2]/div[3]/div/div/div/div/form/div[3]/button";
            return webDriver.FindElement(By.XPath(xPath));
        }

        private IWebElement FindClearButton()
        {
            string xPath = "/html/body/div[1]/div[2]/div[3]/div/div/div/div/form/div[5]/button";
            return webDriver.FindElement(By.XPath(xPath));
        }

        private IWebElement FindPrimaryAddress()
        {
            string xPath = "/html/body/div[1]/div[2]/div[10]/div/div[1]/div[1]/div[1]/div/div[1]/div/div/div[2]/div[2]/h1";
            return webDriver.FindElement(By.XPath(xPath));
        }

        private IWebElement FindSecondaryAddress()
        {
            string xPath = "/html/body/div[1]/div[2]/div[10]/div/div[1]/div[1]/div[1]/div/div[1]/div/div/div[3]/div[1]";
            return webDriver.FindElement(By.XPath(xPath));
        }
    }
}
