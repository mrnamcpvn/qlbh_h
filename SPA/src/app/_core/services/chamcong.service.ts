import { Injectable } from '@angular/core';
import { environment } from "@env/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { ChamCong, ChamCongDTO } from '@models/maintains/cham-cong';
import { FunctionUtility } from '@utilities/function-utility';

@Injectable({
  providedIn: 'root'
})
export class ChamcongService {
  apiUrl = environment.apiUrl;
  constructor(private http: HttpClient, private functionUtility: FunctionUtility) { }

  getDataPagination(pagination: PaginationParam, fromDate: string | Date, toDate: string | Date) {
    let dateStart = this.functionUtility.getDateFormat(fromDate as Date);
    let dateEnd = this.functionUtility.getDateFormat(toDate as Date);
    let params = new HttpParams().appendAll({ ...pagination, 'fromDate': dateStart, 'toDate': dateEnd });
    return this.http.get<PaginationResult<ChamCongDTO>>(`${this.apiUrl}CalculateWages/GetDataPagination`, { params });
  }

  create(model: ChamCong) {
    return this.http.post<boolean>(`${this.apiUrl}CalculateWages/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}CalculateWages/Delete`, { params: { id: id } });
  }

  update(model: ChamCong) {
    return this.http.put<boolean>(`${this.apiUrl}CalculateWages/Update`, model);
  }

  getDetail(id: number) {
    return this.http.get<ChamCongDTO>(`${this.apiUrl}CalculateWages/GetDetail`, { params: { id } });
  }
}
