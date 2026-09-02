using ReraCore;
using ReraCore.Interfaces;


namespace ReraRepo.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContext _dbContext;
        private bool disposedValue;

        public UnitOfWork()
        {
            _dbContext = new DBContext();
        }

        #region UOW
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            {
                {
                    if (_dbContext != null)
                    {
                        _dbContext.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
        #endregion 

    
        private IErrorLogRepository _errorLogs;
        public IErrorLogRepository ErrorLogs
        {
            get
            {
                return _errorLogs ?? new ErrorLogRepository(_dbContext);
            }
        }

        

        private ICommonFunctionRepository _commonFunctionRepository;
        public ICommonFunctionRepository CommonFunctionRepository
        {
            get
            {
                return _commonFunctionRepository ?? new CommonFunctionRepository(_dbContext);
            }
        }

        private IHomeRepository _homeRepository;
        public IHomeRepository HomeRepository
        {
            get
            {
                return _homeRepository ?? new HomeRepository(_dbContext);
            }
        }

        private IUserRepository _userRepository;
        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository ?? new UserRepository(_dbContext);
            }
        }

    }
}

