using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Utilities;
using InverPaper.Repositories;
using InverPaper.Dtos;
using System.Data.SqlClient;

namespace InverPaper.Repositories
{
    public class ProductoRepository
    {
        public List<ProductoDto> ListarProductos()
        {
            var lista = new List<ProductoDto>();
            var db = new DBContextUtility();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT p.Id, p.Nombre, p.Descripcion, p.StockActual, p.PrecioVenta, 
                                p.IdCategoria, c.NombreCategoria, 
                                p.IdMarca, m.NombreMarca, 
                                p.IdEstado, e.NombreEstado
                         FROM Producto p
                         JOIN Categoria c ON p.IdCategoria = c.Id
                         JOIN Marca m ON p.IdMarca = m.Id
                         JOIN Estado e ON p.IdEstado = e.Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ProductoDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Descripcion = reader["Descripcion"].ToString(),
                            StockActual = Convert.ToInt32(reader["StockActual"]),
                            PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"]),
                            IdCategoria = Convert.ToInt32(reader["IdCategoria"]),
                            NombreCategoria = reader["NombreCategoria"].ToString(),
                            IdMarca = Convert.ToInt32(reader["IdMarca"]),
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

    }
}