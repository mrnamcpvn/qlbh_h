import { Component, OnInit } from '@angular/core';
import { Pagination } from '@utilities/pagination-utility';
import { ReportService } from '@services/report.service';
import { InjectBase } from '@utilities/inject-base-app';
import { DatePipe } from '@angular/common';
import { CaptionConstants, MessageConstants } from '@constants/message.enum';
import { IconButton } from '@constants/common.constants';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { KhachHang } from '@models/maintains/nld';
import { Report, ReportMainParam } from '@models/reports/report-main';
import { NldService } from '@services/nld.service';

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
  nlds: KhachHang[] = [];
  constructor(
    private reportService: ReportService,
    private datePipe: DatePipe,
    private nldService: NldService
  ) {
    super();
  }

  ngOnInit(): void {
    //this.getAllNLD();
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

  // getAllNLD() {
  //   this.nldService.getAll().subscribe({
  //     next: (res) => {
  //       this.nlds = res;
  //       this.nlds.unshift({ id: 0, name: 'Chọn NLĐ...'});
  //     }
  //   })
  // }

  clearSearch() {
    this.param = <ReportMainParam>{
      fromDate: new Date(this.now.getFullYear(), this.now.getMonth(), 1),
      toDate: new Date(this.now.getFullYear(), this.now.getMonth() + 1, 0),
      id_NLD: 0
    };
    this.data = [];
  }

  pageChanged(event: any) {
    this.pagination.pageNumber = event.page;
    this.getData();
  }

  exportExcel() {
    this.spinnerService.show();
    this.reportService.exportExcel(this.pagination, this.param)
      .subscribe({
        next: (result: Blob) => {
          const fileName = `Bao_cao_${this.datePipe.transform(new Date(), 'yyyyMMdd_HHmmss')}`;
          this.functionUtility.exportExcel(result, fileName);
        },
        error: () => this.snotifyService.error(MessageConstants.UN_KNOWN_ERROR, CaptionConstants.ERROR),
        complete: () => this.spinnerService.hide()
      });
  }
}
