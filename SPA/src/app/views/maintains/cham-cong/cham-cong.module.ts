import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ChamCongRoutingModule } from './cham-cong-routing.module';
import { MainComponent } from  './main/main.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ModalModule} from "ngx-bootstrap/modal";
import {PaginationModule} from "ngx-bootstrap/pagination";
import {NgSelectModule} from "@ng-select/ng-select";
import {BsDatepickerModule} from "ngx-bootstrap/datepicker";
import { AddOrEditComponent } from './add-or-edit/add-or-edit.component';

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
    NgSelectModule,
    ReactiveFormsModule,
    BsDatepickerModule.forRoot(),
    ChamCongRoutingModule
  ]
})
export class ChamCongModule { }
