﻿using SistemaVentas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.BLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> Lista();
        Task<Usuario> Crear(Usuario entidad, Stream Foto=null, string NombreFoto="", string UrlPlantillaCorreo="");
        Task<Usuario> Editar(Usuario entidad, Stream Foto=null, string NombreFoto="");
        Task<bool> Eliminar(int idUsuario);
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave);
        Task<Usuario> ObtenerPorId(int idUsuario);
        Task<bool> GuardarPerfil(Usuario entidad);
        Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva);
        Task<bool> ReestablecerClave(string correo, string urlPlantillaCorreo);
    }
}
