import { Injectable } from '@angular/core';
import { environment } from "@env/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { BaoCaoThuChiData, BaoCaoThuChiParam } from '@models/maintains/bao-cao-thu-chi';
import { KeyValuePair } from '@utilities/key-value-pair';
import { OperationResult } from '@utilities/operation-result';
import { BehaviorSubject } from 'rxjs';
import { AppStateService } from './app-state.service';

@Injectable({
    providedIn: 'root'
})
export class BaoCaoThuChiService {
    apiUrl = environment.apiUrl + 'BaoCaoThuChi';

    private mainStateSource = new BehaviorSubject<any>(null);

    saveMainState(state: any) {
        this.mainStateSource.next(state);
    }

    getMainState(): any {
        return this.mainStateSource.getValue();
    }

    clearMainState() {
        this.mainStateSource.next(null);
    }

    constructor(private http: HttpClient, private appState: AppStateService) {
        this.appState.reset$.subscribe(() => this.clearMainState());
    }

    getData(param: BaoCaoThuChiParam) {
        let params = new HttpParams()
            .append('FromDate', param.fromDate)
            .append('ToDate', param.toDate);
        if (param.idSP?.length) param.idSP.forEach(id => params = params.append('IdSP', id));
        if (param.idKH?.length) param.idKH.forEach(id => params = params.append('IdKH', id));
        if (param.idNCC?.length) param.idNCC.forEach(id => params = params.append('IdNCC', id));
        if (param.idNV?.length) param.idNV.forEach(id => params = params.append('IdNV', id));
        if (param.loai != null && param.loai > 0) params = params.append('Loai', param.loai);
        return this.http.get<BaoCaoThuChiData>(`${this.apiUrl}/GetData`, { params });
    }

    excel(param: BaoCaoThuChiParam) {
        let params = new HttpParams()
            .append('FromDate', param.fromDate)
            .append('ToDate', param.toDate);
        if (param.idSP?.length) param.idSP.forEach(id => params = params.append('IdSP', id));
        if (param.idKH?.length) param.idKH.forEach(id => params = params.append('IdKH', id));
        if (param.idNCC?.length) param.idNCC.forEach(id => params = params.append('IdNCC', id));
        if (param.idNV?.length) param.idNV.forEach(id => params = params.append('IdNV', id));
        if (param.loai != null && param.loai > 0) params = params.append('Loai', param.loai);
        return this.http.get<OperationResult>(`${this.apiUrl}/Excel`, { params });
    }

    getListSanPham() {
        return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListSanPham`);
    }

    getListKhachHang() {
        return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListKhachHang`);
    }

    getListNhaCungCap() {
        return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListNhaCungCap`);
    }

    getListNhanVien() {
        return this.http.get<KeyValuePair[]>(`${this.apiUrl}/GetListNhanVien`);
    }
}
