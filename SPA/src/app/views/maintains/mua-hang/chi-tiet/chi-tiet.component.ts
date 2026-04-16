import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChiTietDonHang, DonHang, DonHangDTO } from '@models/maintains/don-hang';
import { DonHangService } from '@services/don-hang.service';
import { ToVietnameseService } from '@services/to-vietnamese.service';
import { InjectBase } from "@utilities/inject-base-app";
import { IconButton } from '@constants/common.constants';
import { CuaHang } from '@models/maintains/cua-hang';
import { CuaHangService } from '@services/cua-hang.service';

@Component({
  selector: 'app-chi-tiet',
  templateUrl: './chi-tiet.component.html',
  styleUrls: ['./chi-tiet.component.scss']
})
export class ChiTietComponent extends InjectBase implements OnInit, AfterViewInit {
  iconButton = IconButton;
  id: number;
  tienChu: string = '';
  tongSL: number;
  donHang: DonHang;
  listChiTiet: ChiTietDonHang[] = [];
  listChiTietGrouped: any[] = [];
  cuaHang: CuaHang = <CuaHang>{};
  get printDate() {
    if (this.donHang.date) {
      return new Date(this.donHang.date);
    } else return new Date();
  }
  constructor(
    private donHangService: DonHangService,
    private shopService: CuaHangService,
    private route: ActivatedRoute,
    private toVNService: ToVietnameseService
    ) { super() }

  ngOnInit() {
    this.donHangService.current_DH.subscribe({
      next: res => {
        if(res){
          this.donHang = res
        }
        else this.router.navigate(['/maintain/mua-hang'])
      },
      error: err => this.router.navigate(['/maintain/mua-hang'])
    })
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
        this.groupChiTiet();
        this.tongSL = this.listChiTiet.reduce((x, y) => x + y.soLuong, 0);
      }
    })
  }

  groupChiTiet() {
    const grouped = new Map<number, any>();
    this.listChiTiet.forEach(item => {
      const key = item.iD_SP;
      if (grouped.has(key)) {
        const existing = grouped.get(key);
        existing.soLuong += item.soLuong;
        existing.thanhTien += item.thanhTien;
      } else {
        grouped.set(key, { ...item });
      }
    });
    this.listChiTietGrouped = Array.from(grouped.values());
    this.listChiTietGrouped.forEach((item, index) => item.stt = index + 1);
  }

  update() {
    this.router.navigate(['/maintain/mua-hang/edit', this.donHang.id]);
  }

  back() {
    this.router.navigate(['/maintain/mua-hang']);
  }

  getShop() {
    this.shopService.getFirst().subscribe({
      next: (res) => {
        this.cuaHang = res || <CuaHang>{};
      }
    })
  }
}

