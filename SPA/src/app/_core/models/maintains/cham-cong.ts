export interface ChamCong {
  id: number;
  iD_NLD: number;
  iD_CD: number;
  quantity: number;
  date: string | Date;
}

export interface ChamCongDTO extends ChamCong {
  nld: string;
  cD_Name: string;
  idMaHang: number;
  mH_Name: string;
}
