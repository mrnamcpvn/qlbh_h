import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subject } from 'rxjs';

import { IconSetService } from '@coreui/icons-angular';
import { freeSet } from '@coreui/icons';
import { SignalRService } from '@services/signalr.service';

@Component({
  // tslint:disable-next-line
  selector: 'body',
  template: `
    <router-outlet></router-outlet>
    <ng-snotify></ng-snotify>
    <ngx-spinner bdColor="rgba(51,51,51,0.8)" size="medium" color="#fff" type="ball-scale-multiple"></ngx-spinner>
  `,
  providers: [IconSetService],
})
export class AppComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  constructor(
    private router: Router,
    public iconSet: IconSetService,
    private signalRService: SignalRService,
  ) {
    // iconSet singleton
    iconSet.icons = { ...freeSet };
  }

  ngOnInit() {
    this.signalRService.start();
    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
      window.scrollTo(0, 0);
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.signalRService.stop();
  }
}
