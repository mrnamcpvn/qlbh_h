import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { CuaHang } from '../models/maintains/cua-hang';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';

@Injectable({
  providedIn: 'root'
})
export class CuaHangService {
  apiUrl = environment.apiUrl+'CuaHang';
  baseControler: string = '';
  constructor(private http: HttpClient) { }

  getDataPagination(pagination: PaginationParam, name?: string) {
    let params = new HttpParams().appendAll({ ...pagination, name })
    return this.http.get<PaginationResult<CuaHang>>(`${this.apiUrl}/GetDataPagination`, { params });
  }

  create(model: CuaHang) {
    return this.http.post<boolean>(`${this.apiUrl}/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}/Delete`, { params: { id: id } });
  }

  update(model: CuaHang) {
    return this.http.put<boolean>(`${this.apiUrl}/Update`, model);
  }

  getAll() {
    return this.http.get<CuaHang[]>(`${this.apiUrl}/GetAll`);
  }
}
