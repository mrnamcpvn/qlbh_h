import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MaHangRoutingModule } from './ma-hang-routing.module';
import { MainComponent } from './main/main.component';
import { ModalAddEditComponent } from './modal-add-edit/modal-add-edit.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    MainComponent,
    ModalAddEditComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ModalModule.forRoot(),
    PaginationModule.forRoot(),
    MaHangRoutingModule
  ]
})
export class MaHangModule { }
