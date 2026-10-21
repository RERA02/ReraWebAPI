using System.ComponentModel.DataAnnotations;

namespace ReraModel.HomeModel
{

    public class DashboardVM
    {
        public int ObjectedAGENTRenewal { get; set; }
        public int PendingProjects { get; set; }
        public int PendingProjectExtension { get; set; }
        public int PendingMapRevision { get; set; }

        public int PendingAccountChange4A { get; set; }
        public int PendingBAC { get; set; }
        public int PendingAccountChange4B { get; set; }
        public int PendingEncumbrance { get; set; }

        public int PendingFrmx { get; set; }
        public int PendingFrmr { get; set; }

        public int PendingQPR { get; set; }
        public int PendingAPR { get; set; }
        public int PendingSPM { get; set; }
        public int PendingSPMP { get; set; }
        public int PendingSPMPA { get; set; }
        public int PendingCOMP { get; set; }
        public int PendingAGENTRenewal { get; set; }//AgentRenewal
        public int PendingAGENT { get; set; }
        public int ObjectedProjects { get; set; }
        public int ObjectedProjectExtension { get; set; }
        public int ObjectedMapRevision { get; set; }

        public int ObjectedAccountChange4A { get; set; }
        public int ObjectedAccountChange4B { get; set; }
        public int ObjectedEncumbrance { get; set; }

        public int ObjectedFrmx { get; set; }
        public int ObjectedFrmr { get; set; }

        public int ObjectedQPR { get; set; }
        public int ObjectedAPR { get; set; }

        public int ObjectedSPM { get; set; }
        public int ObjectedSPMP { get; set; }
        public int ObjectedSPMPA { get; set; }
        public int ObjectedCOMP { get; set; }
        public int ObjectedAGENT { get; set; }
        public string? Status { get; set; }
        public int PendingPAA { get; set; }
        public int PendingPAAR { get; set; }
        public int PendingPAAO { get; set; }
        public int PendingPRD { get; set; }

        public int ObjectedPAA { get; set; }
        public int ObjectedPAAR { get; set; }
        public int ObjectedPAAO { get; set; }

        public int ObjectedPRD { get; set; }
        public int ObjectedURR { get; set; }
        public int ObjectedComplaintAct { get; set; }
        public int REVISEDRC { get; set; }
        public int PendingURR { get; set; }
        public int PendingComplaintAct { get; set; }
        public int ProjectApproved { get; set; }
        public int ComplaintNewApproved { get; set; }
        public int ComplaintOldApproved { get; set; }
        public int AgentApproved { get; set; }
        public int PromoterApproved { get; set; }

        public int PendingSuoMoto { get; set; }
        public int ObjectedSuoMoto { get; set; }

        public int PendingExemption { get; set; }
        public int ObjectedExemption { get; set; }

        public int PendingAgentRevocation { get; set; }

        public int ObjectedAgentRevocation { get; set; }

        public int PendingRectificationApp { get; set; }
        public int ObjectedRectificationApp { get; set; }
        public int PendingExecutionApp { get; set; }
        public int ObjectedExecutionApp { get; set; }
        public int PendingprojectRevocation { get; set; }
        public int ObjectedprojectRevocation { get; set; }

        public int PendingMiscellaneousApp { get; set; }
        public int ObjectedMiscellaneousApp { get; set; }
    }

    public class LatestJudgementVM
    {
        public string? DocumentName { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string? ComplaintNumber { get; set; }
        public string? PartyName { get; set; }
    }
    public class SearchModel
    {
        public int UserTypeId { get; set; }
        public int State { get; set; }
        public int DistrictId { get; set; }
        public int DistrictIdAgent { get; set; }
        public int Teshil { get; set; }
        public int TeshilAgent { get; set; }
        public int ProjectId { get; set; }
        public int CertificateNumber { get; set; }
        public int FinYearId { get; set; }
        public int ProjectSubtype { get; set; }
        public int PromoterType { get; set; }
        public int UserprofileID { get; set; }
        public int ProjectType { get; set; }
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
        public int qid { get; set; } //05-05-2021 

        public int AnalyticType { get; set; }

        public int status { get; set; }
        public string? ProjectName { get; set; }
        public string? PromoterName { get; set; }
        public string? AgentName { get; set; }
        public string? RegistrationNo { get; set; }
        public int Annualid { get; set; }

        public List<LatestJudgementVM>? LatestJudgements { get; set; }
    }


    public class HomePageApiVM
    {
        public DashboardVM? Dashboard { get; set; }
        public List<LatestJudgementVM>? LatestJudgements { get; set; }
    }



