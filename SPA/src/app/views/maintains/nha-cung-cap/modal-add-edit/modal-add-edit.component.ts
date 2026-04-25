import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalService } from '@services/modal.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { InjectBase } from "@utilities/inject-base-app";
import { NhaCungCap } from '@models/maintains/nha-cung-cap';
import { NhaCungCapService } from '@services/nha-cung-cap.service';

@Component({
  selector: 'app-modal-add-edit-ncc',
  templateUrl: './modal-add-edit.component.html',
  styleUrls: ['./modal-add-edit.component.scss']
})
export class ModalAddEditComponent extends InjectBase implements OnInit, OnDestroy {

  @ViewChild('modalAdd', { static: false }) modalAdd?: ModalDirective;
  @Input() id: string = '';
  @Input() type: string = '';
  @Input() data: NhaCungCap = <NhaCungCap>{};
  @Output() changeData = new EventEmitter();
  private element: any;
  constructor(
    private modalService: ModalService,
    private el: ElementRef,
    private service: NhaCungCapService
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
    this.service.create(this.data).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.snotifyService.success('Thêm nhà cung cấp thành công', 'Thành công');
          this.changeData.emit(true);
          this.modalAdd.hide();
        } else {
          this.snotifyService.error(res.error, 'Lỗi');
        }
      },
      error: (e) => {
        this.snotifyService.error('Thêm nhà cung cấp thất bại', 'Lỗi');
      }
    })
  }

  update() {
    this.service.update(this.data).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.snotifyService.success('Sửa nhà cung cấp thành công', 'Thành công');
          this.changeData.emit(true);
          this.modalAdd.hide();
        } else {
          this.snotifyService.error(res.error, 'Lỗi');
        }
      },
      error: (e) => {
        this.snotifyService.error('Sửa nhà cung cấp thất bại', 'Lỗi');
      }
    })
  }

  cancel() {
    this.modalAdd.hide();
  }

}
