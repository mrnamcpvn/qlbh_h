export interface Report {
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
