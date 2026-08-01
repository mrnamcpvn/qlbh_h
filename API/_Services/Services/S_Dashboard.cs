using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using Microsoft.EntityFrameworkCore;

namespace API._Services.Services
{
    public class S_Dashboard : I_Dashboard
    {
        private readonly IRepositoryAccessor _repo;

        public S_Dashboard(IRepositoryAccessor repo)
        {
            _repo = repo;
        }

        private (DateTime startCur, DateTime endCur, DateTime startPrev, DateTime endPrev, string filterPeriodName, string prevPeriodName) ComputeDateRange(string filterType, DateTime now, DateTime today)
        {
            switch (filterType)
            {
                case "day":
                    return (today, today.AddDays(1), today.AddDays(-1), today,
                        $"Hôm nay ({today:dd/MM/yyyy})", "Hôm qua");

                case "week":
                    int dayOfWeek = (int)today.DayOfWeek;
                    int diffToMon = dayOfWeek == 0 ? -6 : 1 - dayOfWeek;
                    var mon = today.AddDays(diffToMon);
                    return (mon, mon.AddDays(7), mon.AddDays(-7), mon,
                        $"Tuần này ({mon:dd/MM} - {mon.AddDays(6):dd/MM})", "Tuần trước");

                case "quarter":
                    int q = (now.Month - 1) / 3 + 1;
                    var qStart = new DateTime(now.Year, (q - 1) * 3 + 1, 1);
                    return (qStart, qStart.AddMonths(3), qStart.AddMonths(-3), qStart,
                        $"Quý {q}/{now.Year}", "Quý trước");

                case "year":
                    var yStart = new DateTime(now.Year, 1, 1);
                    return (yStart, new DateTime(now.Year + 1, 1, 1), new DateTime(now.Year - 1, 1, 1), yStart,
                        $"Năm {now.Year}", "Năm trước");

                case "month":
                default:
                    var mStart = new DateTime(now.Year, now.Month, 1);
                    return (mStart, mStart.AddMonths(1), mStart.AddMonths(-1), mStart,
                        $"Tháng {now:MM/yyyy}", "Tháng trước");
            }
        }

        public async Task<DashboardSummaryDTO> GetThuChiSummary(string filterType = "month")
        {
            filterType = (filterType ?? "month").ToLower().Trim();
            var now = DateTime.Now;
            var today = DateTime.Today;
            var (startCur, endCur, startPrev, endPrev, filterPeriodName, prevPeriodName) = ComputeDateRange(filterType, now, today);

            var allOrders = await _repo.DonHang.FindAll().AsNoTracking().ToListAsync();

            var ordersCur = allOrders.Where(x => x.Date.HasValue && x.Date.Value >= startCur && x.Date.Value < endCur).ToList();
            var ordersPrev = allOrders.Where(x => x.Date.HasValue && x.Date.Value >= startPrev && x.Date.Value < endPrev).ToList();

            return new DashboardSummaryDTO
            {
                FilterType = filterType,
                FilterPeriodName = filterPeriodName,
                PrevPeriodName = prevPeriodName,
                TongThuKy = ordersCur.Where(x => x.Loai == 2).Sum(x => (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0)),
                TongChiKy = ordersCur.Where(x => x.Loai == 1).Sum(x => x.TongTien ?? 0),
                TongThuKyTruoc = ordersPrev.Where(x => x.Loai == 2).Sum(x => (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0)),
                TongChiKyTruoc = ordersPrev.Where(x => x.Loai == 1).Sum(x => x.TongTien ?? 0),
                RevenueByDay = BuildChartData(filterType, startCur, endCur, allOrders)
            };
        }

        public async Task<DashboardSummaryDTO> GetSummary(string filterType = "month")
        {
            var result = await GetThuChiSummary(filterType);

            var allOrders = await _repo.DonHang.FindAll().AsNoTracking().ToListAsync();

            result.TongDuNo = allOrders
                .Where(x => x.Loai == 2 && (x.TongTien ?? 0) > (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0))
                .Sum(x => (x.TongTien ?? 0) - (x.TienMat ?? 0) - (x.ChuyenKhoan ?? 0));

            var overdueOrders = await GetOverdueOrders();

            result.SoDonChuaThanhToan = allOrders.Count(x =>
                x.Loai == 2 &&
                (x.TongTien ?? 0) > (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0));

            result.SoDonQuaHan = overdueOrders.Count(x => x.IsOverdue);
            result.DonQuaHan = overdueOrders;
            result.TopCongNoKhachHang = await GetTopCongNoKhachHang(allOrders);

            return result;
        }

