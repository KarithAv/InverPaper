using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using InverPaper.Utilidades;
using InverPaper.Dtos;

namespace InverPaper.Repositorios
{
    public class TokenRepositorio
    {
        public void CrearToken(int idUsuario, Guid token, DateTime expiracion)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = @"INSERT INTO TOKEN_RECUPERACION (IdUsuario, Token, FechaExpiracion)
                                 VALUES (@IdUsuario, @Token, @FechaExpiracion)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@FechaExpiracion", expiracion);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.Disconnect();
            }
        }

        public TokenRecuperacionDto ObtenerIdUsuarioPorToken(Guid token)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();
            TokenRecuperacionDto tokenDto = null;

            try
            {
                db.Connect();

                string query = @"SELECT IdUsuario, Usado, FechaExpiracion FROM TOKEN_RECUPERACION 
                         WHERE Token = @Token AND FechaExpiracion > GETDATE()";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tokenDto = new TokenRecuperacionDto
                            {
                                IdUsuario = reader.GetInt32(0),  // Asegúrate de que se asigna correctamente
                                Usado = reader.GetBoolean(1),
                                FechaExpiracion = reader.GetDateTime(2)
                            };
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return tokenDto;
        }


        public void MarcarTokenComoUsado(Guid token)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = "UPDATE TOKEN_RECUPERACION SET Usado = 1 WHERE Token = @Token";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
    }
}