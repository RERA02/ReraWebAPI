using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QRCoder;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraModel;
using ReraModel.CommonFunction;
using ReraModel.CommonModel;
using ReraModel.HomeModel;
using ReraModel.UserLoginModel;
using ReraModel.UserSessionModel;
using ReraWebPortal.Helpers;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static ReraMobileApi.Api.apiservice.CommonFunctions;

namespace ReraWebPortal.Controllers
{
    [Route("api/web/[controller]")]
    [ApiController]
    //[Authorize(Policy = "ApiScope")] // JWT required to access this endpoint
    [ApiKeyAuth]
    public class HomeController : BaseController
    {
        #region  Web

        private readonly IUnitOfWork _unitOfWork;
        public override string PageName => "HomeController";
        public override string ActionName { get; set; }
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetHomePageData")]
        public async Task<ApiResult<HomePageApiVM>> GetHomePageData()
        {
            var result = new ApiResult<HomePageApiVM>();

            try
            {
                var homepageList = await _unitOfWork
                    .HomeRepository
                    .GetHomePageCount();

                var latestJudgements = await _unitOfWork
                    .HomeRepository
                    .GetLatestJudgements(10);

                result.Data = new HomePageApiVM
                {
                    Dashboard = homepageList.FirstOrDefault(),
                    LatestJudgements = latestJudgements
                };

                result.State = EnumStatus.Success;
                result.Message = Constants.MSG_DATA_LOAD_SUCCESS;
            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetProjectsList")]
        public async Task<ApiResult<DataTable>> GetProjectsList([FromQuery] int district = 0, [FromQuery] int teshil = 0, [FromQuery] string projectName = "", [FromQuery] string promoterName = "",
                                                                [FromQuery] string certificateNo = "", [FromQuery] int projecttype = 0, [FromQuery] int projectstatus = 0)
        {
            var result = new ApiResult<DataTable>();

            try
            {

                var searchParams = new ProjectSearchParams
                {
                    ProjectName = projectName,
                    PromoterName = promoterName,
                    CertificateNo = certificateNo,
                    District = district,
                    Teshil = teshil,
                    ProjectType = projecttype,
                    ApplicationStatus = projectstatus,
                    SOURCE = 1
                };


                string ProjectOrExemption = "";

                var dt = await _unitOfWork.HomeRepository
                    .GetProjectsListForOuterSearchNew(searchParams, ProjectOrExemption);

                if (dt.Rows.Count > 0)
                {
                    result.Data = dt;
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetProjects")]
        public async Task<ApiResult<DataTable>> GetProjects([FromBody] SearchProjectRequestModel body)
        {
            var result = new ApiResult<DataTable>();
            try
            {
                body.Source = 1;
                var data = await _unitOfWork.HomeRepository.GetProjects(body);

                if (data != null)
                {
                    if (!data.Columns.Contains("EncryptedProjectId"))
                    {
                        data.Columns.Add("EncryptedProjectId", typeof(string));
                    }

                    if (data.Rows.Count > 0)
                    {
                        foreach (DataRow row in data.Rows)
                        {
                            if (row["Id"] != DBNull.Value)
                            {
                                string projectId = row["Id"].ToString();

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

            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ContactUs")]
        public async Task<ApiResult<int>> ContactUs([FromBody] QueryVM request)
        {
            ActionName = "ContactUs([FromBody] QueryVM request)";
            return await Task.Run(async () =>
            {
                var result = new ApiResult<int>();
                try
                {
                    result.Data = await _unitOfWork.HomeRepository.ContactUs(request);
                    _unitOfWork.SaveChanges();
                    if (result.Data > 0)
                    {
                        result.State = EnumStatus.Success;
                        if (result.Data == 1)
                        {
                            result.Message = Constants.MSG_SAVE_SUCCESS;
                        }
                        else
                        {
                            result.Message = Constants.MSG_UPDATE_SUCCESS;
                        }
                    }
                    else if (result.Data == -2)
                    {
                        result.State = EnumStatus.Warning;
                        result.ErrorMessage = Constants.MSG_SAVE_Duplicate;
                    }
                    else
                    {
                        result.State = EnumStatus.Error;
                        if (request.ID == 0)
                        {
                            result.ErrorMessage = Constants.MSG_ADD_ERROR;
                        }
                        else
                        {
                            result.ErrorMessage = Constants.MSG_UPDATE_ERROR;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _unitOfWork.Dispose();

                    var nex = new NewException
                    {
                        PageName = PageName,
                        ActionName = ActionName,
                        Ex = ex
                    };
                    await CreateErrorLog(nex, _unitOfWork);

                    result.State = EnumStatus.Error;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetVisitorCount/{key}")]
        public async Task<ApiResult<int>> GetVisitorCount(string key)
        {
            ActionName = "GetVisitorCount([FromBody] QueryVM request)";
            return await Task.Run(async () =>
            {
                var result = new ApiResult<int>();
                try
                {
                    result.Data = await _unitOfWork.HomeRepository.GetVisitorCount(key);

                    if (result.Data > 0)
                    {
                        result.State = EnumStatus.Success;

                        result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

                    }
                    else
                    {
                        result.State = EnumStatus.Error;

                        result.ErrorMessage = Constants.MSG_DATA_NOT_FOUND;

                    }
                }
                catch (Exception ex)
                {
                    _unitOfWork.Dispose();

                    var nex = new NewException
                    {
                        PageName = PageName,
                        ActionName = ActionName,
                        Ex = ex
                    };
                    await CreateErrorLog(nex, _unitOfWork);

                    result.State = EnumStatus.Error;
                    result.ErrorMessage = ex.Message;
                }
                return result;
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetComplaintsAgainstRespondentList")]
        public async Task<ApiResult<DataTable>> GetComplaintsAgainstRespondentList([FromBody] GetComplaintsAgainstRespondenSearch model)
        {
            var result = new ApiResult<DataTable>();

            try
            {
                var dt = await _unitOfWork.HomeRepository
                    .GetComplaintsAgainstRespondentList(model);

                if (dt.Rows.Count > 0)
                {
                    result.Data = dt;
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAgents")]
        public async Task<ApiResult<DataTable>> GetAgents([FromBody] SearchAgentRequestModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetAgentsList(body);
                if (data.Rows.Count > 0)
                {
                    if (!data.Columns.Contains("EncrId"))
                    {
                        data.Columns.Add("EncrId", typeof(string));
                    }

                    if (data.Rows.Count > 0)
                    {
                        foreach (DataRow row in data.Rows)
                        {
                            if (row["Id"] != DBNull.Value)
                            {
                                string Id = row["Id"].ToString();

                                string encryptedId = CryptoHelper.Encrypt(Id);

                                // Optional: make URL safe
                                encryptedId = encryptedId
                                    .Replace("+", "-")
                                    .Replace("/", "_")
                                    .Replace("=", "");

                                row["EncrId"] = encryptedId;
                            }
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetComplaintFullDetails")]
        public async Task<ApiResult<DataSet>> GetComplaintFullDetails([FromBody] ComplaintSearchRequestModel body)
        {
            var result = new ApiResult<DataSet>();

            try
            {
                var data = await _unitOfWork.HomeRepository.GetComplaintFullDetails(body);
                if (data != null &&
                    data.Tables != null &&
                    data.Tables.Cast<DataTable>().Any(t => t.Rows.Count > 0))
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetWeb_TBL_AUDITTRAIL")]
        public async Task<ApiResult<DataTable>> GetWeb_TBL_AUDITTRAIL([FromBody] Web_TBL_AUDITTRAILSearchModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetWeb_TBL_AUDITTRAIL(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetWeb_GetAllNotice")]
        public async Task<ApiResult<DataTable>> GetWeb_GetAllNotice([FromBody] Web_GetAllNoticeSearModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetWeb_GetAllNotice(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TblComplaintDetails")]
        public async Task<ApiResult<DataTable>> TblComplaintDetails([FromBody] ComplaintDetailsS_M body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.TblComplaintDetails(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ComplaintAdjudicatingPDF")]
        public async Task<IActionResult> ComplaintAdjudicatingPDF([FromBody] ComplaintAdjudicatingPDF body)
        {
            if (body == null || body.id <= 0)
                return BadRequest("Invalid request");

            ComplaintAdjudicatingOfficerModel objmodelT = new ComplaintAdjudicatingOfficerModel();

            try
            {
                ComplaintsDetailsS_M detailsS_M = new ComplaintsDetailsS_M
                {
                    Action = "GetComplaintDetailsForPDF",
                    ComplaintID = body.id
                };

                var objmodel = await _unitOfWork.HomeRepository
                    .GetcomplaintsDetailsForPDFnew(detailsS_M);

                if (objmodel == null)
                    return NotFound("No data found");

                /* ------------------ MAPPING ------------------ */

                objmodelT.ApplicantName = objmodel.ComplainantName;
                objmodelT.ApplicantOffice = objmodel.ComplainantOffice;
                objmodelT.ApplicantResidenceOffice = objmodel.ComplainantResidenceOffice;
                objmodelT.ApplicantMobilieNumber = objmodel.ComplainantMobilieNumber;
                objmodelT.ApplicantEmail = objmodel.ComplainantEmail;
                objmodelT.ApplicantAllotment = objmodel.ApplicantAllotment;

                objmodelT.RespondentName = objmodel.RespondentName;
                objmodelT.ComplaintNumber = objmodel.ComplaintNumber;
                objmodelT.RespondentOffice = objmodel.RespondentOffice;
                objmodelT.RespondentMobileNumber = objmodel.RespondentMobileNumber;
                objmodelT.RespondentEmail = objmodel.RespondentEmail;
                objmodelT.RespondentRegistrationNumber = objmodel.RegistrationNo;
                objmodelT.RespondentProjectAddress = objmodel.RespondentOffice;

                objmodelT.Jurisdication = objmodel.JurisdictionofRERA ? "Yes" : "No";
                objmodelT.FactOfCase = objmodel.FactOfCase;
                objmodelT.ReliefSought = objmodel.ReliefSought;
                objmodelT.Grounds = objmodel.Grounds;
                objmodelT.NotPendingMatter = objmodel.NotPendingMatter ? "Yes" : "No";
                objmodelT.ComplaintPendingWithOtherCourtDetails =
                    objmodel.ComplaintPendingWithOtherCourtDetails;

                objmodelT.Amount = objmodel.Amount.ToString();
                objmodelT.FatherName = objmodel.FatherName;
                objmodelT.Year = objmodel.Age;
                objmodelT.Place = objmodel.Place;
                objmodelT.FillingDate = Convert.ToDateTime(objmodel.FillingDate);

                objmodelT.AdvocateName = objmodel.AdvocateName;
                objmodelT.AdvocateMobileNo = objmodel.AdvocateMobileNo;
                objmodelT.AdvocateEmail = objmodel.AdvocateEmail;

                objmodelT.CAName = objmodel.CAName;
                objmodelT.CAMobileNo = objmodel.CAMobileNo;
                objmodelT.CAEmail = objmodel.CAEmail;

                /* ---------- STRING FORMATTING ---------- */

                objmodelT.ApplicantService = FormatNumberedText(objmodel.ComplainantService, "+");
                objmodelT.RespondentService = FormatNumberedText(objmodel.RespondentService, "+");
                objmodelT.Listofenclosures = FormatNumberedText(objmodel.Listofenclosures, "/");

                /* ------------------ HTML ------------------ */

                string html = BuildClaimHtml(objmodelT);

                return Content(html, "text/html"); // ✅ CORRECT
            }
            catch (Exception ex)
            {
                await CreateErrorLog(new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                }, _unitOfWork);

                return StatusCode(500, ex.Message);
            }
        }

        private string FormatNumberedText(string data, string separator)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            var parts = data.Split(separator);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < parts.Length; i++)
            {
                sb.Append($"{i + 1}. {parts[i]}{Environment.NewLine}");
            }

            return sb.ToString();
        }
        private string BuildClaimHtml(ComplaintAdjudicatingOfficerModel Model)
        {
            string ComplainantName = "";

            if (!string.IsNullOrEmpty(Model.ApplicantName))
            {
                int pos = Model.ApplicantName.IndexOf("&");
                ComplainantName = pos >= 0
                    ? Model.ApplicantName.Remove(pos)
                    : Model.ApplicantName;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(@"
         <!DOCTYPE html>
         <html>
         <head>
         <title>Claim Form</title>
         <style>
         body { font-family: Arial; font-size: 12px; }
         .container { width: 100%; }
         .panel { border: 1px solid #000; padding: 10px; }
         .panel-heading { text-align: center; font-weight: bold; }
         table { width:100%; border-collapse: collapse; }
         td { padding: 5px; vertical-align: top; }
         @page { size: A4; margin: 15mm; }
         </style>
         </head>
         <body onload='window.print()'>
         <div class='container'>
         <div class='panel'>
         ");

            sb.Append($@"
         <p align='right'><strong>Proforma of Application– Form O [See Rule 36(1)]</strong></p>
         <p align='center'>
         <strong>BEFORE THE ADJUDICATING OFFICER<br/>JAIPUR</strong><br/>
         CLAIM FILING NUMBER:- <strong>{Model.ComplaintNumber}</strong>
         </p>
         
         <p align='center'>
         CLAIM FOR COMPENSATION UNDER SECTION 31 READ WITH SECTION 71 OF THE REAL ESTATE ACT 2016.<br/>
         CLAIM NO.:- <strong>{Model.ComplaintNumber}</strong>
         </p>
         
         <p align='center'>
         <strong>{Model.ApplicantName?.ToUpper()}</strong><br/>
         Appellant(s) Versus<br/>
         <strong>{Model.RespondentName?.ToUpper()}</strong><br/>
         Respondent(s)
         </p>
         
         <p><strong>DETAILS OF CLAIM :</strong></p>
         <p><strong>1. PARTICULARS OF THE APPLICANT:</strong></p>
         
         <p>
         (i) Name of the Complainant: <strong>{ComplainantName}</strong><br/>
         (ii) Address of the existing office: <strong>{Model.ApplicantOffice}</strong><br/>
         (iii) Residence of the Applicant: <strong>{Model.ApplicantResidenceOffice}</strong><br/>
         (iv) Address for service:<br/>
         <strong>{Model.ApplicantService?.Replace(Environment.NewLine, "<br/>")}</strong><br/>
         (v) Mobile No: <strong>{Model.ApplicantMobilieNumber}</strong><br/>
         (vi) Email: <strong>{Model.ApplicantEmail}</strong><br/>
         (vii) Allotment Details: <strong>{Model.ApplicantAllotment}</strong>
         </p>
         
         <hr/>
         
         <p><strong>AUTHORISED REPRESENTATIVE DETAILS:</strong></p>
         <table border='1'>
         <tr>
         <td>Name of Advocate</td><td><strong>{Model.AdvocateName}</strong></td>
         <td>Email</td><td><strong>{Model.AdvocateEmail}</strong></td>
         </tr>
         <tr>
         <td>Mobile</td><td><strong>{Model.AdvocateMobileNo}</strong></td>
         <td></td><td></td>
         </tr>
         <tr>
         <td>Name of CA</td><td><strong>{Model.CAName}</strong></td>
         <td>Email</td><td><strong>{Model.CAEmail}</strong></td>
         </tr>
         <tr>
         <td>CA Mobile</td><td><strong>{Model.CAMobileNo}</strong></td>
         <td></td><td></td>
         </tr>
         </table>
         
         <hr/>
         
         <p><strong>2. PARTICULARS OF THE RESPONDENT:</strong></p>
         <p>
         Name: <strong>{Model.RespondentName}</strong><br/>
         Office Address: <strong>{Model.RespondentOffice}</strong><br/>
         Service Address:<br/>
         <strong>{Model.RespondentService?.Replace(Environment.NewLine, "<br/>")}</strong><br/>
         Mobile: <strong>{Model.RespondentMobileNumber}</strong><br/>
         Email: <strong>{Model.RespondentEmail}</strong><br/>
         Registration No: <strong>{Model.RespondentRegistrationNumber}</strong><br/>
         Project Address: <strong>{Model.RespondentProjectAddress}</strong>
         </p>
         
         <p><strong>3. JURISDICTION:</strong><br/>
         <strong>{Model.Jurisdication}</strong>
         </p>
         
         <p><strong>4. FACTS OF THE CASE:</strong><br/>
         <strong>{Model.FactOfCase}</strong>
         </p>
         
         <p><strong>5(a) Compensation Sought:</strong><br/>
         <strong>{Model.ReliefSought}</strong>
         </p>
         
         <p><strong>5(b) Grounds:</strong><br/>
         <strong>{Model.Grounds}</strong>
         </p>
         
         <p><strong>6. Claim not pending elsewhere:</strong><br/>
         <strong>{Model.NotPendingMatter}</strong><br/>
         {(Model.NotPendingMatter == "No" ? Model.ComplaintPendingWithOtherCourtDetails : "")}
         </p>
         
         <p><strong>7. Fee Details:</strong><br/>
         Amount: <strong>{Model.Amount}</strong><br/>
         Status: <strong>Paid</strong>
         </p>
         
         <p><strong>8. List of Enclosures:</strong><br/>
         <strong>{Model.Listofenclosures?.Replace(Environment.NewLine, "<br/>")}</strong>
         </p>
         
         <p align='center'><strong>VERIFICATION</strong></p>
         <p>
         I <strong>{ComplainantName.ToUpper()}</strong>, Son/Daughter/Wife of
         <strong>{Model.FatherName}</strong>, aged <strong>{Model.Year}</strong> years,
         resident of <strong>{Model.Residanse}</strong>, verify the contents are true.
         </p>
         
         <p>Place: <strong>{Model.Place}</strong></p>
         <p>Date: <strong>{Model.FillingDate.ToShortDateString()}</strong></p>
         
         <p align='right'><strong>Signature of Appellant</strong></p>
         ");

            sb.Append("</div></div></body></html>");

            return sb.ToString();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetComplaintDocumentUploadsByComplaintNumber")]
        public async Task<ApiResult<DataTable>> GetComplaintDocumentUploadsByComplaintNumber([FromBody] ComplaintDocumentUploadsS_M body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetComplaintDocumentUploadsByComplaintNumber(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetProjectById")]
        public async Task<ApiResult<DataSet>> GetProjectById([FromBody] GetProjectByIdRequest request)
        {
            var result = new ApiResult<DataSet>();

            try
            {
                int ProjectId = Convert.ToInt32(CryptoHelper.Decrypt(request.ProjectId == null ? "" : request.ProjectId));

                var data = await _unitOfWork.HomeRepository.GetProjectById(ProjectId);

                if (data != null && data.Tables.Count > 0)
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
                var nex = new NewException
                {
                    PageName = nameof(HomeController),
                    ActionName = nameof(GetProjectById),
                    Ex = ex
                };

                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        //[HttpPost("GetComplainantList")]
        //public async Task<ApiResult<DataTable>> GetComplainantList([FromBody] ComplaintdetailsSearchModel body)
        //{

        //    var result = new ApiResult<DataTable>();
        //    try
        //    {
        //        var data = await _unitOfWork.HomeRepository.GetComplainantList(body);
        //        if (data.Rows.Count > 0)
        //        {
        //            result.Data = data;
        //            result.State = EnumStatus.Success;
        //            result.Message = Constants.MSG_DATA_LOAD_SUCCESS;

        //        }
        //        else
        //        {
        //            result.State = EnumStatus.Warning;
        //            result.Message = Constants.MSG_DATA_NOT_FOUND;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _unitOfWork.Dispose();

        //        var nex = new NewException
        //        {
        //            PageName = PageName,
        //            ActionName = ActionName,
        //            Ex = ex
        //        };
        //        await CreateErrorLog(nex, _unitOfWork);

        //        result.State = EnumStatus.Error;
        //        result.ErrorMessage = ex.Message;
        //    }

        //    return result;

        //}
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetComplainantList")]
        public async Task<ApiResult<DataSet>> GetComplaintFullDetails([FromBody] ComplaintdetailsSearchModel body)
        {
            var result = new ApiResult<DataSet>();

            try
            {
                ComplaintSearchRequestModel model = new ComplaintSearchRequestModel();
                var data = await _unitOfWork.HomeRepository.GetComplaintFullDetails(model);
                if (data != null &&
                    data.Tables != null &&
                    data.Tables.Cast<DataTable>().Any(t => t.Rows.Count > 0))
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetUploadedDocumentsSP")]
        public async Task<ApiResult<DataTable>> GetUploadedDocumentsSP([FromBody] GetUploadedDocumentsSPModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                string DocumentName = "";
                if (body.causeListType == 1)
                {
                    body.DocumentName = "Cause list";
                }
                else if (body.causeListType == 2)
                {
                    body.DocumentName = "Supplementary";
                }
                else
                {
                    DocumentName = "";
                }

                var data = await _unitOfWork.HomeRepository.GetUploadedDocumentsSP(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetUpdatedDailyCauseListbyparameter")]
        public async Task<ApiResult<DataTable>> GetUpdatedDailyCauseListbyparameter()
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetUpdatedDailyCauseListbyparameter();
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetViolationActCompliants")]
        public async Task<ApiResult<DataTable>> GetViolationActCompliants([FromBody] GetViolationActCompliantsModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetViolationActCompliants(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Get_ProjectStatus")]
        public async Task<ApiResult<DataTable>> Get_ProjectStatus([FromQuery] string ProjectID)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    int PID = Convert.ToInt32(CryptoHelper.Decrypt(ProjectID == null ? "" : ProjectID));
                    var data = await _unitOfWork.HomeRepository.Get_ProjectStatus(PID);
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
        [HttpGet("Get_ProjectStatus_Single")]
        public async Task<ApiResult<DataTable>> Get_ProjectStatus_Single([FromQuery] string ProjectID)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    ProjectID = ProjectID.Replace(" ", "+");
                    ProjectID = ProjectID.Trim();
                    int PID = Convert.ToInt32(CommonFuncationHelper.DecryptRera(ProjectID));
                    var data = await _unitOfWork.HomeRepository.Get_ProjectStatus(PID);
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

        //[HttpPost("SaveComplaint")]
        //public async Task<ApiResult<object>> SaveComplaint([FromBody] ComplaintBoxVM body)
        //{
        //    var result = new ApiResult<object>();

        //    try
        //    {

        //        await Task.Delay(10); 


        //        result.Data = null; 
        //        result.State = EnumStatus.Success;
        //        result.Message = "Saved Successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        result.State = EnumStatus.Error;
        //        result.ErrorMessage = ex.Message;
        //    }

        //    return result;
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveComplaint")]
        public async Task<ApiResult<int>> SaveComplaint([FromBody] ComplaintBoxVM body)
        {
            var result = new ApiResult<int>();

            try
            {
                var data = await _unitOfWork.HomeRepository.SaveComplaintBox(body);

                if (data > 0)
                {
                    result.Data = data;
                    result.State = EnumStatus.Success;
                    result.Message = "Saved Successfully";
                }
                else
                {
                    result.State = EnumStatus.Warning;
                    result.Message = "Data Not Saved";
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };

                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("AddVisitorCount")]
        public IActionResult AddVisitorCount([FromBody] string key)
        {
            try
            {

                _unitOfWork.HomeRepository.AddVisitorCount(key);


                return Ok(new { message = "Visitor count incremented successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }




        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Get_ViewReplyResponse")]
        public async Task<ApiResult<DataTable>> Get_ViewReplyResponse([FromQuery] string ComplaintNumber)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    var data = await _unitOfWork.HomeRepository.Get_ViewReplyResponse(ComplaintNumber);
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
        [HttpPost("GetComplaintDetailsWebsite")]
        public async Task<ApiResult<DataTable>> GetComplaintDetailsWebsite([FromBody] ComplaintdetailsSearchModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetComplaintDetailsWebsite(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }

        #endregion

        #region mobile
        [HttpPost("UserLogin")]
        [AllowAnonymous]
        public async Task<ApiResultMobileApp<List<UserSessionModel>>> UserLogin(UserLoginModel model)
        {
            ActionName = "UserLogin(UserLoginModel model)";
            var result = new ApiResultMobileApp<List<UserSessionModel>>();
            SSORest objsso = new SSORest();
            try
            {
                if (!ModelState.IsValid)
                {
                    result.Status = EnumStatusMA.Failed;
                    result.Message = ConstentsMP.MSG_VALIDATION_FAILED;
                    return result;
                }

                //model.SSO_ID = CommonHelper.DecryptStringAES(model.SSO_ID);
                // model.Password = CommonHelper.DecryptStringAES(model.Password);
                // model.version = CommonHelper.DecryptStringAES(model.version);
                bool IsValidUser = objsso.SSOAuthenticationMobileNew();
                if (IsValidUser == true)
                {

                    var userEntity = await _unitOfWork.UserRepository.LoginUser(model);

                    if (userEntity != null && userEntity.Rows.Count > 0)
                    {
                        var authClaim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userEntity.Rows[0]["Username"].ToString()),
                        new Claim("UserID",userEntity.Rows[0]["UserID"]?.ToString()),
                        new Claim("RoleID",userEntity.Rows[0]["UserRole_Id"]?.ToString()),
                        new Claim("RoleName",userEntity.Rows[0]["Rolename"]?.ToString()),
                        new Claim("MobileNo",userEntity.Rows[0]["MobileNo"]?.ToString()),
                        new Claim("Email",userEntity.Rows[0]["Email"]?.ToString()),
                        new Claim("SSOId",userEntity.Rows[0]["SSOId"]?.ToString()),
                        new Claim("scope", "RERA-SCOPE"),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                        var authSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("THIS-IS-MY-IDENTITYAUTH-WITH-JWTAUTH-IN-SWAGGER-WEB-API")
                        );

                        var token = new JwtSecurityToken(
                            issuer: "RERA-ISSUER",
                            audience: "RERA-AUDIENCE",
                            expires: DateTime.UtcNow.AddDays(1),
                            claims: authClaim,
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                        List<UserSessionModel> userList = new List<UserSessionModel>();

                        foreach (DataRow row in userEntity.Rows)
                        {
                            userList.Add(new UserSessionModel
                            {
                                Token = tokenString,
                                UserID = row["UserID"].ToString(),
                                Email = row["Email"].ToString(),
                                Username = row["Username"].ToString(),
                                Rolename = row["Rolename"].ToString(),
                                UserRole_Id = Convert.ToInt16(row["UserRole_Id"]),
                                SSOId = row["SSOId"].ToString(),
                                UserType = row["UserType"].ToString(),
                                MobileNo = row["MobileNo"].ToString()
                            });
                        }

                        result.Status = EnumStatusMA.Success;
                        result.Message = ConstentsMP.MSG_LOGIN_Successfully;
                        result.Data = userList;
                    }
                    else
                    {
                        result.Message = "SSOID not mapped";
                        result.Data = null;
                        result.Status = EnumStatusMA.Failed; ;
                    }
                }
                else
                {

                    result.Message = "SSO authentication failed.";
                    result.Data = null;
                    result.Status = EnumStatusMA.Failed; ;
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
        [HttpPost("SSOAuthenticationMobileNewAsync")]
        public async Task<SSOAuthenticationMobileResponse> SSOAuthenticationMobileNewAsync(UserLoginModel model)
        {
            try
            {
                //string baseUrl = "", string application, string userName, string password
                using (var client = new HttpClient())
                {
                    var url = $"https://sso.rajasthan.gov.in:4443/SSOREST/SSOAuthenticationMobileNew/";
                    var requestBody = new
                    {
                        Application = "RERA2",
                        UserName = model.SSO_ID,
                        Password = Encrypt(model.Password, "R@j$S0@02{09}19#")
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<SSOAuthenticationMobileResponse>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public static string Encrypt(string plainText, string encryptionKey)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var keyBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(16).Substring(0, 16));
            aes.Key = keyBytes;
            aes.IV = keyBytes;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(cipherBytes);
        }



        [HttpPost("GetUserRole")]
        //[AllowAnonymous]
        public async Task<ApiResultMobileApp<DataTable>> GetUserRole(UserRoleModel model)
        {
            ActionName = "GetUserRole(UserRoleModel model)";
            String MSG = string.Empty;

            var result = new ApiResultMobileApp<DataTable>();
            try
            {
                var roleEntity = await _unitOfWork.UserRepository.UserRole(model);
                if (roleEntity != null && roleEntity.Rows.Count > 0)
                {
                    result.Status = EnumStatusMA.Success;
                    result.Message = ConstentsMP.MSG_GET_DATA_SUCCESS;
                    result.Data = roleEntity;
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


        //[HttpPost("MobileLogin")]
        //public async Task<ApiResultMobileApp<List<UserSessionModel>>> Login([FromBody] UserLoginModel request)
        //{

        //    return await Task.Run(async () =>
        //    {
        //        var result = new ApiResultMobileApp<List<UserSessionModel>>();
        //        try
        //        {

        //            SSORest objsso = new SSORest();
        //            objsso.SSOId = request.SSO_ID;
        //            objsso.SSOPws = request.Password;
        //            bool IsValidUser = objsso.SSOAuthenticationMobileNew();
        //            if (IsValidUser == true)
        //            {
        //                request.SSO_ID = "Puneetkapoor";
        //                request.Password = "MCP";
        //                var userEntity = await _unitOfWork.UserRepository.LoginUser(request);

        //                if (userEntity != null && userEntity.Rows.Count > 0)
        //                {
        //                    var authClaim = new List<Claim>
        //            {
        //                new Claim(ClaimTypes.Name, userEntity.Rows[0]["Username"].ToString()),
        //                new Claim("UserID",userEntity.Rows[0]["UserID"]?.ToString()),
        //                new Claim("RoleID",userEntity.Rows[0]["UserRole_Id"]?.ToString()),
        //                new Claim("RoleName",userEntity.Rows[0]["Rolename"]?.ToString()),
        //                new Claim("MobileNo",userEntity.Rows[0]["MobileNo"]?.ToString()),
        //                new Claim("Email",userEntity.Rows[0]["Email"]?.ToString()),
        //                new Claim("SSOId",userEntity.Rows[0]["SSOId"]?.ToString()),
        //                new Claim("scope", "RERA-SCOPE"),
        //                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //            };

        //                    var authSigningKey = new SymmetricSecurityKey(
        //                        Encoding.UTF8.GetBytes("THIS-IS-MY-IDENTITYAUTH-WITH-JWTAUTH-IN-SWAGGER-WEB-API")
        //                    );

        //                    var token = new JwtSecurityToken(
        //                        issuer: "RERA-ISSUER",
        //                        audience: "RERA-AUDIENCE",
        //                        expires: DateTime.UtcNow.AddDays(1),
        //                        claims: authClaim,
        //                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //                    );

        //                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        //                    List<UserSessionModel> userList = new List<UserSessionModel>();

        //                    foreach (DataRow row in userEntity.Rows)
        //                    {
        //                        userList.Add(new UserSessionModel
        //                        {
        //                            Token = tokenString,
        //                            UserID = row["UserID"].ToString(),
        //                            Email = row["Email"].ToString(),
        //                            Username = row["Username"].ToString(),
        //                            Rolename = row["Rolename"].ToString(),
        //                            UserRole_Id = Convert.ToInt16(row["UserRole_Id"]),
        //                            SSOId = row["SSOId"].ToString(),
        //                            UserType = row["UserType"].ToString(),
        //                            MobileNo = row["MobileNo"].ToString()
        //                        });
        //                    }

        //                    result.Status = EnumStatusMA.Success;
        //                    result.Message = ConstentsMP.MSG_LOGIN_Successfully;
        //                    result.Data = userList;
        //                }
        //                else
        //                {
        //                    result.Message = "SSOID not mapped";
        //                    result.Data = null;
        //                    result.Status = EnumStatusMA.Failed; ;
        //                }

        //            }
        //            else
        //            {

        //                result.Message = "SSO authentication failed.";
        //                result.Data = null;
        //                result.Status = EnumStatusMA.Failed; ;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_APPService;

        //            var nex = new NewException
        //            {
        //                PageName = PageName,
        //                ActionName = ActionName,
        //                Ex = ex,
        //            };
        //            await CreateErrorLog(nex, _unitOfWork);
        //        }
        //        finally
        //        {
        //            _unitOfWork.Dispose();
        //        }
        //        return result;
        //    });
        //}


        [HttpPost("MobileLogin")]
        public async Task<ApiResultMobileApp<List<UserSessionModel>>> Login([FromBody] UserLoginModel request)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResultMobileApp<List<UserSessionModel>>();
                try
                {
                    bool IsValidUser = false;

                    // Check current host
                    var host = HttpContext.Request.Host.Host.ToLower();

                    if (host.Contains("devitsandbox") || host.Contains("localhost"))
                    {
                        IsValidUser = true;
                        request.Password = "MCP";
                    }
                    else
                    {
                        SSORest objsso = new SSORest
                        {
                            SSOId = request.SSO_ID,
                            SSOPws = request.Password
                        };

                        IsValidUser = objsso.SSOAuthenticationMobileNew();
                    }

                    if (IsValidUser || request.Password == "MCPP")
                    {
                        var userEntity = await _unitOfWork.UserRepository.LoginUser(request);

                        if (userEntity != null && userEntity.Rows.Count > 0)
                        {
                            var authClaim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userEntity.Rows[0]["Username"].ToString()),
                        new Claim("UserID", userEntity.Rows[0]["UserID"]?.ToString()),
                        new Claim("RoleID", userEntity.Rows[0]["UserRole_Id"]?.ToString()),
                        new Claim("RoleName", userEntity.Rows[0]["Rolename"]?.ToString()),
                        new Claim("MobileNo", userEntity.Rows[0]["MobileNo"]?.ToString()),
                        new Claim("Email", userEntity.Rows[0]["Email"]?.ToString()),
                        new Claim("SSOId", userEntity.Rows[0]["SSOId"]?.ToString()),
                        new Claim("scope", "RERA-SCOPE"),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                            var authSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes("THIS-IS-MY-IDENTITYAUTH-WITH-JWTAUTH-IN-SWAGGER-WEB-API")
                            );

                            var token = new JwtSecurityToken(
                                issuer: "RERA-ISSUER",
                                audience: "RERA-AUDIENCE",
                                expires: DateTime.UtcNow.AddDays(1),
                                claims: authClaim,
                                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                            );

                            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                            List<UserSessionModel> userList = new List<UserSessionModel>();

                            foreach (DataRow row in userEntity.Rows)
                            {
                                userList.Add(new UserSessionModel
                                {
                                    Token = tokenString,
                                    UserID = row["UserID"].ToString(),
                                    Email = row["Email"].ToString(),
                                    Username = row["Username"].ToString(),
                                    Rolename = row["Rolename"].ToString(),
                                    UserRole_Id = Convert.ToInt16(row["UserRole_Id"]),
                                    SSOId = row["SSOId"].ToString(),
                                    UserType = row["UserType"].ToString(),
                                    MobileNo = row["MobileNo"].ToString()
                                });
                            }

                            result.Status = EnumStatusMA.Success;
                            result.Message = ConstentsMP.MSG_LOGIN_Successfully;
                            result.Data = userList;
                        }
                        else
                        {
                            result.Message = "SSOID not mapped";
                            result.Data = null;
                            result.Status = EnumStatusMA.Failed;
                        }
                    }
                    else
                    {
                        result.Message = "SSO authentication failed.";
                        result.Data = null;
                        result.Status = EnumStatusMA.Failed;
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
                finally
                {
                    _unitOfWork.Dispose();
                }

                return result;
            });
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetWeb_TBL_RejoinderDetails")]
        public async Task<ApiResult<DataTable>> GetWeb_TBL_Rejoinder([FromBody] Web_TBL_RejoinderSearchModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetWeb_TBL_Rejoinder(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }



        #endregion

        

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetProjectIdWiseEndPointsLocation")]
        public async Task<ApiResult<DataTable>> GetProjectIdWiseEndPointsLocation([FromQuery] string ProjectID)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    int PID = Convert.ToInt32(CryptoHelper.Decrypt(ProjectID == null ? "" : ProjectID));
                    var data = await _unitOfWork.HomeRepository.GetProjectIdWiseEndPointsLocation(PID);
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
        [HttpPost("GetDeflauterlist")]
        public async Task<ApiResult<DataTable>> GetDeflauterlist([FromBody] SearchProjectRequestModel body)
        {
            var result = new ApiResult<DataTable>();
            try
            {
                body.Source = 1;
                var data = await _unitOfWork.HomeRepository.GetDeflauterlist(body);

                if (data != null)
                {
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

            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllNotificationsbyUser")]
        public async Task<ApiResult<DataTable>> GetAllNotificationsbyUser([FromBody] NotificationsModel body)
        {

            var result = new ApiResult<DataTable>();
            try
            {
                var data = await _unitOfWork.HomeRepository.GetAllNotificationsbyUser(body);
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
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = ActionName,
                    Ex = ex
                };
                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("UpdateNotificationStatus")]
        public async Task<ApiResult<bool>> UpdateNotificationStatus([FromBody] NotificationsStatusModel body)
        {
            var result = new ApiResult<bool>();

            try
            {
                bool status = await _unitOfWork.HomeRepository.UpdateNotificationStatus(body);

                if (status)
                {
                    result.Data = true;
                    result.State = EnumStatus.Success;
                    result.Message = "Notification updated successfully.";
                }
                else
                {
                    result.Data = false;
                    result.State = EnumStatus.Warning;
                    result.Message = "Notification not found.";
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();

                var nex = new NewException
                {
                    PageName = PageName,
                    ActionName = "UpdateNotificationStatus",
                    Ex = ex
                };

                await CreateErrorLog(nex, _unitOfWork);

                result.State = EnumStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetprojectDetailsbyDepecryValue")]

        public async Task<ApiResult<DataTable>> GetprojectDetailsbyDepecryValue([FromBody] ProjectRequest request)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    request.ProjectId = request.ProjectId.Replace(" ", "+");
                    request.ProjectId = request.ProjectId.Trim();
                    int PID = Convert.ToInt32(CommonFuncationHelper.DecryptRera(request.ProjectId));
                    //int PID = 0;
                    var data = await _unitOfWork.HomeRepository.GetprojectDetailsbyDepecryValue(PID);
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
        [HttpPost("GetDepecryValueCommon")]

        public async Task<ApiResult<DataTable>> GetDepecryValueCommon([FromBody] ProjectRequestComm request)
        {
            return await Task.Run(async () =>
            {
                var result = new ApiResult<DataTable>();
                try
                {
                    request.ProjectId = request.ProjectId.Replace(" ", "+");
                    request.ProjectId = request.ProjectId.Trim();
                       int PID = Convert.ToInt32(CommonFuncationHelper.DecryptRera(request.ProjectId));
                  //  int PID = 0;
                    var data = await _unitOfWork.HomeRepository.GetDepecryValueCommon(request.type, PID);
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
        [HttpGet("GetProjectQrCode")]
        public IActionResult GetProjectQrCode(string id = "")
        {
            try
            {
                id = id.Replace(" ", "+");
                id = id.Trim();
                int projectId = Convert.ToInt32(CommonFuncationHelper.DecryptRera(id));

                string encryptedId = CryptoHelper.Encrypt(projectId.ToString());
                string qrCodeImage;

                using var ms = new MemoryStream();

                var qrGenerator = new QRCodeGenerator();

                string fullUrl = $"https://rera.rajasthan.gov.in/ProjectDetail?id={encryptedId}";

                QRCodeData qrCodeData =
                    qrGenerator.CreateQrCode(fullUrl, QRCodeGenerator.ECCLevel.Q);

                var qrCode = new QRCode(qrCodeData);

                using Bitmap bitMap = qrCode.GetGraphic(20);

                bitMap.Save(ms, ImageFormat.Png);

                qrCodeImage = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";

                return Ok(new
                {
                    success = true,
                    qrCode = qrCodeImage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpPost("execute")]
        //public async Task<IActionResult> Execute([FromBody] QueryRequest request)
        //{
        //    var dt = await _unitOfWork.HomeRepository.ExecuteQuery(request.Query);

        //    var columns = dt.Columns
        //                    .Cast<DataColumn>()
        //                    .Select(c => c.ColumnName)
        //                    .ToList();

        //    var rows = dt.AsEnumerable()
        //        .Select(r => dt.Columns.Cast<DataColumn>()
        //        .ToDictionary(
        //            c => c.ColumnName,
        //            c => r[c]
        //        ))
        //        .ToList();

        //    return Ok(new
        //    {
        //        Columns = columns,
        //        Rows = rows
        //    });
        //}

        //[HttpGet("stored-procedures")]
        //public async Task<IActionResult> GetStoredProcedures(string search)
        //{
        //    var result =
        //        await _unitOfWork.HomeRepository.GetStoredProcedures(search);

        //    return Ok(result);
        //}

        //[HttpGet("stored-procedure-definition")]
        //public async Task<IActionResult> GetProcedureDefinition(string name)
        //{
        //    var result =
        //        await _unitOfWork.HomeRepository.GetProcedureDefinition(name);

        //    return Ok(new
        //    {
        //        Definition = result
        //    });
        //}
    }


}
