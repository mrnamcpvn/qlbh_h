import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { MaHang } from '@models/maintains/ma-hang';

@Injectable({
  providedIn: 'root'
})
export class MaHangService {
  constructor(private http: HttpClient) { }
  apiUrl = environment.apiUrl;

  getDataPagination(pagination: PaginationParam, name?: string) {
    let params = new HttpParams().appendAll({ ...pagination, name })
    return this.http.get<PaginationResult<MaHang>>(`${this.apiUrl}CommodityCode/GetDataPagination`, { params });
  }

  create(name: string) {
    return this.http.post<boolean>(`${this.apiUrl}CommodityCode/Create?name=` + name, null);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}CommodityCode/Delete`, { params: { id: id } });
  }

  update(model: MaHang) {
    return this.http.put<boolean>(`${this.apiUrl}CommodityCode/Update`, model);
  }

  getAll() {
    return this.http.get<MaHang[]>(`${this.apiUrl}CommodityCode/GetAll`);
  }
}
