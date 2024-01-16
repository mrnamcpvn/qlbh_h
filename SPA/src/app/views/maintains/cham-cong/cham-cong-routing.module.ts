import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddOrEditComponent } from './add-or-edit/add-or-edit.component';
import {MainComponent} from "./main/main.component";

const routes: Routes = [
  {
    path: '',
    component: MainComponent,
    data: {
      title: 'Chấm công'
    }
  },
  {
    path: 'add',
    component: AddOrEditComponent,
    data: {
      title: 'Chấm công'
    }
  },
  {
    path: 'edit/:id',
    component: AddOrEditComponent,
    data: {
      title: 'Chấm công'
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ChamCongRoutingModule { }
