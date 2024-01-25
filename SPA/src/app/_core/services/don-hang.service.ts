import { Injectable } from '@angular/core';
import { environment } from "@env/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { ChiTietDonHang, DonHang, DonHangDTO } from '@models/maintains/don-hang';
import { FunctionUtility } from '@utilities/function-utility';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DonHangService {
  apiUrl = environment.apiUrl+'DonHang';
  s_DonHang = new BehaviorSubject<DonHang>(null);
  current_DH = this.s_DonHang.asObservable();
  constructor(private http: HttpClient, private functionUtility: FunctionUtility) { }

  getDataPagination_Mua(pagination: PaginationParam, fromDate: string | Date, toDate: string | Date) {
    let dateStart = this.functionUtility.getDateFormat(fromDate as Date);
    let dateEnd = this.functionUtility.getDateFormat(toDate as Date);
    let params = new HttpParams().appendAll({ ...pagination, 'fromDate': dateStart, 'toDate': dateEnd });
    return this.http.get<PaginationResult<DonHang>>(`${this.apiUrl}/GetMuaHangPagination`, { params });
  }

  create(model: DonHangDTO) {
    return this.http.post<boolean>(`${this.apiUrl}/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}/Delete`, { params: { id: id } });
  }

  update(model: DonHang) {
    return this.http.put<boolean>(`${this.apiUrl}/Update`, model);
  }

  getDetail(id: number) {
    return this.http.get<ChiTietDonHang[]>(`${this.apiUrl}/GetDetail`, { params: { id } });
  }

  changeSDonHang(model: DonHang) {
    this.s_DonHang.next(model);
  }
}
