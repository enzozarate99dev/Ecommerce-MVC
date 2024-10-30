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
    public class VentaService : IVentaService
    {
        private readonly IGenericRepository<Producto> _repositorio;
        private readonly IVentaRepository _ventaRepositorio;

        public VentaService(IGenericRepository<Producto> repositorio, IVentaRepository ventaRepositorio)
        {
            _repositorio = repositorio;
            _ventaRepositorio = ventaRepositorio;
        }

        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _repositorio.Consultar(p => p.EsActivo == true && p.Stock > 0 && string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda)); 
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }
        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _ventaRepositorio.Registrar(entidad);
            }
            catch
            {
                throw;
            }
        }
      

        public async Task<List<Venta>> Historial(string nroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepositorio.Consultar();
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;

            if (fechaInicio != null) 
            {
                DateTime f_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-AR"));
                DateTime f_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-AR"));
            
                return query.Where(v => 
                    v.FechaRegistro.Value.Date >= f_Inicio.Date &&
                    v.FechaRegistro.Value.Date <= f_Fin.Date 
                )
                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }
            else
            {
               return query.Where(v => v.NumeroVenta == nroVenta
                )
                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }
        }


        public async Task<Venta> Detalle(string nroVenta)
        {
            IQueryable<Venta> query = await _ventaRepositorio.Consultar(v => v.NumeroVenta == nroVenta);

            return query
                
                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList()
                    .First();
        }


        public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
        {
            DateTime f_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-AR"));
            DateTime f_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-AR"));

            List<DetalleVenta> lista = await _ventaRepositorio.Reporte(f_Inicio, f_Fin);

            return lista;
        }
    }
}
