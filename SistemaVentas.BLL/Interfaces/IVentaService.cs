using SistemaVentas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.BLL.Interfaces
{
    public interface IVentaService
    {
        Task<List<Producto>> ObtenerProductos(string busqueda);
        Task<Venta> Registrar(Venta entidad);
        Task<List<Venta>> Historial(string nroVenta, string fechaInicio, string fechaFin);
        Task<Venta> Detalle(string nroVenta);
        Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin);
    }
}
