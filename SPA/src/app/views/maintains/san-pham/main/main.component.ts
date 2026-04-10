import { Component, OnInit } from '@angular/core';
import { SanPham } from "@models/maintains/san-pham";
import { Pagination } from '@utilities/pagination-utility';
import { BsModalRef } from "ngx-bootstrap/modal";
import { ModalService } from "@services/modal.service";
import { SanPhamService } from "@services/san-pham.service";
import { InjectBase } from "@utilities/inject-base-app";
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { ClassButton } from '@constants/common.constants';
import { FileResultModel } from 'src/app/views/_shared/file-upload-component/file-upload.component';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent extends InjectBase implements OnInit {
  data: SanPham[] = [];
  pagination: Pagination = <Pagination>{
    pageNumber: 1,
    pageSize: 10
  };
  name: string = '';

  acceptFormat: string = '.xls, .xlsx, .xlsm';
  editData: SanPham = <SanPham>{};
  type: string = 'add';
  modalRef?: BsModalRef;
  classButton = ClassButton;
  constructor(private modalService: ModalService, private spService: SanPhamService) {
    super();
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.spService.getDataPagination(this.pagination, this.name).subscribe({
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

  openModal(id: string, cd?: SanPham) {
    this.type = cd ? 'edit' : 'add';
    this.editData = cd ? { ...cd } : <SanPham>{};
    this.modalService.open(id);
  }

  changeData(e: any) {
    this.search();
  }
  template() {
    this.spinnerService.show();
    this.spService.template().subscribe({
      next: (result) => {
        this.spinnerService.hide();
        this.functionUtility.exportExcel(result.data, 'Mẫu upload sản phẩm')
      },
    });
  }
  upload(event: FileResultModel) {
    this.spinnerService.show();
    this.spService.upload(event.formData).subscribe({
      next: async (res) => {
        this.spinnerService.hide();
        if (res.isSuccess) {
          this.search()
          this.snotifyService.success('Dữ liệu đã được tải lên thành công', 'Thành công');
        } else {
          if (!this.functionUtility.checkEmpty(res.data)) {
            this.functionUtility.exportExcel(res.data, `Báo cáo lỗi dữ liệu`);
          }
          this.snotifyService.error( res.error, 'Lỗi');
        }
      }
    });
  }
  delete(id: number) {
    this.snotifyService.confirm('Bạn có chắc chắn muốn xóa sản phẩm', 'Xóa',
      () => {
        this.spinnerService.show();
        this.spService.delete(id).subscribe({
          next: (res) => {
            if (res) {
              this.snotifyService.success('Xóa sản phẩm Thành Công', 'Thành công');
              this.data = this.data.filter(x => x.id !== id);
            } else {
              this.snotifyService.error('Xóa sản phẩm Thất bại', 'Lỗi');
            }
            this.spinnerService.hide();
          },
          error: () => {
            this.snotifyService.error('Xóa sản phẩm thất bại', 'Lỗi');
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
