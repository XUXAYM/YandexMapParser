using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexMapParser.Domain.Entitites
{
    public struct Point
    {
        // 55
        public double Latitude { get; private set; }
        // 36
        public double Longitude { get; private set; }

        public Point(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public string ToStringDividedByComma() => String.Join(", ", Latitude, Longitude);
    }
}
