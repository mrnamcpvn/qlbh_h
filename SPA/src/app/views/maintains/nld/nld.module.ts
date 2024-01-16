import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NldRoutingModule } from './nld-routing.module';
import { FormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { MainComponent } from './main/main.component';
import { ModalAddEditComponent } from './modal-add-edit/modal-add-edit.component';


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
    NldRoutingModule
  ]
})
export class NldModule { }
