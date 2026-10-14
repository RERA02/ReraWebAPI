using Microsoft.Extensions.Configuration;

namespace ReraCore.Helper
{
    public static class ConfigurationHelper
    {
        private static IConfiguration _configuration;
        private static string _rootPath;
        public static void Configure(IConfiguration configuration, string rootPath)
        {
            _configuration = configuration;
            _rootPath = rootPath;
        }

        public static string EnvironmentName => _configuration["SSOLanding:EnvironmentName"];

        public static string ConnectionString => _configuration.GetConnectionString("DbConnection");
        public static string JwtIssuer => _configuration["Jwt:Issuer"];
        public static string JwtAudience => _configuration["Jwt:Audience"];
        public static string JwtSecret => _configuration["Jwt:SecurityKey"];
        public static int SessionTime => int.Parse(_configuration["SessionTime"] ?? "0");
        public static string JwtAuthority => _configuration["Jwt:Authority"];
        public static string SE_ApiKey => "WiC9J1e89jLUNxf3A85Ywl9e2P0kCLbZlBgsDAiL9gM=";
        public static string SE_Username => "developeradmin";
        public static string SE_Password => "P@ssw0rd123#";
        public static string EgrassPaymentSuccessUrl => _configuration["Egrass-Payment-SuccessUrl"];
        public static string RootPath => $"{_rootPath}/";
        public static string StaticFileRootPath => $"{_rootPath}/StaticFiles/";
        public static string StaticTemplateRootPath => $"{_rootPath}/Report/Template/";
        public static string SSOBaseUrl => _configuration["SSOLanding:SSOBaseUrl"];
        public static string AppName => _configuration["AppName"];

        public static string BaseURL => _configuration["BaseURL"];
        public static string WebServiceUser => _configuration["SSOLanding:WebServiceUser"];
        public static string WebServicePwd => _configuration["SSOLanding:WebServicePwd"];
        public static string ApplicationURL => _configuration["SSOLanding:ApplicationURL"];
        public static string APIURL => _configuration["APIURL"];
        public static string SSOTokenDetailUrl => _configuration["SSOLanding:SSOTokenDetailUrl"];
        public static string SSOProfileDetailUrl => _configuration["SSOLanding:SSOProfileDetailUrl"];
        public static string SSOLogoutUrl => _configuration["SSOLanding:SSOLogoutUrl"];
        public static string SSOBackToUrl => _configuration["SSOLanding:SSOBackToUrl"];
        public static string SSOIncreaseSessionTimeUrl => _configuration["SSOLanding:SSOIncreaseSessionTimeUrl"];

        public static readonly string AadharAuthLicenseKey = "MDAW2xSJCuC-htWkTHcw9LTYXs1rl8kyfygbLztYldZocGJR6DDtE5A";//"MKmyGwbThaYG35Ahinwx35nLtBYXrNMP4ejWD7A9-x6InP7y4xLROXU";

        public static bool IsLocal => Convert.ToBoolean(_configuration["IsLocal"] ?? "false");


        public static readonly string ClientId_EEMS2 = "254eced2ee1bfc019d3a09dc4ef8e8ac";
       // public static string BaseURL => _configuration["BaseURL"] ?? "";
        public static string SMTPHost => _configuration["SMTP:SMTPHost"] ?? "";
        public static int SMTPPort => Convert.ToInt32(_configuration["SMTP:SMTPPort"] ?? "0");
        public static string SMTPEmail => _configuration["SMTP:SMTPEmail"] ?? "";
        public static string SMTPUsername => _configuration["SMTP:SMTPUsername"] ?? "";
        public static string SMTPPassword => _configuration["SMTP:SMTPPassword"] ?? "";
        public static bool IsLiveServer => Convert.ToBoolean(_configuration["SMTP:IsLiveServer"] ?? "true");
        public static bool EnableSsl => Convert.ToBoolean(_configuration["SMTP:EnableSsl"] ?? "false");
        public static bool UseDefaultCredentials => Convert.ToBoolean(_configuration["SMTP:UseDefaultCredentials"] ?? "false");
       // public static bool IsLocal => Convert.ToBoolean(_configuration["IsLocal"] ?? "false");

        #region Configuration

        public static readonly string AadharAuthClientId = "0c8fe990-3b61-406d-b185-0fafc80e73d7";
        public static readonly string AadharAuthSUBAUS = "PRCIS22869"; //"PNSCL22866";
        public static readonly string AadhaarDSMDETOKANIZEV2URL = "https://api.sewadwaar.rajasthan.gov.in/app/live/Aadhaar/Prod/detokenizeV2/doitAadhaar/encDec/demo/hsm/auth";
        public static readonly string SendOTPForAadharAuthURL = "https://api.sewadwaar.rajasthan.gov.in/app/live/was/otp/request/prod";
        public static readonly string VerifyOTPForAadharAuthURL = "https://api.sewadwaar.rajasthan.gov.in/app/live/was/kyc/otp/prod";
        public static readonly string AadharAuthEsignClientId = "02a79073-c2db-4c84-b810-885fdaa8c54a";

