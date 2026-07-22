import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ModalModule } from "ngx-bootstrap/modal";
import { PaginationModule } from "ngx-bootstrap/pagination";
import { NgSelectModule } from "@ng-select/ng-select";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";
import { BaoCaoThuChiModuleRoutingModule } from './bao-cao-thu-chi-routing.module';
import { MainComponent } from './main/main.component';

@NgModule({
    declarations: [
        MainComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ModalModule.forRoot(),
        PaginationModule.forRoot(),
        NgSelectModule,
        ReactiveFormsModule,
        BsDatepickerModule.forRoot(),
        BaoCaoThuChiModuleRoutingModule
    ]
})
export class BaoCaoThuChiModule { }
