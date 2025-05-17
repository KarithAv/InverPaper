using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using InverPaper.Utilidades;
using InverPaper.Dtos;
using System.Data;
using System.Diagnostics;

namespace InverPaper.Repositorios
{
    public class AjustesInventarioRepositorio
    {
        private readonly ContextoBDUtilidad db = new ContextoBDUtilidad();
        public void RegistrarAjuste(AjusteInventarioDto dto)
        {
            SqlConnection conn = db.CONN();

            try
            {
                Debug.WriteLine($"IdProducto: {dto.IdProducto}, CantidadAjustada: {dto.CantidadAjustada}, IdMotivo: {dto.IdMotivo}, IdUsuario: {dto.IdUsuario}, Accion: {dto.Accion}, Comentarios: {dto.Comentarios}");

                db.Connect();
                SqlCommand cmd = new SqlCommand("sp_RegistrarAjusteInventario", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProducto", dto.IdProducto);
                cmd.Parameters.AddWithValue("@CantidadAjustada", dto.CantidadAjustada);
                cmd.Parameters.AddWithValue("@IdMotivo", dto.IdMotivo);
                cmd.Parameters.AddWithValue("@IdUsuario", dto.IdUsuario);
                cmd.Parameters.Add("@Accion", SqlDbType.Bit).Value = dto.Accion ? 1 : 0;
                cmd.Parameters.AddWithValue("@Comentarios", dto.Comentarios ?? "");

                cmd.ExecuteNonQuery();

                dto.Respuesta = 1;
                dto.Mensaje = "Ajuste de inventario registrado con éxito.";
            }
            catch (SqlException ex)
            {
                foreach (SqlError error in ex.Errors)
                {
                    Debug.WriteLine($"SQL Error {error.Number}: {error.Message}");
                }

                dto.Respuesta = 0;
                dto.Mensaje = "Error al registrar el ajuste: " + ex.Message;
                throw; 
            }
            finally
            {
                db.Disconnect();
            }
        }
        public List<AjusteInventarioDto> ObtenerAjustesPorFecha(DateTime fecha)
        {
            var lista = new List<AjusteInventarioDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = @"
            SELECT 
    a.Id,
    a.IdProducto,
    p.Nombre AS NombreProducto,
    ma.NombreMarca AS NombreMarca, -- Marca añadida
    a.CantidadAjustada,
    a.FechaAjuste,
    a.IdMotivo,
    m.Motivo AS Motivo,
    a.IdUsuario,
    CONCAT(u.Nombre, ' ', u.Apellido) AS NombreUsuario,
    a.Accion,
    a.Comentarios
FROM AJUSTES_INVENTARIO a
INNER JOIN Producto p ON p.Id = a.IdProducto
INNER JOIN Marca ma ON ma.Id = p.IdMarca -- JOIN para obtener la marca
INNER JOIN Usuario u ON u.Id = a.IdUsuario
INNER JOIN Motivo m ON m.Id = a.IdMotivo
WHERE CAST(a.FechaAjuste AS DATE) = @Fecha";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Fecha", fecha.Date);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new AjusteInventarioDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                NombreProducto = reader["NombreProducto"].ToString(),
                                NombreMarca = reader["NombreMarca"].ToString(),
                                CantidadAjustada = Convert.ToInt32(reader["CantidadAjustada"]),
                                FechaAjuste = Convert.ToDateTime(reader["FechaAjuste"]),
                                IdMotivo = Convert.ToInt32(reader["IdMotivo"]),
                                NombreMotivo = reader["Motivo"].ToString(),
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                NombreUsuario = reader["NombreUsuario"].ToString(),
                                Accion = Convert.ToBoolean(reader["Accion"]),
                                Comentarios = reader["Comentarios"].ToString(),
                                Mensaje = $"{reader["NombreProducto"]} - {reader["Motivo"]} por {reader["NombreUsuario"]}"
                            });
                        }
                    }
                }

                return lista;
            }
            finally
            {
                db.Disconnect();
            }
        }

    }
}