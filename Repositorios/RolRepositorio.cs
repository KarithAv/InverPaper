using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Utilidades;
using InverPaper.Dtos;
using System.Data.SqlClient;

namespace InverPaper.Repositorios
{
    public class RolRepositorio
    {
        private readonly ContextoBDUtilidad db = new ContextoBDUtilidad();

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