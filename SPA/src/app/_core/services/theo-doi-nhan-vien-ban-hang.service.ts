import {
  TheoDoiNhanVienBanHang_Data,
  TheoDoiNhanVienBanHang_Param
} from '@models/maintains/theo-doi-nhan-vien-ban-hang';
import { Injectable } from '@angular/core';
import { environment } from "@env/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginationParam } from '@utilities/pagination-utility';
import { KeyValuePair } from '@utilities/key-value-pair';
import { OperationResult } from '@utilities/operation-result';
import { BehaviorSubject } from 'rxjs';
import { AppStateService } from './app-state.service';

@Injectable({
  providedIn: 'root'
})
export class TheoDoiNhanVienBanHangService {
  apiUrl = environment.apiUrl + 'TheoDoiNhanVienBanHang';
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

  constructor(private http: HttpClient, private appState: AppStateService) {
    this.appState.reset$.subscribe(() => this.clearMainState());
  }

  getDataPagination(pagination: PaginationParam, param: TheoDoiNhanVienBanHang_Param) {
    let params = new HttpParams().appendAll({ ...pagination, ...param });
    return this.http.get<TheoDoiNhanVienBanHang_Data>(`${this.apiUrl}/GetDataPagination`, { params });
  }
  excel(param: TheoDoiNhanVienBanHang_Param) {
    let params = new HttpParams().appendAll({ ...param });
    return this.http.get<OperationResult>(`${this.apiUrl}/Excel`, { params })
  }
  getListNhanVien() {
    return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListNhanVien`);
  }
  getListSanPham() {
    return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListSanPham`);
  }
}
