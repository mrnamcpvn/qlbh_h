import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MaintainsRoutingModule } from './maintains-routing.module';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { ModalModule } from "ngx-bootstrap/modal";
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from "ngx-bootstrap/pagination";


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MaintainsRoutingModule,
    FormsModule,
    NgSelectModule,
    ModalModule.forRoot(),
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
  ]
})
export class MaintainsModule { }
