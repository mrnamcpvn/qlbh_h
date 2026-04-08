import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { FunctionUtility } from '@utilities/function-utility';
import { Report_Data, ReportMainParam } from '@models/reports/report-main';
import { OperationResult } from '@utilities/operation-result';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  apiUrl = environment.apiUrl + 'Report';
  constructor(private http: HttpClient, private functionUtility: FunctionUtility) { }

  getData(param: ReportMainParam) {
    let fromDate = this.functionUtility.getDateFormat(param.fromDate as Date);
    let toDate = this.functionUtility.getDateFormat(param.toDate as Date);
    let params = new HttpParams().appendAll({ 'FromDate': fromDate, 'ToDate': toDate, 'ID_SP': param.id_sp });
    return this.http.get<Report_Data>(`${this.apiUrl}/GetData`, { params });
  }
  excel(param: ReportMainParam) {
    let fromDate = this.functionUtility.getDateFormat(param.fromDate as Date);
    let toDate = this.functionUtility.getDateFormat(param.toDate as Date);
    let params = new HttpParams().appendAll({ 'FromDate': fromDate, 'ToDate': toDate, 'ID_SP': param.id_sp });
    return this.http.get<OperationResult>(`${this.apiUrl}/Excel`, { params })
  }
}
