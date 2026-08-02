import { Pagination, PaginationResult } from "@utilities/pagination-utility";

export interface TheoDoiNhanVienBanHang_Param {
  filterBy: string
  fromDate_Str: string;
  toDate_Str: string;
  idNV: number[];
  idSP: number[];
  idKH: number[];
  ma_DH: string;
  trangThai: string;
}
export interface TheoDoiNhanVienBanHang_Data {
  fromDate_Str: string;
  toDate_Str: string;
  tong_So_Don: number;
  tong_SL_Ban: number;
  tong_DS_Ban: number;
  tong_DaThu: number;
  tong_CongNo: number;
  soDon_DaThanhToan: number;
  soDon_ChuaThanhToan: number;
  soDon_TreHan: number;
  pagination: Pagination
  result: TheoDoiNhanVienBanHang_NV[]
  nVs: string;
  isTotalExpanded?: boolean;
  tong_SP: TheoDoiNhanVienBanHang_SP[];
}
export interface TheoDoiNhanVienBanHang_NV {
  iD_NV: number;
  ten_NV: string;
  sdT_NV: string;
  so_Don: number;
  sL_Ban: number;
  soLoaiSP: number;
  dS_Ban: number;
  daThu: number;
  congNo: number;
  soDon_DaThanhToan: number;
  soDon_ChuaThanhToan: number;
  soDon_TreHan: number;
  donHang_List: TheoDoiNhanVienBanHang_DonHang[];
  isExpanded?: boolean;
  isSubTotalExpanded?: boolean;
  subTotal_List: TheoDoiNhanVienBanHang_SP[];
}
export interface TheoDoiNhanVienBanHang_DonHang {
  id: number;
  ma_DH: string;
  date: string;
  ten_KH: string;
  soLoaiSP: number;
  tongTien: number;
  daThanhToan: number;
  congNo: number;
  isDaThanhToan: boolean;
  isTreHan: boolean;
  sP_List: TheoDoiNhanVienBanHang_SP[];
  isExpanded?: boolean;
}
export interface TheoDoiNhanVienBanHang_SP {
  ten_SP: string;
  dvt: string;
  soLuong: number;
  gia: number;
  thanhTien: number;
}
