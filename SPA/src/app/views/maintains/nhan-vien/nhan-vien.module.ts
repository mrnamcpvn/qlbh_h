import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NhanVienRoutingModule } from './nhan-vien-routing.module';
import { MainComponent } from './main/main.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { FormsModule } from '@angular/forms';
import { ModalAddEditComponent } from './modal-add-edit/modal-add-edit.component';

@NgModule({
	declarations: [
		MainComponent, ModalAddEditComponent
	],
	imports: [
		CommonModule,
		NhanVienRoutingModule,CommonModule,
		FormsModule,
		ModalModule.forRoot(),
		PaginationModule.forRoot()
	]
})
export class NhanVienModule { }