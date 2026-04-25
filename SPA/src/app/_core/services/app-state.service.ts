import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppStateService {
  private resetSource = new Subject<void>();
  reset$ = this.resetSource.asObservable();

  resetAll() {
    this.resetSource.next();
  }
}
