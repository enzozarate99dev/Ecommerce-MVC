using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVentas.AplicacionWeb.Models.ViewModels;
using SistemaVentas.AplicacionWeb.Utilidades.Response;
using SistemaVentas.BLL.Implementacion;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.Entity;

namespace SistemaVentas.AplicacionWeb.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _productoService;

        public ProductoController(IMapper mapper, IProductoService productoService)
        {
            _mapper = mapper;
            _productoService = productoService;
        }
    
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _productoService.Lista();
            var vmListaProductos = _mapper.Map<List<VMProducto>>(lista);

            return Json(new { data = vmListaProductos });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreImg = "";
                Stream imgStream = null;

                if (imagen != null)
                {
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImg = string.Concat(nombreCodificado, extension);
                    imgStream = imagen.OpenReadStream();
                }


                Producto productoCreado = await _productoService.Crear(_mapper.Map<Producto>(vmProducto), imgStream, nombreImg);

                vmProducto = _mapper.Map<VMProducto>(productoCreado);

                gResponse.Estado = true;
                gResponse.Objeto = vmProducto;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreImg = "";
                Stream imgStream = null;

                if (imagen != null)
                {
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImg = string.Concat(nombreCodificado, extension);
                    imgStream = imagen.OpenReadStream();
                }


                Producto productoEditado = await _productoService.Editar(_mapper.Map<Producto>(vmProducto), imgStream, nombreImg);

                vmProducto = _mapper.Map<VMProducto>(productoEditado);

                gResponse.Estado = true;
                gResponse.Objeto = vmProducto;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idProducto)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _productoService.Eliminar(idProducto);
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