        private async Task<List<CongNoCustomerSummaryDTO>> GetTopCongNoKhachHang(List<Models.DonHang> allOrders)
        {
            var unpaidOrders = allOrders
                .Where(x => x.Loai == 2 && x.ID_KH.HasValue && (x.TongTien ?? 0) > (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0))
                .ToList();

            if (!unpaidOrders.Any())
                return new List<CongNoCustomerSummaryDTO>();

            var khIds = unpaidOrders.Select(x => x.ID_KH!.Value).Distinct().ToList();
            var khachHangs = await _repo.KhachHang.FindAll(x => khIds.Contains(x.ID)).AsNoTracking().ToListAsync();
            var khDict = khachHangs.ToDictionary(x => x.ID);

            var today = DateTime.Today;

            var result = unpaidOrders
                .GroupBy(x => x.ID_KH!.Value)
                .Select(g =>
                {
                    var khId = g.Key;
                    var kh = khDict.GetValueOrDefault(khId);
                    var orders = g.ToList();
                    var tongConNo = orders.Sum(x => (x.TongTien ?? 0) - (x.TienMat ?? 0) - (x.ChuyenKhoan ?? 0));
                    var soDonNo = orders.Count;

                    // Xóa trạng thái công nợ
                    bool isQuaHan = orders.Any(o =>
                    {
                        if (o.SoNgayCongNo.HasValue && o.Date.HasValue) return o.Date.Value.AddDays(o.SoNgayCongNo.Value).Date < today;
                        if (kh?.SoNgayCongNo != null && o.Date.HasValue) return o.Date.Value.AddDays(kh.SoNgayCongNo.Value).Date < today;
                        return false;
                    });

                    bool isSapDenHan = !isQuaHan && orders.Any(o =>
                    {
                        DateTime? dueDate = null;
                        if (o.Date.HasValue && o.SoNgayCongNo.HasValue)
                            dueDate = o.Date.Value.AddDays(o.SoNgayCongNo.Value).Date;
                        if (!dueDate.HasValue && kh?.SoNgayCongNo != null && o.Date.HasValue)
                            dueDate = o.Date.Value.AddDays(kh.SoNgayCongNo.Value);
                        return dueDate.HasValue && (dueDate.Value.Date - today).Days <= 7;
                    });

                    string trangThai = isQuaHan ? "QuaHan" : (isSapDenHan ? "SapDenHan" : "ConHan");
                    string trangThaiText = isQuaHan ? "Quá hạn" : (isSapDenHan ? "Sắp đến hạn" : "Còn hạn");

                    return new CongNoCustomerSummaryDTO
                    {
                        KhachHangID = khId,
                        TenKhachHang = kh?.Ten ?? $"KH #{khId}",
                        SDT = kh?.SDT ?? "",
                        SoDonNo = soDonNo,
                        TongConNo = tongConNo,
                        HanMucCongNo = kh?.HanMucCongNo,
                        TrangThai = trangThai,
                        TrangThaiText = trangThaiText
                    };
                })
                .OrderByDescending(x => x.TongConNo)
                .Take(10)
                .ToList();

            return result;
        }