    public class ProjectSearchParams
    {
        public string? FormD { get; set; } //--21-10-2022
        public string? DailyTDt { get; set; }   //----DailyT
        public int UserId { get; set; }
        public int SrNo { get; set; }
        public int Id { get; set; }
        public int? District { get; set; }
        public int Teshil { get; set; }
        public string? DistrictName { get; set; }
        public string? ProjectName { get; set; }
        public string? PromoterName { get; set; }
        public string? CertificateNo { get; set; }
        public string? EncryptProjectId { get; set; }
        public int ProjectType { get; set; }
        public string? ProjectTypeName { get; set; }
        public DateTime? ApprovedOn { get; set; }
        //10-12-2018
        public DateTime? SendToAdminOn { get; set; }
        public string? ApplicationNo { get; set; }
        public int ApplicationStatus { get; set; }  //ProjectStatus 02-04-2019

        //03-03-2021
        public DateTime? ApprovedOnFrom { get; set; }
        public DateTime? ApprovedOnTo { get; set; }
        public string? RevisedDateofComplation { get; set; }
        public string? RoleDistrict { get; set; }
        public string? RoleModule { get; set; }
        public string? RoleDesgination { get; set; }
        public string? PendingStatus { get; set; }// for chairman dashboard

        public int IsActive { get; set; }
        public DateTime? DateOfComplation { get; set; }
        public int Status { get; set; }
        public int? ProjectStatus { get; set; }
        public string? RegistrationNo { get; set; }

        public string? UploadedCertificatePath { get; set; }
        public int stateid { get; set; }

        public string? WORKFLOWMULTIPLE { get; set; }
        public string? AppStatus { get; set; }
        public int? SOURCE { get; set; }
        public string? RegistrationDate { get; set; }
        public string? RegistrationNoDate { get; set; }

        public string? OriginalCompletionDate { get; set; }

        public string? PresetCompletionDate { get; set; }

        public int NoofExtensions { get; set; }

        public int NoofComplaints { get; set; }

        public string? EncumbranceDetail { get; set; }

        public string? ProjectStatusstr { get; set; }

        public int NoofApartmentBooked { get; set; }
        public string? Name { get; set; }
        public int cnt1 { get; set; }
        public int cnt2 { get; set; }
        public int cnt3 { get; set; }
        public int cnt4 { get; set; }
        public int esignVal { get; set; }
        public int esignProjectVal { get; set; }
        public int esignProfileVal { get; set; }
        public int Special_esignProjectVal { get; set; }
        public int Special_esignProfileVal { get; set; }
        public int RevisedCertiRC { get; set; }
        public string? StatusApporReopen { get; set; }
        public string? DayWiseCount { get; set; }

        public int DayCount { get; set; }

        public string? PType { get; set; }
        public string? PStatus { get; set; }
        public string? UserFullName { get; set; }
        public string? PSendToAdminOn { get; set; }
        public string? SSOID { get; set; }


    }

    public class SearchProjectRequestModel
    {
        public string? ProjectName { get; set; }
        public int ApplicationStatus { get; set; }
        public int DistrictId { get; set; }
        public int TeshilId { get; set; }
        public string? PromoterName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? OrderPassedby { get; set; }
        public string? ApplicationNo { get; set; }
        public int ProjectType { get; set; }

        public DateTime? ApprovedFrom { get; set; }
        public DateTime? ApprovedTo { get; set; }

        public string? RoleDesgination { get; set; }
        public string? RoleModule { get; set; }
        public string? PendingStatus { get; set; }

        public int Source { get; set; }
        public int RevisedCertiRC { get; set; }
        public string? DailyTDt { get; set; }
        public int Status { get; set; }
        public string? ProjectOrExemption { get; set; }
        public string? DayWiseCount { get; set; }

        public string? DistrictName { get; set; }
        public string? UserFullName { get; set; }
        public int Year { get; set; }
    }
    public class PublicPageProjectDtlsVM
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int UserId { get; set; }
        public decimal Area { get; set; }
        public decimal PhaseArea { get; set; }
        public string AggregateAreaOpenSpace { get; set; }
        public int TotalBuildingCount { get; set; }
        public int SanctionedbuildingCount { get; set; }
        public int NotSanctionedbuildingCount { get; set; }
        public string ProjectLocation { get; set; }
        public string ProjectCategory { get; set; }
        public string PromoterName { get; set; }
        public string PromoterType { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime? DateofRegistration { get; set; }
        public DateTime? RevisedDateOfComplation { get; set; }
        public string RcCertificatePath { get; set; }
        public string BankIds { get; set; }
        public string ProjectType { get; set; }
        // Add other fields returned by your SP
    }

