using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.AFC.Data.Interfaces;

namespace Fujitsu.AFC.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AFCDataContext _dbContext;

        public UnitOfWork(AFCDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepositoryTransaction BeginTransaction()
        {
            //READ COMMITTED: A query in the current transaction cannot read data modified by another transaction that has not yet committed, 
            // thus preventing dirty reads. However, data can still be modified by other transactions between issuing statements within the 
            // current transaction, so nonrepeatable reads and phantom reads are still possible.The isolation level uses shared locking or 
            // row versioning to prevent dirty reads, depending on whether the READ_COMMITTED_SNAPSHOT database option is enabled.
            // Read Committed is the default isolation level for all SQL Server databases.
            return new RepositoryTransaction(_dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted));
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Refresh(RefreshMode refreshMode)
        {
            var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
            var refreshableObjects = _dbContext.ChangeTracker
                .Entries()
                .Where(w => w.State != EntityState.Added)
                .Select(c => c.Entity)
                .ToList();
            objectContext.Refresh(refreshMode, refreshableObjects);
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_dbContext.Database.Connection.ConnectionString);
        }

        public void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection)
        {
            ExecuteNonQueryStoredProcedure(storedProcedureName, connection, null, null);
        }

        public void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection, SqlTransaction transaction)
        {
            ExecuteNonQueryStoredProcedure(storedProcedureName, connection, transaction, null);
        }

        public void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection, SqlTransaction transaction,
            params SqlParameter[] parameters)
        {
            // create the command
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            command.ExecuteNonQuery();
        }


        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
