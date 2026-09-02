

namespace ReraCore.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
       


       
        IErrorLogRepository ErrorLogs { get; }        
        ICommonFunctionRepository CommonFunctionRepository { get; }      
        IHomeRepository HomeRepository { get; }
        IUserRepository UserRepository { get; }

        void SaveChanges();

  

    }
}
