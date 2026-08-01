import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { trigger, transition, style, animate } from '@angular/animations';
import { DashboardSummary, DashboardDayStat, OverdueDonHang, CongNoCustomerSummary } from '@models/maintains/dashboard.model';
import { DashboardService } from '@services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  animations: [
    trigger('panelAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.3)', transformOrigin: 'bottom right' }),
        animate('300ms cubic-bezier(0.34, 1.56, 0.64, 1)', style({ opacity: 1, transform: 'scale(1)' }))
      ]),
      transition(':leave', [
        style({ transformOrigin: 'bottom right' }),
        animate('220ms ease-in', style({ opacity: 0, transform: 'scale(0.3)' }))
      ])
    ]),
    trigger('badgeAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.3)' }),
        animate('250ms cubic-bezier(0.175, 0.885, 0.32, 1.275)', style({ opacity: 1, transform: 'scale(1)' }))
      ])
    ])
  ]
})
export class DashboardComponent implements OnInit {

  @ViewChild('notifContainer', { static: false }) notifContainer!: ElementRef;

  notifExpanded = false;
  selectedFilter: string = 'month';

  filterOptions = [
    { value: 'day', label: 'Ngày' },
    { value: 'week', label: 'Tuần' },
    { value: 'month', label: 'Tháng' },
    { value: 'quarter', label: 'Quý' },
    { value: 'year', label: 'Năm' }
  ];

  summary: DashboardSummary = {
    filterType: 'month',
    filterPeriodName: 'Tháng này',
    prevPeriodName: 'Tháng trước',
    tongThuKy: 0,
    tongChiKy: 0,
    tongThuKyTruoc: 0,
    tongChiKyTruoc: 0,
    tongThuThang: 0,
    tongChiThang: 0,
    tongDuNo: 0,
    soDonQuaHan: 0,
    soDonChuaThanhToan: 0,
    tongThuThangTruoc: 0,
    tongChiThangTruoc: 0,
    donQuaHan: [],
    topCongNoKhachHang: [],
    revenueByDay: []
  };

  isLoading = true;

  // Chart
  chartLabels: string[] = [];
  chartDatasets: any[] = [];
  chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      xAxes: [{ gridLines: { display: false } }],
      yAxes: [{ ticks: { callback: (v: any) => this.formatCurrency(v) }, gridLines: { color: 'rgba(0,0,0,0.05)' } }]
    },
    legend: { display: true, position: 'top' },
    tooltips: {
      callbacks: {
        label: (item: any, data: any) => {
          const ds = data.datasets[item.datasetIndex];
          return `${ds.label}: ${this.formatCurrency(item.yLabel)}`;
        }
      }
    }
  };
  chartColors = [
    { backgroundColor: 'rgba(52, 199, 89, 0.7)', borderColor: '#34C759', pointBackgroundColor: '#34C759' },
    { backgroundColor: 'rgba(255, 69, 58, 0.5)', borderColor: '#FF453A', pointBackgroundColor: '#FF453A' }
  ];
  chartType = 'bar';

  constructor(
    private dashboardService: DashboardService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  setFilter(filterValue: string) {
    if (this.selectedFilter === filterValue) return;
    this.selectedFilter = filterValue;
    this.loadThuChi();
  }

  loadData() {
    this.isLoading = true;
    this.dashboardService.getSummary(this.selectedFilter).subscribe({
      next: res => {
        this.summary = res;
        this.buildChart(res.revenueByDay || []);
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; }
    });
  }

  loadThuChi() {
    this.dashboardService.getThuChiSummary(this.selectedFilter).subscribe({
      next: res => {
        this.summary.filterType = res.filterType;
        this.summary.filterPeriodName = res.filterPeriodName;
        this.summary.prevPeriodName = res.prevPeriodName;
        this.summary.tongThuKy = res.tongThuKy;
        this.summary.tongChiKy = res.tongChiKy;
        this.summary.tongThuKyTruoc = res.tongThuKyTruoc;
        this.summary.tongChiKyTruoc = res.tongChiKyTruoc;
        this.summary.revenueByDay = res.revenueByDay || [];
        this.buildChart(res.revenueByDay || []);
      },
      error: () => {}
    });
  }

  private buildChart(data: DashboardDayStat[]) {
    this.chartLabels = data.map(x => x.label);
    this.chartDatasets = [
      {
        label: 'Thu (đ)',
        data: data.map(x => x.tongThu),
        type: 'bar'
      },
      {
        label: 'Chi (đ)',
        data: data.map(x => x.tongChi),
        type: 'bar'
      }
    ];
  }

  getGrowthPct(current: number, prev: number): number {
    if (!prev) return current > 0 ? 100 : 0;
    return Math.round(((current - prev) / prev) * 100);
  }
  goToOrder(order: OverdueDonHang) {
    const url = `/#/maintain/ban-hang?search=${order.ma_DH}`;
    window.open(url, '_blank');
  }

  goToBanHang() {
    this.router.navigate(['/maintain/ban-hang']);
  }

  goToCongNo() {
    this.router.navigate(['/maintain/cong-no']);
  }

  goToKhachHangCongNo(item: CongNoCustomerSummary) {
    const url = `/#/maintain/cong-no?khachHangID=${item.khachHangID}&tenKH=${encodeURIComponent(item.tenKhachHang || '')}`;
    window.open(url, '_blank');
  }

  formatCurrency(value: number): string {
    if (!value) return '0';
    if (value >= 1_000_000_000) return (value / 1_000_000_000).toFixed(1) + 'B';
    if (value >= 1_000_000) return (value / 1_000_000).toFixed(1) + 'M';
    if (value >= 1_000) return (value / 1_000).toFixed(0) + 'K';
    return value.toString();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.notifExpanded && this.notifContainer && !this.notifContainer.nativeElement.contains(event.target)) {
      this.notifExpanded = false;
    }
  }

  get loiNhuan(): number {
    return (this.summary?.tongThuKy || 0) - (this.summary?.tongChiKy || 0);
  }

  get overdueCount(): number {
    return this.summary?.donQuaHan?.filter(d => d.isOverdue).length || 0;
  }

  get upcomingCount(): number {
    return this.summary?.donQuaHan?.filter(d => !d.isOverdue).length || 0;
  }
}
