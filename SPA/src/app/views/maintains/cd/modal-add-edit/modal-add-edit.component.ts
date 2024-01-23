import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from "ngx-bootstrap/modal";
import { SanPham } from "@models/maintains/san-pham";
import { ModalService } from "@services/modal.service";
import { CdService } from "@services/cd.service";
import { InjectBase } from "@utilities/inject-base-app";
import { MaHangService } from '@services/mahang.service';
import { MaHang } from '@models/maintains/ma-hang';
@Component({
  selector: 'app-modal-add-edit',
  templateUrl: './modal-add-edit.component.html',
  styleUrls: ['./modal-add-edit.component.scss']
})
export class ModalAddEditComponent extends InjectBase implements OnInit, OnChanges, OnDestroy {
  @ViewChild('modalAdd', { static: false }) modalAdd?: ModalDirective;
  @Input() id: string = '';
  @Input() type: string = '';
  @Input() data: SanPham = <SanPham>{};
  @Output() changeData = new EventEmitter();
  private element: any;
  maHangs: MaHang[] = [];
  constructor(
    private modalService: ModalService,
    private el: ElementRef,
    private cdService: CdService,
    private maHangService: MaHangService) {
    super();
    this.element = el.nativeElement;
  }

  ngOnInit() {
    // ensure id attribute exists
    if (!this.id) {
      console.error('modal must have an id');
      return;
    }
    // add self (this modal instance) to the modal service so it's accessible from controllers
    this.modalService.add(this);
  }

  ngOnChanges(): void {
    //this.getAllMaHang();
  }

  // open modal
  open(): void {
    this.modalAdd.show();
  }

  cancel() {
    this.modalAdd.hide();
  }


  save() {
    this.type === 'add' ? this.add() : this.update();
  }

  add() {
    this.cdService.create(this.data).subscribe({
      next: () => {
        this.snotifyService.success('Thêm sản phẩm thành công', 'Thành công');
        this.changeData.emit(true);
        this.modalAdd.hide();
        this.maHangs = [];
      },
      error: (e) => {
        this.snotifyService.error('Thêm sản phẩm thất bại', 'Lỗi');
      }
    })
  }

  update() {
    this.cdService.update(this.data).subscribe({
      next: () => {
        this.snotifyService.success('Sửa sản phẩm thành công', 'Thành công');
        this.changeData.emit(true);
        this.modalAdd.hide();
        this.maHangs = [];
      },
      error: (e) => {
        this.snotifyService.error('Sửa sản phẩm thất bại', 'Lỗi');
      }
    })
  }
  // remove self from modal service when directive is destroyed
  ngOnDestroy(): void {
    this.modalService.remove(this.id);
    this.element.remove();
  }
}
