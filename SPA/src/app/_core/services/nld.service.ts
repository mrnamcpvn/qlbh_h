import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { NguoiLaoDong } from '@models/maintains/nld';

@Injectable({
  providedIn: 'root'
})
export class NldService {
  apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getDataPagination(pagination: PaginationParam, name?: string) {
    let params = new HttpParams().appendAll({ ...pagination, name })
    return this.http.get<PaginationResult<NguoiLaoDong>>(`${this.apiUrl}Employee/GetDataPagination`, { params });
  }

  create(name: string) {
    return this.http.post<boolean>(`${this.apiUrl}Employee/Create?name=` + name, null);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}Employee/Delete`, { params: { id: id } });
  }

  update(model: NguoiLaoDong) {
    return this.http.put<boolean>(`${this.apiUrl}Employee/Update`, model);
  }

  getAll() {
    return this.http.get<NguoiLaoDong[]>(`${this.apiUrl}Employee/GetAll`);
  }
}
