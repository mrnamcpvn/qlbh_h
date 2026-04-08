import { Component, inject, Input, Output, EventEmitter } from '@angular/core';
import { ClassButton, IconButton } from '@constants/common.constants';
import { InjectBase } from '@utilities/inject-base-app';
import { FileSizePipe } from 'src/app/_core/pipes/file-size/file-size.pipe';
export interface FileResultModel {
  fileModel: FileModel[]
  formData: FormData
}
export interface FileModel {
  id: number;
  name: string;
  content?: string;
  file: File;
  size: number;
}
export interface UploadSetting {
  multiple: boolean,
  hidden: boolean,
  disabled: boolean,
  maxSize: number,
  allowExtensions: string,
  label: string,
}
@Component({
  selector: 'file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss'],
  providers: [FileSizePipe]
})
export class FileUploadComponent extends InjectBase {
  defaultSetting: UploadSetting = <UploadSetting>{
    multiple: false,
    hidden: false,
    disabled: false,
    maxSize: 30000000,
    allowExtensions: null,
    label: 'Upload'
  }
  fileSizePipe = inject(FileSizePipe)

  @Input() multiple: boolean = this.defaultSetting.multiple;
  @Input() hidden: boolean = this.defaultSetting.hidden;
  @Input() disabled: boolean = this.defaultSetting.disabled;
  @Input() maxSize: number = this.defaultSetting.maxSize;
  @Input() allowExtensions: string = this.defaultSetting.allowExtensions;
  @Input() label: string = this.defaultSetting.label;

  @Output() filesChange = new EventEmitter<FileResultModel>();

  set _files(id: FileResultModel) { this.filesChange.emit(id) }

  iconButton = IconButton;
  classButton = ClassButton;

  constructor() { super(); }

  async checkFiles(event: any) {
    let result: FileResultModel = <FileResultModel>{ fileModel: [], formData: new FormData() }
    try {
      const files = Array.from(event.target.files as File[])
      if (files.length > 0)
        this.multiple ? await this.callMultiple(files, result) : await this.callSingle(files, result)
      if (result.fileModel.length > 0) {
        this.calculateId(result.fileModel)
        this._files = result
      }
    } catch (error) {
      this.snotifyService.error(
        `Error reading file: ${error}`,
        'Lỗi'
      );
    } finally {
      event.target.value = ''
    }
  }
  private async callMultiple(files: File[], result: FileResultModel) {
    if (this.allowExtensions) {
      const notAllowFiles = files.filter(x => !this.allowExtensions.includes(x.name.split('.').pop().toLowerCase()))
      if (notAllowFiles.length > 0)
        return this.snotifyService.warning(
          'Chỉ cho phép tệp excel',
          'Lỗi'
        );
    }
    const overSizeFiles = files.filter(x => x.size > this.maxSize).map(x => x.name)
    if (overSizeFiles.length > 0)
      return this.snotifyService.error(
        `Kích thước File phải bằng ${this.fileSizePipe.transform(this.maxSize, 0)} hoặc nhỏ hơn : \n${overSizeFiles.join('\n')}`,
        'Lỗi'
      );
    for (let i = 0; i < files.length; i++)
      result.formData.append('files[]', files[i]);
    await Promise.all(files.map(async (file) => {
      const dataUrl = await this.readAsDataURLAsync(file);
      const newFile: FileModel = {
        id: 0,
        name: file.name,
        size: file.size,
        content: dataUrl as string,
        file: file
      };
      result.fileModel.push(newFile);
    }))
  }

  private async callSingle(files: File[], result: FileResultModel) {
    const file = files[0]
    if (this.allowExtensions && !this.allowExtensions.includes(file.name.split('.').pop().toLowerCase()))
      return this.snotifyService.warning(
        'Chỉ cho phép tệp excel',
        'Lỗi'
      );
    if (file.size > this.maxSize)
      return this.snotifyService.error(
        `Kích thước File phải bằng ${this.fileSizePipe.transform(this.maxSize, 0)} hoặc nhỏ hơn : \n${file.name}`,
        'Lỗi'
      );
    result.formData.append('file', file);
    const dataUrl = await this.readAsDataURLAsync(file);
    const newFile: FileModel = {
      id: 0,
      name: file.name,
      size: file.size,
      content: dataUrl as string,
      file: file
    };
    result.fileModel.push(newFile);
  }

  private readAsDataURLAsync = (file: File) => new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = (event) => resolve(event.target.result);
    reader.onerror = (error) => reject(error);
    reader.readAsDataURL(file);
  });

  private calculateId = (file_List: FileModel[]) => file_List.map((val, ind) => val.id = ind + 1)
}
