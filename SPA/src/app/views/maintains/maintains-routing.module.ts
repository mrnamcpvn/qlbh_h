import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'nguoi-dung',
    loadChildren: () => import('./tai-khoan/tai-khoan.module').then(m => m.TaiKhoanModule)
  },
  {
    path: 'nld',
    loadChildren: () => import('./nld/nld.module').then(m => m.NldModule)
  },
  {
    path: 'cd',
    loadChildren: () => import('./cd/cd.module').then(m => m.CdModule)
  },
  {
    path: 'cham-cong',
    loadChildren: () => import('./cham-cong/cham-cong.module').then(m => m.ChamCongModule)
  },
  {
    path: 'ma-hang',
    loadChildren: () => import('./ma-hang/ma-hang.module').then(m => m.MaHangModule)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MaintainsRoutingModule {}
