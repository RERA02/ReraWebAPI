using ReraCore.Helper;
using ReraModel.CommonFunction;
using ReraModel.DashboardDepartmentModel;
using ReraModel.DashboardPromoterModel;
using ReraModel.HomeModel;
using ReraModel.UserLoginModel;
using System.Data;



namespace ReraCore.Interfaces
{
    public interface IUserRepository
    {

        Task<DataTable> GetAllUser(GenericPaginationSpecification specification);
        Task<DataTable> LoginUser(UserLoginModel model);

        Task<DataTable> ValidateSSOUserDetail(UserLoginModel model);

        Task<DataTable> UserRole(UserRoleModel model);

        Task<DataTable> DashboardPromoter(DashboardPromoterModel model);
        Task<DataSet> DownloadProjectDocuments(DownloadProjectDocumentsModel model);

        Task<DataTable> DownloadProjectDocumentsNew(DownloadProjectDocumentsModel model);

        Task<DataTable> DashboardAgent(DashboardAgentModel model);

        Task<DataTable> DashboardAllottee(DashboardAllotteeModel model);
        Task<DataTable> DashboardDepartment(DashboardDepartmentModel model);

        Task<DataTable> GetModuleMaster(ModuleMastersModel model);

        Task<DataTable> DepartmentPendencyData(DepartmentPendencyDataModel model);


        Task<DataTable> GetAllProjectMasterForDashboardGrid(PromoterDashboardGridModel model);
        Task<DataTable> GETAuditStatus(ProjectAuditModel model);

        Task<DataTable> ProjectSearchData(ProjectSearchDataModel model);

        Task<DataTable> ProjectsByPromoterId(ProjectSearchPromoterModel model);
        

        Task<DataTable> GetDetailsbyProjectId(string ProjectId);

        Task<DataTable> GETModuleList(ModuleMasterModel model);

        Task<DataTable> GETForwardTo(ForwardToModel model);
        Task<int> SaveServiceRequest(Service_RequestModel model);

        Task<int> SaveLatLong(LatLongModel model);

        Task<int> SaveComment(SaveCommentModel model);



        Task<DataSet> FillProjectSummaryMasterModel(ProjectSummaryModel model);

        Task<DataTable> GetUserWiseDashBoardDetails(UserDashboardSearchModel model);
        Task<DataTable> GetDashBoardDetails(string Action = "");

        Task<SSOUserDetailsModel> MobileLogin(SSO_UserSearchModel model);  // new added 18022026

        Task<DataTable> GetITServiceRequest(GetITServiceRequestModel model); // New Add 23032026 Pawan

        Task<int> UpdateQPR_Payment_DTLS(LatLongModelUpdateQPR model); 
        Task<int> UpdateQPR_Payment_DTLSNew(LatLongModelUpdateQPR model);

        
        
        Task<DataTable> GetProjectEndPointsProjectIdWise(GetProjectEndPointsProjectIdWiseModel  model);
        Task<DataTable> GET_ReraProgressReport();
        Task<int>PostreraProgressReport( int total_projects_registered, int residential_projects_registered, int dwelling_units, int carpet_area_lakh_sqmtr, int agents_registered, int cases_filed, int cases_disposed, int state_code, string date);
        Task<int> updateResponseStatus(int Id,string response);
    }
}
