using ReraModel.CommonFunction;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;

namespace ReraCore.Helper
{
    // use here all the common things except functions  
    public static class CommonHelper
    {

    }

    #region Constants
    public class Constants
    {
        // pagination
        public const int MAX_PAGE_SIZE = 100;

        // common message
        public const string MSG_UNAUTHORIZED_ACCESS = "Unauthorized access!";
        public const string MSG_UNAUTHORIZED_ACCESS_FOR_ROLE = "Unauthorized access for the role!";
        public const string MSG_FILE_NOT_FOUND = "File not found!";
        public const string MSG_DATA_NOT_FOUND = "Data not found!";
        public const string MSG_VALIDATION_FAILED = "Validation failed!";
        public const string MSG_DATA_LOAD_SUCCESS = "Data load successfully.";
        public const string MSG_SAVE_SUCCESS = "Saved successfully.";
        public const string MSG_SAVE_Duplicate = "Duplicate entry.";
        public const string MSG_INVALID_REQUEST = "Invalid Request!";

        public const string MSG_APPROVE_SUCCESS = "User Approved successfully.";
        public const string MSG_APPROVE_ERROR = "There was an error Approved User!";
        public const string MSG_UPDATE_SUCCESS = "Updated successfully.";
        public const string MSG_DELETE_SUCCESS = "Deleted successfully.";
        public const string MSG_ADD_ERROR = "There was an error adding data!";
        public const string MSG_UPDATE_ERROR = "There was an error updating data!";
        public const string MSG_DELETE_ERROR = "There was an error deleting data!";
        public const string MSG_NO_DATA_SAVE = "No Data to Save!";
        public const string MSG_NO_DATA_UPDATE = "No Data to Update!";
        public const string MSG_NO_DATA_DELETE = "No Data to Delete!";
        public const string MSG_ERROR_OCCURRED = "Error occurred!";
        public const string MSG_RECORD_ALREADY_EXISTS = "Record already exists!";
        public const string MSG_RECORD_ALREADY_REGISTERED = "Already registered for this event!";
        public const string MSG_RECORD_recommendation = "Your job recommendation has been successfully sent to the jobseeker!";


        // regex
        public const string RGX_MOBILE_NO = @"^\d{10}$";
        public const string RGX_FAX_NO = @"^\+?[0-9]{6}$";
        public const string RGX_WEBSITE = @"^((https?|ftp|smtp):\/\/)?(www.)?[a-z0-9]+(\.[a-z]{2,}){1,3}(#?\/?[a-zA-Z0-9#]+)*\/?(\?[a-zA-Z0-9-_]+=[a-zA-Z0-9-%]+&?)?$";
        public const string RGX_LANDLINE_STD_NO = @"^[0][1-9]{2,3}(-)[0-9]{8}";
        public const string RGX_PINCODE = @"^[1-9][0-9]{5}$";
        public const string RGX_AADHAR_NO = @"^[2-9]{1}[0-9]{11}$";
        public const string RGX_PAN_CARD = @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$";


