using Microsoft.EntityFrameworkCore;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.DAL.Interfaces;
using SistemaVentas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.BLL.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _repositorio;
        private readonly IFirebaseService _firebaseService;

        public ProductoService(IGenericRepository<Producto> repositorio, IFirebaseService firebaseService)
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
        }
        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _repositorio.Consultar();
            return query.Include(p => p.IdCategoriaNavigation).ToList();
        }
        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);

            if (productoExiste != null)
                throw new TaskCanceledException("El codigo ya existe");

            try
            {
                entidad.NombreImagen = nombreImagen;
                if (imagen != null) 
                {
                    string urlImg = await _firebaseService.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen = urlImg;
                }

                Producto productoCreado = await _repositorio.Crear(entidad);    

                if(productoCreado.IdProducto == 0)
                    throw new TaskCanceledException("No se pudo crear el producto");

                IQueryable<Producto> query = await _repositorio.Consultar(p => p.IdProducto == productoCreado.IdProducto);

                productoCreado = query.Include(c => c.IdCategoriaNavigation).First();
                return productoCreado;


            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);

            if (productoExiste != null)
                throw new TaskCanceledException("El codigo ya existe");

            try
            {
                IQueryable<Producto> queryProducto = await _repositorio.Consultar(p => p.IdProducto == entidad.IdProducto);

                Producto productoEditar = queryProducto.First();

                productoEditar.CodigoBarra = entidad.CodigoBarra;
                productoEditar.Marca = entidad.Marca;
                productoEditar.Descripcion = entidad.Descripcion;
                productoEditar.IdCategoria = entidad.IdCategoria;
                productoEditar.Stock = entidad.Stock;
                productoEditar.Precio = entidad.Precio;
                productoEditar.EsActivo = entidad.EsActivo;

                if (productoEditar.NombreImagen == "")
                    productoEditar.NombreImagen = nombreImagen;


                if (imagen != null)
                {
                    string urlImg = await _firebaseService.SubirStorage(imagen, "carpeta_producto", productoEditar.NombreImagen);
                    productoEditar.UrlImagen = urlImg;
                }

                bool resp = await _repositorio.Editar(productoEditar);
                if (!resp) 
                    throw new TaskCanceledException("No se pudo editar");

                Producto productoEditado = queryProducto.Include(c=>c.IdCategoriaNavigation).First();

                
                return productoEditado;


            }
            catch 
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto encontrado = await _repositorio.Obtener(p => p.IdProducto == idProducto);

                if (encontrado == null)
                    throw new TaskCanceledException("el producto no existe");

                string nombreImg = encontrado.NombreImagen;
                bool resp = await _repositorio.Eliminar(encontrado);

                if (resp)
                    await _firebaseService.EliminarStorage("carpeta_prodcuto", nombreImg);

                return true;
            }
            catch
            {
                throw;
            }
        }

        
    }
}
