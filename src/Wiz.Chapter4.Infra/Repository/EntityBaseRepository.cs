using Microsoft.EntityFrameworkCore;
using System;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Domain.Validations;
using Wiz.Chapter4.Infra.Context;

namespace Wiz.Chapter4.Infra.Repository
{
    public class EntityBaseRepository<TEntity> : IEntityBaseRepository<TEntity> where TEntity : class
    {
        protected readonly EntityContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public EntityBaseRepository(EntityContext context)
        {
            if(context != null)
            {
                Db = context;
                DbSet = Db.Set<TEntity>();
            }
        }

        public virtual void Add(TEntity obj)
        {
            Precondition.Requires(DbSet != null);

            DbSet.Add(obj);
        }

        public virtual void Update(TEntity obj)
        {
            Precondition.Requires(DbSet != null);

            DbSet.Update(obj);
        }

        public virtual void Remove(TEntity obj)
        {
            Precondition.Requires(DbSet != null);

            DbSet.Remove(obj);
        }

        public void Dispose()
        {
            if(Db != null)
            {
                Db.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}