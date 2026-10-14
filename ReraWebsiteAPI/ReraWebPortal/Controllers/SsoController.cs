using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraModel;
using ReraModel.CommonFunction;
using ReraModel.DashboardDepartmentModel;
using ReraModel.DashboardPromoterModel;
using ReraModel.HomeModel;
using ReraModel.UserLoginModel;
using ReraModel.UserSessionModel;
using RJ_RERA_API.Core.Helper;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static ReraMobileApi.Api.apiservice.CommonFunctions;

namespace ReraWebPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsoController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ssolanding")]
        public async Task<IActionResult> SSOLandingThenRedirectToApp()
        {
            try
            {
                var tokenDetail = Request.Form["UserDetails"];

                //api token
                var ssoTokenDetails = SsoServiceHelper.GetSSOTokenDetails(tokenDetail);
                // validate token
                var ssoId = ssoTokenDetails.Data.sAMAccountName ?? string.Empty;
                if (ssoTokenDetails?.Status == EnumStatusMA.Error || string.IsNullOrWhiteSpace(ssoId))
                {
                    Response.Redirect(ConfigurationHelper.SSOBaseUrl, false);
                    return Content("");
                }

                //api user profile
                var ssoUserDetailsApi = SsoServiceHelper.GetSSOUserProfileDetail(ssoId, tokenDetail);
                //var ssoUserDetailsApi = SsoServiceHelper.GetSSOUserDetail(ssoId);
                //CommonFuncationHelper.WriteTextLog($"get the ssoUserDetailsApi : {JsonConvert.SerializeObject(ssoUserDetailsApi)}");
                // validate user profile
                if (ssoUserDetailsApi == null)
                {
                    Response.Redirect(ConfigurationHelper.SSOBaseUrl, false);
                    return Content(""); ;
                }

                //// save
                //var ssoSearchId = await _unitOfWork.SSORepository.AddSSOUserProfileDetails(ssoUserDetailsApi);
                //_unitOfWork.SaveChanges();

                ////encode search id
                //ssoSearchId = HttpUtility.UrlEncode(CommonFuncationHelper.Encrypt(ssoSearchId));

                // cookie

                string encId = Protector.Protect(ssoUserDetailsApi.SSOID);
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(1),
                    SameSite = SameSiteMode.None, // Important for cross-site cookies
                    Secure = true,                // Required when SameSite=None
                    HttpOnly = false,              // Optional: set to false to access via JS
                };
                Response.Cookies.Append(ConfigurationHelper.AppName, encId, cookieOptions);

                // success url
                string appUrl = $"{ConfigurationHelper.ApplicationURL}/Ssointegration/SsoLanding?id={encId}";// no need, pick from cookie

                return Redirect(appUrl);
            }
            catch (System.Exception ex)
            {
                return Redirect(ConfigurationHelper.SSOBaseUrl);
            }
        }

    }
}
