using ReraModel.CommonFunction;
using ReraModel.HomeModel;
using System.Data;



namespace ReraCore.Interfaces
{
    public interface IHomeRepository
    {

        Task<List<LatestJudgementVM>> GetLatestJudgements(int recCount);
        Task<List<DashboardVM>> GetHomePageCount();
        Task<DataTable> GetProjects(SearchProjectRequestModel searchParams);
        Task<DataTable> GetDeflauterlist(SearchProjectRequestModel searchParams);
        Task<DataTable> GetProjectsListForOuterSearchNew(ProjectSearchParams searchParams, string ProjectOrExemption);
        Task<DataTable> GetPublicPageProjectDetailsAsync(string action, int projectId);

        Task<int> ContactUs(QueryVM model);
        Task<int> GetVisitorCount(string key);

        Task<DataTable> GetComplaintsAgainstRespondentList(GetComplaintsAgainstRespondenSearch model);
        Task<DataTable> GetAgentsList(SearchAgentRequestModel searchParams);
        Task<DataSet> GetComplaintFullDetails(ComplaintSearchRequestModel model);

        Task<DataTable> GetWeb_TBL_AUDITTRAIL(Web_TBL_AUDITTRAILSearchModel model);
        Task<DataTable> GetWeb_GetAllNotice(Web_GetAllNoticeSearModel model);
        Task<DataSet> GetProjectById(int projectId);

        Task<DataTable> TblComplaintDetails(ComplaintDetailsS_M model);
        Task<ComplaintPDFVM> GetcomplaintsDetailsForPDFnew(ComplaintsDetailsS_M model);

        Task<DataTable> GetComplaintDocumentUploadsByComplaintNumber(ComplaintDocumentUploadsS_M model);
        Task<DataTable> GetComplainantList(ComplaintdetailsSearchModel model);
        Task<DataTable> GetUploadedDocumentsSP(GetUploadedDocumentsSPModel model);

        Task<DataTable> GetUpdatedDailyCauseListbyparameter();
        Task<DataTable> GetViolationActCompliants(GetViolationActCompliantsModel model);
        Task<DataTable> Get_ProjectStatus(int? ProjectID);

        Task<int> SaveComplaintBox(ComplaintBoxVM model);
        void AddVisitorCount(string key);

        Task<DataTable> Get_ViewReplyResponse(string ComplaintNumber);

        Task<DataTable> GetWeb_TBL_Rejoinder(Web_TBL_RejoinderSearchModel model);
        Task<DataTable> GetComplaintDetailsWebsite(ComplaintdetailsSearchModel model);

        Task<DataTable> GetProjectIdWiseEndPointsLocation(int? ProjectID);

        Task<DataTable> GetAllNotificationsbyUser(NotificationsModel model);
        Task<bool> UpdateNotificationStatus(NotificationsStatusModel model);

        Task<DataTable> GetprojectDetailsbyDepecryValue(int? ProjectID);
        Task<DataTable> GetDepecryValueCommon(string type, int? ProjectID);

        //Task<DataTable> ExecuteQuery(string query);

        //Task<List<string>> GetStoredProcedures(string searchText);

        //Task<string> GetProcedureDefinition(string procedureName);
    }
}
