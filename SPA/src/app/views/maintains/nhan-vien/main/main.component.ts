import { Component, OnInit } from '@angular/core';
import { ModalService } from '@services/modal.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { KhachHangService } from '@services/khach-hang.service';
import { Pagination } from '@utilities/pagination-utility';
import { KhachHang } from '@models/maintains/khach-hang';
import { PageChangedEvent } from "ngx-bootstrap/pagination";
import { InjectBase } from "@utilities/inject-base-app";
import { IconButton } from '@constants/common.constants';
import { NhanVien } from '@models/maintains/nhan-vien';
import { NhanVienService } from '@services/nhan-vien.service';

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
  ten: string = '';
  data: NhanVien[] = [];
  editData: NhanVien = <NhanVien>{};
  type: string = 'add';
  modalRef?: BsModalRef;
  iconButton = IconButton;
  constructor(private modalService: ModalService, private nvService: NhanVienService) {
	super();
  }
   
  ngOnInit(): void {
	this.search();
  }

  openModal(id: string, nv?: NhanVien) {
	this.type = nv ? 'edit' : 'add';
	this.editData = nv ? nv : <NhanVien>{};
	this.modalService.open(id);
  }

  changeData($event) {
	this.search();
  }

  getData() {
	this.spinnerService.show();
	this.nvService.getDataPagination(this.pagination, this.ten).subscribe({
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
	this.snotifyService.confirm('Bạn có chắc chắn muốn xóa nhân viên?', 'Xóa',
	  () => {
		this.spinnerService.show();
		this.nvService.delete(id).subscribe({
		  next: (res) => {
			if (res) {
			  this.snotifyService.success('Xóa Nhân Viên Thành Công', 'Thành Công');
			  this.data = this.data.filter(x => x.id !== id);
			  this.spinnerService.hide();
			}
		  },
		  error: () => {
			this.snotifyService.error('Xóa Nhân Viên Thất bại', 'Lỗi');
			this.spinnerService.hide();
		  }
		})
	  });
  }

  clear() {
	this.ten = '';
	this.search();
  }

}