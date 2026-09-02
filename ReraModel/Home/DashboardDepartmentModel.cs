using System.ComponentModel.DataAnnotations;

namespace ReraModel.DashboardDepartmentModel
{

    public class DashboardDepartmentModel
    {

        [Required]
        public string SSOId { get; set; }

    }

    public class ModuleMastersModel
    {

        [Required]
        public int UserId { get; set; }

    }

    public class DepartmentPendencyDataModel
    {

        [Required]
        public string SSOId { get; set; }

        public string PendingStatus { get; set; }

        public string Module { get; set; }



    }

    public class ProjectSearchDataModel
    {
        [Required]
        public string ApplcationNo { get; set; }

        public string ProjectName { get; set; }


    }

    public class ProjectSearchPromoterModel
    {
        [Required]
        public int PromoterId { get; set; }
    }
    

    public class ProjectSearchbyProjectIdModel
    {
        [Required]
        public int ProjectId { get; set; }
    }

    public class LatLongModel
    {
        [Required]
        public int ProjectId { get; set; }
        public int ModuleId { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public int userId { get; set; }

        public int latLongId { get; set; }
        public string total_area { get; set; }
        public string CaptureImage { get; set; }
    }


    public class UploadFileWithPathDataModel
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Dis_FileName { get; set; }
        public string? FolderName { get; set; }

    }



    public class GetITServiceRequestModel
    {
        public int? projectid { get; set; }
        public string? FromExtension { get; set; }
        public string? ProjectName { get; set; }
        public int? SubjectName { get; set; }
        public int? TaskAssign { get; set; }
        public int? TaskRequestByAssign { get; set; }
        public int? Status { get; set; }
        public int? UserId { get; set; }
        public int? RequestBy { get; set; }


    }


    public class LatLongModelUpdateQPR
    {
        public int userId { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ModuleId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? CaptureImage1 { get; set; }
        public string? CaptureImage2 { get; set; }
        public string? WallsImage { get; set; }
        public string? TerraceImage { get; set; }
        public string? CommonAreaImage { get; set; }
        public int QuarterId { get; set; }


    }

    public class GetProjectEndPointsProjectIdWiseModel
    {
        public int ProjectId { get; set; }
    }

}
