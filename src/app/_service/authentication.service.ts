import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  getUsername(): string | null {
    return localStorage.getItem('username');
  }

  getUserRole(): string | null {
    return localStorage.getItem('userrole');
  }
}