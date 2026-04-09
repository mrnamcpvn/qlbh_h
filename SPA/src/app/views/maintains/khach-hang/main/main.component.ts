import { Component, OnInit } from '@angular/core';
import { ModalService } from '@services/modal.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { KhachHangService } from '@services/khach-hang.service';
import { Pagination } from '@utilities/pagination-utility';
import { KhachHang } from '@models/maintains/khach-hang';
import { PageChangedEvent } from "ngx-bootstrap/pagination";
import { InjectBase } from "@utilities/inject-base-app";
import { ClassButton, IconButton } from '@constants/common.constants';
import { FileResultModel } from 'src/app/views/_shared/file-upload-component/file-upload.component';

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
  data: KhachHang[] = [];
  editData: KhachHang = <KhachHang>{};
  type: string = 'add';
  modalRef?: BsModalRef;
  iconButton = IconButton;
  classButton = ClassButton;
  constructor(
    private modalService: ModalService,
    private khService: KhachHangService
  ) {
    super();
  }

  ngOnInit(): void {
    this.search();
  }

  openModal(id: string, kh?: KhachHang) {
    this.type = kh ? 'edit' : 'add';
    this.editData = kh ? { ...kh } : <KhachHang>{};
    this.modalService.open(id);
  }

  changeData($event) {
    this.search();
  }
  template() {
    this.spinnerService.show();
    this.khService.template().subscribe({
      next: (result) => {
        this.spinnerService.hide();
        this.functionUtility.exportExcel(result.data, 'Mẫu upload khách hàng')
      },
    });
  }
  upload(event: FileResultModel) {
    this.spinnerService.show();
    this.khService.upload(event.formData).subscribe({
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
    this.khService.getDataPagination(this.pagination, this.ten).subscribe({
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
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa khách hàng?', 'Xóa',
      () => {
        this.spinnerService.show();
        this.khService.delete(id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success('Xóa Khách Hàng Thành Công', 'Thành Công');
              this.data = this.data.filter(x => x.id !== id);
              this.spinnerService.hide();
            }
          },
          error: () => {
            this.snotifyService.error('Xóa Khách Hàng Thất bại', 'Lỗi');
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
