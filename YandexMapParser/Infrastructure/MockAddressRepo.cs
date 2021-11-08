using NLog;
using System;

using YandexMapParser.Domain;
using YandexMapParser.Domain.Entitites;

namespace YandexMapParser.Infrastructure
{
    public class MockAddressRepo : IAddressWriter
    {
        public void WriteNext(Address address)
        {
            Program.logger.Debug(address.ToString());
            Console.WriteLine(address.ToString());
        }
    }
}
