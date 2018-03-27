using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simulware.Barcode
{
    public class DBComm
    {
        private string _cs = @"Data Source=WIN-53OQL7NNTP1\SQLEXPRESS;Initial Catalog=Datamatrix;User ID=test;Password=HttpHandlerSimulware1";

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

        public int ReadFromDb(string tipo, int idUser, int idClasse, string label)
        {
            using (SqlConnection connection = new SqlConnection(_cs))
            {
                string query = "SELECT Serial FROM dbo.Data WHERE Label=@label";
                SqlCommand com = new SqlCommand();
                com.Parameters.AddWithValue("@label", label);
                com.CommandText = query;
                com.CommandType = CommandType.Text;
                int result = 0;
                com.Connection = connection;
                connection.Open();
                var reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
                reader.Close();
                connection.Close();
                return result;
            }
        }

        public string ReadFromDb(int serial)
        {
            using (SqlConnection connection = new SqlConnection(_cs))
            {
                string query = "SELECT * FROM dbo.Data WHERE Serial=@serial";
                SqlCommand com = new SqlCommand();
                com.Parameters.AddWithValue("@serial", serial);
                com.CommandText = query;
                com.CommandType = CommandType.Text;
                com.Connection = connection;
                connection.Open();
                var reader = com.ExecuteReader();
                string id="", label="", idCorso="", idClasse="", timestamp="", seriale="", tipo="", result;
                while (reader.Read())
                {
                    id = reader["Id"].ToString();
                    label = reader["Label"].ToString();
                    idCorso = reader["ID_corso"].ToString();
                    idClasse = reader["ID_user"].ToString();
                    timestamp = reader["Timestamp"].ToString();
                    seriale = reader["Serial"].ToString();
                    tipo = reader["Tipo"].ToString();
                }
                reader.Close();
                connection.Close();
                return result = id + " " + label + " " + idCorso + " " + idClasse + " " + timestamp + " " + seriale + " " + tipo;
            }
        }
    }
}