    public class QueryVM
    {
        public int ID { get; set; }


        public int QueryType { get; set; }
        public string? QueryText { get; set; }


        public string? FirstName { get; set; }


        public string? LastName { get; set; }


        public string? MobileNo { get; set; }


        public string? EmailAddress { get; set; }


        public string? Query { get; set; }

        public string? QueryTypeName { get; set; }
    }

    public class GetComplaintsAgainstRespondenSearch
    {
        public string Action { get; set; }
        public string ComplaintNumber { get; set; }

    }

    public class SearchAgentRequestModel
    {

        public int districtId { get; set; }
        public int tehsilId { get; set; }

        public string? registrationNo { get; set; }
        public string? PromoterName { get; set; }
        public string? ProjectName { get; set; }
        public string? agentName { get; set; }


    }
    public class ComplaintSearchRequestModel
    {
        public int Type { get; set; }
        public string? ComplaintNumber { get; set; }
        public int ComplaintTypeId { get; set; }
        public string? NewcomplaintfromDate { get; set; }
        public string? NewcomplainttoDate { get; set; }
        public int Respondentid { get; set; }
        public string? ProjectName { get; set; }
        public int ApplicationStatus { get; set; }
    }

    public class Web_TBL_AUDITTRAILSearchModel
    {
        public int Projectid { get; set; }
        public int TableId { get; set; }
        public int Type { get; set; }

    }
    public class Web_GetAllNoticeSearModel
    {
        public string Action { get; set; }
        public string ComplaintNumber { get; set; }
    }

    public class ComplaintDetailsS_M
    {
        public int Id { get; set; }
    }

    public class ComplaintsDetailsS_M
    {
        public string Action { get; set; }
        public int UserId { get; set; }
        public int ComplaintID { get; set; }
    }

    public class ComplaintAdjudicatingPDF
    {
        public int id { get; set; }
        public int actiontype { get; set; }
        public string fileName { get; set; }

    }


    public class ComplaintAdjudicatingOfficerModel
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string ComplaintNumber { get; set; }

        public string ClaimFillingNumber { get; set; }
        public string ClaimNumber { get; set; }

        public string ApplicantName { get; set; }

        public string ApplicantOffice { get; set; }
        public string ApplicantService { get; set; }

        public string ApplicantMobilieNumber { get; set; }

        public string ApplicantEmail { get; set; }

        public string ApplicantAllotment { get; set; }

        public string RespondentName { get; set; }

        public string RespondentOffice { get; set; }
        public string RespondentService { get; set; }

        public string RespondentMobileNumber { get; set; }

        public string RespondentEmail { get; set; }

        public string RespondentRegistrationNumber { get; set; }

        public string RespondentProjectAddress { get; set; }

        public string Jurisdication { get; set; }

        public string FactOfCase { get; set; }

        public string Grounds { get; set; }

        public string ReliefSought { get; set; }

        public string NotPendingMatter { get; set; }


        public string Amount { get; set; }
        public string BankName { get; set; }
        public string BankerChequeNumber { get; set; }
        public string DetailsOfPayment { get; set; }

        public string AuthenicatedCopyUrl { get; set; }
        public string DocumentsCopiesUrl { get; set; }
        public string OtherDocumentsUrl { get; set; }
        public int? IndexOfDocuments { get; set; }


        public string FullName { get; set; }

        public string FatherName { get; set; }

        public int? Year { get; set; }
        public string Residanse { get; set; }

        public string Place { get; set; }
        public DateTime FillingDate { get; set; }

        public string ViewType { get; set; }
        public string PreForm { get; set; }
        public string Listofenclosures { get; set; }
        public string IterimOrderDetails { get; set; }
        public string ComplaintPendingWithOtherCourtDetails { get; set; }
        public bool? PaymentStatus { get; set; }
        public string ApplicantResidenceOffice { get; set; }
        public string AdvocateMobileNo { get; set; }
        public string AdvocateName { get; set; }

        public string AdvocateEmail { get; set; }
        public string CAMobileNo { get; set; }
        public string CAName { get; set; }

        public string CAEmail { get; set; }
        public int DistrictId { get; set; }
        public string RERARespondentService { get; set; }
        public string TempComplaintNo { get; set; }

        public string MultipleComplainantName { get; set; }
        public string MultipleRespondantName { get; set; }

    }
    public class ComplaintPDFVM
    {
        public int Id { get; set; }

