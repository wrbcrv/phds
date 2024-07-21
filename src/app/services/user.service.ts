import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl: string = 'http://localhost:3000/api/users';

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