        // uploaded folder path in static folder 
        public const string LogFolder = "Log";
        public const string MobileAppId = "comm.RajEmployment.app";
        public const string PlacementCompanyFolder = "PlacementCompany";
        public const string StudentsFolder = "Students";
        public const string StaffMemberFolder = "Staff";
        public const string ReportsFolder = "Reports";
        public const string RDLCFolderBTER = "Report/Files/BTER";
        public const string RDLCFolderITI = "Report/Files/ITI";
        public const string AllotmentReciept = "Reports/AllotmentReciept";
        public const string AadharCertificate = "/StaticFiles/Certificate";
        public const string EsignedPdfFolder = "/EsignedPdf";
        public const string TokenSecretKey = "YN72v8totmj2RiXWkZTMJ61WEMhsCDrrEHGKxpqBRKM=";
        public const string TokenSSOID = "MONIKAC.DOIT";
        public const string JANAADHAR_ENC_KEY = "aRY8bGDm6dByd8QB"; //ENCRYPTION KEY
        public const string clientId = "ClientId";
        //Aadhaar esign auth keys
        public const string AADHAAR_ESIGN_AUTH_ENC_KEY = "O0HP3OOR1DP2KZY752TJDO9DRHURSOX1"; //ENCRYPTION KEY
        public const string AADHAAR_ESIGN_AUTH_APP_CODE = "PRAVL22868ABFAP012";  //APP CODE
        public const string AADHAAR_ESIGN_AUTH_URL = "https://aadhaarauthtest.rajasthan.gov.in/AadhaarAuth";  //Aadhaar Auth URL
        public const string AADHAAR_ESIGN_AUTH_RETURN_URL = "http://10.68.231.168/rajsahakarapp/AadhaarEsignAuth/AadharAuthLanding";  //Return URL
        public const string SEWADWAAR_CLIENT_ID = "02a79073-c2db-4c84-b810-885fdaa8c54a";  //Sewadwaar Client ID
        public const string AADHAAR_ESIGN_SINGLE_MODE_URL = $"https://apitest.sewadwaar.rajasthan.gov.in/app/live/rajesign/Staging/Service/all/webresources/generic/esign/Doc?client_id={SEWADWAAR_CLIENT_ID}";
        public const string AADHAAR_GENERIC_ESIGN_URL = $"https://apitest.sewadwaar.rajasthan.gov.in/app/live/RajeSign/UAT/webresources/generic/eSignedocument?client_id={SEWADWAAR_CLIENT_ID}";
        public const string AADHAAR_GENERIC_MULTIPLE_ESIGN_PAGE_URL = $"https://apitest.sewadwaar.rajasthan.gov.in/app/live/RajeSign/UAT/webresources/generic/multipleEsignPage?client_id={SEWADWAAR_CLIENT_ID}";
        public const string AADHAAR_PAGE_WISE_ESIGNED_DOCUMENT_URL = $"https://apitest.sewadwaar.rajasthan.gov.in/app/live/RajeSign/UAT/webresources/generic/pageWiseeSignedocument?client_id={SEWADWAAR_CLIENT_ID}";
        public const string AADHAAR_BULK_ESIGNED_DOCUMENT_URL = $"https://apitest.sewadwaar.rajasthan.gov.in/app/live/RajeSign/Staging/bulkesign/webresources/generic/bulk/document?client_id={SEWADWAAR_CLIENT_ID}";
        public const string AADHAAR_BULK_PAGE_WISE_DOCUMENT_URL = $"https://apitest.sewadwaar.rajasthan.gov.in/app/live/RajeSign/Staging/bulkesign/webresources/generic/multiple/page?client_id={SEWADWAAR_CLIENT_ID}";


        #region esign
        public static string EsignClientCode
        {
            get
            {
                return ConfigurationHelper.IsLocal ? "EEMS2UAT" : "EEMS2UAT";
            }
        }
        public static string EsignClientId
        {
            get
            {
                return ConfigurationHelper.IsLocal ? "02a79073-c2db-4c84-b810-885fdaa8c54a" : "02a79073-c2db-4c84-b810-885fdaa8c54a";
            }
        }
        public static string ASPID
        {
            get
            {
                return ConfigurationHelper.IsLocal ? "RISL" : "RISL";
            }
        }
        public static string GetToken
        {
            get
            {
                return ConfigurationHelper.IsLocal ? "https://rajesignapitest.rajasthan.gov.in/caEsign/auth/generateToken" : "https://rajesignapitest.rajasthan.gov.in/caEsign/auth/generateToken";
            }
        }
        public static string GetSignedXMLUrl
        {
            get
            {
                return ConfigurationHelper.IsLocal ? "https://rajesignapitest.rajasthan.gov.in/caEsign/v2/generateSignedXmlV2_1" : "https://rajesignapitest.rajasthan.gov.in/caEsign/v2/generateSignedXmlV2_1";
                // return ConfigurationHelper.IsLocal ? "https://apitest.sewadwaar.rajasthan.gov.in/app/live/rajEsign/rajApi/esignApi/GetSignedXML" : "https://api.sewadwaar.rajasthan.gov.in/app/live/rajesign/rajApi/esignApi/GetSignedXML";
            }
        }
        public static string GetSignedPDFUrl
        {
            get
            {
                return ConfigurationHelper.IsLocal ? "https://rajesignapitest.rajasthan.gov.in/caEsign/v2/signingPdfV2_1" : "https://rajesignapitest.rajasthan.gov.in/caEsign/v2/signingPdfV2_1";
                // return ConfigurationHelper.IsLocal ? "https://apitest.sewadwaar.rajasthan.gov.in/app/live/rajEsign/rajApi/esignApi/GetSignedPDF" : "https://api.sewadwaar.rajasthan.gov.in/app/live/rajesign/rajApi/esignApi/GetSignedPDF";
            }
        }
        #endregion

