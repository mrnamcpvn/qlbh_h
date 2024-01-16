import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import {JwtHelperService} from '@auth0/angular-jwt';
import {UserForLogged, UserLoginParam} from '../../models/auth/auth';
import {HttpClient} from '@angular/common/http';
import { Router } from '@angular/router';
import {map} from 'rxjs/operators';
import {LocalStorageConstants} from '../../constants/local-storage.constants';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiUrl = environment.apiUrl;
  jwtHelper = new JwtHelperService();
  currentUser: UserForLogged | null = <UserForLogged>{};
  decodedToken: any;
  constructor(private  http: HttpClient,
              private  router: Router) { }
  login(param: UserLoginParam) {
    return this.http.post(this.apiUrl + 'c_auth/login', param).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem(LocalStorageConstants.TOKEN, user.token);
          localStorage.setItem(LocalStorageConstants.USER, JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
        }
      })
    );
  }

  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }

  loggedIn() {
    const token: string = localStorage.getItem(LocalStorageConstants.TOKEN);
    const user: UserForLogged = JSON.parse(localStorage.getItem(LocalStorageConstants.USER));
    return !(!user) || !this.jwtHelper.isTokenExpired(token);
  }

  isTokenExpired() {
    const token: string = localStorage.getItem(LocalStorageConstants.TOKEN);
    return this.jwtHelper.isTokenExpired(token);
  }
}
