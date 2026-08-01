using API.DTOs.Maintain;

namespace API._Services.Interfaces
{
    public interface I_Dashboard
    {
        Task<DashboardSummaryDTO> GetSummary(string filterType = "month");
        Task<DashboardSummaryDTO> GetThuChiSummary(string filterType = "month");
        Task<List<OverdueDonHangDTO>> GetOverdueOrders();
    }
}
