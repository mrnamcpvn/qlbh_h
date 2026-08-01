import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { trigger, transition, style, animate } from '@angular/animations';
import { AuthService } from '../../_core/services/auth/auth.service';
import { LocalStorageConstants } from '../../_core/constants/local-storage.constants';
import { UserForLogged } from '../../_core/models/auth/auth';
import { navItems } from '../../_nav';
import { DashboardService } from '@services/dashboard.service';
import { SignalRService } from '@services/signalr.service';
import { OverdueDonHang } from '@models/maintains/dashboard.model';
@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
  styleUrls: ['./default-layout.component.scss'],
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
export class DefaultLayoutComponent implements OnInit {
  public sidebarMinimized = false;
  public navItems = navItems;
  user: UserForLogged = JSON.parse((localStorage.getItem(LocalStorageConstants.USER)));
  interval: any;
  donQuaHan: OverdueDonHang[] = [];

  @ViewChild('notifContainer', { static: false }) notifContainer!: ElementRef;
  notifExpanded = false;
  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService,
    private signalRService: SignalRService,
  ) {
  }
  ngOnInit() {
    this.interval = setInterval(async () => {
      if (this.authService.isTokenExpired()) {
        clearInterval(this.interval);
        this.authService.logout();
      }
    }, 1000);
    this.loadOverdueOrders();
    this.signalRService.orderChanged.subscribe(() => this.loadOverdueOrders());
  }
  loadOverdueOrders() {
    this.dashboardService.getOverdueOrders().subscribe({
      next: res => {
        this.donQuaHan = res || [];
      },
      error: () => {}
    });
  }

  toggleMinimize(e) {
    this.sidebarMinimized = e;
  }

  logout() {
    this.authService.logout();
  }

  goToOrder(order: OverdueDonHang) {
    const url = `/#/maintain/ban-hang?search=${order.ma_DH}`;
    window.open(url, '_blank');
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.notifExpanded && this.notifContainer && !this.notifContainer.nativeElement.contains(event.target)) {
      this.notifExpanded = false;
    }
  }

  get overdueCount(): number {
    return this.donQuaHan.filter(d => d.isOverdue).length || 0;
  }

  get upcomingCount(): number {
    return this.donQuaHan.filter(d => !d.isOverdue).length || 0;
  }

  ngOnDestroy(): void {
    clearInterval(this.interval);
  }
}
