using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraModel.CommonFunction;
using ReraModel.DashboardDepartmentModel;
using ReraModel.DashboardPromoterModel;
using ReraModel.HomeModel;
using ReraModel.UserLoginModel;
using ReraRepo.Helper;
using System;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Object = System.Object;


//using static EEMS.Models.CommonFunction.ItiTradeAndCollegesDDL;

namespace ReraRepo.Repositories
{
    public class UserRepository : IUserRepository

    {
        private readonly DBContext _dbContext;
        private readonly string _pageName;
        private string _actionName;
        private string _sqlQuery;
        private IPAddressModel _iPAddress;
        private string _iPv6Address;
        private string _iPv4Address;
        private SqlHelper _sSqlHelper;

        public UserRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
            _pageName = "UserRepository";
            _iPAddress = CommonFuncationHelper.GetIpAddress();
            _iPv6Address = CommonFuncationHelper.GetIpAddress().IPv6_Address;
            _iPv4Address = CommonFuncationHelper.GetIpAddress().IPv4_Address;
            _sSqlHelper = new SqlHelper();
        }
        public async Task<DataTable> GetAllUser(GenericPaginationSpecification specification)
        {
            DataTable dt = null;
            using (var command = _dbContext.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "usp_Users";
                command.Parameters.AddWithValue("@action", "_GetAllUser");
                // input pagination
                command.Parameters.AddWithValue("@PageNumber", specification.PageNumber);
                command.Parameters.AddWithValue("@PageSize", specification.PageSize);
                dt = await command.FillAsync_DataTable();
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt;
            }
            return dt;
        }
        //
        public async Task<DataTable> ValidateSSOUserDetail(UserLoginModel model)
        {
            DataTable dt = null;
            try
            {

                string UserName = "agriculture.test";
                string Application = "RERA2";
                string ssopassword = "Rajasthan@13";
                ssopassword = Encrypt(ssopassword, "R@j$S0@02{09}19#");
                string url = $"https://sso.rajasthan.gov.in:4443/SSOREST/SSOAuthenticationMobileNew/{Application}/{UserName}/{ssopassword}";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response =
                        httpClient.GetAsync(url).GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"SSO API returned status code: {response.StatusCode}");
                    }