        public string ProjectName { get; set; }
        public string ApplicationNo { get; set; }
        public string ComplaintNumber { get; set; }
        public string ComplainantName { get; set; }
        public string ExecutionApplicantName { get; set; }
        public string ApplicantName { get; set; }
        public string NonapplicantName1 { get; set; }
        public string NonapplicantName2 { get; set; }
        public string NonapplicantName3 { get; set; }
        public string NonapplicantName4 { get; set; }
        public string NonapplicantName5 { get; set; }
        public string NonapplicantName6 { get; set; }
        public string NonapplicantName7 { get; set; }
        public string NonapplicantName8 { get; set; }
        public string NonapplicantName9 { get; set; }
        public string NonapplicantName10 { get; set; }

        public string ComplainantAdd_AddressLine1 { get; set; }
        public string ComplainantAdd_AddressLine2 { get; set; }
        public string ComplainantAdd_State { get; set; }
        public string ComplainantAdd_District { get; set; }
        public string ComplainantAdd_Tehsil { get; set; }
        public string ComplainantAdd_Pincode { get; set; }
        public string ComplainantAdd_EmailID { get; set; }
        public string ComplainantAdd_MobileNo { get; set; }
        public string RespondentAdd_AddressLine1 { get; set; }
        public string RespondentAdd_AddressLine2 { get; set; }
        public string RespondentAdd_State { get; set; }
        public string RespondentAdd_District { get; set; }
        public string RespondentAdd_Tehsil { get; set; }
        public string RespondentAdd_Pincode { get; set; }
        public string RespondentAdd_EmailID { get; set; }
        public string RespondentAdd_MobileNo { get; set; }

