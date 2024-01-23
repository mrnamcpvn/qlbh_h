import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { KhachHang } from '@models/maintains/nld';

@Injectable({
  providedIn: 'root'
})
export class NldService {
  apiUrl = environment.apiUrl+'KhachHang';
  baseControler: string = '';
  constructor(private http: HttpClient) { }

  getDataPagination(pagination: PaginationParam, ten?: string) {
    let params = new HttpParams().appendAll({ ...pagination, ten })
    return this.http.get<PaginationResult<KhachHang>>(`${this.apiUrl}/GetDataPagination`, { params });
  }

  create(model: KhachHang) {
    return this.http.post<boolean>(`${this.apiUrl}/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}/Delete`, { params: { id: id } });
  }

  update(model: KhachHang) {
    return this.http.put<boolean>(`${this.apiUrl}/Update`, model);
  }

  getAll() {
    return this.http.get<KhachHang[]>(`${this.apiUrl}/GetAll`);
  }
}
