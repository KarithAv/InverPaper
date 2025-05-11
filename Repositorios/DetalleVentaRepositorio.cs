using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Utilidades;
using InverPaper.Dtos;
using System.Data.SqlClient;

namespace InverPaper.Repositorios
{
    public class DetalleVentaRepositorio
    {
        public List<DetalleVentaDto> ObtenerDetallesPorIdVenta(int idVenta)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();
            var lista = new List<DetalleVentaDto>();
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                db.Connect();

                string query = @"
            SELECT 
                dv.Id,
                dv.IdVenta,
                dv.IdProducto,
                p.Nombre AS NombreProducto,
                dv.Cantidad,
                dv.Subtotal
            FROM DETALLE_VENTA dv
            INNER JOIN Producto p ON dv.IdProducto = p.Id
            WHERE dv.IdVenta = @IdVenta";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var detalle = new DetalleVentaDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        IdVenta = Convert.ToInt32(reader["IdVenta"]),
                        IdProducto = Convert.ToInt32(reader["IdProducto"]),
                        NombreProducto = reader["NombreProducto"].ToString(),
                        Cantidad = Convert.ToInt32(reader["Cantidad"]),
                        Subtotal = Convert.ToDecimal(reader["Subtotal"])
                    };

                    lista.Add(detalle);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los detalles de la venta.", ex);
            }
            finally
            {
                reader?.Close();
                cmd?.Dispose();
                db.Disconnect();
            }

            return lista;
        }

    }
}