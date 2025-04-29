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
    }
}