using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using YandexMapParser.Domain;
using YandexMapParser.Domain.Entitites;

namespace YandexMapParser.Infrastructure
{
    public class MockPointRepo : IPointReader
    {
        private int index;

        public MockPointRepo()
        {
            index = 0;
        }

        public bool CanRead()
        {
            var nextIndex = index;
            return nextIndex < points.Count();
        }

        public AddressPoint ReadNext()
        {
            return points[index++];
        }

        public void Reset()
        {
            index = 0;
        }

        private static ImmutableList<AddressPoint> points = ImmutableList.CreateRange(new List<AddressPoint>
            {
                new AddressPoint(1, 55.835402, 37.298624),
                new AddressPoint(2, 55.395624, 38.986102),
                new AddressPoint(3, 55.829133, 37.296586),
                new AddressPoint(4, 55.862266, 37.101752),
                new AddressPoint(5, 55.888160, 37.029248),
                new AddressPoint(6, 55.835402, 37.298624),
                new AddressPoint(7, 55.395624, 38.986102),
                new AddressPoint(8, 55.829133, 37.296586),
                new AddressPoint(9, 55.862266, 37.101752),
                new AddressPoint(10, 55.888160, 37.029248),
                new AddressPoint(11, 55.835402, 37.298624),
                new AddressPoint(12, 55.395624, 38.986102),
                new AddressPoint(13, 55.829133, 37.296586),
                new AddressPoint(14, 55.862266, 37.101752),
                new AddressPoint(15, 55.888160, 37.029248),
                new AddressPoint(16, 55.835402, 37.298624),
                new AddressPoint(17, 55.395624, 38.986102),
                new AddressPoint(18, 55.829133, 37.296586),
                new AddressPoint(19, 55.862266, 37.101752),
                new AddressPoint(20, 55.888160, 37.029248),
                new AddressPoint(21, 55.835402, 37.298624),
                new AddressPoint(22, 55.395624, 38.986102),
                new AddressPoint(23, 55.829133, 37.296586),
                new AddressPoint(24, 55.862266, 37.101752),
                new AddressPoint(25, 55.888160, 37.029248),
                new AddressPoint(26, 55.835402, 37.298624),
                new AddressPoint(27, 55.395624, 38.986102),
                new AddressPoint(28, 55.829133, 37.296586),
                new AddressPoint(29, 55.862266, 37.101752),
                new AddressPoint(30, 55.888160, 37.029248),
            });
    }
}
