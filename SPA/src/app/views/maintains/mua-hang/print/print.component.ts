import { Component, OnInit } from '@angular/core';
import { ToVietnameseService } from '@services/to-vietnamese.service'

@Component({
  selector: 'app-print',
  templateUrl: './print.component.html',
  styleUrls: ['./print.component.scss']
})
export class PrintComponent implements OnInit {

  tienChu: string = '';
  constructor(private toVNService: ToVietnameseService) { }

  ngOnInit() {
    this.tienChu = this.toVNService.toVietnamese(12345679)
    this.tienChu = this.tienChu.charAt(0).toUpperCase() + this.tienChu.slice(1);
  }

}
