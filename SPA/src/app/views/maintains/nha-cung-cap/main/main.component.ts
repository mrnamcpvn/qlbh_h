import { Component, OnInit } from '@angular/core';
import { ModalService } from '@services/modal.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Pagination } from '@utilities/pagination-utility';
import { PageChangedEvent } from "ngx-bootstrap/pagination";
import { InjectBase } from "@utilities/inject-base-app";
import { ClassButton, IconButton } from '@constants/common.constants';
import { FileResultModel } from 'src/app/views/_shared/file-upload-component/file-upload.component';
import { NhaCungCap } from '@models/maintains/nha-cung-cap';
import { NhaCungCapService } from '@services/nha-cung-cap.service';

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
  acceptFormat: string = '.xls, .xlsx, .xlsm';
  data: NhaCungCap[] = [];
  editData: NhaCungCap = <NhaCungCap>{};
  type: string = 'add';
  modalRef?: BsModalRef;
  iconButton = IconButton;
  classButton = ClassButton;
  constructor(
    private modalService: ModalService,
    private service: NhaCungCapService
  ) {
    super();
  }

  ngOnInit(): void {
    this.search();
  }

  openModal(id: string, NCC?: NhaCungCap) {
    this.type = NCC ? 'edit' : 'add';
    this.editData = NCC ? { ...NCC } : <NhaCungCap>{};
    this.modalService.open(id);
  }

  changeData($event) {
    this.search();
  }
  template() {
    this.spinnerService.show();
    this.service.template().subscribe({
      next: (result) => {
        this.spinnerService.hide();
        this.functionUtility.exportExcel(result.data, 'Mẫu upload nhà cung cấp')
      },
    });
  }
  upload(event: FileResultModel) {
    this.spinnerService.show();
    this.service.upload(event.formData).subscribe({
      next: async (res) => {
        this.spinnerService.hide();
        if (res.isSuccess) {
          this.search()
          this.snotifyService.success('Dữ liệu đã được tải lên thành công', 'Thành công');
        } else {
          if (!this.functionUtility.checkEmpty(res.data)) {
            this.functionUtility.exportExcel(res.data, `Báo cáo lỗi dữ liệu`);
          }
          this.snotifyService.error(res.error, 'Lỗi');
        }
      }
    });
  }
  getData() {
    this.spinnerService.show();
    this.service.getDataPagination(this.pagination, this.ten).subscribe({
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
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa nhà cung cấp?', 'Xóa',
      () => {
        this.spinnerService.show();
        this.service.delete(id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success('Xóa nhà cung cấp Thành Công', 'Thành Công');
              this.data = this.data.filter(x => x.id !== id);
            } else {
              this.snotifyService.error('Xóa nhà cung cấp Thất bại', 'Lỗi');
            }
            this.spinnerService.hide();
          },
          error: () => {
            this.snotifyService.error('Xóa nhà cung cấp Thất bại', 'Lỗi');
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
