using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulware.Barcode
{
    public class Input
    {
        private string cs =
            @"Data Source=WIN-53OQL7NNTP1\SQLEXPRESS;Initial Catalog=Datamatrix;User ID=test;Password=HttpHandlerSimulware1";
        public void WriteOnDb(int tipo, int idUser, int idClasse, string label)
        {
            using (SqlConnection connection = new SqlConnection(cs))
            {
                string query = "INSERT INTO dbo.Data (Label, ID_corso, ID_user, Tipo) " +
                               "VALUES (@Label, @ID_corso, @ID_user, @Tipo)";
                SqlCommand com = new SqlCommand(query, connection);
                com.Parameters.AddWithValue("@Label", label);
                com.Parameters.AddWithValue("@ID_corso", idClasse);
                com.Parameters.AddWithValue("@ID_user", idUser);
                com.Parameters.AddWithValue("@Tipo", tipo);
                connection.Open();
                com.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void ReadFromDb()
        {

        }
    }
}
