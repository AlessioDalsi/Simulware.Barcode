using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Simulware.Barcode
{
    public class DBComm
    {

        private readonly string _cs = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

        public void WriteOnDb(string tipo, int idUser, int idClasse, string label)
        {
            using (SqlConnection connection = new SqlConnection(_cs))
            {
                //string query = "INSERT INTO dbo.Data (Label, ID_corso, ID_user, Tipo) " +
                //               "VALUES (@Label, @ID_corso, @ID_user, @Tipo)";
                SqlCommand com = new SqlCommand("WLF", connection) { CommandType = CommandType.StoredProcedure };
                com.Parameters.AddWithValue("@label", label);
                com.Parameters.AddWithValue("@idCorso", idClasse);
                com.Parameters.AddWithValue("@idUser", idUser);
                com.Parameters.AddWithValue("@typelabel", tipo);
                var ret = com.Parameters.Add("@ReturnVal", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;
                connection.Open();
                com.ExecuteNonQuery();
                var result = ret.Value;
                connection.Close();
            }
        }

        public Tuple<int, string> ReadFromDb(string tipo, int idUser, int idClasse, string label)
        {
            using (SqlConnection connection = new SqlConnection(_cs))
            {
                string query = "SELECT D.Serial, S.Formato FROM dbo.Data D INNER JOIN dbo.SerialType S ON D.Tipo = S.Id WHERE Label=@label";
                SqlCommand com = new SqlCommand();
                com.Parameters.AddWithValue("@label", label);
                com.CommandText = query;
                com.CommandType = CommandType.Text;
                com.Connection = connection;
                connection.Open();
                int Serial = 0;
                string Formato = "";
                var reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Serial = Convert.ToInt32(reader["Serial"]);
                        Formato = reader["Formato"].ToString();
                    }
                }
                reader.Close();
                connection.Close();
                return new Tuple<int, string>(Serial, Formato);
            }
        }

        public Tuple<string, int, int, int, int, string> ReadFromDb(int serial)
        {
            using (SqlConnection connection = new SqlConnection(_cs))
            {
                string query = "SELECT D.Label, D.ID_corso, D.ID_user, D.Timestamp, D.Serial, S.Nome FROM dbo.Data D INNER JOIN dbo.SerialType S ON D.Tipo = S.Id WHERE Serial = @serial";
                SqlCommand com = new SqlCommand();
                com.Parameters.AddWithValue("@serial", serial);
                com.CommandText = query;
                com.CommandType = CommandType.Text;
                com.Connection = connection;
                connection.Open();
                var reader = com.ExecuteReader();
                string label = "", tipo = "";
                int idCorso = 0, idClasse = 0, seriale = 0;
                DateTime timestamp = new DateTime();
                while (reader.Read())
                {
                    label = reader["Label"].ToString();
                    idCorso = Convert.ToInt32(reader["ID_corso"]);
                    idClasse = Convert.ToInt32(reader["ID_user"].ToString());
                    timestamp = Convert.ToDateTime(reader["Timestamp"]);
                    seriale = Convert.ToInt32(reader["Serial"].ToString());
                    tipo = reader["Nome"].ToString();
                }

                var date = (int)timestamp.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                reader.Close();
                connection.Close();
                return new Tuple<string, int, int, int, int, string>(label, idCorso, idClasse, date, seriale, tipo);
            }
        }
    }
}
