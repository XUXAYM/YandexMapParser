using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexMapParser.Domain.Entitites
{
    public class Address
    {
        public int Id { get; private set; }
        public string PrimaryAddressStr { get; private set; }
        public string SecondaryAddressStr { get; private set; }

        public Address(int id, string primaryAddressStr, string secondaryAddressStr)
        {
            Id = id;
            PrimaryAddressStr = primaryAddressStr;
            SecondaryAddressStr = secondaryAddressStr;
        }

        public override string ToString()
        {
            return "Address { Id = " + Id + ", PrimaryAddressStr = \"" + PrimaryAddressStr + "\", SecondaryAddressStr = \"" + SecondaryAddressStr + "\"}";
        }
    }
}
