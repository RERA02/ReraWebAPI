using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RERA.ViewModel.Common;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraModel;
using ReraModel.CommonFunction;
using ReraModel.DashboardDepartmentModel;
using ReraModel.DashboardPromoterModel;
using ReraModel.HomeModel;
using ReraModel.UserLoginModel;
using ReraModel.UserSessionModel;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static ReraMobileApi.Api.apiservice.CommonFunctions;

namespace ReraWebPortal.Controllers
{
    [ApiController]
    [Route("api/Project")]
    //[Authorize(Policy = "ApiScope")]
    [ApiKeyAuth]
    public class UserLoginController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public override string PageName => "UserLoginController";
        public override string ActionName { get; set; }
        public UserLoginController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }


        //[HttpPost("UserLogin")]
        //public async Task<ApiResultMobileApp<UserSessionModel>> UserLogin(UserLoginModel model)

        //{
        //    ActionName = "UserLogin(UserLoginModel model)";
        //    String MSG = string.Empty;
        //    var result = new ApiResultMobileApp<UserSessionModel>();

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_VALIDATION_FAILED;
        //            return result;
        //        }

        //        var userEntity = await _unitOfWork.UserRepository.LoginUser(model);



        //        if (userEntity != null && userEntity.Rows.Count > 0)
        //        {
        //            var authClaim = new List<Claim>
        //                {
        //                    new Claim(ClaimTypes.Name,userEntity.Rows[0]["Username"].ToString()),
        //                    new Claim("BugFrom","Mobile"),
        //                    new Claim("scope", "RERA-SCOPE"),  // policy if token genrate with wrong then ocalet not validate
        //                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        //                };

        //            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS-IS-MY-IDENTITYAUTH-WITH-JWTAUTH-IN-SWAGGER-WEB-API"));

        //            var token = new JwtSecurityToken(
        //                issuer: "RERA-ISSUER",
        //                audience: "RERA-AUDIENCE",
        //                expires: DateTime.Now.AddDays(1),
        //                claims: authClaim,
        //                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //                );

        //            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        //            UserSessionModel obj = new UserSessionModel();

        //            obj.Token = tokenString;
        //            obj.User_ID = userEntity.Rows[0]["UserID"].ToString();
        //            obj.Email = userEntity.Rows[0]["Email"].ToString(); ;
        //            obj.Name = userEntity.Rows[0]["Username"].ToString();
        //            obj.Role = userEntity.Rows[0]["Rolename"].ToString();
        //            obj.UserRoleId = userEntity.Rows[0]["UserRole_Id"].ToString();
        //            obj.UserType = userEntity.Rows[0]["UserType"].ToString();

        //            result.Status = EnumStatusMA.Success;
        //            result.Message = ConstentsMP.MSG_LOGIN_Successfully;
        //            result.Data = obj;
        //            //return result;

        //        }
        //        else
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_LOGIN_UnSuccessfully;
        //            return result;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Status = EnumStatusMA.Failed;
        //        result.Message = ConstentsMP.MSG_APPService;
        //        // write error log
        //        var nex = new NewException
        //        {
        //            PageName = PageName,
        //            ActionName = ActionName,
        //            Ex = ex,
        //        };
        //        await CreateErrorLog(nex, _unitOfWork);
        //    }
        //    return result;
        //}


        //[HttpPost("UserLogin")]

        //public async Task<ApiResultMobileApp<DataTable>> UserLogin(UserLoginModel model)
        //{
        //    ActionName = "UserLogin(UserLoginModel model)";
        //    String MSG = string.Empty;
        //    var result = new ApiResultMobileApp<DataTable>();
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_VALIDATION_FAILED;
        //            return result;
        //        }

        //        var userEntity = await _unitOfWork.UserRepository.LoginUser(model);



        //        if (userEntity != null && userEntity.Rows.Count > 0)
        //        {

        //            result.Status = EnumStatusMA.Success;
        //            result.Message = ConstentsMP.MSG_LOGIN_Successfully;
        //            result.Data = userEntity;
        //            //return result;

        //        }
        //        else
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_LOGIN_UnSuccessfully;
        //            return result;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Status = EnumStatusMA.Failed;
        //        result.Message = ConstentsMP.MSG_APPService;
        //        // write error log
        //        var nex = new NewException
        //        {
        //            PageName = PageName,
        //            ActionName = ActionName,
        //            Ex = ex,
        //        };
        //        await CreateErrorLog(nex, _unitOfWork);
        //    }
        //    return result;
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> UserLogin([FromBody] UserLoginModel model)
        //{
        //    ActionName = "UserLogin(UserLoginModel model)";
        //    var result = new ApiResultMobileApp<List<UserSessionModel>>();

        //    try
        //    {
        //        // ✅ Validate Model
        //        if (!ModelState.IsValid)
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_VALIDATION_FAILED;
        //            return Ok(result);
        //        }

        //        // ✅ Decrypt values
        //        model.SSO_ID = CommonHelper.DecryptStringAES(model.SSO_ID);
        //        model.Password = CommonHelper.DecryptStringAES(model.Password);
        //        model.version = CommonHelper.DecryptStringAES(model.version);

        //        // ✅ Call Login Method
        //        var userEntity = await _unitOfWork.UserRepository.LoginUser(model);

        //        if (userEntity != null && userEntity.Rows.Count > 0)
        //        {
        //            DataRow row = userEntity.Rows[0];

        //            // ✅ Create Claims
        //            var authClaim = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, row["Username"]?.ToString()),
        //        new Claim("UserID", row["UserID"]?.ToString()),
        //        new Claim("RoleID", row["UserRole_Id"]?.ToString()),
        //        new Claim("RoleName", row["Rolename"]?.ToString()),
        //        new Claim("MobileNo", row["MobileNo"]?.ToString()),
        //        new Claim("Email", row["Email"]?.ToString()),
        //        new Claim("SSOId", row["SSOId"]?.ToString()),
        //        new Claim("scope", "RERA-SCOPE"),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    };

        //            // ✅ SAME TOKEN CODE AS YOU PROVIDED
        //            var authSigningKey = new SymmetricSecurityKey(
        //                Encoding.UTF8.GetBytes("THIS-IS-MY-IDENTITYAUTH-WITH-JWTAUTH-IN-SWAGGER-WEB-API")
        //            );

        //            var token = new JwtSecurityToken(
        //                issuer: "RERA-ISSUER",
        //                audience: "RERA-AUDIENCE",
        //                expires: DateTime.UtcNow.AddDays(1),
        //                claims: authClaim,
        //                signingCredentials: new SigningCredentials(
        //                    authSigningKey,
        //                    SecurityAlgorithms.HmacSha256
        //                )
        //            );

        //            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        //            // ✅ Prepare Response List
        //            List<UserSessionModel> userList = new List<UserSessionModel>
        //    {
        //        new UserSessionModel
        //        {
        //            Token = tokenString,
        //            UserID = row["UserID"]?.ToString(),
        //            Email = row["Email"]?.ToString(),
        //            Username = row["Username"]?.ToString(),
        //            Rolename = row["Rolename"]?.ToString(),
        //            UserRole_Id = Convert.ToInt16(row["UserRole_Id"]),
        //            SSOId = row["SSOId"]?.ToString(),
        //            UserType = row["UserType"]?.ToString(),
        //            MobileNo = row["MobileNo"]?.ToString()
        //        }
        //    };

        //            result.Status = EnumStatusMA.Success;
        //            result.Message = ConstentsMP.MSG_LOGIN_Successfully;
        //            result.Data = userList;
        //        }
        //        else
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_LOGIN_UnSuccessfully;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Status = EnumStatusMA.Failed;
        //        result.Message = ConstentsMP.MSG_APPService;

        //        var nex = new NewException
        //        {
        //            PageName = PageName,
        //            ActionName = ActionName,
        //            Ex = ex,
        //        };

        //        await CreateErrorLog(nex, _unitOfWork);
        //    }

        //    return Ok(result);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> UserLogin([FromBody] UserLoginModel model)
        //{
        //    ActionName = "UserLogin(UserLoginModel model)";
        //    var result = new ApiResultMobileApp<List<UserSessionModel>>();

        //    try
        //    {
        //        if (model == null)
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = "Request body cannot be empty.";
        //            return BadRequest(result);
        //        }



        //        // ✅ 2. Validate Allowed Keys (NO JSON parsing – Safe way)
        //        var allowedKeys = new[] { "SSO_ID", "Password", "version" };

        //        var receivedKeys = model.GetType()
        //                                .GetProperties()
        //                                .Where(p => p.GetValue(model) != null)
        //                                .Select(p => p.Name)
        //                                .ToList();

        //        foreach (var key in receivedKeys)
        //        {
        //            if (!allowedKeys.Contains(key))
        //            {
        //                result.Status = EnumStatusMA.Failed;
        //                result.Message = "Invalid parameter detected.";
        //                return BadRequest(result);
        //            }
        //        }

        //        // ✅ 3. Validate ModelState
        //        if (!ModelState.IsValid)
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_VALIDATION_FAILED;
        //            return BadRequest(result);
        //        }

        //        // ✅ 4. Decrypt values
        //        //model.SSO_ID = CommonHelper.DecryptStringAES(model.SSO_ID);
        //        //model.Password = CommonHelper.DecryptStringAES(model.Password);
        //        //model.version = CommonHelper.DecryptStringAES(model.version);

        //        // ✅ 5. Call Database
        //        var userEntity = await _unitOfWork.UserRepository.LoginUser(model);

        //        if (userEntity != null && userEntity.Rows.Count > 0)
        //        {
        //            DataRow row = userEntity.Rows[0];

        //            // ✅ 6. Create Claims
        //            var authClaim = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, row["Username"]?.ToString() ?? ""),
        //        new Claim("UserID", row["UserID"]?.ToString() ?? ""),
        //        new Claim("RoleID", row["UserRole_Id"]?.ToString() ?? ""),
        //        new Claim("RoleName", row["Rolename"]?.ToString() ?? ""),
        //        new Claim("MobileNo", row["MobileNo"]?.ToString() ?? ""),
        //        new Claim("Email", row["Email"]?.ToString() ?? ""),
        //        new Claim("SSOId", row["SSOId"]?.ToString() ?? ""),
        //        new Claim("scope", "RERA-SCOPE"),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    };

        //            // ✅ 7. Generate JWT
        //            var authSigningKey = new SymmetricSecurityKey(
        //                Encoding.UTF8.GetBytes("THIS-IS-MY-IDENTITYAUTH-WITH-JWTAUTH-IN-SWAGGER-WEB-API")
        //            );

        //            var token = new JwtSecurityToken(
        //                issuer: "RERA-ISSUER",
        //                audience: "RERA-AUDIENCE",
        //                expires: DateTime.UtcNow.AddDays(1),
        //                claims: authClaim,
        //                signingCredentials: new SigningCredentials(
        //                    authSigningKey,
        //                    SecurityAlgorithms.HmacSha256)
        //            );

        //            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        //            // ✅ 8. Prepare Response
        //            var userList = new List<UserSessionModel>
        //    {
        //        new UserSessionModel
        //        {
        //            Token = tokenString,
        //            UserID = row["UserID"]?.ToString(),
        //            Email = row["Email"]?.ToString(),
        //            Username = row["Username"]?.ToString(),
        //            Rolename = row["Rolename"]?.ToString(),
        //            UserRole_Id = Convert.ToInt16(row["UserRole_Id"]),
        //            SSOId = row["SSOId"]?.ToString(),
        //            UserType = row["UserType"]?.ToString(),
        //            MobileNo = row["MobileNo"]?.ToString()
        //        }
        //    };

        //            result.Status = EnumStatusMA.Success;
        //            result.Message = ConstentsMP.MSG_LOGIN_Successfully;
        //            result.Data = userList;
        //        }
        //        else
        //        {
        //            result.Status = EnumStatusMA.Failed;
        //            result.Message = ConstentsMP.MSG_LOGIN_UnSuccessfully;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Status = EnumStatusMA.Failed;
        //        result.Message = ConstentsMP.MSG_APPService;

        //        var nex = new NewException
        //        {
        //            PageName = PageName,
        //            ActionName = ActionName,
        //            Ex = ex,
        //        };

        //        await CreateErrorLog(nex, _unitOfWork);
        //    }

        //    return Ok(result);
        //}


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


        [HttpPost("MobileLogin")]
        public async Task<ApiResultMobileApp<List<UserSessionModel>>> Login([FromBody] UserLoginModel request)
        {
             
            return await Task.Run(async () =>
            {
                var result = new ApiResultMobileApp<List<UserSessionModel>>();
                try
                {

                    SSORest objsso = new SSORest();
                    objsso.SSOId = request.SSO_ID;
                    objsso.SSOPws = request.Password;
                    bool IsValidUser = objsso.SSOAuthenticationMobileNew();
                    string UserDtlJson = objsso.GetUserDetailJSON().ToString();
                    if (IsValidUser == true)
                    {
                        var userEntity = await _unitOfWork.UserRepository.LoginUser(request);

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
                finally
                {
                    _unitOfWork.Dispose();
                }
                return result;
            });
        }


    }
}
