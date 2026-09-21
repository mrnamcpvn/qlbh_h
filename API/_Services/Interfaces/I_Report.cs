using API.DTOs.Report;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_Report
    {
        Task<Report_Data> GetData(ReportParam param);
        Task<OperationResult> Excel(ReportParam param);
    }
}