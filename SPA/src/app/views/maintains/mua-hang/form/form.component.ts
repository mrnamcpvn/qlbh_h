import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconButton } from '@constants/common.constants';
import { ChiTietDonHang, DonHang, DonHangDTO } from '@models/maintains/don-hang';
import { SanPham } from '@models/maintains/san-pham';
import { NhaCungCap } from '@models/maintains/nha-cung-cap';
import { NhanVien } from '@models/maintains/nhan-vien';
import { SanPhamService } from '@services/san-pham.service';
import { DonHangService } from '@services/don-hang.service';
import { NhaCungCapService } from '@services/nha-cung-cap.service';
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

  // 1. UI & System Config
  bsConfig: Partial<BsDatepickerConfig> = { dateInputFormat: 'DD/MM/YYYY' };

  // 2. State & Route Params
  id: number;
  isEdit: boolean = false;

  // 3. Form Data Models
  data: DonHangDTO = <DonHangDTO>{};
  donHang: DonHang = <DonHang>{};
  listChiTiet: ChiTietDonHang[] = [];
  date: Date = new Date();
  tongTien: number = 0;

  // 4. Master Data (Dropdown lists)
  nhaCungCaps: NhaCungCap[] = [];
  nhanViens: NhanVien[] = [];
  sanPhams: SanPham[] = [];

  // 5. Tracking Data (For stock recalculation)
  originalSanPhams: SanPham[] = [];
  originalChiTiet: ChiTietDonHang[] = [];

  constructor(
    private donHangService: DonHangService,
    private nccService: NhaCungCapService,
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
            this.data.iD_NCC = res.iD_NCC;
            this.data.iD_NV = res.iD_NV;
            this.data.ma_DH = res.ma_DH;
            if (res.date)
              this.date = new Date(res.date);
            this.tongTien = res.tongTien;
          } else {
            this.router.navigate(['/maintain/mua-hang']);
          }
        },
        error: () => this.router.navigate(['/maintain/mua-hang'])
      });
    }

    this.getAllNCC();
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
        this.listChiTiet = (res || []).map((item: any) => ({
          ...item,
          iD_SP: item.iD_SP || item.ID_SP,
          soLuong: item.soLuong || item.SoLuong || 0,
          gia: item.gia || item.Gia || 0,
          thanhTien: item.thanhTien || item.ThanhTien || 0,
          dvt: item.dvt || item.Dvt
        }));
        this.originalChiTiet = this.listChiTiet.map(x => ({ ...x }));
        this.listChiTiet.forEach((item, index) => {
          item.stt = index + 1;
        });
        this.recalculateStock();
      }
    });
  }

  getAllNCC() {
    this.nccService.getAll().subscribe({
      next: (res) => {
        this.nhaCungCaps = res;
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
        this.originalSanPhams = res;
        this.recalculateStock();
      }
    });
  }

  recalculateStock() {
    if (this.originalSanPhams && this.originalSanPhams.length > 0) {
      // Create new list and preserve current warehouse stock in 'tonHienTai'
      this.sanPhams = this.originalSanPhams.map(sp => ({
        ...sp,
        tonHienTai: sp.soLuong
      }));

      // For purchasing, we subtract what's already in the DB to get pre-purchase levels
      if (this.isEdit && this.originalChiTiet && this.originalChiTiet.length > 0) {
        this.originalChiTiet.forEach(item => {
          let sp = this.sanPhams.find(x => x.id == item.iD_SP);
          if (sp) {
            sp.soLuong -= (item.soLuong || 0);
          }
        });
      }

      // Add all selected products' quantities currently in the form
      if (this.listChiTiet && this.listChiTiet.length > 0) {
        this.listChiTiet.forEach(item => {
          if (item.iD_SP) {
            let sp = this.sanPhams.find(x => x.id == item.iD_SP);
            if (sp) sp.soLuong += (item.soLuong || 0);
          }
        });
      }
    }
  }

  mhChanges(id: number, item: ChiTietDonHang) {
    let sp = this.originalSanPhams.find(x => x.id == id);
    if (sp) {
      item.ten_SP = sp.ten;
      item.ma_SP = sp.maSP;
      item.iD_SP = sp.id;
      item.gia = sp.gia;
      item.dvt = sp.dvt;
    } else {
      delete item.gia;
      delete item.dvt
      delete item.soLuong
    }
    this.onItemChange(item);
  }

  add() {
    const newItem = {
      stt: this.listChiTiet.length + 1,
      iD_SP: null,
      ten_SP: '',
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
            this.router.navigate(['/maintain/mua-hang/detail', res.id]);
          } else {
            this.snotifyService.error('Sửa đơn hàng không thành công', 'Lỗi');
          }
        }
      });
    } else {
      this.data.loai = 1; // Fixed type for purchasing
      this.donHangService.create(this.data).subscribe({
        next: (res) => {
          if (res) {
            this.snotifyService.success('Thêm đơn hàng thành công', 'Thành công');
            this.donHangService.changeSDonHang(res);
            this.router.navigate(['/maintain/mua-hang/detail', res.id]);
          } else {
            this.snotifyService.error('Thêm đơn hàng không thành công', 'Lỗi');
          }
        }
      });
    }
  }

  back() {
    this.router.navigate(['/maintain/mua-hang']);
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
    this.recalculateStock();
    this.calculateTotal();
    if (this.isEdit) {
      this.donHang.tongTien = this.tongTien;
      this.donHangService.changeSDonHang(this.donHang);
    }
  }

  onItemChange(item: ChiTietDonHang) {
    if (item.soLuong == null || item.soLuong == undefined || item.soLuong <= 0)
      delete item.soLuong
    if (item.gia == null || item.gia == undefined || item.gia < 0)
      delete item.gia

    item.thanhTien = (item.soLuong || 0) * (item.gia || 0);
    this.recalculateStock();
    this.calculateTotal();
    if (this.isEdit) {
      this.donHang.tongTien = this.tongTien;
      this.donHangService.changeSDonHang(this.donHang);
    }
  }

  customSearchFn = (term: string, item: any) => {
    const search = this.removeAccents(term);
    return this.removeAccents(item.maSP).includes(search) ||
      this.removeAccents(item.ten).includes(search);
  }

  removeAccents(str: string): string {
    return (str || '').toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd');
  }
}
