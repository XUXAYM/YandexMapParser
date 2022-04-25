using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMapParser.Domain;
using YandexMapParser.Domain.Entitites;

namespace YandexMapParser.Infrastructure
{
    public class SqlServerAddressRepo : IAddressWriter, IDisposable
    {
        private const string dotId = "@dot_id";
        private const string cadastralNumber = "@cadastral_number";
        private const string primaryAddress = "@primary_address";
        private const string secondaryAddress = "@secondary_address";
        private const string sqlCommandStr = "INSERT INTO TMP.dbo.Yandex_Reverse_Geocode (dot_id, cadastral_number, primary_address, secondary_address) VALUES ("+ dotId + ", " + cadastralNumber + ", " + primaryAddress + ", " + secondaryAddress + ");";


        protected readonly SqlConnection connection;

        public SqlServerAddressRepo(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public void Connect()
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        public void WriteNext(Address address)
        {
            using (var command = connection.CreateCommand())
            {
                try
                {
                    command.CommandText = sqlCommandStr;
                    command.Parameters.AddWithValue(dotId, address.Id);
                    command.Parameters.AddWithValue(cadastralNumber, address.CadastralNumber);
                    command.Parameters.AddWithValue(primaryAddress, address.PrimaryAddressStr);
                    command.Parameters.AddWithValue(secondaryAddress, address.SecondaryAddressStr);
                    command.ExecuteNonQuery();
                }
                catch(Exception e) {
                    Program.logger.Error(e.Message);

                    throw e;
                }
            }
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }
    }
}
