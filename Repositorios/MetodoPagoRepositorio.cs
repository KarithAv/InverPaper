using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using InverPaper.Dtos;
using InverPaper.Utilidades;

namespace InverPaper.Repositorios
{
    public class MetodoPagoRepositorio
    {
        private readonly ContextoBDUtilidad db = new ContextoBDUtilidad();
        public List<MetodoPagoDto> ObtenerMetodosPago()
        {
            var lista = new List<MetodoPagoDto>();
            SqlConnection conn = db.CONN();

            try
            {
                db.Connect();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Metodo_Pago", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new MetodoPagoDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MetodoPago = reader["MetodoPago"].ToString()
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