using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.DAL.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class //interfaz restringida para ser implementada solo por clases
    {
        Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro);  // El filtro es una expresión lambda que define las condiciones de búsqueda.
        Task<TEntity> Crear(TEntity entidad);
        Task<bool> Editar(TEntity entidad);
        Task<bool> Eliminar(TEntity entidad);

        // Devuelve una colección de entidades que cumplen con el filtro dado.
        // Si no se proporciona un filtro, se devolverán todas las entidades.
        Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro=null);
        
    }
}
//IQueryable es una interfaz en .NET que permite realizar consultas a una fuente de datos de manera más flexible y eficiente. 
//    A diferencia de IEnumerable, que ejecuta las consultas en memoria, IQueryable permite que las consultas se ejecuten directamente
//    en la fuente de datos, como una base de datos, antes de que se traigan los datos a la aplicación.