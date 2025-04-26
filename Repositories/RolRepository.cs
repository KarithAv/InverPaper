using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Utilities;
using InverPaper.Dtos;
using System.Data.SqlClient;

namespace InverPaper.Repositories
{
    public class RolRepository
    {
        private readonly DBContextUtility db = new DBContextUtility();

        public List<RolDto> ObtenerRoles()
        {
            var lista = new List<RolDto>();
            SqlConnection conn = db.CONN();

            try
            {
                db.Connect();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Rol", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new RolDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Rol = reader["Rol"].ToString()
                    });
                }

                reader.Close();
            }
            finally
            {
                db.Disconnect();
            }

            return lista;
        }
    }
}