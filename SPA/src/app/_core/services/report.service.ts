import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { Pagination, PaginationResult } from '@utilities/pagination-utility';
import { FunctionUtility } from '@utilities/function-utility';
import { Report, ReportMainParam } from '@models/reports/report-main';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  apiUrl = environment.apiUrl + 'Report';
  constructor(private http: HttpClient, private functionUtility: FunctionUtility) { }

  getDatapagination(pagination: Pagination, param: ReportMainParam) {
    let fromDate = this.functionUtility.getDateFormat(param.fromDate as Date);
    let toDate = this.functionUtility.getDateFormat(param.toDate as Date);
    let params = new HttpParams().appendAll({ ...pagination, 'FromDate': fromDate, 'ToDate': toDate, 'ID_NLD': param.id_NLD });
    return this.http.get<PaginationResult<Report>>(`${this.apiUrl}/GetDataPagination`, { params });
  }

  exportExcel(pagination: Pagination, param: ReportMainParam) {
    let fromDate = this.functionUtility.getDateFormat(param.fromDate as Date);
    let toDate = this.functionUtility.getDateFormat(param.toDate as Date);
    let params = new HttpParams().appendAll({ ...pagination, 'FromDate': fromDate, 'ToDate': toDate, 'ID_NLD': param.id_NLD });
    return this.http.get(`${this.apiUrl}/ExportExcel`, { params, responseType: 'blob' });
  }
}
