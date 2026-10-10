import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { IAuthToken } from '../types/auth';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  http = inject(HttpClient);
  constructor() {}

  router = inject(Router);
  login(email: string, password: string) {
    return this.http.post<IAuthToken>(environment.apiUrl + '/api/Auth/login', {
      email: email,
      password: password,
    });
  }

  saveToken(authToken: IAuthToken) {
    localStorage.setItem('auth', JSON.stringify(authToken));
    localStorage.setItem('token', authToken.token);
  }

  logout() {
    localStorage.removeItem('auth');
    localStorage.removeItem('token');
    this.router.navigateByUrl('/login');
  }
  get isLoggedIn() {
    return localStorage.getItem('token') ? true : false;
  }
  get isEmployee() {
    if (!this.isLoggedIn) return false;
    let token = JSON.parse(localStorage.getItem('auth')!);
    if (token.role == 'Employee') {
      return true;
    } else {
      return false;
    }
  }
  get authDetail(): IAuthToken | null {
    if (!this.isLoggedIn) return null;
    let token: IAuthToken = JSON.parse(localStorage.getItem('auth')!);
    return token;
  }

  getProfile(){
    return this.http.get(environment.apiUrl+"/api/Auth/profile")
  }

  updateProfile(profile:any){
    return this.http.post(environment.apiUrl+"/api/Auth/profile",profile)
  }
}
