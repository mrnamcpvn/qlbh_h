import { Pagination } from '@utilities/pagination-utility';

export interface DonHang {
  id: number;
  iD_KH: number;
  ten_KH: string;
  diaChi: string;
  tongTien: number;
  date: string | Date;
  loai: number;
  status: boolean;
  tienMat?: number;
  chuyenKhoan?: number;
  iD_NV: number;
  paymentSummary?: { key: string; value: number }[];
}

export interface DonHangFilter {
  pagination?: Pagination;
  fromDate: string | Date;
  toDate: string | Date;
  loai: number;
  soHoaDon?: number;
  payType?: number;
}

export interface ChiTietDonHang {
  id: number;
  iD_DH: number;
  iD_SP: number;
  ten_SP: string;
  soLuong: number;
  gia: number;
  sL_Ton_Dau: number;
  sL_Ton_Cuoi: number;
  updated_time: string | Date;
  dvt: string;
  thanhTien: number;
}

export interface DonHangDTO extends DonHang{
  chitiet: ChiTietDonHang[];
}
