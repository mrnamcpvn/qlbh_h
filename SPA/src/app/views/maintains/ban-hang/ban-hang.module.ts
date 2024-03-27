import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BanHangRoutingModule } from './ban-hang-routing.module';
import { MainComponent } from  './main/main.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ModalModule} from "ngx-bootstrap/modal";
import {PaginationModule} from "ngx-bootstrap/pagination";
import {NgSelectModule} from "@ng-select/ng-select";
import {NgxPrintModule} from 'ngx-print';
import {BsDatepickerModule} from "ngx-bootstrap/datepicker";
import { AddOrEditComponent } from './add-or-edit/add-or-edit.component';
import { ChiTietComponent } from './chi-tiet/chi-tiet.component';
import { EditComponent } from '../ban-hang/edit/edit.component';


@NgModule({
  declarations: [
    MainComponent,
    AddOrEditComponent,
    ChiTietComponent,
    EditComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ModalModule.forRoot(),
    PaginationModule.forRoot(),
    NgSelectModule,
    NgxPrintModule,
    ReactiveFormsModule,
    BsDatepickerModule.forRoot(),

    BanHangRoutingModule
  ]
})
export class BangHangModule { }
