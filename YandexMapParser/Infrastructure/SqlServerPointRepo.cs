using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMapParser.Domain;
using YandexMapParser.Domain.Entitites;

namespace YandexMapParser.Infrastructure
{
    public class SqlServerPointRepo : IPointReader, IDisposable
    {
        private const string sqlCommandWithoutWhere = "select  [id точки], [Кадастровый номер ЗУ], [Координаты точки] from [TMP].[dbo].[Red-1_NEW] where [id точки] not in (select dot_id from TMP.dbo.Yandex_Reverse_Geocode);";

        protected readonly SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;

        public SqlServerPointRepo(string connectionString) 
        {
            connection = new SqlConnection(connectionString);
        }

        public void Connect()
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        public void ExecuteQuery()
        {
            reader?.Dispose();
            command?.Dispose();
            command = connection.CreateCommand();
            command.CommandText = sqlCommandWithoutWhere;
            reader = command.ExecuteReader();
        }

        public bool CanRead()
        {
            return reader != null && reader.HasRows;
        }

        public AddressPoint ReadNext()
        {
            reader.Read();
            var dotId = reader.GetDecimal(0);
            var cadastralNumber = reader.GetString(1);
            var coordinatesStr = reader.GetString(2);
            var splittedCoordintesStr = coordinatesStr.Split(',').Select(c => c.Trim()).ToArray();

            try
            {
                IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                var latitude = double.Parse(splittedCoordintesStr[0], formatter);
                var longitude = double.Parse(splittedCoordintesStr[1], formatter);

                return new AddressPoint(dotId, cadastralNumber, latitude, longitude);
            }
            catch (Exception e)
            {
                Program.logger.Error("Couldn't parse coordinates. Additional info: " + e.Message);
                throw e;
            }
        }

        public void Dispose()
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }
    }
}
