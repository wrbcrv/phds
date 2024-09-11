import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl: string = `${environment.apiUrl}/auth`;
  private userSubject = new BehaviorSubject<any>(null);
  public user$ = this.userSubject.asObservable();

  constructor(
    private http: HttpClient
  ) { }

  login(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { username, password }, {
      withCredentials: true
    }).pipe(
      tap(user => this.userSubject.next(user))
    );
  }

  logout(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/logout`, {}).pipe(
      tap(() => this.userSubject.next(null))
    );
  }

  getUserInfo(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/me`, {
      withCredentials: true
    }).pipe(
      tap(user => this.userSubject.next(user))
    );
  }

  isAdmin(): boolean {
    const user = this.userSubject.value;
    return user?.role === 'Administrator';
  }

  isAgent(): boolean {
    const user = this.userSubject.value;
    return user?.role === 'Agent';
  }

  isClient(): boolean {
    const user = this.userSubject.value;
    return user?.role === 'Client';
  }
}
