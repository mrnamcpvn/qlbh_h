export interface BaoCaoThuChiParam {
    fromDate: string;
    toDate: string;
    idSP: number[];
    idKH: number[];
    idNCC: number[];
    idNV: number[];
    loai: number;
}

export interface BaoCaoThuChiProductDetail {
    tenSP: string;
    dvt: string;
    soLuong: number;
    thanhTien: number;
}

export interface BaoCaoThuChiOrder {
    date: Date;
    dateStr: string;
    ma_DH: string;
    loai: number;
    doiTac: string;
    nhanVien: string;
    products: BaoCaoThuChiProductDetail[];
    tongTien: number;
    tienMat: number;
    chuyenKhoan: number;
    conThieu: number;
    isExpanded?: boolean;
}

export interface BaoCaoThuChiData {
    fromDateStr: string;
    toDateStr: string;
    orders: BaoCaoThuChiOrder[];
    tongTongTien: number;
    tongTienMat: number;
    tongChuyenKhoan: number;
    tongConThieu: number;
}
