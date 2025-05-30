﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Repositorios;
using InverPaper.Dtos;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using InverPaper.Utilidades;

namespace InverPaper.Repositorios
{
    public class UsuarioRepositorio
    {
        public UsuarioDto Login(string correo, string contraseña)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = "SELECT * FROM Usuario WHERE Correo = @Correo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Correo", correo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string contraseñaAlmacenada = reader["Contraseña"].ToString();
                            Debug.WriteLine("Texto plano: " + contraseña);
                            Debug.WriteLine("Hash guardado: " + contraseñaAlmacenada);
                            Debug.WriteLine("Verificación: " + BCrypt.Net.BCrypt.Verify(contraseña, contraseñaAlmacenada));

                            if (BCrypt.Net.BCrypt.Verify(contraseña, contraseñaAlmacenada))
                            {
                                return new UsuarioDto
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Apellido = reader["Apellido"].ToString(),
                                    Correo = reader["Correo"].ToString(),
                                    IdRol = Convert.ToInt32(reader["IdRol"]),
                                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                                    Respuesta = 1,
                                    Mensaje = "Login exitoso."
                                };
                            }
                        }
                    }
                }

                // Si no se encuentra o contraseña incorrecta
                return new UsuarioDto { Respuesta = 0, Mensaje = "Correo o contraseña incorrectos." };
            }
            finally
            {
                db.Disconnect();
            }
        }
        public List<UsuarioDto> ListarUsuarios()
        {
            var lista = new List<UsuarioDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT u.Id, u.Nombre, u.Apellido, u.Correo, u.Contraseña, 
                                        u.IdRol, r.Rol AS RolNombre, 
                                        u.IdEstado, e.NombreEstado AS EstadoNombre
                                 FROM Usuario u
                                 JOIN Rol r ON u.IdRol = r.Id
                                 JOIN Estado e ON u.IdEstado = e.Id
                                 ORDER BY Nombre ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new UsuarioDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Apellido = reader["Apellido"].ToString(),
                            Correo = reader["Correo"].ToString(),
                            Contraseña = reader["Contraseña"].ToString(),
                            IdRol = Convert.ToInt32(reader["IdRol"]),
                            NombreRol = reader["RolNombre"].ToString(),
                            IdEstado = Convert.ToInt32(reader["IdEstado"]),
                            NombreEstado = reader["EstadoNombre"].ToString()
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

        public UsuarioDto ObtenerUsuarioPorId(int id)
        {
            UsuarioDto usuario = null;
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "SELECT * FROM Usuario WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new UsuarioDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Contraseña = reader["Contraseña"].ToString(),
                                IdRol = Convert.ToInt32(reader["IdRol"]),
                                IdEstado = Convert.ToInt32(reader["IdEstado"])
                            };
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return usuario;
        }



        public void CrearUsuario (UsuarioDto usuario, string usuarioActual)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                using (SqlCommand cmdSetUser = new SqlCommand("EXEC sp_set_session_context @key = N'Usuario', @value = @Usuario", conn))
                {
                    cmdSetUser.Parameters.AddWithValue("@Usuario", usuarioActual);
                    cmdSetUser.ExecuteNonQuery();
                }

                // Encriptar la contraseña antes de insertarla
                string contraseñaEncriptada = EncriptadorUtilidad.EncriptarPassword(usuario.Contraseña);

                string query = @"INSERT INTO Usuario (Nombre, Apellido, Correo, Contraseña, IdRol, IdEstado)
                         VALUES (@Nombre, @Apellido, @Correo, @Contraseña, @IdRol, @IdEstado)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@Contraseña", contraseñaEncriptada); // Aquí se guarda la contraseña encriptada
                    cmd.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                    cmd.Parameters.AddWithValue("@IdEstado", usuario.IdEstado);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public void EditarUsuario(UsuarioDto usuario, string usuarioActual)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {

                db.Connect();
                // 1. Establecer el usuario en el contexto de sesión
                using (SqlCommand cmdSetUser = new SqlCommand("EXEC sp_set_session_context @key = N'Usuario', @value = @Usuario", conn))
                {
                    cmdSetUser.Parameters.AddWithValue("@Usuario", usuarioActual);
                    cmdSetUser.ExecuteNonQuery();
                }

                string query = @"UPDATE Usuario SET 
                            Nombre = @Nombre, 
                            Apellido = @Apellido, 
                            Correo = @Correo, 
                            IdRol = @IdRol, 
                            IdEstado = @IdEstado, 
                            Contraseña = @Contraseña
                         WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                    cmd.Parameters.AddWithValue("@IdEstado", usuario.IdEstado);
                    cmd.Parameters.AddWithValue("@Id", usuario.Id);

                    // Si la contraseña está vacía, dejamos la contraseña tal cual está en la base de datos
                    if (!string.IsNullOrEmpty(usuario.Contraseña))
                    {
                        cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);  // Si hay contraseña, se guarda la nueva
                    }
                    else
                    {
                        // No actualizar la contraseña si no se modificó
                        cmd.Parameters.AddWithValue("@Contraseña", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.Disconnect();
            }
        }

        public void EliminarUsuario(int id, string usuarioActual)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                using (SqlCommand cmdSetUser = new SqlCommand("EXEC sp_set_session_context @key = N'Usuario', @value = @Usuario", conn))
                {
                    cmdSetUser.Parameters.AddWithValue("@Usuario", usuarioActual);
                    cmdSetUser.ExecuteNonQuery();
                }

                string query = "DELETE FROM Usuario WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public bool UsuarioExiste(string correo)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "SELECT COUNT(*) FROM Usuario WHERE Correo = @Correo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            finally
            {
                db.Disconnect();
            }
        }

        public bool UsuarioExisteEditar(string correo, int idIgnorar)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "SELECT COUNT(*) FROM Usuario WHERE Correo = @Correo AND Id != @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar).Value = correo;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idIgnorar;
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public UsuarioDto ObtenerUsuarioPorCorreo(string correo)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();
            UsuarioDto usuario = null;

            try
            {
                db.Connect();

                string query = @"SELECT Id, Nombre, Apellido, Correo, Contraseña, IdRol, IdEstado 
                         FROM Usuario WHERE Correo = @Correo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Correo", correo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new UsuarioDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Contraseña = reader["Contraseña"].ToString(),
                                IdRol = Convert.ToInt32(reader["IdRol"]),
                                IdEstado = Convert.ToInt32(reader["IdEstado"])
                            };
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return usuario;
        }
        public void ActualizarContraseña(int idUsuario, string nuevaContraseña)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                // Encriptar la nueva contraseña (suponiendo que ya tienes un método para encriptarla)
                string contraseñaEncriptada = EncriptadorUtilidad.EncriptarPassword(nuevaContraseña);

                string query = @"UPDATE Usuario 
                             SET Contraseña = @Contraseña 
                             WHERE Id = @IdUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Contraseña", contraseñaEncriptada);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

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
