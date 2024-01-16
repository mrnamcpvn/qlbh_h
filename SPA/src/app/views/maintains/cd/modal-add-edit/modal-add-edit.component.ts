import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from "ngx-bootstrap/modal";
import { CongDoan } from "@models/maintains/cong-doan";
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
  @Input() data: CongDoan = <CongDoan>{};
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
    this.getAllMaHang();
  }

  // open modal
  open(): void {
    this.modalAdd.show();
  }

  cancel() {
    this.modalAdd.hide();
  }

  getAllMaHang() {
    this.maHangService.getAll().subscribe({
      next: res => {
        this.maHangs = res;
      }
    })
  }

  save() {
    this.type === 'add' ? this.add() : this.update();
  }

  add() {
    this.cdService.create(this.data).subscribe({
      next: () => {
        this.snotifyService.success('Thêm công đoạn thành công', 'Thành công');
        this.changeData.emit(true);
        this.modalAdd.hide();
        this.maHangs = [];
      },
      error: (e) => {
        this.snotifyService.error('Thêm công đoạn thất bại', 'Lỗi');
      }
    })
  }

  update() {
    this.cdService.update(this.data).subscribe({
      next: () => {
        this.snotifyService.success('Sửa công đoạn thành công', 'Thành công');
        this.changeData.emit(true);
        this.modalAdd.hide();
        this.maHangs = [];
      },
      error: (e) => {
        this.snotifyService.error('Sửa công đoạn thất bại', 'Lỗi');
      }
    })
  }
  // remove self from modal service when directive is destroyed
  ngOnDestroy(): void {
    this.modalService.remove(this.id);
    this.element.remove();
  }
}
