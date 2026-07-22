import { Component, OnInit, OnDestroy, AfterViewChecked, ViewChild } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { FormGroup, NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { InjectBase } from "@utilities/inject-base-app";
import { ClassButton, IconButton } from "@constants/common.constants";
import { CongNoService } from '@services/cong-no.service';
import { KhachHangService } from '@services/khach-hang.service';
import { CongNoChiTiet, CongNoCustomer, CongNoFilter } from '@models/maintains/cong-no/cong-no';
import { KeyValuePair } from '@utilities/key-value-pair';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent extends InjectBase implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('mainForm') public mainForm: NgForm;
  private savedState: any;
  iconButton = IconButton;
  classButton = ClassButton;
  bsConfig: Partial<BsDatepickerConfig> = { dateInputFormat: 'DD/MM/YYYY', isAnimated: true };

  now: Date = new Date();
  fromDate: Date;
  toDate: Date;
  filterBy: string = '0';
  filterByList: KeyValuePair[] = [
    { key: "0", value: 'Khoảng thời gian' },
    { key: "1", value: 'Năm' },
    { key: "2", value: 'Quý' },
    { key: "3", value: 'Tháng' },
    { key: "4", value: 'Tuần' },
    { key: "5", value: 'Ngày' },
  ];
  trangThaiList: KeyValuePair[] = [
    { key: '', value: 'Tất cả' },
    { key: 'QuaHan', value: 'Quá hạn' },
    { key: 'ConHan', value: 'Còn hạn' },
  ];
  trangThai: string = '';
  khachHangList: KeyValuePair[] = [];
  selectedKH: number[] = [];

  data: CongNoCustomer[] = [];

  constructor(
    private service: CongNoService,
    private khService: KhachHangService,
    private route: ActivatedRoute
  ) { super(); }

  ngOnInit() {
    const khIdParam = this.route.snapshot.queryParamMap.get('khachHangID');
    if (khIdParam) {
      this.savedState = null;
      this.fromDate = undefined;
      this.toDate = undefined;
      this.filterBy = '0';
      this.trangThai = '';
      this.selectedKH = [];
      this.data = [];
      this.getListKhachHang(() => {
        const id = parseInt(khIdParam, 10);
        if (this.khachHangList.some(x => x.key === id)) {
          this.selectedKH = [id];
        }
        this.getData();
      });
    } else {
      this.savedState = this.service.getMainState();
      this.service.clearMainState();
      if (this.savedState) {
        this.restoreState(this.savedState);
      } else {
        this.getListKhachHang();
        this.clear();
      }
    }
  }

  ngAfterViewChecked() {
    if (this.savedState && this.mainForm) {
      const form: FormGroup = this.mainForm.form;
      const values = Object.values(form.value);
      const isLoad = !values.every(v => v === undefined);
      if (isLoad) {
        this.getData();
        this.savedState = null;
      }
    }
  }

  ngOnDestroy(): void {
    this.service.saveMainState({
      fromDate: this.fromDate,
      toDate: this.toDate,
      filterBy: this.filterBy,
      trangThai: this.trangThai,
      selectedKH: this.selectedKH
    });
  }

  getListKhachHang(callback?: () => void) {
    this.khService.getAll().subscribe({
      next: (res) => {
        this.khachHangList = (res || []).map(x => ({ key: x.id, value: `${x.ma_KH} - ${x.ten}` }));
        this.functionUtility.getNgSelectAllCheckbox(this.khachHangList);
        if (callback) callback();
      }
    });
  }

  getData() {
    this.spinnerService.show();
    const filter: CongNoFilter = {
      fromDate: this.fromDate ? this.functionUtility.getDateFormat(this.fromDate) : '',
      toDate: this.toDate ? this.functionUtility.getDateFormat(this.toDate) : '',
      idKH: this.selectedKH || [],
      trangThai: this.trangThai
    };
    this.service.getData(filter).subscribe({
      next: (res) => {
        this.data = (res || []).map(x => ({ ...x, isExpanded: true }));
        this.spinnerService.hide();
      },
      error: () => this.spinnerService.hide()
    });
  }

  search() {
    this.getData();
  }

  clear() {
    this.service.clearMainState();
    this.fromDate = new Date(this.now.getFullYear(), this.now.getMonth(), 1);
    this.toDate = new Date(this.now.getFullYear(), this.now.getMonth() + 1, 0);
    this.filterBy = '0';
    this.trangThai = '';
    this.selectedKH = [];
    this.data = [];
  }

  private restoreState(state: any) {
    this.getListKhachHang();
    this.fromDate = state.fromDate;
    this.toDate = state.toDate;
    this.filterBy = state.filterBy;
    this.trangThai = state.trangThai;
    this.selectedKH = state.selectedKH || [];
  }

  toggleExpand(item: CongNoCustomer) {
    item.isExpanded = !item.isExpanded;
  }

  soNgayConHan(order: CongNoChiTiet): number {
    const now = new Date();
    const denHan = new Date(order.ngayDenHan);
    const diff = denHan.getTime() - now.getTime();
    return Math.ceil(diff / (1000 * 60 * 60 * 24));
  }

  hasOverdue(item: CongNoCustomer): boolean {
    return item.orders?.some(o => o.soNgayQuaHan > 0) ?? false;
  }

  onFilterByChange() {
    if (this.fromDate) {
      this.calculateFromDateByFilterBy(new Date(this.fromDate));
      this.calculateToDateByFilterBy(new Date(this.fromDate));
    }
  }

  onFromDateChange() {
    if (this.fromDate) {
      this.calculateFromDateByFilterBy(new Date(this.fromDate));
      this.calculateToDateByFilterBy(new Date(this.fromDate));
    }
  }

  onToDateChange() {
    if (this.toDate) {
      this.calculateToDateByFilterBy(new Date(this.toDate));
      this.calculateFromDateByFilterBy(new Date(this.toDate));
    }
  }

  calculateToDateByFilterBy(startDate: Date) {
    switch (this.filterBy) {
      case '1': this.toDate = new Date(startDate.getFullYear(), 11, 31); break;
      case '2': const q = Math.floor(startDate.getMonth() / 3); this.toDate = new Date(startDate.getFullYear(), q * 3 + 3, 0); break;
      case '3': this.toDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0); break;
      case '4': const day = startDate.getDay(); const diff = startDate.getDate() + (day === 0 ? 0 : 7 - day); this.toDate = new Date(startDate.getFullYear(), startDate.getMonth(), diff); break;
      case '5': this.toDate = new Date(startDate); break;
    }
  }

  calculateFromDateByFilterBy(endDate: Date) {
    switch (this.filterBy) {
      case '1': this.fromDate = new Date(endDate.getFullYear(), 0, 1); break;
      case '2': const q = Math.floor(endDate.getMonth() / 3); this.fromDate = new Date(endDate.getFullYear(), q * 3, 1); break;
      case '3': this.fromDate = new Date(endDate.getFullYear(), endDate.getMonth(), 1); break;
      case '4': const day = endDate.getDay(); const diff = endDate.getDate() - day + (day === 0 ? -6 : 1); this.fromDate = new Date(endDate.getFullYear(), endDate.getMonth(), diff); break;
      case '5': this.fromDate = new Date(endDate); break;
    }
  }

  tongCongNo(): number {
    return this.data.reduce((s, x) => s + x.tongConNo, 0);
  }

  tongDonNo(): number {
    return this.data.reduce((s, x) => s + x.soDonNo, 0);
  }
}
