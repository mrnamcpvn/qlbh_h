import { AfterViewInit, Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconButton } from '@constants/common.constants';
import { ChiTietDonHang, DonHang, DonHangDTO } from '@models/maintains/don-hang';
import { SanPham } from '@models/maintains/san-pham';
import { MaHang } from '@models/maintains/ma-hang';
import { SanPhamService } from '@services/san-pham.service';
import { DonHangService } from '@services/don-hang.service';
import { InjectBase } from '@utilities/inject-base-app';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NhaCungCapService } from '@services/nha-cung-cap.service';
import { NhaCungCap } from '@models/maintains/nha-cung-cap';

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
  nhaCungCaps: NhaCungCap[] = [];
  sanPhams: SanPham[] = [];
  tenSP: string = '';
  giaSP: number;
  soLuong: number;
  slMax: number | null = null;
  mahangs: MaHang[] = [];
  idSP: number;
  tongTien: number;
  id: number;
  type: string = 'add';
  dvt: string = '';
  date: Date;
  modalRef?: BsModalRef;
  constructor(
    private donHangService: DonHangService,
    private service: NhaCungCapService,
    private spService: SanPhamService,
    private modalService: BsModalService,
    private route: ActivatedRoute) {
    super();
  }

  ngOnInit() {
    this.donHangService.current_DH.subscribe({
      next: res => {
        if (res) {
          this.donHang = res;
          this.data.iD_NCC = res.iD_NCC;
          if (res.date)
            this.date = new Date(res.date);
          this.tongTien = res.tongTien;
        }

        else this.router.navigate(['/maintain/mua-hang'])
      },
      error: err => this.router.navigate(['/maintain/mua-hang'])
    })
    this.getAllNCC();
    this.getAllSP();
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
        this.listChiTiet.forEach((item, index) => {
          item.stt = index + 1;
        });
      }
    })
  }
  getAllNCC() {
    this.service.getAll().subscribe({
      next: (res) => {
        this.nhaCungCaps = res;
      }
    })
  }

  mhChanges(id) {
    this.clearSP();
    let item = this.sanPhams.find(x => x.id == id);
    this.giaSP = item.gia;
    this.tenSP = item.ten;
    this.chiTiet.ten_SP = item.ten;
    this.chiTiet.iD_SP = item.id;
    this.dvt = item.dvt;
    this.slMax = item.soLuong;
  }

  getAllSP() {
    this.spService.getAll().subscribe({
      next: (res) => {
        this.sanPhams = res;
      }
    })
  }
  clearSP() {
    this.giaSP = null;
    this.tenSP = '';
    this.chiTiet.ten_SP = '';
    this.chiTiet.iD_SP = null;
    this.dvt = '';
    this.slMax = null;
  }

  add() {
    this.chiTiet.gia = this.giaSP;
    this.chiTiet.dvt = this.dvt;
    this.chiTiet.soLuong = this.soLuong;
    this.chiTiet.thanhTien = this.chiTiet.soLuong * this.chiTiet.gia;
    this.chiTiet.stt = this.listChiTiet.length + 1;

    this.listChiTiet.push({ ...this.chiTiet });

    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    }, 0)
    this.idSP = null;
    this.tenSP = '';
    this.giaSP = null;
    this.chiTiet = <ChiTietDonHang>{};
    this.slMax = null;
  }

  update() {
    this.data.id = this.donHang.id;
    this.data.loai = this.donHang.loai;
    this.data.tongTien = this.tongTien;
    this.data.chitiet = this.listChiTiet;
    if (this.date)
      this.data.date_Str = this.functionUtility.getDateFormat(this.date)
    this.donHangService.update(this.data).subscribe({
      next: (res) => {
        if (res) {
          this.snotifyService.success('Sửa đơn hàng thành công', 'Thành công');
          this.donHangService.changeSDonHang(res);
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

  deleteItem(item: ChiTietDonHang) {
    this.snotifyService.confirm("Bạn có chắc chắc muốn xóa?", "Xóa",
      () => {

        this.donHangService.deleteItem(item.id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success("Xóa thành công", "Thành công");
              this.listChiTiet = this.listChiTiet.filter(x => x != item);
              this.tongTien = this.listChiTiet.reduce((tt, item) => {
                return tt + (item.gia * item.soLuong);
              }, 0)
            } else this.snotifyService.warning("Xóa không thành công", "Cảnh báo");
          },
          error: (err) => this.snotifyService.error(err, "Lỗi")
        })
      }
    )
  }

  openModal(template: TemplateRef<void>, item: ChiTietDonHang) {
    this.chiTiet = { ...item };
    this.modalRef = this.modalService.show(template);
  }
  cancel() {
    this.modalRef.hide();
  }

  saveModal() {
    const index = this.listChiTiet.findIndex(x => x.stt === this.chiTiet.stt);
    if (index !== -1) {
      this.listChiTiet[index].gia = this.chiTiet.gia;
      this.listChiTiet[index].soLuong = this.chiTiet.soLuong;
      this.listChiTiet[index].thanhTien = this.listChiTiet[index].gia * this.listChiTiet[index].soLuong;
    }
    this.tongTien = this.listChiTiet.reduce((tt, item) => {
      return tt + (item.gia * item.soLuong);
    }, 0)
    this.donHang.tongTien = this.tongTien;
    this.donHangService.changeSDonHang(this.donHang)
    this.modalRef?.hide();
  }
}
