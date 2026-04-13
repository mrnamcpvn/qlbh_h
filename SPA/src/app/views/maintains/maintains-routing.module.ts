import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'nguoi-dung',
    loadChildren: () => import('./tai-khoan/tai-khoan.module').then(m => m.TaiKhoanModule)
  },
  {
    path: 'khach-hang',
    loadChildren: () => import('./khach-hang/kh.module').then(m => m.KhachHangModule)
  },
  {
    path: 'nha-cung-cap',
    loadChildren: () => import('./nha-cung-cap/ncc.module').then(m => m.NhaCungCapModule)
  },
  {
    path: 'san-pham',
    loadChildren: () => import('./san-pham/sp.module').then(m => m.SanPhamModule)
  },
  {
    path: 'mua-hang',
    loadChildren: () => import('./mua-hang/mua-hang.module').then(m => m.MuaHangModule)
  },
  {
    path: 'ban-hang',
    loadChildren: () => import('./ban-hang/ban-hang.module').then(m => m.BangHangModule)
  },
  {
    path: 'ma-hang',
    loadChildren: () => import('./ma-hang/ma-hang.module').then(m => m.MaHangModule)
  },
  {
    path: 'theo-doi-nhan-vien-ban-hang',
    loadChildren: () => import('./theo-doi-nhan-vien-ban-hang/theo-doi-nhan-vien-ban-hang.module').then(m => m.TheoDoiNhanVienBanHangModule)
  },
  {
    path: 'nhan-vien',
    loadChildren: () => import('./nhan-vien/nhan-vien.module').then(m => m.NhanVienModule)
  },
  {
    path: 'cua-hang',
    loadChildren: () => import('./cua-hang/cua-hang.module').then(m => m.CuaHangModule)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MaintainsRoutingModule { }
