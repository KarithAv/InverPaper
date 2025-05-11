using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Utilidades;
using InverPaper.Repositorios;
using InverPaper.Dtos;
using System.Data.SqlClient;

namespace InverPaper.Repositorios
{
    public class ProductoRepositorio
    {
        public List<ProductoDto> ListarProductos()
        {
            var lista = new List<ProductoDto>();
            var db = new ContextoBDUtilidad();
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
        public void CrearProducto(ProductoDto productoDto)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"INSERT INTO Producto (Nombre, Descripcion, StockMin, StockActual, PrecioVenta, IdCategoria, IdMarca, IdEstado)
                         VALUES (@Nombre, @Descripcion,  @StockMin, @StockActual, @PrecioVenta, @IdCategoria, @IdMarca, @IdEstado)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", productoDto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", productoDto.Descripcion);
                    cmd.Parameters.AddWithValue("@StockMin", productoDto.StockMin);
                    cmd.Parameters.AddWithValue("@StockActual", productoDto.StockActual);
                    cmd.Parameters.AddWithValue("@PrecioVenta", productoDto.PrecioVenta);
                    cmd.Parameters.AddWithValue("@IdCategoria", productoDto.IdCategoria);
                    cmd.Parameters.AddWithValue("@IdMarca", productoDto.IdMarca);
                    cmd.Parameters.AddWithValue("@IdEstado", productoDto.IdEstado);

                    cmd.ExecuteNonQuery(); // No necesitamos devolver un valor
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public void EditarProducto(ProductoDto productoDto)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"UPDATE Producto
                         SET Nombre = @Nombre, Descripcion = @Descripcion, StockMin = @StockMin, StockActual = @StockActual, 
                             PrecioVenta = @PrecioVenta, IdCategoria = @IdCategoria, 
                             IdMarca = @IdMarca, IdEstado = @IdEstado
                         WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", productoDto.Id);
                    cmd.Parameters.AddWithValue("@Nombre", productoDto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", productoDto.Descripcion);
                    cmd.Parameters.AddWithValue("@StockMin", productoDto.StockMin);
                    cmd.Parameters.AddWithValue("@StockActual", productoDto.StockActual);
                    cmd.Parameters.AddWithValue("@PrecioVenta", productoDto.PrecioVenta);
                    cmd.Parameters.AddWithValue("@IdCategoria", productoDto.IdCategoria);
                    cmd.Parameters.AddWithValue("@IdMarca", productoDto.IdMarca);
                    cmd.Parameters.AddWithValue("@IdEstado", productoDto.IdEstado);

                    cmd.ExecuteNonQuery(); // No necesitamos devolver un valor
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public void EliminarProducto(int id, string usuario)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                // 1. Establecer el usuario en el contexto de sesión
                using (SqlCommand cmdSetUser = new SqlCommand("EXEC sp_set_session_context @key = N'Usuario', @value = @Usuario", conn))
                {
                    cmdSetUser.Parameters.AddWithValue("@Usuario", usuario);
                    cmdSetUser.ExecuteNonQuery();
                }

                // 2. Ejecutar el DELETE (el trigger lo interceptará)
                string query = @"DELETE FROM Producto WHERE Id = @Id";
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
        public ProductoDto ObtenerProductoPorId(int id)
        {
            ProductoDto producto = null;
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT p.Id, p.Nombre, p.Descripcion, p.StockMin, p.StockActual, p.PrecioVenta, 
                                p.IdCategoria, c.NombreCategoria, 
                                p.IdMarca, m.NombreMarca, 
                                p.IdEstado, e.NombreEstado
                         FROM Producto p
                         JOIN Categoria c ON p.IdCategoria = c.Id
                         JOIN Marca m ON p.IdMarca = m.Id
                         JOIN Estado e ON p.IdEstado = e.Id
                         WHERE p.Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new ProductoDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                StockMin = Convert.ToInt32(reader["StockMin"]),
                                StockActual = Convert.ToInt32(reader["StockActual"]),
                                PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"]),
                                IdCategoria = Convert.ToInt32(reader["IdCategoria"]),
                                NombreCategoria = reader["NombreCategoria"].ToString(),
                                IdMarca = Convert.ToInt32(reader["IdMarca"]),
                                NombreMarca = reader["NombreMarca"].ToString(),
                                IdEstado = Convert.ToInt32(reader["IdEstado"]),
                                NombreEstado = reader["NombreEstado"].ToString()
                            };
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return producto;
        }
        public ProductoDto ObtenerProductoPorNombreYMarca(string nombre, int idMarca)
        {
            ProductoDto producto = null;
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT * FROM Producto WHERE Nombre = @Nombre AND IdMarca = @IdMarca";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@IdMarca", idMarca);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new ProductoDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                IdMarca = Convert.ToInt32(reader["IdMarca"])
                                // Traer más datos si necesitas
                            };
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return producto;
        }
        public List<ProductoDto> ObtenerProductosActivos()
        {
            List<ProductoDto> productos = new List<ProductoDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = @"SELECT Id, Nombre, PrecioVenta, StockActual
                         FROM Producto
                         WHERE IdEstado = 1";
        
        using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductoDto producto = new ProductoDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"]),
                                StockActual = Convert.ToInt32(reader["StockActual"])
                                // Agrega más campos si los necesitas, pero SOLO usa campos que pidas en el SELECT
                            };

                            productos.Add(producto);
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return productos;
        }
        public void ActualizarPrecioVenta(int idProducto, decimal nuevoPrecio)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "UPDATE Producto SET PrecioVenta = @PrecioVenta WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PrecioVenta", nuevoPrecio);
                    cmd.Parameters.AddWithValue("@Id", idProducto);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.Disconnect();
            }
        }
        public List<ProductoDto> ObtenerProductosProximosAgotarse()
        {
            var productos = new List<ProductoDto>();
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();
                string query = "SELECT Id, Nombre, StockActual, StockMin FROM Producto WHERE StockActual < StockMin";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new ProductoDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                StockActual = Convert.ToInt32(reader["StockActual"]),
                                StockMin = Convert.ToInt32(reader["StockMin"])
                            };

                            productos.Add(producto);
                        }
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }

            return productos;
        }
        public List<ProductoDto> ObtenerProductosMasVendidosDelDia(DateTime fecha)
        {
            var db = new ContextoBDUtilidad();
            var conn = db.CONN();

            try
            {
                db.Connect();

                string query = @"
        SELECT TOP 5
            p.Id, 
            p.Nombre, 
            SUM(dv.Cantidad) AS TotalVendido
        FROM Venta v
        INNER JOIN DETALLE_VENTA dv ON dv.IdVenta = v.Id
        INNER JOIN Producto p ON p.Id = dv.IdProducto
        WHERE CAST(v.FechaVenta AS DATE) = @Fecha
        GROUP BY p.Id, p.Nombre
        ORDER BY TotalVendido DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Fecha", fecha.Date);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<ProductoDto> productosVendidos = new List<ProductoDto>();

                        while (reader.Read())
                        {
                            productosVendidos.Add(new ProductoDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                CantidadVendida = Convert.ToInt32(reader["TotalVendido"])
                            });
                        }

                        return productosVendidos;
                    }
                }
            }
            finally
            {
                db.Disconnect();
            }
        }


    }
}