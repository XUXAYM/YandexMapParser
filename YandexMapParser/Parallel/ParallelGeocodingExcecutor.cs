using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMapParser.Domain;
using YandexMapParser.Domain.Entitites;

namespace YandexMapParser.ParallelStuf
{
    class ParallelGeocodingExcecutor : IDisposable
    {
        private bool _disposed = false;

        readonly private IAddressWriter addressWriter;
        readonly private IGeocodingHandler geocodingHandler;

        public ParallelGeocodingExcecutor(IAddressWriter addressWriter, IGeocodingHandler geocodingHandler)
        {
            this.addressWriter = addressWriter;
            this.geocodingHandler = geocodingHandler;
        }

        public void Execute(IEnumerable<AddressPoint> points)
        {
            foreach (var point in points)
            {
                try
                {
                    RunProcess(point);
                }
                catch (Exception e)
                {
                    Program.logger.Error(e.Message);
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void RunProcess(AddressPoint point)
        {
            var address = geocodingHandler.FindAddressByPoint(point);

            addressWriter.WriteNext(address);
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
                    geocodingHandler.Dispose();
                }
                _disposed = true;
            }
        }

        ~ParallelGeocodingExcecutor()
        {
            Dispose(false);
        }

        #endregion
    }
}
