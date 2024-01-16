import { Component, OnInit } from '@angular/core';
import { ModalService } from '@services/modal.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { NldService } from '@services/nld.service';
import { Pagination } from '@utilities/pagination-utility';
import { NguoiLaoDong } from '@models/maintains/nld';
import { PageChangedEvent } from "ngx-bootstrap/pagination";
import { InjectBase } from "@utilities/inject-base-app";
import { IconButton } from '@constants/common.constants';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent extends InjectBase implements OnInit {
  pagination: Pagination = <Pagination>{
    pageNumber: 1,
    pageSize: 10
  };
  name: string = '';
  data: NguoiLaoDong[] = [];
  editData: NguoiLaoDong = <NguoiLaoDong>{};
  type: string = 'add';
  modalRef?: BsModalRef;
  iconButton = IconButton;
  constructor(private modalService: ModalService, private nldService: NldService) {
    super();
  }

  ngOnInit(): void {
    this.search();
  }

  openModal(id: string, nld?: NguoiLaoDong) {
    this.type = nld ? 'edit' : 'add';
    this.editData = nld ? nld : <NguoiLaoDong>{};
    this.modalService.open(id);
  }

  changeData($event) {
    this.search();
  }

  getData() {
    this.spinnerService.show();
    this.nldService.getDataPagination(this.pagination, this.name).subscribe({
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

  delete(id: number) {
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa người lao động', 'Xóa',
      () => {
        this.spinnerService.show();
        this.nldService.delete(id).subscribe({
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
    this.name = '';
    this.search();
  }

}
