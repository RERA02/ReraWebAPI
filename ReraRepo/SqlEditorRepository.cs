using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReraCore;
using ReraCore.Helper;
using ReraRepo.Helper;


namespace ReraRepo
{
    public class SqlEditorRepository : ISqlEditorRepository
    {
        private readonly DBContext _dbContext;
        private readonly string _pageName;
        private string? _actionName;
        private string? _sqlQuery;
        private string? _iPAddress;

        public SqlEditorRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
            _pageName = "SqlEditorRepository";
            _iPAddress = CommonFuncationHelper.GetIpAddress().IPv4_Address;
        }

        public async Task<DataTable> GetSqlEditorList(string _sqlEditorText)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SP_Get_SqlEditorResult";                        
                        command.Parameters.AddWithValue("@SqlEditorText", _sqlEditorText);
                        _sqlQuery = command.GetSqlExecutableQuery();// Get sql query
                        dataTable = await command.FillAsync_DataTable();
                    }
                    return dataTable;
                });
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
        }
        public async Task<DataTable> GetSqlEditor_InsertUpdate(string _sqlEditorText)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    int result = 0;
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SP_Get_SqlEditorResult";
                        command.Parameters.AddWithValue("@SqlEditorText", _sqlEditorText);
                        //_sqlQuery = command.GetSqlExecutableQuery();// Get sql query
                        //result= await command.ExecuteNonQuery();

                        _sqlQuery = command.GetSqlExecutableQuery();
                        result = await command.ExecuteNonQueryAsync();
                        //result = Convert.ToInt32(command.Parameters["@Return"].Value);// out
                    }
                    return dataTable;
                });
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
        }
    }
}
