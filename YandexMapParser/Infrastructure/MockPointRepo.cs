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
                new AddressPoint(1, "1", 55.835402, 37.298624),
                new AddressPoint(2, "1", 55.395624, 38.986102),
                new AddressPoint(3, "1", 55.829133, 37.296586),
            });
    }
}
