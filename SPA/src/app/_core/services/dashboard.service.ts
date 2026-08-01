import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { DashboardSummary, OverdueDonHang } from '@models/maintains/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = environment.apiUrl + 'Dashboard';

  constructor(private http: HttpClient) {}

  getSummary(filterType: string = 'month') {
    const params = new HttpParams().set('filterType', filterType);
    return this.http.get<DashboardSummary>(`${this.apiUrl}/GetSummary`, { params });
  }

  getThuChiSummary(filterType: string = 'month') {
    const params = new HttpParams().set('filterType', filterType);
    return this.http.get<DashboardSummary>(`${this.apiUrl}/GetThuChiSummary`, { params });
  }

  getOverdueOrders() {
    return this.http.get<OverdueDonHang[]>(`${this.apiUrl}/GetOverdueOrders`);
  }
}
