using System.ComponentModel.DataAnnotations;

namespace ReraModel.DashboardPromoterModel
{

    public class DashboardPromoterModel
    {

        [Required]
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class DownloadProjectDocumentsModel
    {
        public int ProjectId { get; set; }
        public int Type { get; set; }
    }



    public class DashboardAgentModel
    {

        [Required]
        public int UserId { get; set; }
        public string Modulename { get; set; }

    }

    public class DashboardAllotteeModel
    {

        [Required]
        public int UserId { get; set; }
        public string Modulename { get; set; }

    }

    public class PromoterDashboardGridModel
    {

        [Required]
        public int UserId { get; set; }
        public string Status { get; set; }

        public string Module { get; set; }


    }

    public class ProjectSummaryModel
    {

        [Required]
        public int projectId { get; set; }
        public int type { get; set; }

        public int UserId { get; set; }



    }

    public class ProjectAuditModel
    {
        [Required]
        public int projectid { get; set; }

        public int TableId { get; set; }

        public int Typefor { get; set; }
    }
    public class ModuleMasterModel
    {
        public string MasterType { get; set; }
    }

    public class ForwardToModel
    {
        public int UserId { get; set; }
        public int moduleid { get; set; }
    }

    public class Service_RequestModel
    {
        public int ID { get; set; }
        public int Userid { get; set; }
        public int PROJECTID { get; set; }
        public int ModuleID { get; set; }
        public string Contact_Email { get; set; }
        public string ContactNo { get; set; }
        public string AttachFile { get; set; }
        public string Request_Details { get; set; }
        public int RequestBy { get; set; }
        public bool? Request_Resolved { get; set; }
        public string TicketNo { get; set; }
        public string Extension_Number { get; set; }
        public int? AssignTaskTo { get; set; }
        public string Comment { get; set; }
        public string SR_Response { get; set; }

        public int ISACTIVE { get; set; }
        public string ProjectName { get; set; }

        public string ApplicationNo { get; set; }

        public string Subject { get; set; }

        public string ServiceRequestNo { get; set; }

    }

    public class SaveCommentModel
    {
        public int ID { get; set; }
        public int Userid { get; set; }

        public int ModuleID { get; set; }

        public string comments { get; set; }

        public string uploadfile { get; set; }

        public int ForwardTo { get; set; }

    }

    public class sendSms
    {
        public string UniqueID { get; set; }
        public string serviceName { get; set; }
        public string language { get; set; }
        public string message { get; set; }
        //  public string mobileNo { get; set; }
        public List<string> mobileNo { get; set; }
        public string templateID { get; set; }
    }

    public class SentMessage
    {
        public int UserId { get; set; }
        public string ToNumber { get; set; }
        public string MsgBody { get; set; }
        public bool IsSent { get; set; }
        public DateTime SentOn { get; set; }
        public string Responce { get; set; }

    }


}
