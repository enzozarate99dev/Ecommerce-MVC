﻿using Microsoft.EntityFrameworkCore;
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
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _repositorio;

        public CategoriaService(IGenericRepository<Categoria> repositorio) 
        {
            _repositorio = repositorio; 
        }

        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await _repositorio.Consultar();
            string sqlQuery = query.ToQueryString();
            Console.WriteLine(sqlQuery);
            return query.ToList();
        }
        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria categoriaCreada = await _repositorio.Crear(entidad);
                
                if(categoriaCreada.IdCategoria == 0)
                    throw new TaskCanceledException("No se pudo crear la categoria");
 
                return categoriaCreada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Categoria> Editar(Categoria entidad)
        {
            try
            {
                Categoria categoriaEncontrada = await _repositorio.Obtener(c => c.IdCategoria == entidad.IdCategoria);
                categoriaEncontrada.Descripcion = entidad.Descripcion;
                categoriaEncontrada.EsActivo = entidad.EsActivo;

                bool respuesta = await _repositorio.Editar(categoriaEncontrada);
                
                
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar la categoria");
                
                return categoriaEncontrada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria categoriaEncontrada = await _repositorio.Obtener(c => c.IdCategoria == idCategoria);
                if (categoriaEncontrada == null)
                    throw new TaskCanceledException("Categoria no existe");

                bool respuesta = await _repositorio.Eliminar(categoriaEncontrada);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        
    }
}
