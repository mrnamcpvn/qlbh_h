import { Component, OnInit } from '@angular/core';
import { Pagination } from '@utilities/pagination-utility';
import { ReportService } from '@services/report.service';
import { InjectBase } from '@utilities/inject-base-app';
import { DatePipe } from '@angular/common';
import { CaptionConstants, MessageConstants } from '@constants/message.enum';
import { IconButton } from '@constants/common.constants';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { KhachHang } from '@models/maintains/khach-hang';
import { Report, ReportExportDetail, ReportMainParam } from '@models/reports/report-main';
import { KhachHangService } from '@services/khach-hang.service';
import { SanPhamService } from '@services/san-pham.service';
import { SanPham } from '@models/maintains/san-pham';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  providers: [DatePipe]
})
export class MainComponent extends InjectBase implements OnInit {
  now: Date = new Date();
  pagination: Pagination = <Pagination>{
    pageNumber: 1,
    pageSize: 10
  };
  param: ReportMainParam = <ReportMainParam>{};
  data: Report[] = [];
  iconButton = IconButton;
  bsConfig: Partial<BsDatepickerConfig> = {
    dateInputFormat: "DD/MM/YYYY",
    isAnimated: true,
  }
  test = [1,2,3,4];
  listSP: SanPham[] = [];
  constructor(
    private reportService: ReportService,
    private datePipe: DatePipe,
    private spService: SanPhamService
  ) {
    super();
  }

  ngOnInit(): void {
    this.getAllSP();
    this.clearSearch();
  }

  getData() {
    this.spinnerService.show();
    this.reportService.getDatapagination(this.pagination, this.param)
      .subscribe({
        next: (res) => {
          this.pagination = res.pagination;
          this.data = res.result;
        },
        error: () => this.snotifyService.error(MessageConstants.UN_KNOWN_ERROR, CaptionConstants.ERROR),
        complete: () => this.spinnerService.hide()
      });
  }

  search() {
    this.pagination.pageNumber == 1 ? this.getData() : this.pagination.pageNumber = 1;
  }

  getAllSP() {
    this.spService.getAll().subscribe({
      next: (res) => {
        this.listSP = res;
        let sp = <SanPham> {id: 0, ten: 'Tất cả sản phẩm'}
        this.listSP.unshift(sp);
      }
    })
  }

  clearSearch() {
    this.param = <ReportMainParam>{
      fromDate: new Date(this.now.getFullYear(), this.now.getMonth(), 1),
      toDate: new Date(this.now.getFullYear(), this.now.getMonth() + 1, 0),
      id_sp: 0
    };
    this.data = [];
  }

  pageChanged(event: any) {
    this.pagination.pageNumber = event.page;
    this.getData();
  }

  exportExcel() {
    // this.spinnerService.show();
    // this.reportService.exportExcel(this.pagination, this.param)
    //   .subscribe({
    //     next: (result: Blob) => {
    //       const fileName = `Bao_cao_${this.datePipe.transform(new Date(), 'yyyyMMdd_HHmmss')}`;
    //       this.functionUtility.exportExcel(result, fileName);
    //     },
    //     error: () => this.snotifyService.error(MessageConstants.UN_KNOWN_ERROR, CaptionConstants.ERROR),
    //     complete: () => this.spinnerService.hide()
    //   });
  }
}
