import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ModalService } from '@services/modal.service';
import { DonHang, ChiTietDonHang, DonHangFilter } from "@models/maintains/don-hang";
import { DonHangService } from "@services/don-hang.service";
import { InjectBase } from "@utilities/inject-base-app";
import { IconButton } from "@constants/common.constants";
import { Pagination } from '@utilities/pagination-utility';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { KeyValuePair } from '@utilities/key-value-pair';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent extends InjectBase implements OnInit {
  iconButton = IconButton;
  bsConfig: Partial<BsDatepickerConfig> = {
    dateInputFormat: "DD/MM/YYYY",
    isAnimated: true,
  }
  now: Date = new Date();
  fromDate: string | Date;
  toDate: string | Date;
  pagination: Pagination = <Pagination>{
    pageNumber: 1,
    pageSize: 10
  };
  param: any = {
    filterBy: '0',
    tinhTrang:'3',
    ma_DH: null,
    payType: '3'
  };
  tinhTrangList: KeyValuePair[] = [
    { key: '1', value: 'Đã Thanh Toán' },
    { key: '2', value: 'Chưa Thanh Toán' },
    { key: '3', value: 'Tất cả' },
  ];
  thanhToanList: KeyValuePair[] = [
    { key: '1', value: 'Tiền Mặt' },
    { key: '2', value: 'Chuyển Khoản' },
    { key: '3', value: 'Tất cả' },
  ];
  filterByList: KeyValuePair[] = [
    { key: "0", value: 'Khoảng thời gian' },
    { key: "1", value: 'Năm' },
    { key: "2", value: 'Quý' },
    { key: "3", value: 'Tháng' },
    { key: "4", value: 'Tuần' },
    { key: "5", value: 'Ngày' },
  ];
  tongTien: number;
  tongTienTT: number;
  data: DonHang[] = [];
  editData: DonHang = <DonHang>{};
  constructor(private donHangService: DonHangService, private modalService: ModalService) {
    super();
  }
  openModal(id: string, item: DonHang) {
    this.editData = { ...item };
    this.modalService.open(id);
  }

  ngOnInit() {
    this.clear();
  }

  getData() {
    this.spinnerService.show();
    const filter: DonHangFilter = {
      pagination: this.pagination,
      fromDate: this.fromDate,
      toDate: this.toDate,
      loai: 2,
      payType: this.param.payType,
      tinhTrang: this.param.tinhTrang,
      ma_DH: this.param.ma_DH
      };
    this.donHangService.getDataPagination(filter).subscribe({
      next: (res) => {
        this.data = res.pagination.result;
        this.pagination = res.pagination.pagination;
        this.tongTien = res.totalAmount;
        //this.tongTienTT = this.data.reduce((x, y) => x + (y.tienMat || 0) + (y.chuyenKhoan || 0), 0);
        this.spinnerService.hide();
      },
      error: () => {
        this.spinnerService.hide();
      }
    });
  }
  excel() {
    this.spinnerService.show();
    const filter: DonHangFilter = {
      fromDate: this.fromDate,
      toDate: this.toDate,
      loai: 2,
      payType: this.param.payType,
      tinhTrang: this.param.tinhTrang,
      ma_DH: this.param.ma_DH
    };
    this.donHangService.excelExport(filter)
      .subscribe({
        next: (result) => {
          this.spinnerService.hide();
          if (!result)
            this.snotifyService.warning('Không có dử liệu', 'Cảnh báo!');
          else {
            this.functionUtility.exportExcel(result.data, 'Download_DonHang');
          }
        }
      });
  }
  search() {
    this.pagination.pageNumber = 1;
    this.getData();
  }

  pageChanged(e: PageChangedEvent) {
    this.pagination.pageNumber = e.page;
    this.getData();
  }

  add() {
    this.router.navigate(['/maintain/ban-hang/add']);
  }

  detail(item: DonHang) {
    this.donHangService.changeSDonHang(item);
    this.router.navigate(['/maintain/ban-hang/detail', item.id]);
  }

  delete(id: number) {
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa đơn hàng', 'Xóa',
      () => {
        this.spinnerService.show();
        this.donHangService.delete(id).subscribe({
          next: (res) => {
            if (res) {
              this.getData();
              this.snotifyService.success('Xóa đơn hàng thành công', 'Thành Công');
              this.spinnerService.hide();
            }
          },
          error: () => {
            this.snotifyService.error('Xóa đơn hàng thất bại', 'Lỗi');
            this.spinnerService.hide();
          }
        })
      });
  }

  clear() {
    // First of month
    this.fromDate = new Date(this.now.getFullYear(), this.now.getMonth(), 1);
    // Last of month
    this.toDate = new Date(this.now.getFullYear(), this.now.getMonth() + 1, 0);
    this.param.payType = '3';
    this.param.ma_DH = null;
    this.param.filterBy = '0';
    this.param.tinhTrang = '3';
    this.search();
  }

  deleteProperty(property: string) {
    delete this.param[property];
    this.search();
  }

  onFilterByChange() {
    if (this.fromDate) {
      this.calculateFromDateByFilterBy(new Date(this.fromDate));
      this.calculateToDateByFilterBy(new Date(this.fromDate));
    }
  }

  onFromDateChange() {
    if (this.fromDate) {
      this.calculateFromDateByFilterBy(new Date(this.fromDate));
      this.calculateToDateByFilterBy(new Date(this.fromDate));
    }
  }

  onToDateChange() {
    if (this.toDate) {
      this.calculateToDateByFilterBy(new Date(this.toDate));
      this.calculateFromDateByFilterBy(new Date(this.toDate));
    }
  }

  calculateToDateByFilterBy(startDate: Date) {
    switch (this.param.filterBy) {
      case '1': // Năm
        this.toDate = new Date(startDate.getFullYear(), 11, 31);
        break;
      case '2': // Quý
        const quarter = Math.floor(startDate.getMonth() / 3);
        this.toDate = new Date(startDate.getFullYear(), quarter * 3 + 3, 0);
        break;
      case '3': // Tháng
        this.toDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0);
        break;
      case '4': // Tuần
        const day = startDate.getDay();
        const diff = startDate.getDate() + (day === 0 ? 0 : 7 - day);
        this.toDate = new Date(startDate.getFullYear(), startDate.getMonth(), diff);
        break;
      case '5': // Ngày
        this.toDate = new Date(startDate);
        break;
      default:
        break;
    }
  }

  calculateFromDateByFilterBy(endDate: Date) {
    switch (this.param.filterBy) {
      case '1': // Năm
        this.fromDate = new Date(endDate.getFullYear(), 0, 1);
        break;
      case '2': // Quý
        const quarter = Math.floor(endDate.getMonth() / 3);
        this.fromDate = new Date(endDate.getFullYear(), quarter * 3, 1);
        break;
      case '3': // Tháng
        this.fromDate = new Date(endDate.getFullYear(), endDate.getMonth(), 1);
        break;
      case '4': // Tuần
        const day = endDate.getDay();
        const diff = endDate.getDate() - day + (day === 0 ? -6 : 1);
        this.fromDate = new Date(endDate.getFullYear(), endDate.getMonth(), diff);
        break;
      case '5': // Ngày
        this.fromDate = new Date(endDate);
        break;
      default:
        break;
    }
  }
  changeData($event) {
    this.search();
  }
}
