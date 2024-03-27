import { AfterViewInit, Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconButton } from '@constants/common.constants';
import { ChiTietDonHang, DonHang, DonHangDTO } from '@models/maintains/don-hang';
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
import { ModalService } from '@services/modal.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditComponent extends InjectBase implements OnInit, AfterViewInit {
  iconButton = IconButton;
  bsConfig: Partial<BsDatepickerConfig> = <Partial<BsDatepickerConfig>>{
    dateInputFormat: 'DD/MM/YYYY'
  };
  donHang: DonHang = <DonHang>{};
  checkDate: boolean = false;
  data: DonHangDTO = <DonHangDTO>{};
  chiTiet: ChiTietDonHang = <ChiTietDonHang>{}
  listChiTiet: ChiTietDonHang[] = [];
  khachHangs: KhachHang[] = [];
  sanPhams: SanPham[] = [];
  tenSP: string = '';
  giaSP: number;
  mahangs: MaHang[] = [];
  idSP: number;
  tongTien: number;
  id: number;
  type: string = 'add';
  dvt: string = '';
  modalRef?: BsModalRef;
  constructor(
    private donHangService: DonHangService,
    private khService: KhachHangService,
    private spService: SanPhamService,
    private modalService: BsModalService,
    private route: ActivatedRoute) {
    super();
  }

  ngOnInit() {
    this.donHangService.current_DH.subscribe({
      next: res => {
        if(res){
          this.donHang = res;
          this.data.iD_KH = res.iD_KH;
          this.tongTien = res.tongTien;
          console.log(" Trang Edit:", this.donHang);

        }

        else this.router.navigate(['/maintain/mua-hang'])
      },
      error: err => this.router.navigate(['/maintain/mua-hang'])
    })
    this.getAllKH();
    this.getAllSP();
    this.clear();
  }

  ngAfterViewInit(): void {
    this.id = this.route.snapshot.params['id'];
    if (this.id) {
      this.getDetail();
    }
  }

  getDetail() {
    this.donHangService.getDetail(this.id).subscribe({
      next: res => {
        this.listChiTiet = res;
       // this.tongSL = this.listChiTiet.reduce((x, y) => x + y.soLuong, 0);
      }
    })
  }
  getAllKH() {
    this.khService.getAll().subscribe({
      next: (res) => {
        this.khachHangs = res;
      }
    })
  }

  mhChanges(id) {
    let item = this.sanPhams.find(x=>x.id == id);
    this.giaSP = item.gia;
    this.tenSP = item.ten;
    this.chiTiet.ten_SP = item.ten;
    this.chiTiet.iD_SP = item.id;
    this.dvt = item.dvt;
  }

  getAllSP() {
    this.spService.getAll().subscribe({
      next: (res) => {
        this.sanPhams = res;
      }
    })
  }

  add() {
    this.chiTiet.gia = this.giaSP;
    this.chiTiet.dvt = this.dvt;
    this.chiTiet.thanhTien = this.chiTiet.soLuong * this.chiTiet.gia;
    console.log(this.listChiTiet.some(x => x.iD_SP == this.chiTiet.iD_SP));

    if(this.listChiTiet.some(x => x.iD_SP == this.chiTiet.iD_SP))

      this.listChiTiet.map(x => {
        if(x.iD_SP == this.chiTiet.iD_SP){
          x.soLuong += this.chiTiet.soLuong;
          x.thanhTien = x.soLuong * x.gia;
        }
    })
    else this.listChiTiet.push(this.chiTiet)


    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    },0)
    this.idSP = null;
    this.tenSP = '';
    this.giaSP = null;
    this.chiTiet = <ChiTietDonHang>{};
  }

  update() {
    this.data.id = this.donHang.id;
    this.data.loai = this.donHang.loai;
    this.data.ten_KH = this.khachHangs.find(x=> x.id == this.data.iD_KH).ten;
    this.data.tongTien = this.tongTien;

    this.data.chitiet = this.listChiTiet;
    this.donHangService.update(this.data).subscribe({
      next: (res) => {
        console.log("Thêm đh: ",res);
        if (res) {
          this.snotifyService.success('Sửa đơn hàng thành công', 'Thành công');
          this.donHangService.changeSDonHang(this.data);
          this.router.navigate(['/maintain/mua-hang/detail', res.id]);
        }
        else
          this.snotifyService.success('Sửa đơn hàng không thành công', 'Lỗi');
      }
    })
  }

  checktime() {
    this.checkDate = this.functionUtility.checkEmpty(this.checkDate);
  }

  back() {
    this.router.navigate(['/maintain/mua-hang']);
  }

  delete(item: ChiTietDonHang) {
    this.listChiTiet = this.listChiTiet.filter(x=> x!= item);
    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    },0)
  }

  openModal(template: TemplateRef<void>, item: ChiTietDonHang) {
    this.chiTiet = item;
    this.modalRef = this.modalService.show(template);
  }
  cancel(){

  }

  saveModal(){
    this.listChiTiet.map(x=> {
      if(x.id == this.chiTiet.id) {
        x.gia = this.chiTiet.gia;
        x.soLuong = this.chiTiet.soLuong;
        x.thanhTien = x.gia * x.soLuong;
      }
    })
    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    },0)
    this.donHang.ten_KH = this.data.ten_KH;
    this.donHang.tongTien = this.tongTien;
    this.donHangService.changeSDonHang(this.donHang)
    this.modalRef?.hide();
  }



  clear() {
    // this.data = <ChiTietDonHang>{
    //   date: new Date()
    // };
    // this.idMH = null;
  }
}
