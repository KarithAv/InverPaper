using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Repositorios;
using InverPaper.Dtos;

namespace InverPaper.Servicios
{
    public class AjustesInventarioServicio
    {
        private AjustesInventarioRepositorio _ajustesRepo = new AjustesInventarioRepositorio();

        public void RegistrarAjuste(AjusteInventarioDto dto)
        {
            // Validaciones básicas
            if (dto.CantidadAjustada == 0)
                throw new Exception("La cantidad ajustada no puede ser cero.");

            _ajustesRepo.RegistrarAjuste(dto);
        }
    }
}