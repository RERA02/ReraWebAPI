using Newtonsoft.Json;
using RERA.ViewModel.Common;
using ReraCore.Helper;
using System.Net;
using System.Text;

namespace RJ_RERA_API.Core.Helper
{
    public class SsoServiceHelper
    {

        #region SSO Landing
        public static ApiResultMobileApp<SSO_TokenDetailModel> GetSSOTokenDetails(string ssoToken)
        {
            string SSOTokenDetailUrl = ConfigurationHelper.SSOTokenDetailUrl;
            var ssoTD = new SSO_TokenDetailModel();
            var isVaildToken = false;

            var result = new ApiResultMobileApp<SSO_TokenDetailModel>();

            var responseString = Task.Run(() =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, SSOTokenDetailUrl);
                request.Headers.Add("SSO-TOKEN", ssoToken);
                request.Headers.Add("Authorization", "Basic " +
                Convert.ToBase64String(Encoding.Default.GetBytes(ConfigurationHelper.WebServiceUser + ":" + ConfigurationHelper.WebServicePwd)));
                var response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }).Result;

            if (responseString != null && !responseString.Contains("User Login Id or Password is incorrect"))
            {
                isVaildToken = true;
                ssoTD = JsonConvert.DeserializeObject<SSO_TokenDetailModel>(responseString);
            }

            result.Status = isVaildToken ? EnumStatusMA.Success : EnumStatusMA.Error;
            result.Data = ssoTD;

            return result;
        }

        public static SSO_UserProfileDetailModel GetSSOUserProfileDetail(string ssoid, string ssoToken)
        {
            string SSOProfileDetailUrl = ConfigurationHelper.SSOProfileDetailUrl;
            var ssoUD = new SSO_UserProfileDetailModel();

            var responseString = Task.Run(() =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{SSOProfileDetailUrl}/{ssoid}");
                request.Headers.Add("SSO-TOKEN", ssoToken);
                request.Headers.Add("Authorization", "Basic " +
                Convert.ToBase64String(Encoding.Default.GetBytes(ConfigurationHelper.WebServiceUser + ":" + ConfigurationHelper.WebServicePwd)));
                var response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }).Result;

            ssoUD = JsonConvert.DeserializeObject<SSO_UserProfileDetailModel>(responseString);
            return ssoUD;
        }
        public static bool IncreaseSessionTimeSSO(string ssoToken)
        {
            string SSOLogoutUrl = ConfigurationHelper.SSOLogoutUrl;
            var responseString = Task.Run(() =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{SSOLogoutUrl}");
                request.Headers.Add("SSO-TOKEN", ssoToken);
                request.Headers.Add("Authorization", "Basic " +
                Convert.ToBase64String(Encoding.Default.GetBytes(ConfigurationHelper.WebServiceUser + ":" + ConfigurationHelper.WebServicePwd)));
                var response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }).Result;

            if (responseString == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void LogoutSSO(string ssoToken)
        {
            string SSOLogoutUrl = ConfigurationHelper.SSOLogoutUrl;
            var responseString = Task.Run(() =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{SSOLogoutUrl}");
                request.Headers.Add("SSO-TOKEN", ssoToken);
                request.Headers.Add("Authorization", "Basic " +
                Convert.ToBase64String(Encoding.Default.GetBytes(ConfigurationHelper.WebServiceUser + ":" + ConfigurationHelper.WebServicePwd)));
                var response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }).Result;
        }
        public static void BackToSSO(string ssoToken)
        {
            string SSOBackToUrl = ConfigurationHelper.SSOBackToUrl;
            var responseString = Task.Run(() =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{SSOBackToUrl}");
                request.Headers.Add("SSO-TOKEN", ssoToken);
                request.Headers.Add("Authorization", "Basic " +
                Convert.ToBase64String(Encoding.Default.GetBytes(ConfigurationHelper.WebServiceUser + ":" + ConfigurationHelper.WebServicePwd)));
                var response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }).Result;

        }
        public static SSO_UserProfileDetailModel GetSSOUserDetail(string ssoid)
        {
            string SSOProfileDetailUrl = "https://ssotest.rajasthan.gov.in/SSO/GetUserDetails/";
            var ssoUD = new SSO_UserProfileDetailModel();

            var responseString = Task.Run(() =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{SSOProfileDetailUrl}/{ssoid}");
                request.Headers.Add("Authorization", "Basic " +
                Convert.ToBase64String(Encoding.Default.GetBytes(ConfigurationHelper.WebServiceUser + ":" + ConfigurationHelper.WebServicePwd)));
                var response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }).Result;

            ssoUD = JsonConvert.DeserializeObject<SSO_UserProfileDetailModel>(responseString);
            return ssoUD;
        }
        #endregion
    }
}