        private List<DashboardDayStatDTO> BuildChartData(string filterType, DateTime startCur, DateTime endCur, List<Models.DonHang> allOrders)
        {
            var chartData = new List<DashboardDayStatDTO>();

            if (filterType == "day")
            {
                for (int i = 6; i >= 0; i--)
                {
                    var day = DateTime.Today.AddDays(-i);
                    var dayOrders = allOrders.Where(x => x.Date.HasValue && x.Date.Value.Date == day.Date).ToList();
                    chartData.Add(new DashboardDayStatDTO
                    {
                        Label = day.ToString("dd/MM"),
                        TongThu = dayOrders.Where(x => x.Loai == 2).Sum(x => (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0)),
                        TongChi = dayOrders.Where(x => x.Loai == 1).Sum(x => x.TongTien ?? 0)
                    });
                }
            }
            else if (filterType == "week")
            {
                string[] dayNames = { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
                for (int i = 0; i < 7; i++)
                {
                    var day = startCur.AddDays(i);
                    var dayOrders = allOrders.Where(x => x.Date.HasValue && x.Date.Value.Date == day.Date).ToList();
                    chartData.Add(new DashboardDayStatDTO
                    {
                        Label = $"{dayNames[i]} ({day:dd/MM})",
                        TongThu = dayOrders.Where(x => x.Loai == 2).Sum(x => (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0)),
                        TongChi = dayOrders.Where(x => x.Loai == 1).Sum(x => x.TongTien ?? 0)
                    });
                }
            }
            else if (filterType == "month")
            {
                for (int day = 1; day <= DateTime.DaysInMonth(startCur.Year, startCur.Month); day += 3)
                {
                    var dayStart = new DateTime(startCur.Year, startCur.Month, day);
                    var dayEnd = dayStart.AddDays(3);
                    if (dayEnd > endCur) dayEnd = endCur;

                    var periodOrders = allOrders.Where(x => x.Date.HasValue && x.Date.Value >= dayStart && x.Date.Value < dayEnd).ToList();
                    chartData.Add(new DashboardDayStatDTO
                    {
                        Label = $"{dayStart:dd/MM}",
                        TongThu = periodOrders.Where(x => x.Loai == 2).Sum(x => (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0)),
                        TongChi = periodOrders.Where(x => x.Loai == 1).Sum(x => x.TongTien ?? 0)
                    });
                }
            }
            else if (filterType == "quarter")
            {
                for (int m = 0; m < 3; m++)
                {
                    var mStart = startCur.AddMonths(m);
                    var mEnd = mStart.AddMonths(1);

                    var mOrders = allOrders.Where(x => x.Date.HasValue && x.Date.Value >= mStart && x.Date.Value < mEnd).ToList();
                    chartData.Add(new DashboardDayStatDTO
                    {
                        Label = $"T{mStart.Month}/{mStart.Year}",
                        TongThu = mOrders.Where(x => x.Loai == 2).Sum(x => (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0)),
                        TongChi = mOrders.Where(x => x.Loai == 1).Sum(x => x.TongTien ?? 0)
                    });
                }
            }
            else if (filterType == "year")
            {
                for (int m = 1; m <= 12; m++)
                {
                    var mStart = new DateTime(startCur.Year, m, 1);
                    var mEnd = mStart.AddMonths(1);

                    var mOrders = allOrders.Where(x => x.Date.HasValue && x.Date.Value >= mStart && x.Date.Value < mEnd).ToList();
                    chartData.Add(new DashboardDayStatDTO
                    {
                        Label = $"Thg {m}",
                        TongThu = mOrders.Where(x => x.Loai == 2).Sum(x => (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0)),
                        TongChi = mOrders.Where(x => x.Loai == 1).Sum(x => x.TongTien ?? 0)
                    });
                }
            }

            return chartData;
        }

        public async Task<List<OverdueDonHangDTO>> GetOverdueOrders()
        {
            var today = DateTime.Today;
            var maxDate = today.AddDays(3);

            var unpaidRaw = await _repo.DonHang
                .FindAll(x =>
                    x.Loai == 2 &&
                    (x.TongTien ?? 0) > (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0))
                .AsNoTracking()
                .ToListAsync();

            if (!unpaidRaw.Any())
                return new List<OverdueDonHangDTO>();

            var khIds = unpaidRaw.Where(x => x.ID_KH.HasValue).Select(x => x.ID_KH.Value).Distinct().ToList();
            var khList = khIds.Any()
                ? await _repo.KhachHang.FindAll(x => khIds.Contains(x.ID))
                    .AsNoTracking()
                    .ToListAsync()
                : new List<Models.KhachHang>();
            var khDict = khList.ToDictionary(x => x.ID);

            var result = new List<OverdueDonHangDTO>();

            foreach (var x in unpaidRaw)
            {
                khDict.TryGetValue(x.ID_KH ?? 0, out var kh);

                DateTime? effectiveDueDate = null;
                if (x.Date.HasValue && x.SoNgayCongNo.HasValue)
                    effectiveDueDate = x.Date.Value.AddDays(x.SoNgayCongNo.Value).Date;
                if (!effectiveDueDate.HasValue && kh != null)
                {
                    if (kh.SoNgayCongNo != null && x.Date.HasValue)
                        effectiveDueDate = x.Date.Value.AddDays(kh.SoNgayCongNo.Value).Date;
                }

                if (!effectiveDueDate.HasValue || effectiveDueDate.Value > maxDate)
                    continue;

                var daTT = (x.TienMat ?? 0) + (x.ChuyenKhoan ?? 0);
                var conNo = (x.TongTien ?? 0) - daTT;
                var soNgayTre = (today - effectiveDueDate.Value).Days;
                var tenKH = kh?.Ten ?? "";
                var sdtKH = kh?.SDT ?? "";

                result.Add(new OverdueDonHangDTO
                {
                    ID = x.ID,
                    Ma_DH = x.Ma_DH,
                    Ten_KH = tenKH,
                    SDT_KH = sdtKH,
                    Date = x.Date,
                    DateStr = x.Date.HasValue ? x.Date.Value.ToString("dd/MM/yyyy") : "",
                    NgayDenHan = effectiveDueDate,
                    NgayDenHanStr = effectiveDueDate.Value.ToString("dd/MM/yyyy"),
                    TongTien = x.TongTien ?? 0,
                    DaTT = daTT,
                    ConNo = conNo,
                    SoNgayTre = soNgayTre,
                    IsOverdue = soNgayTre > 0
                });
            }

            return result.OrderByDescending(r => r.SoNgayTre).ToList();
        }
    }
}
