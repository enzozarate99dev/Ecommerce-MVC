using Microsoft.EntityFrameworkCore;
using SistemaVentas.DAL.DBContext;
using SistemaVentas.DAL.Interfaces;
using SistemaVentas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbContext;

        public VentaRepository(DbventaContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta detalle in entidad.DetalleVenta)
                    {

                        Producto prodEncontrado = _dbContext.Productos.Where(p => p.IdProducto == detalle.IdProducto).First();
                        prodEncontrado.Stock -= detalle.Cantidad;
                        _dbContext.Update(prodEncontrado);
                    }
                    await _dbContext.SaveChangesAsync();

                    NumeroCorrelativo correlativo = _dbContext.NumerosCorrelativos.Where(n => n.Gestion == "venta").First();

                    correlativo.UltimoNumero += 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbContext.NumerosCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string nroVenta = ceros + correlativo.UltimoNumero.ToString();
                    nroVenta = nroVenta.Substring(nroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = nroVenta;
                    await _dbContext.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FInicio, DateTime FFin)
        {
            List<DetalleVenta> listaResumen = await _dbContext.DetalleVentas
                .Include(v=>v.IdVentaNavigation)
                    .ThenInclude(u=>u.IdUsuarioNavigation)
                .Include(v=>v.IdVentaNavigation)
                    .ThenInclude(tdv=>tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv=>dv.IdVentaNavigation.FechaRegistro.Value.Date >= FInicio.Date &&
                    dv.IdVentaNavigation.FechaRegistro.Value.Date <= FFin.Date).ToListAsync();
            return listaResumen;
        }

        
    }
}
