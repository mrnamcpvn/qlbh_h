import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FileUploadComponent } from './file-upload-component/file-upload.component';

@NgModule({
  declarations: [FileUploadComponent],
  exports: [FileUploadComponent],
  imports: [
    CommonModule,
  ]
})
export class SharedModule { }
