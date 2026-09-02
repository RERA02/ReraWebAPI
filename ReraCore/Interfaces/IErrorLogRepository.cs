using System.Data;
using ReraCore.Entities;
using ReraCore.Helper;


namespace ReraCore.Interfaces
{
    public interface IErrorLogRepository
    {
        Task<int> AddErrorLog(Tbl_Trn_ErrorLog data);
        Task<List<Tbl_Trn_ErrorLog>> GetAllErrorLog(GenericPaginationSpecification specification);
        Task<Tbl_Trn_ErrorLog> GetErrorLogById(int errorLogId);

        Task<DataTable> FetchApplicationErrorLog(string SearchKey);

        Task<int> ChangeLogStatus(int Id, int sType);
    }
}
