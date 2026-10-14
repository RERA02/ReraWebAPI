using ReraCore.Helper;
using Microsoft.Data.SqlClient;

namespace ReraRepo
{
    public class DBContext : IDisposable
    {
        private bool disposedValue;

        private SqlConnection _connection;
        private SqlTransaction _transaction;
        string sqlConnectionStaring = "";
        static string sqlConnectionStaringSys = "";


        public DBContext()
        {
         }

        public SqlCommand CreateCommand(bool withTransactionControl = false)
        {
            CreateObject(withTransactionControl);
            var command = _connection.CreateCommand();
            if (withTransactionControl)
            {
                command.Transaction = _transaction;
            }
            command.CommandTimeout = (5 * 60);//5 min.
            return command;
        }
        public void SaveChanges()
        {
            //if (_transaction == null)
            //{
            //    throw new InvalidOperationException("Transaction have already been already been commited. Check your transaction handling.");
            //}
            try
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (!disposedValue)
            {
                //if (disposing)
                {
                    try
                    {
                        // TODO: dispose managed state (managed objects)
                        if (_transaction != null)
                        {
                            _transaction.Rollback();
                            _transaction = null;
                        }
                        if (_connection != null)
                        {
                            _connection.Close();
                            _connection = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        _transaction = null;
                        _connection = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            //GC.SuppressFinalize(this);
        }

        private void CreateObject(bool withTransactionControl)
        {
            if (_connection == null && _transaction == null)
            {
                var connectionString = ConfigurationHelper.ConnectionString;
                _connection = new SqlConnection(connectionString);
                _connection.Open();
                if (withTransactionControl)
                {
                    _transaction = _connection.BeginTransaction();
                }
            }
        }

    }
}
