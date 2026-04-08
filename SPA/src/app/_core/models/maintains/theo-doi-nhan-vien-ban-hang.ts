import { Pagination, PaginationResult } from "@utilities/pagination-utility";

export interface TheoDoiNhanVienBanHang_Param {
  filterBy: string
  fromDate_Str: string;
  toDate_Str: string;
  idNV: number[];
  idSP: number[];
}
export interface TheoDoiNhanVienBanHang_Data {
  fromDate_Str: string;
  toDate_Str: string;
  tong_SL_Ban: number;
  tong_DS_Ban: number;
  pagination: Pagination
  result: TheoDoiNhanVienBanHang_SP[]
  nVs: string;
}
export interface TheoDoiNhanVienBanHang_SP {
  ten_SP: string;
  sL_Ban: number;
  dS_Ban: number;
  nV_List: TheoDoiNhanVienBanHang_NV[];
  isExpanded?: boolean;
}
export interface TheoDoiNhanVienBanHang_NV {
  ten_NV: string;
  sdT_NV: string;
  dvt: string;
  sL_Ban: number;
  dS_Ban: number;
}
