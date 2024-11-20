using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SistemaVentas.AplicacionWeb.Models.ViewModels;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.Entity;
using System.Security.Claims;

namespace SistemaVentas.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        public AccesoController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");

            }

            return View();
        }

        public IActionResult ReestablecerClave()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo)
        {
            Usuario encontrado = await _usuarioService.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

            if (encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View(modelo);
            }

            ViewData["Mensaje"] = null;

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, encontrado.IdRol.ToString()),
                new Claim("UrlFoto", encontrado.UrlFoto ?? ""),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async  Task<IActionResult> ReestablecerClave(VMUsuarioLogin modelo)
        {

            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/ReestablecerClave?clave=[clave]"; //Cambiar Host a url del hosting al publicar

                bool resultado = await _usuarioService.ReestablecerClave(modelo.Correo, urlPlantillaCorreo);

                if (resultado)
                {
                    ViewData["Mensaje"] = "Contraseña reestablecida. Revise su correo";
                    ViewData["MensajeError"] = null;

                }
                else
                {
                    ViewData["MensajeError"] = "Ocurrió un problema, intente mas tarde";
                    ViewData["Mensaje"] = null;
                }

            }
            catch(Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
                ViewData["Mensaje"] = null;
            }

            return View();
        }

    }
}
