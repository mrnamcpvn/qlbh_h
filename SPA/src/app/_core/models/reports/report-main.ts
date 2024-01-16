export interface Report {
  date: string;
  mH_ID: number | null;
  mH_Name: string;
  cD_ID: number;
  cD_Name: string;
  quantity: number;
  money: number;
  total_Money: number;
}

export interface ReportExport {
  mH_Name: string;
  details: ReportExportDetail[];
}

export interface ReportExportDetail {
  sTT: number;
  cD_Name: string;
  quantity: number;
  money: number;
  total_Money: number;
}

export interface ReportMainParam {
  fromDate: string | Date | null;
  toDate: string | Date | null;
  id_NLD: number;
}