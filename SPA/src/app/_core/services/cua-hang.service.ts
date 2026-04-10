import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';
import { CuaHang } from '../models/maintains/cua-hang';

@Injectable({
  providedIn: 'root'
})
export class CuaHangService {
  apiUrl = environment.apiUrl+'CuaHang';
  constructor(private http: HttpClient) { }

  getFirst() {
    return this.http.get<CuaHang>(`${this.apiUrl}/GetFirst`);
  }

  save(model: CuaHang) {
    return this.http.post<boolean>(`${this.apiUrl}/Save`, model);
  }
}
