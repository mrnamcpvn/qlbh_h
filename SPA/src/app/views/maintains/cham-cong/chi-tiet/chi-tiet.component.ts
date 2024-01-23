import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChiTietDonHang, DonHang } from '@models/maintains/don-hang';
import { DonHangService } from '@services/don-hang.service';
import { InjectBase } from "@utilities/inject-base-app";

@Component({
  selector: 'app-chi-tiet',
  templateUrl: './chi-tiet.component.html',
  styleUrls: ['./chi-tiet.component.scss']
})
export class ChiTietComponent extends InjectBase implements OnInit, AfterViewInit {
  id: number;
  donHang: DonHang;
  listChiTiet: ChiTietDonHang[] = [];
  constructor(
    private donHangService: DonHangService,
    private route: ActivatedRoute,
    ) { super() }

  ngOnInit() {
    this.donHangService.current_DH.subscribe({
      next: res => {
        if(res)
          this.donHang = res
        else this.router.navigate(['/maintain/cham-cong'])
      },
      error: err => this.router.navigate(['/maintain/cham-cong'])
    }

    )
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

  }

}

