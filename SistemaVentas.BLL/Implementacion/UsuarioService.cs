using Microsoft.EntityFrameworkCore;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.DAL.Interfaces;
using SistemaVentas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFirebaseService _firebaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;

        public UsuarioService(IGenericRepository<Usuario> reposirotio, IFirebaseService firebaseService, IUtilidadesService utilidadesService, ICorreoService correoService)
        {
            _repositorio = reposirotio;
            _firebaseService = firebaseService;
            _correoService = correoService;
            _utilidadesService = utilidadesService;
        }
            
        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(rol => rol.IdRolNavigation).ToList();
        }
        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario usuarioExiste = await _repositorio.Obtener(u => u.Correo == entidad.Correo);

            if(usuarioExiste != null)
                throw new TaskCanceledException("El usuario ya existe");
            

            try
            {
                string claveGenerada =  _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(claveGenerada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null) {
                    string urlFoto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto); // aqui falla, firebase no devuelve una url
                    entidad.UrlFoto = urlFoto;  
                }

                Usuario usuarioCreado = await _repositorio.Crear(entidad);

                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("El usuario no pudo ser creado");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]", claveGenerada);

                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream data = response.GetResponseStream())
                        {
                            StreamReader reader = null;

                            if (response.CharacterSet == null)
                                reader = new StreamReader(data);
                            else
                                reader = new StreamReader(data, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = reader.ReadToEnd();
                            response.Close();
                            reader.Close(); 
                        }
                    }
                    if (htmlCorreo != "")                
                        await _correoService.EnviarCorreo(usuarioCreado.Correo,"Cuenta creada", htmlCorreo);                        
                }
                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(r => r.IdRolNavigation).First();
                return usuarioCreado;
            }
            catch (Exception ex) {
                throw;
            }

        }

        public async  Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            Usuario usuarioExiste = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if (usuarioExiste != null)
                throw new TaskCanceledException("El correo ya existe");
            try
            {
                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                
                Usuario usuarioEditar = query.First();
                usuarioEditar.Nombre = entidad.Nombre;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.Telefono = entidad.Telefono;
                usuarioEditar.IdRol = entidad.IdRol;
                usuarioEditar.EsActivo = entidad.EsActivo;

                if (usuarioEditar.NombreFoto == "")
                    usuarioEditar.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    usuarioEditar.UrlFoto = urlFoto;
                }

                bool resp = await _repositorio.Editar(usuarioEditar);
                
                if (!resp)
                    throw new TaskCanceledException("El usuario no se puede editar");

                Usuario usuarioEditado = query.Include(r=>r.IdRolNavigation).First(); 
                   
                return usuarioEditado;

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario encontrado =  await _repositorio.Obtener(u=>u.IdUsuario == idUsuario);

                if (encontrado == null)
                    throw new TaskCanceledException("el usuario no existe");

                string nombreFoto = encontrado.NombreFoto;
                bool resp = await _repositorio.Eliminar(encontrado);

                if (resp)
                    await _firebaseService.EliminarStorage("carpeta_usuario", nombreFoto);
              
                return true;
            }
            catch {
                throw;
            }
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string claveEncriptada = _utilidadesService.ConvertirSha256(clave);
            Usuario encontrado = await _repositorio.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(clave));
            return encontrado;
        }

        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == idUsuario);

            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();
            return resultado;
        }
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario u_encontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                if(u_encontrado==null)
                    throw new TaskCanceledException("el usuario no existe");

                u_encontrado.Correo = entidad.Correo;
                u_encontrado.Telefono = entidad.Telefono;

                bool resp = await _repositorio.Editar(u_encontrado);
                return resp;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario u_encontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                if (u_encontrado == null)
                    throw new TaskCanceledException("el usuario no existe");

                if(u_encontrado.Clave != _utilidadesService.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("La contraseña no es correcta");

                u_encontrado.Clave = _utilidadesService.ConvertirSha256(claveNueva);

                bool resp = await _repositorio.Editar(u_encontrado);
                
                return resp;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> ReestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario u_encontrado = await _repositorio.Obtener(u => u.Correo == correo);

                if (u_encontrado == null)
                    throw new TaskCanceledException("el usuario asociado al correo no se encontro");

                string claveGenerada = _utilidadesService.GenerarClave();

                u_encontrado.Clave = _utilidadesService.ConvertirSha256(claveGenerada);

                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", claveGenerada);

                string htmlCorreo = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream data = response.GetResponseStream())
                    {
                        StreamReader reader = null;

                        if (response.CharacterSet == null)
                            reader = new StreamReader(data);
                        else
                            reader = new StreamReader(data, Encoding.GetEncoding(response.CharacterSet));

                        htmlCorreo = reader.ReadToEnd();
                        response.Close();
                        reader.Close();
                    }
                }

                bool correoEnv = false;

                if (htmlCorreo != "")
                    correoEnv =  await _correoService.EnviarCorreo(correo, "clave reestablecida", htmlCorreo);

                if(correoEnv==false)
                    throw new TaskCanceledException("No se pudo enviar el correo. Intente nuevamente mas tarde");

                bool resp = await _repositorio.Editar(u_encontrado);
                return resp;

            }
            catch
            {
                throw;
            }
        }
    }
}
