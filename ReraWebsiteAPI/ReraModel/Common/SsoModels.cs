
namespace RERA.ViewModel.Common
{
  
    public class SSO_TokenDetailModel
    {
        public string sAMAccountName { get; set; }//ssoid
        public List<string> Roles { get; set; }
        public string OldSSOIDs { get; set; }
        public string UserType { get; set; }
        public string DelegateBy { get; set; }
    }
    public class SSO_UserProfileDetailModel
    {
        public string SSOID { get; set; }
        public string aadhaarId { get; set; }
        public string bhamashahId { get; set; }
        public string bhamashahMemberId { get; set; }
        public string displayName { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public string mobile { get; set; }
        public string telephoneNumber { get; set; }
        public string ipPhone { get; set; }
        public string mailPersonal { get; set; }
        public string postalAddress { get; set; }
        public string postalCode { get; set; }
        public string l { get; set; }//city
        public string st { get; set; }//state
        public string jpegPhoto { get; set; }
        public string designation { get; set; }
        public string department { get; set; }
        public string mailOfficial { get; set; }
        public string employeeNumber { get; set; }
        public string departmentId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string oldSSOIDs { get; set; }
        public string janaadhaarId { get; set; }
        public string janaadhaarMemberId { get; set; }
        public string userType { get; set; }
        public string mfa { get; set; }
        public string sansthaAadhaar { get; set; }
        public string KIOSKCODE { get; set; } = string.Empty;
        public string SERVICEID { get; set; } = string.Empty;
        public string SSoToken { get; set; } = string.Empty;
        public string EmitraDepartmentID { get; set; } = string.Empty;
        public bool IsKiosk { get; set; } = false;


    }


    public class SSOResponseModel
    {
        public string? Status { get; set; }
        public string? IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}
