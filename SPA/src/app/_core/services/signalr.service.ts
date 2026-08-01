import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '@env/environment';
import { Subject } from 'rxjs';

export interface OrderChangedNotification {
  action: string;
  id: number;
  ma_DH: string;
  loai: number;
  tongTien: number;
  ten_KH: string;
  ten_NCC: string;
  date: string;
  soNgayCongNo: number;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection;
  private orderChanged$ = new Subject<OrderChangedNotification>();
  orderChanged = this.orderChanged$.asObservable();

  start() {
    if (this.hubConnection) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.baseUrl.replace(/\/$/, '')}/hubs/notifications`)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection.on('OrderChanged', (data: OrderChangedNotification) => {
      this.orderChanged$.next(data);
    });

    this.hubConnection
      .start()
      .catch(err => console.error('SignalR connection error:', err));
  }

  stop() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch(err => console.error('SignalR stop error:', err));
    }
  }
}
