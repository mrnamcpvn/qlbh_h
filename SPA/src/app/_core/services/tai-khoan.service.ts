import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { NguoiDung } from '@models/maintains/nguoi-dung';

@Injectable({
  providedIn: 'root'
})
export class TaiKhoanService {
  apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getDataPagination(pagination: PaginationParam, name?: string) {
    let params = new HttpParams().appendAll({ ...pagination, name })
    return this.http.get<PaginationResult<NguoiDung>>(`${this.apiUrl}User/GetDataPagination`, { params });
  }

  create(model: NguoiDung) {
    return this.http.post<boolean>(`${this.apiUrl}User/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}User/Delete`, { params: { id: id } });
  }

  update(model: NguoiDung) {
    return this.http.put<boolean>(`${this.apiUrl}User/Update`, model);
  }
}
