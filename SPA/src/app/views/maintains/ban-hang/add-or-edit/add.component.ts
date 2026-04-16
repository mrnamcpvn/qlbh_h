import { Component, OnInit, TemplateRef } from '@angular/core';
import { IconButton } from '@constants/common.constants';
import { ChiTietDonHang, DonHangDTO } from '@models/maintains/don-hang';
import { SanPham } from '@models/maintains/san-pham';
import { MaHang } from '@models/maintains/ma-hang';
import { KhachHang } from '@models/maintains/khach-hang';
import { SanPhamService } from '@services/san-pham.service';
import { DonHangService } from '@services/don-hang.service';
import { KhachHangService } from '@services/khach-hang.service';
import { InjectBase } from '@utilities/inject-base-app';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { NhanVien } from '@models/maintains/nhan-vien';
import { NhanVienService } from '@services/nhan-vien.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent extends InjectBase implements OnInit {
  iconButton = IconButton;
  bsConfig: Partial<BsDatepickerConfig> = <Partial<BsDatepickerConfig>>{
    dateInputFormat: 'DD/MM/YYYY'
  };
  checkDate: boolean = false;
  data: DonHangDTO = <DonHangDTO>{};
  chiTiet: ChiTietDonHang = <ChiTietDonHang>{}
  listChiTiet: ChiTietDonHang[] = [];
  khachHangs: KhachHang[] = [];
  nhanViens: NhanVien[] = [];
  sanPhams: SanPham[] = [];
  tenSP: string = '';
  giaSP: number;
  soLuong: number;
  slMax: number | null = null;
  mahangs: MaHang[] = [];
  iD_NV: number;
  idSP: number;
  tongTien: number;
  id: number;
  date: Date = new Date();
  dvt: string = '';
  modalRef?: BsModalRef;
  constructor(
    private donHangService: DonHangService,
    private khService: KhachHangService,
    private nvService: NhanVienService,
    private spService: SanPhamService,
    private modalService: BsModalService) {
    super();
  }

  ngOnInit() {
    this.getAllKH();
    this.getAllNV();
    this.getAllSP();
    this.clear();
  }

  getAllKH() {
    this.khService.getAll().subscribe({
      next: (res) => {
        this.khachHangs = res;
      }
    })
  }

  getAllNV() {
    this.nvService.getAll().subscribe({
      next: (res) => {
        this.nhanViens = res;
      }
    })
  }

  getAllMH() {

  }

  mhChanges(id) {
    this.clearSP();
    let item = this.sanPhams.find(x => x.id == id);
    this.giaSP = item.gia;
    this.tenSP = item.ten;
    this.chiTiet.ten_SP = item.ten;
    this.chiTiet.iD_SP = item.id;
    this.slMax = item.soLuong;
    this.dvt = item.dvt;
  }
  clearSP() {
    this.giaSP = null;
    this.tenSP = '';
    this.chiTiet.ten_SP = '';
    this.chiTiet.iD_SP = null;
    this.slMax = null;
    this.dvt = '';
  }

  getAllSP() {
    this.spService.getAll().subscribe({
      next: (res) => {
        this.sanPhams = res;
      }
    })
  }

  add() {
    if (this.chiTiet.soLuong > this.slMax) {
      let text = '';
      this.slMax ? text += "Số lượng trong kho chỉ còn " + this.slMax + " " + this.dvt : text += "Không có trong kho"
      this.snotifyService.warning(text, 'Cảnh báo')
    } else {
      this.chiTiet.gia = this.giaSP;
      this.chiTiet.dvt = this.dvt;
      this.chiTiet.soLuong = this.soLuong;
      this.chiTiet.thanhTien = this.chiTiet.soLuong * this.chiTiet.gia;
      this.chiTiet.stt = this.listChiTiet.length + 1;

      this.listChiTiet.push({ ...this.chiTiet });

      this.tongTien = this.listChiTiet.reduce((tt, item) => {
        return tt + (item.gia * item.soLuong);
      }, 0)
      this.idSP = null;
      this.tenSP = '';
      this.giaSP = null;
      this.chiTiet = <ChiTietDonHang>{};
      this.slMax = null;
    }
  }

  create(type: string) {
    this.data.tongTien = this.tongTien;
    this.data.loai = 2;
    this.data.iD_NV = this.iD_NV;
    this.data.chitiet = this.listChiTiet;
    if (this.date)
      this.data.date_Str = this.functionUtility.getDateFormat(this.date)
    this.donHangService.create(this.data).subscribe({
      next: (res) => {
        if (res) {
          this.snotifyService.success('Thêm đơn hàng thành công', 'Thành công');
          this.donHangService.changeSDonHang(res);
          this.router.navigate(['/maintain/ban-hang/detail', res.id]);
        }
        else
          this.snotifyService.success('Thêm đơn hàng không thành công', 'Lỗi');
      }
    })
  }

  checktime() {
    this.checkDate = this.functionUtility.checkEmpty(this.checkDate);
  }

  back() {
    this.router.navigate(['/maintain/ban-hang']);
  }

  editRow(index: number) {
    const item = this.listChiTiet[index];
    this.idSP = item.iD_SP;
    this.tenSP = item.ten_SP;
    this.giaSP = item.gia;
    this.chiTiet.soLuong = item.soLuong;
    this.chiTiet.ten_SP = item.ten_SP;
    this.chiTiet.iD_SP = item.iD_SP;
    this.chiTiet.dvt = item.dvt;
    this.chiTiet.thanhTien = item.thanhTien;
    this.slMax = item.soLuong;
    this.dvt = item.dvt;
    this.deleteRow(index);
  }

  deleteRow(index: number) {
    this.listChiTiet.splice(index, 1);
    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    }, 0);
  }

  openModal(template: TemplateRef<void>, item: ChiTietDonHang) {
    this.chiTiet = { ...item };
    this.modalRef = this.modalService.show(template);
  }

  cancel() {
    this.modalRef?.hide();
  }

  saveModal() {
    const index = this.listChiTiet.findIndex(x => x.stt === this.chiTiet.stt);
    if (index !== -1) {
      this.listChiTiet[index].gia = this.chiTiet.gia;
      this.listChiTiet[index].soLuong = this.chiTiet.soLuong;
      this.listChiTiet[index].thanhTien = this.listChiTiet[index].gia * this.listChiTiet[index].soLuong;
    }
    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    }, 0);
    this.modalRef?.hide();
  }

  deleteItem(item: ChiTietDonHang) {
    this.listChiTiet = this.listChiTiet.filter(x => x !== item);
    this.listChiTiet.forEach((x, i) => x.stt = i + 1);
    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    }, 0);
  }

  clear() {
  }
}
