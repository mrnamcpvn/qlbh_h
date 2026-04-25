import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FileUploadComponent } from './file-upload-component/file-upload.component';
import { PrintComponent } from './print/print.component';

@NgModule({
  declarations: [FileUploadComponent, PrintComponent],
  exports: [FileUploadComponent, PrintComponent],
  imports: [
    CommonModule,
  ]
})
export class SharedModule { }
