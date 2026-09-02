using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraModel.CommonFunction;
using ReraModel.CommonModel;
using ReraModel.HomeModel;
using ReraRepo.Helper;
using System;
using System.Data;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

//using static EEMS.Models.CommonFunction.ItiTradeAndCollegesDDL;

namespace ReraRepo.Repositories
{
    public class CommonFunctionRepository : ICommonFunctionRepository

    {
        private readonly DBContext _dbContext;
        private readonly string _pageName;
        private string _actionName;
        private string _sqlQuery;
        private IPAddressModel _iPAddress;
        private string _NEWiPAddress;
        private string _iPv6Address;
        private string _iPv4Address;
        public CommonFunctionRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
            _pageName = "CommonFunctionRepository";
            _iPAddress = CommonFuncationHelper.GetIpAddress();
            _iPv6Address = CommonFuncationHelper.GetIpAddress().IPv6_Address;
            _iPv4Address = CommonFuncationHelper.GetIpAddress().IPv4_Address;
            _NEWiPAddress = CommonFuncationHelper.NEWGetIpAddress();


        }        

     

        public async Task<DataTable> GetComponentsNew(int? sewadwar_State_ID, int? TypeId)
        {
            _actionName = "GetComponentsNew()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "OldCompliantDetailsTransfertoNewTables";
                        command.Parameters.AddWithValue("@Action", "GETALLDISTRICTBy_SewadwarStateId");
                        command.Parameters.AddWithValue("@Stateid", "0");
                        command.Parameters.AddWithValue("@Districtid", "0");
                        command.Parameters.AddWithValue("@ComplaintID", "0");
                        command.Parameters.AddWithValue("@Tehsil", "0");
                        command.Parameters.AddWithValue("@PROJECTID", "0");
                        command.Parameters.AddWithValue("@TypeId", TypeId.ToString());
                        command.Parameters.AddWithValue("@PromoterId", "0");
                        command.Parameters.AddWithValue("@Sewadwar_State_ID", sewadwar_State_ID.ToString());
                        command.Parameters.AddWithValue("@ParentId", "0");                       
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

        public async Task<DataTable> GetProjectType()
        {
            _actionName = "GetProjectType()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dt = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                       // command.CommandText = $"select * from ApplicationMasters Where MasterType='ProjectType' and IsActive=1 and IsDeleted=0";
                        command.CommandText = "Get_projectType";
                        _sqlQuery = command.GetSqlExecutableQuery();// sql query for log
                        dt = await command.FillAsync_DataTable();
                    }

                    return dt;
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

        public async Task<DataTable> GetQuarterList(int? FinYearId)
        {
            _actionName = "GetQuarterList()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetQuarterListQPR";
                        command.Parameters.AddWithValue("@Action", "QualyerlyListFinancial");                        
                        command.Parameters.AddWithValue("@projectid", 0);
                        command.Parameters.AddWithValue("@ExpectedDtOfCompletion", DateTime.Today);
                        command.Parameters.AddWithValue("@quarterid", FinYearId); 
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
        public async Task<DataTable> GetFinancialyearList()
        {
            _actionName = "GetFinancialyearList()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetFinYear";                        
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


        public async Task<DataTable> GetPromoterforddl()
        {
            _actionName = "Get_PromoterForddl()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dt = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;                        
                        command.CommandText = "Get_PromoterForddl";
                        command.Parameters.AddWithValue("@Action", "GETddl");
                        _sqlQuery = command.GetSqlExecutableQuery();// sql query for log
                        dt = await command.FillAsync_DataTable();
                    }

                    return dt;
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

        public async Task<DataTable> GetProjectList(int? UserId)
        {
            _actionName = "GetProjectList()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "usp_GetProject";                   
                        command.Parameters.AddWithValue("@UserId", UserId);
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
        public async Task<DataTable> GetComponents(int? parentID, int? TypeId)
        {
            _actionName = "GetComponents()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "OldCompliantDetailsTransfertoNewTables";
                        command.Parameters.AddWithValue("@Action", "GetTehsilByRajdwarDistrictID_TypeId");
                        command.Parameters.AddWithValue("@Stateid", "0");
                        command.Parameters.AddWithValue("@Districtid", "0");
                        command.Parameters.AddWithValue("@ComplaintID", "0");
                        command.Parameters.AddWithValue("@Tehsil", "0");
                        command.Parameters.AddWithValue("@PROJECTID", "0");
                        command.Parameters.AddWithValue("@TypeId", TypeId.ToString());
                        command.Parameters.AddWithValue("@PromoterId", "0");
                        command.Parameters.AddWithValue("@Sewadwar_State_ID", "0");
                        command.Parameters.AddWithValue("@ParentId", parentID.ToString());
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

        public async Task<DataTable> GetVistorCount(string Actionname , string ip)
        {
            _actionName = "GetVistorCount()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SP_TotalVistorCountOnDefaulterlist";
                        command.Parameters.AddWithValue("@Action", Actionname);
                        command.Parameters.AddWithValue("@IPAddress", _NEWiPAddress);
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


        public async Task<DataTable> CommonDetails(CommonModel searchParams)
        {
            _actionName = "CommonDetails";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Web_CommonDetails";
                    command.Parameters.Add(new SqlParameter("@Action", searchParams.Action));
                    command.Parameters.Add(new SqlParameter("@Id", searchParams.Id));                    
                    command.Parameters.Add(new SqlParameter("@pnrno", searchParams.pnrno ?? ""));
                    command.Parameters.Add(new SqlParameter("@Type", searchParams.Type ?? ""));
                    command.Parameters.Add(new SqlParameter("@isProfileOrProject", searchParams.isProfileOrProject));

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

                throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
            }
        }
        public async Task<DataTable> GetAllAgentDocumentUploadList(AgentDocumentModel searchParams)
        {
            _actionName = "GetAllAgentDocumentUploadList";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Web_GetAllAgentDocumentUploadList";
                    command.Parameters.Add(new SqlParameter("@UserId", searchParams.UserId));
                    command.Parameters.Add(new SqlParameter("@ProfileType", searchParams.ProfileType));                    
                    

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

                throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
            }
        }


        public async Task<DataTable> GetYear()
        {
            _actionName = "GetYear()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetYearCurrent";
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

        public async Task<DataTable> GetDeflauterlist(int? sewadwar_State_ID, int? TypeId)
        {
            _actionName = "GetDeflauterlist()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SP_GEtDefaultersProjectList";
                        command.Parameters.AddWithValue("@Action", "DefaulterProjectListAPI");
                        command.Parameters.AddWithValue("@OrderPassby", "0");
                        command.Parameters.AddWithValue("@ApprovedOnDate", null);
                        command.Parameters.AddWithValue("@ApprovedToDate", null);
                        command.Parameters.AddWithValue("@Financialyear", 0);
                        command.Parameters.AddWithValue("@Monthwise", 0);                      
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

    }
}
