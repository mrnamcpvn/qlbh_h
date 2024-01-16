import { Injectable } from '@angular/core';
import { environment } from "@env/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { CongDoan } from "@models/maintains/cong-doan";
@Injectable({
  providedIn: 'root'
})
export class CdService {
  apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getDataPagination(pagination: PaginationParam, name?: string) {
    let params = new HttpParams().appendAll({ ...pagination, name })
    return this.http.get<PaginationResult<CongDoan>>(`${this.apiUrl}StepInProcess/GetDataPagination`, { params });
  }

  create(model: CongDoan) {
    return this.http.post<boolean>(`${this.apiUrl}StepInProcess/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}StepInProcess/Delete`, { params: { id: id } });
  }

  update(model: CongDoan) {
    return this.http.put<boolean>(`${this.apiUrl}StepInProcess/Update`, model);
  }

  getAll() {
    return this.http.get<CongDoan[]>(`${this.apiUrl}StepInProcess/GetAll`);
  }

  getAllByCommodityCodeId(id: number) {
    return this.http.get<CongDoan[]>(`${this.apiUrl}StepInProcess/GetAllByCommodityCodeId`, { params: { id } });
  }
}