        //readonly
        /// <summary>
        /// Here value(int) is TypeID (Configuration Type ID), ex: 1 for Admission
        /// </summary>
        public static readonly ReadOnlyCollection<RestrictedUrlModel> RESTRICTED_URLS =
            new ReadOnlyCollection<RestrictedUrlModel>(new List<RestrictedUrlModel>
            {
                new RestrictedUrlModel{Url = "Test/AdmissionDateTest", Message = "Sorry! Admission date is over on {0}.", TypeID = 1 },
                new RestrictedUrlModel{Url = "StudentJanAadharDetail/SendJanaadharOTP", Message = "", TypeID = 1 },
                new RestrictedUrlModel{Url = "StudentJanAadharDetail/SavePersonalData", Message = "Sorry! Your admission date is over on {0}.", TypeID = 1 },
                new RestrictedUrlModel{Url = "EmitraPaymentService/EmitraApplicationPayment", Message = "Sorry! Your fee payment date is over on {0}.", TypeID = 14 },
                new RestrictedUrlModel{Url = "EmitraPaymentService/EmitraApplicationPayment", Message = "Sorry! Your fee payment date is over on {0}.", TypeID = 15 }
            });

    }
    #endregion


    #region classe
    public class GenericPaginationSpecification
    {
        private int _pageNumber = 1;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                _pageNumber = value <= 0 ? _pageNumber : value;
            }

        }

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value <= 0 ? _pageSize : value > Constants.MAX_PAGE_SIZE ? Constants.MAX_PAGE_SIZE : value;
            }
        }
        public string OrderBy { get; set; }
    }
    public class PagigantionList
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageRecord { get; set; }
        public int TotalRecord { get; set; }
        public int TotalPagination { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }


    public class ApiResultMobileApp<T>
    {
        public EnumStatusMA Status { get; set; }
        public EnumStatusApi StatusApi { get; set; }
        public string Message { get; set; }
        public string? ErrorMessage { get; set; }
        public T Data { get; set; }
    }

    public class ApiResult<T>
    {
        public EnumStatus State { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string? ErrorMessage { get; set; }
        public T Data { get; set; }
    }
    public class NewException
    {
        public string PageName { get; set; }
        public string ActionName { get; set; }
        public Exception Ex { get; set; }        
    }
    public class ErrorDescription
    {
        public string Message { get; set; }
        public string PageName { get; set; }
        public string ActionName { get; set; }
        public string SqlExecutableQuery { get; set; }
        public string InnerExceptionMessage { get; set; }
    }
    public class UploadFileModel
    {
        public string? FolderName { get; set; }
        public IFormFile file { get; set; }
        public string? FileExtention { get; set; }
        public string? MinFileSize { get; set; }
        public string? MaxFileSize { get; set; }
        public string? Password { get; set; }
    }
    #endregion

    #region enums
    public enum EnumStatus
    {
        Success = 1,
        Error = 2,
        Warning = 3,

        
    }
    public enum EnumRole
    {
        Guest = 0,
        Developer = 1,
        Admin = 2,//bter admin eng
        Admin_NonEng = 12,//bter admin
        AdminNodel = 5,
        TPO = 6,
        Principal = 7,
        Principal_NonEng = 13,
        Student = 3,
        Invigilator = 11,
        DTETraing = 16,
        ITIPrincipal = 20,
        Emitra = 4,
        Teacher = 8,
        DTE_Eng = 17,
        DTE_NonEng = 18,
        ITIAdmin_SCVT = 22,
        ITIAdmin_NCVT = 42,
        Principal_NCVT = 43
    }


    public enum EnumStatusMA
    {
        Failed,
        Success,
        Logout,
        Error
    }

    public enum EnumStatusApi
    {
        Success = 1,
        Error = 2,
        Warning = 3,


    }
    public enum EnumRoleMA
    {
        Admin = 1,
        Guest = 2
    }

    public enum EnmPaymentGatway
    {
        RPP = 1
    }
    public enum EnmPaymetRequest
    {
        PaymentRequest = 1,
        RefundRequest = 2
    }
    public enum EnumExamStudentStatus
    {
        Enrolled = 8,
        New_Enrolled = 9,
        SelectedForExamination = 10,
        SubmittedExamStudentStatus = 11,
        EnrolledFeePaid = 12,
        EligibleForExamination = 13,
        NewEligibleForExamination = 14,
        RejectatBTER = 15,
        ExaminationFeesPaid = 32,
        BackFeePaid = 33,
        VerifiedForExamination = 34,
        SelectedForEnrollment = 35,
        Addimited = 36,
        VerifiedForEnrollment = 37,
        EligibleForEnrollment = 53,
        New_Addimited = 139,
        ApprovalReject = 161,
        Dropout = 4,
        RevokeDropout = 209
    }


    public enum EnumPdfType
    { 
    
       RollList=1,
       AdmitCard=2,
       TimeTable=3

    }
    public enum EnumRollAdmitStatus
    {
        Genrated = 11,
        Forward = 12,
        Verify = 13,
        Publish = 14,
        Revert = 15,
    }



    
    #endregion

    public class ConstentsMP
    {
        public const int MAX_PAGE_SIZE = 100;
        public const string MSG_ERROR_OCCURRED = "Error occurred!";
        public const string MSG_FILE_NOT_FOUND = "File not found!";
        public const string MSG_Save_Success = "File Save Successfully.";
        public const string MSG_DATA_NOT_FOUND = "Data not found!";
        public const string MSG_VALIDATION_FAILED = "Validation failed!";
        public const string MSG_SHG_VALIDATION_Success = "Account No Validation Successfully!";
        public const string MSG_GET_DATA_SUCCESS = "Data fetched sucessfully";
        public const string MSG_SESSION_OUT = "Session Timed Out, Please login again";
        public const string MSG_INVALID_REQUEST = "Invalid Request, Please try again";
        public const string MSG_ALREADY_LOGIN = "This user is already logged in";
        public const string MSG_LOGIN_Successfully = "User Login Successfully.";
        public const string MSG_LOGIN_UnSuccessfully = "User Login UNSuccessfully.";
        public const string MSG_conetct_HO = "There are some network error, please try again";
        public const string MSG_DATA_SAVE = "Data Save successfully";
        public const string MSG_Ivalid_Ceredinatial = "Insert valid UserID and Password";
        public const string MSG_RESTMPIN = "Member MPIN Updated successfully";
        public const string MSG_REST_DATA_NOT_FOUND = "Reset MPIN Data not found!";
        public const string MSG_DBService = "Data base service was down. plese try again";
        public const string MSG_APPService = "There are Server Issue.Please try again";
        public const string MSG_InterError = "An internal error occurred. please try again later.";
        public const string MSG_Feedback_submit = "Feedback Submitted successfully.";
        public const string MSG_Feedback_UNSave = "UnSave successfully.";
        public const string MSG_DataSave_Success = "Data has already been saved successfully.";
        public const string MSG_UPDATE_SUCCESS = "Updated successfully.";
    }
}
