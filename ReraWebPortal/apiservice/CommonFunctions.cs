using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ReraMobileApi.Api.apiservice
{
    public class CommonFunctions
    {

        #region SSORestAPI Integration
        public class AES
        {
            public string ConvertToBase64(string planText)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(planText));
            }
            private string ConvertFromBase64(string base64String)
            {
                return
               Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
            }
            private RijndaelManaged GetRijndaelManaged(string secretKey)
            {
                var keyBytes = new byte[16];
                var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length,
               secretKeyBytes.Length));
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
            public string Decrypt(string cipherText, string encryptionKey)
            {
                if (!string.IsNullOrEmpty(cipherText))
                {
                    var encryptedBytes = Convert.FromBase64String(cipherText);
                    var cipher = GetRijndaelManaged(encryptionKey);
                    return
                   Encoding.UTF8.GetString(cipher.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public class SSORest
        {
            string serviceURL = string.Empty;
            string serviceSignOutURL = string.Empty;
            string servicebckToSSOURL = string.Empty;
            string ServiceReq = string.Empty;
            SSOResponce ServiceRes;
            public string SSOId = string.Empty;
            public string SSOPws = string.Empty;
            const string EncKey = "R@j$S0@02{09}19#";
            public string SSOToken = string.Empty;  // New Added 03122024
            AES objAES = new AES();
            public SSORest()
            {
                //if (HttpContext.Current.Request.Url.Host.ToLower().Contains("rajkisan.rajasthan.gov.in"))
                //{
                //    serviceURL = "https://sso.rajasthan.gov.in:4443/";
                //    servicebckToSSOURL = "https://sso.rajasthan.gov.in/sso";
                //    serviceSignOutURL = "https://sso.rajasthan.gov.in/signout";
                //}
                //else
                //{
                //    serviceURL = "https://ssotest.rajasthan.gov.in:4443/";
                //    servicebckToSSOURL = "https://ssotest.rajasthan.gov.in/sso";
                //    serviceSignOutURL = "https://ssotest.rajasthan.gov.in/signout";
                //}

                serviceURL = "https://ssotest.rajasthan.gov.in:4443/";
            }

            public Boolean SSOAuthenticationMobileNew()
            {
                ServiceReq = "{\"Application\":\"rajkisan\",\"UserName\":\"" + this.SSOId + "\",\"Password\":\"" + objAES.Encrypt(this.SSOPws, EncKey) + "\"}";
                string response = WebRequestinJson(serviceURL + "SSOREST/SSOAuthenticationMobileNew", ServiceReq);
                //  ServiceRes = (new JavaScriptSerializer()).Deserialize<SSOResponce>(response);
                ServiceRes = JsonConvert.DeserializeObject<SSOResponce>(response);
                return ServiceRes.valid;
            }

            public string GetUserDetailJSON()
            {
                ServiceReq = "{\"SSOID\":\"" + this.SSOId + "\",\"WSUSERNAME\":\"agriculture.test\",\"WSPASSWORD\":\"" + objAES.ConvertToBase64("Rajasthan@13") + "\"}";
                string response = WebRequestinJson(serviceURL + "SSOREST/GetUserDetailJSON", ServiceReq);
                //ServiceRes = (new JavaScriptSerializer()).Deserialize<SSOResponce>(response);
                ServiceRes = JsonConvert.DeserializeObject<SSOResponce>(response);
                return response;
            }

            public string GetUserDetailJSON_New()
            {
                ServiceReq = "{\"SSOID\":\"" + this.SSOId + "\",\"WSUSERNAME\":\"DMIS.TEST\",\"WSPASSWORD\":\"" + objAES.ConvertToBase64("Rajasthan@27") + "\"}";
                string response = WebRequestinJson(serviceURL + "SSOREST/GetUserDetailJSON", ServiceReq);
                // ServiceRes = (new JavaScriptSerializer()).Deserialize<SSOResponce>(response);
                ServiceRes = JsonConvert.DeserializeObject<SSOResponce>(response);
                return response;
            }

            #region Mahesh 04122024
            public async Task<bool> IncreaseSessionTime()
            {
                bool result = false;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, serviceURL + "/SSORESTNEW/IncreaseSessionTime"))
                    {
                        request.Headers.Add("SSO-TOKEN", this.SSOToken);
                        request.Headers.Add("Authorization", "Basic " +
                            Convert.ToBase64String(Encoding.Default.GetBytes("agriculture.test" + ":" + "Rajasthan@13")));

                        try
                        {
                            using (var response = await client.SendAsync(request))
                            {
                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var resString = await response.Content.ReadAsStringAsync();
                                    if (resString.Equals("true", StringComparison.OrdinalIgnoreCase))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                        }
                    }
                }
                return result;
            }
            public async void BackToSSO()
            {
                bool result = false;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, servicebckToSSOURL))
                    {
                        request.Headers.Add("SSO-TOKEN", this.SSOToken);
                        request.Headers.Add("Authorization", "Basic " +
                            Convert.ToBase64String(Encoding.Default.GetBytes("agriculture.test" + ":" + "Rajasthan@13")));

                        try
                        {
                            using (var response = await client.SendAsync(request))
                            {
                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var resString = await response.Content.ReadAsStringAsync();
                                    if (resString.Equals("true", StringComparison.OrdinalIgnoreCase))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                        }
                    }
                }
            }
            public async void SignoutToSSO()
            {
                bool result = false;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, serviceSignOutURL))
                    {
                        request.Headers.Add("SSO-TOKEN", this.SSOToken);
                        request.Headers.Add("Authorization", "Basic " +
                            Convert.ToBase64String(Encoding.Default.GetBytes("agriculture.test" + ":" + "Rajasthan@13")));

                        try
                        {
                            using (var response = await client.SendAsync(request))
                            {
                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var resString = await response.Content.ReadAsStringAsync();
                                    if (resString.Equals("true", StringComparison.OrdinalIgnoreCase))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                        }
                    }
                }
            }

            public async Task<TokenDetailResponse> GetTokenDetail()
            {
                TokenDetailResponse responseData = null;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, serviceURL + "/SSORESTNEW/TokenDetail"))
                    {
                        request.Headers.Add("SSO-TOKEN", this.SSOToken);
                        request.Headers.Add("Authorization", "Basic " +
                            Convert.ToBase64String(Encoding.Default.GetBytes("agriculture.test" + ":" + "Rajasthan@13")));

                        try
                        {
                            using (var response = await client.SendAsync(request))
                            {
                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var resString = await response.Content.ReadAsStringAsync();
                                    responseData = JsonConvert.DeserializeObject<TokenDetailResponse>(resString);
                                }
                                else
                                {
                                    responseData = null;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            responseData = null;
                        }
                    }
                }
                return responseData;
            }

            public async Task<SSORestProfileResponse> GetUserProfileDetail()
            {
                SSORestProfileResponse responseData = null;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, serviceURL + "/SSORESTNEW/Profile/" + this.SSOId.ToString()))
                    {
                        request.Headers.Add("SSO-TOKEN", this.SSOToken);
                        request.Headers.Add("Authorization", "Basic " +
                            Convert.ToBase64String(Encoding.Default.GetBytes("agriculture.test" + ":" + "Rajasthan@13")));

                        try
                        {
                            using (var response = await client.SendAsync(request))
                            {
                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var resString = await response.Content.ReadAsStringAsync();
                                    responseData = JsonConvert.DeserializeObject<SSORestProfileResponse>(resString);
                                }
                                else
                                {
                                    responseData = null;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            responseData = null;
                        }
                    }
                }
                return responseData;
            }

            #endregion

            string WebRequestinJson(string URL, string postData)
            {
                try
                {
                    string ret = string.Empty;
                    JObject json = JObject.Parse(postData);
                    StreamWriter requestWriter;

                    var webRequest = System.Net.WebRequest.Create(URL) as HttpWebRequest;
                    if (webRequest != null)
                    {
                        webRequest.Method = "POST";
                        webRequest.ServicePoint.Expect100Continue = false;
                        webRequest.Timeout = 40000;
                        webRequest.ContentType = "application/json";
                        using (requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                        {
                            requestWriter.Write(postData);
                        }
                    }
                    HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
                    Stream resStream = resp.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream);
                    ret = reader.ReadToEnd();
                    return ret;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }
        }

        public class SSOResponce
        {
            public Boolean valid { get; set; }
            public string message { get; set; }
            public string[] roles { get; set; }
            public string displayName { get; set; }
            public string designation { get; set; }
            public string mobile { get; set; }
            public string mailPersonal { get; set; }
            public string aadhaarId { get; set; }
            public string bhamashahId { get; set; }
            public string bhamashahMId { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string oldSSOIDs { get; set; }
            public string janaadhaarId { get; set; }
            public string janaadhaarMId { get; set; }
            public string userType { get; set; }
            public string mfa { get; set; }
            public string SSOID { get; set; }
            public string bhamashahMemberId { get; set; }
            public string dateOfBirth { get; set; }
            public string gender { get; set; }
            public string telephoneNumber { get; set; }
            public string ipPhone { get; set; }
            public string postalAddress { get; set; }
            public string postalCode { get; set; }
            public string department { get; set; }
            public string mailOfficial { get; set; }
            public string employeeNumber { get; set; }
            public string departmentId { get; set; }
            public string jpegPhoto { get; set; }
            public string l { get; set; }
            public string st { get; set; }
        }

        public class TokenDetailResponse
        {
            public string sAMAccountName { get; set; }
            public string[] Roles { get; set; }
            public string OldSSOIDs { get; set; }
            public string UserType { get; set; }
        }


        public class SSORestProfileResponse
        {
            public string SSOID { get; set; }
            public string AadhaarId { get; set; }
            public string BhamashahId { get; set; }
            public string BhamashahMemberId { get; set; }
            public string DisplayName { get; set; }
            public string DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Mobile { get; set; }
            public string TelephoneNumber { get; set; }
            public string IpPhone { get; set; }
            public string MailPersonal { get; set; }
            public string PostalAddress { get; set; }
            public string PostalCode { get; set; }
            public string L { get; set; }
            public string St { get; set; }
            public string JpegPhoto { get; set; }
            public string Designation { get; set; }
            public string Department { get; set; }
            public string MailOfficial { get; set; }
            public string EmployeeNumber { get; set; }
            public string DepartmentId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string OldSSOIDs { get; set; }
            public string JanaadhaarId { get; set; }
            public string JanaadhaarMemberId { get; set; }
            public string UserType { get; set; }
            public int MFA { get; set; }
            public string SansthaAadhaar { get; set; }
        }
        #endregion
    }
}
