using ReraWebPortal.Code.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using ReraCore.Entities;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraModel;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReraWebPortal.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase, IDisposable
    {
        private bool disposedValue;

        public abstract string PageName { get; }
        public abstract string ActionName { get; set; }

        public BaseController()
        {
        }
        public CustomPrincipal CustomPrincipal => new CustomPrincipal(User);

        #region Common Function
        //protected async Task<string> CreateAuthentication(UserSessionModel model)
        //{
        //    if (model != null)
        //    {
        //        return await GenrateJwtToken(model);
        //    }
        //    return null;
        //}
        //protected async Task<string> GenrateJwtToken(UserSessionModel model)
        //{
        //    return await Task.Run(() =>
        //    {
        //        var authClaim = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.NameIdentifier,model.UserID.ToString()),
        //            new Claim(ClaimTypes.Name,model.UserName),
        //            new Claim(ClaimTypes.Email,model.Email),
        //            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

        //            new Claim(nameof(model.RoleIDs),model.RoleIDs),
        //            new Claim(nameof(model.RoleNames),model.RoleNames),
        //        };

        //        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.JwtSecret));

        //        var token = new JwtSecurityToken(
        //            issuer: ConfigurationHelper.JwtIssuer,
        //            audience: ConfigurationHelper.JwtAudience,
        //            expires: DateTime.Now.AddMinutes(ConfigurationHelper.SessionTime),
        //            claims: authClaim,
        //            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //            );

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    });
        //}
        protected async Task CreateErrorLog(NewException nex, IUnitOfWork unitOfWork)
        {
            await Task.Run(async () =>
            {
                var data = new Tbl_Trn_ErrorLog();
                if (nex.Ex.Message.Contains("@SqlExecutableQuery = "))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Error = {nex.Ex.Message}");
                    sb.AppendLine($"Page = {nex.PageName}.{nex.ActionName}");
                    data.ErrorDescription = sb.ToString();
                    data.ErrorMessage = nex.Ex.Message;
                    data.ErrorLocation = $"Page = {nex.PageName}.{nex.ActionName}";
                    data.LineNumber = $"Line Number: {new StackTrace(nex.Ex, true)?.GetFrame(0)?.GetFileLineNumber()}";
                    data.StackTrace = $"{nex.Ex.ToString()}, Stacktrace : {nex.Ex.StackTrace ?? ""}, Line Number: {new StackTrace(nex.Ex, true)?.GetFrame(0)?.GetFileLineNumber()}";
                    data.ErrorGeneratedFrom = "Stored procedure";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Error = {nex.Ex.Message}");
                    sb.AppendLine($"Page = {nex.PageName}.{nex.ActionName}");
                    data.ErrorDescription = sb.ToString();
                    data.ErrorMessage = nex.Ex.Message;
                    data.ErrorLocation = $"Page = {nex.PageName}.{nex.ActionName}";
                    data.LineNumber = $"Line Number: {new StackTrace(nex.Ex, true)?.GetFrame(0)?.GetFileLineNumber()}";
                    data.StackTrace = $"{nex.Ex.ToString()}, Stacktrace : {nex.Ex.StackTrace ?? ""}, Line Number: {new StackTrace(nex.Ex, true)?.GetFrame(0)?.GetFileLineNumber()}";
                    data.ErrorGeneratedFrom = "Web";
                }
                await unitOfWork.ErrorLogs.AddErrorLog(data);
                unitOfWork.SaveChanges();
            });
        }
        protected async Task CreateErrorLog1(NewException nex, IUnitOfWork unitOfWork)
        {
            try
            {
                var data = new Tbl_Trn_ErrorLog();

                // --- FIX 1: Safe Exception Message ---
                string safeMessage = nex.Ex?.Message ?? "No exception message";
                string safeStack = nex.Ex?.StackTrace ?? "";
                int lineNum = 0;

                if (nex.Ex != null)
                {
                    var frame = new StackTrace(nex.Ex, true)?.GetFrame(0);
                    if (frame != null)
                        lineNum = frame.GetFileLineNumber();
                }

                // --- FIX 2: Build Description Safely ---
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Error = {safeMessage}");
                sb.AppendLine($"Page = {nex.PageName}.{nex.ActionName}");

                data.ErrorDescription = sb.ToString();
                data.ErrorMessage = safeMessage;
                data.ErrorLocation = $"Page = {nex.PageName}.{nex.ActionName}";
                data.LineNumber = $"Line Number: {lineNum}";
                data.StackTrace = $"{nex.Ex?.ToString() ?? ""}, Stacktrace : {safeStack}, Line Number: {lineNum}";
                data.ErrorGeneratedFrom = (nex.Ex?.Message?.Contains("@SqlExecutableQuery = ") == true)
                    ? "Stored procedure"
                    : "Web";

                // --- FIX 3: Ensure unitOfWork is not null ---
                if (unitOfWork?.ErrorLogs == null)
                    return;

                await unitOfWork.ErrorLogs.AddErrorLog(data);

                unitOfWork.SaveChanges();
            }
            catch
            {
                // DO NOT THROW - Logger should NEVER crash
            }
        }

        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        #endregion


    }
}