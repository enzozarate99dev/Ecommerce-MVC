﻿using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVentas.AplicacionWeb.Models.ViewModels;
using SistemaVentas.AplicacionWeb.Utilidades.Response;
using SistemaVentas.BLL.Implementacion;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.Entity;

namespace SistemaVentas.AplicacionWeb.Controllers
{
    public class VentaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ITipoDocumentoVentaService _tipoDocumentoVentaService;
        private readonly IVentaService _ventaService;
        private readonly IConverter _converter;

        public VentaController(IMapper mapper, ITipoDocumentoVentaService tipoDocumentoVentaService, IVentaService ventaService, IConverter converter)
        {
            _mapper = mapper;
            _tipoDocumentoVentaService = tipoDocumentoVentaService;
            _ventaService = ventaService;
            _converter = converter;
        }

        public IActionResult NuevaVenta()
        {
            return View();
        }
        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta()
        {
            var lista = await _tipoDocumentoVentaService.Lista();
            List<VMTipoDocumentoVenta> vmListaTipoDocumentoVenta = _mapper.Map<List<VMTipoDocumentoVenta>>(lista);

            //return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumentoVenta);
            return Json(new { data = vmListaTipoDocumentoVenta });
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            var lista = await _ventaService.ObtenerProductos(busqueda);
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(lista);

            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }
        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta modelo)
        {
           

            if (modelo == null)
            {
                return BadRequest("El modelo de venta es nulo");
            }
            GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();

            try
            {
                modelo.IdUsuario = 1;

                Venta ventaCreada = await _ventaService.Registrar(_mapper.Map<Venta>(modelo));

                modelo = _mapper.Map<VMVenta>(ventaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
        [HttpGet]
        public async Task<IActionResult> Historial(string nroVenta, string fechaInicio, string fechaFin)
        {
            var lista = await _ventaService.Historial(nroVenta,fechaInicio,fechaFin);
            List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(lista);

            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);

        }

        public IActionResult MostrarPDFVenta(string nroVenta)
        {
            string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFVenta?nroVenta={nroVenta}";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = urlPlantillaVista
                    }
                }
            };

            var archivo = _converter.Convert(pdf);
            return File(archivo, "application/pdf");
            
        }
    }
}
