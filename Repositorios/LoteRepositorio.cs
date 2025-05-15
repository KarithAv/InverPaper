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
    public class LoteRepositorio
    {
        public void RegistrarLote(LoteDto lote)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var command = new SqlCommand("sp_RegistrarLote", conn, transaction)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        command.Parameters.AddWithValue("@IdProducto", lote.IdProducto);
                        command.Parameters.AddWithValue("@Cantidad", lote.Cantidad);
                        command.Parameters.AddWithValue("@PrecioCompra", lote.PrecioCompra);
                        command.Parameters.AddWithValue("@NumeroLote", lote.NumeroLote);

                        // Intentar ejecutar el procedimiento almacenado
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        // Si se lanza un error de SQL (como el RAISERROR en el stored procedure)
                        throw new Exception("Error al registrar el lote: " + ex.Message);
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public List<LoteDto> ListarLotes()
        {
            var lista = new List<LoteDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT l.Id, l.PrecioCompra, l.Cantidad, l.FechaIngreso, l.NumeroLote, 
                                l.IdProducto, p.Nombre
                         FROM Lote l
                         JOIN Producto p ON l.IdProducto = p.Id
                         JOIN Estado e ON p.IdEstado = e.Id";



                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new LoteDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            PrecioCompra = Convert.ToDecimal(reader["PrecioCompra"]),
                            IdProducto = Convert.ToInt32(reader["IdProducto"]),
                            NombreProducto = reader["Nombre"].ToString(),
                            Cantidad = Convert.ToInt32(reader["Cantidad"]),
                            NumeroLote = reader["NumeroLote"].ToString(),
                            FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]),
                        });
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return lista;
        }

    }
}