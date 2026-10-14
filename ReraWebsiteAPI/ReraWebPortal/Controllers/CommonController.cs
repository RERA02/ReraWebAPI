using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraModel.CommonModel;
using ReraModel.HomeModel;
using ReraWebPortal.Helpers;
using System.Data;
using System.Net;

namespace ReraWebPortal.Controllers
{
    [Route("api/web/[controller]")]
    [ApiController]
    //[Authorize(Policy = "ApiScope")] // JWT required to access this endpoint
   [ApiKeyAuth]
    public class CommonController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public CommonController(IUnitOfWork unitOfWork)
        {
                _unitOfWork = unitOfWork;
        }


        [HttpGet("GetComponentsNew")]
        public async Task<ApiResult<DataTable>> GetComponentsNew([FromQuery] int parentId, [FromQuery] int? typeId)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetComponentsNew(parentId, typeId);
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }

        [HttpGet("GetProjectType")]
        public async Task<ApiResult<DataTable>> GetProjectType()
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetProjectType();
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetQuarterList")]
        public async Task<ApiResult<DataTable>> GetQuarterList([FromQuery] int? FinYearId)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetQuarterList(FinYearId);
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetFinancialyearList")]
        public async Task<ApiResult<DataTable>> GetFinancialyearList()
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetFinancialyearList();
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetPromoterforddl")]
        public async Task<ApiResult<DataTable>> GetPromoterforddl()
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetPromoterforddl();
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetProjectList")]
        public async Task<ApiResult<DataTable>> GetProjectList([FromQuery] int? UserId)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetProjectList(UserId);
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetComponents")]
        public async Task<ApiResult<DataTable>> GetComponents([FromQuery] int parentId, [FromQuery] int? typeId)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetComponents(parentId, typeId);
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllAgentDocumentUploadList")]
        public async Task<ApiResult<DataTable>> GetAllAgentDocumentUploadList([FromBody] AgentDocumentModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.CommonFunctionRepository.GetAllAgentDocumentUploadList(body);
                if (data.Rows.Count > 0)
                {
                    result.Data = data;
                    result.State = EnumStatus.Success;
                    result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                }
                else
                {
                    result.State = EnumStatus.Warning;
                    result.Message = Constants.MSG_DATA_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                result.State = EnumStatus.Warning;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("CommonDetails")]
        public async Task<ApiResult<DataTable>> CommonDetails([FromBody] CommonModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.CommonFunctionRepository.CommonDetails(body);
                if (data.Rows.Count > 0)
                {
                    result.Data = data;
                    result.State = EnumStatus.Success;
                    result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                }
                else
                {
                    result.State = EnumStatus.Warning;
                    result.Message = Constants.MSG_DATA_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                result.State = EnumStatus.Warning;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetYear")]
        public async Task<ApiResult<DataTable>> GetYear()
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetYear();
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }

        [HttpGet("GetDeflauterlist")]
        public async Task<ApiResult<DataTable>> GetDeflauterlist([FromQuery] int parentId, [FromQuery] int? typeId)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetDeflauterlist(parentId, typeId);

                    if (!data.Columns.Contains("EncryptedProjectId"))
                    {
                        data.Columns.Add("EncryptedProjectId", typeof(string));
                    }
                    if (data.Rows.Count > 0)
                    {
                        foreach (DataRow row in data.Rows)
                        {
                            if (row["ProjectID"] != DBNull.Value)
                            {
                                string projectId = row["ProjectID"].ToString();

                                string encryptedId = CryptoHelper.Encrypt(projectId);

                                // Optional: make URL safe
                                encryptedId = encryptedId
                                    .Replace("+", "-")
                                    .Replace("/", "_")
                                    .Replace("=", "");

                                row["EncryptedProjectId"] = encryptedId;
                            }
                        }

                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetVistorCount")]
        public async Task<ApiResult<DataTable>> GetVistorCount([FromQuery] string Actionname)
        {
            string ip =  "";
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.CommonFunctionRepository.GetVistorCount(Actionname , ip);
                    if (data.Rows.Count > 0)
                    {
                        result.Data = data;
                        result.State = EnumStatus.Success;
                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Warning;
                        result.Message = Constants.MSG_DATA_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.State = EnumStatus.Warning;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }

    }
}
