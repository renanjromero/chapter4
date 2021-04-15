﻿using Microsoft.EntityFrameworkCore.Storage;
using System;
using Wiz.Chapter4.Domain.Interfaces.UoW;
using Wiz.Chapter4.Infra.Context;

namespace Wiz.Chapter4.Infra.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly EntityContext _entityContext;
        private IDbContextTransaction _transaction;

        public UnitOfWork(EntityContext entityContext)
        {
            _entityContext = entityContext;
        }

        public int Commit()
        {
            return _entityContext.SaveChanges();
        }

        public void BeginTransaction()
        {
            _transaction = _entityContext.Database.BeginTransaction();
        }

        public void BeginCommit()
        {
            _transaction.Commit();
        }

        public void BeginRollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _entityContext.Dispose();

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
