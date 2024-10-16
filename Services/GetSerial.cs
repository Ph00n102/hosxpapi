using MySqlConnector;


namespace hosxpapi.Services
{
    public class GetSerial
    {
        private readonly string _connectionString;

        public  GetSerial(string connectionString)
        {
            _connectionString = connectionString;
        } 

        public async Task<string> GetSerialNumber()
        {
            string serialNumber = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT get_serialnumber('ovst-q-631010') AS oqueue", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            serialNumber = reader["oqueue"].ToString();
                        }
                    }
                }
            }

            return serialNumber;
        }  

        public async Task<string> GetSerialNumberByParam(string param)
        {
            string serialNumber = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT get_serialnumber(@param) AS oqueue", connection))
                {
                    command.Parameters.Add(new MySqlParameter("@param", param));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            serialNumber = reader["oqueue"].ToString();
                        }
                    }
                }
            }

            return serialNumber;
        } 


     }
}