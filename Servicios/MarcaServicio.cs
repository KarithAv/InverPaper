using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Repositorios;
using InverPaper.Dtos;

namespace InverPaper.Servicios
{
    public class MarcaServicio
    {
        MarcaRepositorio _marcaRepo = new MarcaRepositorio();

        public List<MarcaDto> ListarMarcas()
        {
            return _marcaRepo.ListarMarcas();
        }

        // Método para obtener una categoría por ID
        public MarcaDto ObtenerMarcaPorId(int id)
        {
            return _marcaRepo.ObtenerMarcaPorId(id);
        }

        // Método para crear una nueva categoría
        public void CrearMarca(MarcaDto marcaDto)
        {
            // Validar si ya existe un usuario con ese correo
            if (_marcaRepo.MarcaExiste(marcaDto.NombreMarca))
            {
                throw new ArgumentException("Ya existe una marca con ese nombre.");
            }
            // Aquí podrías agregar lógica adicional, como validaciones
            _marcaRepo.CrearMarca(marcaDto);
        }

        // Método para eliminar una categoría
        public void EliminarMarca (int id)
        {
            _marcaRepo.EliminarMarca(id);
        }
    }
}