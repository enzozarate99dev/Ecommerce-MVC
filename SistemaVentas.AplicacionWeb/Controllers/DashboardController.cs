using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaVentas.AplicacionWeb.Models.ViewModels;
using SistemaVentas.AplicacionWeb.Utilidades.Response;
using SistemaVentas.BLL.Interfaces;

namespace SistemaVentas.AplicacionWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {

            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();


            try
            {
                VMDashBoard vmDashboard = new VMDashBoard();
                vmDashboard.TotalVentas = await _dashboardService.TotalVentasUltimaSemana();
                vmDashboard.TotalIngresos = await _dashboardService.TotalIngresosUltimaSemana();
                vmDashboard.TotalProductos = await _dashboardService.TotalProductos();
                vmDashboard.TotalCategorias = await _dashboardService.TotalCategorias();

                List<VMVentasSemana> listaVentasSemana = new List<VMVentasSemana>();
                List < VMProductosSemana > listaProductosSemana = new List<VMProductosSemana>();

                foreach(KeyValuePair<string, int> item in await _dashboardService.VentasUltimaSemana())
                {
                    listaVentasSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });

                }
                foreach (KeyValuePair<string, int> item in await _dashboardService.ProductosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });

                }

                vmDashboard.VentasUltimaSemana = listaVentasSemana;
                vmDashboard.ProductosTopUltimaSemana = listaProductosSemana;

                gResponse.Estado = true;
                gResponse.Objeto = vmDashboard;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
