import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { NguoiDung } from '@models/maintains/nguoi-dung';
import { ModalService } from '@services/modal.service';
import { TaiKhoanService } from '@services/tai-khoan.service';
import { InjectBase } from '@utilities/inject-base-app';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-add-or-edit',
  templateUrl: './add-or-edit.component.html',
  styleUrls: ['./add-or-edit.component.scss']
})
export class AddOrEditComponent extends InjectBase implements OnInit {
  @ViewChild('modalAdd', { static: false }) modalAdd?: ModalDirective;
  @Input() id: string = '';
  @Input() type: string = '';
  @Input() data: NguoiDung = <NguoiDung>{};
  @Output() changeData = new EventEmitter();
  private element: any;
  constructor(
    private modalService: ModalService,
    private el: ElementRef,
    private taiKhoanService: TaiKhoanService
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
    this.taiKhoanService.create(this.data).subscribe({
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
    this.taiKhoanService.update(this.data).subscribe({
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
