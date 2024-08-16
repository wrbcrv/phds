import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl: string = `${environment.apiUrl}/users`

  constructor(
    private http: HttpClient
  ) { }

  findAll(page: number, size: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}?page=${page}&size=${size}`, { withCredentials: true });
  }

  findByPartialName(partial: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/search/${partial}`, { withCredentials: true });
  }
}
