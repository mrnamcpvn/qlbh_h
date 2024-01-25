export interface DonHang {
  id: number;
  iD_KH: number;
  ten_KH: string;
  tongTien: number;
  date: string | Date;
  loai: number;
}

export interface ChiTietDonHang {
  id: number;
  iD_DH: number;
  iD_SP: number;
  ten_SP: string;
  soLuong: number;
  gia: number;
  thanhTien: number;
}

export interface DonHangDTO extends DonHang{
  chitiet: ChiTietDonHang[];
}
