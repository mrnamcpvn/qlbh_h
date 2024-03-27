import { Injectable } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {
  constructor(private authService: AuthService, private router: Router) { }
  canActivate(): boolean {
    // let date = new Date();
    // if(date >= new Date("2024/04/02"))
    //   return false;
    if (this.authService.loggedIn()) {
      return true;
    }
    this.router.navigate(["/login"]);
    return false;
  }
}
