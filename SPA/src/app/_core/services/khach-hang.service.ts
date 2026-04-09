import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { KhachHang } from '@models/maintains/khach-hang';
import { OperationResult } from '@utilities/operation-result';

@Injectable({
  providedIn: 'root'
})
export class KhachHangService {
  apiUrl = environment.apiUrl + 'KhachHang';
  baseControler: string = '';
  constructor(private http: HttpClient) { }

  getDataPagination(pagination: PaginationParam, name?: string) {
    let params = new HttpParams().appendAll({ ...pagination, name })
    return this.http.get<PaginationResult<KhachHang>>(`${this.apiUrl}/GetDataPagination`, { params });
  }

  create(model: KhachHang) {
    return this.http.post<OperationResult>(`${this.apiUrl}/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}/Delete`, { params: { id: id } });
  }

  update(model: KhachHang) {
    return this.http.put<OperationResult>(`${this.apiUrl}/Update`, model);
  }

  getAll() {
    return this.http.get<KhachHang[]>(`${this.apiUrl}/GetAll`);
  }
  template() {
    return this.http.get<OperationResult>(`${this.apiUrl}/Template`);
  }
  upload(file: FormData) {
    return this.http.post<OperationResult>(`${this.apiUrl}/Upload`, file)
  }
}
