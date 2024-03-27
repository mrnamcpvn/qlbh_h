import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddOrEditComponent } from './add/add-or-edit.component';
import {MainComponent} from "./main/main.component";
import { ChiTietComponent } from "./chi-tiet/chi-tiet.component"
import { EditComponent } from './edit/edit.component';

const routes: Routes = [
  {
    path: '',
    component: MainComponent,
    data: {
      title: 'Đơn hàng'
    }
  },
  {
    path: 'add',
    component: AddOrEditComponent,
    data: {
      title: 'Thêm mới đơn hàng'
    }
  },
  {
    path: 'detail/:id',
    component: ChiTietComponent,
    data: {
      title: 'Chi tiết đơn hàng'
    }
  },
  {
    path: 'edit/:id',
    component: EditComponent,
    data: {
      title: 'Chỉnh sửa đơn hàng'
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MuaHangRoutingModule { }
