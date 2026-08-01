export interface OverdueDonHang {
  id: number;
  ma_DH: string;
  ten_KH: string;
  sdT_KH?: string;
  date?: Date | string;
  dateStr: string;
  ngayDenHan?: Date | string;
  ngayDenHanStr: string;
  tongTien: number;
  daTT: number;
  conNo: number;
  soNgayTre: number;
  isOverdue: boolean;
}

export interface CongNoCustomerSummary {
  khachHangID: number;
  tenKhachHang: string;
  sdt: string;
  soDonNo: number;
  tongConNo: number;
  hanMucCongNo?: number;
  trangThai: string;
  trangThaiText: string;
}

export interface DashboardDayStat {
  label: string;
  tongThu: number;
  tongChi: number;
}

export interface DashboardSummary {
  filterType: string;
  filterPeriodName: string;
  prevPeriodName: string;

  tongThuKy: number;
  tongChiKy: number;
  tongThuKyTruoc: number;
  tongChiKyTruoc: number;

  tongThuThang: number;
  tongChiThang: number;
  tongThuThangTruoc: number;
  tongChiThangTruoc: number;

  tongDuNo: number;
  soDonQuaHan: number;
  soDonChuaThanhToan: number;

  donQuaHan: OverdueDonHang[];
  topCongNoKhachHang: CongNoCustomerSummary[];
  revenueByDay: DashboardDayStat[];
}
