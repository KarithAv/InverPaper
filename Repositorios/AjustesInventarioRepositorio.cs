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

    }
}