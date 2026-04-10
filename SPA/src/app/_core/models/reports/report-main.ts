export interface Report {
  iD_SP: number;
  ten_SP: string;
  dvt: string;
  soLuongTonDau: number;
  giaTon: number;
  soLuongNhap: number;
  tongTienNhap: number;
  soLuongXuat: number;
  tongTienXuat: number;
  soLuongTonCuoi: number;
  doanhThu: number;
}
export interface Report_Data {
  tong_SoLuongTonDau: number;
  tong_GiaTon: number;
  tong_SoLuongNhap: number;
  tong_TongTienNhap: number;
  tong_SoLuongXuat: number;
  tong_TongTienXuat: number;
  tong_SoLuongTonCuoi: number;
  tong_DoanhThu: number;
  result: Report[];
}
export interface ReportExport {
  mH_Name: string;
  details: ReportExportDetail[];
}

export interface ReportExportDetail {
  iD_SP: number;
  ten_SP: string;
  soLuongNhap: number;
  soLuongXuat: number;
  soLuongTonDau: number;
  soLuongTonCuoi: number;
  tongTienNhap: number;
  giaTon: number;
  tongTienXuat: number;
  doanhThu: number;
}

export interface ReportMainParam {
  fromDate: string | Date | null;
  toDate: string | Date | null;
  id_sp: number;
}
