import { Component, OnInit } from '@angular/core';
import { CongDoan } from "@models/maintains/cong-doan";
import { Pagination } from '@utilities/pagination-utility';
import { BsModalRef } from "ngx-bootstrap/modal";
import { ModalService } from "@services/modal.service";
import { CdService } from "@services/cd.service";
import { InjectBase } from "@utilities/inject-base-app";
import { PageChangedEvent } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent extends InjectBase implements OnInit {
  data: CongDoan[] = [];
  pagination: Pagination = <Pagination>{
    pageNumber: 1,
    pageSize: 10
  };
  name: string = '';

  editData: CongDoan = <CongDoan>{};
  type: string = 'add';
  modalRef?: BsModalRef;
  constructor(private modalService: ModalService, private cdService: CdService) {
    super();
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.cdService.getDataPagination(this.pagination, this.name).subscribe({
      next: (res) => {
        this.data = res.result;
        this.pagination = res.pagination;
      }
    })
  }

  search() {
    this.pagination.pageNumber !== 1 ? this.pagination.pageNumber = 1 : this.getData();
  }

  pageChanged(e: PageChangedEvent) {
    this.pagination.pageNumber = e.page;
    this.getData();
  }

  openModal(id: string, cd?: CongDoan) {
    this.type = cd ? 'edit' : 'add';
    this.editData = cd ? cd : <CongDoan>{};
    this.modalService.open(id);
  }

  changeData(e: any) {
    this.search();
  }

  delete(id: number) {
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa công đoạn', 'Xóa',
      () => {
        this.spinnerService.show();
        this.cdService.delete(id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success('Xóa công đoạn Thành Công', 'Thành công');
              this.data = this.data.filter(x => x.id !== id);
              this.spinnerService.hide();
            }
          },
          error: () => {
            this.snotifyService.error('Xóa công đoạn thất bại', 'Lỗi');
            this.spinnerService.hide();
          }
        })
      });
  }

  clear() {
    this.name = '';
    this.search();
  }
}
