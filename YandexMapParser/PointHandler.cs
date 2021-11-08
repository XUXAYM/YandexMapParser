using System;

using YandexMapParser.Domain;

namespace YandexMapParser
{
    public class PointHandler
    {
        readonly private IPointReader pointReader;
        readonly private IAddressWriter addressWriter;
        readonly private IGeocodingHandler geocodingHandler;

        public PointHandler(IPointReader pointReader, IAddressWriter addressWriter, IGeocodingHandler geocodingHandler)
        {
            this.pointReader = pointReader;
            this.addressWriter = addressWriter;
            this.geocodingHandler = geocodingHandler;
        }

        public void Run()
        {
            while (pointReader.CanRead())
            {
                try
                {
                    RunProcess();
                }
                catch(Exception e)
                {

                    Program.logger.Error(e.Message);
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void Run(int count, bool catchOutOfRangeException = true)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("The argument must be positive number");

            for(int i = 0; i < count; i++)
            {
                try
                {
                    RunProcess();
                }
                catch (Exception e)
                {
                    if (catchOutOfRangeException) throw new IndexOutOfRangeException();
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void RunProcess()
        {
            var point = pointReader.ReadNext();
            var address = geocodingHandler.FindAddressByPoint(point);

            addressWriter.WriteNext(address);
        }
    }
}
