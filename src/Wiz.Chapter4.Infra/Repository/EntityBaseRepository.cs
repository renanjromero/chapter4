﻿using Microsoft.EntityFrameworkCore;
using System;
using Wiz.Chapter4.Domain.Interfaces.Repository;
using Wiz.Chapter4.Infra.Context;

namespace Wiz.Chapter4.Infra.Repository
{
    public class EntityBaseRepository<TEntity> : IEntityBaseRepository<TEntity> where TEntity : class
    {
        protected readonly EntityContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public EntityBaseRepository(EntityContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual void Add(TEntity obj)
        {
            DbSet.Add(obj);
        }

        public virtual void Update(TEntity obj)
        {
            DbSet.Update(obj);
        }

        public virtual void Remove(TEntity obj)
        {
            DbSet.Remove(obj);
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}