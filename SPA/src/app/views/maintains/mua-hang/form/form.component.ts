import { Component, OnInit, AfterViewInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IconButton } from '@constants/common.constants';
import { ChiTietDonHang, DonHang, DonHangDTO } from '@models/maintains/don-hang';
import { NhaCungCap } from '@models/maintains/nha-cung-cap';
import { NhanVien } from '@models/maintains/nhan-vien';
import { SanPham } from '@models/maintains/san-pham';
import { DonHangService } from '@services/don-hang.service';
import { NhaCungCapService } from '@services/nha-cung-cap.service';
import { NhanVienService } from '@services/nhan-vien.service';
import { SanPhamService } from '@services/san-pham.service';
import { InjectBase } from '@utilities/inject-base-app';

@Component({
  selector: 'app-form',
  templateUrl: './form.component.html',
  styleUrls: ['./form.component.scss']
})
export class FormComponent extends InjectBase implements OnInit, AfterViewInit, OnDestroy {

  iconButton = IconButton;
  bsConfig: Partial<BsDatepickerConfig> = { dateInputFormat: 'DD/MM/YYYY' };

  id: number;
  isEdit: boolean = false;

  data: DonHangDTO = <DonHangDTO>{};
  donHang: DonHang = <DonHang>{};
  listChiTiet: ChiTietDonHang[] = [];
  date: Date = new Date();
  tongTien: number = 0;

  nhaCungCaps: NhaCungCap[] = [];
  nhanViens: NhanVien[] = [];
  sanPhams: SanPham[] = [];

  private originalSanPhams: SanPham[] = [];
  private originalChiTiet: ChiTietDonHang[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private donHangService: DonHangService,
    private nccService: NhaCungCapService,
    private nvService: NhanVienService,
    private spService: SanPhamService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {
    super();
  }

  ngOnInit() {
    this.id = this.route.snapshot.params['id'];
    this.isEdit = !!this.id;

    if (this.isEdit) {
      this.loadEditData();
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadEditData() {
    this.donHangService.current_DH
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (res) {
            this.donHang = res;
            this.data.iD_NCC = res.iD_NCC;
            this.data.iD_NV = res.iD_NV;
            this.data.ma_DH = res.ma_DH;
            if (res.date) this.date = new Date(res.date);
            this.tongTien = res.tongTien;
          } else {
            this.router.navigate(['/maintain/mua-hang']);
          }
        },
        error: () => this.router.navigate(['/maintain/mua-hang'])
      });
  }

