import { Component, OnInit } from '@angular/core';
import { InjectBase } from "@utilities/inject-base-app";
import { IconButton } from '@constants/common.constants';
import { CuaHang } from '@models/maintains/cua-hang';
import { CuaHangService } from '@services/cua-hang.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent extends InjectBase implements OnInit {
  cuaHang: CuaHang = <CuaHang>{};
  iconButton = IconButton;

  constructor(private chService: CuaHangService) {
	super();
  }

  ngOnInit(): void {
	this.getData();
  }

  getData() {
	this.spinnerService.show();
	// Gọi API GetFirst được tối ưu
	this.chService.getFirst().subscribe({
	  next: (res) => {
		this.cuaHang = res || <CuaHang>{};
		this.spinnerService.hide();
	  },
	  error: () => {
		this.spinnerService.hide();
	  }
	});
  }

  save() {
	if (!this.cuaHang.ten) {
	  this.snotifyService.warning('Vui lòng nhập tên cửa hàng', 'Cảnh báo');
	  return;
	}

	this.spinnerService.show();
	// Sử dụng API Save chung để xử lý cả Create và Update
	this.chService.save(this.cuaHang).subscribe({
	  next: () => {
		this.snotifyService.success('Lưu thông tin cửa hàng thành công', 'Thành công');
		this.getData();
	  },
	  error: () => {
		this.snotifyService.error('Lưu thất bại', 'Lỗi');
		this.spinnerService.hide();
	  }
	});
  }

  clear() {
	this.getData();
  }

}
