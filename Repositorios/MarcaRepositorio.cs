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
                string query = @"SELECT * FROM Marca"; 

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new MarcaDto
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
                string query = "DELETE FROM Marca WHERE Id = @Id";
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
    }
}