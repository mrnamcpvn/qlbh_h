import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../_core/services/auth/auth.service';
import { LocalStorageConstants } from '../../_core/constants/local-storage.constants';
import { UserForLogged } from '../../_core/models/auth/auth';
import { navItems } from '../../_nav';
@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html'
})
export class DefaultLayoutComponent implements OnInit {
  public sidebarMinimized = false;
  public navItems = navItems;
  user: UserForLogged = JSON.parse((localStorage.getItem(LocalStorageConstants.USER)));
  interval: any;
  constructor(private authService: AuthService,
  ) {
  }
  ngOnInit() {
    this.interval = setInterval(async () => {
      if (this.authService.isTokenExpired()) {
        clearInterval(this.interval);
        this.authService.logout();
      }
    }, 1000);
  }

  toggleMinimize(e) {
    this.sidebarMinimized = e;
  }

  logout() {
    this.authService.logout();
  }

  ngOnDestroy(): void {
    clearInterval(this.interval);
  }
}
