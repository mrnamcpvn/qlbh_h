import {
  TheoDoiNhanVienBanHang_Param,
  TheoDoiNhanVienBanHang_Data
} from '@models/maintains/theo-doi-nhan-vien-ban-hang';
import { Component, OnInit, ViewChild } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { InjectBase } from "@utilities/inject-base-app";
import { ClassButton, IconButton } from "@constants/common.constants";
import { Pagination } from '@utilities/pagination-utility';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { KeyValuePair } from '@utilities/key-value-pair';
import { TheoDoiNhanVienBanHangService } from '@services/theo-doi-nhan-vien-ban-hang.service';
import { FormGroup, NgForm } from '@angular/forms';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent extends InjectBase implements OnInit {
  @ViewChild('mainForm') public mainForm: NgForm;
  iconButton = IconButton;
  classButton = ClassButton
  bsConfig: Partial<BsDatepickerConfig> = {
    dateInputFormat: "DD/MM/YYYY",
    isAnimated: true,
  }
  now: Date = new Date();
  fromDate: Date;
  toDate: Date;
  param: TheoDoiNhanVienBanHang_Param = <TheoDoiNhanVienBanHang_Param>{}
  data: TheoDoiNhanVienBanHang_Data = <TheoDoiNhanVienBanHang_Data>{};
  filterByList: KeyValuePair[] = [
    { key: "0", value: 'Khoảng thời gian' },
    { key: "1", value: 'Năm' },
    { key: "2", value: 'Quý' },
    { key: "3", value: 'Tháng' },
    { key: "4", value: 'Tuần' },
    { key: "5", value: 'Ngày' },
  ];
  sPList: KeyValuePair[] = []
  nVList: KeyValuePair[] = []
  constructor(private service: TheoDoiNhanVienBanHangService) {
    super();
  }

  ngOnInit() {
    this.getListSanPham()
    this.getListNhanVien()
    this.clear();
  }

  getData() {
    if (this.mainForm) {
      const form: FormGroup = this.mainForm.form
      if (form.valid) {
        this.spinnerService.show();
        this.param.fromDate_Str = this.functionUtility.getDateFormat(this.fromDate)
        this.param.toDate_Str = this.functionUtility.getDateFormat(this.toDate)
        this.service.getDataPagination(this.data.pagination, this.param).subscribe({
          next: (res) => {
            this.data = res;
            this.spinnerService.hide();
          }
        });
      }
      else {
        this.data = <TheoDoiNhanVienBanHang_Data>{
          result: [],
          pagination: <Pagination>{
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0
          }
        };
      }
    }
  }
  search() {
    this.data.pagination.pageNumber !== 1 ? this.data.pagination.pageNumber = 1 : this.getData();
  }

  pageChanged(e: PageChangedEvent) {
    this.data.pagination.pageNumber = e.page;
    this.getData();
  }
  getListSanPham() {
    this.service.getListSanPham().subscribe({
      next: (res) => {
        this.sPList = res;
        this.functionUtility.getNgSelectAllCheckbox(this.sPList)
      }
    });
  }
  getListNhanVien() {
    this.service.getListNhanVien().subscribe({
      next: (res) => {
        this.nVList = res;
        this.functionUtility.getNgSelectAllCheckbox(this.nVList)
      }
    });
  }
  excel() {
    this.spinnerService.show();
    this.service.excel(this.param).subscribe({
      next: (result) => {
        this.spinnerService.hide();
        result.isSuccess
          ? this.functionUtility.exportExcel(result.data, `Báo cáo theo dõi nhân viên bán hàng`)
          : this.snotifyService.error(result.error, 'Lỗi');
      },
    });
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
        this.toDate = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate() + 6);
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

  clear() {
    // First of month
    this.fromDate = new Date(this.now.getFullYear(), this.now.getMonth(), 1)
    this.toDate = new Date(this.now.getFullYear(), this.now.getMonth() + 1, 0)
    this.param = <TheoDoiNhanVienBanHang_Param>{
      filterBy: '0',
      idSP: [],
      idNV: []
    }
    this.data = <TheoDoiNhanVienBanHang_Data>{
      result: [],
      pagination: <Pagination>{
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0
      }
    };
  }
}
