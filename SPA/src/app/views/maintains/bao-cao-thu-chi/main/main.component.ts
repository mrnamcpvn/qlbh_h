import { Component, OnInit, OnDestroy, AfterViewChecked, ViewChild } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { InjectBase } from "@utilities/inject-base-app";
import { ClassButton, IconButton } from "@constants/common.constants";
import { BaoCaoThuChiService } from '@services/bao-cao-thu-chi.service';
import { FormGroup, NgForm } from '@angular/forms';
import { BaoCaoThuChiData, BaoCaoThuChiParam } from '@models/maintains/bao-cao-thu-chi';
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
  classButton = ClassButton
  bsConfig: Partial<BsDatepickerConfig> = {
    dateInputFormat: "DD/MM/YYYY",
    isAnimated: true,
  }
  now: Date = new Date();
  fromDate: Date;
  toDate: Date;
  param: BaoCaoThuChiParam = <BaoCaoThuChiParam>{ idSP: [], idKH: [], idNV: [], loai: 0 };
  loaiList: KeyValuePair[] = [
    { key: 1, value: 'Nhập' },
    { key: 2, value: 'Xuất' },
    { key: 0, value: 'Tất Cả' },
  ];
  data: BaoCaoThuChiData = <BaoCaoThuChiData>{};
  filterByList: KeyValuePair[] = [
    { key: "0", value: 'Khoảng thời gian' },
    { key: "1", value: 'Năm' },
    { key: "2", value: 'Quý' },
    { key: "3", value: 'Tháng' },
    { key: "4", value: 'Tuần' },
    { key: "5", value: 'Ngày' },
  ];
  filterBy: string = '0';
  sPList: KeyValuePair[] = []
  doiTacList: KeyValuePair[] = []
  selectedDoiTac: number[] = []
  nvList: KeyValuePair[] = []
  selectedNV: number[] = []
  khList: KeyValuePair[] = []
  nccList: KeyValuePair[] = []

  constructor(private service: BaoCaoThuChiService) {
    super();
  }

  ngOnInit() {
    this.savedState = this.service.getMainState();
    this.service.clearMainState();
    if (this.savedState) {
      this.restoreState(this.savedState);
    } else {
      this.getListSanPham()
      this.getListKhachHang()
      this.getListNhaCungCap()
      this.getListNhanVien()
      this.clear();
    }
  }

  ngAfterViewChecked() {
    if (this.savedState && this.mainForm) {
      const form: FormGroup = this.mainForm.form
      const values = Object.values(form.value)
      const isLoad = !values.every(v => v === undefined)
      if (isLoad) {
        if (form.valid)
          this.getData();
        this.savedState = null;
      }
    }
  }

  ngOnDestroy(): void {
    this.service.saveMainState({ fromDate: this.fromDate, toDate: this.toDate, param: this.param, filterBy: this.filterBy, selectedDoiTac: this.selectedDoiTac, selectedNV: this.selectedNV });
  }

  getData() {
    this.spinnerService.show();
    this.param.fromDate = this.functionUtility.getDateFormat(this.fromDate)
    this.param.toDate = this.functionUtility.getDateFormat(this.toDate)
    this.param.idSP = this.param.idSP || [];
    this.mapDoiTacSelection();
    this.param.idNV = this.selectedNV || [];
    this.service.getData(this.param).subscribe({
      next: (res) => {
        this.data = res;
        this.spinnerService.hide();
      }
    });
  }

  search() {
    this.getData();
  }

  excel() {
    this.spinnerService.show();
    this.param.fromDate = this.functionUtility.getDateFormat(this.fromDate)
    this.param.toDate = this.functionUtility.getDateFormat(this.toDate)
    this.param.idSP = this.param.idSP || [];
    this.mapDoiTacSelection();
    this.param.idNV = this.selectedNV || [];
    this.service.excel(this.param).subscribe({
      next: (result) => {
        this.spinnerService.hide();
        result.isSuccess
          ? this.functionUtility.exportExcel(result.data, `Báo cáo thu chi`)
          : this.snotifyService.error(result.error, 'Lỗi');
      },
    });
  }

  getListSanPham() {
    this.service.getListSanPham().subscribe({
      next: (res) => {
        this.sPList = res;
        this.functionUtility.getNgSelectAllCheckbox(this.sPList)
      }
    });
  }

  getListKhachHang() {
    this.service.getListKhachHang().subscribe({
      next: (res) => {
        this.khList = res;
        this.functionUtility.getNgSelectAllCheckbox(this.khList)
        this.buildDoiTacList();
      }
    });
  }

  getListNhaCungCap() {
    this.service.getListNhaCungCap().subscribe({
      next: (res) => {
        this.nccList = res;
        this.functionUtility.getNgSelectAllCheckbox(this.nccList)
        this.buildDoiTacList();
      }
    });
  }

  getListNhanVien() {
    this.service.getListNhanVien().subscribe({
      next: (res) => {
        this.nvList = res;
        this.functionUtility.getNgSelectAllCheckbox(this.nvList)
      }
    });
  }

  onLoaiChange() {
    this.selectedDoiTac = [];
    this.buildDoiTacList();
  }

  onDoiTacLoaiChange() {
    this.selectedDoiTac = [];
    this.buildDoiTacList();
  }

  buildDoiTacList() {
    if (this.param.loai == 1) {
      this.doiTacList = [...this.nccList];
    } else if (this.param.loai == 2) {
      this.doiTacList = [...this.khList];
    } else {
      this.doiTacList = [
        ...this.khList.map(x => ({ ...x, value: `[KH] ${x.value}` })),
        ...this.nccList.map(x => ({ ...x, key: -(x.key as number), value: `[NCC] ${x.value}` }))
      ];
    }
  }

  mapDoiTacSelection() {
    if (this.param.loai == 1) {
      this.param.idNCC = [...this.selectedDoiTac];
      this.param.idKH = [];
    } else if (this.param.loai == 2) {
      this.param.idKH = [...this.selectedDoiTac];
      this.param.idNCC = [];
    } else {
      this.param.idKH = this.selectedDoiTac.filter(id => id > 0);
      this.param.idNCC = this.selectedDoiTac.filter(id => id < 0).map(id => Math.abs(id));
    }
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
      case '1':
        this.toDate = new Date(startDate.getFullYear(), 11, 31);
        break;
      case '2':
        const quarter = Math.floor(startDate.getMonth() / 3);
        this.toDate = new Date(startDate.getFullYear(), quarter * 3 + 3, 0);
        break;
      case '3':
        this.toDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0);
        break;
      case '4':
        const day = startDate.getDay();
        const diff = startDate.getDate() + (day === 0 ? 0 : 7 - day);
        this.toDate = new Date(startDate.getFullYear(), startDate.getMonth(), diff);
        break;
      case '5':
        this.toDate = new Date(startDate);
        break;
      default:
        break;
    }
  }

  calculateFromDateByFilterBy(endDate: Date) {
    switch (this.filterBy) {
      case '1':
        this.fromDate = new Date(endDate.getFullYear(), 0, 1);
        break;
      case '2':
        const quarter = Math.floor(endDate.getMonth() / 3);
        this.fromDate = new Date(endDate.getFullYear(), quarter * 3, 1);
        break;
      case '3':
        this.fromDate = new Date(endDate.getFullYear(), endDate.getMonth(), 1);
        break;
      case '4':
        const day = endDate.getDay();
        const diff = endDate.getDate() - day + (day === 0 ? -6 : 1);
        this.fromDate = new Date(endDate.getFullYear(), endDate.getMonth(), diff);
        break;
      case '5':
        this.fromDate = new Date(endDate);
        break;
      default:
        break;
    }
  }

  clear() {
    this.service.clearMainState();
    this.fromDate = new Date(this.now.getFullYear(), this.now.getMonth(), 1)
    this.toDate = new Date(this.now.getFullYear(), this.now.getMonth() + 1, 0)
    this.filterBy = '0';
    this.param = <BaoCaoThuChiParam>{
      idSP: [],
      idKH: [],
      idNCC: [],
      idNV: [],
      loai: 0,
    };
    this.selectedDoiTac = []
    this.selectedNV = []
    this.buildDoiTacList();
    this.data = <BaoCaoThuChiData>{
      orders: []
    };
  }

  private restoreState(state: any) {
    this.getListSanPham();
    this.getListKhachHang();
    this.getListNhaCungCap();
    this.getListNhanVien();
    this.fromDate = state.fromDate;
    this.toDate = state.toDate;
    this.param = state.param;
    this.filterBy = state.filterBy;
    this.selectedDoiTac = state.selectedDoiTac;
    this.selectedNV = state.selectedNV;
    this.buildDoiTacList();
  }

}
