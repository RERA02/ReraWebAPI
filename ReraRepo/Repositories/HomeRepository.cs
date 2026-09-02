using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QRCoder;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraModel.CommonFunction;
using ReraModel.HomeModel;
using ReraRepo.Helper;
using System;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


//using static EEMS.Models.CommonFunction.ItiTradeAndCollegesDDL;

namespace ReraRepo.Repositories
{
    public class HomeRepository : IHomeRepository

    {
        private readonly DBContext _dbContext;
        private readonly string _pageName;
        private string _actionName;
        private string _sqlQuery;
        private IPAddressModel _iPAddress;
        private string _iPv6Address;
        private string _iPv4Address;
        public HomeRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
            _pageName = "HomeRepository";
            _iPAddress = CommonFuncationHelper.GetIpAddress();
            _iPv6Address = CommonFuncationHelper.GetIpAddress().IPv6_Address;
            _iPv4Address = CommonFuncationHelper.GetIpAddress().IPv4_Address;
        }



        public async Task<List<LatestJudgementVM>> GetLatestJudgements(int recCount)
        {
            var list = new List<LatestJudgementVM>();

            using (var command = _dbContext.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_Get_Website_LatestJudgements";
                command.Parameters.AddWithValue("@RecCount", recCount);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new LatestJudgementVM
                        {
                            DocumentName = reader["DocumentName"]?.ToString(),
                            ComplaintNumber = reader["ComplaintNumber"]?.ToString(),
                            PartyName = reader["PartyName"]?.ToString(),
                            DocumentDate = reader["DocumentDate"] == DBNull.Value
                                ? DateTime.MinValue
                                : Convert.ToDateTime(reader["DocumentDate"])
                        });
                    }
                }
            }

            return list;
        }


        public async Task<List<DashboardVM>> GetHomePageCount()
        {
            _actionName = "GetHomePageCount()";

            return await Task.Run(async () =>
            {
                try
                {
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetHomePageCount";

                        _sqlQuery = command.GetSqlExecutableQuery();
                        var ds = await command.FillAsync();

                        var list = new List<DashboardVM>();

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                list.Add(new DashboardVM
                                {
                                    ProjectApproved = row["ProjectApproved"] == DBNull.Value ? 0 : Convert.ToInt32(row["ProjectApproved"]),
                                    ComplaintNewApproved = row["ComplaintNewApproved"] == DBNull.Value ? 0 : Convert.ToInt32(row["ComplaintNewApproved"]),
                                    ComplaintOldApproved = row["ComplaintOldApproved"] == DBNull.Value ? 0 : Convert.ToInt32(row["ComplaintOldApproved"]),
                                    AgentApproved = row["AgentApproved"] == DBNull.Value ? 0 : Convert.ToInt32(row["AgentApproved"]),
                                    PromoterApproved = row["PromoterApproved"] == DBNull.Value ? 0 : Convert.ToInt32(row["PromoterApproved"])


                                });
                            }
                        }

                        return list;
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

                    throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
                }
            });
        }

        public async Task<DataTable> GetProjectsListForOuterSearchNew(ProjectSearchParams searchParams, string ProjectOrExemption)
        {
            _actionName = "GetProjectsListForOuterSearchNew()";

            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dt = new DataTable();

                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        if (searchParams.ApplicationStatus == 7)
                        {
                            command.CommandText = "GetLapsedSuspendedRevokedProjectForGrid_New";

                            command.Parameters.AddWithValue("@PROJECTNAME", searchParams.ProjectName ?? "");
                            command.Parameters.AddWithValue("@APPLICATIONNO", searchParams.ApplicationNo ?? "");
                            command.Parameters.AddWithValue("@REGISTRATIONNO", searchParams.CertificateNo ?? "");
                            command.Parameters.AddWithValue("@ApplicationStatus", searchParams.ApplicationStatus);
                            command.Parameters.AddWithValue("@DISTRICTID", searchParams.District);
                            command.Parameters.AddWithValue("@TESHILID", searchParams.Teshil);
                            command.Parameters.AddWithValue("@UserFullName", searchParams.UserFullName ?? "");
                        }
                        else
                        {
                            command.CommandText = "SEARCHPROJECTDTLS";

                            command.Parameters.AddWithValue("@PROJECTNAME", searchParams.ProjectName ?? "");
                            command.Parameters.AddWithValue("@PROJECTSTATUS", searchParams.ApplicationStatus);
                            command.Parameters.AddWithValue("@DISTRICTID", searchParams.District);
                            command.Parameters.AddWithValue("@PROMOTERNAME", searchParams.PromoterName ?? "");
                            command.Parameters.AddWithValue("@REGISTRATIONNO", searchParams.CertificateNo ?? "");
                            command.Parameters.AddWithValue("@APPROVEDFROM",
                                searchParams.ApprovedOnFrom == null
                                ? "1900-01-01 00:00:00.000"
                                : Convert.ToString(searchParams.ApprovedOnFrom));

                            command.Parameters.AddWithValue("@APPROVEDTO",
                                searchParams.ApprovedOnTo == null
                                ? "1900-01-01 00:00:00.000"
                                : Convert.ToString(searchParams.ApprovedOnTo));

                            command.Parameters.AddWithValue("@RoleDesgination", searchParams.RoleDesgination ?? "");
                            command.Parameters.AddWithValue("@RoleModule", searchParams.RoleModule ?? "");
                            command.Parameters.AddWithValue("@PendingStatus", searchParams.PendingStatus ?? "");
                            command.Parameters.AddWithValue("@APPLICATIONNO", searchParams.ApplicationNo ?? "");
                            command.Parameters.AddWithValue("@SOURCE", searchParams.SOURCE ?? 0);
                            command.Parameters.AddWithValue("@RevisedRC", searchParams.RevisedCertiRC);
                            command.Parameters.AddWithValue("@DailyTDt", searchParams.DailyTDt ?? "");
                            command.Parameters.AddWithValue("@Status", searchParams.Status);
                            command.Parameters.AddWithValue("@ProjectOrExemption", ProjectOrExemption);
                            command.Parameters.AddWithValue("@DayWiseCount", searchParams.DayWiseCount ?? "");
                            command.Parameters.AddWithValue("@TESHILID", searchParams.Teshil);
                            command.Parameters.AddWithValue("@PROJECTTYPE", searchParams.ProjectType);
                        }

                        _sqlQuery = command.GetSqlExecutableQuery();
                        dt = await command.FillAsync_DataTable();
                    }


                    if (!dt.Columns.Contains("EncryptProjectId"))
                        dt.Columns.Add("EncryptProjectId", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        int appStatus = dt.Columns.Contains("ApplicationStatus")
                        ? Convert.ToInt32(row["ApplicationStatus"] == DBNull.Value ? 0 : row["ApplicationStatus"])
                        : 0;

                        if (appStatus == 5)
                        {
                            row["CertificateNo"] = "Revoked Project";
                            row["RegistrationNo"] = "Revoked Project";
                            row["ApplicationNo"] = "Revoked Project";
                        }
                        else if (appStatus == 6)
                        {
                            row["CertificateNo"] = "Suspended Project";
                            row["RegistrationNo"] = "Suspended Project";
                            row["ApplicationNo"] = "Suspended Project";
                        }

                        row["EncryptProjectId"] =
                            CommonFuncationHelper.EncryptRera(Convert.ToString(row["Id"]));
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

                    throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
                }
            });
        }


        public async Task<DataTable> GetProjects(SearchProjectRequestModel searchParams)
        {
            _actionName = "GetProjects";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;


                    if (searchParams.ApplicationStatus == 7)
                    {
                        command.CommandText = "GetLapsedSuspendedRevokedProjectForGrid";
                        command.Parameters.Add(new SqlParameter("@ApplicationStatus", searchParams.ApplicationStatus));
                        //command.Parameters.Add(new SqlParameter("@DistrictName", searchParams.DistrictName));
                        command.Parameters.Add(new SqlParameter("@UserFullName", searchParams.PromoterName ?? ""));
                        command.Parameters.Add(new SqlParameter("@DistrictName", searchParams.DistrictId));
                        command.Parameters.Add(new SqlParameter("@PROJECTNAME", searchParams.ProjectName ?? ""));
                        command.Parameters.Add(new SqlParameter("@applicationNo", searchParams.ApplicationNo ?? ""));
                        command.Parameters.Add(new SqlParameter("@RegistrationNo", searchParams.RegistrationNo ?? ""));
                        command.Parameters.Add(new SqlParameter("@TESHILID", searchParams.TeshilId));
                        command.Parameters.Add(new SqlParameter("@PROJECTTYPE", searchParams.ProjectType));
                        command.Parameters.Add(new SqlParameter("@YEAR", searchParams.Year));
                        //command.Parameters.Add(new SqlParameter("@PROMOTERNAME", searchParams.PromoterName ?? ""));
                    }
                    else
                    {
                        command.CommandText = "SEARCHPROJECTDTLS";

                        command.Parameters.Add(new SqlParameter("@PROJECTNAME", searchParams.ProjectName ?? ""));
                        command.Parameters.Add(new SqlParameter("@PROJECTSTATUS", searchParams.ApplicationStatus));
                        command.Parameters.Add(new SqlParameter("@DISTRICTID", searchParams.DistrictId));
                        command.Parameters.Add(new SqlParameter("@PROMOTERNAME", searchParams.PromoterName ?? ""));
                        command.Parameters.Add(new SqlParameter("@REGISTRATIONNO", searchParams.RegistrationNo ?? ""));
                        command.Parameters.Add(new SqlParameter("@APPROVEDFROM", searchParams.ApprovedFrom == null ? Convert.ToString("1900-01-01 00:00:00.000") : Convert.ToString(searchParams.ApprovedFrom)));
                        command.Parameters.Add(new SqlParameter("@APPROVEDTO", searchParams.ApprovedTo == null ? Convert.ToString("1900-01-01 00:00:00.000") : Convert.ToString(searchParams.ApprovedTo)));
                        command.Parameters.Add(new SqlParameter("@RoleDesgination", searchParams.RoleDesgination ?? ""));
                        command.Parameters.Add(new SqlParameter("@RoleModule", searchParams.RoleModule ?? ""));
                        command.Parameters.Add(new SqlParameter("@PendingStatus", searchParams.PendingStatus ?? ""));
                        command.Parameters.Add(new SqlParameter("@APPLICATIONNO", searchParams.ApplicationNo ?? ""));
                        command.Parameters.Add(new SqlParameter("@SOURCE", searchParams.Source));
                        command.Parameters.Add(new SqlParameter("@RevisedRC", searchParams.RevisedCertiRC));
                        command.Parameters.Add(new SqlParameter("@DailyTDt", searchParams.DailyTDt ?? ""));
                        command.Parameters.Add(new SqlParameter("@Status", searchParams.Status));
                        command.Parameters.Add(new SqlParameter("@ProjectOrExemption", searchParams.ProjectOrExemption ?? ""));
                        command.Parameters.Add(new SqlParameter("@DayWiseCount", searchParams.DayWiseCount ?? ""));
                        command.Parameters.Add(new SqlParameter("@TESHILID", searchParams.TeshilId));
                        command.Parameters.Add(new SqlParameter("@PROJECTTYPE", searchParams.ProjectType));
                        command.Parameters.Add(new SqlParameter("@YEAR", searchParams.Year));


                    }

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

        public async Task<DataTable?> GetPublicPageProjectDetailsAsync(string action, int projectId)
        {
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PROJECTDTL_PUBLICPAGE";
                    command.Parameters.Add(new SqlParameter("@Action", action));
                    command.Parameters.Add(new SqlParameter("@ProjectId", projectId));
                    var dataTable = new DataTable();
                    _sqlQuery = command.GetSqlExecutableQuery();
                    dataTable = await command.FillAsync_DataTable();

                    return dataTable;
                }

            }
            catch (Exception ex)
            {
                // Log exception if needed
                Console.Error.WriteLine($"Error fetching project details: {ex.Message}");
                return null;
            }
        }


        public async Task<List<DataTable>> GetCommonFunctionByProjectAsync(string action, int projectId)
        {
            var result = new List<DataTable>();

            string spName = action switch
            {
                "Extension" => "SP_Extension_Website_Table",
                "Revocation" => "SP_Revocation_Website_Table",
                "Encumbrance" => "SP_Encumbrance_Website_Table",
                "DocumentType" => "GetUploaded_ProjectDocuments",
                "Project" => "SP_Project_Website_Table",
                _ => throw new ArgumentException($"Unknown action: {action}")
            };

            try
            {
                using var command = _dbContext.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                // Special handling: Project vs DocumentType vs others
                if (action == "DocumentType")
                {
                    command.CommandText = spName;
                    command.Parameters.Add(new SqlParameter("@ProjectId", projectId));
                }
                else
                {
                    command.CommandText = spName;
                    command.Parameters.Add(new SqlParameter("@action", action));
                    command.Parameters.Add(new SqlParameter("@ProjectId", projectId));
                }

                // Optional: store generated SQL for debugging
                _sqlQuery = command.GetSqlExecutableQuery();

                using var reader = await command.ExecuteReaderAsync();

                do
                {
                    var table = new DataTable();
                    table.Load(reader);
                    result.Add(table);
                }
                while (!reader.IsClosed && await reader.NextResultAsync());

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching project details for {action}: {ex.Message}");
                return new List<DataTable>();
            }
        }

        public async Task<int> ContactUs(QueryVM body)
        {
            _actionName = "ContactUs(QueryVM body)";

            try
            {
                using (var command = _dbContext.CreateCommand(true))
                {
                    command.CommandText = "Usp_Web_Save_WebSite_ContactUs";
                    command.CommandType = CommandType.StoredProcedure;

                    // INPUT parameters
                    command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 200)
                        .Value = body.FirstName ?? (object)DBNull.Value;

                    command.Parameters.Add("@LastName", SqlDbType.NVarChar, 200)
                        .Value = body.LastName ?? (object)DBNull.Value;

                    command.Parameters.Add("@EmailId", SqlDbType.NVarChar, 500)
                        .Value = body.EmailAddress ?? (object)DBNull.Value;

                    command.Parameters.Add("@PhoneNo", SqlDbType.NVarChar, 20)
                        .Value = body.MobileNo ?? (object)DBNull.Value;

                    command.Parameters.Add("@QueryRelatedToId", SqlDbType.Int)
                        .Value = body.QueryType > 0 ? body.QueryType : (object)DBNull.Value;

                    command.Parameters.Add("@QueryRelatedToText", SqlDbType.NVarChar, 200)
                        .Value = body.QueryText ?? (object)DBNull.Value;

                    command.Parameters.Add("@QueryMessage", SqlDbType.NVarChar)
                        .Value = body.Query ?? (object)DBNull.Value;

                    command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 200)
                        .Value = "Visitor";

                    // OUTPUT parameter
                    var returnParam = command.Parameters.Add("@Return", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.Output;

                    _sqlQuery = command.GetSqlExecutableQuery();

                    await command.ExecuteNonQueryAsync();

                    return Convert.ToInt32(returnParam.Value);
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

                throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
            }
        }


        public async Task<int> GetVisitorCount(string key)
        {
            _actionName = "GetVisitorCount";

            try
            {
                using (var command = _dbContext.CreateCommand(true))
                {
                    command.CommandText = "SP_Increment_Website_Visitor";
                    command.CommandType = CommandType.StoredProcedure;

                    // INPUT parameter
                    command.Parameters.Add("@ActionType", SqlDbType.NVarChar, 200)
                           .Value = key ?? (object)DBNull.Value;

                    _sqlQuery = command.GetSqlExecutableQuery();

                    // Since SP returns SELECT VisitCount
                    var result = await command.ExecuteScalarAsync();

                    return result != null ? Convert.ToInt32(result) : 0;
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

                throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
            }
        }


        public async Task<DataTable> GetComplaintsAgainstRespondentList(GetComplaintsAgainstRespondenSearch model)
        {
            _actionName = "GetComplaintsAgainstRespondentList(GetComplaintsAgainstRespondenSearch model)";

            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dt = new DataTable();

                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Usp_Web_GetComplaintsdetailsbyComplaintNumber";

                        command.Parameters.AddWithValue("@Action", model.Action ?? "");
                        command.Parameters.AddWithValue("@ComplaintNumber", model.ComplaintNumber ?? "");
                        _sqlQuery = command.GetSqlExecutableQuery();
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

                    throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
                }
            });
        }

        public async Task<DataTable> GetAgentsList(SearchAgentRequestModel searchParams)
        {
            _actionName = "GetAgentsList";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetProjectSearchResults";

                    command.Parameters.Add(new SqlParameter("@District", searchParams.districtId));
                    command.Parameters.Add(new SqlParameter("@Taluka", searchParams.tehsilId));
                    command.Parameters.Add(new SqlParameter("@ProjectName", searchParams.agentName ?? ""));
                    //command.Parameters.Add(new SqlParameter("@PromoterName", searchParams.ProjectName ?? ""));
                    command.Parameters.Add(new SqlParameter("@CertificateNo", searchParams.registrationNo ?? ""));

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


        public async Task<DataSet> GetComplaintFullDetails(ComplaintSearchRequestModel model)
        {
            _actionName = "GetComplaintFullDetails";
            DataSet ds = new DataSet();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;


                    command.CommandText = "GetComplaintFullDetails";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", model.ComplaintNumber ?? ""));

                    var newTable = await command.FillAsync_DataTable();
                    newTable.TableName = "NewComplaints";
                    newTable.Columns.Add("TypeOldNew", typeof(string));

                    foreach (DataRow row in newTable.Rows)
                        row["TypeOldNew"] = "New";

                    ds.Tables.Add(newTable);


                    command.CommandText = "GetOldComplaintsfordisplay";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", model.ComplaintNumber ?? ""));
                    command.Parameters.Add(new SqlParameter("@ComplaintTypeId", model.ComplaintTypeId));

                    var oldTable = await command.FillAsync_DataTable();
                    oldTable.TableName = "OldComplaints";
                    oldTable.Columns.Add("TypeOldNew", typeof(string));

                    foreach (DataRow row in oldTable.Rows)
                        row["TypeOldNew"] = "Old";

                    ds.Tables.Add(oldTable);
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

                throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
            }
        }

        public async Task<DataTable> GetWeb_TBL_AUDITTRAIL(Web_TBL_AUDITTRAILSearchModel model)
        {
            _actionName = "GetWeb_TBL_AUDITTRAIL";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Web_TBL_AUDITTRAIL";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@Projectid", model.Projectid));
                    command.Parameters.Add(new SqlParameter("@TableId", model.TableId));
                    command.Parameters.Add(new SqlParameter("@Type", model.Type));

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

        public async Task<DataTable> GetWeb_GetAllNotice(Web_GetAllNoticeSearModel model)
        {
            _actionName = "GetWeb_GetAllNotice";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Web_GetAllNotice";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@Action", model.Action));
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", model.ComplaintNumber));
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

        public async Task<DataTable> TblComplaintDetails(ComplaintDetailsS_M model)
        {
            _actionName = "TblComplaintDetails";
            DataTable dataTable = new DataTable();
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Web_ComplaintDetails";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@Id", model.Id));
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

        //public async Task<DataSet> GetProjectById(int projectId)
        //{
        //    List<DataTable> Project = await GetCommonFunctionByProjectAsync("Project", projectId);
        //    List<DataTable> extensionTask = await GetCommonFunctionByProjectAsync("Extension", projectId);
        //    List<DataTable> revocationTask = await GetCommonFunctionByProjectAsync("Revocation", projectId);
        //    List<DataTable> encumbranceTask = await GetCommonFunctionByProjectAsync("Encumbrance", projectId);
        //    List<DataTable> documentTypeTask = await GetCommonFunctionByProjectAsync("DocumentType", projectId);

        //    var result = new DataSet();

        //    return result;
        //}

        public async Task<DataSet> GetProjectById(int projectId)
        {
            // Run all SP calls in parallel
            var projectTask = GetCommonFunctionByProjectAsync("Project", projectId);
            var extensionTask = GetCommonFunctionByProjectAsync("Extension", projectId);
            var revocationTask = GetCommonFunctionByProjectAsync("Revocation", projectId);
            var encumbranceTask = GetCommonFunctionByProjectAsync("Encumbrance", projectId);
            var documentTypeTask = GetCommonFunctionByProjectAsync("DocumentType", projectId);

            await Task.WhenAll(projectTask, extensionTask, revocationTask, encumbranceTask, documentTypeTask);

            var result = new DataSet();

            // Helper to safely add tables with proper names
            void AddTables(List<DataTable> tables, string prefix)
            {
                if (tables == null || tables.Count == 0) return;

                for (int i = 0; i < tables.Count; i++)
                {
                    var table = tables[i].Copy();
                    table.TableName = tables.Count > 1 ? $"{prefix}_{i + 1}" : prefix;
                    result.Tables.Add(table);
                }
            }

            AddTables(projectTask.Result, "Project");
            AddTables(extensionTask.Result, "Extension");
            AddTables(revocationTask.Result, "Revocation");
            AddTables(encumbranceTask.Result, "Encumbrance");
            AddTables(documentTypeTask.Result, "DocumentType");

            return result;
        }


        public async Task<ComplaintPDFVM> GetcomplaintsDetailsForPDFnew(ComplaintsDetailsS_M model)
        {
            _actionName = "GetcomplaintsDetailsForPDFnew";
            DataTable dataTable;

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Web_GetComplaintsdetails";
                    command.Parameters.Clear();

                    command.Parameters.Add(new SqlParameter("@Action", model.Action));
                    command.Parameters.Add(new SqlParameter("@UserID", model.UserId));
                    command.Parameters.Add(new SqlParameter("@ComplaintID", model.ComplaintID));
                    command.Parameters.Add(new SqlParameter("@offAddOrResAdd", 0));
                    command.Parameters.Add(new SqlParameter("@RegisteredOrNonRegistered", 0));
                    command.Parameters.Add(new SqlParameter("@RespondentDetailId", 0));
                    command.Parameters.Add(new SqlParameter("@RespondentType", 0));
                    command.Parameters.Add(new SqlParameter("@TempComplaintNumber", string.Empty));
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", string.Empty));
                    command.Parameters.Add(new SqlParameter("@complaintType", 0));

                    _sqlQuery = command.GetSqlExecutableQuery();
                    dataTable = await command.FillAsync_DataTable();
                }


                ComplaintPDFVM result = new ComplaintPDFVM();

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];

                    result.TempComplaintNo = Convert.ToString(row["TempComplaintNo"]);
                    result.UserRoleId = row["UserRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(row["UserRoleId"]);

                    result.Listofenclosures = Convert.ToString(row["Listofenclosures"]);
                    result.ComplaintNumber = Convert.ToString(row["ComplaintNumber"]);

                    result.MultipleComplainantName = Convert.ToString(row["MultipleComplainantName"]);
                    result.MultipleRespondantName = Convert.ToString(row["MultipleRespondantName"]);

                    result.ComplainantName = Convert.ToString(row["ComplainantName"]);
                    result.ComplainantOffice = Convert.ToString(row["ComplainantOffice"]);
                    result.ComplainantResidenceOffice = Convert.ToString(row["ComplainantResidenceOffice"]);
                    //result.ComplainantService1 = Convert.ToString(row["ComplainantService1"]);
                    result.ComplainantService = Convert.ToString(row["ComplainantService"]);
                    result.ComplainantMobilieNumber = Convert.ToString(row["ComplainantMobilieNumber"]);
                    result.ComplainantEmail = Convert.ToString(row["ComplainantEmail"]);

                    result.ProjectName = Convert.ToString(row["ProjectName"]);

                    result.RespondentName = Convert.ToString(row["RespondentName"]);
                    result.RespondentOffice = Convert.ToString(row["RespondentOffice"]);
                    result.RegistrationNo = Convert.ToString(row["RegistrationNo"]);
                    result.RespondentService = Convert.ToString(row["RespondentService"]);
                    result.RERARespondentService = Convert.ToString(row["RERARespondentService"]);
                    result.RespondentMobileNumber = Convert.ToString(row["RespondentMobileNumber"]);
                    result.RespondentEmail = Convert.ToString(row["RespondentEmail"]);

                    //result.RERACaseDateofDecision =
                    //    row["RERACaseDateofDecision"] == DBNull.Value ? null : Convert.ToString(row["RERACaseDateofDecision"]);
                    result.RERACaseDateofDecision =
    row["RERACaseDateofDecision"] == DBNull.Value
        ? string.Empty
        : Convert.ToDateTime(row["RERACaseDateofDecision"]).ToString("yyyy-MM-dd");


                    result.RERACaseNumber = Convert.ToString(row["RERACaseNumber"]);

                    result.JurisdictionofRERA =
                        row["JurisdictionofRERA"] != DBNull.Value && Convert.ToInt32(row["JurisdictionofRERA"]) == 1;

                    result.ReliefSought = Convert.ToString(row["ReliefSought"]);
                    result.FactOfCase = Convert.ToString(row["FactOfCase"]);

                    result.InterimOrder = row["InterimOrder"] == DBNull.Value ? false : Convert.ToBoolean(row["InterimOrder"]);
                    result.IterimOrderDetails = Convert.ToString(row["IterimOrderDetails"]);
                    result.Grounds = Convert.ToString(row["Grounds"]);

                    result.Residanse = Convert.ToString(row["Residanse"]);
                    result.Place = Convert.ToString(row["Place"]);
                    result.FillingDate = row["FillingDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["FillingDate"]).ToString("yyyy-MM-dd");

                    result.NotPendingMatter =
                        row["NotPendingMatter"] != DBNull.Value && Convert.ToInt32(row["NotPendingMatter"]) == 1;

                    result.ComplaintPendingWithOtherCourtDetails =
                        Convert.ToString(row["ComplaintPendingWithOtherCourtDetails"]);

                    result.PaymentStatus =
                        row["PaymentStatus"] == DBNull.Value ? 0 : Convert.ToInt32(row["PaymentStatus"]);

                    result.Amount =
                        row["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Amount"]);

                    result.FatherName = Convert.ToString(row["FatherName"]);
                    result.Age = row["Age"] == DBNull.Value ? 0 : Convert.ToInt32(row["Age"]);

                    result.FactOfCaseUnderSection = Convert.ToString(row["FactOfCaseUnderSection"]);
                    result.FactOfCaseYear = Convert.ToString(row["FactOfCaseYear"]);
                    result.FactOfCaseAct = Convert.ToString(row["FactOfCaseAct"]);

                    //result.Limitation = row["Limitation"] == DBNull.Value ? 0 : Convert.ToInt32(row["Limitation"]);
                    result.LimitationDetails = Convert.ToString(row["LimitationDetails"]);

                    result.ApplicantAllotment = Convert.ToString(row["ApplicantAllotment"]);

                    result.AdvocateName = Convert.ToString(row["AdvocateName"]);
                    result.AdvocateMobileNo = Convert.ToString(row["AdvocateMobileNo"]);
                    result.AdvocateEmail = Convert.ToString(row["AdvocateEmail"]);

                    result.CAName = Convert.ToString(row["CAName"]);
                    result.CAMobileNo = Convert.ToString(row["CAMobileNo"]);
                    result.CAEmail = Convert.ToString(row["CAEmail"]);
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

                throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
            }
        }



        //private static void AddTableIfExists(DataSet target, DataSet source, string tableName)
        //{
        //    if (source != null && source.Tables.Count > 0)
        //    {
        //        var table = source.Tables[0].Copy();
        //        table.TableName = tableName;
        //        target.Tables.Add(table);
        //    }
        //}


        public async Task<DataTable> GetComplaintDocumentUploadsByComplaintNumber(ComplaintDocumentUploadsS_M model)
        {
            _actionName = "GetComplaintDocumentUploadsByComplaintNumber";
            DataTable dataTable = new DataTable();
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Web_ComplaintDocumentUploads";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", model.ComplaintNumber));
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

        public async Task<DataTable> GetComplainantList(ComplaintdetailsSearchModel searchParams)
        {
            _actionName = "GetComplainantList";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //command.CommandText = "Usp_Web_GetAgentsList";

                    //command.Parameters.Add(new SqlParameter("@District", searchParams.districtId));
                    //command.Parameters.Add(new SqlParameter("@Teshil", searchParams.tehsilId));
                    //command.Parameters.Add(new SqlParameter("@ProjectName", searchParams.agentName ?? ""));
                    //command.Parameters.Add(new SqlParameter("@PromoterName", searchParams.ProjectName ?? ""));
                    //command.Parameters.Add(new SqlParameter("@CertificateNo", searchParams.registrationNo ?? ""));

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

        public async Task<DataTable> GetUploadedDocumentsSP(GetUploadedDocumentsSPModel model)
        {
            _actionName = "GetUploadedDocumentsSP";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    if (model.id != 1)
                    {
                        model.DocumentName = "";
                    }
                    string fromDate = string.IsNullOrEmpty(model.OrderfromDate) ? "" : DateTime.Parse(model.OrderfromDate).ToString("yyyy-MM-dd");
                    string toDate = string.IsNullOrEmpty(model.OrdertoDate) ? "" : DateTime.Parse(model.OrdertoDate).ToString("yyyy-MM-dd");


                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Web_GetDocuments";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@PageId", model.id));
                    command.Parameters.Add(new SqlParameter("@OrderfromDate", fromDate));
                    command.Parameters.Add(new SqlParameter("@OrdertoDate", toDate));
                    command.Parameters.Add(new SqlParameter("@complientNo", model.complientNo));
                    command.Parameters.Add(new SqlParameter("@PartyName", model.PartyName));
                    command.Parameters.Add(new SqlParameter("@DocumentName", model.DocumentName));
                    command.Parameters.Add(new SqlParameter("@Commontext", model.Commontext));
                    command.Parameters.Add(new SqlParameter("@FinYearId", model.FinYearId));
                    command.Parameters.Add(new SqlParameter("@ordernumber", model.OrderNumber));
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


        public async Task<DataTable> GetUpdatedDailyCauseListbyparameter()
        {
            _actionName = "GetUpdatedDailyCauseListbyparameter";
            DataTable dataTable = new DataTable();
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetUpdatedDailyCauseListbyparameter";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@Action", "PUBLIC"));
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", ""));
                    command.Parameters.Add(new SqlParameter("@PartyName", ""));
                    command.Parameters.Add(new SqlParameter("@FromDate", ""));
                    command.Parameters.Add(new SqlParameter("@ToDate", ""));

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

        public async Task<DataTable> GetViolationActCompliants(GetViolationActCompliantsModel model)
        {
            _actionName = "GetViolationActCompliants";
            DataTable dataTable = new DataTable();
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetViolationComplaintsdetails";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@Action", model.Action));
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", model.Compalaintno));
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


        public async Task<DataTable> Get_ProjectStatus(int? ProjectID)
        {
            _actionName = "Get_ProjectStatus()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Get_ProjectStatus";
                        command.Parameters.AddWithValue("@ProjectID", ProjectID);

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

        public async Task<int> SaveComplaintBox(ComplaintBoxVM model)
        {
            _actionName = "SaveComplaintBox";
            int newId = 0;

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SP_Save_WebSite_ComplaintBox";
                    command.Parameters.Clear();

                    command.Parameters.Add(new SqlParameter("@IsIdentityDisclosed", model.IsIdentityDisclosed ?? false));
                    command.Parameters.Add(new SqlParameter("@ComplainantName", model.ComplainantName ?? ""));
                    command.Parameters.Add(new SqlParameter("@ComplainantMobile", model.ComplainantMobile ?? ""));
                    command.Parameters.Add(new SqlParameter("@ComplainantEmail", model.ComplainantEmail ?? ""));
                    command.Parameters.Add(new SqlParameter("@ComplainantAddress", model.ComplainantAddress ?? ""));
                    command.Parameters.Add(new SqlParameter("@ProjectName", model.ProjectName ?? ""));
                    command.Parameters.Add(new SqlParameter("@ProjectAddress", model.ProjectAddress ?? ""));
                    command.Parameters.Add(new SqlParameter("@DistrictName", model.DistrictName ?? ""));
                    command.Parameters.Add(new SqlParameter("@Location", model.Location ?? ""));
                    command.Parameters.Add(new SqlParameter("@ComplaintDescription", model.ComplaintDescription ?? ""));
                    command.Parameters.Add(new SqlParameter("@PromoterName", model.PromoterName ?? ""));
                    command.Parameters.Add(new SqlParameter("@PromoterAddress", model.PromoterAddress ?? ""));
                    command.Parameters.Add(new SqlParameter("@PromoterContactNumber", model.PromoterContactNumber ?? ""));
                    command.Parameters.Add(new SqlParameter("@UploadedDocument", model.UploadedDocument ?? ""));
                    command.Parameters.Add(new SqlParameter("@CreatedBy", model.CreatedBy ?? ""));

                    _sqlQuery = command.GetSqlExecutableQuery();

                    var result = await command.ExecuteScalarAsync();
                    newId = result != null ? Convert.ToInt32(result) : 0;
                }

                return newId;
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

        public void AddVisitorCount(string key)
        {
            _actionName = "AddVisitorCount";

            try
            {

                using (var command = _dbContext.CreateCommand(false))
                {
                    command.CommandText = "SP_Increment_Website_Visitor";
                    command.CommandType = System.Data.CommandType.StoredProcedure;


                    command.Parameters.Add("@ActionType", System.Data.SqlDbType.NVarChar, 200)
                           .Value = key ?? (object)DBNull.Value;


                    command.ExecuteNonQuery();
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

                throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
            }
        }


        public async Task<DataTable> Get_ViewReplyResponse(string ComplaintNumber)
        {
            _actionName = "Get_ViewReplyResponse()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetComplaintResponseforHome";
                        command.Parameters.AddWithValue("@ComplaintNumber", ComplaintNumber);

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

        public async Task<DataTable> GetWeb_TBL_Rejoinder(Web_TBL_RejoinderSearchModel model)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetAllFiledRejoinder";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", model.ComplaintNumber));

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


        public async Task<DataTable> GetComplaintDetailsWebsite(ComplaintDetailsModel model)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetComplaintFullDetails_web";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@ComplaintNumber", model.compalaint_no));
                    command.Parameters.Add(new SqlParameter("@complaint_status", model.complaint_status));
                    command.Parameters.Add(new SqlParameter("@complainant_name", model.complainant));
                    command.Parameters.Add(new SqlParameter("@respondent_name", model.respondent_name));
                    command.Parameters.Add(new SqlParameter("@ComplaintTypeId", model.ComplaintTypeId));
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



        public async Task<DataTable> GetProjectIdWiseEndPointsLocation(int? ProjectID)
        {
            _actionName = "GetProjectIdWiseEndPointsLocation()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Web_Sp_ProjectEndPoints";
                        command.Parameters.AddWithValue("@ProjectID", ProjectID);

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

        public async Task<DataTable> GetDeflauterlist(SearchProjectRequestModel searchParams)
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
                        command.Parameters.AddWithValue("@Financialyear", searchParams.Year);
                        command.Parameters.AddWithValue("@Monthwise", 0);
                        command.Parameters.AddWithValue("@RegistrationNo", searchParams.RegistrationNo);
                        command.Parameters.AddWithValue("@PromoterName", searchParams.PromoterName);
                        command.Parameters.AddWithValue("@DistrictId", searchParams.DistrictId);
                        command.Parameters.AddWithValue("@ProjectName", searchParams.ProjectName);
                        command.Parameters.AddWithValue("@OrderPassedby", searchParams.OrderPassedby);
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

        public async Task<DataTable> GetAllNotificationsbyUser(NotificationsModel model)
        {
            _actionName = "GetAllNotificationsbyUser";
            DataTable dataTable = new DataTable();

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Save_NotificationbyDeviceId";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@Action", "GetData"));
                    command.Parameters.Add(new SqlParameter("@UserId", model.UserId));
                    command.Parameters.Add(new SqlParameter("@Sso_Id", model.SSOId));
                    command.Parameters.Add(new SqlParameter("@DeviceId", model.DeviceId));

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

        public async Task<bool> UpdateNotificationStatus(NotificationsStatusModel model)
        {
            _actionName = "UpdateNotificationStatus";

            try
            {
                using (var command = _dbContext.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Usp_Update_NotificationbyDeviceId";

                    command.Parameters.Clear();

                    //command.Parameters.Add(new SqlParameter("@Action", "UpdateNotificationStatus"));
                    command.Parameters.Add(new SqlParameter("@Id", model.Id));
                    command.Parameters.Add(new SqlParameter("@CreatedBy", model.UpdatedBy));

                    _sqlQuery = command.GetSqlExecutableQuery();

                    object result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result) == 1;
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

                throw new Exception(CommonFuncationHelper.MakeError(errorDesc), ex);
            }
        }


        public async Task<DataTable> GetprojectDetailsbyDepecryValue(int? ProjectID)
        {
            _actionName = "GetprojectDetailsbyDepecryValue()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Web_Sp_SingleProjectDetails";
                        command.Parameters.AddWithValue("@action", "Project");
                        command.Parameters.AddWithValue("@ProjectID", ProjectID);

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

        public async Task<DataTable> GetDepecryValueCommon(string type, int? ProjectID)
        {
            _actionName = "GetDepecryValueCommon()";
            return await Task.Run(async () =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (var command = _dbContext.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Web_Sp_SingleProjectDetails";
                        command.Parameters.AddWithValue("@action", type);
                        command.Parameters.AddWithValue("@ProjectID", ProjectID);

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
        //public async Task<DataTable> ExecuteQuery(string query)
        //{
        //    _actionName = "ExecuteQuery()";

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(query))
        //            throw new Exception("Query cannot be empty.");

        //        string sql = query.Trim().ToUpperInvariant();

        //        // Dangerous keywords block
        //        string[] blockedKeywords =
        //        {
        //            "DROP "
        //        };
        //        if (blockedKeywords.Any(x => sql.Contains(x)))
        //        {
        //            throw new Exception("Only Read Only Queries Allowed.");
        //        }

        //        DataTable dataTable = new DataTable();
        //        using (var command = _dbContext.CreateCommand())
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.CommandText = "USP_ProjectDetailFormSPs";

        //            command.Parameters.AddWithValue("@Query", query);
        //            command.Parameters.AddWithValue("@ProjectID", 0);

        //            _sqlQuery = command.GetSqlExecutableQuery();

        //            dataTable = await command.FillAsync_DataTable();
        //        }

        //        return dataTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorDesc = new ErrorDescription
        //        {
        //            Message = ex.Message,
        //            PageName = _pageName,
        //            ActionName = _actionName,
        //            SqlExecutableQuery = _sqlQuery
        //        };

        //        var errordetails = CommonFuncationHelper.MakeError(errorDesc);

        //        throw new Exception(errordetails, ex);
        //    }
        //}
        //public async Task<List<string>> GetStoredProcedures(string searchText)
        //{
        //    List<string> procedures = new();

        //    using (var command = _dbContext.CreateCommand())
        //    {
        //        command.CommandType = CommandType.Text;
        //        command.CommandText = @"SELECT Name FROM sys.procedures  WHERE Name LIKE '%' + @Search + '%' ORDER BY Name";
        //        command.Parameters.AddWithValue("@Search", searchText);

        //        var dt = await command.FillAsync_DataTable();

        //        procedures = dt.AsEnumerable().Select(x => x["Name"].ToString()).ToList();
        //    }

        //    return procedures;
        //}
        //public async Task<string> GetProcedureDefinition(string procedureName)
        //{
        //    using (var command = _dbContext.CreateCommand())
        //    {
        //        command.CommandType = CommandType.Text;

        //        command.CommandText = @" SELECT OBJECT_DEFINITION ( OBJECT_ID(@ProcedureName)) AS Definition";
        //        command.Parameters.AddWithValue("@ProcedureName",
        //                                        procedureName);

        //        var dt = await command.FillAsync_DataTable();

        //        if (dt.Rows.Count > 0)
        //        {
        //            return Convert.ToString(dt.Rows[0]["Definition"]);
        //        }

        //        return string.Empty;
        //    }
        //}
        //public async Task<DataTable> ExecuteQuery(string query)
        //{
        //    _actionName = "ExecuteQuery()";

        //    try
        //    {
        //        DataTable dataTable = new DataTable();
        //        using (var command = _dbContext.CreateCommand())
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.CommandText = "USP_ExecuteDynamicQuery";
        //            command.Parameters.AddWithValue("@Query", query);
        //            _sqlQuery = command.GetSqlExecutableQuery();
        //            dataTable = await command.FillAsync_DataTable();
        //        }

        //        return dataTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorDesc = new ErrorDescription
        //        {
        //            Message = ex.Message,
        //            PageName = _pageName,
        //            ActionName = _actionName,
        //            SqlExecutableQuery = _sqlQuery
        //        };

        //        var errordetails = CommonFuncationHelper.MakeError(errorDesc);

        //        throw new Exception(errordetails, ex);
        //    }
        //}
    }
}
