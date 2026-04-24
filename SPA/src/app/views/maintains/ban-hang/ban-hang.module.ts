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
import { FormComponent } from './form/form.component';
import { ChiTietComponent } from './chi-tiet/chi-tiet.component';
import { ModelThanhToanComponent } from './model-thanh-toan/model-thanh-toan.component';


@NgModule({
  declarations: [
		MainComponent,
    FormComponent,
    ChiTietComponent,
		ModelThanhToanComponent
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
