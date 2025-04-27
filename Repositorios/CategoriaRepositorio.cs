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
                string query = @"SELECT * FROM Categoria";  // Ajusta el nombre de la tabla si es necesario

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new CategoriaDto
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
                string query = "DELETE FROM Categoria WHERE Id = @Id";
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
    }
}