import { Injectable } from '@angular/core';
import { environment } from "@env/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { FunctionUtility } from '@utilities/function-utility';
import { LichSuHangHoaData, LichSuHangHoaParam } from '@models/maintains/lich-su-hang-hoa';
import { KeyValuePair } from '@utilities/key-value-pair';
import { OperationResult } from '@utilities/operation-result';
import { PaginationParam } from '@utilities/pagination-utility';
import { BehaviorSubject } from 'rxjs';
import { AppStateService } from './app-state.service';

@Injectable({
  providedIn: 'root'
})
export class LichSuHangHoaService {
  apiUrl = environment.apiUrl + 'LichSuHangHoa';

  private mainStateSource = new BehaviorSubject<any>(null);

  saveMainState(state: any) {
    this.mainStateSource.next(state);
  }

  getMainState(): any {
    return this.mainStateSource.getValue();
  }

  clearMainState() {
    this.mainStateSource.next(null);
  }

  constructor(private http: HttpClient, private functionUtility: FunctionUtility, private appState: AppStateService) {
    this.appState.reset$.subscribe(() => this.clearMainState());
  }

  getDataPagination(pagination: PaginationParam, param: LichSuHangHoaParam) {
    let params = new HttpParams().appendAll({ ...pagination, ...param });
    return this.http.get<LichSuHangHoaData>(`${this.apiUrl}/GetDataPagination`, { params });
  }

  excel(param: LichSuHangHoaParam) {
    let params = new HttpParams().appendAll({ ...param });
    return this.http.get<OperationResult>(`${this.apiUrl}/Excel`, { params });
  }

  getListSanPham() {
    return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListSanPham`);
  }

  getListKhachHang() {
    return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListKhachHang`);
  }

  getListNhaCungCap() {
    return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListNhaCungCap`);
  }

  getListNhanVien() {
    return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListNhanVien`);
  }
}
