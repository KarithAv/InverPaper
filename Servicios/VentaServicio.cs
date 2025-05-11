using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InverPaper.Dtos;
using InverPaper.Repositorios;

namespace InverPaper.Servicios
{
    public class VentaServicio
    {
        private ProductoRepositorio _productoRepositorio = new ProductoRepositorio();
        private VentaRepositorio _ventaRepositorio = new VentaRepositorio();

        // Obtener los productos activos
        public List<SelectListItem> ObtenerProductosDisponibles()
        {
            var productosActivos = _productoRepositorio.ObtenerProductosActivos(); // Fíjate, ya no es "ObtenerProductoActivo"
            return productosActivos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nombre
            }).ToList();
        }

        // Lógica para registrar una venta
        public void RegistrarVenta(VentaDto venta, List<DetalleVentaDto> detallesVenta)
        {
            _ventaRepositorio.RegistrarVenta(venta, detallesVenta);
        }
        public List<SelectListItem> ObtenerMetodosPagoDisponibles()
        {
            var lista = new List<SelectListItem>();
            var metodos = new MetodoPagoRepositorio().ObtenerMetodosPago(); // Asume que devuelve List<MetodoPagoDto>

            foreach (var metodo in metodos)
            {
                lista.Add(new SelectListItem
                {
                    Value = metodo.Id.ToString(),
                    Text = metodo.MetodoPago
                });
            }

            return lista;
        }
        public List<VentaDto> ObtenerVentasDelDia(DateTime fecha)
        {
            return _ventaRepositorio.ObtenerVentasDelDia(fecha);
        }
        public List<ProductoDto> ObtenerProductosMasVendidosDelDia(DateTime fecha)
        {
            return _productoRepositorio.ObtenerProductosMasVendidosDelDia(fecha);
        }
    }
}