using Microsoft.EntityFrameworkCore;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.DAL.Interfaces;
using SistemaVentas.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.BLL.Implementacion
{
    public class DashboardService : IDashboardService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepository;
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IGenericRepository<Categoria> _categoriaVentaRepository;
        private DateTime FechaInicio = DateTime.Now;

        public DashboardService(IVentaRepository ventaRepository, 
            IGenericRepository<DetalleVenta> detalleVentaRepository, 
            IGenericRepository<Producto> productoRepository, 
            IGenericRepository<Categoria> categoriaVentaRepository)
        {
            _ventaRepository = ventaRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _productoRepository = productoRepository;
            _categoriaVentaRepository = categoriaVentaRepository;

            FechaInicio = FechaInicio.AddDays(-7);
        }

        public async Task<int> TotalVentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                decimal resp = query.Select(v => v.Total).Sum(v => v.Value);
                return Convert.ToString(resp, new CultureInfo("es-AR"));
            }
            catch
            {
                throw;
            }
        }
        
        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Producto> query = await _productoRepository.Consultar();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> TotalCategorias()
        {
            try
            {
                IQueryable<Categoria> query = await _categoriaVentaRepository.Consultar();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }



        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);

                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);
                return resultado;

            }
            catch
            {
                throw;
            }
        }
        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _detalleVentaRepository.Consultar();

                Dictionary<string, int> resultado = query
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date)
                    .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count())
                    .Select(dv => new { producto = dv.Key, total = dv.Count() })
                    .Take(4)
                    .ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total);
                return resultado;

            }
            catch
            {
                throw;
            }
        }
    }
}
