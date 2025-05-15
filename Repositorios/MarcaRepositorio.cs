using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Utilidades;
using InverPaper.Dtos;
using System.Data.SqlClient;

namespace InverPaper.Repositorios
{
    public class MarcaRepositorio
    {


        private ContextoBDUtilidad _contextoBD;

        // Cambia el constructor para no requerir el parámetro 'contextoBD'
        public MarcaRepositorio()
        {
            _contextoBD = new ContextoBDUtilidad();  // Aquí instancias el contexto internamente
        }

        public List<MarcaDto> ListarMarcas()
        {
            var lista = new List<MarcaDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT m.Id, m.NombreMarca, m.IdEstado, e.NombreEstado
                         FROM Marca m
                         JOIN Estado e ON m.IdEstado = e.Id
                                 ORDER BY NombreMarca ASC"; 

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new MarcaDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NombreMarca = reader["NombreMarca"].ToString(),
                            IdEstado = Convert.ToInt32(reader["IdEstado"]),
                            NombreEstado = reader["NombreEstado"].ToString()
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
        public MarcaDto ObtenerMarcaPorId(int id)
        {
            MarcaDto marca = null;
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "SELECT * FROM Marca WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            marca = new MarcaDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                NombreMarca = reader["NombreMarca"].ToString()
                            };
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return marca;
        }


        public void CrearMarca(MarcaDto marca)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = @"INSERT INTO Marca (NombreMarca)
                         VALUES (@NombreMarca)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreMarca", marca.NombreMarca);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.Disconnect();
            }
        }


        public void EliminarMarca(int id)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                // Verificar si la marca está siendo utilizada por productos
                string verificarQuery = "SELECT COUNT(*) FROM Producto WHERE IdMarca = @IdMarca";
                using (SqlCommand cmdVerificar = new SqlCommand(verificarQuery, conn))
                {
                    cmdVerificar.Parameters.AddWithValue("@IdMarca", id);
                    int cantidad = (int)cmdVerificar.ExecuteScalar();

                    if (cantidad > 0)
                    {
                        throw new Exception("No se puede eliminar la marca porque está siendo utilizada por productos.");
                    }
                }

                // Eliminación lógica: cambiar el estado a Inactivo (IdEstado = 2)
                string query = "UPDATE Marca SET IdEstado = 2 WHERE Id = @Id";
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
        public void ActivarMarca(int id)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = "UPDATE Marca SET IdEstado = 1 WHERE Id = @Id";
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

        public bool MarcaExiste(string marca)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "SELECT COUNT(*) FROM Marca WHERE NombreMarca = @NombreMarca";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreMarca", marca);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            finally
            {
                db.Disconnect();
            }
        }

        public List<MarcaDto> ObtenerMarcas()
        {
            var marcas = new List<MarcaDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT m.Id, m.NombreMarca, m.IdEstado, e.NombreEstado
                         FROM Marca m
                         JOIN Estado e ON m.IdEstado = e.Id
                         WHERE m.IdEstado = 1";  // Modificado para obtener todas las marcas ACTIVAS
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        marcas.Add(new MarcaDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NombreMarca = reader["NombreMarca"].ToString()
                        });
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return marcas;
        }

    }
}