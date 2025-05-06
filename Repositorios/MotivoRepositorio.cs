using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using InverPaper.Dtos;
using InverPaper.Utilidades;

namespace InverPaper.Repositorios
{
    public class MotivoRepositorio
    {
        private readonly ContextoBDUtilidad db = new ContextoBDUtilidad();
        public List<MotivoDto> ObtenerMotivos()
        {
            var lista = new List<MotivoDto>();
            SqlConnection conn = db.CONN();

            try
            {
                db.Connect();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Motivo", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new MotivoDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Motivo = reader["Motivo"].ToString()
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