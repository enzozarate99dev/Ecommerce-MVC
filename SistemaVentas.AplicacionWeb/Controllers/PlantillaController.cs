using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaVentas.AplicacionWeb.Models.ViewModels;
using SistemaVentas.BLL.Interfaces;

namespace SistemaVentas.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
        private readonly IVentaService _ventaService;

        public PlantillaController(IMapper mapper, INegocioService negocioService, IVentaService ventaService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
            _ventaService = ventaService;
        }
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

            return View();
        }
        public IActionResult ReestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;

            return View();
        }

        public async Task<IActionResult> PDFVenta(string nroVenta)
        {
            VMVenta vmVenta = _mapper.Map<VMVenta>(await _ventaService.Detalle(nroVenta));
            VMNegocio vmNegocio = _mapper.Map <VMNegocio>(await _negocioService.Obtener());

            VMPDFVenta modelo = new VMPDFVenta();

            modelo.venta = vmVenta;
            modelo.negocio = vmNegocio;

            return View(modelo);
        }
    }
}
