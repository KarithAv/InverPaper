using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InverPaper.Repositorios;
using InverPaper.Dtos;

namespace InverPaper.Servicios
{
    public class LoteServicio
    {
        private readonly LoteRepositorio _loteRepo = new LoteRepositorio();
        private readonly ProductoRepositorio _productoRepo = new ProductoRepositorio();

        public LoteDto RegistrarLote(LoteDto lote)
        {
            var result = new LoteDto();

            // Obtener producto para comparar precios
            var producto = _productoRepo.ObtenerProductoPorId(lote.IdProducto);

            if (producto == null)
            {
                result.Response = 0;
                result.Mensaje = "Producto no encontrado.";
                return result;
            }

            // Validación básica
            if (lote.Cantidad <= 0 || lote.PrecioCompra <= 0)
            {
                result.Response = 0;
                result.Mensaje = "La cantidad y el precio deben ser mayores a cero.";
                return result;
            }

            // Detectar diferencia de precio
            if (producto.PrecioVenta != 0 && lote.PrecioCompra != producto.PrecioVenta)
            {
                result.Response = 2; // Código especial para advertencia de diferencia
                result.Mensaje = $"El precio ingresado ({lote.PrecioCompra:C}) es diferente al anterior ({producto.PrecioVenta:C}). ¿Deseas ajustarlo?";
                return result;
            }

            // Registrar el lote
            _loteRepo.RegistrarLote(lote);

            result.Response = 1;
            result.Mensaje = "Lote registrado con éxito.";
            return result;
        }
    }
}