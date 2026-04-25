import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';
import { BehaviorSubject } from 'rxjs';
import { CongNoChiTiet, CongNoCustomer, CongNoFilter, CongNoSummary, CustomerDebtInfo } from '@models/maintains/cong-no/cong-no';
import { AppStateService } from './app-state.service';

@Injectable({ providedIn: 'root' })
export class CongNoService {
  apiUrl = environment.apiUrl + 'CongNo';
  private mainStateSource = new BehaviorSubject<any>(null);

  saveMainState(state: any) { this.mainStateSource.next(state); }
  getMainState(): any { return this.mainStateSource.getValue(); }
  clearMainState() { this.mainStateSource.next(null); }

  constructor(private http: HttpClient, private appState: AppStateService) {
    this.appState.reset$.subscribe(() => this.clearMainState());
  }

  getSummary() {
    return this.http.get<CongNoSummary[]>(`${this.apiUrl}/GetSummary`);
  }

  getData(filter?: CongNoFilter) {
    let params = {};
    if (filter) {
      params = {
        fromDate: filter.fromDate || '',
        toDate: filter.toDate || '',
        trangThai: filter.trangThai || '',
        idKH: filter.idKH || []
      };
    }
    return this.http.get<CongNoCustomer[]>(`${this.apiUrl}/GetData`, { params: params as any });
  }

  getDetail(khachHangID: number) {
    return this.http.get<CongNoChiTiet[]>(`${this.apiUrl}/GetDetail`, { params: { khachHangID } });
  }

  getCustomerDebtInfo(khachHangID: number) {
    return this.http.get<CustomerDebtInfo>(`${this.apiUrl}/GetCustomerDebtInfo`, { params: { khachHangID } });
  }
}
