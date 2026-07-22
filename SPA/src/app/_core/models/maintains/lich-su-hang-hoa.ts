import { Pagination, PaginationResult } from "@utilities/pagination-utility";

export interface LichSuHangHoaParam {
  filterBy: string
  idSP: number[];
  idKH: number[];
  idNCC: number[];
  idNV: number[];
  fromDate: string;
  toDate: string;
  loai: number;
}

export interface LichSuHangHoaItem {
  id: number;
  id_SP: number;
  maSP: string;
  ten_SP: string;
  dvt: string;
  id_DH: number;
  ma_DH: string;
  loai: number;
  loaiStr: string;
  doiTac: string;
  ten_NV: string;
  soLuong: number;
  gia: number;
  thanhTien: number;
  sL_Ton_Dau: number;
  sL_Ton_Cuoi: number;
  date: Date;
  updated_Time: Date;
}

export interface LichSuHangHoaData {
  result: LichSuHangHoaItem[]
  pagination: Pagination
  tongSLNhap: number;
  tongSLXuat: number;
  tongTienNhap: number;
  tongTienXuat: number;
}
