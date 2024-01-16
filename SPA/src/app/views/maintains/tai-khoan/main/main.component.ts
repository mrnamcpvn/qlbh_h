import { Component, OnInit } from '@angular/core';
import { ModalService } from '@services/modal.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Pagination } from '@utilities/pagination-utility';
import { NguoiDung } from '@models/maintains/nguoi-dung';
import { PageChangedEvent } from "ngx-bootstrap/pagination";
import { InjectBase } from "@utilities/inject-base-app";
import { IconButton } from '@constants/common.constants';
import { TaiKhoanService } from '@services/tai-khoan.service';

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
  data: NguoiDung[] = [];
  editData: NguoiDung = <NguoiDung>{};
  type: string = 'add';
  modalRef?: BsModalRef;
  iconButton = IconButton;
  constructor(private modalService: ModalService, private taiKhoanService: TaiKhoanService) {
    super();
  }
  ngOnInit(): void {
    this.search();
  }

  openModal(id: string, taiKhoan?: NguoiDung) {
    this.type = taiKhoan ? 'edit' : 'add';
    this.editData = taiKhoan ? taiKhoan : <NguoiDung>{};
    this.modalService.open(id);
  }

  changeData($event) {
    this.search();
  }

  getData() {
    this.spinnerService.show();
    this.taiKhoanService.getDataPagination(this.pagination, this.name).subscribe({
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
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa người dùng này', 'Xóa',
      () => {
        this.spinnerService.show();
        this.taiKhoanService.delete(id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success('Xóa người dùng thành Công', 'Thành Công');
              this.data = this.data.filter(x => x.id !== id);
              this.spinnerService.hide();
            }
          },
          error: () => {
            this.snotifyService.error('Xóa người dùng thất bại', 'Lỗi');
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
