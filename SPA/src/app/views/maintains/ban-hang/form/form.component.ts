import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconButton } from '@constants/common.constants';
import { ChiTietDonHang, DonHang, DonHangDTO } from '@models/maintains/don-hang';
import { SanPham } from '@models/maintains/san-pham';
import { KhachHang } from '@models/maintains/khach-hang';
import { NhanVien } from '@models/maintains/nhan-vien';
import { SanPhamService } from '@services/san-pham.service';
import { DonHangService } from '@services/don-hang.service';
import { KhachHangService } from '@services/khach-hang.service';
import { NhanVienService } from '@services/nhan-vien.service';
import { InjectBase } from '@utilities/inject-base-app';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-form',
  templateUrl: './form.component.html',
  styleUrls: ['./form.component.scss']
})
export class FormComponent extends InjectBase implements OnInit, AfterViewInit {
  iconButton = IconButton;
  bsConfig: Partial<BsDatepickerConfig> = <Partial<BsDatepickerConfig>>{
    dateInputFormat: 'DD/MM/YYYY'
  };

  // Master data
  id: number;
  isEdit: boolean = false;
  donHang: DonHang = <DonHang>{};
  data: DonHangDTO = <DonHangDTO>{};
  listChiTiet: ChiTietDonHang[] = [];
  date: Date = new Date();
  tongTien: number = 0;

  // Selection data (dropdowns)
  khachHangs: KhachHang[] = [];
  nhanViens: NhanVien[] = [];
  sanPhams: SanPham[] = [];




  constructor(
    private donHangService: DonHangService,
    private khService: KhachHangService,
    private nvService: NhanVienService,
    private spService: SanPhamService,
    private route: ActivatedRoute
  ) {
    super();
  }

  ngOnInit() {
    this.id = this.route.snapshot.params['id'];
    this.isEdit = !!this.id;

    if (this.isEdit) {
      this.donHangService.current_DH.subscribe({
        next: res => {
          if (res) {
            this.donHang = res;
            this.data.iD_KH = res.iD_KH;
            this.data.iD_NV = res.iD_NV;
            this.data.ma_DH = res.ma_DH;
            if (res.date)
              this.date = new Date(res.date);
            this.tongTien = res.tongTien;
          } else {
            // If no current_DH (e.g. direct access via URL), we might need to fetch it or redirect
            // For now follow the original logic
            this.router.navigate(['/maintain/ban-hang']);
          }
        },
        error: () => this.router.navigate(['/maintain/ban-hang'])
      });
    }

    this.getAllKH();
    this.getAllNV();
    this.getAllSP();
  }

  ngAfterViewInit(): void {
    if (this.isEdit && this.id) {
      this.getDetail();
    }
  }

  getDetail() {
    this.donHangService.getDetail(this.id).subscribe({
      next: res => {
        this.listChiTiet = res;
        this.listChiTiet.forEach((item, index) => {
          item.stt = index + 1;
        });
      }
    });
  }

  getAllKH() {
    this.khService.getAll().subscribe({
      next: (res) => {
        this.khachHangs = res;
      }
    });
  }

  getAllNV() {
    this.nvService.getAll().subscribe({
      next: (res) => {
        this.nhanViens = res;
      }
    });
  }

  getAllSP() {
    this.spService.getAll().subscribe({
      next: (res) => {
        this.sanPhams = res;
      }
    });
  }

  mhChanges(id: number, item: ChiTietDonHang) {
    let sp = this.sanPhams.find(x => x.id == id);
    if (sp) {
      item.ten_SP = sp.ten;
      (item as any).ma_SP = sp.maSP;
      item.iD_SP = sp.id;
      item.gia = sp.gia;
      item.dvt = sp.dvt;
      item.soLuong = 1;
      this.onItemChange(item);
    }
  }

  add() {
    const newItem = {
      stt: this.listChiTiet.length + 1,
      iD_SP: null,
      ten_SP: '',
      soLuong: 1,
      gia: 0,
      thanhTien: 0,
      dvt: ''
    } as any;
    this.listChiTiet.push(newItem);
    this.calculateTotal();
  }

  calculateTotal() {
    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    }, 0);
  }

  save() {
    this.data.tongTien = this.tongTien;
    this.data.chitiet = this.listChiTiet;
    if (this.date)
      this.data.date_Str = this.functionUtility.getDateFormat(this.date);

    if (this.isEdit) {
      this.data.id = this.donHang.id;
      this.data.loai = this.donHang.loai;
      this.data.ma_DH = this.donHang.ma_DH;

      this.donHangService.update(this.data).subscribe({
        next: (res) => {
          if (res) {
            this.snotifyService.success('Sửa đơn hàng thành công', 'Thành công');
            this.donHangService.changeSDonHang(res);
            this.router.navigate(['/maintain/ban-hang/detail', res.id]);
          } else {
            this.snotifyService.error('Sửa đơn hàng không thành công', 'Lỗi');
          }
        }
      });
    } else {
      this.data.loai = 2; // Fixed type for sales
      this.donHangService.create(this.data).subscribe({
        next: (res) => {
          if (res) {
            this.snotifyService.success('Thêm đơn hàng thành công', 'Thành công');
            this.donHangService.changeSDonHang(res);
            this.router.navigate(['/maintain/ban-hang/detail', res.id]);
          } else {
            this.snotifyService.error('Thêm đơn hàng không thành công', 'Lỗi');
          }
        }
      });
    }
  }

  back() {
    this.router.navigate(['/maintain/ban-hang']);
  }

  deleteItem(item: ChiTietDonHang) {
    if (this.isEdit && item.id) {
      this.snotifyService.confirm("Bạn có chắc chắc muốn xóa?", "Xóa", () => {
        this.donHangService.deleteItem(item.id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success("Xóa thành công", "Thành công");
              this.removeItemFromList(item);
            } else {
              this.snotifyService.warning("Xóa không thành công", "Cảnh báo");
            }
          },
          error: (err) => this.snotifyService.error(err, "Lỗi")
        });
      });
    } else {
      this.removeItemFromList(item);
    }
  }

  removeItemFromList(item: ChiTietDonHang) {
    this.listChiTiet = this.listChiTiet.filter(x => x !== item);
    this.listChiTiet.forEach((x, i) => x.stt = i + 1);
    this.calculateTotal();
    if (this.isEdit) {
      this.donHang.tongTien = this.tongTien;
      this.donHangService.changeSDonHang(this.donHang);
    }
  }

  onItemChange(item: ChiTietDonHang) {
    item.thanhTien = (item.soLuong || 0) * (item.gia || 0);
    this.calculateTotal();
    if (this.isEdit) {
      this.donHang.tongTien = this.tongTien;
      this.donHangService.changeSDonHang(this.donHang);
    }
  }
}
