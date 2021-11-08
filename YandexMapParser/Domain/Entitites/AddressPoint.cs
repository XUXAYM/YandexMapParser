using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexMapParser.Domain.Entitites
{
    public class AddressPoint
    {
        public int Id { get; private set; }
        public Point Point { get; private set; }

        public AddressPoint(int id, Point point)
        {
            Id = id;
            Point = point;
        }

        public AddressPoint(int id, double latitude, double longitude) : this(id, new Point(latitude, longitude)) { }
    }
}
