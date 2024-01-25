import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalService } from '@services/modal.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { KhachHang } from '@models/maintains/khach-hang';
import { KhachHangService } from '@services/khach-hang.service'
import { InjectBase } from "@utilities/inject-base-app";

@Component({
  selector: 'app-modal-add-edit',
  templateUrl: './modal-add-edit.component.html',
  styleUrls: ['./modal-add-edit.component.scss']
})
export class ModalAddEditComponent extends InjectBase implements OnInit, OnDestroy {

  @ViewChild('modalAdd', { static: false }) modalAdd?: ModalDirective;
  @Input() id: string = '';
  @Input() type: string = '';
  @Input() data: KhachHang = <KhachHang>{};
  @Output() changeData = new EventEmitter();
  private element: any;
  constructor(
    private modalService: ModalService,
    private el: ElementRef,
    private khService: KhachHangService
  ) {
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

  // remove self from modal service when directive is destroyed
  ngOnDestroy(): void {
    this.modalService.remove(this.id);
    this.element.remove();
  }

  // open modal
  open(): void {
    this.modalAdd.show();
  }

  // close modal
  close(): void {
    this.modalAdd.hide();
  }

  save() {
    this.type === 'add' ? this.add() : this.update();
  }

  add() {
    this.khService.create(this.data).subscribe({
      next: () => {
        this.snotifyService.success('Thêm khách hàng thành công', 'Thành công');
        this.changeData.emit(true);
        this.modalAdd.hide();
      },
      error: (e) => {
        this.snotifyService.error('Thêm khách hàng thất bại', 'Lỗi');
      }
    })
  }

  update() {
    this.khService.update(this.data).subscribe({
      next: () => {
        this.snotifyService.success('Sửa khách hàng thành công', 'Thành công');
        this.changeData.emit(true);
        this.modalAdd.hide();
      },
      error: (e) => {
        this.snotifyService.error('Sửa khách hàng thất bại', 'Lỗi');
      }
    })
  }

  cancel() {
    this.modalAdd.hide();
  }

}
