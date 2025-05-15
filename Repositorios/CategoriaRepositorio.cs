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
    public class CategoriaRepositorio
    {

        private ContextoBDUtilidad _contextoBD;

        // Cambia el constructor para no requerir el parámetro 'contextoBD'
        public CategoriaRepositorio()
        {
            _contextoBD = new ContextoBDUtilidad();  // Aquí instancias el contexto internamente
        }

        public List<CategoriaDto> ListarCategorias()
        {
            var lista = new List<CategoriaDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT c.Id, c.NombreCategoria, c.IdEstado, e.NombreEstado
                         FROM Categoria c
                         JOIN Estado e ON c.IdEstado = e.Id
                                 ORDER BY NombreCategoria ASC";  

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new CategoriaDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NombreCategoria = reader["NombreCategoria"].ToString(),
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
        public CategoriaDto ObtenerCategoriaPorId(int id)
        {
            CategoriaDto categoria = null;
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "SELECT * FROM Categoria WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            categoria = new CategoriaDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                NombreCategoria = reader["NombreCategoria"].ToString()
                            };
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return categoria;
        }


        public void CrearCategoria(CategoriaDto categoria)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = @"INSERT INTO Categoria (NombreCategoria)
                         VALUES (@NombreCategoria)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreCategoria", categoria.NombreCategoria);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.Disconnect();
            }
        }


        public void EliminarCategoria(int id)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                // Verificar si la categoria está siendo utilizada por productos
                string verificarQuery = "SELECT COUNT(*) FROM Producto WHERE IdCategoria = @IdCategoria";
                using (SqlCommand cmdVerificar = new SqlCommand(verificarQuery, conn))
                {
                    cmdVerificar.Parameters.AddWithValue("@IdCategoria", id);
                    int cantidad = (int)cmdVerificar.ExecuteScalar();

                    if (cantidad > 0)
                    {
                        throw new Exception("No se puede eliminar la categoria porque está siendo utilizada por productos.");
                    }
                }

                // Eliminación lógica: cambiar el estado a Inactivo (IdEstado = 2)
                string query = "UPDATE Categoria SET IdEstado = 2 WHERE Id = @Id";
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
        public void ActivarCategoria(int id)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = "UPDATE Categoria SET IdEstado = 1 WHERE Id = @Id";
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
        public bool CategoriaExiste(string categoria)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "SELECT COUNT(*) FROM Categoria WHERE NombreCategoria = @NombreCategoria";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreCategoria", categoria);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public List<CategoriaDto> ObtenerCategorias()
        {
            var categorias = new List<CategoriaDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT c.Id, c.NombreCategoria, c.IdEstado, e.NombreEstado
                         FROM Categoria c
                         JOIN Estado e ON c.IdEstado = e.Id
                         WHERE c.IdEstado = 1";  // Modificado para obtener todas las categorías
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categorias.Add(new CategoriaDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NombreCategoria = reader["NombreCategoria"].ToString()
                        });
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return categorias;
        }

    }
}