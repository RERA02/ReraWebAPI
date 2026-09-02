using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraMobileApi.Api.Validators;
using ReraModel;
using ReraModel.CommonFunction;
using ReraModel.DashboardDepartmentModel;
using ReraModel.DashboardPromoterModel;
using ReraModel.HomeModel;
using ReraRepo.Repositories;
using ReraWebPortal.Middleware;
using System.Data;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReraWebPortal.Controllers
{
    [ApiController]
    [Route("api/Project")]
    //[Authorize(Policy = "ApiScope")]
    [ApiKeyAuth]
    public class ProjectController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public override string PageName => "UserLoginController";
        public override string ActionName { get; set; }
        public ProjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("TEST")]
        public async Task<ApiResultMobileApp<string>> TEST()
        {
            ActionName = "UserLogin(UserLoginModel model)";
            String MSG = string.Empty;
            var result = new ApiResultMobileApp<string>();
            try
            {
                if (!ModelState.IsValid)
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_VALIDATION_FAILED;
                    return result;
                }


                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_LOGIN_Successfully;
                result.Data = "TEST Success";
                return result;

            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }





        [HttpPost("DashboardPromoter")]
        [AllowAnonymous]
        public async Task<ApiResultMobileApp<DataTable>> DashboardPromoter(DashboardPromoterModel model)
        {
            ActionName = "DashboardPromoter(UserRoleModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {

                if (model.RoleId == 1)
                {
                    var DashboardPromoter = await _unitOfWork.UserRepository.DashboardPromoter(model);
                    //DashboardPromoter.Rows[1][0].ToString();

                    if (DashboardPromoter != null && DashboardPromoter.Rows.Count > 0)
                    {
                        result.Status = EnumStatusMA.Success;
                        result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                        result.Data = DashboardPromoter;
                    }
                    else
                    {
                        result.Status = EnumStatusMA.Failed;
                        result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                    }

                }

            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DownloadProjectDocuments")]
        public async Task<ApiResultMobileApp<DataSet>> DownloadProjectDocuments(DownloadProjectDocumentsModel model)
        {
            ActionName = "DownloadProjectDocuments(DownloadProjectDocumentsModel model)";
            String MSG = string.Empty;
            var result = new ApiResultMobileApp<DataSet>();

            try
            {

                var DownloadProjectDocuments = await _unitOfWork.UserRepository.DownloadProjectDocuments(model);
                if (DownloadProjectDocuments != null && DownloadProjectDocuments.Tables[0].Rows.Count > 0)
                {
                    DownloadProjectDocuments.Tables[0].TableName = "ProjectDocumentLegal";
                    DownloadProjectDocuments.Tables[0].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[0].Rows)
                    {
                        row["DocSection"] = "Project Legal Document";
                    }
                    DownloadProjectDocuments.Tables[1].TableName = "ProjectDocumentApproval";
                    DownloadProjectDocuments.Tables[1].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[1].Rows)
                    {
                        row["DocSection"] = "Project Approval Document";
                    }

                    DownloadProjectDocuments.Tables[2].TableName = "ProjectDocumentComman";
                    DownloadProjectDocuments.Tables[2].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[2].Rows)
                    {
                        row["DocSection"] = "ProjectComman Document";
                    }
                    DownloadProjectDocuments.Tables[3].TableName = "Commonareadocuments ";
                    DownloadProjectDocuments.Tables[3].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[3].Rows)
                    {
                        row["DocSection"] = "Common area documents";
                    }
                    DownloadProjectDocuments.Tables[4].TableName = "BuildingDocuments";
                    DownloadProjectDocuments.Tables[4].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[4].Rows)
                    {
                        row["DocSection"] = "Building Documents";
                    }

                    DownloadProjectDocuments.Tables[5].TableName = "AppartmentDocuments  ";
                    DownloadProjectDocuments.Tables[5].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[5].Rows)
                    {
                        row["DocSection"] = "Appartment Documents";
                    }

                    DownloadProjectDocuments.Tables[6].TableName = "PlotDocuments";
                    DownloadProjectDocuments.Tables[6].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[6].Rows)
                    {
                        row["DocSection"] = "Plot Documents";
                    }

                    DownloadProjectDocuments.Tables[7].TableName = "UploadedCertificatePath";
                    DownloadProjectDocuments.Tables[7].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[7].Rows)
                    {
                        row["DocSection"] = "Uploaded Certificate Path";
                    }

                    DownloadProjectDocuments.Tables[8].TableName = "form-E";
                    DownloadProjectDocuments.Tables[8].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[8].Rows)
                    {
                        row["DocSection"] = "form-E";
                    }
                    DownloadProjectDocuments.Tables[9].TableName = "form-F";
                    DownloadProjectDocuments.Tables[9].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[9].Rows)
                    {
                        row["DocSection"] = "form-F";
                    }
                    DownloadProjectDocuments.Tables[10].TableName = "ccandoc";
                    DownloadProjectDocuments.Tables[10].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[10].Rows)
                    {
                        row["DocSection"] = "cc and oc";
                    }
                    DownloadProjectDocuments.Tables[11].TableName = "ccandoc1";
                    DownloadProjectDocuments.Tables[11].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[11].Rows)
                    {
                        row["DocSection"] = "cc and oc 1";
                    }
                    DownloadProjectDocuments.Tables[12].TableName = "Ic";
                    DownloadProjectDocuments.Tables[12].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[12].Rows)
                    {
                        row["DocSection"] = "Ic";
                    }
                    DownloadProjectDocuments.Tables[13].TableName = "RWA";
                    DownloadProjectDocuments.Tables[13].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[13].Rows)
                    {
                        row["DocSection"] = "RWA";
                    }
                    DownloadProjectDocuments.Tables[14].TableName = "Form-D";
                    DownloadProjectDocuments.Tables[14].Columns.Add("DocSection", typeof(string));
                    foreach (DataRow row in DownloadProjectDocuments.Tables[14].Rows)
                    {
                        row["DocSection"] = "Form-D";
                    }
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = DownloadProjectDocuments;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }


            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DownloadProjectDocumentsNew")]
        public async Task<ApiResultMobileApp<DataTable>> DownloadProjectDocumentsNew(DownloadProjectDocumentsModel model)
        {
            ActionName = "DownloadProjectDocumentsNew(DownloadProjectDocumentsModel model)";
            String MSG = string.Empty;
            var result = new ApiResultMobileApp<DataTable>();

            try
            {

                var DownloadProjectDocuments = await _unitOfWork.UserRepository.DownloadProjectDocumentsNew(model);
                if (DownloadProjectDocuments != null && DownloadProjectDocuments.Rows.Count > 0)
                {

                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = DownloadProjectDocuments;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }


            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        //
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DashboardAgent")]
        public async Task<ApiResultMobileApp<DataTable>> DashboardAgent(DashboardAgentModel model)
        {
            ActionName = "DashboardAgent(DashboardAgentModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {


                var DashboardAgent = await _unitOfWork.UserRepository.DashboardAgent(model);


                if (DashboardAgent != null && DashboardAgent.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = DashboardAgent;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DashboardAllottee")]
        public async Task<ApiResultMobileApp<DataTable>> DashboardAllottee(DashboardAllotteeModel model)
        {
            ActionName = "DashboardAllottee(DashboardAllotteeModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {


                var DashboardAllottee = await _unitOfWork.UserRepository.DashboardAllottee(model);


                if (DashboardAllottee != null && DashboardAllottee.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = DashboardAllottee;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }
        //
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DashboardDepartment")]
        public async Task<ApiResultMobileApp<DataTable>> DashboardDepartment(DashboardDepartmentModel model)
        {
            ActionName = "DashboardDepartment(DashboardDepartmentModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {

                var DashboardDepartment = await _unitOfWork.UserRepository.DashboardDepartment(model);
                //DashboardPromoter.Rows[1][0].ToString();

                if (DashboardDepartment != null && DashboardDepartment.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = DashboardDepartment;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetModuleMaster")]
        public async Task<ApiResultMobileApp<DataTable>> GetModuleMaster(ModuleMastersModel model)
        {
            ActionName = "GetModuleMaster(ModuleMastersModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {

                var ModuleMaster = await _unitOfWork.UserRepository.GetModuleMaster(model);
                //DashboardPromoter.Rows[1][0].ToString();

                if (ModuleMaster != null && ModuleMaster.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ModuleMaster;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DepartmentPendencyData")]
        public async Task<ApiResultMobileApp<DataTable>> DepartmentPendencyData(DepartmentPendencyDataModel model)
        {
            ActionName = "DepartmentPendencyData(DepartmentPendencyDataModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {

                var DepartmentPendencyData = await _unitOfWork.UserRepository.DepartmentPendencyData(model);
                //DashboardPromoter.Rows[1][0].ToString();

                if (DepartmentPendencyData != null && DepartmentPendencyData.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = DepartmentPendencyData;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ProjectSearchData")]
        public async Task<ApiResultMobileApp<DataTable>> ProjectSearchData(ProjectSearchDataModel model)
        {
            ActionName = "ProjectSearchData(ProjectSearchDataModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {

                var ProjectSearchData = await _unitOfWork.UserRepository.ProjectSearchData(model);
                //DashboardPromoter.Rows[1][0].ToString();

                if (ProjectSearchData != null && ProjectSearchData.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ProjectSearchData;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }


        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllProjectMasterForDashboardGrid")]
        public async Task<ApiResultMobileApp<DataTable>> GetAllProjectMasterForDashboardGrid(PromoterDashboardGridModel model)
        {
            ActionName = "GetAllProjectMasterForDashboardGrid(UserRoleModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {


                var DashboardPromoter = await _unitOfWork.UserRepository.GetAllProjectMasterForDashboardGrid(model);
                if (DashboardPromoter != null && DashboardPromoter.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = DashboardPromoter;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }


            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GETAuditStatus")]
        public async Task<ApiResultMobileApp<DataTable>> GETAuditStatus(ProjectAuditModel model)
        {
            ActionName = "GETAuditStatus(ProjectAuditModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {


                var AuditStatus = await _unitOfWork.UserRepository.GETAuditStatus(model);
                if (AuditStatus != null && AuditStatus.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = AuditStatus;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }


            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GETModuleList")]
        public async Task<ApiResultMobileApp<DataTable>> GETModuleList(ModuleMasterModel model)
        {
            ActionName = "GETModuleList(ModuleMasterModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {


                var ModuleList = await _unitOfWork.UserRepository.GETModuleList(model);
                if (ModuleList != null && ModuleList.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ModuleList;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }


            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GETForwardTo")]
        public async Task<ApiResultMobileApp<DataTable>> GETForwardTo(ForwardToModel model)
        {
            ActionName = "GETForwardTo(ForwardToModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {


                var ModuleList = await _unitOfWork.UserRepository.GETForwardTo(model);
                if (ModuleList != null && ModuleList.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ModuleList;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }


            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveServiceRequest")]
        public async Task<ApiResultMobileApp<string>> SaveServiceRequest(Service_RequestModel model)
        {
            ActionName = "SaveServiceRequest(Service_RequestModel model)";
            string MSG = string.Empty;

            var result = new ApiResultMobileApp<string>();
            try
            {

                int moduleList = await _unitOfWork.UserRepository.SaveServiceRequest(model);

                if (moduleList > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = "Service Request Data Saved Successfully";
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;
                    result.Data = null;
                }
            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };

                await CreateErrorLog(nex, _unitOfWork);
            }

            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveLatLong")]
        public async Task<ApiResultMobileApp<string>> SaveLatLong(LatLongModel model)
        {
            ActionName = "SaveLatLong(LatLongModel model)";
            string MSG = string.Empty;

            var result = new ApiResultMobileApp<string>();
            try
            {

                int moduleList = await _unitOfWork.UserRepository.SaveLatLong(model);

                if (moduleList > 0)
                {

                    if (moduleList == 1) {
                        result.Status = EnumStatusMA.Success;
                        result.StatusApi = EnumStatusApi.Success;
                        result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                        result.Data = "Data Saved Successfully";
                    }
                    else
                    {
                        result.Status = EnumStatusMA.Success;
                        result.StatusApi = EnumStatusApi.Success;
                        result.Message = ConstentsMP.MSG_UPDATE_SUCCESS;
                        result.Data = "Updated successfully";
                    }
                   
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.StatusApi = EnumStatusApi.Error;
                    result.Message = ConstentsMP.MSG_ERROR_OCCURRED;
                    result.Data = "some error!";
                }
            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };

                await CreateErrorLog(nex, _unitOfWork);
            }

            return result;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveComment")]
        public async Task<ApiResultMobileApp<string>> SaveComment(SaveCommentModel model)
        {
            ActionName = "SaveComment(SaveCommentModel model)";
            string MSG = string.Empty;

            var result = new ApiResultMobileApp<string>();
            try
            {

                int moduleList = await _unitOfWork.UserRepository.SaveComment(model);

                if (moduleList > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = "Comments Saved Successfully";
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;
                    result.Data = null;
                }
            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };

                await CreateErrorLog(nex, _unitOfWork);
            }

            return result;
        }


        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Summary")]
        public async Task<ApiResultMobileApp<DataSet>> Summary(ProjectSummaryModel model)
        {
            ActionName = "Summary(UserRoleModel model)";
            String MSG = string.Empty;
            var result = new ApiResultMobileApp<DataSet>();

            try
            {

                var ProjectSummary = await _unitOfWork.UserRepository.FillProjectSummaryMasterModel(model);
                if (ProjectSummary != null && ProjectSummary.Tables[0].Rows.Count > 0)
                {
                    ProjectSummary.Tables[0].TableName = "Project";
                    ProjectSummary.Tables[1].TableName = "Project_Summary_Modification";
                    ProjectSummary.Tables[2].TableName = "ProjectEndPoints";
                    ProjectSummary.Tables[3].TableName = "ProjectMOUHolderDetails";
                    ProjectSummary.Tables[4].TableName = "ProjectLitigations";
                    ProjectSummary.Tables[5].TableName = "ProjectCommanArea";
                    ProjectSummary.Tables[6].TableName = "ProjectDevelopementWork";
                    ProjectSummary.Tables[7].TableName = "ProjectCommonArea";
                    ProjectSummary.Tables[8].TableName = "ProjectCommanAreaByTypeIdProjectIdOldData";
                    ProjectSummary.Tables[9].TableName = "ProjectCommanAreaByTypeIdProjectIdNewData";
                    ProjectSummary.Tables[10].TableName = "ProjectCommanAreaByTypeIdProjectId";
                    ProjectSummary.Tables[11].TableName = "CommonAreaItemsCharged";
                    ProjectSummary.Tables[12].TableName = "PlotDetails";
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ProjectSummary;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }


            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }


        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ProjectsByPromoterId")]
        public async Task<ApiResultMobileApp<DataTable>> ProjectsByPromoterId(ProjectSearchPromoterModel model)
        {
            ActionName = "ProjectsByPromoterId(ProjectSearchPromoterModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {

                var ProjectSearchData = await _unitOfWork.UserRepository.ProjectsByPromoterId(model);
                //DashboardPromoter.Rows[1][0].ToString();

                if (ProjectSearchData != null && ProjectSearchData.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ProjectSearchData;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDetailsbyProjectId")]
        [AllowAnonymous]
        public async Task<ApiResultMobileApp<DataTable>> GetDetailsbyProjectId(string REFID_id)
        {
            ActionName = "GetDetailsbyProjectId(string REFID_id)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {
                //string[] chkREFID_id = REFID_id.Split("!*!");
                var ProjectSearchData = await _unitOfWork.UserRepository.GetDetailsbyProjectId(REFID_id);
                //DashboardPromoter.Rows[1][0].ToString();

                if (ProjectSearchData != null && ProjectSearchData.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ProjectSearchData;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }


        [HttpPost("UploadDocumentold"), DisableRequestSizeLimit]
        [ActionName("UploadDocumentold")]
        public async Task<ApiResultMobileApp<List<UploadFileWithPathDataModel>>> UploadDocumentold([FromForm] UploadFileModel model)
        {
            ActionName = "UploadDocument([FromForm] UploadFileModel model)";
            return await Task.Run(async () =>
            {
                var result = new ApiResultMobileApp<List<UploadFileWithPathDataModel>>();
                try
                {
                    //required file
                    if (model.file == null || model.file.Length == 0)
                    {
                        result.Status = EnumStatusMA.Error;
                        result.ErrorMessage = Constants.MSG_INVALID_REQUEST;
                        return result;
                    }

                    //type(extention)
                    if (!string.IsNullOrWhiteSpace(model.FileExtention))
                    {
                        var fileExtensions = Path.GetExtension(model.file.FileName).ToLower();
                        if (model.FileExtention?.Split().Any(x => x.ToLower() == fileExtensions) == true)
                        {
                            result.Status = EnumStatusMA.Error;
                            result.ErrorMessage = $"Invalid extension, allowed only {string.Join(", ", model.FileExtention)}";
                            return result;
                        }
                    }

                    //Special Char
                    var (isValid, message) = FileValidationHelper.IsValidFileName(model.file.FileName);
                    if (!isValid)
                    {
                        result.Status = EnumStatusMA.Error;
                        result.ErrorMessage = $"{message}";
                        return result;
                    }

                    //min size
                    if (!string.IsNullOrWhiteSpace(model.MinFileSize) && int.TryParse(model.MinFileSize.Replace("mb", string.Empty).Replace("kb", string.Empty), out int acceptMinFileSize) == true)
                    {
                        decimal fileSize = 0;
                        if (model.MinFileSize.ToLower().EndsWith("mb"))
                        {
                            fileSize = Math.Round((decimal)model.file.Length / 1024 / 1024);
                        }
                        else if (model.MinFileSize.ToLower().EndsWith("kb"))
                        {
                            fileSize = Math.Round((decimal)model.file.Length / 1024);
                        }
                        if (fileSize < acceptMinFileSize)
                        {
                            result.Status = EnumStatusMA.Error;
                            result.ErrorMessage = $"Invalid file size, Min allowed only {model.MinFileSize}";
                            return result;
                        }
                    }

                    //max size
                    if (!string.IsNullOrWhiteSpace(model.MaxFileSize) && int.TryParse(model.MaxFileSize.Replace("mb", string.Empty).Replace("kb", string.Empty), out int acceptMaxFileSize) == true)
                    {
                        decimal fileSize = 0;
                        if (model.MaxFileSize.ToLower().EndsWith("mb"))
                        {
                            fileSize = Math.Round((decimal)model.file.Length / 1024 / 1024);
                        }
                        else if (model.MaxFileSize.ToLower().EndsWith("kb"))
                        {
                            fileSize = Math.Round((decimal)model.file.Length / 1024);
                        }
                        if (fileSize > acceptMaxFileSize)
                        {
                            result.Status = EnumStatusMA.Error;
                            result.ErrorMessage = $"Invalid file size, Max allowed only {model.MaxFileSize}";
                            return result;
                        }
                    }


                    //set and create the folder
                    var uploadFolder = Path.Combine(ConfigurationHelper.StaticFileRootPath,  model.FolderName ?? "");
                    if (!System.IO.Directory.Exists(uploadFolder))
                    {
                        System.IO.Directory.CreateDirectory(uploadFolder);
                    }

                    //read the file name of the received file
                    var OrgfileName = ContentDispositionHeaderValue.Parse(model.file.ContentDisposition).FileName.Trim('"');

                    // save the file on Path
                    var FileName = $"{System.DateTime.Now.ToString("MMMddyyyyhhmmssffffff")}{Path.GetExtension(OrgfileName)}";
                    var finalPathSave = Path.Combine(uploadFolder, FileName);

                    //model
                    List<UploadFileWithPathDataModel> uploadFileDataModels = new List<UploadFileWithPathDataModel>();
                    UploadFileWithPathDataModel uploadFileDataModel = new UploadFileWithPathDataModel();
                    uploadFileDataModel.Dis_FileName = OrgfileName;
                    uploadFileDataModel.FileName = FileName;
                    uploadFileDataModel.FilePath = Path.Combine(model.FolderName ?? "", FileName);
                    uploadFileDataModel.FolderName = model.FolderName ?? "";
                    uploadFileDataModels.Add(uploadFileDataModel);
                    result.Data = uploadFileDataModels;

                    //physical save
                    var finalPath = Path.Combine(uploadFolder, FileName);
                    using (var fileStream = new FileStream(finalPath, FileMode.Create))
                    {
                        model.file.CopyTo(fileStream);
                    }

                    //resize
                    using (FileStream pngStream = new FileStream(finalPath, FileMode.OpenOrCreate))
                    {
                        CommonFuncationHelper.ResizeImage(pngStream, finalPathSave, 0, 0);
                    }

                    //success
                    result.Status = EnumStatusMA.Success;
                    result.Message = Constants.MSG_SAVE_SUCCESS;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Dispose();
                    result.Status = EnumStatusMA.Error;
                    result.Message = Constants.MSG_ERROR_OCCURRED;
                    result.ErrorMessage = ex.Message;

                    // Log the error
                    var nex = new NewException
                    {
                        PageName = PageName,
                        ActionName = ActionName,
                        Ex = ex
                    };
                    await CreateErrorLog(nex, _unitOfWork);
                }

                return result;
            });
        }



        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetITServiceRequest")]
        public async Task<ApiResultMobileApp<DataTable>> GetITServiceRequest(GetITServiceRequestModel model)
        {
            ActionName = "GetITServiceRequest(GetITServiceRequestModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {

                var ProjectSearchData = await _unitOfWork.UserRepository.GetITServiceRequest(model);
                //DashboardPromoter.Rows[1][0].ToString();

                if (ProjectSearchData != null && ProjectSearchData.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ProjectSearchData;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }



        //        [HttpPost("UploadDocument"), DisableRequestSizeLimit]
        //        public async Task<ApiResultMobileApp<List<UploadFileWithPathDataModel>>> UploadDocument([FromForm] UploadFileModel model)
        //        {
        //            var result = new ApiResultMobileApp<List<UploadFileWithPathDataModel>>();

        //            try
        //            {

        //                if (model.file == null || model.file.Length == 0)
        //                {
        //                    result.Status = EnumStatusMA.Error;
        //                    result.ErrorMessage = Constants.MSG_INVALID_REQUEST;
        //                    return result;
        //                }


        //                if (!string.IsNullOrWhiteSpace(model.FileExtention))
        //                {
        //                    var allowedExtensions = model.FileExtention
        //                                                .Split(',')
        //                                                .Select(x => x.Trim().ToLower())
        //                                                .ToList();

        //                    var fileExtension = Path.GetExtension(model.file.FileName).ToLower();

        //                    if (!allowedExtensions.Contains(fileExtension))
        //                    {
        //                        result.Status = EnumStatusMA.Error;
        //                        result.ErrorMessage = $"Invalid extension, allowed only {string.Join(", ", allowedExtensions)}";
        //                        return result;
        //                    }
        //                }


        //                var (isValid, message) = FileValidationHelper.IsValidFileName(model.file.FileName);
        //                if (!isValid)
        //                {
        //                    result.Status = EnumStatusMA.Error;
        //                    result.ErrorMessage = message;
        //                    return result;
        //                }


        //                if (!string.IsNullOrWhiteSpace(model.MinFileSize))
        //                {
        //                    if (TryParseFileSize(model.MinFileSize, out long minBytes))
        //                    {
        //                        if (model.file.Length < minBytes)
        //                        {
        //                            result.Status = EnumStatusMA.Error;
        //                            result.ErrorMessage = $"Invalid file size, Min allowed only {model.MinFileSize}";
        //                            return result;
        //                        }
        //                    }
        //                }


        //                if (!string.IsNullOrWhiteSpace(model.MaxFileSize))
        //                {
        //                    if (TryParseFileSize(model.MaxFileSize, out long maxBytes))
        //                    {
        //                        if (model.file.Length > maxBytes)
        //                        {
        //                            result.Status = EnumStatusMA.Error;
        //                            result.ErrorMessage = $"Invalid file size, Max allowed only {model.MaxFileSize}";
        //                            return result;
        //                        }
        //                    }
        //                }


        //                var uploadFolder = Path.Combine(ConfigurationHelper.StaticFileRootPath, model.FolderName ?? "");

        //                if (!Directory.Exists(uploadFolder))
        //                {
        //                    Directory.CreateDirectory(uploadFolder);
        //                }


        //                var originalFileName = Path.GetFileName(model.file.FileName);


        //                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        //                var finalPath = Path.Combine(uploadFolder, fileName);


        //                using (var stream = new FileStream(finalPath, FileMode.Create))
        //                {
        //                    await model.file.CopyToAsync(stream);
        //                }


        //                try
        //                {
        //                    using (var stream = new FileStream(finalPath, FileMode.Open, FileAccess.ReadWrite))
        //                    {
        //                        CommonFuncationHelper.ResizeImage(stream, finalPath, 0, 0);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    result.Status = EnumStatusMA.Error;
        //                    result.Message = Constants.MSG_ERROR_OCCURRED;
        //                    result.ErrorMessage = ex.Message;

        //                    var nex = new NewException
        //                    {
        //                        PageName = "UploadDocument",
        //                        ActionName = "UploadDocument API",
        //                        Ex = ex
        //                    };

        //                    await CreateErrorLog(nex, _unitOfWork);
        //                }


        //                var uploadFileDataModels = new List<UploadFileWithPathDataModel>
        //{
        //    new UploadFileWithPathDataModel
        //    {
        //        Dis_FileName = originalFileName,
        //        FileName = fileName,
        //        FilePath = Path.Combine(model.FolderName ?? "", fileName),
        //        FolderName = model.FolderName ?? ""
        //    }
        //};

        //                result.Data = uploadFileDataModels;
        //                result.Status = EnumStatusMA.Success;
        //                result.Message = Constants.MSG_SAVE_SUCCESS;
        //            }
        //            catch (Exception ex)
        //            {
        //                result.Status = EnumStatusMA.Error;
        //                result.Message = Constants.MSG_ERROR_OCCURRED;
        //                result.ErrorMessage = ex.Message;

        //                var nex = new NewException
        //                {
        //                    PageName = "UploadDocument",
        //                    ActionName = "UploadDocument API",
        //                    Ex = ex
        //                };

        //                await CreateErrorLog(nex, _unitOfWork);
        //            }

        //            return result;
        //        }
        //        private bool TryParseFileSize(string size, out long bytes)
        //        {
        //            bytes = 0;

        //            if (string.IsNullOrWhiteSpace(size))
        //                return false;

        //            size = size.Trim().ToLower();

        //            if (size.EndsWith("mb"))
        //            {
        //                if (int.TryParse(size.Replace("mb", ""), out int mb))
        //                {
        //                    bytes = mb * 1024L * 1024L;
        //                    return true;
        //                }
        //            }
        //            else if (size.EndsWith("kb"))
        //            {
        //                if (int.TryParse(size.Replace("kb", ""), out int kb))
        //                {
        //                    bytes = kb * 1024L;
        //                    return true;
        //                }
        //            }

        //            return false;
        //        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("UploadDocument"), DisableRequestSizeLimit]
        [ActionName("UploadDocument")]
        public async Task<ApiResultMobileApp<List<UploadFileWithPathDataModel>>> UploadDocument([FromForm] UploadFileModel model)
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            ActionName = "UploadDocument([FromForm] UploadFileModel model)";
            return await Task.Run(async () =>
            {
                var result = new ApiResultMobileApp<List<UploadFileWithPathDataModel>>();
                try
                {
                    //required file
                    if (model.file == null || model.file.Length == 0)
                    {
                        result.Status = EnumStatusMA.Error;
                        result.Message = Constants.MSG_INVALID_REQUEST;
                        return result;
                    }



                    //Special Char
                    var (isValid, message) = FileValidationHelper.IsValidFileName(model.file.FileName);
                    if (!isValid)
                    {
                        result.Status = EnumStatusMA.Error;
                        result.Message = $"{message}";
                        return result;
                    }





                    //set and create the folder
                    var uploadFolder = Path.Combine(ConfigurationHelper.StaticFileRootPath, model.FolderName ?? "");
                    if (!System.IO.Directory.Exists(uploadFolder))
                    {
                        System.IO.Directory.CreateDirectory(uploadFolder);
                    }

                    //read the file name of the received file
                    var OrgfileName = ContentDispositionHeaderValue.Parse(model.file.ContentDisposition).FileName.Trim('"');

                    // save the file on Path
                    var FileName = $"{System.DateTime.Now.ToString("MMMddyyyyhhmmssffffff")}{Path.GetExtension(OrgfileName)}";
                    var finalPathSave = Path.Combine(uploadFolder, FileName);

                    //model
                    var finalPath = Path.Combine(uploadFolder, FileName);
                    List<UploadFileWithPathDataModel> uploadFileDataModels = new List<UploadFileWithPathDataModel>();
                    UploadFileWithPathDataModel uploadFileDataModel = new UploadFileWithPathDataModel();
                    uploadFileDataModel.Dis_FileName = OrgfileName;
                    uploadFileDataModel.FileName = FileName;

                    //uploadFileDataModel.FilePath = finalPath;//Path.Combine(model.FolderName ?? "", FileName);
                    var relativePath = Path.Combine(
                                                    "StaticFiles",
                                                    model.FolderName ?? "",
                                                    FileName
                                                ).Replace("\\", "/");

                    uploadFileDataModel.FilePath = $"{baseUrl}/{relativePath}";
                    uploadFileDataModel.FolderName = model.FolderName ?? "";
                    uploadFileDataModels.Add(uploadFileDataModel);
                    result.Data = uploadFileDataModels;

                    //physical save

                    using (var fileStream = new FileStream(finalPath, FileMode.Create))
                    {
                        model.file.CopyTo(fileStream);
                    }

                    //resize
                    using (FileStream pngStream = new FileStream(finalPath, FileMode.OpenOrCreate))
                    {
                        CommonFuncationHelper.ResizeImage(pngStream, finalPathSave, 0, 0);
                    }

                    //success
                    result.Status = EnumStatusMA.Success;
                    result.Message = Constants.MSG_SAVE_SUCCESS;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Dispose();
                    result.Status = EnumStatusMA.Error;
                    result.Message = Constants.MSG_ERROR_OCCURRED;
                    //result.err = ex.Message;

                    // Log the error
                    var nex = new NewException
                    {
                        PageName = PageName,
                        ActionName = ActionName,
                        Ex = ex
                    };
                    await CreateErrorLog(nex, _unitOfWork);
                }

                return result;
            });
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("UpdateQPR_Payment_DTLS")]
        public async Task<ApiResultMobileApp<string>> UpdateQPR_Payment_DTLS(LatLongModelUpdateQPR model)
        {
            ActionName = "UpdateQPR_Payment_DTLS(LatLongModelUpdateQPR model)";
            string MSG = string.Empty;

            var result = new ApiResultMobileApp<string>();
            try
            {

                int moduleList = await _unitOfWork.UserRepository.UpdateQPR_Payment_DTLS(model);

                if (moduleList > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.StatusApi = EnumStatusApi.Success;
                    result.Message = ConstentsMP.MSG_DataSave_Success;
                    result.Data = "Data Saved Successfully";
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.StatusApi = EnumStatusApi.Error;
                    result.Message = ConstentsMP.MSG_ERROR_OCCURRED;
                    result.Data = "Some error!";
                }
            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.StatusApi = EnumStatusApi.Error;
                result.Message = ConstentsMP.MSG_APPService;

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };

                await CreateErrorLog(nex, _unitOfWork);
            }

            return result;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpPost("UpdateQPR_Payment_DTLSNew")]
        //[Consumes("multipart/form-data")]
        //public async Task<ApiResultMobileApp<string>> UpdateQPR_Payment_DTLSNew(LatLongModelUpdateQPR model)
        [HttpPost("UpdateQPR_Payment_DTLSNew")]
        public async Task<ApiResultMobileApp<string>> UpdateQPR_Payment_DTLSNew(LatLongModelUpdateQPR model)
        {
            ActionName = "UpdateQPR_Payment_DTLSNew(LatLongModelUpdateQPR model)";
            string MSG = string.Empty;

            var result = new ApiResultMobileApp<string>();
            try
            {
               

                int moduleList = await _unitOfWork.UserRepository.UpdateQPR_Payment_DTLSNew(model);

                if (moduleList > 0)
                {

                    if(moduleList == 1)
                    {
                        result.Status = EnumStatusMA.Success;
                        result.StatusApi = EnumStatusApi.Success;
                        result.Message = ConstentsMP.MSG_DataSave_Success;
                        result.Data = "Data Saved Successfully";
                    }
                    else
                    {
                        result.Status = EnumStatusMA.Success;
                        result.StatusApi = EnumStatusApi.Success;
                        result.Message = ConstentsMP.MSG_UPDATE_SUCCESS;
                        result.Data = "Updated successfully.";

                    }
                   
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.StatusApi = EnumStatusApi.Error;
                    result.Message = ConstentsMP.MSG_ERROR_OCCURRED;
                    result.Data = "Some error!";
                }
            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.StatusApi = EnumStatusApi.Error;
                result.Message = ConstentsMP.MSG_APPService;

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };

                await CreateErrorLog(nex, _unitOfWork);
            }

            return result;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetProjectEndPointsProjectIdWise")]
        
        public async Task<ApiResultMobileApp<DataTable>> GetProjectEndPointsProjectIdWise(GetProjectEndPointsProjectIdWiseModel  model)
        {
            ActionName = "GetProjectEndPointsProjectIdWise(string REFID_id)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {
                
                var ProjectSearchData = await _unitOfWork.UserRepository.GetProjectEndPointsProjectIdWise(model);
              

                if (ProjectSearchData != null && ProjectSearchData.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = ProjectSearchData;
                }
                else
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_DATA_NOT_FOUND;

                }



            }
            catch (Exception ex)
            {
                result.Status = EnumStatusMA.Failed;
                result.Message = ConstentsMP.MSG_APPService;
                // write error log
                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex,
                };
                await CreateErrorLog(nex, _unitOfWork);
            }
            return result;
        }
       
        
        #region call backgroung service post data third party url 
        public class ReraBackgroundService : BackgroundService
        {
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly HttpClient _httpClient;
            private readonly IConfiguration _configuration;

            public ReraBackgroundService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
            {
                _scopeFactory = scopeFactory;
                _configuration = configuration;
                //var apikey = _configuration["ApiKeySettings:ApiKey"];
                //var Header = _configuration["ApiKeySettings:Header"];
                //var BaseUrl = _configuration["ApiKeySettings:BaseUrl"];
                var apikey = "MySuperSecretApiKey_123";
                var Header = "X-API-KEY";
                var BaseUrl = "https://ReraAPI.rajasthan.gov.in/";
                //var BaseUrl = "http://localhost:4558/";
                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri(BaseUrl);
                _httpClient.DefaultRequestHeaders.Add(Header, apikey);
            }
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        try
                        {
                            // ✅ STEP 1: Fetch data from DB
                            var ReraProgressReport = await unitOfWork.UserRepository.GET_ReraProgressReport();

                            if (ReraProgressReport == null || ReraProgressReport.Rows.Count == 0)
                            {
                                Console.WriteLine("❌ No Data Found");
                            }
                            else
                            {
                                DataTable dt = ReraProgressReport;
                                var row = dt.Rows[0];

                                // ✅ STEP 2: Prepare Request Body
                                var requestBody = new
                                {
                                    total_projects_registered = int.TryParse(row["total_projects_registered"]?.ToString(), out int t1) ? t1 : 0,
                                    residential_projects_registered = int.TryParse(row["residential_projects_registered"]?.ToString(), out int t2) ? t2 : 0,
                                    dwelling_units = int.TryParse(row["dwelling_units"]?.ToString(), out int t3) ? t3 : 0,
                                    carpet_area_lakh_sqmtr = int.TryParse(row["carpet_area_lakh_sqmtr"]?.ToString(), out int t4) ? t4 : 0,
                                    agents_registered = int.TryParse(row["agents_registered"]?.ToString(), out int t5) ? t5 : 0,
                                    cases_filed = int.TryParse(row["cases_filed"]?.ToString(), out int t6) ? t6 : 0,
                                    cases_disposed = int.TryParse(row["cases_disposed"]?.ToString(), out int t7) ? t7 : 0,
                                    state_code = int.TryParse(row["state_code"]?.ToString(), out int t8) ? t8 : 0,
                                    date = row["date"]?.ToString()
                                };

                                DataTable apiResponseTable = new DataTable();

                                // ✅ STEP 3: Call External API
                                var Responseval = await unitOfWork.UserRepository.PostreraProgressReport(
                                            requestBody.total_projects_registered,
                                            requestBody.residential_projects_registered,
                                            requestBody.dwelling_units,
                                            requestBody.carpet_area_lakh_sqmtr,
                                            requestBody.agents_registered,
                                            requestBody.cases_filed,
                                            requestBody.cases_disposed,
                                            requestBody.state_code,
                                            requestBody.date
                                            

                                        );
                                if (Responseval >0)
                                {
                                    using (HttpClient client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri("https://dashboard.pmay-urban.gov.in/");

                                        client.DefaultRequestHeaders.Add(
                                            "Authorization",
                                            "Claim a55d444409a03feb7d07e9397b3fce1c968cedcf8b288f13859ca5cf06ad2258"
                                        );

                                        var jsonContent = new StringContent(
                                            JsonConvert.SerializeObject(requestBody),
                                            Encoding.UTF8,
                                            "application/json"
                                        );
                                       
                                        var response = await client.PostAsync("v1/api/reraProgressReport", jsonContent);
                                        string responseBody = await response.Content.ReadAsStringAsync();
                                        var ReraProgressResmsg1 = await unitOfWork.UserRepository.updateResponseStatus(Responseval, Convert.ToString(response));

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var ResponseBd = "Data inserted in rera_progress_report_new_api Manual";
                                            var ReraProgressResmsg = await unitOfWork.UserRepository.updateResponseStatus(Responseval, Convert.ToString(ResponseBd));
                                            var jsonString = await response.Content.ReadAsStringAsync();

                                            if (!string.IsNullOrEmpty(jsonString))
                                            {
                                                var jsonObj = JObject.Parse(jsonString);
                                                var dataArray = jsonObj["Data"];

                                                if (dataArray != null && dataArray.HasValues)
                                                {
                                                    apiResponseTable = JsonConvert.DeserializeObject<DataTable>(dataArray.ToString());
                                                }
                                            }

                                            Console.WriteLine("✅ API Call Success & Data Inserted");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"❌ API Error: {response.StatusCode}");
                                        }
                                    }
                                }
                             }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("❌ Exception: " + ex.Message);
                        }
                    }

                    // ✅ Run every 8 HR
                    //await Task.Delay(60000, stoppingToken);//1Minutes
                    //await Task.Delay(3600000, stoppingToken);//1HR
                    await Task.Delay(28800000, stoppingToken);//8HR
                    //await Task.Delay(86400000, stoppingToken);//24HR
                    //await Task.Delay(604800000, stoppingToken);//7 days
                }
            }
            #endregion
        }


    }
}
