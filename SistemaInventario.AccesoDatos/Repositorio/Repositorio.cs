using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repositorio(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>(); 
        }

        public async Task Agregar(T entidad)
        {
            await dbSet.AddAsync(entidad); //Inser into tabla
        }

        public async Task<T> Obtener(int id)
        {
            return await dbSet.FindAsync(id); //SELECT^* FROM (solo id)
        }


        public async Task<IEnumerable<T>> ObtenerTodos(Expression<Func<T, bool>> filtro = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if(filtro != null) 
            {
                query = query.Where(filtro); // select /* from where...
            }
            if(incluirPropiedades != null)
            {
                foreach(var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); // categoria: marca
                }
            }
            if(OrderBy != null)
            {
                query = OrderBy(query);
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();   

        }


        public async Task<T> ObtenerPrimero(Expression<Func<T, bool>> filtro = null, 
            string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro); // select /* from where...
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); // categoria: marca
                }
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }


        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }

        public void RemoverRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad); 
        }
    }
}
