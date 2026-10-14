
using System.Net;

namespace ReraModel.CommonFunction
{
    public class CommonDDLModel
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Unit { get; set; }
        public string? Quantity { get; set; }
        public int TypeID { get; set; }
        public string? EventName { get; set; }
        public string? CounselorName { get; set; }
        public string? sessionDate { get; set; }

        public string? startTime { get; set; }
        public string? endTime { get; set; }
        public int? SessionId { get; set; }
        public string? Description { get; set; }
        public string? types { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? Age { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public string? Qualification_ENG { get; set; }
        public string? Gender { get; set; }
        public string? DistrictEn { get; set; }
        public string? RecommendationDate { get; set; }
        public string? RecommendationStatus { get; set; }
        public string? JobseekerName { get; set; }
        public string? MobileNumber { get; set; }
        public string? JobDescription { get; set; }
    }

    public class SmsResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseId { get; set; }
        public string Response { get; set; }
    }

    public class SendSms
    {
        public string UniqueID { get; set; }
        public string serviceName { get; set; }
        public string language { get; set; }
        public string message { get; set; }
        public List<string> mobileNo { get; set; }
        public string templateID { get; set; }
        public string entityID { get; set; }
    }

    public class EmailTemplate
    {
        public string Subject { get; set; }
        public string Body { get; set; } // This will contain the HTML
    }


    public class CommonAddressDataModel
    {
        public int ID { get; set; }
        public string? LGCODE { get; set; }
        public string? CODE { get; set; }
        public string? Name_ENG { get; set; }
        public string? Name_MANGAL { get; set; }
        public int? CityId { get; set; }
    }

    public class CommonDistrictModel
    {
        public int DISTRICT_ID { get; set; }
        public string? DISTRICT_ENG { get; set; }
        public int DIVISION_ID { get; set; }
        public string? Status { get; set; }
        public int DISTRICT_CODE { get; set; }
    }

    public class ActiveTabModel
    {
        public string? ActiveTab { get; set; }
        public string? TabName { get; set; }
        public bool[]? ActiveTabAList { get; set; }
    }

    public class StreamDDL_InstituteWiseModel
    {
        public int InstituteID { get; set; }
        public int ApplicationID { get; set; }
        public string? action { get; set; }
        public int StreamType { get; set; }
    }

    public class DateSettingConfigModel
    {
        public int DepartmentID { get; set; }
        public int CourseTypeId { get; set; }
        public int AcademicYearID { get; set; }
        public int EndtermID { get; set; }
        public string? Key { get; set; }
    }

    public class SessionConfigModel
    {
        public int FinancialYearID { get; set; }
        public string? FinancialYearName { get; set; }
        public int ActiveStatus { get; set; }
        public int CreatedBy { get; set; }
        public string? IPAddress { get; set; }
        public Boolean IsCurrentFY { get; set; }
        public int DepartmentID { get; set; }
        public int CourseTypeID { get; set; }
        public string? Action { get; set; }
        public string? EndTermName { get; set; }
    }
    public class IPAddressModel
    {
        public string? IPv6_Address { get; set; } = "";
        public string? IPv4_Address { get; set; } = "";
    }
    public class CommonActEstablishmentModel
    {
        public int Range_ID { get; set; }
        public int MinEmployees { get; set; }
        public int MaxEmployees { get; set; }
        public string? ActEstablishment { get; set; }
    }

    public class UserRequestApprovalModel
    {
        public string? RequestType { get; set; }
        public int RoleId { get; set; }
        public string? Remarks { get; set; }
        public string? SSOId { get; set; }
        public int UserId { get; set; }
    }

    public class CommonDdl
    {
        public string? ActionName { get; set; }

    }

    public class CommonAppliedStatus
    {
        public string? ActionName { get; set; }

    }

    public class PdfRequest
    {
        public int UserId { get; set; }
        //public string PdfBase64 { get; set; }
        //public string HtmlContent { get; set; }
        //public string Password { get; set; } = "1234"; // default if needed
    }
    // Models
    public class HtmlPdfRequest
    {
        public int UserId { get; set; }
        //public string Html { get; set; }  // outerHTML or full document string
        public string PdfBase64 { get; set; }
    }
    public class SendSMSLogPrintModel
    {
        public string? SMSCategoryType { get; set; }
        public string? TemplateID { get; set; }
        public string? MobileNo { get; set; }
        public string? Request_Status { get; set; }
        public string? RequestJson { get; set; }
        public string? ResponseJson { get; set; }
        public string? TextSMS { get; set; }
        public int EventId { get; set; }


    }

    public class JobseekerSearchCouncellorDataModel
    {
        public string? CounselorName { get; set; }
        public string? Location { get; set; }
        public int? PrimaryDomainExpertiseID { get; set; }
        public string?  FilterDate { get; set; }
    }


    public class NotificationsReadModel
    {

        public int CreatedBy { get; set; }
    
        public List<NotificationModelList> NotificationList { get; set; }


    }


    public class NotificationModelList
    {        
        public int NotificationId { get; set; }
        public int JobPostId { get; set; }


    }

}
