using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using StokSayim.Data;


namespace StokSayim.Data.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly DatabaseContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual T GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public virtual void Add(T entity)
        {
            try
            {
                _dbSet.Add(entity);
                var result = _context.SaveChanges();
                Console.WriteLine($"Entity added. SaveChanges returned: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Add method: {ex.Message}");
                throw; // Hatayı yukarı fırlat
            };
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.AddRange(entities);
                var result = _context.SaveChanges();
                Console.WriteLine($"Entities added. SaveChanges returned: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddRange method: {ex.Message}");
                throw; // Hatayı yukarı fırlat
            }
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public virtual void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}
