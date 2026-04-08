import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalService } from '@services/modal.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { InjectBase } from "@utilities/inject-base-app";
import { DonHangService } from '@services/don-hang.service';
import { DonHang } from '@models/maintains/don-hang';

@Component({
  standalone: false,
  selector: 'app-model-thanh-toan',
  templateUrl: './model-thanh-toan.component.html',
  styleUrls: ['./model-thanh-toan.component.scss']
})
export class ModelThanhToanComponent extends InjectBase implements OnInit, OnDestroy {
  @ViewChild('modalAdd', { static: false }) modalAdd?: ModalDirective;
  @Input() id: string = '';
  @Input() data: DonHang = <DonHang>{};
  @Output() changeData = new EventEmitter<boolean>();
  private element: any;
  constructor(
    private modalService: ModalService,
    private el: ElementRef,
    private donHang: DonHangService
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
  save() {
    this.donHang.updatePayment(this.data).subscribe({
      next: (res: any) => {
        if (res) {
          this.snotifyService.success('Thanh toán thành công', 'Thành công');
          this.changeData.emit(true);
          this.modalAdd?.hide();
        } else {
          this.snotifyService.error('Thanh toán thất bại', 'Lỗi');
        }
      },
      error: () => {
        this.snotifyService.error('Có lỗi xảy ra', 'Lỗi');
      }
    });
  }

  // open modal
  open(): void {
    this.modalAdd?.show();
  }

  // close modal
  close(): void {
    this.modalAdd.hide();
  }

  cancel() {
    this.modalAdd.hide();
  }

  formatCurrency(value: number | string | undefined): string {
    if (value === null || value === undefined || value === '') return '';
    const numericStr = value.toString().replace(/\D/g, '');
    if (!numericStr) return '';
    return numericStr.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  }

  parseCurrency(value: string): number {
    if (!value) return 0;
    return parseInt(value.toString().replace(/,/g, ''), 10) || 0;
  }
}
