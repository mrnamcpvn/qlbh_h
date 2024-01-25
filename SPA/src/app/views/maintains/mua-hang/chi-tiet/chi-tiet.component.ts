import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChiTietDonHang, DonHang } from '@models/maintains/don-hang';
import { DonHangService } from '@services/don-hang.service';
import { ToVietnameseService } from '@services/to-vietnamese.service';
import { InjectBase } from "@utilities/inject-base-app";

@Component({
  selector: 'app-chi-tiet',
  templateUrl: './chi-tiet.component.html',
  styleUrls: ['./chi-tiet.component.scss']
})
export class ChiTietComponent extends InjectBase implements OnInit, AfterViewInit {
  id: number;
  tienChu: string = '';
  donHang: DonHang;
  listChiTiet: ChiTietDonHang[] = [];
  constructor(
    private donHangService: DonHangService,
    private route: ActivatedRoute,
    private toVNService: ToVietnameseService
    ) { super() }

  ngOnInit() {
    this.donHangService.current_DH.subscribe({
      next: res => {
        if(res)
          this.donHang = res
        else this.router.navigate(['/maintain/mua-hang'])
      },
      error: err => this.router.navigate(['/maintain/mua-hang'])
    })

    this.tienChu = this.toVNService.toVietnamese(12345679)
    this.tienChu = this.tienChu.charAt(0).toUpperCase() + this.tienChu.slice(1);
  }

  ngAfterViewInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.getData();
  }

  getData() {
    this.donHangService.getDetail(this.id).subscribe({
      next: res => {
        console.log(res);
        this.listChiTiet = res;
        console.log(this.listChiTiet);

      }
    })
  }

  back() {
    this.router.navigate(['/maintain/mua-hang']);
  }

}