        public static readonly string VerifyOTPForAadharAuthEsignURL = "https://api.sewadwaar.rajasthan.gov.in/app/live/RajeSign/Prod/all/webresources/generic/authOTP/Aadhar";
        public static readonly string EsignPDFURL = "https://api.sewadwaar.rajasthan.gov.in/app/live/rajesign/Prod/Service/all/webresources/generic/esign/Doc";
        public static readonly string Certificatename = "uidai_auth_prod.cer";
        public static readonly string SecretKeyJanAadhar = "aRY8bGDm6dByd8QB";

        public static readonly string SendJanAadharOTP = "https://api.sewadwaar.rajasthan.gov.in/app/live/RajeSign/Prod/all/webresources/generic/sendOTP/Aadhar?client_id=f6de7747-60d3-4cf0-a0ae-71488abd6e95";

        public static readonly string GetStateNodalCountURL = "https://employment.livelihoods.rajasthan.gov.in/API/EEMSAppService.asmx/GetTotRegAgegrp";
        public static readonly string SendEsignOTP = "https://apitest.sewadwaar.rajasthan.gov.in/app/live/rajesignfly_v2/webresources/uat/genericsendAadhar";
        public static readonly string ESignSignURL = "https://apitest.sewadwaar.rajasthan.gov.in/app/live/rajesignfly_v2/webresources/uat/genericsendAadhar";
        public static readonly string ESancharMessageRequest = "https://apitest.sewadwaar.rajasthan.gov.in/app/live/eSanchar/api/OBD";

        #endregion
        //public static readonly string AadharAuthLicenseKey = "MJSazxO49Eh5vQ2BlcbUG--uNQ4tCpqKPF-OoFa0BZo0CE4CDBBtXVA"; 
        //" ";

        #region eSign
        //public static readonly string SendOTPForAadharAuthEsignURL = "https://api.sewadwaar.rajasthan.gov.in/app/live/RajeSign/Prod/all/webresources/generic/sendOTP/Aadhar";

        public static readonly string SendOTPForAadharAuthEsignURL = "https://apitest.sewadwaar.rajasthan.gov.in/app/live/RajeSign/Staging/bulkesign/webresources/generic/sendOTP/Aadhar";
        #endregion


        #region Jan Aadhar Configuration
        //public static readonly string JanAadhaarMemberPhotoClientID = "0cbe6b41-c050-44a5-a3b7-3ec37f27b624";
        //public static readonly string JanAadharAuthClientId = "f6de7747-60d3-4cf0-a0ae-71488abd6e95";

        public static readonly string SecretKeyJanAadharMemberList = "q4Ts4gDjbIjufekL";
        public static readonly string SecretKeyJanAadharMemberDetails = "aRY8bGDm6dByd8QB";
        public static readonly string secretKeyJanaadhaarMemberPhoto = "dTcVC3Fx2jqHRdyG";


        //public static readonly string JaAdharMemberListURL = "https://api.sewadwaar.rajasthan.gov.in/app/live/Janaadhaar/Prod/Service/action/fetchJayFamily";
        public static readonly string JaAdharMemberListURL = "https://janapp.rajasthan.gov.in/Service/action/encrptyApiInputGenric";
        public static readonly string JaAdharMemberListURL_Staging = "http://172.21.245.45/Service/action/encrptyApiInputGenric";

        //public static readonly string JaAdharMemberDetailsURL = "https://api.sewadwaar.rajasthan.gov.in/app/live/Janaadhaar/Prod/Service/Info/Fetch";
        public static readonly string JaAdharMemberDetailsURL = "https://janapp.rajasthan.gov.in/Service/action/encrptyApiInputGenric";
        public static readonly string JaAdharMemberDetailsURL_Staging = "http://172.21.245.45/Service/action/encrptyApiInputGenric";

        //public static readonly string JanAadhaarMemberPhotoURL = "https://api.sewadwaar.rajasthan.gov.in/app/live/Janaadhaar/Prod/Service/details/hofAndMemberPhoto/";
        public static readonly string JanAadhaarMemberPhotoURL = "https://janapp.rajasthan.gov.in/Service/action/encrptyApiInputGenric";
        public static readonly string JanAadhaarMemberPhotoURL_Staging = "http://172.21.245.45/Service/action/encrptyApiInputGenric";

        public static readonly string JanAdhaar_SendValidateOTP_ClientID = "b10d20ae-367c-4cab-8b14-49330d4a392d";
        public static readonly string JanAdhaar_SendOTP_URL = "https://api.sewadwaar.rajasthan.gov.in/app/live/Janaadhaar/Prod/Service/Gen/Otp/For?client_id=";
        public static readonly string JanAdhaar_ValidateOTP_URL = "https://api.sewadwaar.rajasthan.gov.in/app/live/Janaadhaar/Prod/Service/Validate/Otp?client_id=";
        public static readonly string JanAdharMemberUrlNew = "https://apitest.sewadwaar.rajasthan.gov.in/app/live/apiservice/janAadhaar/v1/member-list?client_id=";
        public static readonly string JanAdharGenerateOTPUrlNew = "https://apitest.sewadwaar.rajasthan.gov.in/app/live/apiservice/janAadhaar/v1/generate-otp?client_id=";
        public static readonly string JanAdharVerifyOTPUrlNew = "https://apitest.sewadwaar.rajasthan.gov.in/app/live/apiservice/janAadhaar/v1/validate-otp?client_id=";
        #endregion
    }
}




