import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TaiKhoanRoutingModule } from './tai-khoan-routing.module';
import { MainComponent } from './main/main.component';
import { AddOrEditComponent } from './add-or-edit/add-or-edit.component';
import { FormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PaginationModule } from 'ngx-bootstrap/pagination';


@NgModule({
  declarations: [
    MainComponent,
    AddOrEditComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ModalModule.forRoot(),
    PaginationModule.forRoot(),
    TaiKhoanRoutingModule
  ]
})
export class TaiKhoanModule { }
