using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Utilidades;
using InverPaper.Dtos;
using System.Data.SqlClient;

namespace InverPaper.Repositorios
{
    public class EstadoRepositorio
    {
        private readonly ContextoBDUtilidad db = new ContextoBDUtilidad();

        public List<EstadoDto> ObtenerEstados()
        {
            var lista = new List<EstadoDto>();
            SqlConnection conn = db.CONN();

            try
            {
                db.Connect();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Estado", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new EstadoDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NombreEstado = reader["NombreEstado"].ToString()
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