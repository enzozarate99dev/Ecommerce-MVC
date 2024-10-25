using AutoMapper;
using SistemaVentas.AplicacionWeb.Models.ViewModels;
using SistemaVentas.Entity;
using System.Globalization;

namespace SistemaVentas.AplicacionWeb.Utilidades.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            #region Rol
            CreateMap<Rol, VMRol>().ReverseMap();
            #endregion

            #region Usuario
            CreateMap<Usuario, VMUsuario>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                )
                .ForMember(dest =>
                dest.NombreRol,
                opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion)
                );
            CreateMap<VMUsuario, Usuario>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(dest =>
                dest.IdRolNavigation,
                opt => opt.Ignore()
                );

           
            #endregion

            #region Negocio
            CreateMap<Negocio, VMNegocio>()
                .ForMember(dest =>
                dest.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value, new CultureInfo("es-AR")))
                );
            CreateMap<VMNegocio, Negocio>()
                .ForMember(dest =>
                dest.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto, new CultureInfo("es-AR")))
                );
            #endregion

            #region Categoria
            CreateMap<Categoria, VMCategoria>()
                .ForMember(dest =>
                    dest.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                );

            CreateMap<VMCategoria, Categoria>()
                .ForMember(dest =>
                    dest.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                );
            #endregion

            #region Producto
            CreateMap<Producto, VMProducto>()
                .ForMember(dest =>
                dest.EsActivo,
                opt => opt.MapFrom(o => o.EsActivo == true ? 1 : 0))
                .ForMember(dest =>
                dest.NombreCategoria,
                opt => opt.MapFrom(o => o.IdCategoriaNavigation.Descripcion))
            .ForMember(dest =>
                dest.Precio,
                opt => opt.MapFrom(o => Convert.ToString(o.Precio.Value, new CultureInfo("es-AR"))));

            CreateMap<VMProducto, Producto>()
               .ForMember(dest =>
               dest.EsActivo,
               opt => opt.MapFrom(o => o.EsActivo == 1 ? true : false))
               .ForMember(dest =>
               dest.IdCategoriaNavigation,
               opt => opt.Ignore())
               .ForMember(dest =>
               dest.Precio,
               opt => opt.MapFrom(o => Convert.ToDecimal(o.Precio, new CultureInfo("es-AR"))));

            #endregion

            #region TipoDocumentoVta
            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap();
            #endregion

            #region Venta
            CreateMap<Venta, VMVenta>()
                .ForMember(dest =>
                dest.TipoDocumentoVenta,
                opt => opt.MapFrom(o => o.IdTipoDocumentoVentaNavigation.Descripcion))
                
                .ForMember(dest =>
                dest.Usuario,
                opt => opt.MapFrom(o => o.IdUsuarioNavigation.Nombre))
                
                .ForMember(dest =>
                dest.SubTotal,
                opt => opt.MapFrom(o => Convert.ToString(o.SubTotal.Value, new CultureInfo("es-AR"))))
                
                .ForMember(dest =>
                dest.ImpuestoTotal,
                opt => opt.MapFrom(o => Convert.ToString(o.ImpuestoTotal.Value, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.Total,
                opt => opt.MapFrom(o => Convert.ToString(o.Total.Value, new CultureInfo("es-AR"))))
                
                .ForMember(dest =>
                dest.FechaRegistro,
                opt => opt.MapFrom(o => o.FechaRegistro.Value.ToString("dd/MM/yyyy")));


            CreateMap<VMVenta, Venta>()

                .ForMember(dest =>
                dest.SubTotal,
                opt => opt.MapFrom(o => Convert.ToDecimal(o.SubTotal, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.ImpuestoTotal,
                opt => opt.MapFrom(o => Convert.ToDecimal(o.ImpuestoTotal, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.Total,
                opt => opt.MapFrom(o => Convert.ToDecimal(o.Total, new CultureInfo("es-AR"))));

            #endregion

            #region DetalleVenta
            CreateMap<DetalleVenta, VMDetalleVenta>()
                .ForMember(dest =>
                dest.Precio,
                opt => opt.MapFrom(o => Convert.ToString(o.Precio.Value, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.Total,
                opt => opt.MapFrom(o => Convert.ToString(o.Total.Value, new CultureInfo("es-AR"))));

            CreateMap<VMDetalleVenta, DetalleVenta>()
                .ForMember(dest =>
                dest.Precio,
                opt => opt.MapFrom(o => Convert.ToDecimal(o.Precio, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.Total,
                opt => opt.MapFrom(o => Convert.ToDecimal(o.Total, new CultureInfo("es-AR"))));


            CreateMap<DetalleVenta, VMReporteVenta>()
                .ForMember(dest =>
                dest.FechaRegistro,
                opt => opt.MapFrom(o => o.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy")))

                .ForMember(dest =>
                dest.NroVenta,
                opt => opt.MapFrom(o => o.IdVentaNavigation.NumeroVenta))

                .ForMember(dest =>
                dest.TipoDocumento,
                opt => opt.MapFrom(o => o.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion))

                .ForMember(dest =>
                dest.DocumentoCliente,
                opt => opt.MapFrom(o => o.IdVentaNavigation.DocumentoCliente))

                .ForMember(dest =>
                dest.NombreCliente,
                opt => opt.MapFrom(o => o.IdVentaNavigation.NombreCliente))

                .ForMember(dest =>
                dest.SubtotalVenta,
                opt => opt.MapFrom(o => Convert.ToString(o.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.ImpuestoTotalVenta,
                opt => opt.MapFrom(o => Convert.ToString(o.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.TotalVenta,
                opt => opt.MapFrom(o => Convert.ToString(o.IdVentaNavigation.Total.Value, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.Producto,
                opt => opt.MapFrom(o => o.DescripcionProducto))

                .ForMember(dest =>
                dest.Precio,
                opt => opt.MapFrom(o => Convert.ToString(o.Precio.Value, new CultureInfo("es-AR"))))

                .ForMember(dest =>
                dest.Total,
                opt => opt.MapFrom(o => Convert.ToString(o.Total.Value, new CultureInfo("es-AR"))));
            #endregion

            #region Menu
            CreateMap<Menu, VMMenu>()
                .ForMember(dest =>
                dest.SubMenus,
                opt => opt.MapFrom(o=>o.InverseIdMenuPadreNavigation));
            #endregion
        }
    }
}