        public string RespondentName { get; set; }
        public string ComplainantOffice { get; set; }
        public string ComplainantResidenceOffice { get; set; }  //21-11-2019
        public string ComplainantService { get; set; }
        public string ComplainantMobilieNumber { get; set; }
        public string ComplainantEmail { get; set; }
        public string RespondentService { get; set; }
        public string RespondentOffice { get; set; }
        public string RespondentMobileNumber { get; set; }
        public string ComplainantMobileNo { get; set; }
        public string RespondentEmail { get; set; }
        public string FactOfCase { get; set; }
        public string Grounds { get; set; }
        public string ReliefSought { get; set; }
        public bool JurisdictionofRERA { get; set; }   //26-11-2019
        public bool InterimOrder { get; set; }
        public bool NotPendingMatter { get; set; }
        public decimal Amount { get; set; }
        public int PaymentStatus { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        // public string Year              { get; set; }
        public int? Age { get; set; }
        public string Residanse { get; set; }
        public string Place { get; set; }
        public string FillingDate { get; set; }
        public string IterimOrderDetails { get; set; }   //03-12-2019
        public string ComplaintPendingWithOtherCourtDetails { get; set; }  //03-12-2019
        public string Listofenclosures { get; set; }   //05-12-2019


        public bool? Limitation { get; set; }   //05-12-2019
        public string LimitationDetails { get; set; }   //09-12-2019
        public string FactOfCaseUnderSection { get; set; }   //09-12-2019
        public string FactOfCaseYear { get; set; }   //09-12-2019
        public string FactOfCaseAct { get; set; }   //09-12-2019
        public string RegistrationNo { get; set; }   //09-12-2019
        public string ApplicantAllotment { get; set; }   //13-12-2019

        public string TempComplaintNo { get; set; }//10-01-2020
        public string RERACaseDateofDecision { get; set; } //12-02-2020
        public string RERACaseNumber { get; set; } //12-02-2020

        public string CitizenName { get; set; }


        public string resAddress1 { get; set; }
        public string resAddress2 { get; set; }
        public string resdistrict { get; set; }
        public string resTaluka { get; set; }
        public string resState { get; set; }
        public int resPincode { get; set; }


        public string compAddress1 { get; set; }
        public string compAddress2 { get; set; }
        public string compdistrict { get; set; }
        public string compTaluka { get; set; }
        public string compState { get; set; }
        public int compPincode { get; set; }
        public DateTime? HearingDate { get; set; }

        public string HearingTime { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string DepartentComment { get; set; }

        public string MobileNo1 { get; set; }

        public string MobileNo2 { get; set; }



        public int UserRoleId { get; set; }
        public string PromoterComplaintFilePath { get; set; }

        public string AdvocateMobileNo { get; set; }
        public string AdvocateName { get; set; }

        public string AdvocateEmail { get; set; }
        public string CAMobileNo { get; set; }
        public string CAName { get; set; }

        public string CAEmail { get; set; }

        public string Fileno { get; set; }

        public int Fileno2 { get; set; }

        public string FormXFileNo { get; set; }
        public DateTime? impugnedOrderRecallDate { get; set; }
        public string AddressTypeTo { get; set; }

        public string RespondentNameAlt { get; set; }

        public string copyOfImpugnedOrder { get; set; }

        public string formR5 { get; set; }

        public string docInSupportOfApplication { get; set; }

        public int RespondentTypeId { get; set; }
        public int PromoterId { get; set; }
        public string PromoterAddress { get; set; }

        public string PromoterEmail { get; set; }
        public string PromoterMobileNo { get; set; }

        public string PromoterName { get; set; }

        public int IsShowCauseNoticeIssued { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string RERARespondentService { get; set; }

        public string formsubmitted { get; set; }
        public string MultipleComplainantName { get; set; }
        public string MultipleRespondantName { get; set; }

        public int complaintTypeID { get; set; }

        public string Registered { get; set; }


    }

    public class SendEmailVM
    {
        public string ApplicationNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string RejectResion { get; set; }
        public string Amount { get; set; }
        public string PaymentId { get; set; }
        public string ComplaintNumber { get; set; }
        public string ComplaintKey { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string FilePath { get; set; }
        public string SecondFilePath { get; set; }
        public string CC { get; set; }
        public string PedingForHearingDate { get; set; }
        public string PedingForHearingTime { get; set; }
        public string PedingForHearingPlace { get; set; }
        public string PedingForHearingDocuments { get; set; }
    }

    public class GetProjectByIdRequest
    {
        public string? ProjectId { get; set; }
    }

    public class ComplaintDocumentUploadsS_M
    {
        public string ComplaintNumber { get; set; }
    }

    public class ComplaintdetailsSearchModel
    {
        public string? complaint_status { get; set; }
        public int? ComplaintTypeId { get; set; }
        public string? compalaint_no { get; set; }
        public string? respondent_name { get; set; }
        public string? complainant { get; set; }
        public string? actionType { get; set; }

    }

    public class GetUploadedDocumentsSPModel
    {
        public int id { get; set; }
        public string? complientNo { get; set; }
        public string? PartyName { get; set; }
        public string? OrderfromDate { get; set; }
        public string? OrdertoDate { get; set; }
        public string? DocumentName { get; set; }
        public string? Commontext { get; set; }
        public int FinYearId { get; set; }
        public int causeListType { get; set; }
        public string? OrderNumber { get; set; }



    }

    public class DocumentModel
    {
        public int Id { get; set; }


    }

    public class GetViolationActCompliantsModel
    {
        public string? Action { get; set; }
        public string? Compalaintno { get; set; }
    }


    public class ComplaintBoxVM
    {
        public int ID { get; set; }


        public bool? IsIdentityDisclosed { get; set; }


        public string? ComplainantName { get; set; }

        public string? ComplainantMobile { get; set; }


        public string? ComplainantEmail { get; set; }


        public string? ComplainantAddress { get; set; }


        public string? ProjectName { get; set; }


        public string? ProjectAddress { get; set; }


        public string? DistrictName { get; set; }


        public string? Location { get; set; }


        public string? ComplaintDescription { get; set; }

        public string? PromoterName { get; set; }

        public string? PromoterAddress { get; set; }

        public string? PromoterContactNumber { get; set; }

        public string? UploadedDocument { get; set; }
        public string? CreatedBy { get; set; } = "Visitor";
    }

    public class Web_TBL_RejoinderSearchModel
    {
        public string? ComplaintNumber { get; set; }

    }

    public class ComplaintDetailsModel
    {
        public string? complaint_status { get; set; }
        public string? compalaint_no { get; set; }
        public string? complainant { get; set; }
        public string? respondent_name { get; set; }
        public int? ComplaintTypeId { get; set; }
    }

    public class SearchProjectEndPointsModel
    {

        public int ProjectId { get; set; }



    }

    public class NotificationsModel
    {
        public int UserId { get; set; }
        public string SSOId { get; set; }
        public string DeviceId { get; set; }
        public int Id { get; set; }

    }
    public class NotificationsStatusModel
    {
        public int UpdatedBy { get; set; }
        public int Id { get; set; }

    }

    public class ProjectRequest
    {
        public string ProjectId { get; set; }
    }
    public class ProjectRequestComm
    {
        public string type { get; set; }
        public string ProjectId { get; set; }
    }


}
