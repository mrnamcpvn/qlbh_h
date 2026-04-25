export interface CongNoFilter {
  fromDate: string;
  toDate: string;
  idKH: number[];
  trangThai: string;
}

export interface CongNoSummary {
  khachHangID: number;
  tenKhachHang: string;
  sdt: string;
  soNgayCongNo: number;
  soDonNo: number;
  tongConNo: number;
  ngayDenHanGanNhat: string | Date;
  trangThai: string;
}

export interface CongNoCustomer {
  khachHangID: number;
  tenKhachHang: string;
  sdt: string;
  soNgayCongNo: number;
  hanMucCongNo: number | null;
  soDonNo: number;
  tongConNo: number;
  ngayDenHanGanNhat: string | Date;
  trangThai: string;
  orders: CongNoChiTiet[];
  isExpanded?: boolean;
}

export interface CustomerDebtInfo {
  currentDebt: number;
  creditLimit: number | null;
  soNgayCongNo: number;
  hasOverdueOrders: boolean;
  overdueOrderCount: number;
  isOverLimit: boolean;
  remainingCredit: number;
}

export interface CongNoChiTiet {
  donHangID: number;
  ma_DH: string;
  ngayXuat: string | Date;
  ngayDenHan: string | Date;
  tongTien: number;
  daThanhToan: number;
  conNo: number;
  soNgayQuaHan: number;
}
