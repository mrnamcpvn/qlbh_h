import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { DonHang, ChiTietDonHang, DonHangFilter } from "@models/maintains/don-hang";
import { DonHangService } from "@services/don-hang.service";
import { InjectBase } from "@utilities/inject-base-app";
import { IconButton } from "@constants/common.constants";
import { Pagination } from '@utilities/pagination-utility';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { ModalService } from '@services/modal.service';
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
  tongTien: number;
  data: DonHang[] = [];
  editData: DonHang = <DonHang>{};
  param: any = {
    filterType: null,
    soHoaDon: null,
    payType: '3'
  };
  thanhToanList: KeyValuePair[] = [
    { key: '1', value: 'Tiền Mặt' },
    { key: '2', value: 'Chuyển Khoản' },
    { key: '3', value: 'Tất cả' },
  ];
  filterTypeList: KeyValuePair[] = [
    {
      key: '1',
      value: 'Theo Năm',
    },
    {
      key: '2',
      value: 'Theo Quý',
    },
    {
      key: '3',
      value: 'Theo Tháng',
    },
    {
      key: '4',
      value: 'Theo Tuần',
    },
    {
      key: '5',
      value: 'Theo Ngày',
    }
  ];
  constructor(private donHangService: DonHangService, private modalService: ModalService) {
    super();
  }

  openModal(id: string, item: DonHang) {
    this.editData = item;
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
      loai: 1,
      payType: this.param.payType
    };
    this.donHangService.getDataPagination(filter).subscribe({
      next: (res) => {
        this.data = res.result;
        this.pagination = res.pagination;
        this.tongTien = this.data.reduce((x, y) => x + y.tongTien, 0);
        this.spinnerService.hide();
      }
    });
  }

  search() {
    this.pagination.pageNumber !== 1 ? this.pagination.pageNumber = 1 : this.getData();
  }
  excel() {
    this.spinnerService.show();
    const filter: DonHangFilter = {
      fromDate: this.fromDate,
      toDate: this.toDate,
      loai: 1,
      payType: this.param.payType
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

  pageChanged(e: PageChangedEvent) {
    this.pagination.pageNumber = e.page;
    this.getData();
  }

  add() {
    this.router.navigate(['/maintain/mua-hang/add']);
  }

  detail(item: DonHang) {
    this.donHangService.changeSDonHang(item);
    this.router.navigate(['/maintain/mua-hang/detail', item.id]);
  }

  delete(id: number) {
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa đơn hàng', 'Xóa',
      () => {
        this.spinnerService.show();
        this.donHangService.delete(id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success('Xóa đơn hàng thành công', 'Thành Công');
              this.data = this.data.filter(x => x.id !== id);
              this.tongTien = this.data.reduce((x, y) => x + y.tongTien, 0);
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
    this.search();
  }
  onFilterTypeChange() {
    if (!this.fromDate) return;
    const referenceDate = new Date(this.fromDate);
    const year = referenceDate.getFullYear();
    const month = referenceDate.getMonth();

    switch (this.param.filterType) {
      case '1': // Theo Năm
        this.fromDate = new Date(year, 0, 1);
        this.toDate = new Date(year, 11, 31);
        break;
      case '2': // Theo Quý
        const quarterStartMonth = Math.floor(month / 3) * 3;
        this.fromDate = new Date(year, quarterStartMonth, 1);
        this.toDate = new Date(year, quarterStartMonth + 3, 0);
        break;
      case '3': // Theo Tháng
        this.fromDate = new Date(year, month, 1);
        this.toDate = new Date(year, month + 1, 0);
        break;
      case '4': // Theo Tuần
        const day = referenceDate.getDay();
        const diff = referenceDate.getDate() - day + (day === 0 ? -6 : 1);
        this.fromDate = new Date(year, month, diff);
        this.toDate = new Date(year, month, diff + 6);
        break;
      case '5': // Theo Ngày
        this.fromDate = referenceDate;
        this.toDate = referenceDate;
        break;
    }
    this.search();
  }
  changeData($event) {
    this.search();
  }
}
