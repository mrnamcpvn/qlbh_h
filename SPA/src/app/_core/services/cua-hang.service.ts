import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';
import { CuaHang } from '../models/maintains/cua-hang';

@Injectable({
  providedIn: 'root'
})
export class CuaHangService {
  apiUrl = environment.apiUrl+'CuaHang';
  baseControler: string = '';
  constructor(private http: HttpClient) { }

//   getDataPagination(pagination: PaginationParam, ten?: string) {
//     let params = new HttpParams().appendAll({ ...pagination, ten })
//     return this.http.get<PaginationResult<NhanVien>>(`${this.apiUrl}/GetDataPagination`, { params });
//   }

//   create(model: NhanVien) {
//     return this.http.post<boolean>(`${this.apiUrl}/Create`, model);
//   }

//   delete(id: number) {
//     return this.http.delete<boolean>(`${this.apiUrl}/Delete`, { params: { id: id } });
//   }

//   update(model: NhanVien) {
//     return this.http.put<boolean>(`${this.apiUrl}/Update`, model);
//   }

  getAll() {
    return this.http.get<CuaHang[]>(`${this.apiUrl}/GetAll`);
  }
}
