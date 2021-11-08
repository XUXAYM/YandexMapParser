using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMapParser.Domain.Entitites;

namespace YandexMapParser.Domain
{
    public interface IAddressWriter
    {
        void WriteNext(Address address);
    }
}
