import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MaHang } from '@models/maintains/ma-hang';
import { ModalService } from '@services/modal.service';
import { InjectBase } from '@utilities/inject-base-app';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { MaHangService } from '@services/mahang.service';

@Component({
  selector: 'app-modal-add-edit',
  templateUrl: './modal-add-edit.component.html',
  styleUrls: ['./modal-add-edit.component.scss']
})
export class ModalAddEditComponent extends InjectBase implements OnInit {
  @ViewChild('modalAdd', { static: false }) modalAdd?: ModalDirective;
  @Input() id: string = '';
  @Input() type: string = '';
  @Input() data: MaHang = <MaHang>{};
  @Output() changeData = new EventEmitter();
  private element: any;
  constructor(
    private modalService: ModalService,
    private el: ElementRef,
    private maHangService: MaHangService
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
    this.maHangService.create(this.data.name).subscribe({
      next: () => {
        this.snotifyService.success('Thêm mã hàng thành công', 'Thành công');
        this.changeData.emit(true);
        this.modalAdd.hide();
      },
      error: (e) => {
        this.snotifyService.error('Thêm mã hàng thất bại', 'Lỗi');
      }
    })
  }

  update() {
    this.maHangService.update(this.data).subscribe({
      next: () => {
        this.snotifyService.success('Sửa mã hàng thành công', 'Thành công');
        this.changeData.emit(true);
        this.modalAdd.hide();
      },
      error: (e) => {
        this.snotifyService.error('Sửa mã hàng thất bại', 'Lỗi');
      }
    })
  }

  cancel() {
    this.modalAdd.hide();
  }
}
