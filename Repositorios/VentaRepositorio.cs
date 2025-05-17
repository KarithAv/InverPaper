using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using InverPaper.Dtos;
using InverPaper.Utilidades;

namespace InverPaper.Repositorios
{
    public class VentaRepositorio
    {
        public void RegistrarVenta(VentaDto venta, List<DetalleVentaDto> detallesVenta)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                // Calcular el total de la venta
                venta.Total = detallesVenta.Sum(d => d.Subtotal);  // Sumar los subtotales

                // Iniciar transacción
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Preparar el parámetro de tipo tabla para los detalles de la venta
                        DataTable dtDetalleVenta = new DataTable();
                        dtDetalleVenta.Columns.Add("IdProducto", typeof(int));
                        dtDetalleVenta.Columns.Add("Cantidad", typeof(int));
                        dtDetalleVenta.Columns.Add("Subtotal", typeof(decimal));

                        foreach (var detalle in detallesVenta)
                        {
                            dtDetalleVenta.Rows.Add(detalle.IdProducto, detalle.Cantidad, detalle.Subtotal);
                        }

                        // Llamar al Stored Procedure para registrar la venta
                        var command = new SqlCommand("sp_RegistrarVenta", conn, transaction)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@IdUsuario", venta.IdUsuario);
                        command.Parameters.AddWithValue("@IdMetPago", venta.IdMetPago);
                        command.Parameters.AddWithValue("@DetalleVenta", dtDetalleVenta).SqlDbType = SqlDbType.Structured;
                        command.Parameters.AddWithValue("@Total", venta.Total);  // Agregar Total

                        // Ejecutar el Stored Procedure
                        command.ExecuteNonQuery();

                        // Confirmar la transacción
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre un error, revertir la transacción
                        transaction.Rollback();
                        throw new Exception("Error al registrar la venta y sus detalles.", ex);
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public List<VentaDto> ObtenerVentasDelDia(DateTime fecha)
        {
            List<VentaDto> lista = new List<VentaDto>();
            using (SqlConnection conn = new ContextoBDUtilidad().CONN())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT v.Id, v.FechaVenta, v.Total, CONCAT(u.Nombre,' ', u.Apellido) AS NombreUsuario " +
                                                "FROM Venta v " +
                                                "INNER JOIN Usuario u ON v.IdUsuario = u.Id " +
                                                "WHERE CAST(v.FechaVenta AS DATE) = @FechaVenta", conn);
                cmd.Parameters.AddWithValue("@FechaVenta", fecha.Date);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new VentaDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FechaVenta = Convert.ToDateTime(reader["FechaVenta"]),
                        Total = Convert.ToDecimal(reader["Total"]),
                        NombreUsuario = reader["NombreUsuario"].ToString()
                    });
                }
            }
            return lista;
        }

    }
}