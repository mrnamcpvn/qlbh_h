using API.DTOs.Report;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_Report
    {
        Task<Report_Data> GetData(ReportParam param);
       Task<OperationResult> Excel(ReportParam param);
    }
}