  private getDetail() {
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
        this.listChiTiet.forEach((item, i) => item.stt = i + 1);
        this.recalculateStock();
      }
    });
  }

  private getAllNCC() {
    this.nccService.getAll().subscribe({
      next: res => this.nhaCungCaps = res
    });
  }

  private getAllNV() {
    this.nvService.getAll().subscribe({
      next: res => this.nhanViens = res
    });
  }

  private getAllSP() {
    this.spService.getAll().subscribe({
      next: res => {
        this.originalSanPhams = res;
        this.recalculateStock();
      }
    });
  }

  /**
   * Tính lại tồn kho ảo (soLuong trên sanPhams) dựa trên:
   * 1. Tồn gốc (originalSanPhams)
   * 2. Trừ số lượng đã nhập trong DB (originalChiTiet) - chỉ khi edit
   * 3. Cộng thêm số lượng đang nhập trên form (listChiTiet)
   */
  private recalculateStock() {
    if (!this.originalSanPhams?.length) return;

    this.sanPhams = this.originalSanPhams.map(sp => ({
      ...sp,
      tonHienTai: sp.soLuong
    }));

    // Edit mode: trừ lại tồn đã nhập trong DB (hoàn tồn về trạng thái trước nhập)
    if (this.isEdit && this.originalChiTiet?.length) {
      this.originalChiTiet.forEach(item => {
        const sp = this.sanPhams.find(x => x.id == item.iD_SP);
        if (sp) sp.soLuong -= (item.soLuong || 0);
      });
    }

    // Cộng thêm số lượng đang nhập trên form
    this.listChiTiet.forEach(item => {
      if (!item.iD_SP) return;
      const sp = this.sanPhams.find(x => x.id == item.iD_SP);
      if (sp) sp.soLuong += (item.soLuong || 0);
    });
  }

  private calculateTotal() {
    this.tongTien = this.listChiTiet.reduce(
      (sum, item) => sum + ((item.gia || 0) * (item.soLuong || 0)), 0
    );
  }

  /** Đồng bộ tổng tiền với donHang khi ở chế độ Edit */
  private syncEditState() {
    if (this.isEdit) {
      this.donHang.tongTien = this.tongTien;
      this.donHangService.changeSDonHang(this.donHang);
    }
  }

  add() {
    this.listChiTiet.push({
      stt: this.listChiTiet.length + 1,
      iD_SP: null,
      ten_SP: '',
      dvt: ''
    } as any);
    this.calculateTotal();
  }

  deleteItem(item: ChiTietDonHang) {
    if (this.isEdit && item.id) {
      this.snotifyService.confirm("Bạn có chắc chắc muốn xóa?", "Xóa", () => {
        this.donHangService.deleteItem(item.id).subscribe({
          next: res => {
            if (res) {
              this.snotifyService.success("Xóa thành công", "Thành công");
              this.removeItemFromList(item);
            } else {
              this.snotifyService.warning("Xóa không thành công", "Cảnh báo");
            }
          },
          error: err => this.snotifyService.error(err, "Lỗi")
        });
      });
    } else {
      this.removeItemFromList(item);
    }
  }

  private removeItemFromList(item: ChiTietDonHang) {
    this.listChiTiet = this.listChiTiet.filter(x => x !== item);
    this.listChiTiet.forEach((x, i) => x.stt = i + 1);
    this.refreshCalculations();
  }

  mhChanges(id: number, item: ChiTietDonHang) {
    const sp = this.originalSanPhams.find(x => x.id == id);
    if (sp) {
      item.ten_SP = sp.ten;
      item.ma_SP = sp.maSP;
      item.iD_SP = sp.id;
      item.gia = sp.gia;
      item.dvt = sp.dvt;
    } else {
      delete item.gia;
      delete item.dvt;
      delete item.soLuong;
    }
    this.onItemChange(item);
  }

  onItemChange(item: ChiTietDonHang) {
    let needDetectChanges = false;

    // Xóa giá trị không hợp lệ
    if (item.soLuong != null && item.soLuong <= 0) {
      delete item.soLuong;
      needDetectChanges = true;
    }
    if (item.gia != null && item.gia < 0) {
      delete item.gia;
      needDetectChanges = true;
    }

    item.thanhTien = (item.soLuong || 0) * (item.gia || 0);
    this.recalculateStock();
    this.calculateTotal();
    this.syncEditState();

    // Chỉ gọi detectChanges khi thực sự có delete property
    if (needDetectChanges) this.cdr.detectChanges();
  }

  save() {
    this.data.tongTien = this.tongTien;
    this.data.chitiet = this.listChiTiet;
    if (this.date) this.data.date_Str = this.functionUtility.getDateFormat(this.date);

    if (this.isEdit) {
      this.data.id = this.donHang.id;
      this.data.loai = this.donHang.loai;
      this.data.ma_DH = this.donHang.ma_DH;

      this.donHangService.update(this.data).subscribe({
        next: res => {
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
      this.data.loai = 1; // Loại: Mua hàng
      this.donHangService.create(this.data).subscribe({
        next: res => {
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

  /** Gọi lại tất cả tính toán sau khi thay đổi dữ liệu */
  private refreshCalculations() {
    this.recalculateStock();
    this.calculateTotal();
    this.syncEditState();
  }

  customSearchFn = (term: string, item: any) => {
    const search = this.removeAccents(term);
    return this.removeAccents(item.maSP).includes(search) ||
      this.removeAccents(item.ten).includes(search);
  }

  private removeAccents(str: string): string {
    return (str || '').toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd');
  }
}
