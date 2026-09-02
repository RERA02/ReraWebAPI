namespace ReraModel
{
    public class RequestBaseModel
    {
        public int? DepartmentID { get; set; } = 0;
        public int? Eng_NonEng { get; set; } = 0;// course type
        public int? EndTermID { get; set; } = 0;
        public int? TermPart { get; set; } = 0;
        public int? FinancialYearID { get; set; } = 0;
        public int? IsYearly { get; set; } = 0;

    }
}
