namespace API.DTOs.Report
{
    public class Report
    {
        public int ID_NLD { get; set; }
        public string NLD { get; set; }
        public DateTime Date { get; set; }
        public string DateView { get; set; }
        public int? MH_ID { get; set; }
        public string MH_Name { get; set; }
        public int CD_ID { get; set; }
        public string CD_Name { get; set; }
        public int Quantity { get; set; }
        public decimal Money { get; set; }
        public decimal Total_Money { get => Money * Quantity; }
        public string Total_Money_View { get => Total_Money.ToString("#,##"); }
    }

    public class ReportParam
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int  ID_NLD { get; set; }
    }
}