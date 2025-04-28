using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Dtos;
using InverPaper.Repositorios;

namespace InverPaper.Servicios
{
    public class ProductoServicio
    {
        private ProductoRepositorio _productoRepositorio = new ProductoRepositorio();
        public void CrearProducto(ProductoDto productoDto)
        {
            var productoExistente = _productoRepositorio.ObtenerProductoPorNombreYMarca(productoDto.Nombre, productoDto.IdMarca);

            if (productoExistente != null)
            {
                throw new Exception("Ya existe un producto con el mismo nombre y marca.");
            }

            _productoRepositorio.CrearProducto(productoDto);
        }

        // Editar un producto existente
        public void EditarProducto(ProductoDto productoDto)
        {
            var productoExistente = _productoRepositorio.ObtenerProductoPorNombreYMarca(productoDto.Nombre, productoDto.IdMarca);

            if (productoExistente != null && productoExistente.Id != productoDto.Id)
            {
                // Si existe otro producto distinto con el mismo nombre y marca, no se permite
                throw new Exception("Ya existe otro producto con el mismo nombre y marca.");
            }
            _productoRepositorio.EditarProducto(productoDto);
        }

        // Eliminar un producto por ID
        public void EliminarProducto(int id)
        {
            // Puedes agregar validaciones aquí si es necesario
            _productoRepositorio.EliminarProducto(id);
        }
    }
}