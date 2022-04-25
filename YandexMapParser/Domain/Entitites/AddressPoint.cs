using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexMapParser.Domain.Entitites
{
    public class AddressPoint
    {
        public decimal Id { get; private set; }
        public string CadastralNumber { get; private set; }
        public Point Point { get; private set; }

        public AddressPoint(decimal id, string cadastralNumber, Point point)
        {
            Id = id;
            CadastralNumber = cadastralNumber;
            Point = point;
        }

        public AddressPoint(decimal id, string cadastralNumber, double latitude, double longitude) : this(id, cadastralNumber, new Point(latitude, longitude)) { }
    }
}