                    string responseContent =
                        response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        throw new Exception("SSO API response is empty.");
                    }

                    // Deserialize JSON
                    SSOUserDetail ssoUser =
                        JsonConvert.DeserializeObject<SSOUserDetail>(responseContent);
                    string json = JsonConvert.SerializeObject(new List<SSOUserDetail> { ssoUser });
                    dt = JsonConvert.DeserializeObject<DataTable>(json);
                }
                //
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public string Encrypt(string plainText, string encryptionKey)
        {
            if (!string.IsNullOrEmpty(plainText))
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var cipher = GetRijndaelManaged(encryptionKey);
                return
                Convert.ToBase64String(cipher.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length));
            }
            else
            {
                return string.Empty;
            }
        }

        private RijndaelManaged GetRijndaelManaged(string secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        //
        public async Task<DataTable> LoginUser(UserLoginModel model)
        {
            DataTable dt = null;
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "USP_Mobile_Login";
                    command.Parameters.AddWithValue("@SsoId", model.SSO_ID);
                    command.Parameters.AddWithValue("@PassWord", model.Password);
                //    command.Parameters.AddWithValue("@DeviceId", model.deviceId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);
                    CommonFuncationHelper.WriteTextLog($"non static:, dt: {JsonConvert.SerializeObject(model) + "##" + command.CommandText + "##" + JsonConvert.SerializeObject(dt)}");
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }



        public async Task<DataTable> UserRole(UserRoleModel model)
        {
            DataTable dt = null;
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "USP_Mobile_APIs";
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    command.Parameters.AddWithValue("@APIType", "UserRole");
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);
                    CommonFuncationHelper.WriteTextLog($"non static:, dt: {JsonConvert.SerializeObject(model) + "##" + command.CommandText + "##" + JsonConvert.SerializeObject(dt)}");
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> DashboardPromoter(DashboardPromoterModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DashBoardDetails_PromoterAPI";
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataSet> DownloadProjectDocuments(DownloadProjectDocumentsModel model)
        {
            DataSet ds = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DownloadProjectDocuments_API";
                    command.Parameters.AddWithValue("@projectid", model.ProjectId);
                    command.Parameters.AddWithValue("@type", model.Type);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    ds = await _sSqlHelper.FillAsync(command);

                }
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return ds;
                    }
                }
                return ds;
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
                await AddErrorLog(errordetails);
                return ds;
            }
        }

        public async Task<DataTable> DownloadProjectDocumentsNew(DownloadProjectDocumentsModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DownloadProjectDocumentsNew_API";
                    command.Parameters.AddWithValue("@projectid", model.ProjectId);
                    command.Parameters.AddWithValue("@type", model.Type);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }



        public async Task<DataTable> DashboardAgent(DashboardAgentModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DashBoardDetails_AgentAPI";
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    command.Parameters.AddWithValue("@Modulename", model.Modulename);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> DashboardAllottee(DashboardAllotteeModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DashBoardDetails_AllotteeAPI";
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    command.Parameters.AddWithValue("@Modulename", model.Modulename);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> DashboardDepartment(DashboardDepartmentModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetUserWiseDashBoardDetails_API";
                    command.Parameters.AddWithValue("@SSOID", model.SSOId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> GetModuleMaster(ModuleMastersModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetModuleMaster_API";
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> DepartmentPendencyData(DepartmentPendencyDataModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DepartmentPendencyData_API";
                    command.Parameters.AddWithValue("@SSOID", model.SSOId);
                    command.Parameters.AddWithValue("@PendingStatus", model.PendingStatus);
                    command.Parameters.AddWithValue("@Module", model.Module);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }


        public async Task<DataTable> ProjectSearchData(ProjectSearchDataModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "ProjectSearchData_API";
                    command.Parameters.AddWithValue("@APPLICATIONNO", model.ApplcationNo);
                    command.Parameters.AddWithValue("@PROJECTNAME", model.ProjectName);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> ProjectsByPromoterId(ProjectSearchPromoterModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "ProjectsByPromoterId_API";
                    command.Parameters.AddWithValue("@PromoterId", model.PromoterId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }


        public async Task<DataTable> GetDetailsbyProjectId(string ProjectId)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "ProjectsDetails_API";
                    command.Parameters.AddWithValue("@ProjectId", ProjectId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }
        
       

        //
        public async Task<DataTable> GetAllProjectMasterForDashboardGrid(PromoterDashboardGridModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DashBoardDetails_ProjectData_API";
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    command.Parameters.AddWithValue("@Status", model.Status);
                    command.Parameters.AddWithValue("@Module", model.Module);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> GETAuditStatus(ProjectAuditModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GETAuditStatus_API";
                    command.Parameters.AddWithValue("@projectid", model.projectid);
                    command.Parameters.AddWithValue("@TableId", model.TableId);
                    command.Parameters.AddWithValue("@type", model.Typefor);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> GETModuleList(ModuleMasterModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GETModuleList_API";
                    command.Parameters.AddWithValue("@MasterType", model.MasterType);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<DataTable> GETForwardTo(ForwardToModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GETForwardTo_API";
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    command.Parameters.AddWithValue("@moduleid", model.moduleid);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<int> SaveServiceRequest(Service_RequestModel model)
        {
            try
            {
                int serviceRequestId = 0;

                using (var command = _dbContext.CreateCommand())
                {
                    Random r = new Random();
                    model.TicketNo = r.Next(100000, 999999).ToString();

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SaveServiceRequest_API";

                    command.Parameters.AddWithValue("@ID", model.ID);
                    command.Parameters.AddWithValue("@Userid", model.Userid);
                    command.Parameters.AddWithValue("@PROJECTID", model.PROJECTID);
                    command.Parameters.AddWithValue("@Contact_Email", model.Contact_Email);
                    command.Parameters.AddWithValue("@ContactNo", model.ContactNo);
                    command.Parameters.AddWithValue("@AttachFile", model.AttachFile);
                    command.Parameters.AddWithValue("@Request_Details", model.Request_Details);
                    command.Parameters.AddWithValue("@RequestBy", model.RequestBy);
                    command.Parameters.AddWithValue("@Request_Resolved", model.Request_Resolved);
                    command.Parameters.AddWithValue("@ISACTIVE", model.ISACTIVE);
                    command.Parameters.AddWithValue("@TicketNo", model.TicketNo);
                    command.Parameters.AddWithValue("@ModuleID", model.ModuleID);
                    command.Parameters.AddWithValue("@ProjectName", model.ProjectName);
                    command.Parameters.AddWithValue("@ApplicationNo", model.ApplicationNo);
                    command.Parameters.AddWithValue("@Subject", model.Subject);
                    command.Parameters.AddWithValue("@ServiceRequestNo", model.ServiceRequestNo);


                    serviceRequestId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                if (serviceRequestId > 0)
                {
                    string message =
                        $"Dear Promoter, Your concern request is raised successfully. Ticket No: {model.TicketNo}";

                    SendMessageWithTemplateid(
                        model.ContactNo,
                        message,
                        "1107170600982056979"
                    );

                    CreateActivityLog(26, "Service Request Data Saved Successfully", model.Userid);
                }

                return serviceRequestId;
            }
            catch (Exception ex)
            {
                // log error
                return -1;
            }
        }

        public async Task<int> SaveLatLong(LatLongModel model)
        {
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SaveLatLong_API";

                    command.Parameters.Add(new SqlParameter("@ProjectId", model.ProjectId));
                    command.Parameters.Add(new SqlParameter("@ModuleId", model.ModuleId));
                    command.Parameters.Add(new SqlParameter("@userId", model.userId));
                    command.Parameters.Add(new SqlParameter("@latLongId", model.latLongId));
                    command.Parameters.Add(new SqlParameter("@total_area", model.total_area));

                    command.Parameters.Add(new SqlParameter("@Latitude", (object?)model.Latitude ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Longitude", (object?)model.Longitude ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CaptureImage", (object?)model.CaptureImage ?? DBNull.Value));

                    _sqlQuery = command.GetSqlExecutableQuery();

                    var result = await command.ExecuteScalarAsync();

                    return (result != null && result != DBNull.Value)
                        ? Convert.ToInt32(result)
                        : 0;
                }
            }
            catch (Exception ex)
            {
                // TODO: log error properly
                return -1;
            }
        }

        public async Task<int> SaveComment(SaveCommentModel model)
        {
            try
            {
                int serviceRequestId = 0;

                using (var command = _dbContext.CreateCommand())
                {
                    Random r = new Random();


                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SaveComment_API";

                    command.Parameters.AddWithValue("@ID", model.ID);
                    command.Parameters.AddWithValue("@Userid", model.Userid);
                    command.Parameters.AddWithValue("@ModuleID", model.ModuleID);
                    command.Parameters.AddWithValue("@comments", model.comments);
                    command.Parameters.AddWithValue("@uploadfile", model.uploadfile);
                    command.Parameters.AddWithValue("@ForwardTo", model.ForwardTo);

                    serviceRequestId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                if (serviceRequestId > 0)
                {
                    //string message =
                    //    $"Dear Promoter, Your concern request is raised successfully. Ticket No: {model.TicketNo}";

                    //SendMessageWithTemplateid(
                    //    model.ContactNo,
                    //    message,
                    //    "1107170600982056979"
                    //);

                    //CreateActivityLog(26, "Service Request Data Saved Successfully", model.Userid);
                }

                return serviceRequestId;
            }
            catch (Exception ex)
            {
                // log error
                return -1;
            }
        }

        public void SendMessageWithTemplateid(string strMobileno, string StrMsg, string templateID)
        {
            try
            {
                if (string.IsNullOrEmpty(strMobileno) == false)  //// Applied as stop msg without mobileno
                {
                    string username = "rajmsdg-rspcb";
                    string password = "Rajasthan@11";
                    string senderID = "RAJSMS";


                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;

                    HttpClient client = new HttpClient();

                    client.BaseAddress = new Uri("https://api.sewadwaar.rajasthan.gov.in/live/eSanchar/Prod/api/OBD/CreateSMS/"); //10-06-2020

                    client.DefaultRequestHeaders.Add("username", username);
                    client.DefaultRequestHeaders.Add("password", password);

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                    {
                        return (true);
                    };

                    var inputparams = new sendSms();
                    inputparams.UniqueID = senderID;
                    inputparams.serviceName = "SMS";
                    inputparams.language = "ENG";
                    inputparams.message = StrMsg;
                    inputparams.mobileNo = new System.Collections.Generic.List<string> { strMobileno };
                    inputparams.templateID = templateID;


                    string json = JsonConvert.SerializeObject(inputparams, Formatting.Indented);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");  //10-06-2020
                    var response = client.PostAsync("Request?client_id=c79a13d4-58c1-44ef-b3aa-4f5240141834", httpContent).Result;
                    var asyncResponse = response.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JObject.Parse(asyncResponse);
                    string status = "Response Code: " + jsonResponse["responseCode"] + "\n\nResponse Message - " + jsonResponse["responseMessage"];


                    LogSentMessage(strMobileno, StrMsg, "");
                }
            }
            catch (Exception ex)
            {
            }
        }

        public int CreateActivityLog(int ActivityTypeID, string Activity, int UserID)
        {
            try
            {

                IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
                var myIP = heserver.AddressList[1].ToString();
                ///////////////////////////////////
                IPHostEntry Host = default(IPHostEntry);
                string Hostname = null;
                Hostname = System.Environment.MachineName;
                Host = Dns.GetHostEntry(Hostname);
                foreach (IPAddress IP in Host.AddressList)
                {
                    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        myIP = Convert.ToString(IP);
                    }
                }
                System.String ip = "";
                //HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                //update IP address

                string currentIpAddress = "";//_webHelper.GetCurrentIpAddress();

                if (string.IsNullOrEmpty(ip))
                {
                    //string ipAdd = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //string ipHost = HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
                    //ip = HttpContext.Current.Request.ServerVariables["REMOTE_USER"];
                    // Request.ServerVariables["REMOTE_ADDR"]
                    //Request.ServerVariables["REMOTE_HOST"]
                    //Request.ServerVariables["REMOTE_USER"]
                }

                //string hostName = Dns.GetHostName();
                //        string strHostName = "";
                //        strHostName = System.Net.Dns.GetHostName();

                //        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

                //        string ipaddress = Convert.ToString(ipEntry.AddressList[2]);

                //        string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                int res = 0;
                var param2 = new SqlParameter("@ActivityTypeID", ActivityTypeID);
                var param3 = new SqlParameter("@Activity", Activity);
                var param4 = new SqlParameter("@UserID", UserID);
                var param5 = new SqlParameter("@IPAddress", currentIpAddress);

                res = 1;//_dbContext.SqlQuery<int>("EXEC CreateActivityLog @ActivityTypeID,@Activity,@UserID,@IPAddress", param2, param5, param3, param4).ToList().Count();
                return res;
            }
            catch (Exception ex) { return 0; }
        }

        public void LogSentMessage(string toNumber, string msgBody, string responce)
        {
            var msgRow = new SentMessage();
            msgRow.ToNumber = toNumber;
            msgRow.MsgBody = msgBody;
            msgRow.Responce = responce;

            //if (responce.Length == 16)
            msgRow.IsSent = true;
            //else
            //    msgRow.IsSent = false;

            //var messageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
            //messageService.SaveSentMessage(msgRow);
        }

        public async Task<DataSet> FillProjectSummaryMasterModel(ProjectSummaryModel model)
        {
            DataSet ds = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "ProjectSummary_API";
                    command.Parameters.AddWithValue("@projectid", model.projectId);
                    command.Parameters.AddWithValue("@type", model.type);
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    ds = await _sSqlHelper.FillAsync(command);

                }
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return ds;
                    }
                }
                return ds;
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
                await AddErrorLog(errordetails);
                return ds;
            }
        }

        //
        //public async Task<ProjectSummaryMasterModel> FillProjectSummaryMasterModel(ProjectSummaryModel model)
        //{
        //    ProjectSummaryMasterModel objmodel = new ProjectSummaryMasterModel();
        //    var obj = _projectService.GetProjectMasterById(model.projectId);
        //    //if (obj != null)
        //    //{

        //    //    objmodel.GetProjectBasic = new ProjectModel();
        //    //    objmodel.ProjectId = model.projectId;
        //    //    objmodel.ProjectType = obj.ProjectType;

        //    //    if (typemode == 1 && PNR == "")
        //    //    {
        //    //        Project obj_modify = _projectService.Get_ModifiedSummary<Project>(obj.UserId, projectId, PNR, 21, 0);
        //    //        objmodel.GetProjectBasic = FillProjectModel(obj_modify, projectId);
        //    //        if (obj_modify.Litigations == true)
        //    //            objmodel.ProjectLitigations = _projectService.Get_ModifiedSummaryList<ProjectLitigationsVM>(obj.UserId, projectId, PNR, 22, 0);
        //    //        objmodel.GetProjectCostDetail = _projectService.Get_ModifiedSummaryList<ProjectCostDetailVM>(obj.UserId, projectId, PNR, 23, 0);
        //    //        objmodel.ProjectCommanArea = FillProjectCommonAreaList(projectId);
        //    //        objmodel.CommonAreaItemsCharged = _projectService.GetCommonAreaItemsCharged(projectId);
        //    //        objmodel.GetProjectAreaFacilities = _projectService.GetProjectCommonAreaFacilities(projectId);
        //    //        objmodel.ProjectCommanArea.ProjectDevelopementWork.Clear();
        //    //        objmodel.ProjectCommanArea.ProjectDevelopementWork = _projectService.Get_ModifiedSummaryList<ProjectDevelopementWorkVM>(obj.UserId, projectId, PNR, 24, 0);
        //    //        if (obj.ProjectType == 4)
        //    //        {
        //    //            objmodel.PlotDetails = _projectService.GetPlotDetailByProjectId(projectId);
        //    //            objmodel.Tbl_Plots = _projectService.GetTblPlotById(projectId);
        //    //            if (objmodel.PlotDetails.Count != 0)
        //    //            {
        //    //                objmodel.Sanctioned_Notsanctioned = objmodel.PlotDetails[0].sanctioned_NotSanctioned;
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            var Buildingdata = _projectService.Get_ModifiedSummaryList<BuildingVM>(obj.UserId, projectId, PNR, 25, 0);
        //    //            objmodel.GetBuildingDetails = FillProjectSummaryBuilding(null, 0, 0, typemode, Buildingdata, obj.UserId, projectId, PNR);
        //    //        }
        //    //        objmodel.ProjectProFessionAlDetail = _projectService.GetModifiedProfessionaldetail(obj.UserId, projectId, PNR, 28, 0);
        //    //        //objmodel.GetDocumentsList = FillProjectDocumentList(projectId, obj.UserId, PNR, typemode);
        //    //        objmodel.ProjectDocuments = FillProjectDocuments(projectId);
        //    //        objmodel.ProjectPromoterDetail = FillProjectPromoterDetails(projectId);
        //    //    }
        //    //    else if (typemode == 1 && PNR != "")
        //    //    {
        //    //        Project obj_modify = _projectService.Get_ModifiedSummary<Project>(obj.UserId, projectId, PNR, 1, 0);
        //    //        objmodel.GetProjectBasic = FillProjectModel(obj_modify, projectId);
        //    //        if (obj_modify.Litigations == true)
        //    //            objmodel.ProjectLitigations = _projectService.Get_ModifiedSummaryList<ProjectLitigationsVM>(obj.UserId, projectId, PNR, 2, 0);
        //    //        objmodel.GetProjectCostDetail = _projectService.Get_ModifiedSummaryList<ProjectCostDetailVM>(obj.UserId, projectId, PNR, 3, 0);
        //    //        objmodel.ProjectCommanArea = FillProjectCommonAreaList(projectId);
        //    //        objmodel.GetProjectAreaFacilities = _projectService.GetProjectCommonAreaFacilities(projectId);
        //    //        objmodel.ProjectCommanArea.ProjectDevelopementWork.Clear();
        //    //        objmodel.ProjectCommanArea.ProjectDevelopementWork = _projectService.Get_ModifiedSummaryList<ProjectDevelopementWorkVM>(obj.UserId, projectId, PNR, 4, 0);
        //    //        if (obj.ProjectType == 4)
        //    //        {
        //    //            objmodel.PlotDetails = _projectService.GetPlotDetailByProjectId(projectId);
        //    //            objmodel.Tbl_Plots = _projectService.GetTblPlotById(projectId);
        //    //            //objmodel.PlotDetails = _projectService.Get_ModifiedSummaryList<PlotDetailsVM>(obj.UserId, projectId, PNR, 4, 0);
        //    //            if (objmodel.PlotDetails.Count != 0)
        //    //            {
        //    //                objmodel.Sanctioned_Notsanctioned = objmodel.PlotDetails[0].sanctioned_NotSanctioned;
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            var Buildingdata = _projectService.Get_ModifiedSummaryList<BuildingVM>(obj.UserId, projectId, PNR, 5, 0);
        //    //            objmodel.GetBuildingDetails = FillProjectSummaryBuilding(null, 0, 0, typemode, Buildingdata, obj.UserId, projectId, PNR);
        //    //        }
        //    //        objmodel.ProjectProFessionAlDetail = _projectService.GetModifiedProfessionaldetail(obj.UserId, projectId, PNR, 8, 0);
        //    //        //objmodel.GetDocumentsList = FillProjectDocumentList(projectId, obj.UserId, PNR, typemode);
        //    //        objmodel.ProjectDocuments = FillProjectDocuments(projectId);
        //    //        objmodel.ProjectPromoterDetail = FillProjectPromoterDetails(projectId);
        //    //    }
        //    //    else
        //    //    {
        //    //        objmodel.GetProjectBasic = FillProjectModel(obj, projectId);
        //    //        if (objmodel.GetProjectBasic.Status == 3)
        //    //        {
        //    //            var ExemptionList = _masterService.GetExemptionReason("AllReasonbyID", Convert.ToInt32(obj.Reasonforexemption));
        //    //            objmodel.GetProjectBasic.Reasonforexemption = Convert.ToInt32(obj.Reasonforexemption);
        //    //            objmodel.ReasonofExemptionSummary = ExemptionList[0].Text;
        //    //        }
        //    //        if (obj.Litigations == true)
        //    //            objmodel.ProjectLitigations = _projectService.GetAllProjectLitigationsList(obj.Id);
        //    //        objmodel.GetProjectCostDetail = _projectService.GetProjectCostdetail(projectId);
        //    //        objmodel.ProjectCommanArea = FillProjectCommonAreaList(projectId);
        //    //        objmodel.CommonAreaItemsCharged = _projectService.GetCommonAreaItemsCharged(projectId);
        //    //        objmodel.GetProjectAreaFacilities = _projectService.GetProjectCommonAreaFacilities(projectId);
        //    //        objmodel.GetApartmentAllotteeDetailsList = _projectService.GetApartAllotteeDetailsForSummary(projectId);

        //    //        var promoter = _userService.GetUserProfileByUserId(obj.UserId);
        //    //        if (promoter != null)
        //    //        {
        //    //            PromoterDetailsModel PD = new PromoterDetailsModel();
        //    //            PD.InformationType = promoter.InformationType;
        //    //            objmodel.GetProjectBasic.PromoterType = promoter.InformationType;
        //    //            if (promoter.InformationType != 2)
        //    //            {
        //    //                PD.FirstName = promoter.FirstName;
        //    //                PD.MiddleName = promoter.MiddleName;
        //    //                PD.LastName = promoter.LastName;
        //    //                objmodel.GetProjectBasic.PromoterName = promoter.FirstName;
        //    //            }
        //    //            else
        //    //            {
        //    //                PD.OrgName = promoter.OrgName;
        //    //                objmodel.GetProjectBasic.PromoterName = promoter.OrgName;
        //    //            }
        //    //            objmodel.PromoterDetails = PD;
        //    //        }
        //    //        if (obj.ProjectType == 4)
        //    //        {
        //    //            objmodel.PlotDetails = _projectService.GetPlotDetailByProjectId(projectId);
        //    //            var data = _projectService.GetTbl_PlotsByProjectId(projectId);
        //    //            if (data.Count > 0)
        //    //                objmodel.Tbl_Plots = data.FirstOrDefault();
        //    //            else
        //    //                objmodel.Tbl_Plots = new Tbl_Plots();
        //    //            if (objmodel.PlotDetails.Count != 0)
        //    //            {
        //    //                objmodel.Sanctioned_Notsanctioned = objmodel.PlotDetails[0].sanctioned_NotSanctioned;
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            var Buildingdata = _projectService.GetBuildingMasterByProjectId(projectId);
        //    //            objmodel.GetBuildingDetails = FillProjectSummaryBuilding(Buildingdata);

        //    //        }
        //    //        objmodel.ProjectProFessionAlDetail = _projectService.GetProfessionaldetail(projectId);
        //    //        //objmodel.GetDocumentsList = FillProjectDocumentList(projectId);
        //    //        objmodel.ProjectDocuments = FillProjectDocuments(projectId);
        //    //        objmodel.ProjectPromoterDetail = FillProjectPromoterDetails(projectId);
        //    //    }
        //    //    ProjectSummaryPaymentModel payment = new ProjectSummaryPaymentModel();
        //    //    if (obj.PaymentStatus != null && obj.PaymentStatus == 1)
        //    //    {
        //    //        //  var payementRow = _userService.GetPaymentDetailById(Convert.ToInt32(obj.PaymentId)); 23/09/2019
        //    //        var payementRow = _userService.GetPaymentDetailByIdandUser(obj.PaymentId, obj.UserId);

        //    //        if (payementRow != null)
        //    //        {
        //    //            decimal num1;
        //    //            bool res = decimal.TryParse(payementRow.Amount, out num1);
        //    //            if (res)
        //    //                payment.PaidAmount = num1;

        //    //            #region   //Get List of ProjectPaymentDetails
        //    //            List<ProjectPaymentDetailsModel> ProjectPaymentDetails = new List<ProjectPaymentDetailsModel>();
        //    //            var propaymentdtls = _projectService.GETProjectPaydetlByProjectidSummary(projectId);
        //    //            if (propaymentdtls != null && propaymentdtls.Count > 0)
        //    //            {
        //    //                foreach (var ExtPayDet in propaymentdtls)
        //    //                {
        //    //                    ProjectPaymentDetailsModel PExtDet = new ProjectPaymentDetailsModel();
        //    //                    PExtDet.PaymentId = ExtPayDet.PaymentID;
        //    //                    PExtDet.PhaseArea = ExtPayDet.PhaseArea;
        //    //                    PExtDet.RegistrationFees = Convert.ToDecimal(ExtPayDet.RegistrationFees);
        //    //                    PExtDet.PenaltyFees = Convert.ToDecimal(ExtPayDet.PenaltyFees);
        //    //                    PExtDet.StandardFees = Convert.ToDecimal(ExtPayDet.StandardFees);
        //    //                    PExtDet.TotalFeetobepaid = Convert.ToDecimal(ExtPayDet.TotalFees);
        //    //                    PExtDet.PaymentStatus = ExtPayDet.paymentstatus;
        //    //                    PExtDet.PaymentStatusId = Convert.ToInt32(ExtPayDet.paymentstatus);
        //    //                    PExtDet.Amount = ExtPayDet.Amount;

        //    //                    ProjectPaymentDetails.Add(PExtDet);
        //    //                }

        //    //                var aDDpENALTYpAY = _projectService.gETADDPenaltyALL(obj.Id, 31, obj.ApplicationNo, 20);
        //    //                if (aDDpENALTYpAY != null && aDDpENALTYpAY.Count > 0)
        //    //                {
        //    //                    foreach (var itm in aDDpENALTYpAY)
        //    //                    {
        //    //                        var mapPay = new ProjectPaymentDetailsModel();
        //    //                        mapPay.Amount = itm.Amount;
        //    //                        mapPay.PaymentId = itm.PaymentID;
        //    //                        mapPay.PaymentStatus = itm.paymentstatus;
        //    //                        ProjectPaymentDetails.Add(mapPay);
        //    //                    }
        //    //                }
        //    //                objmodel.LstProjectPaymentDetls = ProjectPaymentDetails;
        //    //            }
        //    //            #endregion
        //    //        }
        //    //        else//24-09-2018 added case of otherpayment
        //    //        {
        //    //            if (payementRow == null)
        //    //            {
        //    //                var OtherpaymentRow = _userService.GetOtherPaymentdetails(obj.PaymentId);
        //    //                if (OtherpaymentRow != null)
        //    //                {
        //    //                    decimal num1;
        //    //                    bool res = decimal.TryParse(OtherpaymentRow.Amount, out num1);
        //    //                    if (res)
        //    //                        payment.PaidAmount = num1;
        //    //                }
        //    //            }
        //    //        }

        //    //        payment.Amount = obj.Fees != null ? obj.Fees.Value : 0;
        //    //        if ((payment.Amount == payment.PaidAmount) || (payment.Amount < payment.PaidAmount))
        //    //            payment.PaymentStatus = "Paid";
        //    //        else if (payment.PaidAmount > 0)
        //    //            payment.PaymentStatus = "Partially Paid";
        //    //        else
        //    //            payment.PaymentStatus = "Not Paid";

        //    //        payment.PaymentId = Convert.ToInt32(obj.PaymentId);
        //    //    }
        //    //    else
        //    //    {
        //    //        payment.PaidAmount = 0;
        //    //        payment.Amount = obj.Fees != null ? obj.Fees.Value : 0;
        //    //        payment.PaymentStatus = "Not Paid";
        //    //    }
        //    //    objmodel.ProjectSummaryPayment = payment;
        //    //    /////////////////////////////////////////17-07-2019
        //    //    PenaltyProjectSummaryPaymentModel Penaltypayment = new PenaltyProjectSummaryPaymentModel();
        //    //    var objPenalty = _projectService.GetProjectPenalityByProjectId(projectId, 1);
        //    //    if (objPenalty != null)
        //    //    {
        //    //        if (objPenalty.paymentstatus == 1)
        //    //        {
        //    //            var PenaltypayementRow = _userService.GetPaymentDetailById(Convert.ToInt32(objPenalty.PaymentID));

        //    //            if (PenaltypayementRow != null)
        //    //            {
        //    //                decimal num1;
        //    //                bool res = decimal.TryParse(PenaltypayementRow.Amount, out num1);
        //    //                if (res)
        //    //                    // Penaltypayment.PenaltyAmount = num1; //23-09-2019
        //    //                    Penaltypayment.PenaltyPaidAmount = num1;
        //    //            }
        //    //            // Penaltypayment.PenaltyPaidAmount = objPenalty.penalityAmount != null ? objPenalty.penalityAmount.Value : 0; 23-09-2019
        //    //            Penaltypayment.PenaltyAmount = objPenalty.penalityAmount != null ? objPenalty.penalityAmount.Value : 0;
        //    //            if ((Penaltypayment.PenaltyAmount == Penaltypayment.PenaltyPaidAmount) || (Penaltypayment.PenaltyAmount < Penaltypayment.PenaltyPaidAmount))
        //    //                Penaltypayment.PenaltyPaymentStatus = "Paid";
        //    //            else if (Penaltypayment.PenaltyPaidAmount > 0)
        //    //                Penaltypayment.PenaltyPaymentStatus = "Partially Paid";
        //    //            else
        //    //                Penaltypayment.PenaltyPaymentStatus = "Not Paid";
        //    //        }
        //    //        else
        //    //        {
        //    //            Penaltypayment.PenaltyPaidAmount = 0;
        //    //            // Penaltypayment.PenaltyPaidAmount = objPenalty.penalityAmount != null ? objPenalty.penalityAmount.Value : 0; //23-09-2019
        //    //            Penaltypayment.PenaltyAmount = objPenalty.penalityAmount != null ? objPenalty.penalityAmount.Value : 0;
        //    //            Penaltypayment.PenaltyPaymentStatus = "Not Paid";
        //    //        }
        //    //        objmodel.PenaltyProjectSummaryPayment = Penaltypayment;
        //    //    }
        //    //    ///////////////////////////////////////////////////////////////

        //    //    objmodel._ProposedEncumbrance = bREPOSITORY.GetEncumProjectView(projectId);

        //    //    ////////////////////////////////////Project extension details(22-07-2020)/////////////////
        //    //    List<PreviousExt> lst = new List<PreviousExt>();
        //    //    List<ProjectEXtensionVM> PreviouslyExtendedProjectList = new List<ProjectEXtensionVM>();
        //    //    PreviouslyExtendedProjectList = _projectService.GetProjectExtensionbyProjectId("GetAllExtDtlsByProject", projectId);
        //    //    if (PreviouslyExtendedProjectList != null)
        //    //    {
        //    //        foreach (var item in PreviouslyExtendedProjectList)
        //    //        {
        //    //            //string url1 = item.FormF_UploadedPath.Substring(item.FormF_UploadedPath.LastIndexOf('/'), (item.FormF_UploadedPath.Length - item.FormF_UploadedPath.LastIndexOf('/')));
        //    //            PreviousExt modelPre = new PreviousExt();
        //    //            modelPre.ExtensionFromDt = DateTime.ParseExact(item.ExtensionFromDt, "dd/MM/yyyy", CultureInfo.InvariantCulture);// DateTime.TryParse(item.ExtensionFromDt); //Convert.ToDateTime(item.ExtensionFromDt);
        //    //            modelPre.ExtensionToDt = DateTime.ParseExact(item.ExtensionToDt, "dd/MM/yyyy", CultureInfo.InvariantCulture); //Convert.ToDateTime(item.ExtensionToDt);
        //    //            if (string.IsNullOrEmpty(item.FormF_UploadedPath) == false)
        //    //            {
        //    //                if (item.FormF_UploadedPath.Contains("PostApproval"))
        //    //                { modelPre.FormFUploadedPathVal = item.FormF_UploadedPath; }
        //    //                else
        //    //                {
        //    //                    modelPre.FormFUploadedPathVal =
        //    //                        item.FormF_UploadedPath.Substring(item.FormF_UploadedPath.LastIndexOf('/'), (item.FormF_UploadedPath.Length - item.FormF_UploadedPath.LastIndexOf('/')));
        //    //                }
        //    //            }
        //    //            // modelPre.Id = item.Id;
        //    //            lst.Add(modelPre);
        //    //        }
        //    //    }

        //    //    objmodel.GetPreviousExtsList = lst;
        //    //    /////////////////////////////////////////////////////end//////////////////
        //    //    ////////////////////////////////////Map Revision details/////////////////
        //    //    List<MapRevisionModel> lst1 = new List<MapRevisionModel>();
        //    //    List<MapRevisionCore> MapRevisionProjectList = new List<MapRevisionCore>();
        //    //    MapRevisionProjectList = _projectService.GETMAPREVISIONView(projectId);
        //    //    if (MapRevisionProjectList != null)
        //    //    {
        //    //        foreach (var item in MapRevisionProjectList)
        //    //        {
        //    //            MapRevisionModel modelPre1 = new MapRevisionModel();
        //    //            modelPre1.APPROVEDON = item.APPROVEDON;
        //    //            if (string.IsNullOrEmpty(item.PROJECTCERTIFICATE) == false)
        //    //                modelPre1.PROJECTCERTIFICATE =
        //    //                    item.PROJECTCERTIFICATE;
        //    //            lst1.Add(modelPre1);
        //    //        }
        //    //    }

        //    //    objmodel.GetMapRevisionList = lst1;
        //    //    /////////////////////////////////////////////////////end//////////////////
        //    //    List<generalModificationPaymentStatus> GMPayStus = new List<generalModificationPaymentStatus>();
        //    //    var GetGMPayStus = _userService.GetGMPayStus(projectId, 2);
        //    //    if (GetGMPayStus != null && GetGMPayStus.Count > 0)
        //    //    {
        //    //        for (int i = 0; i < GetGMPayStus.Count; i++)
        //    //        {
        //    //            var GMPS = new generalModificationPaymentStatus();
        //    //            GMPS.UpdatedOn = GetGMPayStus[i].UpdatedOn;
        //    //            GMPS.PaymentId = GetGMPayStus[i].PaymentId;
        //    //            GMPS.PNR = GetGMPayStus[i].PNR;
        //    //            GMPayStus.Add(GMPS);
        //    //        }

        //    //    }
        //    //    objmodel.GMPayStusM = GMPayStus;

        //    //    ////Audit trail
        //    //    var auditobj = _projectService.GETAuditStatus(projectId, projectId, 20);
        //    //    objmodel._audit = auditobj;
        //    //    objmodel.UserFlag = CurrentUserRoleId;

        //    //    //getallupdation
        //    //    List<EncumbranceVM> showall = new List<EncumbranceVM>();
        //    //    var showalldata = _projectService.ShowAll(projectId);
        //    //    objmodel._ShowAllByProjectIDList = showalldata;

        //    //    #region Consulatnt contact details
        //    //    var modelCC = new ConsultantContactDetailsModel();
        //    //    var CoreModel = new ConsultantContactDetails();
        //    //    CoreModel = _projectService.GetConsultantDetails(projectId, projectId, "Registration");
        //    //    if (CoreModel != null)
        //    //    {
        //    //        //modelCC.id = CoreModel.Id;
        //    //        objmodel.ConsultantEmail = CoreModel.Email;
        //    //        objmodel.ConsultantMobileNo = CoreModel.MobileNo;
        //    //        objmodel.ConsultantName = CoreModel.Name;
        //    //    }
        //    //    #endregion

        //    //}
        //    return objmodel;
        //}

        //public Project GetProjectMasterById(int id = 0)
        //{
        //    var projectData = _projectRepository.GetById(id);
        //    return projectData;
        //}

        //

        public async Task<int> AddErrorLog(string ErrorDescription)
        {

            int result = 0;
            using (var command = _dbContext.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "usp_Trn_ErrorLog_Mobile";
                command.Parameters.AddWithValue("@action", "_adderrorlog");
                command.Parameters.AddWithValue("@Errordescription", ErrorDescription);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString());
                command.Parameters.Add("@retval_ID", SqlDbType.Int);// out
                command.Parameters["@retval_ID"].Direction = ParameterDirection.Output;// out
                result = await command.ExecuteNonQueryAsync();
                var retval_ID = command.Parameters["@retval_ID"].Value;// out
            }
            return result;

        }


        public async Task<DataTable> GetUserWiseDashBoardDetails(UserDashboardSearchModel model)
        {
            DataTable dt = null;
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetUserWiseDashBoardDetails_API";
                    command.Parameters.AddWithValue("@SSOID", model.SSO_ID);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);
                    CommonFuncationHelper.WriteTextLog($"non static:, dt: {JsonConvert.SerializeObject(model) + "##" + command.CommandText + "##" + JsonConvert.SerializeObject(dt)}");
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }



        public async Task<DataTable> GetDashBoardDetails(string Action = "")
        {
            DataTable dt = null;

            using (var command = _dbContext.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GetDashBoardDetails";
                command.Parameters.AddWithValue("@Action", Action);
                _sqlQuery = command.GetSqlExecutableQuery();// sql query
                dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);
            }
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
            }
            return dt;
        }


        public async Task<SSOUserDetailsModel> MobileLogin(SSO_UserSearchModel model)
        {
            _actionName = "Login(string SSOID, string Password)";
            return await Task.Run(async () =>
            {
                try
                {
                    
                    DataTable dataTable = new DataTable();
                    //using (var command = _dbContext.CreateCommand())
                    //{
                    //    command.CommandType = CommandType.StoredProcedure;
                    //    command.CommandText = "USP_SSOLogin";
                    //    command.Parameters.AddWithValue("@SSOID", model.SSOID);
                    //    command.Parameters.AddWithValue("@Password", model.Password);

                    //    _sqlQuery = command.GetSqlExecutableQuery();
                    //    dataTable = await command.FillAsync_DataTable();
                    //}
                    var data = new SSOUserDetailsModel();
                    if (dataTable != null)
                    {
                        data = CommonFuncationHelper.ConvertDataTable<SSOUserDetailsModel>(dataTable);
                    }
                    return data;
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

        public async Task<DataTable> GetITServiceRequest(GetITServiceRequestModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "API_GetServiceRequestList";
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    command.Parameters.AddWithValue("@PROJECTNAME", model.ProjectName);
                    command.Parameters.AddWithValue("@SubjectName", model.SubjectName);
                    command.Parameters.AddWithValue("@TaskAssignTo", model.TaskAssign);
                    command.Parameters.AddWithValue("@RequestBy", model.RequestBy);
                    command.Parameters.AddWithValue("@RequestResolved", model.Status);
                    command.Parameters.AddWithValue("@Action", "ForAmin");





                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

        public async Task<int> UpdateQPR_Payment_DTLS(LatLongModelUpdateQPR model)
        {
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UpdateQPR_Api";
                    command.Parameters.Add(new SqlParameter("@userId", model.userId));
                    command.Parameters.Add(new SqlParameter("@ProjectId", model.ProjectId));
                    command.Parameters.Add(new SqlParameter("@ModuleId", model.ModuleId));
                    command.Parameters.Add(new SqlParameter("@Latitude", (object?)model.Latitude ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Longitude", (object?)model.Longitude ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CaptureImage1", (object?)model.CaptureImage1 ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CaptureImage2", (object?)model.CaptureImage2 ?? DBNull.Value));
                    _sqlQuery = command.GetSqlExecutableQuery();
                    var result = await command.ExecuteScalarAsync();

                    return (result != null && result != DBNull.Value)
                        ? Convert.ToInt32(result)
                        : 0;
                }
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
                await AddErrorLog(errordetails);
                return -1;
            }
        }

        public async Task<int> UpdateQPR_Payment_DTLSNew(LatLongModelUpdateQPR model)
        {
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SP_InsertUpdate_Tbl_QPRGisImages";
                    command.Parameters.Add(new SqlParameter("@Id", model.Id));
                    command.Parameters.Add(new SqlParameter("@userId", model.userId));
                    command.Parameters.Add(new SqlParameter("@ProjectId", model.ProjectId));
                    command.Parameters.Add(new SqlParameter("@ModuleId", model.ModuleId));
                    command.Parameters.Add(new SqlParameter("@Latitude", (object?)model.Latitude ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Longitude", (object?)model.Longitude ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CaptureImage1", (object?)model.CaptureImage1 ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CaptureImage2", (object?)model.CaptureImage2 ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@WallsImage", (object?)model.WallsImage ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@TerraceImage", (object?)model.TerraceImage ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CommonAreaImage", (object?)model.CommonAreaImage ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@QuarterId", model.QuarterId));
                    _sqlQuery = command.GetSqlExecutableQuery();
                    var result = await command.ExecuteScalarAsync();

                    return (result != null && result != DBNull.Value)
                        ? Convert.ToInt32(result)
                        : 0;
                }
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
                await AddErrorLog(errordetails);
                return -1;
            }
        }


        public async Task<DataTable> GetProjectEndPointsProjectIdWise(GetProjectEndPointsProjectIdWiseModel model)
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SP_GetProjectEndPointsProjectIdWise";
                    command.Parameters.AddWithValue("@projectId", model.ProjectId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }
        public async Task<DataTable> GET_ReraProgressReport()
        {
            DataTable dt = null;
            try
            {

                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GET_ReraProgressReport";
                    //command.Parameters.AddWithValue("@UserId", model.UserId);
                    _sqlQuery = command.GetSqlExecutableQuery();// sql query
                    dt = await _sSqlHelper.FillAsync_ExecuteReaderDT(command);

                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
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
                await AddErrorLog(errordetails);
                return dt;
            }
        }

       public async Task<int> PostreraProgressReport(int total_projects_registered, int residential_projects_registered, int dwelling_units, int carpet_area_lakh_sqmtr, int agents_registered, int cases_filed, int cases_disposed, int state_code, string date)
        {
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SP_InsertWeeklyReraProgressReport";
                    command.Parameters.Add(new SqlParameter("@total_projects_registered", total_projects_registered));
                    command.Parameters.Add(new SqlParameter("@residential_projects_registered", residential_projects_registered));
                    command.Parameters.Add(new SqlParameter("@dwelling_units", dwelling_units));
                    command.Parameters.Add(new SqlParameter("@carpet_area_lakh_sqf", carpet_area_lakh_sqmtr));
                    command.Parameters.Add(new SqlParameter("@agents_registered", agents_registered));
                    command.Parameters.Add(new SqlParameter("@cases_filed", cases_filed));
                    command.Parameters.Add(new SqlParameter("@cases_disposed", cases_disposed));
                    command.Parameters.Add(new SqlParameter("@state_code", state_code));
                    command.Parameters.Add(new SqlParameter("@date", date));
                    _sqlQuery = command.GetSqlExecutableQuery();
                    var result = await command.ExecuteScalarAsync();

                    return (result != null && result != DBNull.Value)
                        ? Convert.ToInt32(result)
                        : 0;
                }
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
                await AddErrorLog(errordetails);
                return -1;
            }
        }

        public async Task<int> updateResponseStatus(int Id, string response)
        {
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SP_updateResponseStatusFromThirdParty";
                    command.Parameters.Add(new SqlParameter("@Id", Id));
                    command.Parameters.Add(new SqlParameter("@response", response));
                    _sqlQuery = command.GetSqlExecutableQuery();
                    var result = await command.ExecuteScalarAsync();

                    return (result != null && result != DBNull.Value)
                        ? Convert.ToInt32(result)
                        : 0;
                }
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
                await AddErrorLog(errordetails);
                return -1;
            }
        }

    }
}
