import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconButton } from '@constants/common.constants';
import { DonHangDTO } from '@models/maintains/don-hang';
import { SanPham } from '@models/maintains/san-pham';
import { MaHang } from '@models/maintains/ma-hang';
import { KhachHang } from '@models/maintains/nld';
import { CdService } from '@services/cd.service';
import { DonHangService } from '@services/don-hang.service';
import { MaHangService } from '@services/mahang.service';
import { NldService } from '@services/nld.service';
import { InjectBase } from '@utilities/inject-base-app';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

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
  nguoilaodongs: KhachHang[] = [];
  SanPhams: SanPham[] = [];
  mahangs: MaHang[] = [];
  idMH: number;
  id: number;
  type: string = 'add';
  constructor(
    private donHangService: DonHangService,
    private nldService: NldService,
    private cdService: CdService,
    private mhService: MaHangService,
    private route: ActivatedRoute) {
    super();
  }

  ngOnInit() {
    this.getAllNLD();
    this.getAllMH();
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

  getAllNLD() {
    this.nldService.getAll().subscribe({
      next: (res) => {
        this.nguoilaodongs = res;
      }
    })
  }

  getAllMH() {
    this.mhService.getAll().subscribe({
      next: (res) => {
        this.mahangs = res;
      }
    })
  }

  mhChanges(event) {
    this.getAllCD(event);
  }

  getAllCD(id: number) {
    this.cdService.getAllByCommodityCodeId(id).subscribe({
      next: (res) => {
        this.SanPhams = res;
      }
    })
  }

  create(type: string) {
    // this.data.date = this.functionUtility.getUTCDate(new Date(this.data.date));
    // this.chamcongService.create(this.data).subscribe({
    //   next: (res) => {
    //     if (res) {
    //       this.snotifyService.success('Chấm công thành công', 'Thành công');
    //       type == 'SAVE' ? this.back() : this.clear();
    //     }
    //     else
    //       this.snotifyService.success('Chấm công không thành công', 'Lỗi');
    //   }
    // })
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
    this.router.navigate(['/maintain/cham-cong']);
  }

  clear() {
    // this.data = <ChiTietDonHang>{
    //   date: new Date()
    // };
    // this.idMH = null;
  }
}
