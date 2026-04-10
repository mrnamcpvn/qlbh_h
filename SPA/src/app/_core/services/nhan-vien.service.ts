import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { NhanVien } from '@models/maintains/nhan-vien';

@Injectable({
  providedIn: 'root'
})
export class NhanVienService {
  apiUrl = environment.apiUrl+'NhanVien';
  baseControler: string = '';
  constructor(private http: HttpClient) { }

  getDataPagination(pagination: PaginationParam, name?: string) {
    let params = new HttpParams().appendAll({ ...pagination, name })
    return this.http.get<PaginationResult<NhanVien>>(`${this.apiUrl}/GetDataPagination`, { params });
  }

  create(model: NhanVien) {
    return this.http.post<boolean>(`${this.apiUrl}/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}/Delete`, { params: { id: id } });
  }

  update(model: NhanVien) {
    return this.http.put<boolean>(`${this.apiUrl}/Update`, model);
  }

  getAll() {
    return this.http.get<NhanVien[]>(`${this.apiUrl}/GetAll`);
  }
}
