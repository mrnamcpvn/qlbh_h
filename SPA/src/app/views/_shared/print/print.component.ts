import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { CuaHang } from '@models/maintains/cua-hang';
import { ChiTietDonHang, DonHang } from '@models/maintains/don-hang';

@Component({
  selector: 'app-print',
  templateUrl: './print.component.html',
  styleUrls: ['./print.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PrintComponent implements OnInit {
  @Input() type: 'ban-hang' | 'mua-hang' = 'ban-hang';
  @Input() cuaHang: CuaHang = <CuaHang>{};
  @Input() donHang: DonHang;
  @Input() listChiTiet: ChiTietDonHang[] = [];
  @Input() tongSL: number;
  @Input() tienChu: string = '';
  @Input() ten_NV: string = '';

  get printDate() {
    if (this.donHang?.date) {
      return new Date(this.donHang.date);
    } else return new Date();
  }

  constructor() { }

  ngOnInit(): void {
  }
}
