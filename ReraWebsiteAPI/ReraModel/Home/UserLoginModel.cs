using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ReraModel.UserLoginModel
{
    public class UserLoginModel
    {
        [Required]
        public string SSO_ID { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string version { get; set; }
        //public string deviceId { get; set; }


    }

    public class UserRoleModel
    {
        [Required]
        public int UserId { get; set; }


    }

    public class UserDashboardSearchModel
    {
        [Required]
        public string SSO_ID { get; set; }
    }

    public class SSOUserDetail
    {
        public string SSOID { get; set; }
        public string aadhaarId { get; set; }
        public string bhamashahId { get; set; }
        public string bhamashahMemberId { get; set; }
        public string displayName { get; set; }
        public string dateOfBirth { get; set; }
        public DateTime? DateOfBirthParsed
        {
            get
            {
                if (DateTime.TryParseExact(dateOfBirth, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime dob))
                {
                    return dob;
                }
                return null;
            }
        }
        public string gender { get; set; }
        public string mobile { get; set; }
        public string telephoneNumber { get; set; }
        public string ipPhone { get; set; }
        public string mailPersonal { get; set; }
        public string postalAddress { get; set; }
        public string postalCode { get; set; }
        public string l { get; set; }
        public string st { get; set; }
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
    }


    public class SSO_UserSearchModel
    {
        public string SSOID { get; set; }
        public string Password { get; set; }
        public string DeviceID { get; set; }
    }

    public class SSOUserDetailsModel
    {
        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int ApplicationID { get; set; }
        public int ApplicationFinalSubmit { get; set; }
        public string SSOID { get; set; }
        public string AadhaarID { get; set; }
        public string BhamashahId { get; set; }
        public string BhamashahMemberId { get; set; }
        public string DisplayName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Mobileno { get; set; }
        public string TelephoneNumber { get; set; }
        public string IpPhone { get; set; }
        public string MailPersonal { get; set; }
        public string PostalAddress { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Photo { get; set; }
        public string Designation { get; set; }
        public string DepartmentName { get; set; }
        public string MailOfficial { get; set; }
        public string EmployeeNumber { get; set; }
        public int DepartmentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[]? SldSSOIDs { get; set; }
        public string JanaadhaarId { get; set; }
        public string ManaadhaarMemberId { get; set; }
        public string UserType { get; set; }
        public string Mfa { get; set; }
        public int InstituteID { get; set; }
        public int StudentID { get; set; }
        public string? InstituteName { get; set; }
        public int Eng_NonEng { get; set; }//1 for eng 2 for non eng
        public string? Eng_NonEngName { get; set; }
        public int FinancialYearID { get; set; }
        public int EndTermID { get; set; }
        public int FinancialYearID_Session { get; set; }// working session
        public int EndTermID_Session { get; set; }// working session
        public int HostelID { get; set; }// working session
        public string HostelIDs { get; set; }

        public string? UserRequestStatus { get; set; }
        public string? LoginStatus { get; set; }
        public bool? IsCitizenQueryUser { get; set; }
        public int? QueryType { get; set; }
        public int StaffID { get; set; }
        public int ExamScheme { get; set; }

        public string Encrypted_SSOID { get; set; }
        public string SearchRecordID { get; set; }
    }
}
