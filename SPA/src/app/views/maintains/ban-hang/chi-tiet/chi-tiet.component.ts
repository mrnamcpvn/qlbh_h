import { AfterViewInit, Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconButton } from '@constants/common.constants';
import { ChiTietDonHang, DonHang } from '@models/maintains/don-hang';
import { DonHangService } from '@services/don-hang.service';
import { ToVietnameseService } from '@services/to-vietnamese.service';
import { InjectBase } from "@utilities/inject-base-app";
import { NhanVienService } from '@services/nhan-vien.service';
import { NhanVien } from '@models/maintains/nhan-vien';
import { CuaHangService } from '@services/cua-hang.service';
import { CuaHang } from '@models/maintains/cua-hang';

@Component({
  selector: 'app-chi-tiet',
  templateUrl: './chi-tiet.component.html',
  styleUrls: ['./chi-tiet.component.scss']
})
export class ChiTietComponent extends InjectBase implements OnInit, AfterViewInit {
  iconButton = IconButton;
  id: number;
  tienChu: string = '';
  donHang: DonHang;
  listChiTiet: ChiTietDonHang[] = [];
  nhanViens: NhanVien[] = [];
  cuaHang: CuaHang = <CuaHang>{};
  tongSL: number;
  get printDate() {
    if (this.donHang.date) {
      return new Date(this.donHang.date);
    } else return new Date();
  }
  get ten_NV() {
    if (this.donHang.iD_NV != null && this.nhanViens.length > 0) {
      const nvId = this.donHang.iD_NV;
      return this.nhanViens.find(x => x.id === nvId)?.ten || '';
    }
    return '';
  }
  constructor(
    private donHangService: DonHangService,
    private nvService: NhanVienService,
    private shopService: CuaHangService,
    private route: ActivatedRoute,
    private toVNService: ToVietnameseService
  ) { super() }

  ngOnInit() {
    this.donHangService.current_DH.subscribe({
      next: res => {
        if (res)
          this.donHang = res
        else this.router.navigate(['/maintain/ban-hang'])
      },
      error: err => this.router.navigate(['/maintain/ban-hang'])
    })
    this.getAllNV();
    this.getShop();
    this.tienChu = this.toVNService.toVietnamese(this.donHang.tongTien)
    this.tienChu = this.tienChu.charAt(0).toUpperCase() + this.tienChu.slice(1);
  }

  ngAfterViewInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.getData();
  }

  getData() {
    this.donHangService.getDetail(this.id).subscribe({
      next: res => {
        this.listChiTiet = res;
        this.tongSL = this.listChiTiet.reduce((x, y) => x + y.soLuong, 0);
      }
    })
  }

  back() {
    this.router.navigate(['/maintain/ban-hang']);
  }

  update() {
    this.router.navigate(['/maintain/ban-hang/edit', this.donHang.id]);
  }
  getAllNV() {
    this.nvService.getAll().subscribe({
      next: (res) => {
        this.nhanViens = res;
      }
    })
  }
  getShop() {
    this.shopService.getFirst().subscribe({
      next: (res) => {
        this.cuaHang = res || <CuaHang>{};
      }
    })
  }
}

