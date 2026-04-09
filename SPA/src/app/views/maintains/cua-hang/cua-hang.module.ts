import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CuaHangRoutingModule } from './cua-hang-routing.module';
import { MainComponent } from './main/main.component';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
	declarations: [
		MainComponent
	],
	imports: [
		CommonModule,
		CuaHangRoutingModule,
		FormsModule,
		ModalModule.forRoot(),
		PaginationModule.forRoot()
	]
})
export class CuaHangModule { }
