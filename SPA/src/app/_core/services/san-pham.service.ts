import { Injectable } from '@angular/core';
import { environment } from "@env/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { SanPham } from "@models/maintains/san-pham";
@Injectable({
  providedIn: 'root'
})
export class SanPhamService {
  apiUrl = environment.apiUrl + 'SanPham';
  constructor(private http: HttpClient) { }

  getDataPagination(pagination: PaginationParam, name?: string) {
    let params = new HttpParams().appendAll({ ...pagination, name })
    return this.http.get<PaginationResult<SanPham>>(`${this.apiUrl}/GetDataPagination`, { params });
  }

  create(model: SanPham) {
    return this.http.post<boolean>(`${this.apiUrl}/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}/Delete`, { params: { id: id } });
  }

  update(model: SanPham) {
    return this.http.put<boolean>(`${this.apiUrl}/Update`, model);
  }

  getAll() {
    return this.http.get<SanPham[]>(`${this.apiUrl}/GetAll`);
  }

  getAllByCommodityCodeId(id: number) {
    return this.http.get<SanPham[]>(`${this.apiUrl}/GetAllByCommodityCodeId`, { params: { id } });
  }
}
