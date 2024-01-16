import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ChamCong, ChamCongDTO } from "@models/maintains/cham-cong";
import { ChamcongService } from "@services/chamcong.service";
import { InjectBase } from "@utilities/inject-base-app";
import { IconButton } from "@constants/common.constants";
import { Pagination } from '@utilities/pagination-utility';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';

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
  data: ChamCongDTO[] = [];
  constructor(private chamcongService: ChamcongService) {
    super();
  }

  ngOnInit() {
    this.clear();
  }

  getData() {
    this.spinnerService.show();
    this.chamcongService.getDataPagination(this.pagination, this.fromDate, this.toDate).subscribe({
      next: (res) => {
        this.data = res.result;
        this.pagination = res.pagination;
        this.spinnerService.hide();
      }
    });
  }

  search() {
    this.pagination.pageNumber !== 1 ? this.pagination.pageNumber = 1 : this.getData();
  }

  pageChanged(e: PageChangedEvent) {
    this.pagination.pageNumber = e.page;
    this.getData();
  }

  add() {
    this.router.navigate(['/maintain/cham-cong/add']);
  }

  edit(item: ChamCong) {
    this.router.navigate(['/maintain/cham-cong/edit', item.id]);
  }

  delete(id: number) {
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa mã hàng', 'Xóa',
      () => {
        this.spinnerService.show();
        this.chamcongService.delete(id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success('Xóa Người Lao Động Thành Công', 'Thành Công');
              this.data = this.data.filter(x => x.id !== id);
              this.spinnerService.hide();
            }
          },
          error: () => {
            this.snotifyService.error('Xóa Người Lao Động Thất bại', 'Lỗi');
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
}
