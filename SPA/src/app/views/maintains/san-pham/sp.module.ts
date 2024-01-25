import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SanPhamRoutingModule } from './sp-routing.module';
import { FormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { MainComponent } from "./main/main.component";
import { ModalAddEditComponent } from "./modal-add-edit/modal-add-edit.component";
import { NgSelectModule } from '@ng-select/ng-select';

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
    NgSelectModule,
    SanPhamRoutingModule
  ]
})
export class SanPhamModule { }
