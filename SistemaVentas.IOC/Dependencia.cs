using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using SistemaVentas.DAL.DBContext;
using SistemaVentas.DAL.Implementacion;
using SistemaVentas.DAL.Interfaces;
using SistemaVentas.BLL.Implementacion;
using SistemaVentas.BLL.Interfaces;
namespace SistemaVentas.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration) {

            services.AddDbContext<DbventaContext>(opt => {
                opt.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
                }
            );

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IVentaRepository, VentaRepository>();

            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IFirebaseService, FirebaseService>();
            
            services.AddScoped<IUtilidadesService, UtilidadesService>();
            services.AddScoped<IRolService, RolService>();

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<INegocioService, NegocioService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IProductoService, ProductoService>();

            services.AddScoped<ITipoDocumentoVentaService, TipoDocumentoVentaService>();
            services.AddScoped<IVentaService, VentaService>();

        }

    }
}
