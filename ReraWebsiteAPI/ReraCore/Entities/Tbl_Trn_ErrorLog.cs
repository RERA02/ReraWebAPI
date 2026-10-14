namespace ReraCore.Entities
{
    public class Tbl_Trn_ErrorLog
    {
        public string? ErrorMessage { get; set; }
        public string? ErrorDescription { get; set; }
        public string? ErrorLocation { get; set; }
        public string? LineNumber { get; set; }
        public string? StackTrace { get; set; }
        public string? ErrorGeneratedFrom { get; set; }

    }
}
