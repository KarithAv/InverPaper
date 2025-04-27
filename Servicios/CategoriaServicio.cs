using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Repositorios;
using InverPaper.Dtos;

namespace InverPaper.Servicios

{
    public class CategoriaServicio
    {
        CategoriaRepositorio _categoriaRepo = new CategoriaRepositorio(); 

        public List<CategoriaDto> ListarCategorias()
        {
            return _categoriaRepo.ListarCategorias();
        }

        // Método para obtener una categoría por ID
        public CategoriaDto ObtenerCategoriaPorId(int id)
        {
            return _categoriaRepo.ObtenerCategoriaPorId(id);
        }

        // Método para crear una nueva categoría
        public void CrearCategoria(CategoriaDto categoriaDto)
        {
            // Validar si ya existe un usuario con ese correo
            if (_categoriaRepo.CategoriaExiste(categoriaDto.NombreCategoria))
            {
                throw new ArgumentException("Ya existe una categoria con ese nombre.");
            }
            // Aquí podrías agregar lógica adicional, como validaciones
            _categoriaRepo.CrearCategoria(categoriaDto);
        }

        // Método para eliminar una categoría
        public void EliminarCategoria(int id)
        {
            _categoriaRepo.EliminarCategoria(id);
        }
    }
}