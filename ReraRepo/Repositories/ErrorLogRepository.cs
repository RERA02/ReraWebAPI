using ReraCore.Entities;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraRepo.Helper;
using System.Data;


namespace ReraRepo.Repositories
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly DBContext _dbContext;
        private readonly string _pageName;
        private string _actionName;
        private string _sqlQuery;

        public ErrorLogRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
            _pageName = "ErrorLogRepository";
        }

        public async Task<int> AddErrorLog(Tbl_Trn_ErrorLog data)
        {
            return await Task.Run(async () =>
            {
                int result = 0;
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "USP_trn_ErrorLog";
                    command.Parameters.AddWithValue("@ActionType", "INSERT");
                    command.Parameters.AddWithValue("@ErrorMessage", data.ErrorMessage);
                    command.Parameters.AddWithValue("@ErrorDescription", data.ErrorDescription);
                    command.Parameters.AddWithValue("@ErrorLocation", data.ErrorLocation);
                    command.Parameters.AddWithValue("@ErrorLine", data.LineNumber);
                    command.Parameters.AddWithValue("@StackTrace", data.StackTrace); // added new
                    command.Parameters.AddWithValue("@ErrorGeneratedFrom", data.ErrorGeneratedFrom); // added new
                    //command.Parameters.Add("@retval_ID", SqlDbType.Int);// out
                    //command.Parameters["@retval_ID"].Direction = ParameterDirection.Output;// out
                    result = await command.ExecuteNonQueryAsync();
                   // var retval_ID = command.Parameters["@retval_ID"].Value;// out
                }
                return result;
            });
        }

        public async Task<List<Tbl_Trn_ErrorLog>> GetAllErrorLog(GenericPaginationSpecification specification)
        {
            return await Task.Run(async () =>
            {
                DataSet ds = null;
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_ErrorLog";
                    command.Parameters.AddWithValue("@action", "usp_ErrorLog");
                    // input pagination
                    command.Parameters.AddWithValue("@PageNumber", specification.PageNumber);
                    command.Parameters.AddWithValue("@PageSize", specification.PageSize);
                    ds = await command.FillAsync();
                }
                // class
                var data = new List<Tbl_Trn_ErrorLog>();
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        data = CommonFuncationHelper.ConvertDataTable<List<Tbl_Trn_ErrorLog>>(ds.Tables[0]);
                    }
                }
                return data;
            });
        }

        public async Task<Tbl_Trn_ErrorLog> GetErrorLogById(int errorLogId)
        {
            return await Task.Run(async () =>
            {
                DataSet ds = null;
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_ErrorLog";
                    command.Parameters.AddWithValue("@action", "_GetErrorLogById");
                    command.Parameters.AddWithValue("@Id", errorLogId);
                    ds = await command.FillAsync();
                }
                // class
                var data = new Tbl_Trn_ErrorLog();
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        data = CommonFuncationHelper.ConvertDataTable<Tbl_Trn_ErrorLog>(ds.Tables[0]);
                    }
                }
                return data;
            });
        }


        public async Task<DataTable> FetchApplicationErrorLog(string SearchKey)
        {
            _actionName = "FetchApplicationErrorLog(string SearchKey)";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand(true))
                    {
                        // Set the stored procedure name and type
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "USP_FetchAllApplicationLog";
                        command.Parameters.AddWithValue("@SearchKey", SearchKey);    

                        _sqlQuery = command.GetSqlExecutableQuery();
                        dataTable = await command.FillAsync_DataTable();
                    }
                    return dataTable;
                }

                catch (Exception ex)
                {
                    var errorDesc = new ErrorDescription
                    {
                        Message = ex.Message,
                        PageName = _pageName,
                        ActionName = _actionName,
                        SqlExecutableQuery = _sqlQuery
                    };
                    var errordetails = CommonFuncationHelper.MakeError(errorDesc);
                    throw new Exception(errordetails, ex);
                }
            });
        }

        public async Task<int> ChangeLogStatus(int Id, int sType)
        {
            _actionName = "ChangeLogStatus(int Id, int sType)";
            return await Task.Run(async () =>
            {
                try
                {
                    int result = 0;
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;               
                        command.CommandText = "USP_ChangeLogStatus";                                       
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@sType", sType);

                        _sqlQuery = command.GetSqlExecutableQuery();
                        result = await command.ExecuteNonQueryAsync();                    
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    var errorDesc = new ErrorDescription
                    {
                        Message = ex.Message,
                        PageName = _pageName,
                        ActionName = _actionName,
                        SqlExecutableQuery = _sqlQuery
                    };
                    var errordetails = CommonFuncationHelper.MakeError(errorDesc);
                    throw new Exception(errordetails, ex);
                }
            });
        }
    }
}
