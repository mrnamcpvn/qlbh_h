import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconButton } from '@constants/common.constants';
import { ChiTietDonHang, DonHangDTO } from '@models/maintains/don-hang';
import { SanPham } from '@models/maintains/san-pham';
import { MaHang } from '@models/maintains/ma-hang';
import { KhachHang } from '@models/maintains/khach-hang';
import { SanPhamService } from '@services/san-pham.service';
import { DonHangService } from '@services/don-hang.service';
import { MaHangService } from '@services/mahang.service';
import { KhachHangService } from '@services/khach-hang.service';
import { InjectBase } from '@utilities/inject-base-app';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { KeyValuePair } from '@utilities/key-value-pair';
import { NhanVien } from '@models/maintains/nhan-vien';
import { NhanVienService } from '@services/nhan-vien.service';

@Component({
  selector: 'app-add-or-edit',
  templateUrl: './add-or-edit.component.html',
  styleUrls: ['./add-or-edit.component.scss']
})
export class AddOrEditComponent extends InjectBase implements OnInit, AfterViewInit {
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
  slMax: number | null = null;
  mahangs: MaHang[] = [];
  iD_NV: number;
  idSP: number;
  tongTien: number;
  id: number;
  type: string = 'add';
  dvt: string = '';
  constructor(
    private donHangService: DonHangService,
    private khService: KhachHangService,
    private nvService: NhanVienService,
    private spService: SanPhamService,
    private route: ActivatedRoute) {
    super();
  }

  ngOnInit() {
    this.getAllKH();
    this.getAllNV();
    this.getAllSP();
    this.clear();
  }

  ngAfterViewInit(): void {
    this.id = this.route.snapshot.params['id'];
    if (this.id) {
      this.type = 'edit';
      this.getDetail();
    }
  }

  getDetail() {
    // this.chamcongService.getDetail(this.id)
    //   .subscribe({
    //     next: res => {
    //       this.data = res;
    //       this.data.date = new Date(res.date);
    //       this.idMH = this.data.idMaHang;
    //       this.getAllCD(this.idMH);
    //     }
    //   })
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
    let item = this.sanPhams.find(x=>x.id == id);
    this.giaSP = item.gia;
    this.tenSP = item.ten;
    this.chiTiet.ten_SP = item.ten;
    this.chiTiet.iD_SP = item.id;
    this.slMax = item.soLuong;
    this.dvt = item.dvt;
  }
  clearSP(){
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
        console.log(res)
      }
    })
  }

  add() {
    if (!this.iD_NV) {
      this.snotifyService.warning('Vui lòng chọn nhân viên!', 'Cảnh báo');
      return;
    }
    if (this.chiTiet.soLuong > this.slMax) {
      let text = '';
      this.slMax ? text += "Số lượng trong kho chỉ còn " + this.slMax + " " + this.dvt : text += "Không có trong kho"
      this.snotifyService.warning(text, 'Cảnh báo')
    } else {
      this.chiTiet.gia = this.giaSP;
      this.chiTiet.dvt = this.dvt;
      this.chiTiet.thanhTien = this.chiTiet.soLuong * this.chiTiet.gia;
      console.log(this.listChiTiet.some(x => x.iD_SP == this.chiTiet.iD_SP));

      if (this.listChiTiet.some(x => x.iD_SP == this.chiTiet.iD_SP))

        this.listChiTiet.map(x => {
          if (x.iD_SP == this.chiTiet.iD_SP) {
            x.soLuong += this.chiTiet.soLuong;
            x.thanhTien = x.soLuong * x.gia;
          }
        })
      else this.listChiTiet.push(this.chiTiet)


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
    this.data.ten_KH = this.khachHangs.find(x => x.id == this.data.iD_KH).ten;
    this.data.tongTien = this.tongTien;
    this.data.loai = 2;
    this.data.iD_NV = this.iD_NV;
    this.data.chitiet = this.listChiTiet;
    this.donHangService.create(this.data).subscribe({
      next: (res) => {
        console.log("Thêm đh: ", res);
        if (res) {
          this.snotifyService.success('Thêm đơn hàng thành công', 'Thành công');
          this.donHangService.changeSDonHang(this.data);
          this.router.navigate(['/maintain/ban-hang/detail', res.id]);
        }
        else
          this.snotifyService.success('Thêm đơn hàng không thành công', 'Lỗi');
      }
    })
  }

  update() {
    // this.data.date = this.functionUtility.getUTCDate(new Date(this.data.date));
    // this.chamcongService.update(this.data).subscribe({
    //   next: (res) => {
    //     if (res) {
    //       this.snotifyService.success('Sửa công thành công', 'Thành công');
    //       this.back();
    //     }
    //     else
    //       this.snotifyService.success('Sửa công không thành công', 'Lỗi');
    //   }
    // })
  }

  checktime() {
    this.checkDate = this.functionUtility.checkEmpty(this.checkDate);
  }

  back() {
    this.router.navigate(['/maintain/ban-hang']);
  }

  clear() {
    // this.data = <ChiTietDonHang>{
    //   date: new Date()
    // };
    // this.idMH = null;
  }
}
