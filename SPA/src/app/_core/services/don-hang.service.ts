import { Injectable } from '@angular/core';
import { environment } from "@env/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginationParam, PaginationResult } from '@utilities/pagination-utility';
import { OperationResult } from '@utilities/operation-result';
import { ChiTietDonHang, DonHang, DonHangDTO, DonHangFilter } from '@models/maintains/don-hang';
import { ThanhToan } from '@models/maintains/thanh-toan';
import { FunctionUtility } from '@utilities/function-utility';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DonHangService {
  apiUrl = environment.apiUrl+'DonHang';
  s_DonHang = new BehaviorSubject<DonHang>(null);
  current_DH = this.s_DonHang.asObservable();
  s_DonHangDTO = new BehaviorSubject<DonHangDTO>(null);
  current_DHDTO = this.s_DonHang.asObservable();
  constructor(private http: HttpClient, private functionUtility: FunctionUtility) { }

  getDataPagination(filter: DonHangFilter) {
    let dateStart = this.functionUtility.getDateFormat(filter.fromDate as Date);
    let dateEnd = this.functionUtility.getDateFormat(filter.toDate as Date);
    let params = new HttpParams()
      .append('Pagination.PageNumber', filter.pagination.pageNumber)
      .append('Pagination.PageSize', filter.pagination.pageSize)
      .append('fromDate', dateStart)
      .append('toDate', dateEnd)
      .append('loai', filter.loai);
    if (filter.soHoaDon) params = params.append('soHoaDon', filter.soHoaDon);
    if (filter.payType) params = params.append('payType', filter.payType);
    return this.http.get<PaginationResult<DonHang>>(`${this.apiUrl}/GetDonHangPagination`, { params });
  }

  excelExport(filter: DonHangFilter) {
    let dateStart = this.functionUtility.getDateFormat(filter.fromDate as Date);
    let dateEnd = this.functionUtility.getDateFormat(filter.toDate as Date);
    let params = new HttpParams()
      .append('fromDate', dateStart)
      .append('toDate', dateEnd)
      .append('loai', filter.loai);
    if (filter.soHoaDon) params = params.append('soHoaDon', filter.soHoaDon);
    if (filter.payType) params = params.append('payType', filter.payType);
    return this.http.get<OperationResult>(`${this.apiUrl}/ExcelExport`, { params });
  }

  create(model: DonHangDTO) {
    return this.http.post<DonHang>(`${this.apiUrl}/Create`, model);
  }

  delete(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}/Delete`, { params: { id: id } });
  }

  deleteItem(id: number) {
    return this.http.delete<boolean>(`${this.apiUrl}/DeleteItem`, { params: { id: id } });
  }

  update(model: DonHangDTO) {
    return this.http.put<DonHang>(`${this.apiUrl}/Update`, model);
  }

  getDetail(id: number) {
    return this.http.get<ChiTietDonHang[]>(`${this.apiUrl}/GetDetail`, { params: { id } });
  }

  changeStatus(model: DonHang) {
    return this.http.post<boolean>(`${this.apiUrl}/ChangeStatus`, model);
  }

  updatePayment(model: DonHang) {
    return this.http.post<boolean>(`${this.apiUrl}/UpdatePayment`, model);
  }

  createThanhToan(model: ThanhToan) {
    return this.http.post<ThanhToan>(`${this.apiUrl.replace('DonHang', 'ThanhToan')}/Create`, model);
  }

  getThanhToanByDonHang(idDh: number) {
    return this.http.get<ThanhToan[]>(`${this.apiUrl.replace('DonHang', 'ThanhToan')}/GetByDonHang`, { params: { idDh } });
  }

  changeSDonHang(model: DonHang) {
    this.s_DonHang.next(model);
  }
  changeSDonHangDTO(model: DonHangDTO) {
    this.s_DonHangDTO.next(model);
  }
}
