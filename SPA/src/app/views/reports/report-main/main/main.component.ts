import { Component, OnInit } from '@angular/core';
import { Pagination } from '@utilities/pagination-utility';
import { ReportService } from '@services/report.service';
import { InjectBase } from '@utilities/inject-base-app';
import { DatePipe } from '@angular/common';
import { CaptionConstants, MessageConstants } from '@constants/message.enum';
import { ClassButton, IconButton } from '@constants/common.constants';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Report_Data, ReportMainParam } from '@models/reports/report-main';
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

  param: ReportMainParam = <ReportMainParam>{};
  data: Report_Data = <Report_Data>{};
  iconButton = IconButton;
  classButton = ClassButton;
  bsConfig: Partial<BsDatepickerConfig> = {
    dateInputFormat: "DD/MM/YYYY",
    isAnimated: true,
  }
  test = [1, 2, 3, 4];
  listSP: SanPham[] = [];
  constructor(
    private service: ReportService,
    private spservice: SanPhamService
  ) {
    super();
  }

  ngOnInit(): void {
    this.getAllSP();
    this.clearSearch();
  }

  getData() {
    this.spinnerService.show();
    this.service.getData(this.param)
      .subscribe({
        next: (res) => {
          this.data = res;
        },
        error: () => this.snotifyService.error(MessageConstants.UN_KNOWN_ERROR, CaptionConstants.ERROR),
        complete: () => this.spinnerService.hide()
      });
  }

  getAllSP() {
    this.spservice.getAll().subscribe({
      next: (res) => {
        this.listSP = res;
        let sp = <SanPham>{ id: 0, ten: 'Tất cả sản phẩm' }
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
    this.data = <Report_Data>{ result: [] };
  }
  excel() {
    this.spinnerService.show();
    this.service.excel(this.param).subscribe({
      next: (result) => {
        this.spinnerService.hide();
        result.isSuccess
          ? this.functionUtility.exportExcel(result.data, `Báo cáo tồn kho`)
          : this.snotifyService.error(result.error, 'Lỗi');
      },
    });
  }
}
